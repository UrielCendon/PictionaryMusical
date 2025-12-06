using System.Windows;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Ajustes;

namespace PictionaryMusicalCliente.Vista
{
    /// <summary>
    /// Ventana para modificar configuraciones especificas durante una partida en curso.
    /// </summary>
    public partial class AjustesPartida : Window
    {
        private readonly AjustesPartidaVistaModelo _vistaModelo;

        /// <summary>
        /// Inicializa la ventana de ajustes de partida.
        /// </summary>
        /// <param name="servicioCancion">Servicio para controlar el volumen de la música del 
        /// juego.</param>
        public AjustesPartida(ICancionManejador servicioCancion)
        {
            InitializeComponent();

            _vistaModelo = new AjustesPartidaVistaModelo(servicioCancion);

            _vistaModelo.OcultarVentana = () => Close();

            _vistaModelo.MostrarDialogoSalirPartida = () =>
            {
                var confirmacionSalirPartida = new ConfirmacionSalirPartida
                {
                    Owner = this
                };
                confirmacionSalirPartida.ShowDialog();
            };

            DataContext = _vistaModelo;
        }
    }
}