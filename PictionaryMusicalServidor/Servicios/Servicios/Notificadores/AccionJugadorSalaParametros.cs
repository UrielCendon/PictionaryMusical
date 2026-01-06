using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Tipo de accion que se realiza sobre un jugador en una sala.
    /// </summary>
    public enum TipoAccionJugador
    {
        /// <summary>
        /// El jugador fue expulsado de la sala.
        /// </summary>
        Expulsion,

        /// <summary>
        /// El jugador fue baneado de la sala por reportes.
        /// </summary>
        Baneo
    }

    /// <summary>
    /// Contiene los parametros necesarios para notificar una accion sobre un jugador en una sala
    /// (expulsion o baneo).
    /// </summary>
    public class AccionJugadorSalaParametros
    {
        /// <summary>
        /// Obtiene o establece el tipo de accion realizada sobre el jugador.
        /// </summary>
        public TipoAccionJugador TipoAccion { get; set; }

        /// <summary>
        /// Obtiene o establece el codigo de la sala donde ocurre la accion.
        /// </summary>
        public string CodigoSala { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del jugador afectado.
        /// </summary>
        public string NombreJugadorAfectado { get; set; }

        /// <summary>
        /// Obtiene o establece el callback del jugador afectado para notificarle directamente.
        /// </summary>
        public ISalasManejadorCallback CallbackAfectado { get; set; }

        /// <summary>
        /// Obtiene o establece la informacion actualizada de la sala tras la accion.
        /// </summary>
        public SalaDTO SalaActualizada { get; set; }
    }
}
