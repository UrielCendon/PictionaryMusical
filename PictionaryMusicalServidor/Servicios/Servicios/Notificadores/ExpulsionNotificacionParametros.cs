using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Contiene los parametros necesarios para notificar una expulsion de sala.
    /// </summary>
    public class ExpulsionNotificacionParametros
    {
        /// <summary>
        /// Obtiene o establece el codigo de la sala donde ocurre la expulsion.
        /// </summary>
        public string CodigoSala { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del jugador expulsado.
        /// </summary>
        public string NombreExpulsado { get; set; }

        /// <summary>
        /// Obtiene o establece el callback del jugador expulsado para notificarle directamente.
        /// </summary>
        public ISalasManejadorCallback CallbackExpulsado { get; set; }

        /// <summary>
        /// Obtiene o establece la informacion actualizada de la sala tras la expulsion.
        /// </summary>
        public SalaDTO SalaActualizada { get; set; }
    }
}
