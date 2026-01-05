namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Interfaz para expulsar jugadores de salas.
    /// Permite abstraer las operaciones de expulsion para facilitar pruebas unitarias.
    /// </summary>
    public interface ISalaExpulsor
    {
        /// <summary>
        /// Abandona una sala especifica.
        /// </summary>
        /// <param name="codigoSala">Codigo de la sala a abandonar.</param>
        /// <param name="nombreUsuario">Nombre del usuario que abandona.</param>
        void AbandonarSala(string codigoSala, string nombreUsuario);

        /// <summary>
        /// Banea a un jugador de una sala especifica.
        /// </summary>
        /// <param name="codigoSala">Codigo de la sala.</param>
        /// <param name="nombreUsuario">Nombre del usuario a banear.</param>
        void BanearJugador(string codigoSala, string nombreUsuario);
    }
}
