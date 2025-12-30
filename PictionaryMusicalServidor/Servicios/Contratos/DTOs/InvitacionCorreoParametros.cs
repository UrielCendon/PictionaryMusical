namespace PictionaryMusicalServidor.Servicios.Contratos.DTOs
{
    /// <summary>
    /// Contiene los parametros necesarios para enviar una invitacion a una sala por correo.
    /// </summary>
    public class InvitacionCorreoParametros
    {
        /// <summary>
        /// Obtiene o establece la direccion de correo electronico del destinatario.
        /// </summary>
        public string CorreoDestino { get; set; }

        /// <summary>
        /// Obtiene o establece el codigo de la sala a la que se invita.
        /// </summary>
        public string CodigoSala { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario creador de la sala.
        /// </summary>
        public string Creador { get; set; }

        /// <summary>
        /// Obtiene o establece el idioma preferido para el contenido del correo.
        /// </summary>
        public string Idioma { get; set; }
    }
}
