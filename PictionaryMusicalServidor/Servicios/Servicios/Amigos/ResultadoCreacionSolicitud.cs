using Datos.Modelo;

namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Contiene los datos resultantes de la creacion de una solicitud de amistad.
    /// </summary>
    public class ResultadoCreacionSolicitud
    {
        /// <summary>
        /// Usuario que envio la solicitud.
        /// </summary>
        public Usuario Emisor { get; set; }

        /// <summary>
        /// Usuario que recibira la solicitud.
        /// </summary>
        public Usuario Receptor { get; set; }
    }
}
