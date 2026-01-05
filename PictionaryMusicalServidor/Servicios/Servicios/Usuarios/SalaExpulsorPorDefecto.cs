using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Implementacion por defecto del expulsor de salas.
    /// Utiliza SalasManejador para realizar las operaciones de expulsion.
    /// </summary>
    public class SalaExpulsorPorDefecto : ISalaExpulsor
    {
        private readonly SalasManejador _salasManejador;

        /// <summary>
        /// Constructor que inicializa el manejador de salas.
        /// </summary>
        public SalaExpulsorPorDefecto()
        {
            _salasManejador = new SalasManejador();
        }

        /// <summary>
        /// Abandona una sala especifica usando SalasManejador.
        /// </summary>
        /// <param name="codigoSala">Codigo de la sala a abandonar.</param>
        /// <param name="nombreUsuario">Nombre del usuario que abandona.</param>
        public void AbandonarSala(string codigoSala, string nombreUsuario)
        {
            _salasManejador.AbandonarSala(codigoSala, nombreUsuario);
        }

        /// <summary>
        /// Banea a un jugador de una sala especifica usando SalasManejador.
        /// </summary>
        /// <param name="codigoSala">Codigo de la sala.</param>
        /// <param name="nombreUsuario">Nombre del usuario a banear.</param>
        public void BanearJugador(string codigoSala, string nombreUsuario)
        {
            _salasManejador.BanearJugador(codigoSala, nombreUsuario);
        }
    }
}
