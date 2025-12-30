using System;
using System.Collections.Generic;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Implementacion del gestor de callbacks para partidas.
    /// Centraliza la logica de gestion de callbacks de jugadores en partidas activas.
    /// </summary>
    internal sealed class GestorCallbacksPartida : IGestorCallbacksPartida
    {
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(GestorCallbacksPartida));

        private readonly Dictionary<string, 
            Dictionary<string, ICursoPartidaManejadorCallback>> _callbacksPorSala;

        private readonly object _sincronizacion;
        private readonly Action<string, string> _accionRemoverJugador;

        /// <summary>
        /// Inicializa una nueva instancia del gestor de callbacks.
        /// </summary>
        /// <param name="callbacksPorSala">Diccionario compartido de callbacks por sala.</param>
        /// <param name="sincronizacion">Objeto de sincronizacion compartido.</param>
        /// <param name="accionRemoverJugador">Accion para remover jugador del controlador.
        /// </param>
        public GestorCallbacksPartida(
            Dictionary<string, Dictionary<string, ICursoPartidaManejadorCallback>> callbacksPorSala,
            object sincronizacion,
            Action<string, string> accionRemoverJugador)
        {
            _callbacksPorSala = callbacksPorSala ?? 
                throw new ArgumentNullException(nameof(callbacksPorSala));
            _sincronizacion = sincronizacion ?? 
                throw new ArgumentNullException(nameof(sincronizacion));
            _accionRemoverJugador = accionRemoverJugador;
        }

        /// <inheritdoc/>
        public void RegistrarCallback(
            string idSala,
            string idJugador,
            ICursoPartidaManejadorCallback callback)
        {
            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacks;
                if (!_callbacksPorSala.TryGetValue(idSala, out callbacks))
                {
                    callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>(
                        StringComparer.OrdinalIgnoreCase);
                    _callbacksPorSala[idSala] = callbacks;
                }

                callbacks[idJugador] = callback;
            }

            ConfigurarEventosCanal(idSala, idJugador);
        }

        /// <inheritdoc/>
        public void RemoverCallback(string idSala, string idJugador)
        {
            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacks;
                if (_callbacksPorSala.TryGetValue(idSala, out callbacks))
                {
                    callbacks.Remove(idJugador);
                }
            }
        }

        /// <inheritdoc/>
        public ICursoPartidaManejadorCallback ObtenerCallback(string idSala, string idJugador)
        {
            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacks;
                if (_callbacksPorSala.TryGetValue(idSala, out callbacks))
                {
                    ICursoPartidaManejadorCallback callback;
                    if (callbacks.TryGetValue(idJugador, out callback))
                    {
                        return callback;
                    }
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public bool EsCanalActivo(ICursoPartidaManejadorCallback callback)
        {
            var canal = callback as ICommunicationObject;
            if (canal != null)
            {
                return canal.State == CommunicationState.Opened;
            }
            return true;
        }

        /// <inheritdoc/>
        public void EjecutarCallbackSeguro(CallbackEjecucionParametros parametros)
        {
            try
            {
                if (!EsCanalActivo(parametros.Callback))
                {
                    _logger.WarnFormat(
                        "Canal inactivo para jugador {0} en sala {1}. Removiendo.",
                        parametros.IdJugador, parametros.IdSala);
                    RemoverCallbackYJugador(parametros.IdSala, parametros.IdJugador);
                    return;
                }
                parametros.Accion(parametros.Callback);
            }
            catch (ObjectDisposedException excepcion)
            {
                ManejarErrorCallback(
                    "Canal desechado", 
                    parametros.IdJugador, 
                    parametros.IdSala, 
                    excepcion);
            }
            catch (CommunicationObjectFaultedException excepcion)
            {
                ManejarErrorCallback(
                    "Canal en falta", 
                    parametros.IdJugador, 
                    parametros.IdSala, 
                    excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejarErrorCallback(
                    "Error comunicacion", 
                    parametros.IdJugador, 
                    parametros.IdSala, 
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                ManejarErrorCallback(
                    "Timeout", 
                    parametros.IdJugador, 
                    parametros.IdSala, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                ManejarErrorCallback(
                    "Error inesperado", 
                    parametros.IdJugador, 
                    parametros.IdSala, 
                    excepcion);
            }
        }

        /// <inheritdoc/>
        public List<KeyValuePair<string, ICursoPartidaManejadorCallback>> ObtenerCallbacksSala(
            string idSala)
        {
            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacksSala;
                if (!_callbacksPorSala.TryGetValue(idSala, out callbacksSala))
                {
                    return new List<KeyValuePair<string, ICursoPartidaManejadorCallback>>();
                }
                return new List<KeyValuePair<string, ICursoPartidaManejadorCallback>>(callbacksSala);
            }
        }

        private void ConfigurarEventosCanal(string idSala, string idJugador)
        {
            var canal = OperationContext.Current?.Channel;
            if (canal != null)
            {
                EventHandler manejadorClosed = null;
                EventHandler manejadorFaulted = null;

                manejadorClosed = delegate(object remitente, EventArgs argumentos)
                {
                    RemoverCallbackYJugador(idSala, idJugador);
                };

                manejadorFaulted = delegate(object remitente, EventArgs argumentos)
                {
                    RemoverCallbackYJugador(idSala, idJugador);
                };

                canal.Closed += manejadorClosed;
                canal.Faulted += manejadorFaulted;
            }
        }

        private void RemoverCallbackYJugador(string idSala, string idJugador)
        {
            RemoverCallback(idSala, idJugador);
            _accionRemoverJugador?.Invoke(idSala, idJugador);
        }

        private void ManejarErrorCallback(
            string tipoError,
            string idJugador,
            string idSala,
            Exception excepcion)
        {
            _logger.Warn(
                string.Format("{0} para jugador {1} en sala {2}. Removiendo.",
                    tipoError, idJugador, idSala),
                excepcion);
            RemoverCallbackYJugador(idSala, idJugador);
        }
    }
}
