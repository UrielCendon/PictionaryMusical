using System;
using System.ComponentModel;
using System.Windows;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Properties.Langs;

namespace PictionaryMusicalCliente.Utilidades.Idiomas
{
    /// <summary>
    /// Provee un contexto de enlace de datos para recursos de idioma en XAML.
    /// Permite la actualizacion dinamica de cadenas cuando cambia la cultura.
    /// </summary>
    public class LocalizacionContexto : INotifyPropertyChanged
    {
        private const string PropiedadIndexador = "Item[]";

        /// <summary>
        /// Inicializa una nueva instancia con un servicio de localizacion especifico.
        /// </summary>
        /// <param name="localizacionServicio">
        /// Servicio que notifica cuando cambia el idioma de la aplicacion.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="localizacionServicio"/> es nulo.
        /// </exception>
        public LocalizacionContexto(ILocalizacionServicio localizacionServicio)
        {
            if (localizacionServicio == null)
            {
                throw new ArgumentNullException(nameof(localizacionServicio));
            }

            SuscribirseACambiosDeIdioma(localizacionServicio);
        }

        /// <summary>
        /// Evento que notifica cambios en las propiedades enlazadas (cadenas de texto).
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Obtiene el recurso de cadena localizado para la clave especificada.
        /// </summary>
        /// <param name="clave">Clave del recurso en el archivo de recursos.</param>
        /// <returns>Texto localizado o cadena vacia si no se encuentra.</returns>
        public string this[string clave]
        {
            get
            {
                if (EsClaveInvalida(clave))
                {
                    return string.Empty;
                }

                return ObtenerValorLocalizado(clave);
            }
        }

        private void SuscribirseACambiosDeIdioma(ILocalizacionServicio localizacionServicio)
        {
            WeakEventManager<ILocalizacionServicio, EventArgs>.AddHandler(
                localizacionServicio,
                nameof(ILocalizacionServicio.IdiomaActualizado),
                AlActualizarseIdioma);
        }

        private static bool EsClaveInvalida(string clave)
        {
            return string.IsNullOrWhiteSpace(clave);
        }

        private static string ObtenerValorLocalizado(string clave)
        {
            string valor = Lang.ResourceManager.GetString(clave, Lang.Culture);

            return valor ?? string.Empty;
        }

        private void AlActualizarseIdioma(object remitente, EventArgs argumentosEvento)
        {
            NotificarCambioEnIndexador();
        }

        private void NotificarCambioEnIndexador()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropiedadIndexador));
        }
    }
}