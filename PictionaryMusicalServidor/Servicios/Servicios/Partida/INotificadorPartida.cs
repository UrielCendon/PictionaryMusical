using System;
using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Interfaz para el servicio de notificaciones de partida.
    /// Centraliza el envio de notificaciones a los jugadores conectados.
    /// </summary>
    public interface INotificadorPartida
    {
        /// <summary>
        /// Notifica a todos los jugadores que la partida ha iniciado.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="callbacks">Diccionario de callbacks de jugadores.</param>
        void NotificarPartidaIniciada(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks);

        /// <summary>
        /// Notifica a todos los jugadores que un jugador adivino.
        /// </summary>
        /// <param name="parametros">Parametros de la notificacion.</param>
        void NotificarJugadorAdivino(NotificacionJugadorAdivinoParametros parametros);

        /// <summary>
        /// Notifica a todos los jugadores un mensaje de chat.
        /// </summary>
        /// <param name="parametros">Parametros de la notificacion.</param>
        void NotificarMensajeChat(NotificacionMensajeChatParametros parametros);

        /// <summary>
        /// Notifica a todos los jugadores un trazo dibujado.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="callbacks">Diccionario de callbacks de jugadores.</param>
        /// <param name="trazo">Datos del trazo.</param>
        void NotificarTrazoRecibido(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks,
            TrazoDTO trazo);

        /// <summary>
        /// Notifica a todos los jugadores el fin de una ronda.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="callbacks">Diccionario de callbacks de jugadores.</param>
        /// <param name="tiempoAgotado">Indica si la ronda termino por tiempo agotado.</param>
        void NotificarFinRonda(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks,
            bool tiempoAgotado);

        /// <summary>
        /// Notifica a todos los jugadores el fin de la partida.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="callbacks">Diccionario de callbacks de jugadores.</param>
        /// <param name="resultado">Resultado de la partida.</param>
        void NotificarFinPartida(
            string idSala,
            Dictionary<string, ICursoPartidaManejadorCallback> callbacks,
            ResultadoPartidaDTO resultado);

        /// <summary>
        /// Evento disparado cuando un callback debe ser removido.
        /// </summary>
        event Action<string, string> CallbackInvalido;
    }
}
