namespace PictionaryMusicalServidor.Servicios.Contratos.DTOs
{
    /// <summary>
    /// Contiene los parametros necesarios para enviar una notificacion de codigo de verificacion.
    /// </summary>
    public class NotificacionCodigoParametros
    {
        /// <summary>
        /// Obtiene o establece la direccion de correo electronico del destinatario.
        /// </summary>
        public string CorreoDestino { get; set; }

        /// <summary>
        /// Obtiene o establece el codigo de verificacion a enviar.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario destinatario.
        /// </summary>
        public string UsuarioDestino { get; set; }

        /// <summary>
        /// Obtiene o establece el idioma para el contenido del correo.
        /// </summary>
        public string Idioma { get; set; }
    }
}
