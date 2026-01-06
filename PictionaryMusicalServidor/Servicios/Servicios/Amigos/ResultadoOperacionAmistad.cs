namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Contiene los nombres normalizados de dos usuarios involucrados en una operacion de amistad.
    /// Sirve como clase base para resultados de operaciones de amistad y tambien puede usarse
    /// directamente para operaciones simples como aceptar solicitudes.
    /// </summary>
    public class ResultadoOperacionAmistad
    {
        /// <summary>
        /// Nombre normalizado del primer usuario (emisor en solicitudes).
        /// </summary>
        public string NombrePrimerUsuario { get; set; }

        /// <summary>
        /// Nombre normalizado del segundo usuario (receptor en solicitudes).
        /// </summary>
        public string NombreSegundoUsuario { get; set; }
    }
}
