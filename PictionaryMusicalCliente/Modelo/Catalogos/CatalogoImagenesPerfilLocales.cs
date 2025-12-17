using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PictionaryMusicalCliente.Modelo.Catalogos
{
    /// <summary>
    /// Provee acceso centralizado a los recursos de imagen estaticos utilizados en el perfil.
    /// </summary>
    public class CatalogoImagenesPerfilLocales : ICatalogoImagenesPerfil
    {
        private static readonly ImageSource ImagenVacia = null;

        private static readonly IReadOnlyDictionary<string, ImageSource> IconosRedesSociales =
            new Dictionary<string, ImageSource>(StringComparer.OrdinalIgnoreCase)
            {
                ["Instagram"] = CrearImagen("instagram.png"),
                ["Facebook"] = CrearImagen("facebook.png"),
                ["X"] = CrearImagen("x_logo.png"),
                ["Discord"] = CrearImagen("discord.png")
            };

        /// <summary>
        /// Recupera el icono asociado a una red social especifica.
        /// </summary>
        /// <param name="nombre">Nombre clave de la red social.</param>
        /// <returns>
        /// El recurso grafico correspondiente, o un valor vacio si el nombre es invalido 
        /// o no existe la red social.
        /// </returns>
        public ImageSource ObtenerIconoRedSocial(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return ImagenVacia;
            }

            if (IconosRedesSociales.TryGetValue(nombre, out ImageSource icono))
            {
                return icono;
            }

            return ImagenVacia;
        }

        private static ImageSource CrearImagen(string archivo)
        {
            var uri = new Uri(
                $"pack://application:,,,/Recursos/{archivo}",
                UriKind.Absolute);
            return new BitmapImage(uri);
        }
    }
}