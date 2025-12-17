using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Chat
{
    /// <summary>
    /// Define las operaciones necesarias para registrar puntuaciones en el juego.
    /// </summary>
    public interface IChatAciertosServicio
    {
        /// <summary>
        /// Obtiene el nombre del jugador que esta utilizando la sesion actual.
        /// </summary>
        /// <returns>Nombre del jugador.</returns>
        string ObtenerNombreJugadorActual();

        /// <summary>
        /// Registra los puntos ganados tanto por el adivinador como por el dibujante.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador que acerto.</param>
        /// <param name="puntosAdivinador">Puntos para quien adivino.</param>
        /// <param name="puntosDibujante">Puntos para quien estaba dibujando.</param>
        /// <returns>Tarea que representa la operacion asincrona.</returns>
        Task RegistrarAciertoAsync(
            string nombreJugador,
            int puntosAdivinador,
            int puntosDibujante);
    }
}
