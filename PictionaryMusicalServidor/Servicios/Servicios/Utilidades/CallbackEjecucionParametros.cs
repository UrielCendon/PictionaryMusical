using System;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Contiene los parametros necesarios para ejecutar un callback de forma segura.
    /// </summary>
    internal class CallbackEjecucionParametros
    {
        /// <summary>
        /// Obtiene o establece el callback sobre el que ejecutar la accion.
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
