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
            Action navegarAlInicio)
        {
            _usuarioAutenticado = usuarioAutenticado ??
                throw new ArgumentNullException(nameof(usuarioAutenticado));

            InitializeComponent();

            _navegarAlInicio = navegarAlInicio ?? (() => Close());

            _vistaModelo = new TerminacionSesionVistaModelo(_usuarioAutenticado);
            _vistaModelo.OcultarDialogo = () => Close();
            _vistaModelo.EjecutarCierreSesionYNavegacion = EjecutarNavegacionSegura;

            DataContext = _vistaModelo;
        }

        private void EjecutarNavegacionSegura()
        {
            var ventanas = Application.Current.Windows.Cast<Window>().ToList();
            foreach (var v in ventanas)
            {
                if (v != this) v.Close();
            }

            _navegarAlInicio?.Invoke();
            Close();
        }
    }
}