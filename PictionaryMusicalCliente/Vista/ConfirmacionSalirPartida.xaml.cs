using System.Windows;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Salas;

namespace PictionaryMusicalCliente.Vista
{
    /// <summary>
    /// Dialogo de confirmacion para abandonar la partida en curso.
    /// </summary>
    public partial class ConfirmacionSalirPartida : Window
    {
        private readonly ISonidoManejador _sonidoManejador;

        /// <summary>
        /// Inicializa el dialogo.
        /// </summary>
        public ConfirmacionSalirPartida(ISonidoManejador sonidoManejador)
        {
            InitializeComponent();
            _sonidoManejador = sonidoManejador;
        }

        private void BotonAceptarSalirPartida(object sender, RoutedEventArgs e)
        {
            _sonidoManejador?.ReproducirClick();
            DialogResult = true;
            Close();
        }

        private void BotonCancelarSalirPartida(object sender, RoutedEventArgs e)
        {
            _sonidoManejador?.ReproducirClick();
            DialogResult = false;
            Close();
        }
    }
}