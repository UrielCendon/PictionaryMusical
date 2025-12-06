using System;
using System.Windows;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Amigos;

namespace PictionaryMusicalCliente.Vista
{
    /// <summary>
    /// Ventana para gestionar las solicitudes de amistad pendientes.
    /// </summary>
    public partial class Solicitudes : Window
    {
        private readonly SolicitudesVistaModelo _vistaModelo;

        /// <summary>
        /// Inicializa la ventana inyectando el servicio de amigos.
        /// </summary>
        /// <param name="amigosServicio">Servicio de gestion de amigos ya instanciado.</param>
        public Solicitudes(IAmigosServicio amigosServicio)
        {
            if (amigosServicio == null)
            {
                throw new ArgumentNullException(nameof(amigosServicio));
            }

            InitializeComponent();

            _vistaModelo = new SolicitudesVistaModelo(amigosServicio);
            DataContext = _vistaModelo;

            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            _vistaModelo.Cerrar += VistaModelo_Cerrar;
            Closed += Solicitudes_Closed;
        }

        private void VistaModelo_Cerrar()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(VistaModelo_Cerrar);
                return;
            }
            Close();
        }

        private void Solicitudes_Closed(object sender, EventArgs e)
        {
            Closed -= Solicitudes_Closed;
            if (_vistaModelo != null)
            {
                _vistaModelo.Cerrar -= VistaModelo_Cerrar;
                _vistaModelo.Dispose();
            }
        }
    }
}