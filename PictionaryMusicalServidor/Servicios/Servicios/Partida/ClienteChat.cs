using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Representa un cliente conectado al chat con su nombre y callback.
    /// </summary>
    public sealed class ClienteChat
    {
        /// <summary>
        /// Inicializa una nueva instancia del cliente de chat.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador.</param>
        /// <param name="callback">Canal de comunicacion del cliente.</param>
        public ClienteChat(string nombreJugador, IChatManejadorCallback callback)
        {
            NombreJugador = nombreJugador;
            Callback = callback;
        }

        /// <summary>
        /// Nombre del jugador conectado.
        /// </summary>
        public string NombreJugador { get; }

        /// <summary>
        /// Canal de comunicacion para enviar mensajes al cliente.
        /// </summary>
        public IChatManejadorCallback Callback { get; set; }
    }
}
