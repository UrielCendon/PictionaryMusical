using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Manejador generico para callbacks de servicios WCF.
    /// Gestiona el ciclo de vida de las suscripciones de callbacks.
    /// </summary>
    /// <typeparam name="TCallback">Tipo de callback a gestionar.</typeparam>
    internal class ManejadorCallback<TCallback> : IManejadorCallback<TCallback> 
        where TCallback : class
    {
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(ManejadorCallback<TCallback>));
        private readonly ConcurrentDictionary<string, TCallback> _suscripciones;

        public ManejadorCallback(StringComparer comparer = null)
        {
            _suscripciones = comparer != null 
                ? new ConcurrentDictionary<string, TCallback>(comparer)
                : new ConcurrentDictionary<string, TCallback>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Registra una suscripcion de callback para un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        /// <param name="callback">Callback a registrar.</param>
        public void Suscribir(string nombreUsuario, TCallback callback)
        {
            if (!EntradaComunValidador.EsMensajeValido(nombreUsuario) || callback == null)
            {
                return;
            }

            _suscripciones[nombreUsuario] = callback;
        }

        /// <summary>
        /// Configura eventos de canal para limpieza automatica de suscripcion.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        public void ConfigurarEventosCanal(string nombreUsuario)
        {
            var canal = OperationContext.Current?.Channel;
            if (canal != null)
            {
                EventHandler manejadorClosed = null;
                EventHandler manejadorFaulted = null;

                manejadorClosed = delegate(object remitente, EventArgs argumentos)
                {
                    Desuscribir(nombreUsuario);
                };

                manejadorFaulted = delegate(object remitente, EventArgs argumentos)
                {
                    _logger.Warn("Canal fallado (Faulted). Desuscribiendo cliente.");
                    Desuscribir(nombreUsuario);
                };

                canal.Closed += manejadorClosed;
                canal.Faulted += manejadorFaulted;
            }
        }

        /// <summary>
        /// Elimina la suscripcion de un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        public void Desuscribir(string nombreUsuario)
        {
            if (!EntradaComunValidador.EsMensajeValido(nombreUsuario))
            {
                return;
            }

            TCallback valorDescartado;
            _suscripciones.TryRemove(nombreUsuario, out valorDescartado);
        }

        /// <summary>
        /// Intenta obtener el callback registrado para un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        /// <returns>Callback registrado si existe, null en caso contrario.</returns>
        public TCallback ObtenerCallback(string nombreUsuario)
        {
            TCallback callback;
            if (_suscripciones.TryGetValue(nombreUsuario, out callback))
            {
                return callback;
            }
            return null;
        }

        /// <summary>
        /// Obtiene el callback actual del contexto de operacion.
        /// </summary>
        /// <returns>Callback del contexto actual.</returns>
        /// <exception cref="FaultException">Se lanza si no se puede obtener el callback.
        /// </exception>
        public static TCallback ObtenerCallbackActual()
        {
            var contexto = OperationContext.Current;
            if (contexto != null)
            {
                var callback = contexto.GetCallbackChannel<TCallback>();
                if (callback != null)
                {
                    return callback;
                }
                
                throw new FaultException(MensajesError.Cliente.ErrorObtenerCallback);
            }

            throw new FaultException(MensajesError.Cliente.ErrorContextoOperacion);
        }
    }
}
