using System;
using System.Collections.Concurrent;
using System.Linq;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios
{
    /// <summary>
    /// Servicio interno para la logica de negocio de verificacion de registro de cuentas.
    /// Maneja el almacenamiento temporal de solicitudes y validacion de codigos de verificacion.
    /// Verifica disponibilidad de usuario y correo antes de enviar codigos.
    /// </summary>
    public class VerificacionRegistroServicio : IVerificacionRegistroServicio
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(VerificacionRegistroServicio));

        private const int MinutosExpiracionCodigo = 5;

        private static readonly ConcurrentDictionary<string, SolicitudCodigoPendiente> 
            _solicitudes = new ConcurrentDictionary<string, SolicitudCodigoPendiente>();

        private static readonly ConcurrentDictionary<string, byte> _verificacionesConfirmadas =
            new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

        private readonly IContextoFactory _contextoFactory;
        private readonly INotificacionCodigosServicio _notificacionCodigosServicio;

        /// <summary>
        /// Constructor con inyeccion de dependencias.
        /// </summary>
        public VerificacionRegistroServicio(IContextoFactory contextoFactory, 
            INotificacionCodigosServicio notificacionCodigosServicio)
        {
            _contextoFactory = contextoFactory ??
                throw new ArgumentNullException(nameof(contextoFactory));
            _notificacionCodigosServicio = notificacionCodigosServicio ??
                throw new ArgumentNullException(nameof(notificacionCodigosServicio));
        }

        /// <summary>
        /// Solicita un codigo de verificacion para registrar una nueva cuenta.
        /// Valida datos, verifica disponibilidad, genera codigo y lo envia por correo.
        /// </summary>
        /// <param name="nuevaCuenta">Datos de la nueva cuenta a registrar.</param>
        /// <returns>Resultado indicando si el codigo fue enviado y posibles conflictos de usuario
        /// o correo.</returns>
        public ResultadoSolicitudCodigoDTO SolicitarCodigo(NuevaCuentaDTO nuevaCuenta)
        {
            if (nuevaCuenta == null)
            {
                throw new ArgumentNullException(nameof(nuevaCuenta));
            }

            ResultadoOperacionDTO validacionDatos =
                EntradaComunValidador.ValidarNuevaCuenta(nuevaCuenta);

            if (!validacionDatos.OperacionExitosa)
            {
                return new ResultadoSolicitudCodigoDTO
                {
                    CodigoEnviado = false,
                    Mensaje = validacionDatos.Mensaje
                };
            }

            using (var contexto = _contextoFactory.CrearContexto())
            {
                bool usuarioRegistrado = contexto.Usuario.Any(
                    u => u.Nombre_Usuario == nuevaCuenta.Usuario);
                bool correoRegistrado = contexto.Jugador.Any(
                    j => j.Correo == nuevaCuenta.Correo);

                if (usuarioRegistrado || correoRegistrado)
                {
                    _logger.Warn("Registro duplicado. El usuario y el correo existen.");

                    return new ResultadoSolicitudCodigoDTO
                    {
                        CodigoEnviado = false,
                        UsuarioRegistrado = usuarioRegistrado,
                        CorreoRegistrado = correoRegistrado,
                        Mensaje = MensajesError.Cliente.UsuarioOCorreoRegistrado
                    };
                }
            }

            string token = TokenGenerador.GenerarToken();
            string codigo = CodigoVerificacionGenerador.GenerarCodigo();
            NuevaCuentaDTO datosCuenta = CopiarCuenta(nuevaCuenta);

            bool enviado = _notificacionCodigosServicio.EnviarNotificacion(
                datosCuenta.Correo,
                codigo,
                datosCuenta.Usuario,
                datosCuenta.Idioma);

            if (!enviado)
            {
                _logger.ErrorFormat(
                    "Error al enviar codigo de verificacion a '{0}'.",
                    datosCuenta.Correo);

                return new ResultadoSolicitudCodigoDTO
                {
                    CodigoEnviado = false,
                    Mensaje = MensajesError.Cliente.ErrorSolicitudVerificacion
                };
            }

            var solicitud = new SolicitudCodigoPendiente
            {
                DatosCuenta = datosCuenta,
                Codigo = codigo,
                Expira = DateTime.UtcNow.AddMinutes(MinutosExpiracionCodigo)
            };

            _solicitudes[token] = solicitud;

            return new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true,
                TokenCodigo = token
            };
        }

        /// <summary>
        /// Reenvia un codigo de verificacion previamente solicitado para registro.
        /// Valida el token, genera un nuevo codigo con nueva expiracion y lo envia por correo.
        /// </summary>
        public ResultadoSolicitudCodigoDTO ReenviarCodigo(ReenvioCodigoVerificacionDTO solicitud)
        {
            if (solicitud == null)
            {
                throw new ArgumentNullException(nameof(solicitud));
            }

            string token = EntradaComunValidador.NormalizarTexto(solicitud.TokenCodigo);
            if (!EntradaComunValidador.EsTokenValido(token))
            {
                return new ResultadoSolicitudCodigoDTO
                {
                    CodigoEnviado = false,
                    Mensaje = MensajesError.Cliente.DatosReenvioCodigo
                };
            }

            if (!_solicitudes.TryGetValue(token, out SolicitudCodigoPendiente existente))
            {
                _logger.Warn("Intento de reenvio de codigo no encontrado o expirado.");
                return new ResultadoSolicitudCodigoDTO
                {
                    CodigoEnviado = false,
                    Mensaje = MensajesError.Cliente.SolicitudVerificacionNoEncontrada
                };
            }

            string codigoAnterior = existente.Codigo;
            DateTime expiracionAnterior = existente.Expira;

            string nuevoCodigo = CodigoVerificacionGenerador.GenerarCodigo();
            existente.Codigo = nuevoCodigo;
            existente.Expira = DateTime.UtcNow.AddMinutes(MinutosExpiracionCodigo);

            bool enviado = _notificacionCodigosServicio.EnviarNotificacion(
                existente.DatosCuenta.Correo,
                nuevoCodigo,
                existente.DatosCuenta.Usuario,
                existente.DatosCuenta.Idioma);

            if (!enviado)
            {
                existente.Codigo = codigoAnterior;
                existente.Expira = expiracionAnterior;
                _logger.ErrorFormat(
                    "Error al reenviar codigo de verificacion a '{0}'.",
                    existente.DatosCuenta.Correo);

                return new ResultadoSolicitudCodigoDTO
                {
                    CodigoEnviado = false,
                    Mensaje = MensajesError.Cliente.ErrorReenviarCodigoVerificacion
                };
            }

            return new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = true,
                TokenCodigo = token
            };
        }

        /// <summary>
        /// Confirma el codigo de verificacion ingresado para registro de cuenta.
        /// Valida el token y el codigo, y marca la verificacion como confirmada.
        /// </summary>
        /// <param name="confirmacion">Datos con el token y codigo ingresado.</param>
        /// <returns>Resultado indicando si el codigo fue confirmado correctamente.</returns>
        public ResultadoRegistroCuentaDTO ConfirmarCodigo(ConfirmacionCodigoDTO confirmacion)
        {
            if (confirmacion == null)
            {
                throw new ArgumentNullException(nameof(confirmacion));
            }

            string token = EntradaComunValidador.NormalizarTexto(confirmacion.TokenCodigo);
            string codigoIngresado = EntradaComunValidador.NormalizarTexto(
                confirmacion.CodigoIngresado);

            if (!EntradaComunValidador.EsTokenValido(token) ||
                !EntradaComunValidador.EsCodigoVerificacionValido(codigoIngresado))
            {
                return new ResultadoRegistroCuentaDTO
                {
                    RegistroExitoso = false,
                    Mensaje = MensajesError.Cliente.DatosConfirmacionInvalidos
                };
            }

            if (!_solicitudes.TryGetValue(token, out SolicitudCodigoPendiente pendiente))
            {
                return new ResultadoRegistroCuentaDTO
                {
                    RegistroExitoso = false,
                    Mensaje = MensajesError.Cliente.SolicitudVerificacionNoEncontrada
                };
            }

            if (pendiente.Expira < DateTime.UtcNow)
            {
                _solicitudes.TryRemove(token, out _);
                _logger.WarnFormat(
                    "Codigo expirado para usuario '{0}'.",
                    pendiente.DatosCuenta.Usuario);

                return new ResultadoRegistroCuentaDTO
                {
                    RegistroExitoso = false,
                    Mensaje = MensajesError.Cliente.CodigoVerificacionExpirado
                };
            }

            if (!string.Equals(
                pendiente.Codigo,
                codigoIngresado,
                StringComparison.OrdinalIgnoreCase))
            {
                _logger.WarnFormat(
                    "Codigo incorrecto ingresado para usuario '{0}'.",
                    pendiente.DatosCuenta.Usuario);

                return new ResultadoRegistroCuentaDTO
                {
                    RegistroExitoso = false,
                    Mensaje = MensajesError.Cliente.CodigoVerificacionIncorrecto
                };
            }

            _solicitudes.TryRemove(token, out _);

            string clave = ObtenerClave(
                pendiente.DatosCuenta.Usuario,
                pendiente.DatosCuenta.Correo);

            _verificacionesConfirmadas[clave] = 0;

            return new ResultadoRegistroCuentaDTO
            {
                RegistroExitoso = true
            };
        }

        /// <summary>
        /// Verifica si una cuenta tiene una verificacion confirmada pendiente.
        /// </summary>
        /// <param name="nuevaCuenta">Datos de la cuenta a verificar.</param>
        /// <returns>True si la verificacion esta confirmada, false en caso contrario o si 
        /// nuevaCuenta es null.</returns>
        public bool EstaVerificacionConfirmada(NuevaCuentaDTO nuevaCuenta)
        {
            if (nuevaCuenta == null)
            {
                return false;
            }

            string clave = ObtenerClave(nuevaCuenta.Usuario, nuevaCuenta.Correo);
            return _verificacionesConfirmadas.ContainsKey(clave);
        }

        /// <summary>
        /// Limpia la verificacion confirmada de una cuenta despues de completar el registro.
        /// </summary>
        /// <param name="nuevaCuenta">Datos de la cuenta cuya verificacion se limpiara.</param>
        public void LimpiarVerificacion(NuevaCuentaDTO nuevaCuenta)
        {
            if (nuevaCuenta == null)
            {
                return;
            }

            string clave = ObtenerClave(nuevaCuenta.Usuario, nuevaCuenta.Correo);
            _verificacionesConfirmadas.TryRemove(clave, out _);
        }

        private static NuevaCuentaDTO CopiarCuenta(NuevaCuentaDTO original)
        {
            return new NuevaCuentaDTO
            {
                Usuario = original.Usuario,
                Correo = original.Correo,
                Nombre = original.Nombre,
                Apellido = original.Apellido,
                Contrasena = original.Contrasena,
                AvatarId = original.AvatarId,
                Idioma = original.Idioma
            };
        }

        private static string ObtenerClave(string usuario, string correo)
        {
            return ($"{usuario}|{correo}").ToLowerInvariant();
        }

        private sealed class SolicitudCodigoPendiente
        {
            public NuevaCuentaDTO DatosCuenta { get; set; }
            public string Codigo { get; set; }
            public DateTime Expira { get; set; }
        }
    }
}