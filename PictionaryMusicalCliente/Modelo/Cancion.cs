namespace PictionaryMusicalCliente.Modelo
{
    /// <summary>
    /// Representa una cancion del catalogo de audio de la partida.
    /// </summary>
    public sealed class Cancion
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Cancion"/>.
        /// </summary>
        /// <param name="id">Identificador unico de la cancion.</param>
        /// <param name="nombre">Nombre de la cancion.</param>
        /// <param name="archivo">Nombre del archivo de audio.</param>
        /// <param name="idioma">Idioma de la cancion.</param>
        public Cancion(int id, string nombre, string archivo, string idioma)
        {
            Id = id;
            Nombre = nombre;
            Archivo = archivo;
            Idioma = idioma;
        }

        /// <summary>
        /// Obtiene el identificador unico de la cancion.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Obtiene el nombre de la cancion.
        /// </summary>
        public string Nombre { get; }

        /// <summary>
        /// Obtiene el nombre del archivo de audio.
        /// </summary>
        public string Archivo { get; }

        /// <summary>
        /// Obtiene el idioma de la cancion.
        /// </summary>
        public string Idioma { get; }
    }
}
