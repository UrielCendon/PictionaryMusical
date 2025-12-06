using System;
using System.Windows;
using PictionaryMusicalCliente.VistaModelo.Salas;

namespace PictionaryMusicalCliente.Vista

{
    /// <summary>
    /// Lógica de interacción para ReportarJugador.xaml
    /// </summary>
    public partial class ReportarJugador : Window
    {
        private readonly ReportarJugadorVistaModelo _vistaModelo;

        public ReportarJugador(ReportarJugadorVistaModelo vistaModelo)
        {
            _vistaModelo = vistaModelo ?? throw new ArgumentNullException(nameof(vistaModelo));
            InitializeComponent();
            DataContext = _vistaModelo;
            _vistaModelo.Cerrar = resultado => DialogResult = resultado;

            Closed += ReportarJugador_Closed;
        }

        private void ReportarJugador_Closed(object sender, EventArgs e)
        {
            _vistaModelo.Cerrar = null;
            Closed -= ReportarJugador_Closed;
        }
    }
}