namespace PictionaryMusicalCliente.Modelo
{
    /// <summary>
    /// Agrupa los identificadores de redes sociales de un usuario.
    /// </summary>
    public sealed class ObjetoRedSocial
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="RedesSocialesDTO"/>.
        /// </summary>
        /// <param name="instagram">Identificador de Instagram.</param>
        /// <param name="facebook">Identificador de Facebook.</param>
        /// <param name="x">Identificador de X.</param>
        /// <param name="discord">Identificador de Discord.</param>
        public ObjetoRedSocial(
            string instagram,
            string facebook,
            string x,
            string discord)
        {
            Instagram = instagram ?? string.Empty;
            Facebook = facebook ?? string.Empty;
            X = x ?? string.Empty;
            Discord = discord ?? string.Empty;
        }

        /// <summary>
        /// Obtiene el identificador de Instagram.
        /// </summary>
        public string Instagram { get; }

        /// <summary>
        /// Obtiene el identificador de Facebook.
        /// </summary>
        public string Facebook { get; }

        /// <summary>
        /// Obtiene el identificador de X.
        /// </summary>
        public string X { get; }

        /// <summary>
        /// Obtiene el identificador de Discord.
        /// </summary>
        public string Discord { get; }
    }
}
