using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Contiene los parametros necesarios para notificar un mensaje de chat.
    /// </summary>
    public class NotificacionMensajeChatParametros
    {
        /// <summary>
        /// Obtiene o establece el identificador de la sala.
        /// </summary>
        public string IdSala { get; set; }

        /// <summary>
        /// Obtiene o establece el diccionario de callbacks de los jugadores.
        /// </summary>
        public Dictionary<string, ICursoPartidaManejadorCallback> Callbacks { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del jugador que envio el mensaje.
        /// </summary>
        public string NombreJugador { get; set; }

        /// <summary>
        /// Obtiene o establece el contenido del mensaje.
        /// </summary>
        public string Mensaje { get; set; }
    }
}
