using Datos.Modelo;

namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Contiene los datos resultantes de una operacion de eliminacion de amistad.
    /// </summary>
    public class ResultadoEliminacionAmistad
    {
        /// <summary>
        /// Relacion de amistad que fue eliminada.
        /// </summary>
        public Amigo Relacion { get; set; }

        /// <summary>
        /// Nombre del primer usuario normalizado.
        /// </summary>
        public string NombreANormalizado { get; set; }

        /// <summary>
        /// Nombre del segundo usuario normalizado.
        /// </summary>
        public string NombreBNormalizado { get; set; }
    }
}
