namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Contiene los nombres normalizados resultantes de la aceptacion de una solicitud.
    /// </summary>
    public class ResultadoAceptacionSolicitud
    {
        /// <summary>
        /// Nombre normalizado del usuario que envio la solicitud.
        /// </summary>
        public string NombreNormalizadoEmisor { get; set; }

        /// <summary>
        /// Nombre normalizado del usuario que acepto la solicitud.
        /// </summary>
        public string NombreNormalizadoReceptor { get; set; }
    }
}
