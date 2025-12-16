namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Chat
{
    /// <summary>
    /// Define la capacidad de enviar mensajes al sistema de chat.
    /// </summary>
    public interface IChatMensajeria
    {
        /// <summary>
        /// Envia un mensaje de texto al canal de chat.
        /// </summary>
        /// <param name="mensaje">Contenido del mensaje.</param>
        void Enviar(string mensaje);
    }
}
