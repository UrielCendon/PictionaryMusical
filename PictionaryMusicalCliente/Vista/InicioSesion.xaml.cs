using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Dialogos;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Administrador;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.InicioSesion;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PictionaryMusicalCliente.Vista
{
    /// <summary>
    /// Ventana principal de acceso a la aplicacion.
    /// </summary>
    public partial class InicioSesion : Window
    {
        private readonly IMusicaManejador _musica;

        private readonly IWcfClienteEjecutor _ejecutor;
        private readonly IWcfClienteFabrica _fabrica;
        private readonly IManejadorErrorServicio _manejadorError;
        private readonly ILocalizadorServicio _traductor;
        private readonly IAvisoServicio _aviso;
        private readonly ICatalogoAvatares _avatares;
        private readonly ISonidoManejador _sonidos;
        private readonly IValidadorEntrada _validador;
        private readonly IUsuarioAutenticado _usuarioSesion;
        private readonly ILocalizacionServicio _idioma;
        private readonly ICatalogoImagenesPerfil _imagenesPerfil;
        private readonly IVerificacionCodigoDialogoServicio _verificacionCodigoDialogo;

        /// <summary>
        /// Inicializa la ventana recibiendo todas las dependencias del sistema.
        /// </summary>
        public InicioSesion(
            IMusicaManejador musica,
            IInicioSesionServicio inicioSesion,
            ICambioContrasenaServicio cambioPass,
            IRecuperacionCuentaServicio recuperacion,
            ILocalizacionServicio idioma,
            Func<ISalasServicio> fabricaSalas,
            IWcfClienteFabrica fabricaWcf,
            IWcfClienteEjecutor ejecutorWcf,
            IManejadorErrorServicio errorWcf,
            ILocalizadorServicio traductor,
            IAvisoServicio aviso,
            ICatalogoAvatares avatares,
            ISonidoManejador sonidos,
            IValidadorEntrada validador,
            IUsuarioAutenticado usuarioSesion,
            ICatalogoImagenesPerfil imagenesPerfil,
            IVerificacionCodigoDialogoServicio verificacionCodigoDialogo)
        {
            InitializeComponent();

            _musica = musica ?? throw new ArgumentNullException(nameof(musica));
            _ejecutor = ejecutorWcf;
            _fabrica = fabricaWcf;
            _manejadorError = errorWcf;
            _traductor = traductor;
            _aviso = aviso;
            _avatares = avatares;
            _sonidos = sonidos;
            _validador = validador;
            _usuarioSesion = usuarioSesion;
            _idioma = idioma;
            _imagenesPerfil = imagenesPerfil;
            _verificacionCodigoDialogo = verificacionCodigoDialogo;

            _musica.ReproducirEnBucle("inicio_sesion_musica.mp3");

            var vm = new InicioSesionVistaModelo(
                inicioSesion,
                cambioPass,
                recuperacion,
                idioma,
                traductor,
                aviso,
                sonidos,
                usuarioSesion,
                fabricaSalas);

            ConfigurarNavegacion(vm);
            vm.MostrarCamposInvalidos = MarcarCamposInvalidos;

            DataContext = vm;
        }

        private void ConfigurarNavegacion(InicioSesionVistaModelo vm)
        {
            vm.CerrarAccion = Close;

            vm.AbrirCrearCuenta = () =>
            {
                var codigoServ = new VerificacionCodigoServicio(
                    _ejecutor, _fabrica, _traductor, _manejadorError);
                var cuentaServ = new CuentaServicio(_ejecutor, _fabrica, _manejadorError);
                var selectAvatar = new SeleccionAvatarDialogoServicio(
                    _aviso, _avatares); 
                var verifCodigo = new VerificacionCodigoDialogoServicio();

                var vmCrear = new CreacionCuentaVistaModelo(
                    codigoServ, cuentaServ, selectAvatar, verifCodigo,
                    _sonidos, _validador, _avatares, _idioma);

                var ventana = new CreacionCuenta(vmCrear) { Owner = this };
                ventana.ShowDialog();
            };

            vm.InicioSesionCompletado = _ => NavegarAVentanaPrincipal();

            vm.MostrarIngresoInvitado = vmInvitado =>
            {
                if (vmInvitado == null) return;
                var ventana = new IngresoPartidaInvitado(vmInvitado) { Owner = this };
                ventana.ShowDialog();
            };

            vm.AbrirVentanaJuegoInvitado = (sala, servicio, nombre) =>
            {
                if (sala == null || servicio == null) return;
                NavegarAVentanaJuego(sala, servicio, nombre, true);
            };
        }

        private void NavegarAVentanaPrincipal()
        {
            var listaAmigos = new ListaAmigosServicio(_manejadorError, _fabrica);
            var amigos = new AmigosServicio(
                new SolicitudesAmistadAdministrador(), _manejadorError, _fabrica);
            var salas = new SalasServicio(_fabrica, _manejadorError);
            var perfil = new PerfilServicio(_ejecutor, _fabrica, _manejadorError);
            var clasif = new ClasificacionServicio(_ejecutor, _fabrica, _manejadorError);
            var cambioPass = new CambioContrasenaServicio(
                _ejecutor, _fabrica, _manejadorError, _traductor);
            var recup = new RecuperacionCuentaDialogoServicio(
                _verificacionCodigoDialogo, _aviso, _validador, _sonidos);
            var selectAvatar = new SeleccionAvatarDialogoServicio(_aviso, _avatares);
            var invitaciones = new InvitacionesServicio(
                _ejecutor, _fabrica, _manejadorError, _traductor);
            var reportes = new ReportesServicio(
                _ejecutor, _fabrica, _manejadorError, _traductor);

            var principal = new VentanaPrincipal(
                _musica, listaAmigos, amigos, salas, _idioma, _aviso, perfil, cambioPass,
                recup, selectAvatar, _avatares, clasif, _imagenesPerfil);

            principal.Show();
            Close();
        }

        private void NavegarAVentanaJuego(
            PictionaryMusicalServidor.Servicios.Contratos.DTOs.SalaDTO sala,
            ISalasServicio servicio,
            string nombre,
            bool esInvitado)
        {
            _musica.Detener();

            var invitaciones = new InvitacionesServicio(
                _ejecutor, _fabrica, _manejadorError, _traductor);
            var reportes = new ReportesServicio(
                _ejecutor, _fabrica, _manejadorError, _traductor);
            var perfil = new PerfilServicio(_ejecutor, _fabrica, _manejadorError);
            var listaAmigos = new ListaAmigosServicio(_manejadorError, _fabrica);

            var ventanaJuego = new Sala(
                sala, servicio, _aviso,
                esInvitado, nombre,
                () =>
                {
                    System.Diagnostics.Process.Start(
                        Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                });

            ventanaJuego.Show();
            Close();
        }

        private void PasswordBoxChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is InicioSesionVistaModelo vm && sender is PasswordBox pb)
            {
                vm.EstablecerContrasena(pb.Password);
            }
        }

        private void MarcarCamposInvalidos(IList<string> campos)
        {
            ControlVisual.RestablecerEstadoCampo(campoTextoUsuario);
            ControlVisual.RestablecerEstadoCampo(campoContrasenaContrasena);

            if (campos == null) return;

            foreach (var campo in campos)
            {
                if (campo == nameof(InicioSesionVistaModelo.Identificador))
                    ControlVisual.MarcarCampoInvalido(campoTextoUsuario);
                else if (campo == InicioSesionVistaModelo.CampoContrasena)
                    ControlVisual.MarcarCampoInvalido(campoContrasenaContrasena);
            }
        }

        private void InicioSesion_Cerrado(object sender, EventArgs e)
        {
            _musica.Detener();
            _musica.Dispose();
        }

        private void BotonAudio_Click(object sender, RoutedEventArgs e)
        {
            bool silenciado = _musica.AlternarSilencio();
            string ruta = silenciado ? "Audio_Apagado.png" : "Audio_Encendido.png";
            imagenBotonAudio.Source = new BitmapImage(
                new Uri($"/Recursos/{ruta}", UriKind.Relative));
        }
    }
}