using System;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Chat
{
    /// <summary>
    /// Implementacion delegada de la mensajeria que invoca una accion externa.
    /// </summary>
    public class ChatMensajeriaDelegado : IChatMensajeria
    {
        private readonly Action<string> _enviarMensaje;

        /// <summary>
        /// Inicializa una nueva instancia con la accion de envio.
        /// </summary>
        /// <param name="enviarMensaje">Accion a ejecutar al enviar.</param>
        /// <exception cref="ArgumentNullException">
        /// Si la accion de envio es nula.
        /// </exception>
        public ChatMensajeriaDelegado(Action<string> enviarMensaje)
        {
            _enviarMensaje = enviarMensaje ??
                throw new ArgumentNullException(nameof(enviarMensaje));
        }

        /// <inheritdoc />
        public void Enviar(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return;
            }

            _enviarMensaje(mensaje);
        }
    }
}
