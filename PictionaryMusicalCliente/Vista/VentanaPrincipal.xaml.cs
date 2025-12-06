using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.VentanaPrincipal;
using PictionaryMusicalCliente.VistaModelo.Perfil;
using System;
using System.Windows;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Utilidades.Abstracciones;

namespace PictionaryMusicalCliente
{
    /// <summary>
    /// Ventana principal (Lobby) que gestiona la creacion de partidas y navegacion.
    /// </summary>
    public partial class VentanaPrincipal : Window
    {
        private readonly IMusicaManejador _musica;
        private readonly IListaAmigosServicio _listaAmigos;
        private readonly IAmigosServicio _amigos;
        private readonly ISalasServicio _salas;
        private readonly ILocalizacionServicio _idioma;
        private readonly IAvisoServicio _aviso;
        
        private readonly ICatalogoAvatares _avatares;
        private readonly ICatalogoImagenesPerfil _imagenesPerfil;
        private readonly IPerfilServicio _perfilServicio;
        private readonly ICambioContrasenaServicio _cambioPass;
        private readonly IRecuperacionCuentaServicio _recuperacion;
        private readonly ISeleccionarAvatarServicio _selectAvatar;
        private readonly IClasificacionServicio _clasificacion;

        private readonly VentanaPrincipalVistaModelo _vistaModelo;
        private bool _abrioVentanaJuego;

        /// <summary>
        /// Inicializa el Lobby con todas las dependencias requeridas.
        /// </summary>
        public VentanaPrincipal(
            IMusicaManejador musica,
            IListaAmigosServicio listaAmigos,
            IAmigosServicio amigos,
            ISalasServicio salas,
            ILocalizacionServicio idioma,
            IAvisoServicio aviso,
            IPerfilServicio perfilServicio,
            ICambioContrasenaServicio cambioPass,
            IRecuperacionCuentaServicio recuperacion,
            ISeleccionarAvatarServicio selectAvatar,
            ICatalogoAvatares avatares,
            IClasificacionServicio clasificacion
            )
        {
            InitializeComponent();

            _musica = musica ?? throw new ArgumentNullException(nameof(musica));
            _listaAmigos = listaAmigos ?? throw new ArgumentNullException(nameof(listaAmigos));
            _amigos = amigos ?? throw new ArgumentNullException(nameof(amigos));
            _salas = salas ?? throw new ArgumentNullException(nameof(salas));
            _idioma = idioma ?? throw new ArgumentNullException(nameof(idioma));
            _aviso = aviso ?? throw new ArgumentNullException(nameof(aviso));
            
            _perfilServicio = perfilServicio;
            _cambioPass = cambioPass;
            _recuperacion = recuperacion;
            _selectAvatar = selectAvatar;
            _avatares = avatares;
            _clasificacion = clasificacion;

            _musica.ReproducirEnBucle("ventana_principal_musica.mp3");

            _vistaModelo = new VentanaPrincipalVistaModelo(
                _idioma,
                _listaAmigos,
                _amigos,
                _salas);

            ConfigurarNavegacion();
            _vistaModelo.MostrarMensaje = _aviso.Mostrar;

            DataContext = _vistaModelo;

            Loaded += VentanaPrincipal_LoadedAsync;
            Closed += VentanaPrincipal_ClosedAsync;
        }

        private void ConfigurarNavegacion()
        {
            _vistaModelo.AbrirPerfil = AbrirPerfil;
            _vistaModelo.AbrirAjustes = AbrirAjustes;
            _vistaModelo.AbrirComoJugar = () => MostrarDialogo(new ComoJugar());
            _vistaModelo.AbrirClasificacion = () => 
                MostrarDialogo(new Clasificacion(_clasificacion));
            _vistaModelo.AbrirBuscarAmigo = () => 
                MostrarDialogo(new BusquedaAmigo(_amigos));
            _vistaModelo.AbrirSolicitudes = () => 
                MostrarDialogo(new Solicitudes(_amigos));
            _vistaModelo.ConfirmarEliminarAmigo = MostrarConfirmacionEliminar;
            _vistaModelo.IniciarJuego = MostrarVentanaJuego;
            _vistaModelo.UnirseSala = MostrarVentanaJuego;
        }

        private void AbrirPerfil()
        {
            var vmPerfil = new PerfilVistaModelo(
                _perfilServicio, 
                _selectAvatar, 
                _cambioPass, 
                _recuperacion);
            
            // Nota: SolicitarReinicioSesion deberia inyectarse o manejarse globalmente
            // vmPerfil.SolicitarReinicioSesion = ... 

            MostrarDialogo(new Perfil(vmPerfil));
        }

        private void AbrirAjustes()
        {
            MostrarDialogo(new Ajustes(_musica));
        }

        private async void VentanaPrincipal_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await _vistaModelo.InicializarAsync().ConfigureAwait(true);
        }

        private async void VentanaPrincipal_ClosedAsync(object sender, EventArgs e)
        {
            Loaded -= VentanaPrincipal_LoadedAsync;
            Closed -= VentanaPrincipal_ClosedAsync;

            await _vistaModelo.FinalizarAsync().ConfigureAwait(false);

            _listaAmigos?.Dispose();
            _amigos?.Dispose();

            if (!_abrioVentanaJuego)
            {
                _salas?.Dispose();
            }
            
            _musica.Detener();
        }

        private bool? MostrarConfirmacionEliminar(string amigo)
        {
            var ventana = new EliminacionAmigo(amigo) { Owner = this };
            return ventana.ShowDialog();
        }

        private void MostrarDialogo(Window ventana)
        {
            if (ventana == null) return;
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        private void MostrarVentanaJuego(DTOs.SalaDTO sala)
        {
            _musica.Detener();
            _abrioVentanaJuego = true;

            var ventanaJuego = new Sala(sala, _salas);
            ventanaJuego.Show();

            Close();
        }
    }
}