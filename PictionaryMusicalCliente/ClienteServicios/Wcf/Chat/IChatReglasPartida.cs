using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Chat
{
    /// <summary>
    /// Define la logica de negocio para evaluar los mensajes del chat segun 
    /// el estado de la partida.
    /// </summary>
    public interface IChatReglasPartida
    {
        /// <summary>
        /// Obtiene o establece el nombre de la cancion que se debe adivinar.
        /// </summary>
        string NombreCancionCorrecta { get; set; }

        /// <summary>
        /// Obtiene o establece el tiempo restante de la ronda para el calculo de puntos.
        /// </summary>
        int TiempoRestante { get; set; }

        /// <summary>
        /// Evalua un mensaje para determinar si es un acierto, un mensaje normal o 
        /// si debe bloquearse.
        /// </summary>
        /// <param name="mensaje">Texto ingresado por el usuario.</param>
        /// <param name="esPartidaIniciada">Indica si la ronda esta activa.</param>
        /// <param name="esDibujante">Indica si el usuario actual es quien dibuja.</param>
        /// <returns>La decision tomada sobre el mensaje.</returns>
        Task<ChatDecision> EvaluarMensajeAsync(
            string mensaje,
            bool esPartidaIniciada,
            bool esDibujante);
    }
}
