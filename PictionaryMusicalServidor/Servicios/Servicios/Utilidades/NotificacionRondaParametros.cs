using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Contiene los parametros necesarios para notificar el inicio de una ronda a un jugador.
    /// </summary>
    internal class NotificacionRondaParametros
    {
        /// <summary>
        /// Obtiene o establece el callback del jugador a notificar.
        /// </summary>
        public ICursoPartidaManejadorCallback Callback { get; set; }

        /// <summary>
        /// Obtiene o establece los datos de la ronda.
        /// </summary>
        public RondaDTO Ronda { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la sala.
        /// </summary>
        public string IdSala { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del jugador.
        /// </summary>
        public string IdJugador { get; set; }
    }
}
