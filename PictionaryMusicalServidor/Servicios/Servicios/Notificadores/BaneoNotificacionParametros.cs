using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Contiene los parametros necesarios para notificar un baneo de sala.
    /// </summary>
    public class BaneoNotificacionParametros
    {
        /// <summary>
        /// Obtiene o establece el codigo de la sala donde ocurre el baneo.
        /// </summary>
        public string CodigoSala { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del jugador baneado.
        /// </summary>
        public string NombreBaneado { get; set; }

        /// <summary>
        /// Obtiene o establece el callback del jugador baneado para notificarle directamente.
        /// </summary>
        public ISalasManejadorCallback CallbackBaneado { get; set; }

        /// <summary>
        /// Obtiene o establece la informacion actualizada de la sala tras el baneo.
        /// </summary>
        public SalaDTO SalaActualizada { get; set; }
    }
}
