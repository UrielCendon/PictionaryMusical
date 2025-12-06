using System;
using System.Linq;
using System.Windows;
using PictionaryMusicalCliente.VistaModelo.Sesion;

namespace PictionaryMusicalCliente
{
    /// <summary>
    /// Ventana de dialogo para confirmar y ejecutar el cierre de sesion.
    /// </summary>
    public partial class TerminacionSesion : Window
    {
        private readonly TerminacionSesionVistaModelo _vistaModelo;
        private readonly Action _navegarAlInicio;

        /// <summary>
        /// Inicializa la ventana.
        /// </summary>
        /// <param name="navegarAlInicio">Accion opcional para navegar al login.</param>
        public TerminacionSesion(Action navegarAlInicio = null)
        {
            InitializeComponent();

            _navegarAlInicio = navegarAlInicio ?? EjecutarNavegacionInicioSesionPorDefecto;

            _vistaModelo = new TerminacionSesionVistaModelo();
            _vistaModelo.OcultarDialogo = () => Close();
            _vistaModelo.EjecutarCierreSesionYNavegacion = _navegarAlInicio;

            DataContext = _vistaModelo;
        }

        private void EjecutarNavegacionInicioSesionPorDefecto()
        {
            // Nota: Esto fallara si InicioSesion no tiene constructor vacio.
            // Por arquitectura, se recomienda pasar la accion desde quien abre esta ventana.
            // Para mantener compatibilidad con tu codigo actual, lo dejo comentado como advertencia.
            // var inicioSesion = new InicioSesion(...); 
            // inicioSesion.Show();
            var ventanasACerrar = Application.Current.Windows
                .Cast<Window>()
                .ToList();

            foreach (var ventana in ventanasACerrar)
            {
                ventana.Close();
            }
        }
    }
}