using System;
using System.ServiceModel;
using System.Threading.Tasks;
using log4net;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante
{
    /// <summary>
    /// Helper para manejar el ciclo de vida de los clientes WCF de forma segura.
    /// </summary>
    public static class WcfClienteAyudante
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WcfClienteAyudante));

        /// <summary>
        /// Ejecuta una operación asíncrona en un cliente WCF, asegurando el cierre correcto 
        /// del canal o su aborto en caso de fallo.
        /// </summary>
        /// <typeparam name="TClient">El tipo del cliente WCF que implementa ICommunicationObject.
        /// </typeparam>
        /// <typeparam name="TResult">El tipo de dato que retorna la operación asíncrona.
        /// </typeparam>
        /// <param name="cliente">La instancia del cliente WCF a utilizar.</param>
        /// <param name="operacion">La función asíncrona a ejecutar con el cliente.</param>
        /// <returns>El resultado de la operación asíncrona.</returns>
        public static async Task<TResult> UsarAsincronoAsync<TClient, TResult>(
            TClient cliente,
            Func<TClient, Task<TResult>> operacion)
            where TClient : class, ICommunicationObject
        {
            ValidarParametrosEntrada(cliente, operacion);

            try
            {
                TResult resultado = await operacion(cliente).ConfigureAwait(false);
                IntentarCerrarCliente(cliente);
                return resultado;
            }
            catch (CommunicationException)
            {
                ForzarAbortoCliente(cliente);
                throw;
            }
            catch (TimeoutException)
            {
                ForzarAbortoCliente(cliente);
                throw;
            }
            catch (InvalidOperationException)
            {
                ForzarAbortoCliente(cliente);
                throw;
            }
            catch (Exception)
            {
                ForzarAbortoCliente(cliente);
                throw;
            }
        }

        private static void ValidarParametrosEntrada<TClient, TResult>(
            TClient cliente,
            Func<TClient, Task<TResult>> operacion)
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

        private static void IntentarCerrarCliente(ICommunicationObject cliente)
        {
            if (cliente == null)
            {
                return;
            }

            try
            {
                if (cliente.State == CommunicationState.Opened)
                {
                    cliente.Close();
                }
                else
                {
                    cliente.Abort();
                }
            }
            catch (CommunicationException ex)
            {
                _logger.Error("Excepción de comunicación al intentar cerrar el cliente WCF.", ex);
                cliente.Abort();
            }
            catch (TimeoutException ex)
            {
                _logger.Error("Tiempo de espera agotado al intentar cerrar el cliente WCF.", ex);
                cliente.Abort();
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error("Operación inválida al intentar cerrar el cliente WCF.", ex);
                cliente.Abort();
            }
        }

        private static void ForzarAbortoCliente(ICommunicationObject cliente)
        {
            if (cliente == null)
            {
                return;
            }

            try
            {
                cliente.Abort();
            }
            catch (Exception ex)
            {
                // Ignorado de manera intencional: No se puede hacer nada para manejar
                // una excepcion al abortar, pero se registra como error.
                _logger.Error("Error crítico inesperado al abortar cliente WCF.", ex);
            }
        }
    }
}