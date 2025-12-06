using System;
using System.Linq;
using System.Windows;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.VistaModelo.Sesion;

namespace PictionaryMusicalCliente.Vista
{
    /// <summary>
    /// Ventana de dialogo para confirmar y ejecutar el cierre de sesion.
    /// </summary>
    public partial class TerminacionSesion : Window
    {
        private readonly TerminacionSesionVistaModelo _vistaModelo;
        private readonly Action _navegarAlInicio;
        IUsuarioAutenticado _usuarioAutenticado;

        /// <summary>
        /// Inicializa la ventana.
        /// </summary>
        /// <param name="navegarAlInicio">Accion opcional para navegar al login.</param>
        public TerminacionSesion(IUsuarioAutenticado usuarioAutenticado,
            Action navegarAlInicio = null)
        {
            _usuarioAutenticado = usuarioAutenticado ??
                throw new ArgumentNullException(nameof(usuarioAutenticado));

            InitializeComponent();

            _navegarAlInicio = navegarAlInicio ?? EjecutarNavegacionInicioSesionPorDefecto;

            _vistaModelo = new TerminacionSesionVistaModelo(_usuarioAutenticado);
            _vistaModelo.OcultarDialogo = () => Close();
            _vistaModelo.EjecutarCierreSesionYNavegacion = _navegarAlInicio;

            DataContext = _vistaModelo;
        }

        private void EjecutarNavegacionInicioSesionPorDefecto()
        {
            var inicioSesion = new InicioSesion(); 
            inicioSesion.Show();
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