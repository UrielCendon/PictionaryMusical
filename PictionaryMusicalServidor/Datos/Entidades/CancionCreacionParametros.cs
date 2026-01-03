namespace PictionaryMusicalServidor.Datos.Entidades
{
    /// <summary>
    /// Encapsula los datos necesarios para crear una nueva cancion en el catalogo.
    /// </summary>
    internal class CancionCreacionParametros
    {
        /// <summary>
        /// Identificador unico de la cancion.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la cancion.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Artista o interprete de la cancion.
        /// </summary>
        public string Artista { get; set; }

        /// <summary>
        /// Genero musical de la cancion.
        /// </summary>
        public string Genero { get; set; }

        /// <summary>
        /// Idioma de la cancion.
        /// </summary>
        public string Idioma { get; set; }
    }
}
