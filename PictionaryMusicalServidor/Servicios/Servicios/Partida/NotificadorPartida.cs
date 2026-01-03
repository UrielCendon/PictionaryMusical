using System;
using System.Collections.Generic;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Servicio encargado de enviar notificaciones a los jugadores de una partida.
    /// Centraliza la logica de notificacion y manejo de errores de comunicacion.
    /// </summary>
    public class NotificadorPartida : INotificadorPartida
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(NotificadorPartida));

        /// <summary>
        /// Evento disparado cuando un callback debe ser removido por error de comunicacion.
        /// </summary>
        public event Action<string, string> CallbackInvalido;

        /// <summary>
        /// Notifica a todos los jugadores que la partida ha iniciado.
        /// </summary>
        public void NotificarPartidaIniciada(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks)
        {
            NotificarATodos(
                idSala,
                callbacks,
                callback => callback.NotificarPartidaIniciada());
        }

        /// <summary>
        /// Notifica a todos los jugadores que un jugador adivino.
        /// </summary>
        public void NotificarJugadorAdivino(NotificacionJugadorAdivinoParametros parametros)
        {
            NotificarATodos(
                parametros.IdSala,
                parametros.Callbacks,
                callback => callback.NotificarJugadorAdivino(
                    parametros.NombreJugador,
                    parametros.Puntos));
        }

        /// <summary>
        /// Notifica a todos los jugadores un mensaje de chat.
        /// </summary>
        public void NotificarMensajeChat(NotificacionMensajeChatParametros parametros)
        {
            NotificarATodos(
                parametros.IdSala,
                parametros.Callbacks,
                callback => callback.NotificarMensajeChat(
                    parametros.NombreJugador,
                    parametros.Mensaje));
        }

        /// <summary>
        /// Notifica a todos los jugadores un trazo dibujado.
        /// </summary>
        public void NotificarTrazoRecibido(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks,
            TrazoDTO trazo)
        {
            NotificarATodos(
                idSala,
                callbacks,
                callback => callback.NotificarTrazoRecibido(trazo));
        }

        /// <summary>
        /// Notifica a todos los jugadores el fin de una ronda.
        /// </summary>
        public void NotificarFinRonda(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks,
            bool tiempoAgotado)
        {
            NotificarATodos(
                idSala,
                callbacks,
                callback => callback.NotificarFinRonda(tiempoAgotado));
        }

        /// <summary>
        /// Notifica a todos los jugadores el fin de la partida.
        /// </summary>
        public void NotificarFinPartida(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks,
            ResultadoPartidaDTO resultado)
        {
            NotificarATodos(
                idSala,
                callbacks,
                callback => callback.NotificarFinPartida(resultado));
        }

        /// <summary>
        /// Notifica a todos los jugadores que deben limpiar el lienzo.
        /// </summary>
        public void NotificarLimpiarLienzo(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks)
        {
            var trazoLimpiar = new TrazoDTO
            {
                PuntosX = new double[0],
                PuntosY = new double[0],
                ColorHex = string.Empty,
                Grosor = 0,
                EsBorrado = true,
                EsLimpiarTodo = true
            };

            NotificarATodos(
                idSala,
                callbacks,
                callback => callback.NotificarTrazoRecibido(trazoLimpiar));
        }

        private void NotificarATodos(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks,
            Action<ICursoPartidaManejadorCallback> accion)
        {
            if (callbacks == null)
            {
                return;
            }

            var copiaCallbacks = new List<KeyValuePair<string, ICursoPartidaManejadorCallback>>
                (callbacks);

            foreach (var par in copiaCallbacks)
            {
                var parametrosCallback = new NotificacionCallbackParametros
                {
                    Callback = par.Value,
                    IdJugador = par.Key,
                    IdSala = idSala,
                    Accion = accion
                };
                NotificarCallbackSeguro(parametrosCallback);
            }
        }

        private void NotificarCallbackSeguro(NotificacionCallbackParametros parametros)
        {
            try
            {
                if (!EsCanalActivo(parametros.Callback))
                {
                    _logger.WarnFormat(
                        "Canal inactivo para jugador {0} en sala {1}. Removiendo.",
                        parametros.IdJugador,
                        parametros.IdSala);
                    DispararCallbackInvalido(parametros.IdSala, parametros.IdJugador);
                    return;
                }
                parametros.Accion(parametros.Callback);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Canal desechado para jugador {0} en sala {1}. Removiendo.",
                        parametros.IdJugador,
                        parametros.IdSala),
                    excepcion);
                DispararCallbackInvalido(parametros.IdSala, parametros.IdJugador);
            }
            catch (CommunicationObjectFaultedException excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Canal en falta para jugador {0} en sala {1}. Removiendo.",
                        parametros.IdJugador,
                        parametros.IdSala),
                    excepcion);
                DispararCallbackInvalido(parametros.IdSala, parametros.IdJugador);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Error comunicacion con jugador {0} en sala {1}. Removiendo.",
                        parametros.IdJugador,
                        parametros.IdSala),
                    excepcion);
                DispararCallbackInvalido(parametros.IdSala, parametros.IdJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Timeout con jugador {0} en sala {1}. Removiendo.",
                        parametros.IdJugador,
                        parametros.IdSala),
                    excepcion);
                DispararCallbackInvalido(parametros.IdSala, parametros.IdJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Error inesperado con jugador {0} en sala {1}. Removiendo.",
                        parametros.IdJugador,
                        parametros.IdSala),
                    excepcion);
                DispararCallbackInvalido(parametros.IdSala, parametros.IdJugador);
            }
        }

        private static bool EsCanalActivo(ICursoPartidaManejadorCallback callback)
        {
            var canal = callback as ICommunicationObject;
            if (canal != null)
            {
                return canal.State == CommunicationState.Opened;
            }
            return true;
        }

        private void DispararCallbackInvalido(string idSala, string idJugador)
        {
            CallbackInvalido?.Invoke(idSala, idJugador);
        }
    }
}
