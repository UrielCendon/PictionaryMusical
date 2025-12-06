using System;
using System.Windows;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.VentanaPrincipal;

namespace PictionaryMusicalCliente.Vista
{
    /// <summary>
    /// Ventana que muestra la tabla de puntuaciones globales y estadisticas de jugadores.
    /// </summary>
    public partial class Clasificacion : Window
    {
        /// <summary>
        /// Inicializa la ventana de clasificacion inyectando el servicio requerido.
        /// </summary>
        /// <param name="clasificacionServicio">Servicio para obtener los datos del ranking.
        /// </param>
        public Clasificacion(IClasificacionServicio clasificacionServicio)
        {
            if (clasificacionServicio == null)
            {
                throw new ArgumentNullException(nameof(clasificacionServicio));
            }

            InitializeComponent();

            var vistaModelo = new ClasificacionVistaModelo(clasificacionServicio)
            {
                CerrarAccion = Close
            };

            DataContext = vistaModelo;
        }

        private async void Clasificacion_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (DataContext is ClasificacionVistaModelo vistaModelo)
            {
                await vistaModelo.CargarClasificacionAsync().ConfigureAwait(true);
            }
        }
    }
}