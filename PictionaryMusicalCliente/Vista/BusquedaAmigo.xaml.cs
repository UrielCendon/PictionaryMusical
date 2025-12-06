using System;
using System.Windows;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Amigos;

namespace PictionaryMusicalCliente
{
    /// <summary>
    /// Logica de interaccion para la ventana de busqueda y solicitud de amistad.
    /// </summary>
    public partial class BusquedaAmigo : Window
    {
        private readonly BusquedaAmigoVistaModelo _vistaModelo;

        /// <summary>
        /// Inicializa la ventana inyectando el servicio requerido.
        /// </summary>
        /// <param name="amigosServicio">Servicio de gestion de amigos ya configurado.</param>
        public BusquedaAmigo(IAmigosServicio amigosServicio)
        {
            if (amigosServicio == null)
            {
                throw new ArgumentNullException(nameof(amigosServicio));
            }

            InitializeComponent();

            _vistaModelo = new BusquedaAmigoVistaModelo(amigosServicio);
            DataContext = _vistaModelo;

            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            _vistaModelo.SolicitudEnviada += VistaModelo_SolicitudEnviada;
            _vistaModelo.Cancelado += VistaModelo_Cancelado;
            Closed += BuscarAmigo_Closed;
        }

        private void VistaModelo_SolicitudEnviada()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(VistaModelo_SolicitudEnviada);
                return;
            }
            Close();
        }

        private void VistaModelo_Cancelado()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(VistaModelo_Cancelado);
                return;
            }
            Close();
        }

        private void BuscarAmigo_Closed(object sender, EventArgs e)
        {
            Closed -= BuscarAmigo_Closed;
            if (_vistaModelo != null)
            {
                _vistaModelo.SolicitudEnviada -= VistaModelo_SolicitudEnviada;
                _vistaModelo.Cancelado -= VistaModelo_Cancelado;
            }
        }
    }
}