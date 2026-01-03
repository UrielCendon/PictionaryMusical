namespace PictionaryMusicalCliente.VistaModelo.InicioSesion.Auxiliares
{
    /// <summary>
    /// Encapsula los datos necesarios para la creación de una cuenta.
    /// </summary>
    public sealed class DatosCreacionCuenta
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DatosCreacionCuenta"/>.
        /// </summary>
        /// <param name="usuario">Nombre de usuario.</param>
        /// <param name="nombre">Nombre del usuario.</param>
        /// <param name="apellido">Apellido del usuario.</param>
        /// <param name="correo">Correo electrónico.</param>
        /// <param name="contrasena">Contraseña.</param>
        /// <param name="avatarId">ID del avatar seleccionado.</param>
        public DatosCreacionCuenta(
            string usuario,
            string nombre,
            string apellido,
            string correo,
            string contrasena,
            int avatarId)
        {
            Usuario = usuario ?? string.Empty;
            Nombre = nombre ?? string.Empty;
            Apellido = apellido ?? string.Empty;
            Correo = correo ?? string.Empty;
            Contrasena = contrasena ?? string.Empty;
            AvatarId = avatarId;
        }

        /// <summary>
        /// Obtiene el nombre de usuario.
        /// </summary>
        public string Usuario { get; }

        /// <summary>
        /// Obtiene el nombre del usuario.
        /// </summary>
        public string Nombre { get; }

        /// <summary>
        /// Obtiene el apellido del usuario.
        /// </summary>
        public string Apellido { get; }

        /// <summary>
        /// Obtiene el correo electrónico.
        /// </summary>
        public string Correo { get; }

        /// <summary>
        /// Obtiene la contraseña.
        /// </summary>
        public string Contrasena { get; }

        /// <summary>
        /// Obtiene el ID del avatar seleccionado.
        /// </summary>
        public int AvatarId { get; }
    }
}
