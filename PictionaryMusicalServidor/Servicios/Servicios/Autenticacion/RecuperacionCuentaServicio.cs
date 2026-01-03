using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using Datos.Modelo;
using log4net;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Datos.Excepciones;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Servicio interno para la logica de negocio de recuperacion y cambio de contrasena.
    /// Maneja el almacenamiento temporal de solicitudes, generacion y validacion de codigos.
    /// Delega el acceso a datos a los repositorios correspondientes.
    /// </summary>
    public class RecuperacionCuentaServicio : IRecuperacionCuentaServicio
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(RecuperacionCuentaServicio));

        private const int MinutosExpiracionCodigo = 5;

        private static readonly ConcurrentDictionary<string, SolicitudRecuperacionPendiente>
            _solicitudesRecuperacion =
            new ConcurrentDictionary<string, SolicitudRecuperacionPendiente>();

        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;
        private readonly INotificacionCodigosServicio _notificacionServicio;

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        /// <param name="notificacionServicio">Servicio de notificacion de codigos.</param>
        public RecuperacionCuentaServicio(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria,
            INotificacionCodigosServicio notificacionServicio)
        {
            _contextoFactoria = contextoFactoria ??
                throw new ArgumentNullException(nameof(contextoFactoria));

            _repositorioFactoria = repositorioFactoria ??
                throw new ArgumentNullException(nameof(repositorioFactoria));

            _notificacionServicio = notificacionServicio ??
                throw new ArgumentNullException(nameof(notificacionServicio));
        }

        /// <summary>
        /// Constructor por defecto para uso en WCF (compatibilidad hacia atras).
        /// </summary>
        public RecuperacionCuentaServicio(
            IContextoFactoria contextoFactoria,
            INotificacionCodigosServicio notificacionServicio) 
            : this(contextoFactoria, new RepositorioFactoria(), notificacionServicio)
        {
        }

        /// <summary>
        /// Solicita un codigo de recuperacion para una cuenta de usuario.
        /// </summary>
        public ResultadoSolicitudRecuperacionDTO SolicitarCodigoRecuperacion(
            SolicitudRecuperarCuentaDTO solicitud)
        {
            if (!ValidarSolicitudEntrada(solicitud))
            {
                return CrearFalloSolicitud(
                    MensajesError.Cliente.SolicitudRecuperacionIdentificadorObligatorio);
            }

            Usuario usuario;
            try
            {
                usuario = BuscarUsuarioParaRecuperacion(solicitud.Identificador);
            }
            catch (BaseDatosExcepcion excepcion)
            {
                _logger.Error(
                    "Error de base de datos al buscar usuario para proceso de " +
                    "recuperacion de cuenta.",
                    excepcion);
                return CrearFalloSolicitud(
                    MensajesError.Cliente.ErrorBaseDatosRecuperacion);
            }

            if (usuario == null)
            {
                return CrearFalloSolicitud(
                    MensajesError.Cliente.SolicitudRecuperacionCuentaNoEncontrada);
            }

            LimpiarSolicitudesRecuperacion(usuario.idUsuario);

            var generacion = GenerarYEnviarCodigo(usuario, solicitud.Idioma);
            if (!generacion.Exito)
            {
                return CrearFalloSolicitud(MensajesError.Cliente.ErrorRecuperarCuenta);
            }

            AlmacenarSolicitud(generacion.Token, generacion.Pendiente);

            return new ResultadoSolicitudRecuperacionDTO
            {
                CuentaEncontrada = true,
                CodigoEnviado = true,
                CorreoDestino = generacion.Pendiente.Correo,
                TokenCodigo = generacion.Token
            };
        }

        /// <summary>
        /// Reenvia un codigo de recuperacion previamente solicitado.
        /// </summary>
        public ResultadoSolicitudCodigoDTO ReenviarCodigoRecuperacion(ReenvioCodigoDTO solicitud)
        {
            if (!ValidarReenvioEntrada(solicitud))
            {
                return CrearFalloReenvio(MensajesError.Cliente.DatosReenvioCodigo);
            }

            SolicitudRecuperacionPendiente pendiente;
            if (!_solicitudesRecuperacion.TryGetValue(
                solicitud.TokenCodigo,
                out pendiente))
            {
                return CrearFalloReenvio(
                    MensajesError.Cliente.SolicitudRecuperacionNoEncontrada);
            }

            if (pendiente.Expira < DateTime.UtcNow)
            {
                SolicitudRecuperacionPendiente solicitudDescartada;
                _solicitudesRecuperacion.TryRemove(solicitud.TokenCodigo, out solicitudDescartada);
                return CrearFalloReenvio(MensajesError.Cliente.CodigoRecuperacionExpirado);
            }

            return ProcesarReenvio(solicitud.TokenCodigo, pendiente);
        }

        /// <summary>
        /// Confirma el codigo de recuperacion ingresado por el usuario.
        /// </summary>
        public ResultadoOperacionDTO ConfirmarCodigoRecuperacion(
            ConfirmacionCodigoDTO confirmacion)
        {
            if (!ValidarConfirmacionEntrada(confirmacion))
            {
                return CrearFalloOperacion(MensajesError.Cliente.DatosConfirmacionInvalidos);
            }

            SolicitudRecuperacionPendiente pendiente;
            if (!_solicitudesRecuperacion.TryGetValue(
                confirmacion.TokenCodigo,
                out pendiente))
            {
                return CrearFalloOperacion(
                    MensajesError.Cliente.SolicitudRecuperacionNoEncontrada);
            }

            return VerificarCodigo(pendiente, confirmacion.TokenCodigo, 
                confirmacion.CodigoIngresado);
        }

        /// <summary>
        /// Actualiza la contrasena de un usuario despues de confirmar el codigo de recuperacion.
        /// </summary>
        public ResultadoOperacionDTO ActualizarContrasena(ActualizacionContrasenaDTO solicitud)
        {
            if (!ValidarActualizacionEntrada(solicitud))
            {
                return CrearFalloOperacion(MensajesError.Cliente.DatosActualizacionContrasena);
            }

            var validacionToken = VerificarTokenYExpiracion(solicitud.TokenCodigo);
            if (!validacionToken.Exito)
            {
                return CrearFalloOperacion(validacionToken.MensajeError);
            }

            var pendiente = validacionToken.Pendiente;
            if (!pendiente.Confirmado)
            {
                return CrearFalloOperacion(
                    MensajesError.Cliente.SolicitudRecuperacionNoVigente);
            }

            return EjecutarCambioContrasena(
                pendiente.UsuarioId,
                solicitud.NuevaContrasena,
                solicitud.TokenCodigo);
        }

        private static bool ValidarSolicitudEntrada(SolicitudRecuperarCuentaDTO solicitud)
        {
            if (solicitud == null)
            {
                return false;
            }
            string identificador = EntradaComunValidador.NormalizarTexto(solicitud.Identificador);
            return EntradaComunValidador.EsLongitudValida(identificador);
        }

        private Usuario BuscarUsuarioParaRecuperacion(string identificador)
        {
            using (var contexto = _contextoFactoria.CrearContexto())
            {
                IUsuarioRepositorio repositorio = 
                    _repositorioFactoria.CrearUsuarioRepositorio(contexto);
                string idNormalizado = EntradaComunValidador.NormalizarTexto(identificador);
                var usuarioPorNombre = repositorio.ObtenerPorNombreConJugador(idNormalizado);

                if (usuarioPorNombre != null)
                {
                    return usuarioPorNombre;
                }

                return repositorio.ObtenerPorCorreo(idNormalizado);
            }
        }

        private (bool Exito, string Token, SolicitudRecuperacionPendiente Pendiente)
            GenerarYEnviarCodigo(Usuario usuario, string idioma)
        {
            string token = GeneradorAleatorio.GenerarToken();
            string codigo = GeneradorAleatorio.GenerarCodigoVerificacion();

            var pendiente = new SolicitudRecuperacionPendiente
            {
                UsuarioId = usuario.idUsuario,
                Correo = usuario.Jugador?.Correo,
                NombreUsuario = usuario.Nombre_Usuario,
                Codigo = codigo,
                Expira = DateTime.UtcNow.AddMinutes(MinutosExpiracionCodigo),
                Confirmado = false,
                Idioma = idioma
            };

            var parametrosNotificacion = new NotificacionCodigoParametros
            {
                CorreoDestino = pendiente.Correo,
                Codigo = codigo,
                UsuarioDestino = pendiente.NombreUsuario,
                Idioma = pendiente.Idioma
            };

            bool enviado = _notificacionServicio.EnviarNotificacion(parametrosNotificacion);

            if (!enviado)
            {
                _logger.Error(
                    "Error al enviar correo electronico con codigo de recuperacion de cuenta.");
                return (false, null, null);
            }

            return (true, token, pendiente);
        }

        private static void AlmacenarSolicitud(
            string token,
            SolicitudRecuperacionPendiente pendiente)
        {
            _solicitudesRecuperacion[token] = pendiente;
        }

        private ResultadoSolicitudRecuperacionDTO CrearFalloSolicitud(string mensaje)
        {
            return new ResultadoSolicitudRecuperacionDTO
            {
                CuentaEncontrada = false,
                CodigoEnviado = false,
                Mensaje = mensaje
            };
        }

        private static void LimpiarSolicitudesRecuperacion(int usuarioId)
        {
            var registros = new List<KeyValuePair<string, SolicitudRecuperacionPendiente>>();
            foreach (var solicitud in _solicitudesRecuperacion)
            {
                if (solicitud.Value.UsuarioId == usuarioId)
                {
                    registros.Add(solicitud);
                }
            }

            foreach (var registro in registros)
            {
                SolicitudRecuperacionPendiente solicitudDescartada;
                _solicitudesRecuperacion.TryRemove(registro.Key, out solicitudDescartada);
            }
        }

        private static bool ValidarReenvioEntrada(ReenvioCodigoDTO solicitud)
        {
            if (solicitud == null) return false;
            string token = EntradaComunValidador.NormalizarTexto(solicitud.TokenCodigo);
            return EntradaComunValidador.EsTokenValido(token);
        }

        private ResultadoSolicitudCodigoDTO ProcesarReenvio(
            string token,
            SolicitudRecuperacionPendiente pendiente)
        {
            string codigoAnterior = pendiente.Codigo;
            DateTime expiracionAnterior = pendiente.Expira;
            bool confirmadoAnterior = pendiente.Confirmado;

            string nuevoCodigo = GeneradorAleatorio.GenerarCodigoVerificacion();
            pendiente.Codigo = nuevoCodigo;
            pendiente.Expira = DateTime.UtcNow.AddMinutes(MinutosExpiracionCodigo);
            pendiente.Confirmado = false;

            var parametrosNotificacion = new NotificacionCodigoParametros
            {
                CorreoDestino = pendiente.Correo,
                Codigo = nuevoCodigo,
                UsuarioDestino = pendiente.NombreUsuario,
                Idioma = pendiente.Idioma
            };

            bool enviado = _notificacionServicio.EnviarNotificacion(parametrosNotificacion);

            if (!enviado)
            {
                pendiente.Codigo = codigoAnterior;
                pendiente.Expira = expiracionAnterior;
                pendiente.Confirmado = confirmadoAnterior;

                _logger.Error(
                    "Error al reenviar correo electronico con codigo de recuperacion.");
                return CrearFalloReenvio(
                    MensajesError.Cliente.ErrorReenviarCodigoRecuperacion);
            }

            return new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true,
                TokenCodigo = token
            };
        }

        private ResultadoSolicitudCodigoDTO CrearFalloReenvio(string mensaje)
        {
            return new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                Mensaje = mensaje
            };
        }

        private static bool ValidarConfirmacionEntrada(ConfirmacionCodigoDTO confirmacion)
        {
            if (confirmacion == null) return false;
            string token = EntradaComunValidador.NormalizarTexto(confirmacion.TokenCodigo);
            string codigo = EntradaComunValidador.NormalizarTexto(confirmacion.CodigoIngresado);

            return EntradaComunValidador.EsTokenValido(token) &&
                   EntradaComunValidador.EsCodigoVerificacionValido(codigo);
        }

        private static ResultadoOperacionDTO VerificarCodigo(
            SolicitudRecuperacionPendiente pendiente,
            string token,
            string codigoIngresado)
        {
            if (pendiente.Expira < DateTime.UtcNow)
            {
                SolicitudRecuperacionPendiente solicitudDescartada;
                _solicitudesRecuperacion.TryRemove(token, out solicitudDescartada);
                return CrearFalloOperacion(MensajesError.Cliente.CodigoRecuperacionExpirado);
            }

            if (!string.Equals(
                pendiente.Codigo,
                codigoIngresado,
                StringComparison.OrdinalIgnoreCase))
            {
                return CrearFalloOperacion(MensajesError.Cliente.CodigoRecuperacionIncorrecto);
            }

            pendiente.Confirmado = true;
            pendiente.Codigo = null;
            pendiente.Expira = DateTime.UtcNow.AddMinutes(MinutosExpiracionCodigo);

            return new ResultadoOperacionDTO { OperacionExitosa = true };
        }

        private static bool ValidarActualizacionEntrada(ActualizacionContrasenaDTO solicitud)
        {
            if (solicitud == null) return false;
            string token = EntradaComunValidador.NormalizarTexto(solicitud.TokenCodigo);
            string pass = EntradaComunValidador.NormalizarTexto(solicitud.NuevaContrasena);

            return EntradaComunValidador.EsTokenValido(token) &&
                   EntradaComunValidador.EsContrasenaValida(pass);
        }

        private static (bool Exito, SolicitudRecuperacionPendiente Pendiente, string MensajeError)
            VerificarTokenYExpiracion(string token)
        {
            SolicitudRecuperacionPendiente pendiente;
            if (!_solicitudesRecuperacion.TryGetValue(
                token,
                out pendiente))
            {
                return (false, null, MensajesError.Cliente.SolicitudRecuperacionNoEncontrada);
            }

            if (pendiente.Expira < DateTime.UtcNow)
            {
                SolicitudRecuperacionPendiente solicitudDescartada;
                _solicitudesRecuperacion.TryRemove(token, out solicitudDescartada);
                return (false, null, MensajesError.Cliente.SolicitudRecuperacionInvalida);
            }

            return (true, pendiente, null);
        }

        private ResultadoOperacionDTO EjecutarCambioContrasena(
            int usuarioId,
            string nuevaContrasena,
            string token)
        {
            try
            {
                using (var contexto = _contextoFactoria.CrearContexto())
                {
                    IUsuarioRepositorio repositorio = 
                        _repositorioFactoria.CrearUsuarioRepositorio(contexto);
                    string hash = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena);

                    repositorio.ActualizarContrasena(usuarioId, hash);
                }

                SolicitudRecuperacionPendiente solicitudDescartada;
                _solicitudesRecuperacion.TryRemove(token, out solicitudDescartada);
                return new ResultadoOperacionDTO { OperacionExitosa = true };
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    "Error de entidad al actualizar contrasena en base de datos.",
                    excepcion);
                return CrearFalloOperacion(
                    MensajesError.Cliente.ErrorActualizarContrasena);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    "Error de datos al actualizar contrasena del usuario.",
                    excepcion);
                return CrearFalloOperacion(
                    MensajesError.Cliente.ErrorActualizarContrasena);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Error inesperado al actualizar contrasena del usuario.",
                    excepcion);
                return CrearFalloOperacion(
                    MensajesError.Cliente.ErrorActualizarContrasena);
            }
        }

        private static ResultadoOperacionDTO CrearFalloOperacion(string mensaje)
        {
            return new ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = mensaje
            };
        }
    }
}
