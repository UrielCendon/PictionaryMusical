using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante
{
    /// <summary>
    /// Helper para manejar el ciclo de vida de los clientes WCF de forma segura.
    /// </summary>
    public class WcfClienteEjecutor : IWcfClienteEjecutor
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WcfClienteEjecutor));

        /// <summary>
        /// Ejecuta una operacion asincrona en un cliente WCF, asegurando el cierre correcto 
        /// del canal o su aborto en caso de fallo.
        /// </summary>
        /// <typeparam name="TClient">El tipo del cliente WCF que implementa ICommunicationObject.
        /// </typeparam>
        /// <typeparam name="TResult">El tipo de dato que retorna la operacion asincrona.
        /// </typeparam>
        /// <param name="cliente">La instancia del cliente WCF a utilizar.</param>
        /// <param name="operacion">La funcion asincrona a ejecutar con el cliente.</param>
        /// <returns>El resultado de la operacion asincrona.</returns>
        public async Task<TResult> EjecutarAsincronoAsync<TClient, TResult>(
            TClient cliente,
            Func<TClient, Task<TResult>> operacion)
            where TClient : class
        {
            ValidarParametrosEntrada(cliente, operacion);

            string nombreCliente = typeof(TClient).Name;
            
            try
            {
                TResult resultado = await operacion(cliente).ConfigureAwait(false);
                IntentarCerrarCliente(cliente);
                return resultado;
            }
            catch (ObjectDisposedException)
            {
                _logger.ErrorFormat(
                    "Modulo: {0} - Canal WCF desechado prematuramente. " +
                    "Posible perdida de conexion con el servidor.",
                    nombreCliente);
                ForzarAbortoCliente(cliente);
                throw;
            }
            catch (FaultException)
            {
                ForzarAbortoCliente(cliente);
                throw;
            }
            catch (CommunicationException)
            {
                _logger.ErrorFormat(
                    "Modulo: {0} - Error de comunicacion WCF detectado. " +
                    "El servidor puede no estar disponible o hubo perdida de conexion de red.",
                    nombreCliente);
                ForzarAbortoCliente(cliente);
                throw;
            }
            catch (TimeoutException)
            {
                _logger.ErrorFormat(
                    "Modulo: {0} - Tiempo de espera agotado en operacion WCF. " +
                    "El servidor no respondio a tiempo o hay problemas de conectividad.",
                    nombreCliente);
                ForzarAbortoCliente(cliente);
                throw;
            }
            catch (InvalidOperationException)
            {
                ForzarAbortoCliente(cliente);
                throw;
            }
        }

        private static void ValidarParametrosEntrada<TClient, TResult>(
            TClient cliente,
            Func<TClient, Task<TResult>> operacion)
            where TClient : class
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente));
            }

            if (operacion == null)
            {
                throw new ArgumentNullException(nameof(operacion));
            }
        }

        private static void IntentarCerrarCliente(object cliente)
        {
            if (cliente == null)
            {
                return;
            }

            if (cliente is ICommunicationObject canal)
            {
                try
                {
                    if (canal.State == CommunicationState.Opened)
                    {
                        canal.Close();
                    }
                    else
                    {
                        canal.Abort();
                    }
                }
                catch (CommunicationException excepcion)
                {
                    _logger.Error(
                        "Excepcion de comunicacion al cerrar cliente WCF.",
                        excepcion);
                    canal.Abort();
                }
                catch (TimeoutException excepcion)
                {
                    _logger.Error(
                        "Tiempo agotado al cerrar cliente WCF.",
                        excepcion);
                    canal.Abort();
                }
                catch (InvalidOperationException excepcion)
                {
                    _logger.Error(
                        "Operacion invalida al cerrar cliente WCF.",
                        excepcion);
                    canal.Abort();
                }
            }
        }

        private static void ForzarAbortoCliente(object cliente)
        {
            if (cliente == null)
            {
                return;
            }

            if (cliente is ICommunicationObject canal)
            {
                try
                {
                    canal.Abort();
                }
                catch (Exception excepcion)
                {
                    _logger.Error("Error critico inesperado al abortar cliente WCF.", excepcion);
                }
            }
        }
    }
}