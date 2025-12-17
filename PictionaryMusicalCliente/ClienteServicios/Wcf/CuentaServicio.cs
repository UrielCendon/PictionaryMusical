using System;
using System.ServiceModel;
using System.Threading.Tasks;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Servicio encargado de la creacion de nuevas cuentas de usuario.
    /// </summary>
    public class CuentaServicio : ICuentaServicio
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CuentaServicio)); 
        private readonly IWcfClienteEjecutor _ejecutor;
        private readonly IWcfClienteFabrica _fabricaClientes;
        private readonly IManejadorErrorServicio _manejadorError;

        /// <summary>
        /// Inicializa el servicio de cuentas con sus dependencias.
        /// </summary>
        /// <param name="ejecutor">Ejecutor de operaciones WCF.</param>
        /// <param name="fabricaClientes">Fabrica para crear clientes WCF.</param>
        /// <param name="manejadorError">Manejador para procesar errores de servicio.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia es nula.
        /// </exception>
        public CuentaServicio(
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
        /// Envia una solicitud de registro al servidor.
        /// </summary>
        public async Task<DTOs.ResultadoRegistroCuentaDTO> RegistrarCuentaAsync(
            DTOs.NuevaCuentaDTO solicitud)
        {
            if (solicitud == null)
            {
                throw new ArgumentNullException(nameof(solicitud));
            }

            try
            {
                var resultado = await _ejecutor.EjecutarAsincronoAsync(
                    _fabricaClientes.CrearClienteCuenta(),
                    c => c.RegistrarCuentaAsync(solicitud)
                ).ConfigureAwait(false);

                RegistrarLogResultado(resultado, solicitud.Correo);
                return resultado;
            }
            catch (FaultException excepcion)
            {
                _logger.Warn("Servidor rechazo el registro (Validacion/Negocio).", excepcion);
                string mensaje = _manejadorError.ObtenerMensaje(
                    excepcion,
                    Lang.errorTextoRegistrarCuentaMasTarde);
                throw new ServicioExcepcion(TipoErrorServicio.FallaServicio, mensaje, excepcion);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("Error de comunicacion al registrar cuenta.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible,
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error("Timeout al registrar cuenta.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.TiempoAgotado,
                    Lang.errorTextoServidorTiempoAgotado,
                    excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida al registrar cuenta.", excepcion);
                throw new ServicioExcepcion(
                    TipoErrorServicio.OperacionInvalida,
                    Lang.errorTextoErrorProcesarSolicitud,
                    excepcion);
            }
        }

        private static void RegistrarLogResultado(
            DTOs.ResultadoRegistroCuentaDTO resultado,
            string correo)
        {
            if (!resultado?.RegistroExitoso == true)
            {
                _logger.WarnFormat("Registro fallido para: {0}. Razon: {1}",
                    correo, resultado?.Mensaje);
            }
        }
    }
}