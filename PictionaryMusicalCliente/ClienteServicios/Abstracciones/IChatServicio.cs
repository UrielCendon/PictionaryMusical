using System;
using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Abstracciones
{
    /// <summary>
    /// Gestiona la conexion y comunicacion en tiempo real del chat de la sala.
    /// </summary>
    public interface IChatServicio
    {
        /// <summary>
        /// Se dispara cuando se recibe un nuevo mensaje de chat desde el servidor.
        /// Parametros: NombreJugador, Mensaje.
        /// </summary>
        event Action<string, string> MensajeRecibido;

        /// <summary>
        /// Se dispara cuando un jugador entra a la sala de chat.
        /// Parametro: NombreJugador.
        /// </summary>
        event Action<string> JugadorIngreso;

        /// <summary>
        /// Se dispara cuando un jugador sale de la sala de chat.
        /// Parametro: NombreJugador.
        /// </summary>
        event Action<string> JugadorSalio;

        /// <summary>
        /// Conecta al usuario al canal de chat de una sala especifica.
        /// </summary>
        /// <param name="idSala">Codigo de la sala.</param>
        /// <param name="nombreJugador">Nombre del usuario.</param>
        Task ConectarAsync(string idSala, string nombreJugador);

        /// <summary>
        /// Envia un mensaje de texto al chat de la sala.
        /// </summary>
        /// <param name="idSala">Codigo de la sala.</param>
        /// <param name="mensaje">Contenido del mensaje.</param>
        /// <param name="nombreJugador">Nombre del usuario que envia.</param>
        Task EnviarMensajeAsync(string idSala, string mensaje, string nombreJugador);

        /// <summary>
        /// Desconecta al usuario del canal de chat.
        /// </summary>
        /// <param name="idSala">Codigo de la sala.</param>
        /// <param name="nombreJugador">Nombre del usuario.</param>
        Task DesconectarAsync(string idSala, string nombreJugador);
    }
}