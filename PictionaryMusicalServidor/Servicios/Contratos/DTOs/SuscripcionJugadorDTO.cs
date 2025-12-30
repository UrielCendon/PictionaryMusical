using System.Runtime.Serialization;

namespace PictionaryMusicalServidor.Servicios.Contratos.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos para la suscripcion de un jugador a una partida.
    /// </summary>
    [DataContract]
    public class SuscripcionJugadorDTO
    {
        /// <summary>
        /// Identificador de la sala.
        /// </summary>
        [DataMember]
        public string IdSala { get; set; }

        /// <summary>
        /// Identificador unico del jugador.
        /// </summary>
        [DataMember]
        public string IdJugador { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        [DataMember]
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Indica si el jugador es el host de la sala.
        /// </summary>
        [DataMember]
        public bool EsHost { get; set; }
    }
}
