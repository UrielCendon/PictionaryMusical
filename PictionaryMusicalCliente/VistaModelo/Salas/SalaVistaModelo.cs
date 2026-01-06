using log4net;
using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Chat;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Amigos;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Salas
{
    /// <summary>
    /// ViewModel que gestiona la logica de una sala de juego.
    /// Coordina la comunicacion entre jugadores, el chat y el estado de la partida.
    /// </summary>
    public class SalaVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int MaximoJugadoresSala = 4;
        private const int MinimoJugadoresParaIniciar = 2;
        private const int MinimoPartesAcierto = 3;
        private const int IndicePuntosAdivinador = 2;

        private ISalasServicio _salasServicio;
        private IInvitacionSalaServicio _invitacionSalaServicio;
        private IReportesServicio _reportesServicio;
        private SonidoManejador _sonidoManejador;
        private IAvisoServicio _avisoServicio;
        private IUsuarioAutenticado _usuarioSesion;
        private IWcfClienteFabrica _fabricaClientes;
        private CancionManejador _cancionManejador;
        private ICatalogoCanciones _catalogoCanciones;

        private readonly DTOs.SalaDTO _sala;
        private readonly string _nombreUsuarioSesion;
        private readonly bool _esInvitado;
        private readonly bool _esHost;
        private readonly string _idJugador;

        private PartidaVistaModelo _partidaVistaModelo;
        private ChatVistaModelo _chatVistaModelo;
        private SalaNavegacionManejador _navegacionManejador;
        private SalaEventosManejador _eventosManejador;
        private SalaInvitacionesManejador _invitacionesManejador;
        private SalaJugadoresManejador _jugadoresManejador;
        private GestorSesionPartida _gestorSesion;

        private string _textoBotonIniciarPartida;
        private bool _botonIniciarPartidaHabilitado;
        private bool _mostrarBotonIniciarPartida;
        private bool _mostrarLogo;
        private string _codigoSala;
        private string _correoInvitacion;
        private bool _aplicacionCerrando;
        private bool _expulsionNavegada;
        private bool _cancelacionNavegada;
        private bool _desconexionInternetProcesada;
        private HashSet<string> _adivinadoresQuienYaAcertaron;
        private string _nombreDibujanteActual;
        private bool _rondaTerminadaTemprano;
        private string _mensajeChat;
        private DTOs.ResultadoPartidaDTO _resultadoPartidaPendiente;
        private ContextoFinPartida _contextoFinPartidaPendiente;

        private const int LimiteCaracteresChat = 150;
        private const double PorcentajePuntosDibujante = 0.2;

        /// <summary>
        /// Contiene datos del contexto para procesar el fin de partida.
        /// </summary>
        private sealed class ContextoFinPartida
        {
            public bool EsCancelacionPorFaltaDeJugadores { get; set; }
            public bool EsCancelacionPorHost { get; set; }
            public string MensajeLocalizado { get; set; }
        }

        /// <summary>
        /// Define los destinos posibles al salir de la partida.
        /// </summary>
        public enum DestinoNavegacion
        {
            InicioSesion,
            VentanaPrincipal
        }

        /// <param name="sala">Datos de la sala actual.</param>
        /// <param name="salasServicio">Servicio de comunicacion de salas.</param>
        /// <param name="invitacionesServicio">Servicio para invitar usuarios.</param>
        /// <param name="listaAmigosServicio">Servicio para obtener amigos.</param>
        /// <param name="perfilServicio">Servicio de perfil de usuario.</param>
        /// <param name="nombreJugador">Nombre del jugador actual (opcional).</param>
        /// <param name="esInvitado">Indica si el usuario es invitado.</param>
        public SalaVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            DTOs.SalaDTO sala,
            SalaVistaModeloDependencias dependencias,
            string nombreJugador = null,
            bool esInvitado = false)
            : base(ventana, localizador)
        {
            if (dependencias == null)
            {
                throw new ArgumentNullException(nameof(dependencias));
            }

            ValidarDependenciasSala(sala);
            AsignarServicios(dependencias);

            _sala = sala;
            _esInvitado = esInvitado;
            _nombreUsuarioSesion = !string.IsNullOrWhiteSpace(nombreJugador)
                ? nombreJugador
                : _usuarioSesion.NombreUsuario ?? string.Empty;
            _esHost = string.Equals(_sala.Creador, _nombreUsuarioSesion,
                StringComparison.OrdinalIgnoreCase);
            _idJugador = ObtenerIdentificadorJugador();

            InicializarColecciones();
            InicializarViewModelsHijos();
            ConfigurarEventosViewModels();
            InicializarEstadoInicial();
            InicializarComandos();
            ConfigurarPermisos();
            InicializarGestorSesion();
            ConfigurarEventoDesconexion();
            SuscribirMonitorConectividad();
        }

        private void ConfigurarEventoDesconexion()
        {
            DesconexionDetectada += ManejarDesconexionServidor;
        }

        private void SuscribirMonitorConectividad()
        {
            ConectividadRedMonitor.Instancia.ConexionPerdida += EnConexionInternetPerdida;
        }

        private void DesuscribirMonitorConectividad()
        {
            ConectividadRedMonitor.Instancia.ConexionPerdida -= EnConexionInternetPerdida;
        }

        private void DesuscribirEventosDesconexion()
        {
            DesconexionDetectada -= ManejarDesconexionServidor;
            DesuscribirMonitorConectividad();
        }

        private void EnConexionInternetPerdida(object remitente, EventArgs argumentos)
        {
            if (_desconexionInternetProcesada || _aplicacionCerrando || 
                _expulsionNavegada || _cancelacionNavegada)
            {
                return;
            }

            _desconexionInternetProcesada = true;
            _logger.Warn("Se detectó pérdida de conexión a internet durante la partida.");
            
            EjecutarEnDispatcher(() =>
            {
                ManejarPerdidaConexionInternet();
            });
        }

        private void ManejarPerdidaConexionInternet()
        {
            DesuscribirEventosDesconexion();
            _aplicacionCerrando = true;
            _sonidoManejador.ReproducirError();
            
            AbortarCanalPartidaSiNecesario();
            
            Navegar(DestinoNavegacion.InicioSesion);
            string mensaje = _esInvitado
                ? Lang.errorTextoSesionExpiradaGenerico
                : Lang.errorTextoServidorNoDisponible;
            _avisoServicio.Mostrar(mensaje);
        }

        private void ManejarDesconexionConVerificacionInternet(string mensajeServidorCaido)
        {
            string mensaje;
            if (ConectividadRedMonitor.HayConexion)
            {
                mensaje = _esInvitado
                    ? Lang.errorTextoSesionExpiradaGenerico
                    : mensajeServidorCaido;
            }
            else
            {
                mensaje = _esInvitado
                    ? Lang.errorTextoSesionExpiradaGenerico
                    : Lang.errorTextoServidorNoDisponible;
            }
            ManejarDesconexionCritica(mensaje);
        }

        /// <summary>
        /// Maneja una desconexion detectada desde un dialogo hijo.
        /// </summary>
        /// <param name="mensaje">Mensaje de la desconexion.</param>
        public void ManejarDesconexionDesdeDialogo(string mensaje)
        {
            ManejarDesconexionConVerificacionInternet(mensaje);
        }

        private void AbortarCanalPartidaSiNecesario()
        {
            try
            {
                _gestorSesion?.AbortarCanal();
            }
            catch (Exception excepcion)
            {
                _logger.Warn("Error al abortar canal de partida tras pérdida de internet.", 
                    excepcion);
            }
        }

        private void ManejarDesconexionServidor(string mensaje)
        {
            if (_desconexionInternetProcesada)
            {
                return;
            }

            DesuscribirEventosDesconexion();
            _logger.WarnFormat("Desconexion del servidor detectada en sala: {0}", mensaje);
            _aplicacionCerrando = true;
            Navegar(DestinoNavegacion.InicioSesion);
            
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                _avisoServicio.Mostrar(mensaje);
            }
        }

        private static void ValidarDependenciasSala(DTOs.SalaDTO sala)
        {
            if (sala == null)
            {
                throw new ArgumentNullException(nameof(sala));
            }
        }

        private void AsignarServicios(SalaVistaModeloDependencias dependencias)
        {
            _salasServicio = dependencias.Comunicacion.SalasServicio;
            _invitacionSalaServicio = dependencias.Comunicacion.InvitacionSalaServicio;
            _fabricaClientes = dependencias.Comunicacion.FabricaClientes;
            _reportesServicio = dependencias.Perfiles.ReportesServicio;
            _usuarioSesion = dependencias.Perfiles.UsuarioSesion;
            _sonidoManejador = dependencias.Audio.SonidoManejador;
            _cancionManejador = dependencias.Audio.CancionManejador;
            _catalogoCanciones = dependencias.Audio.CatalogoCanciones;
            _avisoServicio = dependencias.AvisoServicio;
        }

        private void InicializarColecciones()
        {
            _adivinadoresQuienYaAcertaron =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _nombreDibujanteActual = string.Empty;
            _rondaTerminadaTemprano = false;
        }

        private void InicializarViewModelsHijos()
        {
            _partidaVistaModelo = new PartidaVistaModelo(
                _ventana,
                _localizador,
                _sonidoManejador,
                _cancionManejador,
                _catalogoCanciones);
            _chatVistaModelo = CrearChatVistaModelo();
            _navegacionManejador = new SalaNavegacionManejador(
                _ventana,
                _localizador,
                _sonidoManejador,
                _avisoServicio,
                _usuarioSesion,
                _esInvitado);
            _eventosManejador = new SalaEventosManejador(
                _salasServicio,
                _sala.Codigo,
                _nombreUsuarioSesion,
                _sala.Creador);
            _invitacionesManejador = new SalaInvitacionesManejador(_esInvitado);

            var jugadoresDependencias = new SalaJugadoresManejadorDependencias(
                _salasServicio,
                _reportesServicio,
                _sonidoManejador,
                _avisoServicio,
                _localizador);
            var jugadoresContexto = new ContextoSalaJugador(
                _nombreUsuarioSesion,
                _sala.Creador,
                _sala.Codigo,
                _esHost,
                _esInvitado);
            _jugadoresManejador = new SalaJugadoresManejador(
                jugadoresDependencias,
                jugadoresContexto);
        }

        private void ConfigurarEventosViewModels()
        {
            _chatVistaModelo.PropertyChanged += ChatVistaModelo_PropertyChanged;
            _partidaVistaModelo.PropertyChanged += PartidaVistaModelo_PropertyChanged;
            ConfigurarPartidaVistaModelo();
            ConfigurarEventosManejador();
            ConfigurarJugadoresManejador();
        }

        private void ConfigurarJugadoresManejador()
        {
            _jugadoresManejador.MostrarConfirmacion = MostrarConfirmacion;
            _jugadoresManejador.SolicitarDatosReporte = SolicitarDatosReporte;
            _jugadoresManejador.EstablecerObtenerJuegoIniciado(ObtenerJuegoIniciado);
            _jugadoresManejador.ProgresoRondaCambiado += ManejarProgresoRondaCambiado;
            _jugadoresManejador.DesconexionDetectada += ManejarDesconexionDesdeJugadoresManejador;
        }

        private void ManejarDesconexionDesdeJugadoresManejador(string mensaje)
        {
            ManejarDesconexionConVerificacionInternet(mensaje);
        }

        private bool ObtenerJuegoIniciado()
        {
            return JuegoIniciado;
        }

        private void ManejarProgresoRondaCambiado(int conteo)
        {
            _partidaVistaModelo.AjustarProgresoRondaTrasCambioJugadores(conteo);
        }

        private void ConfigurarEventosManejador()
        {
            _eventosManejador.JugadorSeUnio += ManejarJugadorSeUnio;
            _eventosManejador.JugadorSalio += ManejarJugadorSalio;
            _eventosManejador.JugadorExpulsado += ManejarJugadorExpulsado;
            _eventosManejador.JugadorBaneado += ManejarJugadorBaneado;
            _eventosManejador.SalaActualizada += ManejarSalaActualizada;
            _eventosManejador.SalaCanceladaPorAnfitrion += ManejarSalaCanceladaPorAnfitrion;
            _eventosManejador.ExpulsionPropia += ManejarExpulsionPropia;
            _eventosManejador.BaneoPropio += ManejarBaneoPropio;
        }

        private void InicializarEstadoInicial()
        {
            _textoBotonIniciarPartida = Lang.partidaAdminTextoIniciarPartida;
            _botonIniciarPartidaHabilitado = _esHost;
            _mostrarBotonIniciarPartida = _esHost;
            _mostrarLogo = true;
            _codigoSala = _sala.Codigo;
            _jugadoresManejador.ActualizarJugadores(_sala.Jugadores);
        }

        private void ConfigurarPermisos()
        {
            _invitacionesManejador.ConfigurarPermisos();
            _chatVistaModelo.PuedeEscribir = true;
        }

        private void ConfigurarPartidaVistaModelo()
        {
            _partidaVistaModelo.JuegoIniciadoCambiado += OnJuegoIniciadoCambiado;
            _partidaVistaModelo.PuedeEscribirCambiado += ManejarPuedeEscribirCambiado;
            _partidaVistaModelo.EsDibujanteCambiado += ManejarEsDibujanteCambiado;
            _partidaVistaModelo.NombreCancionCambiado += ManejarNombreCancionCambiado;
            _partidaVistaModelo.TiempoRestanteCambiado += ManejarTiempoRestanteCambiado;
            _partidaVistaModelo.EnviarTrazoAlServidor = EnviarTrazoAlServidor;
            _partidaVistaModelo.NotificarTrazoDemasiadoGrande = ManejarTrazoDemasiadoGrande;
            _partidaVistaModelo.FinPartidaListoParaMostrar += ManejarFinPartidaListoParaMostrar;
        }

        private void ManejarTrazoDemasiadoGrande()
        {
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(Lang.errorTextoTrazoDemasiadoGrande);
        }

        private void ManejarPuedeEscribirCambiado(bool valor)
        {
            _chatVistaModelo.PuedeEscribir = valor;
        }

        private void ManejarEsDibujanteCambiado(bool valor)
        {
            _chatVistaModelo.EsDibujante = valor;
        }

        private void ManejarNombreCancionCambiado(string valor)
        {
            _chatVistaModelo.NombreCancionCorrecta = valor;
        }

        private void ManejarTiempoRestanteCambiado(int valor)
        {
            _chatVistaModelo.TiempoRestante = valor;
        }

        private ChatVistaModelo CrearChatVistaModelo()
        {
            var chatMensajeria = new ChatMensajeriaDelegado(EjecutarEnviarMensaje);
            var chatAciertos = new ChatAciertosDelegado(
                () => _nombreUsuarioSesion,
                EjecutarRegistrarAciertoAsync);
            var chatReglas = new ChatReglasPartida(chatAciertos);

            return new ChatVistaModelo(_ventana, _localizador, chatMensajeria, chatReglas);
        }


        private void ChatVistaModelo_PropertyChanged(object remitente, 
            PropertyChangedEventArgs argumentosEvento)
        {
            if (string.Equals(
                argumentosEvento.PropertyName,
                nameof(ChatVistaModelo.PuedeEscribir),
                StringComparison.Ordinal))
            {
                NotificarCambio(nameof(PuedeEscribir));
            }
        }

        private void PartidaVistaModelo_PropertyChanged(object remitente, 
            PropertyChangedEventArgs argumentosEvento)
        {
            NotificarCambio(argumentosEvento.PropertyName);
        }

        private void OnJuegoIniciadoCambiado(bool juegoIniciado)
        {
            MostrarBotonIniciarPartida = _esHost && !juegoIniciado;
            MostrarLogo = !juegoIniciado;
            _jugadoresManejador.ActualizarVisibilidadBotonesExpulsion();
            _jugadoresManejador.ActualizarVisibilidadBotonesReporte();
            _chatVistaModelo.EsPartidaIniciada = juegoIniciado;
        }

        /// <summary>
        /// Obtiene el ViewModel de la partida.
        /// </summary>
        public PartidaVistaModelo PartidaVistaModelo => _partidaVistaModelo;

        /// <summary>
        /// Obtiene el ViewModel del chat.
        /// </summary>
        public ChatVistaModelo ChatVistaModelo => _chatVistaModelo;

        /// <summary>
        /// Indica si el juego ha iniciado.
        /// </summary>
        public bool JuegoIniciado => _partidaVistaModelo.JuegoIniciado;

        /// <summary>
        /// Indica si el usuario actual es el anfitrion de la sala.
        /// </summary>
        public bool EsHost => _esHost;

        /// <summary>
        /// Obtiene o establece el texto del boton para iniciar partida.
        /// </summary>
        public string TextoBotonIniciarPartida
        {
            get => _textoBotonIniciarPartida;
            set => EstablecerPropiedad(ref _textoBotonIniciarPartida, value);
        }

        /// <summary>
        /// Obtiene o establece si el boton de iniciar partida esta habilitado.
        /// </summary>
        public bool BotonIniciarPartidaHabilitado
        {
            get => _botonIniciarPartidaHabilitado;
            set => EstablecerPropiedad(ref _botonIniciarPartidaHabilitado, value);
        }

        /// <summary>
        /// Indica si se debe mostrar el boton de iniciar partida.
        /// </summary>
        public bool MostrarBotonIniciarPartida
        {
            get => _mostrarBotonIniciarPartida;
            private set => EstablecerPropiedad(ref _mostrarBotonIniciarPartida, value);
        }

        /// <summary>
        /// Indica si se debe mostrar el logo de la sala.
        /// </summary>
        public bool MostrarLogo
        {
            get => _mostrarLogo;
            private set => EstablecerPropiedad(ref _mostrarLogo, value);
        }

        /// <summary>
        /// Obtiene o establece el codigo de la sala.
        /// </summary>
        public string CodigoSala
        {
            get => _codigoSala;
            set => EstablecerPropiedad(ref _codigoSala, value);
        }

        /// <summary>
        /// Obtiene la coleccion de jugadores en la sala.
        /// </summary>
        public ObservableCollection<JugadorElemento> Jugadores =>
            _jugadoresManejador?.Jugadores;

        /// <summary>
        /// Obtiene o establece el correo para invitacion.
        /// </summary>
        public string CorreoInvitacion
        {
            get => _correoInvitacion;
            set => EstablecerPropiedad(ref _correoInvitacion, value);
        }

        /// <summary>
        /// Indica si puede invitar jugadores por correo.
        /// </summary>
        public bool PuedeInvitarPorCorreo
        {
            get => _invitacionesManejador?.PuedeInvitarPorCorreo ?? false;
            private set
            {
                if (_invitacionesManejador != null)
                {
                    _invitacionesManejador.PuedeInvitarPorCorreo = value;
                    NotificarCambio(nameof(PuedeInvitarPorCorreo));
                    NotificarComando(InvitarCorreoComando);
                    NotificarComando(InvitarAmigosComando);
                }
            }
        }

        /// <summary>
        /// Indica si puede invitar amigos a la sala.
        /// </summary>
        public bool PuedeInvitarAmigos
        {
            get => _invitacionesManejador?.PuedeInvitarAmigos ?? false;
            private set
            {
                if (_invitacionesManejador != null)
                {
                    _invitacionesManejador.PuedeInvitarAmigos = value;
                    NotificarCambio(nameof(PuedeInvitarAmigos));
                    NotificarComando(InvitarAmigosComando);
                }
            }
        }

        /// <summary>
        /// Indica si el usuario actual es un invitado.
        /// </summary>
        public bool EsInvitado => _esInvitado;

        /// <summary>
        /// Indica si el usuario puede escribir en el chat.
        /// </summary>
        public bool PuedeEscribir
        {
            get => _chatVistaModelo.PuedeEscribir;
            private set => _chatVistaModelo.PuedeEscribir = value;
        }

        /// <summary>
        /// Indica si el usuario actual es el dibujante.
        /// </summary>
        public bool EsDibujante => _partidaVistaModelo.EsDibujante;

        /// <summary>
        /// Obtiene o establece el mensaje actual del chat.
        /// </summary>
        public string MensajeChat
        {
            get => _mensajeChat;
            set => EstablecerPropiedad(ref _mensajeChat, LimitarMensajePorCaracteres(value));
        }

        /// <summary>
        /// Comando para invitar por correo electronico.
        /// </summary>
        public ICommand InvitarCorreoComando { get; private set; }

        /// <summary>
        /// Comando asincrono para invitar amigos.
        /// </summary>
        public IComandoAsincrono InvitarAmigosComando { get; private set; }

        /// <summary>
        /// Comando para abrir la ventana de ajustes.
        /// </summary>
        public ICommand AbrirAjustesComando { get; private set; }

        /// <summary>
        /// Comando para iniciar la partida.
        /// </summary>
        public ICommand IniciarPartidaComando { get; private set; }

        /// <summary>
        /// Comando para cerrar la ventana.
        /// </summary>
        public ICommand CerrarVentanaComando { get; private set; }

        /// <summary>
        /// Comando para enviar un mensaje en el chat.
        /// </summary>
        public ICommand EnviarMensajeChatComando { get; private set; }

        private Func<string, bool> _mostrarConfirmacion;
        private Func<string, ResultadoReporteJugador> _solicitarDatosReporte;

        /// <summary>
        /// Delegado para mostrar dialogos de confirmacion.
        /// </summary>
        public Func<string, bool> MostrarConfirmacion
        {
            get => _mostrarConfirmacion;
            set
            {
                _mostrarConfirmacion = value;
                if (_jugadoresManejador != null)
                {
                    _jugadoresManejador.MostrarConfirmacion = value;
                }
            }
        }

        /// <summary>
        /// Delegado para solicitar datos de un reporte de jugador.
        /// </summary>
        public Func<string, ResultadoReporteJugador> SolicitarDatosReporte
        {
            get => _solicitarDatosReporte;
            set
            {
                _solicitarDatosReporte = value;
                if (_jugadoresManejador != null)
                {
                    _jugadoresManejador.SolicitarDatosReporte = value;
                }
            }
        }

        /// <summary>
        /// Accion para cerrar la ventana actual.
        /// </summary>
        public Action CerrarVentana { get; set; }

        /// <summary>
        /// Delegado para mostrar el dialogo de invitar amigos.
        /// </summary>
        public Func<InvitarAmigosVistaModelo, Task> MostrarInvitarAmigos { get; set; }

        /// <summary>
        /// Delegado para verificar si la aplicacion esta cerrando globalmente.
        /// </summary>
        public Func<bool> ChequearCierreAplicacionGlobal { get; set; }

        private DestinoNavegacion ObtenerDestinoSegunSesion()
        {
            return _navegacionManejador.ObtenerDestinoSegunSesion();
        }

        private static void NotificarComando(ICommand comando)
        {
            if (comando is IComandoNotificable notificable)
            {
                notificable.NotificarPuedeEjecutar();
            }
        }

        private void InicializarComandos()
        {
            InvitarCorreoComando = new ComandoAsincrono(
                EjecutarComandoInvitarCorreoAsync,
                ValidarPuedeInvitarPorCorreo);
            InvitarAmigosComando = new ComandoAsincrono(
                EjecutarComandoInvitarAmigosAsync,
                ValidarPuedeInvitarAmigos);
            AbrirAjustesComando = new ComandoDelegado(EjecutarComandoAbrirAjustes);
            IniciarPartidaComando = new ComandoAsincrono(EjecutarComandoIniciarPartidaAsync);
            CerrarVentanaComando = new ComandoDelegado(EjecutarComandoCerrarVentana);
            EnviarMensajeChatComando = new ComandoDelegado(EjecutarComandoEnviarMensajeChat);
        }

        private async Task EjecutarComandoInvitarCorreoAsync(object parametro)
        {
            await EjecutarInvitarCorreoAsync();
        }

        private bool ValidarPuedeInvitarPorCorreo(object parametro)
        {
            return PuedeInvitarPorCorreo;
        }

        private async Task EjecutarComandoInvitarAmigosAsync()
        {
            await EjecutarInvitarAmigosAsync();
        }

        private bool ValidarPuedeInvitarAmigos()
        {
            return PuedeInvitarAmigos;
        }

        private void EjecutarComandoAbrirAjustes(object parametro)
        {
            EjecutarAbrirAjustes();
        }

        private async Task EjecutarComandoIniciarPartidaAsync(object parametro)
        {
            await EjecutarIniciarPartidaAsync();
        }

        private void EjecutarComandoCerrarVentana(object parametro)
        {
            EjecutarCerrarVentana();
        }

        private void EjecutarComandoEnviarMensajeChat(object parametro)
        {
            EjecutarEnviarMensajeChat();
        }

        private void InicializarGestorSesion()
        {
            var parametros = new GestorSesionPartidaParametros(
                _fabricaClientes,
                _codigoSala,
                _idJugador,
                _nombreUsuarioSesion,
                _esHost);

            _gestorSesion = new GestorSesionPartida(parametros);
            SuscribirEventosGestorSesion();

            if (!_gestorSesion.Inicializar())
            {
                MostrarErrorEnUI(Lang.errorTextoErrorProcesarSolicitud);
            }
        }

        private void SuscribirEventosGestorSesion()
        {
            _gestorSesion.PartidaIniciada += ManejarPartidaIniciada;
            _gestorSesion.RondaIniciada += ManejarRondaIniciada;
            _gestorSesion.JugadorAdivino += ManejarJugadorAdivino;
            _gestorSesion.MensajeChatRecibido += ManejarMensajeChatRecibido;
            _gestorSesion.TrazoRecibido += ManejarTrazoRecibido;
            _gestorSesion.RondaFinalizada += ManejarRondaFinalizada;
            _gestorSesion.PartidaFinalizada += ManejarPartidaFinalizada;
            _gestorSesion.CanalFallido += ManejarCanalFallido;
            _gestorSesion.ErrorComunicacion += ManejarErrorComunicacionGestor;
        }

        private void DesuscribirEventosGestorSesion()
        {
            if (_gestorSesion == null)
            {
                return;
            }

            _gestorSesion.PartidaIniciada -= ManejarPartidaIniciada;
            _gestorSesion.RondaIniciada -= ManejarRondaIniciada;
            _gestorSesion.JugadorAdivino -= ManejarJugadorAdivino;
            _gestorSesion.MensajeChatRecibido -= ManejarMensajeChatRecibido;
            _gestorSesion.TrazoRecibido -= ManejarTrazoRecibido;
            _gestorSesion.RondaFinalizada -= ManejarRondaFinalizada;
            _gestorSesion.PartidaFinalizada -= ManejarPartidaFinalizada;
            _gestorSesion.CanalFallido -= ManejarCanalFallido;
            _gestorSesion.ErrorComunicacion -= ManejarErrorComunicacionGestor;
        }

        private void ManejarCanalFallido()
        {
            if (!_aplicacionCerrando && !_expulsionNavegada && !_cancelacionNavegada)
            {
                string mensajeServidorCaido = _esInvitado
                    ? Lang.errorTextoSesionExpiradaGenerico
                    : Lang.errorTextoServidorNoDisponible;
                ManejarDesconexionConVerificacionInternet(mensajeServidorCaido);
            }
        }

        private void ManejarErrorComunicacionGestor(string mensaje)
        {
            ManejarDesconexionConVerificacionInternet(mensaje);
        }

        private string ObtenerIdentificadorJugador()
        {
            if (_usuarioSesion.JugadorId > 0)
            {
                return _usuarioSesion.JugadorId.ToString();
            }

            if (!string.IsNullOrWhiteSpace(_nombreUsuarioSesion))
            {
                return _nombreUsuarioSesion;
            }

            return Guid.NewGuid().ToString();
        }

        private void AplicarInicioVisualPartida()
        {
            _partidaVistaModelo.AplicarInicioVisualPartida(Jugadores?.Count ?? 0);
            ReiniciarPuntajesJugadores();
            BotonIniciarPartidaHabilitado = false;
            TextoBotonIniciarPartida = Lang.partidaTextoPartidaEnCurso;
        }

        private async Task EjecutarInvitarCorreoAsync()
        {
            try
            {
                var resultado = await _invitacionSalaServicio
                    .InvitarPorCorreoAsync(_codigoSala, CorreoInvitacion)
                    .ConfigureAwait(true);

                if (resultado.Exitoso)
                {
                    _sonidoManejador.ReproducirNotificacion();
                    _avisoServicio.Mostrar(resultado.Mensaje);
                    CorreoInvitacion = string.Empty;
                    return;
                }

                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(resultado.Mensaje);
            }
            catch (ServicioExcepcion excepcion) when (
                excepcion.Tipo == TipoErrorServicio.Comunicacion ||
                excepcion.Tipo == TipoErrorServicio.TiempoAgotado)
            {
                _logger.Error("Error de conexion al invitar por correo.", excepcion);
                ManejarDesconexionConVerificacionInternet(Lang.errorTextoServidorNoDisponible);
            }
        }

        private async Task EjecutarInvitarAmigosAsync()
        {
            try
            {
                var parametros = new InvitacionAmigosParametros(
                    _codigoSala,
                    _nombreUsuarioSesion,
                    _invitacionesManejador.AmigosInvitados);

                var resultado = await _invitacionSalaServicio
                    .ObtenerInvitacionAmigosAsync(parametros)
                    .ConfigureAwait(true);

                if (!resultado.Exitoso)
                {
                    _sonidoManejador.ReproducirError();
                    _avisoServicio.Mostrar(resultado.Mensaje);
                    return;
                }

                if (MostrarInvitarAmigos != null && resultado.VistaModelo != null)
                {
                    await MostrarInvitarAmigos(resultado.VistaModelo).ConfigureAwait(true);
                }
            }
            catch (ServicioExcepcion excepcion) when (
                excepcion.Tipo == TipoErrorServicio.Comunicacion ||
                excepcion.Tipo == TipoErrorServicio.TiempoAgotado)
            {
                _logger.Error("Error de conexion al obtener amigos para invitar.", excepcion);
                ManejarDesconexionConVerificacionInternet(Lang.errorTextoServidorNoDisponible);
            }
        }

        private void EjecutarAbrirAjustes()
        {
            AbrirAjustesPartida();
        }

        private static string LimitarMensajePorCaracteres(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return mensaje;
            }

            if (mensaje.Length <= LimiteCaracteresChat)
            {
                return mensaje;
            }

            return mensaje.Substring(0, LimiteCaracteresChat);
        }

        private void EjecutarEnviarMensajeChat()
        {
            if (string.IsNullOrWhiteSpace(MensajeChat))
            {
                return;
            }

            _ = _chatVistaModelo.EnviarMensaje(MensajeChat);
            MensajeChat = string.Empty;
        }

        private void EjecutarEnviarMensaje(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return;
            }

            _ = EnviarMensajeAsync(mensaje);
        }

        private async Task EnviarMensajeAsync(string mensaje)
        {
            try
            {
                if (_gestorSesion == null || !_gestorSesion.ProxyDisponible)
                {
                    _logger.Warn("Gestor de sesion no disponible para enviar mensaje.");
                    return;
                }

                await _gestorSesion.EnviarMensajeAsync(mensaje).ConfigureAwait(false);
            }
            catch (FaultException excepcion)
            {
                _logger.Error("Fallo del servicio al enviar mensaje de juego.", excepcion);
                _sonidoManejador.ReproducirError();
                EjecutarEnDispatcher(() => 
                    _avisoServicio.Mostrar(Lang.errorTextoEnviarMensaje));
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("Error de comunicacion al enviar mensaje de juego.", excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida al enviar mensaje de juego.", excepcion);
                _sonidoManejador.ReproducirError();
            }
        }

        private async Task EjecutarRegistrarAciertoAsync(
            string nombreJugador, 
            int puntosAdivinador, 
            int puntosDibujante)
        {
            if (string.IsNullOrWhiteSpace(nombreJugador))
            {
                return;
            }

            _logger.InfoFormat(
                "Registrando acierto. Puntos adivinador: {0}, Puntos dibujante: {1}",
                puntosAdivinador,
                puntosDibujante);

            try
            {
                string mensajeAcierto = string.Format(
                    "ACIERTO:{0}:{1}:{2}",
                    nombreJugador,
                    puntosAdivinador,
                    puntosDibujante);

                if (_gestorSesion != null && _gestorSesion.ProxyDisponible)
                {
                    await _gestorSesion.EnviarMensajeAsync(mensajeAcierto)
                        .ConfigureAwait(false);
                }
            }
            catch (FaultException excepcion)
            {
                _logger.Error("Fallo del servicio al registrar acierto.", excepcion);
                _sonidoManejador.ReproducirError();
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("No se pudo registrar el acierto en el servidor.", excepcion);
                _sonidoManejador.ReproducirError();
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida al registrar acierto.", excepcion);
                _sonidoManejador.ReproducirError();
            }
        }

        /// <summary>
        /// Envia un trazo de dibujo al servidor para su distribucion a otros jugadores.
        /// </summary>
        /// <param name="trazo">Datos del trazo a enviar.</param>
        public void EnviarTrazoAlServidor(DTOs.TrazoDTO trazo)
        {
            if (trazo == null)
            {
                return;
            }

            _gestorSesion?.EnviarTrazo(trazo);
        }

        private async Task EjecutarIniciarPartidaAsync()
        {
            if (JuegoIniciado)
            {
                return;
            }

            if (Jugadores.Count < MinimoJugadoresParaIniciar)
            {
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(Lang.errorTextoPartidaUnJugador);
                return;
            }

            try
            {
                BotonIniciarPartidaHabilitado = false;

                if (_gestorSesion != null && _gestorSesion.ProxyDisponible)
                {
                    await _gestorSesion.IniciarPartidaAsync().ConfigureAwait(true);
                }
                else
                {
                    AplicarInicioVisualPartida();
                }
            }
            catch (FaultException excepcion)
            {
                _logger.Error("Fallo del servicio al iniciar la partida.", excepcion);
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(Lang.errorTextoErrorProcesarSolicitud);
                BotonIniciarPartidaHabilitado = true;
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("No se pudo solicitar el inicio de la partida.", excepcion);
                ManejarDesconexionConVerificacionInternet(Lang.errorTextoServidorNoDisponible);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error("Tiempo agotado al iniciar la partida.", excepcion);
                ManejarDesconexionConVerificacionInternet(Lang.errorTextoServidorNoDisponible);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida al iniciar la partida.", excepcion);
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(Lang.errorTextoErrorProcesarSolicitud);
                BotonIniciarPartidaHabilitado = true;
            }
        }

        private void EjecutarCerrarVentana()
        {
            bool cerrandoPorVentana = ChequearCierreAplicacionGlobal?.Invoke() ?? true;

            if (cerrandoPorVentana)
            {
                NotificarCierreAplicacionCompleta();
            }
        }

        private void ManejarPartidaIniciada()
        {
            AplicarInicioVisualPartida();
            _partidaVistaModelo.NotificarPartidaIniciada();
            BotonIniciarPartidaHabilitado = false;
            TextoBotonIniciarPartida = string.Empty;
            PuedeInvitarAmigos = false;
            PuedeInvitarPorCorreo = false;
        }

        private void ManejarRondaIniciada(DTOs.RondaDTO ronda)
        {
            _adivinadoresQuienYaAcertaron.Clear();
            _rondaTerminadaTemprano = false;

            _nombreDibujanteActual = ronda.NombreDibujante ?? string.Empty;

            _partidaVistaModelo.NotificarInicioRonda(ronda, Jugadores?.Count ?? 0);
        }

        private void ManejarJugadorAdivino(string nombreJugador, int puntos)
        {
            if (Jugadores != null && puntos > 0)
            {
                _jugadoresManejador.AgregarPuntos(nombreJugador, puntos);

                if (!_adivinadoresQuienYaAcertaron.Contains(nombreJugador))
                {
                    _adivinadoresQuienYaAcertaron.Add(nombreJugador);

                    int puntosBonusDibujante = (int)(puntos * PorcentajePuntosDibujante);

                    AgregarPuntosAlDibujante(puntosBonusDibujante);

                    if (TodosLosAdivinadoresAcertaron() && !_rondaTerminadaTemprano)
                    {
                        _rondaTerminadaTemprano = true;
                        _partidaVistaModelo.NotificarFinRondaTemprano();
                    }
                }
            }

            _chatVistaModelo.NotificarJugadorAdivinoEnChat(nombreJugador);
            _partidaVistaModelo.NotificarJugadorAdivino(
                nombreJugador, 
                puntos, 
                _nombreUsuarioSesion);
        }

        private void AgregarPuntosAlDibujante(int puntosBonusDibujante)
        {
            if (puntosBonusDibujante <= 0 || Jugadores == null)
            {
                return;
            }

            _jugadoresManejador.AgregarPuntos(_nombreDibujanteActual, puntosBonusDibujante);
        }

        private int ObtenerTotalAdivinadores()
        {
            return Jugadores?.Count > 0 ? Jugadores.Count - 1 : 0;
        }

        private bool TodosLosAdivinadoresAcertaron()
        {
            int totalAdivinadores = ObtenerTotalAdivinadores();
            return totalAdivinadores > 0
                && _adivinadoresQuienYaAcertaron.Count >= totalAdivinadores;
        }

        private void ManejarMensajeChatRecibido(string nombreJugador, string mensaje)
        {
            if (EsMensajeAcierto(mensaje) && IntentarProcesarAciertoDesdeMensaje(mensaje))
            {
                return;
            }
            _chatVistaModelo.NotificarMensajeChat(nombreJugador, mensaje);
        }

        private void ManejarTrazoRecibido(DTOs.TrazoDTO trazo)
        {
            _partidaVistaModelo.NotificarTrazoRecibido(trazo);
        }

        private void ManejarRondaFinalizada(bool tiempoAgotado)
        {
            if (_rondaTerminadaTemprano)
            {
                return;
            }

            _partidaVistaModelo.NotificarFinRonda(tiempoAgotado);
        }

        private void ManejarPartidaFinalizada(DTOs.ResultadoPartidaDTO resultado)
        {
            var contextoFinPartida = CrearContextoFinPartida(resultado);
            _resultadoPartidaPendiente = resultado;
            _contextoFinPartidaPendiente = contextoFinPartida;
            _partidaVistaModelo.NotificarFinPartida();
        }

        private void ManejarFinPartidaListoParaMostrar()
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(() =>
            {
                var resultado = _resultadoPartidaPendiente;
                var contexto = _contextoFinPartidaPendiente;
                _resultadoPartidaPendiente = null;
                _contextoFinPartidaPendiente = null;

                if (resultado != null && contexto != null)
                {
                    ProcesarFinPartidaEnDispatcher(resultado, contexto);
                }
            }));
        }

        private ContextoFinPartida CrearContextoFinPartida(DTOs.ResultadoPartidaDTO resultado)
        {
            string mensajeOriginal = resultado?.Mensaje;

            _logger.InfoFormat(
                "CrearContextoFinPartida - Mensaje recibido del servidor: '{0}'",
                mensajeOriginal ?? "(nulo)");

            bool esCancelacionPorFaltaDeJugadores =
                EsMensajeCancelacionPorFaltaJugadores(mensajeOriginal);
            bool esCancelacionPorHost = EsMensajeCancelacionPorHost(mensajeOriginal);

            _logger.InfoFormat(
                "CrearContextoFinPartida - " +
                "EsCancelacionPorFaltaJugadores: {0}, EsCancelacionPorHost: {1}",
                esCancelacionPorFaltaDeJugadores,
                esCancelacionPorHost);

            string mensajeLocalizado = ObtenerMensajeLocalizadoSegunTipo(
                mensajeOriginal,
                esCancelacionPorFaltaDeJugadores,
                esCancelacionPorHost);

            return new ContextoFinPartida
            {
                EsCancelacionPorFaltaDeJugadores = esCancelacionPorFaltaDeJugadores,
                EsCancelacionPorHost = esCancelacionPorHost,
                MensajeLocalizado = mensajeLocalizado
            };
        }

        private static bool EsMensajeCancelacionPorFaltaJugadores(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return false;
            }

            bool tieneJugadores =
                mensaje.IndexOf("jugador", StringComparison.OrdinalIgnoreCase) >= 0;
            bool tieneFalta =
                mensaje.IndexOf("falta", StringComparison.OrdinalIgnoreCase) >= 0;
            bool tieneSuficientes =
                mensaje.IndexOf("suficientes", StringComparison.OrdinalIgnoreCase) >= 0;
            bool tieneInsuficientes =
                mensaje.IndexOf("insuficientes", StringComparison.OrdinalIgnoreCase) >= 0;

            return tieneJugadores && (tieneFalta || tieneSuficientes || tieneInsuficientes);
        }

        private static bool EsMensajeCancelacionPorHost(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return false;
            }

            return mensaje.IndexOf("anfitrion", StringComparison.OrdinalIgnoreCase) >= 0
                && mensaje.IndexOf("abandon", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string ObtenerMensajeLocalizadoSegunTipo(
            string mensajeOriginal,
            bool esCancelacionPorFaltaJugadores,
            bool esCancelacionPorHost)
        {
            if (esCancelacionPorHost)
            {
                return Lang.partidaTextoHostCanceloSala;
            }

            if (esCancelacionPorFaltaJugadores)
            {
                return Lang.partidaTextoJugadoresInsuficientes;
            }

            if (!string.IsNullOrWhiteSpace(mensajeOriginal))
            {
                return _localizador.Localizar(mensajeOriginal, mensajeOriginal);
            }

            return mensajeOriginal;
        }

        private void ProcesarFinPartidaEnDispatcher(
            DTOs.ResultadoPartidaDTO resultado,
            ContextoFinPartida contexto)
        {
            if (_eventosManejador.SalaCancelada)
            {
                return;
            }

            _partidaVistaModelo.NotificarFinPartida();
            _rondaTerminadaTemprano = false;
            _adivinadoresQuienYaAcertaron.Clear();
            BotonIniciarPartidaHabilitado = false;

            if (contexto.EsCancelacionPorHost && _esHost)
            {
                return;
            }

            if (contexto.EsCancelacionPorFaltaDeJugadores || contexto.EsCancelacionPorHost)
            {
                ProcesarCancelacionPartida(contexto.MensajeLocalizado);
                return;
            }

            string mensajeErrorClasificacion = null;
            if (!_esInvitado && !string.IsNullOrWhiteSpace(resultado?.Mensaje))
            {
                mensajeErrorClasificacion = _localizador.Localizar(
                    resultado.Mensaje,
                    Lang.clasificacionErrorActualizar);
            }

            NavegarSegunSesion();
            MostrarAvisoResultadoPartida(resultado);

            if (!string.IsNullOrWhiteSpace(mensajeErrorClasificacion))
            {
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(mensajeErrorClasificacion);
            }
        }

        private void ProcesarCancelacionPartida(string mensaje)
        {
            NavegarSegunSesion();

            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                _avisoServicio.Mostrar(mensaje);
            }
        }

        private void NavegarSegunSesion()
        {
            var destino = ObtenerDestinoSegunSesion();

            if (destino == DestinoNavegacion.InicioSesion)
            {
                _aplicacionCerrando = true;
            }

            Navegar(destino);
        }

        private void MostrarAvisoResultadoPartida(DTOs.ResultadoPartidaDTO resultado)
        {
            bool esGanador = DeterminarSiEsGanador(resultado);
            string titulo = esGanador
                ? Lang.partidaTextoGanasteTitulo
                : Lang.partidaTextoPerdisteTitulo;
            string mensajeResultado = esGanador
                ? Lang.partidaTextoGanasteMensaje
                : Lang.partidaTextoPerdisteMensaje;

            string mensajeFinal = $"{titulo}\n{mensajeResultado}";
            _avisoServicio.Mostrar(mensajeFinal);
        }

        private bool DeterminarSiEsGanador(DTOs.ResultadoPartidaDTO resultado)
        {
            if (resultado?.Clasificacion == null || resultado.Clasificacion.Count == 0)
            {
                return false;
            }

            var clasificacion = resultado.Clasificacion;
            var ganador = clasificacion.FirstOrDefault();

            if (ganador == null)
            {
                return false;
            }

            return string.Equals(
                ganador.Usuario,
                _nombreUsuarioSesion,
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsMensajeAcierto(string mensaje)
        {
            return !string.IsNullOrWhiteSpace(mensaje) && mensaje.StartsWith("ACIERTO:", 
                StringComparison.OrdinalIgnoreCase);
        }

        private bool IntentarProcesarAciertoDesdeMensaje(string mensaje)
        {
            string[] partes = mensaje?.Split(':');
            if (partes == null || partes.Length < MinimoPartesAcierto)
            {
                return false;
            }

            string nombreJugador = partes[1];
            if (string.IsNullOrWhiteSpace(nombreJugador))
            {
                return false;
            }

            if (!int.TryParse(partes[IndicePuntosAdivinador], out int puntosAdivinador))
            {
                return false;
            }

            ManejarJugadorAdivino(nombreJugador, puntosAdivinador);
            return true;
        }

        private void ManejarJugadorSeUnio(string nombreJugador)
        {
            if (Jugadores.Any(jugador => string.Equals(
                jugador.Nombre,
                nombreJugador,
                StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            if (Jugadores.Count >= MaximoJugadoresSala)
            {
                return;
            }

            _jugadoresManejador.AgregarJugador(nombreJugador);
            NotificarCambio(nameof(Jugadores));
        }

        private void ManejarJugadorSalio(string nombreJugador)
        {
            _jugadoresManejador.EliminarJugador(nombreJugador);
            NotificarCambio(nameof(Jugadores));
        }

        private void ManejarJugadorExpulsado(string nombreJugador)
        {
            ManejarJugadorSalio(nombreJugador);
        }

        private void ManejarSalaActualizada(DTOs.SalaDTO sala)
        {
            if (sala == null)
            {
                return;
            }

            _jugadoresManejador.ActualizarJugadores(sala.Jugadores);
            NotificarCambio(nameof(Jugadores));
            
            bool anfitrionSiguePresente = sala.Jugadores?.Any(jugador =>
                string.Equals(
                    jugador,
                    _sala.Creador,
                    StringComparison.OrdinalIgnoreCase)) == true;

            bool usuarioSiguePresente = sala.Jugadores?.Any(jugador =>
                string.Equals(
                    jugador,
                    _nombreUsuarioSesion,
                    StringComparison.OrdinalIgnoreCase)) == true;

            if (!_expulsionNavegada && !usuarioSiguePresente)
            {
                ManejarExpulsionPropia();
                return;
            }

            if (!anfitrionSiguePresente)
            {
                CancelarSalaPorAnfitrion();
            }
        }

        private void ManejarSalaCanceladaPorAnfitrion()
        {
            CancelarSalaPorAnfitrion();
        }

        private void ManejarExpulsionPropia()
        {
            if (_expulsionNavegada)
            {
                return;
            }

            _expulsionNavegada = true;
            _eventosManejador.MarcarSalaCancelada();

            var destino = ObtenerDestinoSegunSesion();

            if (destino == DestinoNavegacion.InicioSesion)
            {
                _aplicacionCerrando = true;
            }

            Navegar(destino);
            _avisoServicio.Mostrar(Lang.expulsarJugadorTextoFuisteExpulsado);
        }

        private void ManejarBaneoPropio()
        {
            if (_expulsionNavegada)
            {
                return;
            }

            _expulsionNavegada = true;
            _eventosManejador.MarcarSalaCancelada();

            _aplicacionCerrando = true;
            Navegar(DestinoNavegacion.InicioSesion);
            _avisoServicio.Mostrar(Lang.expulsarJugadorTextoFuisteBaneado);
        }

        private void ManejarJugadorBaneado(string nombreJugador)
        {
            ManejarJugadorSalio(nombreJugador);
        }

        private void ReiniciarPuntajesJugadores()
        {
            _jugadoresManejador.ReiniciarPuntajes();
        }

        private void CancelarSalaPorAnfitrion()
        {
            if (_cancelacionNavegada)
            {
                return;
            }

            _cancelacionNavegada = true;
            _eventosManejador.MarcarSalaCancelada();
            _logger.Warn("La sala se cancelo porque el anfitrion abandono la partida.");

            _partidaVistaModelo.ReiniciarEstadoVisualSalaCancelada();
            BotonIniciarPartidaHabilitado = false;
            _jugadoresManejador.Limpiar();
            NotificarCambio(nameof(Jugadores));

            var destino = ObtenerDestinoSegunSesion();

            if (destino == DestinoNavegacion.InicioSesion)
            {
                _aplicacionCerrando = true;
            }

            Navegar(destino);
            _avisoServicio.Mostrar(Lang.partidaTextoHostCanceloSala);
        }

        /// <summary>
        /// Finaliza la sala, desuscribe eventos y cierra conexiones.
        /// </summary>
        /// <returns>Tarea que representa la operacion asincrona.</returns>
        public async Task FinalizarAsync()
        {
            DesuscribirEventosDesconexion();

            _partidaVistaModelo.PropertyChanged -= PartidaVistaModelo_PropertyChanged;

            _partidaVistaModelo.Detener();

            _eventosManejador?.Dispose();

            DesuscribirEventosGestorSesion();

            if (_gestorSesion != null)
            {
                await _gestorSesion.CerrarCanalAsync().ConfigureAwait(false);
                _gestorSesion.Dispose();
                _gestorSesion = null;
            }

            if (_sala != null && !string.IsNullOrWhiteSpace(_sala.Codigo)
                && !string.IsNullOrWhiteSpace(_nombreUsuarioSesion))
            {
                try
                {
                    await _salasServicio.AbandonarSalaAsync(
                        _sala.Codigo,
                        _nombreUsuarioSesion).ConfigureAwait(false);
                }
                catch (ServicioExcepcion excepcion)
                {
                    _logger.WarnFormat("Error al abandonar sala en finalizacion: {0}",
						excepcion.Message);
                }
            }
        }

        /// <summary>
        /// Notifica que la aplicacion esta cerrando completamente.
        /// </summary>
        public void NotificarCierreAplicacionCompleta()
        {
            _aplicacionCerrando = true;
        }

        /// <summary>
        /// Determina si se debe ejecutar la accion al cerrar.
        /// </summary>
        /// <returns>True si se debe ejecutar la accion, false en caso contrario.</returns>
        public bool DebeEjecutarAccionAlCerrar()
        {
            return !_aplicacionCerrando;
        }

        private void Navegar(DestinoNavegacion destino)
        {
            _navegacionManejador.CerrarVentana = CerrarVentana;
            _navegacionManejador.Navegar(destino);
        }

        private void AbrirAjustesPartida()
        {
            var ajustesVistaModelo = new Ajustes.AjustesPartidaVistaModelo(
                _ventana,
                _localizador,
                _cancionManejador,
                _sonidoManejador);
            ajustesVistaModelo.SalirPartidaConfirmado = EjecutarSalidaPartidaConfirmada;

            _ventana.MostrarVentanaDialogo(ajustesVistaModelo);
        }

        private void EjecutarSalidaPartidaConfirmada()
        {
            NavegarSegunSesion();
        }
    }
}
