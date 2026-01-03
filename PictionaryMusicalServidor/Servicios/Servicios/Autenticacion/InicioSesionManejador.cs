using Datos.Modelo;
using log4net;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using BCryptNet = BCrypt.Net.BCrypt;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Implementacion del servicio de autenticacion de usuarios.
    /// Valida credenciales comparando identificador y contrasena con hash BCrypt.
    /// </summary>
    public class InicioSesionManejador : IInicioSesionManejador
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(InicioSesionManejador));

        private const int LimiteReportesParaBaneo = 3;

        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;

        /// <summary>
        /// Constructor por defecto para uso en WCF.
        /// </summary>
        public InicioSesionManejador() : this(new ContextoFactoria(), new RepositorioFactoria())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        public InicioSesionManejador(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria)
        {
            _contextoFactoria = contextoFactoria ??
                throw new ArgumentNullException(nameof(contextoFactoria));
            _repositorioFactoria = repositorioFactoria ??
                throw new ArgumentNullException(nameof(repositorioFactoria));
        }

        /// <summary>
        /// Inicia sesion de un usuario validando sus credenciales.
        /// </summary>
        /// <param name="credenciales">Credenciales de inicio de sesion del usuario.</param>
        /// <returns>Resultado del inicio de sesion con datos del usuario si es exitoso.</returns>
        public ResultadoInicioSesionDTO IniciarSesion(
            CredencialesInicioSesionDTO credenciales)
        {
            if (credenciales == null)
            {
                throw new ArgumentNullException(nameof(credenciales));
            }

            if (!SonCredencialesValidas(credenciales))
            {
                _logger.Warn(MensajesError.Bitacora.IntentoInicioSesionFormatoInvalido);
                return CrearRespuestaDatosInvalidos();
            }

            try
            {
                using (var contexto = _contextoFactoria.CrearContexto())
                {
                    return ProcesarAutenticacion(contexto, credenciales);
                }
            }
            catch (Datos.Excepciones.BaseDatosExcepcion excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorBaseDatosInicioSesion, excepcion);
                return CrearErrorGenerico();
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorBaseDatosInicioSesion, excepcion);
                return CrearErrorGenerico();
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorDatosInicioSesion, excepcion);
                return CrearErrorGenerico();
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.OperacionInvalidaInicioSesion, excepcion);
                return CrearErrorGenerico();
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoInicioSesion, excepcion);
                return CrearErrorGenerico();
            }
        }

        private static bool SonCredencialesValidas(CredencialesInicioSesionDTO credenciales)
        {
            string identificador = EntradaComunValidador.NormalizarTexto(
                credenciales.Identificador);

            string contrasena = credenciales.Contrasena?.Trim();

            return EntradaComunValidador.EsLongitudValida(identificador) &&
                   !string.IsNullOrWhiteSpace(contrasena);
        }

        private ResultadoInicioSesionDTO CrearRespuestaDatosInvalidos()
        {
            return new ResultadoInicioSesionDTO
            {
                CuentaEncontrada = true,
                Mensaje = MensajesError.Cliente.CredencialesInvalidas
            };
        }

        private ResultadoInicioSesionDTO ProcesarAutenticacion(
            BaseDatosPruebaEntities contexto,
            CredencialesInicioSesionDTO credenciales)
        {
            string identificador = EntradaComunValidador.NormalizarTexto(
                credenciales.Identificador);

            Usuario usuario;
            try
            {
                usuario = ObtenerUsuarioPorCredencial(contexto, identificador);
            }
            catch (Datos.Excepciones.BaseDatosExcepcion excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorBaseDatosInicioSesion,
                    excepcion);
                return CrearErrorGenerico();
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.InicioSesionUsuarioNoEncontrado,
                    excepcion);
                return new ResultadoInicioSesionDTO
                {
                    CuentaEncontrada = false,
                    Mensaje = MensajesError.Cliente.CredencialesIncorrectas
                };
            }

            if (!VerificarContrasena(credenciales.Contrasena, usuario.Contrasena))
            {
                _logger.Warn(MensajesError.Bitacora.InicioSesionContrasenaIncorrecta);

                return new ResultadoInicioSesionDTO
                {
                    ContrasenaIncorrecta = true,
                    Mensaje = MensajesError.Cliente.CredencialesIncorrectas
                };
            }

            if (UsuarioAlcanzoLimiteReportes(contexto, usuario.idUsuario))
            {
                _logger.WarnFormat(
                    "Usuario con identificador {0} bloqueado por superar limite de reportes.",
                    usuario.idUsuario);

                return new ResultadoInicioSesionDTO
                {
                    InicioSesionExitoso = false,
                    CuentaEncontrada = true,
                    Mensaje = MensajesError.Cliente.UsuarioBaneadoPorReportes
                };
            }

            _logger.InfoFormat(
                "Inicio de sesion exitoso para usuario con identificador {0}.",
                usuario.idUsuario);

            return new ResultadoInicioSesionDTO
            {
                InicioSesionExitoso = true,
                Usuario = MapearUsuario(usuario)
            };
        }

        private static bool VerificarContrasena(string contrasenaIngresada, string hashAlmacenado)
        {
            if (string.IsNullOrWhiteSpace(contrasenaIngresada) ||
                string.IsNullOrWhiteSpace(hashAlmacenado))
            {
                return false;
            }

            return BCryptNet.Verify(contrasenaIngresada.Trim(), hashAlmacenado);
        }

        private bool UsuarioAlcanzoLimiteReportes(BaseDatosPruebaEntities contexto, int usuarioId)
        {
            IReporteRepositorio reporteRepositorio = 
                _repositorioFactoria.CrearReporteRepositorio(contexto);
            int totalReportes = reporteRepositorio.ContarReportesRecibidos(usuarioId);

            return totalReportes >= LimiteReportesParaBaneo;
        }

        private static ResultadoInicioSesionDTO CrearErrorGenerico()
        {
            return new ResultadoInicioSesionDTO
            {
                InicioSesionExitoso = false,
                CuentaEncontrada = true,
                Mensaje = MensajesError.Cliente.ErrorInicioSesion
            };
        }

        private Usuario ObtenerUsuarioPorCredencial(
            BaseDatosPruebaEntities contexto,
            string identificador)
        {
            try
            {
                return BuscarPorNombreUsuario(contexto, identificador);
            }
            catch (Datos.Excepciones.BaseDatosExcepcion)
            {
                throw;
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Usuario no encontrado por nombre '{0}', se intentara buscar por correo electronico.",
                        identificador),
                    excepcion);
                return BuscarPorCorreoElectronico(contexto, identificador);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    string.Format(
                        "Error inesperado al buscar usuario por nombre '{0}', intentando por correo electronico.",
                        identificador),
                    excepcion);
                return BuscarPorCorreoElectronico(contexto, identificador);
            }
        }

        private Usuario BuscarPorNombreUsuario(
            BaseDatosPruebaEntities contexto,
            string nombreUsuario)
        {
            IUsuarioRepositorio repositorio = 
                _repositorioFactoria.CrearUsuarioRepositorio(contexto);
            return repositorio.ObtenerPorNombreUsuario(nombreUsuario);
        }

        private Usuario BuscarPorCorreoElectronico(
            BaseDatosPruebaEntities contexto,
            string correo)
        {
            IUsuarioRepositorio repositorio = 
                _repositorioFactoria.CrearUsuarioRepositorio(contexto);
            Usuario usuario = repositorio.ObtenerPorCorreo(correo);

            if (usuario == null)
            {
                throw new KeyNotFoundException(
                    string.Format(
                        "Usuario no encontrado por correo electronico: {0}",
                        correo));
            }
            return usuario;
        }

        private static UsuarioDTO MapearUsuario(Usuario usuario)
        {
            Jugador jugador = usuario.Jugador;

            return new UsuarioDTO
            {
                UsuarioId = usuario.idUsuario,
                JugadorId = jugador?.idJugador ?? 0,
                NombreUsuario = usuario.Nombre_Usuario,
                Nombre = jugador?.Nombre,
                Apellido = jugador?.Apellido,
                Correo = jugador?.Correo,
                AvatarId = jugador?.Id_Avatar ?? 0,
            };
        }
    }
}
