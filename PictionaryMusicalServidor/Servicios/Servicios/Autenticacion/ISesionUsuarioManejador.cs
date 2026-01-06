namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Interfaz para la gestion de sesiones activas de usuarios.
    /// Permite registrar, verificar y eliminar sesiones para prevenir inicios de sesion 
    /// duplicados.
    /// </summary>
    public interface ISesionUsuarioManejador
    {
        /// <summary>
        /// Verifica si un usuario tiene una sesion activa.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        /// <returns>True si el usuario tiene una sesion activa.</returns>
        bool TieneSesionActiva(int usuarioId);

        /// <summary>
        /// Intenta registrar una nueva sesion para el usuario.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        /// <param name="nombreUsuario">Nombre del usuario para registro.</param>
        /// <returns>True si la sesion fue registrada exitosamente, false si ya existe una sesion 
        /// activa.</returns>
        bool IntentarRegistrarSesion(int usuarioId, string nombreUsuario);

        /// <summary>
        /// Elimina la sesion activa de un usuario.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        void EliminarSesion(int usuarioId);

        /// <summary>
        /// Elimina la sesion activa de un usuario por nombre de usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        void EliminarSesionPorNombre(string nombreUsuario);

        /// <summary>
        /// Obtiene el numero total de sesiones activas.
        /// </summary>
        /// <returns>Numero de sesiones activas.</returns>
        int ObtenerConteoSesiones();
    }
}
