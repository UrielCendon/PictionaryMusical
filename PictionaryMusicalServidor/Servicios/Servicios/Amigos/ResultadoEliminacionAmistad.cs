using Datos.Modelo;

namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Contiene los datos resultantes de una operacion de eliminacion de amistad.
    /// Hereda los nombres normalizados de ResultadoOperacionAmistad.
    /// </summary>
    public class ResultadoEliminacionAmistad : ResultadoOperacionAmistad
    {
        /// <summary>
        /// Relacion de amistad que fue eliminada.
        /// </summary>
        public Amigo Relacion { get; set; }
    }
}
