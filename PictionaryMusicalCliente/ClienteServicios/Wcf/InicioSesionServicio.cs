using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Gestiona la autenticacion del usuario en el sistema.
    /// </summary>
    public class InicioSesionServicio : IInicioSesionServicio
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(InicioSesionServicio));
        private readonly IWcfClienteEjecutor _ejecutor;
        private readonly IWcfClienteFabrica _fabricaClientes;
        private readonly IManejadorErrorServicio _manejadorError;
        private readonly IUsuarioMapeador _usuarioMapeador;
        private readonly ILocalizadorServicio _localizador;

        /// <summary>
        /// Inicializa el servicio de inicio de sesion.
        /// </summary>
        /// <param name="ejecutor">Ejecutor de operaciones WCF.</param>
        /// <param name="fabricaClientes">Fabrica para crear clientes WCF.</param>
        /// <param name="manejadorError">Manejador para procesar errores de servicio.</param>
        /// <param name="usuarioMapeador">Mapeador para actualizar datos de sesion.</param>
        /// <param name="localizador">Servicio de localizacion de textos.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia es nula.
        /// </exception>
        public InicioSesionServicio(
            IWcfClienteEjecutor ejecutor,
            IWcfClienteFabrica fabricaClientes,
            IManejadorErrorServicio manejadorError,
            IUsuarioMapeador usuarioMapeador,
            ILocalizadorServicio localizador)
        {
            _ejecutor = ejecutor ?? throw new ArgumentNullException(nameof(ejecutor));
            _fabricaClientes = fabricaClientes ??
                throw new ArgumentNullException(nameof(fabricaClientes));
            _manejadorError = manejadorError ??
                throw new ArgumentNullException(nameof(manejadorError));
            _usuarioMapeador = usuarioMapeador ??
                throw new ArgumentNullException(nameof(usuarioMapeador));
            _localizador = localizador ?? throw new ArgumentNullException(nameof(localizador));
        }

        /// <summary>
        /// Valida las credenciales del usuario e inicia la sesion local si es correcto.
        /// </summary>
        public async Task<DTOs.ResultadoInicioSesionDTO> IniciarSesionAsync(
            DTOs.CredencialesInicioSesionDTO solicitud)
        {
            if (solicitud == null)
            {
                throw new ArgumentNullException(nameof(solicitud));
            }

            try
            {
                var resultado = await _ejecutor.EjecutarAsincronoAsync(
                    _fabricaClientes.CrearClienteInicioSesion(),
                    c => c.IniciarSesionAsync(solicitud)
                ).ConfigureAwait(false);

                ProcesarResultadoSesion(resultado);
                return resultado;
            }
            catch (FaultException excepcion)
            {
                _logger.Warn("Error de logica servidor en inicio de sesion.", excepcion);
                string mensaje = _manejadorError.ObtenerMensaje(
                    excepcion,
                    Lang.errorTextoServidorInicioSesion);
                throw new ServicioExcepcion(TipoErrorServicio.FallaServicio, mensaje, excepcion);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("Error WCF en inicio de sesion.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible,
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error("Timeout en inicio de sesion.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.TiempoAgotado,
                    Lang.errorTextoServidorNoDisponible,
                    excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida en inicio de sesion.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.OperacionInvalida,
                    Lang.errorTextoErrorProcesarSolicitud,
                    excepcion);
            }
        }

        private void ProcesarResultadoSesion(DTOs.ResultadoInicioSesionDTO resultado)
        {
            if (resultado == null)
            {
                RegistrarResultadoNulo();
                return;
            }

            ActualizarDatosSesion(resultado);
            LocalizarMensaje(resultado);
            RegistrarResultadoSesion(resultado);
        }

        private void ActualizarDatosSesion(DTOs.ResultadoInicioSesionDTO resultado)
        {
            _usuarioMapeador.ActualizarSesion(resultado.Usuario);
        }

        private void LocalizarMensaje(DTOs.ResultadoInicioSesionDTO resultado)
        {
            resultado.Mensaje = _localizador.Localizar(resultado.Mensaje, resultado.Mensaje);
        }

        private static void RegistrarResultadoNulo()
        {
            _logger.Warn("Servicio retorno null en inicio de sesion.");
        }

        private static void RegistrarResultadoSesion(DTOs.ResultadoInicioSesionDTO resultado)
        {
            if (resultado.Usuario != null)
            {
                _logger.InfoFormat(
                    "Usuario con id '{0}' inicio sesion.",
                    resultado.Usuario.UsuarioId);
            }
            else
            {
                _logger.Warn("Log in fallido: Credenciales incorrectas o cuenta no encontrada.");
            }
        }
    }
}