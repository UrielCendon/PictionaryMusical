namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Interfaz generica para gestionar callbacks de servicios WCF.
    /// Define operaciones para suscribir, desuscribir y configurar canales de callback.
    /// </summary>
    /// <typeparam name="T">Tipo de callback a gestionar.</typeparam>
    public interface IManejadorCallback<T> where T : class
    {
        /// <summary>
        /// Registra una suscripcion de callback para un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario a suscribir.</param>
        /// <param name="callback">Callback a registrar para el usuario.</param>
        void Suscribir(string nombreUsuario, T callback);

        /// <summary>
        /// Elimina la suscripcion de callback de un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario a desuscribir.</param>
        void Desuscribir(string nombreUsuario);

        /// <summary>
        /// Configura los eventos del canal WCF para limpieza automatica de suscripcion.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario cuyo canal se configurara.</param>
        void ConfigurarEventosCanal(string nombreUsuario);

        /// <summary>
        /// Intenta obtener el callback registrado para un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        /// <returns>Callback registrado si existe, null en caso contrario.</returns>
        T ObtenerCallback(string nombreUsuario);
    }
}
