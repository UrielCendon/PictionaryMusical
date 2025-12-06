using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Dialogos;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
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
    /// Ventana principal de acceso a la aplicacion. Actua como punto de entrada para el usuario.
    /// </summary>
    public partial class InicioSesion : Window
    {
        private readonly IMusicaManejador _servicioMusica;
        private readonly IWcfClienteEjecutor _ejecutor;
        private readonly IWcfClienteFabrica _fabrica;
        private readonly IManejadorErrorServicio _manejadorError;
        private readonly ILocalizadorServicio _traductor;
        private readonly IAvisoServicio _avisoServicio;
        private readonly ICatalogoAvatares _catalogoAvatares;

        /// <summary>
        /// Inicializa la ventana con todas las dependencias inyectadas desde el App.xaml.
        /// </summary>
        public InicioSesion(
            IMusicaManejador musicaManejador,
            IInicioSesionServicio inicioSesionServicio,
            ICambioContrasenaServicio cambioContrasenaServicio,
            IRecuperacionCuentaServicio recuperacionCuentaDialogo,
            ILocalizacionServicio localizacionServicio,
            Func<ISalasServicio> fabricaSalas,
            IWcfClienteFabrica fabricaClientes,
            IWcfClienteEjecutor ejecutor,
            IManejadorErrorServicio manejadorError,
            ILocalizadorServicio traductor,
            IAvisoServicio avisoServicio,
            ICatalogoAvatares catalogoAvatares)
        {
            InitializeComponent();

            _servicioMusica = musicaManejador
                ?? throw new ArgumentNullException(nameof(musicaManejador));
            _ejecutor = ejecutor ?? throw new ArgumentNullException(nameof(ejecutor));
            _fabrica = fabricaClientes ?? throw new ArgumentNullException(nameof(fabricaClientes));
            _manejadorError = manejadorError
                ?? throw new ArgumentNullException(nameof(manejadorError));
            _traductor = traductor ?? throw new ArgumentNullException(nameof(traductor));
            _avisoServicio = avisoServicio
                ?? throw new ArgumentNullException(nameof(avisoServicio));
            _catalogoAvatares = catalogoAvatares
                ?? throw new ArgumentNullException(nameof(catalogoAvatares));

            _servicioMusica.ReproducirEnBucle("inicio_sesion_musica.mp3");

            var vistaModelo = new InicioSesionVistaModelo(
                inicioSesionServicio,
                cambioContrasenaServicio,
                recuperacionCuentaDialogo,
                localizacionServicio,
                fabricaSalas);

            ConfigurarNavegacion(vistaModelo);
            ConfigurarInteraccion(vistaModelo);

            DataContext = vistaModelo;
        }

        private void ConfigurarNavegacion(InicioSesionVistaModelo vistaModelo)
        {
            vistaModelo.CerrarAccion = Close;

            vistaModelo.AbrirCrearCuenta = MostrarVentanaCreacionCuenta;

            vistaModelo.MostrarIngresoInvitado = vmInvitado =>
            {
                if (vmInvitado == null) return;
                var ventana = new IngresoPartidaInvitado(vmInvitado) { Owner = this };
                ventana.ShowDialog();
            };

            vistaModelo.AbrirVentanaJuegoInvitado = (sala, servicio, nombre) =>
            {
                if (sala == null || servicio == null) return;
                NavegarAVentanaJuego(sala, servicio, nombre);
            };

            vistaModelo.InicioSesionCompletado = _ =>
            {
                var ventanaPrincipal = new VentanaPrincipal();
                ventanaPrincipal.Show();
                Close();
            };
        }

        private void ConfigurarInteraccion(InicioSesionVistaModelo vistaModelo)
        {
            vistaModelo.MostrarCamposInvalidos = MarcarCamposInvalidos;
        }

        private void MostrarVentanaCreacionCuenta()
        {
            // Componemos los servicios necesarios para la ventana hija aqui mismo
            // usando la infraestructura inyectada.
            var codigoServicio = new VerificacionCodigoServicio(
                _ejecutor, _fabrica, _traductor, _manejadorError);

            var cuentaServicio = new CuentaServicio(_ejecutor, _fabrica, _manejadorError);

            var seleccionarAvatarDialogo = new SeleccionAvatarDialogoServicio(_avisoServicio,
                _catalogoAvatares);

            var verifCodigoDialogo = new VerificacionCodigoDialogoServicio();

            var vmCreacion = new CreacionCuentaVistaModelo(
                codigoServicio,
                cuentaServicio,
                seleccionarAvatarDialogo,
                verifCodigoDialogo,
                _avisoServicio);

            var ventana = new CreacionCuenta(vmCreacion) { Owner = this };
            ventana.ShowDialog();
        }

        private void NavegarAVentanaJuego(
            PictionaryMusicalServidor.Servicios.Contratos.DTOs.SalaDTO sala,
            ISalasServicio servicio,
            string nombre)
        {
            _servicioMusica.Detener();

            var ventanaJuego = new VentanaJuego(
                sala,
                servicio,
                esInvitado: true,
                nombreJugador: nombre,
                accionAlCerrar: () =>
                {
                    // Al cerrar el juego, volvemos a instanciar el inicio de sesion
                    // Nota: Aqui idealmente se usaria una fabrica de ventanas para no perder
                    // las dependencias, pero por simplicidad se instancia.
                    // Para produccion, se recomienda pasar una Func<InicioSesion>.
                    var nuevoInicio = new InicioSesion(
                        _servicioMusica,
                        (IInicioSesionServicio)((InicioSesionVistaModelo)DataContext)
                            .InicioSesionServicio,
                        ((InicioSesionVistaModelo)DataContext).CambioContrasenaServicio,
                        ((InicioSesionVistaModelo)DataContext).RecuperacionCuentaServicio,
                        ((InicioSesionVistaModelo)DataContext).LocalizacionServicio,
                        ((InicioSesionVistaModelo)DataContext).FabricaSalas,
                        _fabrica, _ejecutor, _manejadorError, _traductor,
                        _avisoServicio, _catalogoAvatares
                    );
                    nuevoInicio.Show();
                });

            ventanaJuego.Show();
            Close();
        }

        private void PasswordBoxChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is InicioSesionVistaModelo vistaModelo &&
                sender is PasswordBox passwordBox)
            {
                vistaModelo.EstablecerContrasena(passwordBox.Password);
            }
        }

        private void MarcarCamposInvalidos(IList<string> camposInvalidos)
        {
            ControlVisual.RestablecerEstadoCampo(campoTextoUsuario);
            ControlVisual.RestablecerEstadoCampo(campoContrasenaContrasena);

            if (camposInvalidos == null) return;

            foreach (string campo in camposInvalidos)
            {
                AplicarEstiloError(campo);
            }
        }

        private void AplicarEstiloError(string campo)
        {
            if (campo == nameof(InicioSesionVistaModelo.Identificador))
            {
                ControlVisual.MarcarCampoInvalido(campoTextoUsuario);
            }
            else if (campo == InicioSesionVistaModelo.CampoContrasena)
            {
                ControlVisual.MarcarCampoInvalido(campoContrasenaContrasena);
            }
        }

        private void InicioSesion_Cerrado(object sender, EventArgs e)
        {
            _servicioMusica.Detener();
            _servicioMusica.Dispose();
        }

        private void BotonAudio_Click(object sender, RoutedEventArgs e)
        {
            bool estaSilenciado = _servicioMusica.AlternarSilencio();
            ActualizarIconoAudio(estaSilenciado);
        }

        private void ActualizarIconoAudio(bool estaSilenciado)
        {
            string rutaImagen = estaSilenciado
                ? "/Recursos/Audio_Apagado.png"
                : "/Recursos/Audio_Encendido.png";

            imagenBotonAudio.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
        }
    }
}