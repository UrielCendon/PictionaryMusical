namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Interfaz para el servicio de operaciones de amistad en base de datos.
    /// Define operaciones para ejecutar transacciones de base de datos relacionadas con 
    /// amistades.
    /// </summary>
    public interface IOperacionAmistadServicio
    {
        /// <summary>
        /// Obtiene los datos de un usuario para la suscripcion a notificaciones.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        /// <returns>Objeto con los datos del usuario para suscripcion.</returns>
        DatosSuscripcionUsuario ObtenerDatosUsuarioSuscripcion(string nombreUsuario);

        /// <summary>
        /// Ejecuta la creacion de una solicitud de amistad en la base de datos.
        /// </summary>
        /// <param name="nombreEmisor">Nombre del usuario emisor.</param>
        /// <param name="nombreReceptor">Nombre del usuario receptor.</param>
        /// <returns>Objeto con los usuarios emisor y receptor.</returns>
        ResultadoCreacionSolicitud EjecutarCreacionSolicitud(
            string nombreEmisor, 
            string nombreReceptor);

        /// <summary>
        /// Ejecuta la aceptacion de una solicitud de amistad en la base de datos.
        /// </summary>
        /// <param name="nombreEmisor">Nombre del usuario emisor.</param>
        /// <param name="nombreReceptor">Nombre del usuario receptor.</param>
        /// <returns>Objeto con los nombres normalizados de ambos usuarios.</returns>
        ResultadoAceptacionSolicitud EjecutarAceptacionSolicitud(
            string nombreEmisor, 
            string nombreReceptor);

        /// <summary>
        /// Ejecuta la eliminacion de una amistad en la base de datos.
        /// </summary>
        /// <param name="nombrePrimerUsuario">Nombre del primer usuario.</param>
        /// <param name="nombreSegundoUsuario">Nombre del segundo usuario.</param>
        /// <returns>Objeto con el resultado de la eliminacion.</returns>
        ResultadoEliminacionAmistad EjecutarEliminacion(
            string nombrePrimerUsuario, 
            string nombreSegundoUsuario);
    }
}
