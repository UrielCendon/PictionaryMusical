using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.VistaModelo.Auxiliares;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace PictionaryMusicalCliente.VistaModelo.Perfil.Auxiliares
{
    /// <summary>
    /// Gestiona las redes sociales del perfil de usuario.
    /// </summary>
    public sealed class RedesSocialesManejador
    {
        private const string RedSocialInstagram = "Instagram";
        private const string RedSocialFacebook = "Facebook";
        private const string RedSocialX = "X";
        private const string RedSocialDiscord = "Discord";
        private const int LongitudMaximaRedSocial = 50;

        private readonly ICatalogoImagenesPerfil _catalogoPerfil;
        private readonly Dictionary<string, RedSocialItemVistaModelo> _redesPorNombre;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="RedesSocialesManejador"/>.
        /// </summary>
        /// <param name="catalogoPerfil">
        /// Catalogo de imagenes de perfil.
        /// </param>
        public RedesSocialesManejador(ICatalogoImagenesPerfil catalogoPerfil)
        {
            _catalogoPerfil = catalogoPerfil;
            RedesSociales = CrearRedesSociales();
            _redesPorNombre = RedesSociales.ToDictionary(
                r => r.Nombre,
                System.StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Obtiene la coleccion de redes sociales.
        /// </summary>
        public ObservableCollection<RedSocialItemVistaModelo> RedesSociales { get; }

        /// <summary>
        /// Establece el identificador de una red social.
        /// </summary>
        /// <param name="redSocial">Nombre de la red social.</param>
        /// <param name="valor">Identificador a establecer.</param>
        public void EstablecerIdentificador(string redSocial, string valor)
        {
            ResultadoOperacion<RedSocialItemVistaModelo> resultadoItem = 
                ObtenerItemRedSocial(redSocial);
            if (resultadoItem.Exitoso)
            {
                resultadoItem.Valor.Identificador = valor;
                resultadoItem.Valor.TieneError = false;
            }
        }

        /// <summary>
        /// Obtiene el identificador de una red social.
        /// </summary>
        /// <param name="redSocial">Nombre de la red social.</param>
        /// <returns>El identificador o cadena vacia si no existe.</returns>
        public string ObtenerIdentificador(string redSocial)
        {
            ResultadoOperacion<RedSocialItemVistaModelo> resultadoItem = 
                ObtenerItemRedSocial(redSocial);
            if (resultadoItem.Exitoso)
            {
                string valor = resultadoItem.Valor.Identificador?.Trim();
                return string.IsNullOrWhiteSpace(valor) ? string.Empty : valor;
            }

            return string.Empty;
        }

        private ResultadoOperacion<RedSocialItemVistaModelo> ObtenerItemRedSocial(string redSocial)
        {
            if (_redesPorNombre.TryGetValue(redSocial, out RedSocialItemVistaModelo item))
            {
                return ResultadoOperacion<RedSocialItemVistaModelo>.Exito(item);
            }

            return ResultadoOperacion<RedSocialItemVistaModelo>.Fallo();
        }

        /// <summary>
        /// Obtiene el identificador de Instagram.
        /// </summary>
        public string Instagram => ObtenerIdentificador(RedSocialInstagram);

        /// <summary>
        /// Obtiene el identificador de Facebook.
        /// </summary>
        public string Facebook => ObtenerIdentificador(RedSocialFacebook);

        /// <summary>
        /// Obtiene el identificador de X.
        /// </summary>
        public string X => ObtenerIdentificador(RedSocialX);

        /// <summary>
        /// Obtiene el identificador de Discord.
        /// </summary>
        public string Discord => ObtenerIdentificador(RedSocialDiscord);

        /// <summary>
        /// Valida todas las redes sociales.
        /// </summary>
        /// <returns>
        /// Tupla indicando si es valido y el mensaje de error.
        /// </returns>
        public (bool EsValido, string MensajeError) ValidarRedesSociales()
        {
            string primerMensaje = null;
            bool algunaInvalida = false;

            foreach (RedSocialItemVistaModelo item in RedesSociales)
            {
                bool esInvalida = ValidarRedSocialIndividual(
                    item, 
                    ref primerMensaje);

                if (esInvalida)
                {
                    algunaInvalida = true;
                }
            }

            return (!algunaInvalida, primerMensaje);
        }

        /// <summary>
        /// Limpia los errores de todas las redes sociales.
        /// </summary>
        public void LimpiarErrores()
        {
            foreach (RedSocialItemVistaModelo redSocial in RedesSociales)
            {
                redSocial.TieneError = false;
            }
        }

        /// <summary>
        /// Carga los valores de las redes sociales desde un perfil.
        /// </summary>
        /// <param name="redesSociales">Datos de las redes sociales.</param>
        public void CargarDesdeDTO(RedesSocialesDTO redesSociales)
        {
            if (redesSociales == null)
            {
                return;
            }

            EstablecerIdentificador(RedSocialInstagram, redesSociales.Instagram);
            EstablecerIdentificador(RedSocialFacebook, redesSociales.Facebook);
            EstablecerIdentificador(RedSocialX, redesSociales.X);
            EstablecerIdentificador(RedSocialDiscord, redesSociales.Discord);
        }

        private ObservableCollection<RedSocialItemVistaModelo> CrearRedesSociales()
        {
            return new ObservableCollection<RedSocialItemVistaModelo>
            {
                CrearRedSocial(RedSocialInstagram),
                CrearRedSocial(RedSocialFacebook),
                CrearRedSocial(RedSocialX),
                CrearRedSocial(RedSocialDiscord)
            };
        }

        private RedSocialItemVistaModelo CrearRedSocial(string nombre)
        {
            ImageSource icono = _catalogoPerfil.ObtenerIconoRedSocial(nombre);
            return new RedSocialItemVistaModelo(nombre, icono);
        }

        private static bool ValidarRedSocialIndividual(
            RedSocialItemVistaModelo item,
            ref string primerMensaje)
        {
            string valor = item.Identificador;

            if (string.IsNullOrWhiteSpace(valor))
            {
                item.TieneError = false;
                return false;
            }

            string normalizado = valor.Trim();
            bool excedeLongitud = normalizado.Length > LongitudMaximaRedSocial;

            item.TieneError = excedeLongitud;

            if (excedeLongitud)
            {
                primerMensaje ??= CrearMensajeErrorLongitud(item.Nombre);
            }

            return excedeLongitud;
        }

        private static string CrearMensajeErrorLongitud(string nombreRed)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Lang.errorTextoIdentificadorRedSocialLongitud,
                nombreRed,
                LongitudMaximaRedSocial);
        }
    }
}
