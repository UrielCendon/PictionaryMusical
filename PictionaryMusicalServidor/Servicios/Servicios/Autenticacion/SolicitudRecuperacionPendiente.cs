namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Clase interna que representa una solicitud de recuperacion de cuenta pendiente.
    /// Almacena los datos temporales durante el proceso de recuperacion de contrasena.
    /// </summary>
    internal class SolicitudRecuperacionPendiente : SolicitudPendienteBase
    {
        /// <summary>
        /// Identificador del usuario que solicita la recuperacion.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Correo electronico del usuario.
        /// </summary>
        public string Correo { get; set; }

        /// <summary>
        /// Nombre de usuario de la cuenta.
        /// </summary>
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Indica si el codigo ha sido confirmado por el usuario.
        /// </summary>
        public bool Confirmado { get; set; }
    }
}
