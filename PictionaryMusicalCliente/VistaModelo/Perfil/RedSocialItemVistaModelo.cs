using System;
using System.ComponentModel;
using System.Windows.Media;

namespace PictionaryMusicalCliente.VistaModelo.Perfil
{
    /// <summary>
    /// Representa un elemento de red social editable en el perfil del usuario.
    /// </summary>
    public class RedSocialItemVistaModelo : INotifyPropertyChanged
    {
        private string _identificador;
        private bool _tieneError;

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="RedSocialItemVistaModelo"/>.
        /// </summary>
        /// <param name="nombre">
        /// Nombre de la red social. No puede ser nulo.
        /// </param>
        /// <param name="icono">
        /// Imagen del icono representativo de la red social.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="nombre"/> es nulo.
        /// </exception>
        public RedSocialItemVistaModelo(string nombre, ImageSource icono)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            RutaIcono = icono;
        }

        /// <summary>
        /// Evento que se dispara cuando una propiedad cambia.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Obtiene el nombre de la red social.
        /// </summary>
        public string Nombre { get; }

        /// <summary>
        /// Obtiene la imagen del icono de la red social.
        /// </summary>
        public ImageSource RutaIcono { get; }

        /// <summary>
        /// Obtiene o establece el identificador del usuario en esta red social.
        /// </summary>
        public string Identificador
        {
            get => _identificador;
            set
            {
                if (_identificador != value)
                {
                    _identificador = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Identificador)));
                }
            }
        }

        /// <summary>
        /// Obtiene o establece si el campo tiene un error de validacion.
        /// </summary>
        public bool TieneError
        {
            get => _tieneError;
            set
            {
                if (_tieneError != value)
                {
                    _tieneError = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(TieneError)));
                }
            }
        }
    }
}
