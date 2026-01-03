namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Contiene los datos necesarios para notificar una solicitud de amistad.
    /// </summary>
    public class DatosNotificacionSolicitud
    {
        /// <summary>
        /// Nombre del usuario emisor como fue ingresado originalmente.
        /// </summary>
        public string NombreEmisorOriginal { get; set; }

        /// <summary>
        /// Nombre del usuario receptor como fue ingresado originalmente.
        /// </summary>
        public string NombreReceptorOriginal { get; set; }
    }
}
