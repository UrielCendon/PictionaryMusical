using System;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Contiene los parametros necesarios para notificar de forma segura a un callback.
    /// </summary>
    internal class NotificacionCallbackParametros
    {
        /// <summary>
        /// Obtiene o establece el callback del jugador.
        /// </summary>
        public ICursoPartidaManejadorCallback Callback { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del jugador.
        /// </summary>
        public string IdJugador { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la sala.
        /// </summary>
        public string IdSala { get; set; }

        /// <summary>
        /// Obtiene o establece la accion a ejecutar sobre el callback.
        /// </summary>
        public Action<ICursoPartidaManejadorCallback> Accion { get; set; }
    }
}
