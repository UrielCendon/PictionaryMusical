using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Contiene los parametros necesarios para notificar que un jugador adivino.
    /// </summary>
    public class NotificacionJugadorAdivinoParametros
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
        /// Obtiene o establece el nombre del jugador que adivino.
        /// </summary>
        public string NombreJugador { get; set; }

        /// <summary>
        /// Obtiene o establece los puntos obtenidos por adivinar.
        /// </summary>
        public int Puntos { get; set; }
    }
}
