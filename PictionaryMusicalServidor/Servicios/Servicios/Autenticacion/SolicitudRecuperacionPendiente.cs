using System;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Clase interna que representa una solicitud de recuperacion de cuenta pendiente.
    /// Almacena los datos temporales durante el proceso de recuperacion de contrasena.
    /// </summary>
    internal class SolicitudRecuperacionPendiente
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
        /// Codigo de verificacion generado.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Fecha y hora de expiracion del codigo.
        /// </summary>
        public DateTime Expira { get; set; }

        /// <summary>
        /// Indica si el codigo ha sido confirmado por el usuario.
        /// </summary>
        public bool Confirmado { get; set; }

        /// <summary>
        /// Idioma preferido del usuario para las notificaciones.
        /// </summary>
        public string Idioma { get; set; }
    }
}
