using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Maneja el envio de invitaciones a partidas mediante correo electronico.
    /// </summary>
    public class InvitacionesServicio : IInvitacionesServicio
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(InvitacionesServicio));
        private readonly IWcfClienteEjecutor _ejecutor;
        private readonly IWcfClienteFabrica _fabricaClientes;
        private readonly IManejadorErrorServicio _manejadorError;

        /// <summary>
        /// Inicializa el servicio de invitaciones.
        /// </summary>
        /// <param name="ejecutor">Ejecutor de operaciones WCF.</param>
        /// <param name="fabricaClientes">Fabrica para crear clientes WCF.</param>
        /// <param name="manejadorError">Manejador para procesar errores de servicio.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia es nula.
        /// </exception>
        public InvitacionesServicio(
            IWcfClienteEjecutor ejecutor,
            IWcfClienteFabrica fabricaClientes,
            IManejadorErrorServicio manejadorError)
        {
            _ejecutor = ejecutor ?? throw new ArgumentNullException(nameof(ejecutor));
            _fabricaClientes = fabricaClientes ??
                throw new ArgumentNullException(nameof(fabricaClientes));
            _manejadorError = manejadorError ??
                throw new ArgumentNullException(nameof(manejadorError));
        }

        /// <summary>
        /// Envia una invitacion por correo para unirse a una sala especifica.
        /// </summary>
        public async Task<DTOs.ResultadoOperacionDTO> EnviarInvitacionAsync(
            string codigoSala,
            string correoDestino)
        {
            ValidarDatosEntrada(codigoSala, correoDestino);
            VerificarConexionRed();

            var solicitud = new DTOs.InvitacionSalaDTO
            {
                CodigoSala = codigoSala.Trim(),
                Correo = correoDestino.Trim(),
                Idioma = ObtenerCodigoIdiomaActual()
            };

            try
            {
                var resultado = await _ejecutor.EjecutarAsincronoAsync(
                    _fabricaClientes.CrearClienteInvitaciones(),
                    c => c.EnviarInvitacionAsync(solicitud)
                ).ConfigureAwait(false);

                return resultado;
            }
            catch (FaultException excepcion)
            {
                _logger.Warn("El servidor rechazo la invitacion.", excepcion);
                string mensaje = _manejadorError.ObtenerMensaje(excepcion, 
                    Lang.errorTextoEnviarCorreo);
                throw new ServicioExcepcion(TipoErrorServicio.FallaServicio, mensaje, excepcion);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("Error WCF al enviar invitacion.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible,
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error("Timeout al enviar invitacion.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.TiempoAgotado,
                    Lang.errorTextoServidorNoDisponible,
                    excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida en invitaciones.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.OperacionInvalida,
                    Lang.errorTextoErrorProcesarSolicitud,
                    excepcion);
            }
        }

        private static void ValidarDatosEntrada(string codigoSala, string correo)
        {
            if (string.IsNullOrWhiteSpace(codigoSala))
                throw new ArgumentException("Codigo sala obligatorio.", nameof(codigoSala));

            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("Correo destino obligatorio.", nameof(correo));
        }

        private static string ObtenerCodigoIdiomaActual()
        {
            var culturaActual = Lang.Culture;
            if (culturaActual != null)
            {
                return culturaActual.Name;
            }

            return System.Globalization.CultureInfo.CurrentUICulture.Name;
        }

        private static void VerificarConexionRed()
        {
            if (!ConectividadRedMonitor.HayConexion)
            {
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible);
            }
        }
    }
}