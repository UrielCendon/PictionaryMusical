namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Contiene los parametros necesarios para notificar un mensaje de forma segura.
    /// </summary>
    internal class NotificacionMensajeSeguroParametros
    {
        /// <summary>
        /// Obtiene o establece el identificador de la sala.
        /// </summary>
        public string IdSala { get; set; }

        /// <summary>
        /// Obtiene o establece el cliente de chat destinatario.
        /// </summary>
        public ClienteChat Cliente { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del jugador que envia el mensaje.
        /// </summary>
        public string NombreJugador { get; set; }

        /// <summary>
        /// Obtiene o establece el contenido del mensaje.
        /// </summary>
        public string Mensaje { get; set; }
    }
}
