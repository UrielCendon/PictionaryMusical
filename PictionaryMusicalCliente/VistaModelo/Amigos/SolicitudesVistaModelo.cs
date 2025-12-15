using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalCliente.Utilidades;

namespace PictionaryMusicalCliente.VistaModelo.Amigos
{
    /// <summary>
    /// Gestiona la visualizacion y respuesta a las solicitudes de amistad pendientes.
    /// </summary>
    /// <remarks>
    /// Permite aceptar o rechazar solicitudes de amistad recibidas
    /// y cancelar solicitudes enviadas.
    /// </remarks>
    public sealed class SolicitudesVistaModelo : BaseVistaModelo, IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAmigosServicio _amigosServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _avisoServicio;
        private readonly IUsuarioAutenticado _usuarioSesion;
        private readonly string _usuarioActual;
        private bool _estaProcesando;

        /// <summary>
        /// Inicializa una nueva instancia de la clase.
        /// </summary>
        /// <param name="ventana">Servicio para gestionar ventanas.</param>
        /// <param name="localizador">Servicio de localizacion de mensajes.</param>
        /// <param name="amigosServicio">Servicio para operaciones de amigos.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        /// <param name="avisoServicio">Servicio para mostrar avisos.</param>
        /// <param name="usuarioSesion">Datos del usuario autenticado.</param>
        /// <exception cref="ArgumentNullException">
        /// Si algun parametro requerido es nulo.
        /// </exception>
        public SolicitudesVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            IAmigosServicio amigosServicio,
            SonidoManejador sonidoManejador,
            IAvisoServicio avisoServicio,
            IUsuarioAutenticado usuarioSesion)
            : base(ventana, localizador)
        {
            _amigosServicio = amigosServicio ??
                throw new ArgumentNullException(nameof(amigosServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            _avisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            _usuarioSesion = usuarioSesion ?? 
                throw new ArgumentNullException(nameof(usuarioSesion));
            _usuarioActual = _usuarioSesion.NombreUsuario ?? string.Empty;

            Solicitudes = new ObservableCollection<SolicitudAmistadEntrada>();

            AceptarSolicitudComando = new ComandoAsincrono(async param =>
            {
                _sonidoManejador.ReproducirClick();
                await ResponderSolicitudAsync(param as SolicitudAmistadEntrada);
            }, param => PuedeAceptar(param as SolicitudAmistadEntrada));

            RechazarSolicitudComando = new ComandoAsincrono(async param =>
            {
                _sonidoManejador.ReproducirClick();
                await RechazarSolicitudAsync(param as SolicitudAmistadEntrada);
            }, param => PuedeRechazar(param as SolicitudAmistadEntrada));

            CerrarComando = new ComandoDelegado(_ =>
            {
                _sonidoManejador.ReproducirClick();
                _ventana.CerrarVentana(this);
            });

            _amigosServicio.SolicitudesActualizadas += SolicitudesActualizadas;
            ActualizarSolicitudes(_amigosServicio.SolicitudesPendientes);
        }

        /// <summary>
        /// Coleccion de solicitudes de amistad pendientes.
        /// </summary>
        public ObservableCollection<SolicitudAmistadEntrada> Solicitudes { get; }

        /// <summary>
        /// Comando para aceptar una solicitud de amistad.
        /// </summary>
        public IComandoAsincrono AceptarSolicitudComando { get; }

        /// <summary>
        /// Comando para rechazar una solicitud de amistad.
        /// </summary>
        public IComandoAsincrono RechazarSolicitudComando { get; }

        /// <summary>
        /// Comando para cerrar la ventana.
        /// </summary>
        public ICommand CerrarComando { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _amigosServicio.SolicitudesActualizadas -= SolicitudesActualizadas;
        }

        private bool PuedeAceptar(SolicitudAmistadEntrada entrada)
        {
            return !EstaProcesando
                && entrada != null
                && entrada.PuedeAceptar;
        }

        private bool PuedeRechazar(SolicitudAmistadEntrada entrada)
        {
            return !EstaProcesando
                && entrada != null;
        }

        private bool EstaProcesando
        {
            get => _estaProcesando;
            set
            {
                if (EstablecerPropiedad(ref _estaProcesando, value))
                {
                    AceptarSolicitudComando?.NotificarPuedeEjecutar();
                    RechazarSolicitudComando?.NotificarPuedeEjecutar();
                }
            }
        }

        private void SolicitudesActualizadas(
            object sender,
            IReadOnlyCollection<DTOs.SolicitudAmistadDTO> solicitudes)
        {
            EjecutarEnDispatcher(() => ActualizarSolicitudes(solicitudes));
        }

        private void ActualizarSolicitudes(
            IReadOnlyCollection<DTOs.SolicitudAmistadDTO> solicitudes)
        {
            if (Solicitudes == null)
            {
                return;
            }

            LimpiarSolicitudesActuales();

            if (solicitudes == null)
            {
                return;
            }

            AgregarSolicitudesValidas(solicitudes);
        }

        private void LimpiarSolicitudesActuales()
        {
            Solicitudes.Clear();
        }

        private void AgregarSolicitudesValidas(
            IReadOnlyCollection<DTOs.SolicitudAmistadDTO> solicitudes)
        {
            foreach (var solicitud in solicitudes)
            {
                if (!EsSolicitudValida(solicitud))
                {
                    continue;
                }

                if (IntentarCrearEntradaSolicitud(solicitud, out var entrada))
                {
                    Solicitudes.Add(entrada);
                }
            }
        }

        private static bool EsSolicitudValida(DTOs.SolicitudAmistadDTO solicitud)
        {
            return solicitud != null && !solicitud.SolicitudAceptada;
        }

        private bool IntentarCrearEntradaSolicitud(
            DTOs.SolicitudAmistadDTO solicitud,
            out SolicitudAmistadEntrada entrada)
        {
            entrada = null;

            bool esEmisorActual = EsUsuarioActual(solicitud.UsuarioEmisor);
            bool esReceptorActual = EsUsuarioActual(solicitud.UsuarioReceptor);

            if (!esEmisorActual && !esReceptorActual)
            {
                return false;
            }

            string nombreMostrado = ObtenerNombreMostrado(
                solicitud, 
                esEmisorActual);

            if (string.IsNullOrWhiteSpace(nombreMostrado))
            {
                return false;
            }

            bool puedeAceptar = esReceptorActual;

            entrada = new SolicitudAmistadEntrada(
                solicitud,
                nombreMostrado,
                puedeAceptar);

            return true;
        }

        private bool EsUsuarioActual(string nombreUsuario)
        {
            return string.Equals(
                nombreUsuario,
                _usuarioActual,
                StringComparison.OrdinalIgnoreCase);
        }

        private static string ObtenerNombreMostrado(
            DTOs.SolicitudAmistadDTO solicitud,
            bool esEmisorActual)
        {
            string nombre = esEmisorActual
                ? solicitud.UsuarioReceptor
                : solicitud.UsuarioEmisor;

            return nombre?.Trim();
        }

        private async Task ResponderSolicitudAsync(SolicitudAmistadEntrada entrada)
        {
            if (!ValidarEntrada(entrada, "responder"))
            {
                return;
            }

            EstaProcesando = true;

            await EjecutarOperacionAsync(
                async () => await EjecutarAceptacionAsync(entrada),
                excepcion => ManejarErrorAceptacion(excepcion));

            EstaProcesando = false;
        }

        private async Task EjecutarAceptacionAsync(SolicitudAmistadEntrada entrada)
        {
            _logger.InfoFormat(
                "Aceptando solicitud de amistad de: {0}",
                entrada.Solicitud.UsuarioEmisor);

            await _amigosServicio.ResponderSolicitudAsync(
                entrada.Solicitud.UsuarioEmisor,
                entrada.Solicitud.UsuarioReceptor).ConfigureAwait(true);

            NotificarExitoAceptacion();
        }

        private void NotificarExitoAceptacion()
        {
            _sonidoManejador.ReproducirNotificacion();
            _avisoServicio.Mostrar(Lang.amigosTextoSolicitudAceptada);
        }

        private void ManejarErrorAceptacion(Exception excepcion)
        {
            _logger.Error("Error al aceptar solicitud de amistad.", excepcion);
            _sonidoManejador.ReproducirError();
            string mensaje = excepcion.Message ?? Lang.errorTextoErrorProcesarSolicitud;
            _avisoServicio.Mostrar(mensaje);
        }

        private async Task RechazarSolicitudAsync(SolicitudAmistadEntrada entrada)
        {
            if (!ValidarEntrada(entrada, "rechazar"))
            {
                return;
            }

            EstaProcesando = true;

            await EjecutarOperacionAsync(
                async () => await EjecutarRechazoAsync(entrada),
                excepcion => ManejarErrorRechazo(excepcion));

            EstaProcesando = false;
        }

        private async Task EjecutarRechazoAsync(SolicitudAmistadEntrada entrada)
        {
            _logger.InfoFormat(
                "Rechazando/Cancelando solicitud con: {0}",
                entrada.NombreUsuario);

            await _amigosServicio.EliminarAmigoAsync(
                entrada.Solicitud.UsuarioEmisor,
                entrada.Solicitud.UsuarioReceptor).ConfigureAwait(true);

            NotificarExitoRechazo();
        }

        private void NotificarExitoRechazo()
        {
            _sonidoManejador.ReproducirNotificacion();
            _avisoServicio.Mostrar(Lang.amigosTextoSolicitudCancelada);
        }

        private void ManejarErrorRechazo(Exception excepcion)
        {
            _logger.Error(
                "Error al rechazar/cancelar solicitud de amistad.",
                excepcion);
            _sonidoManejador.ReproducirError();
            string mensaje = excepcion.Message ?? Lang.errorTextoErrorProcesarSolicitud;
            _avisoServicio.Mostrar(mensaje);
        }

        private bool ValidarEntrada(SolicitudAmistadEntrada entrada, string operacion)
        {
            if (entrada == null)
            {
                _logger.WarnFormat(
                    "Intento de {0} solicitud con entrada nula.",
                    operacion);
                return false;
            }

            return true;
        }

        private static void EjecutarEnDispatcher(Action accion)
        {
            if (accion == null)
            {
                return;
            }

            Application application = Application.Current;

            if (application?.Dispatcher == null || application.Dispatcher.CheckAccess())
            {
                accion();
            }
            else
            {
                application.Dispatcher.BeginInvoke(accion);
            }
        }
    }
}