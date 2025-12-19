using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Salas
{
    /// <summary>
    /// Gestiona la logica del estado de una partida en curso.
    /// </summary>
    public class PartidaVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SonidoManejador _sonidoManejador;
        private readonly CancionManejador _cancionManejador;
        private readonly ICatalogoCanciones _catalogoCanciones;
        private readonly PartidaTemporizadores _temporizadores;

        private bool _juegoIniciado;
        private int _numeroRondaActual;
        private double _grosor;
        private Color _color;
        private int _contador;
        private int _tiempoRondaSegundos;
        private readonly Stopwatch _cronometroRonda;
        private string _textoContador;
        private Brush _colorContador;
        private bool _esHerramientaLapiz;
        private bool _esHerramientaBorrador;
        private Visibility _visibilidadCuadriculaDibujo;
        private Visibility _visibilidadOverlayDibujante;
        private Visibility _visibilidadOverlayAdivinador;
        private Visibility _visibilidadOverlayAlarma;
        private Visibility _visibilidadPalabraAdivinar;
        private Visibility _visibilidadInfoCancion;
        private string _palabraAdivinar;
        private string _textoArtista;
        private string _textoGenero;
        private Visibility _visibilidadArtista;
        private Visibility _visibilidadGenero;
        private bool _mostrarEstadoRonda;
        private int _turnosCompletadosEnCiclo;
        private bool _esDibujante;
        private bool _alarmaActiva;
        private DTOs.RondaDTO _rondaPendiente;
        private int _totalJugadoresPendiente;
        private string _nombreCancionActual;
        private string _archivoCancionActual;
        private Brush _colorPalabraAdivinar;
        private string _textoDibujoDe;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="PartidaVistaModelo"/>.
        /// </summary>
        /// <param name="ventana">Servicio de ventana.</param>
        /// <param name="localizador">Servicio localizador.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <param name="cancionManejador">Manejador de reproduccion de canciones.</param>
        /// <param name="catalogoCanciones">Catalogo de canciones disponibles.</param>
        public PartidaVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            SonidoManejador sonidoManejador,
            CancionManejador cancionManejador,
            ICatalogoCanciones catalogoCanciones)
            : base(ventana, localizador)
        {
            _cancionManejador = cancionManejador ??
                throw new ArgumentNullException(nameof(cancionManejador));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            _catalogoCanciones = catalogoCanciones ??
                throw new ArgumentNullException(nameof(catalogoCanciones));
            _temporizadores = new PartidaTemporizadores();
                
            _cronometroRonda = new Stopwatch();

            InicializarEstadoInicial();
            InicializarTemporizadores();
            InicializarComandos();
        }

        private void InicializarEstadoInicial()
        {
            _numeroRondaActual = 0;
            _grosor = 6;
            _color = Colors.Black;
            _contador = 0;
            _textoContador = string.Empty;
            _colorContador = Brushes.Black;
            _esHerramientaLapiz = true;
            _esHerramientaBorrador = false;
            _visibilidadCuadriculaDibujo = Visibility.Collapsed;
            _visibilidadOverlayDibujante = Visibility.Collapsed;
            _visibilidadOverlayAdivinador = Visibility.Collapsed;
            _visibilidadOverlayAlarma = Visibility.Collapsed;
            _visibilidadPalabraAdivinar = Visibility.Collapsed;
            _visibilidadInfoCancion = Visibility.Collapsed;
            _colorPalabraAdivinar = Brushes.Black;
            _visibilidadArtista = Visibility.Collapsed;
            _visibilidadGenero = Visibility.Collapsed;
            _mostrarEstadoRonda = false;
            _turnosCompletadosEnCiclo = 0;
            _nombreCancionActual = string.Empty;
            _archivoCancionActual = string.Empty;
            _textoDibujoDe = string.Empty;
        }

        private void InicializarTemporizadores()
        {
            _temporizadores.OverlayTick += OverlayTimer_Tick;
            _temporizadores.AlarmaTick += TemporizadorAlarma_Tick;
            _temporizadores.TemporizadorTick += Temporizador_Tick;
        }

        /// <summary>
        /// Indica si el juego ha iniciado.
        /// </summary>
        public bool JuegoIniciado
        {
            get => _juegoIniciado;
            private set
            {
                if (EstablecerPropiedad(ref _juegoIniciado, value))
                {
                    JuegoIniciadoCambiado?.Invoke(value);
                    if (!value)
                    {
                        _turnosCompletadosEnCiclo = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el numero de la ronda actual.
        /// </summary>
        public int NumeroRondaActual
        {
            get => _numeroRondaActual;
            private set => EstablecerPropiedad(ref _numeroRondaActual, value);
        }

        /// <summary>
        /// Obtiene o establece el grosor del trazo de dibujo.
        /// </summary>
        public double Grosor
        {
            get => _grosor;
            set => EstablecerPropiedad(ref _grosor, value);
        }

        /// <summary>
        /// Obtiene o establece el color del trazo de dibujo.
        /// </summary>
        public Color Color
        {
            get => _color;
            set => EstablecerPropiedad(ref _color, value);
        }

        /// <summary>
        /// Obtiene o establece el texto del contador de tiempo.
        /// </summary>
        public string TextoContador
        {
            get => _textoContador;
            set => EstablecerPropiedad(ref _textoContador, value);
        }

        /// <summary>
        /// Obtiene o establece el color del texto del contador.
        /// </summary>
        public Brush ColorContador
        {
            get => _colorContador;
            set => EstablecerPropiedad(ref _colorContador, value);
        }

        /// <summary>
        /// Indica si se debe mostrar el estado de la ronda.
        /// </summary>
        public bool MostrarEstadoRonda
        {
            get => _mostrarEstadoRonda;
            private set => EstablecerPropiedad(ref _mostrarEstadoRonda, value);
        }

        /// <summary>
        /// Indica si la herramienta actual es el lapiz.
        /// </summary>
        public bool EsHerramientaLapiz
        {
            get => _esHerramientaLapiz;
            set
            {
                if (EstablecerPropiedad(ref _esHerramientaLapiz, value))
                {
                    EsHerramientaBorrador = !value;
                    NotificarCambioHerramienta?.Invoke(value);
                }
            }
        }

        /// <summary>
        /// Indica si la herramienta actual es el borrador.
        /// </summary>
        public bool EsHerramientaBorrador
        {
            get => _esHerramientaBorrador;
            set
            {
                if (EstablecerPropiedad(ref _esHerramientaBorrador, value))
                {
                    if (value)
                    {
                        EsHerramientaLapiz = false;
                    }
                    NotificarCambioHerramienta?.Invoke(!value);
                }
            }
        }

        /// <summary>
        /// Visibilidad de la cuadricula de dibujo.
        /// </summary>
        public Visibility VisibilidadCuadriculaDibujo
        {
            get => _visibilidadCuadriculaDibujo;
            set => EstablecerPropiedad(ref _visibilidadCuadriculaDibujo, value);
        }

        /// <summary>
        /// Visibilidad del overlay para el dibujante.
        /// </summary>
        public Visibility VisibilidadOverlayDibujante
        {
            get => _visibilidadOverlayDibujante;
            set => EstablecerPropiedad(ref _visibilidadOverlayDibujante, value);
        }

        /// <summary>
        /// Visibilidad del overlay para el adivinador.
        /// </summary>
        public Visibility VisibilidadOverlayAdivinador
        {
            get => _visibilidadOverlayAdivinador;
            set => EstablecerPropiedad(ref _visibilidadOverlayAdivinador, value);
        }

        /// <summary>
        /// Visibilidad del overlay de alarma de tiempo.
        /// </summary>
        public Visibility VisibilidadOverlayAlarma
        {
            get => _visibilidadOverlayAlarma;
            set => EstablecerPropiedad(ref _visibilidadOverlayAlarma, value);
        }

        /// <summary>
        /// Visibilidad de la palabra a adivinar.
        /// </summary>
        public Visibility VisibilidadPalabraAdivinar
        {
            get => _visibilidadPalabraAdivinar;
            set => EstablecerPropiedad(ref _visibilidadPalabraAdivinar, value);
        }

        /// <summary>
        /// Visibilidad de la informacion de la cancion.
        /// </summary>
        public Visibility VisibilidadInfoCancion
        {
            get => _visibilidadInfoCancion;
            set => EstablecerPropiedad(ref _visibilidadInfoCancion, value);
        }
        /// <summary>
        /// Visibilidad del texto de artista.
        /// </summary>
        public Visibility VisibilidadArtista
        {
            get => _visibilidadArtista;
            set => EstablecerPropiedad(ref _visibilidadArtista, value);
        }

        /// <summary>
        /// Visibilidad del texto de genero musical.
        /// </summary>
        public Visibility VisibilidadGenero
        {
            get => _visibilidadGenero;
            set => EstablecerPropiedad(ref _visibilidadGenero, value);
        }

        /// <summary>
        /// Palabra que se debe dibujar o adivinar.
        /// </summary>
        public string PalabraAdivinar
        {
            get => _palabraAdivinar;
            set => EstablecerPropiedad(ref _palabraAdivinar, value);
        }

        /// <summary>
        /// Color del texto de la palabra a adivinar.
        /// </summary>
        public Brush ColorPalabraAdivinar
        {
            get => _colorPalabraAdivinar;
            set => EstablecerPropiedad(ref _colorPalabraAdivinar, value);
        }

        /// <summary>
        /// Nombre del artista de la cancion actual.
        /// </summary>
        public string TextoArtista
        {
            get => _textoArtista;
            set => EstablecerPropiedad(ref _textoArtista, value);
        }

        /// <summary>
        /// Genero musical de la cancion actual.
        /// </summary>
        public string TextoGenero
        {
            get => _textoGenero;
            set => EstablecerPropiedad(ref _textoGenero, value);
        }

        /// <summary>
        /// Texto que muestra quien es el dibujante actual.
        /// </summary>
        public string TextoDibujoDe
        {
            get => _textoDibujoDe;
            set => EstablecerPropiedad(ref _textoDibujoDe, value);
        }

        /// <summary>
        /// Indica si el usuario es el dibujante de la ronda.
        /// </summary>
        public bool EsDibujante
        {
            get => _esDibujante;
            private set => EstablecerPropiedad(ref _esDibujante, value);
        }

        /// <summary>
        /// Obtiene el manejador de reproduccion de canciones.
        /// </summary>
        public CancionManejador CancionManejador => _cancionManejador;

        /// <summary>
        /// Comando para seleccionar el lapiz como herramienta.
        /// </summary>
        public ICommand SeleccionarLapizComando { get; private set; }

        /// <summary>
        /// Comando para seleccionar el borrador como herramienta.
        /// </summary>
        public ICommand SeleccionarBorradorComando { get; private set; }

        /// <summary>
        /// Comando para cambiar el grosor del trazo.
        /// </summary>
        public ICommand CambiarGrosorComando { get; private set; }

        /// <summary>
        /// Comando para cambiar el color del trazo.
        /// </summary>
        public ICommand CambiarColorComando { get; private set; }

        /// <summary>
        /// Comando para limpiar el lienzo de dibujo.
        /// </summary>
        public ICommand LimpiarDibujoComando { get; private set; }

        /// <summary>
        /// Comando para ocultar el overlay de tiempo terminado.
        /// </summary>
        public ICommand OcultarOverlayAlarmaComando { get; private set; }

        /// <summary>
        /// Accion para notificar cambio de herramienta a la vista.
        /// </summary>
        public Action<bool> NotificarCambioHerramienta { get; set; }

        /// <summary>
        /// Accion para aplicar estilo visual de lapiz.
        /// </summary>
        public Action AplicarEstiloLapiz { get; set; }

        /// <summary>
        /// Accion para actualizar cursor de goma.
        /// </summary>
        public Action ActualizarFormaGoma { get; set; }

        /// <summary>
        /// Accion para limpiar el Canvas.
        /// </summary>
        public Action LimpiarTrazos { get; set; }

        /// <summary>
        /// Evento para trazo recibido desde el servidor.
        /// </summary>
        public event Action<DTOs.TrazoDTO> TrazoRecibidoServidor;

        /// <summary>
        /// Evento que notifica cuando cambia el estado de juego iniciado.
        /// </summary>
        public event Action<bool> JuegoIniciadoCambiado;

        /// <summary>
        /// Accion para enviar trazo al servidor.
        /// </summary>
        public Action<DTOs.TrazoDTO> EnviarTrazoAlServidor { get; set; }

        /// <summary>
        /// Accion para notificar que el trazo es demasiado grande.
        /// </summary>
        public Action NotificarTrazoDemasiadoGrande { get; set; }

        /// <summary>
        /// Evento que notifica cuando cambia el estado de poder escribir.
        /// </summary>
        public event Action<bool> PuedeEscribirCambiado;

        /// <summary>
        /// Evento que notifica cuando cambia el rol de dibujante.
        /// </summary>
        public event Action<bool> EsDibujanteCambiado;

        /// <summary>
        /// Evento que notifica cuando cambia el nombre de la cancion correcta.
        /// </summary>
        public event Action<string> NombreCancionCambiado;

        /// <summary>
        /// Evento que notifica cuando cambia el tiempo restante.
        /// </summary>
        public event Action<int> TiempoRestanteCambiado;

        /// <summary>
        /// Evento de celebracion de fin de ronda temprano terminada.
        /// </summary>
        public event Action CelebracionFinRondaTerminada;

        private void InicializarComandos()
        {
            SeleccionarLapizComando = new ComandoDelegado(EjecutarComandoSeleccionarLapiz);
            SeleccionarBorradorComando = new ComandoDelegado(EjecutarComandoSeleccionarBorrador);
            CambiarGrosorComando = new ComandoDelegado(EjecutarComandoCambiarGrosor);
            CambiarColorComando = new ComandoDelegado(EjecutarComandoCambiarColor);
            LimpiarDibujoComando = new ComandoDelegado(EjecutarComandoLimpiarDibujo);
            OcultarOverlayAlarmaComando = new ComandoDelegado(EjecutarComandoOcultarOverlayAlarma);
        }

        private void EjecutarComandoSeleccionarLapiz(object parametro)
        {
            EjecutarSeleccionarLapiz();
        }

        private void EjecutarComandoSeleccionarBorrador(object parametro)
        {
            EjecutarSeleccionarBorrador();
        }

        private void EjecutarComandoCambiarGrosor(object parametro)
        {
            EjecutarCambiarGrosor(parametro);
        }

        private void EjecutarComandoCambiarColor(object parametro)
        {
            EjecutarCambiarColor(parametro);
        }

        private void EjecutarComandoLimpiarDibujo(object parametro)
        {
            EjecutarLimpiarDibujo();
        }

        private void EjecutarComandoOcultarOverlayAlarma(object parametro)
        {
            OcultarOverlayAlarma();
        }

        private static Visibility DeterminarVisibilidadPista(string textoPista)
        {
            return string.IsNullOrWhiteSpace(textoPista)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// Aplica los cambios visuales al iniciar la partida.
        /// </summary>
        /// <param name="totalJugadores">Numero total de jugadores en la sala.</param>
        public void AplicarInicioVisualPartida(int totalJugadores)
        {
            RegistrarInicioPartida();
            ReiniciarEstadoOverlays();
            InicializarEstadoPartida();
            ConfigurarHerramientasIniciales();
        }

        private static void RegistrarInicioPartida()
        {
            _logger.Info("Iniciando partida...");
        }

        private void ReiniciarEstadoOverlays()
        {
            _alarmaActiva = false;
            VisibilidadOverlayAlarma = Visibility.Collapsed;
            VisibilidadOverlayDibujante = Visibility.Collapsed;
            VisibilidadOverlayAdivinador = Visibility.Collapsed;
            VisibilidadPalabraAdivinar = Visibility.Collapsed;
            VisibilidadInfoCancion = Visibility.Collapsed;
        }

        private void InicializarEstadoPartida()
        {
            JuegoIniciado = true;
            NumeroRondaActual = 0;
            _turnosCompletadosEnCiclo = 0;
            MostrarEstadoRonda = false;
            VisibilidadCuadriculaDibujo = Visibility.Visible;
        }

        private void ConfigurarHerramientasIniciales()
        {
            EsHerramientaLapiz = true;
            AplicarEstiloLapiz?.Invoke();
            ActualizarFormaGoma?.Invoke();
        }

        private void EjecutarSeleccionarLapiz()
        {
            EsHerramientaLapiz = true;
        }

        private void EjecutarSeleccionarBorrador()
        {
            EsHerramientaBorrador = true;
        }

        private void EjecutarCambiarGrosor(object parametro)
        {
            if (parametro != null &&
                double.TryParse(parametro.ToString(), out var nuevoGrosor))
            {
                Grosor = nuevoGrosor;
                if (EsHerramientaLapiz)
                {
                    AplicarEstiloLapiz?.Invoke();
                }
                else
                {
                    ActualizarFormaGoma?.Invoke();
                }
            }
        }

        private void EjecutarCambiarColor(object parametro)
        {
            if (parametro is string colorName)
            {
                Color = (Color)ColorConverter.ConvertFromString(colorName);
                EsHerramientaLapiz = true;
                AplicarEstiloLapiz?.Invoke();
            }
        }

        private void EjecutarLimpiarDibujo()
        {
            LimpiarTrazos?.Invoke();

            var trazoLimpiar = new DTOs.TrazoDTO
            {
                PuntosX = Array.Empty<double>(),
                PuntosY = Array.Empty<double>(),
                ColorHex = string.Empty,
                Grosor = 0,
                EsBorrado = true,
                EsLimpiarTodo = true
            };

            EnviarTrazoAlServidor?.Invoke(trazoLimpiar);
        }

        private void MostrarOverlayAlarma()
        {
            _alarmaActiva = true;

            if (!string.IsNullOrWhiteSpace(_nombreCancionActual))
            {
                PalabraAdivinar = _nombreCancionActual;
                _cancionManejador.Reproducir(_archivoCancionActual);
            }

            VisibilidadPalabraAdivinar = Visibility.Visible;
            ColorPalabraAdivinar = Brushes.Blue;
            VisibilidadOverlayAlarma = Visibility.Visible;

            _temporizadores.DetenerAlarma();
            _temporizadores.IniciarAlarma();
        }

        private void OcultarOverlayAlarma()
        {
            FinalizarAlarma();
        }

        private void OverlayTimer_Tick(object remitente, EventArgs argumentosEvento)
        {
            _temporizadores.DetenerOverlay();
            VisibilidadOverlayDibujante = Visibility.Collapsed;
            VisibilidadOverlayAdivinador = Visibility.Collapsed;
            HabilitarEscrituraTrasOverlay();
            IniciarTemporizador();
        }

        private void HabilitarEscrituraTrasOverlay()
        {
            if (_alarmaActiva)
            {
                return;
            }

            bool puedeEscribir = !EsDibujante;
            PuedeEscribirCambiado?.Invoke(puedeEscribir);
        }

        private void IniciarTemporizador()
        {
            if (_alarmaActiva)
            {
                return;
            }

            OcultarOverlayAlarma();

            _cronometroRonda.Restart();
            _contador = Math.Max(0, _contador);
            TextoContador = _contador.ToString();
            ColorContador = Brushes.Black;

            _temporizadores.IniciarTemporizador();
        }

        private void Temporizador_Tick(object remitente, EventArgs argumentosEvento)
        {
            var tiempoTranscurrido = (int)_cronometroRonda.Elapsed.TotalSeconds;
            _contador = Math.Max(0, _tiempoRondaSegundos - tiempoTranscurrido);
            TextoContador = _contador.ToString();
            TiempoRestanteCambiado?.Invoke(_contador);

            if (_contador <= 0)
            {
                _temporizadores.DetenerTemporizador();
                TextoContador = "0";
                _cancionManejador.Detener();

                VisibilidadPalabraAdivinar = Visibility.Collapsed;
                VisibilidadInfoCancion = Visibility.Collapsed;
                VisibilidadArtista = Visibility.Collapsed;
                VisibilidadGenero = Visibility.Collapsed;

                MostrarOverlayAlarma();
            }
        }

        private void TemporizadorAlarma_Tick(object remitente, EventArgs argumentosEvento)
        {
            FinalizarAlarma();
        }

        private void FinalizarAlarma()
        {
            if (!_alarmaActiva && VisibilidadOverlayAlarma == Visibility.Collapsed)
            {
                return;
            }

            _temporizadores.DetenerAlarma();
            _cancionManejador.Detener();
            VisibilidadOverlayAlarma = Visibility.Collapsed;
            _alarmaActiva = false;

            RestablecerPalabraTrasAlarma();

            if (_rondaPendiente != null)
            {
                var rondaPendiente = _rondaPendiente;
                _rondaPendiente = null;
                ProcesarInicioRonda(rondaPendiente);
            }
            else
            {
                CelebracionFinRondaTerminada?.Invoke();
            }
        }

        private void RestablecerPalabraTrasAlarma()
        {
            ColorPalabraAdivinar = Brushes.Black;
            PalabraAdivinar = string.Empty;
            VisibilidadPalabraAdivinar = Visibility.Collapsed;
            VisibilidadInfoCancion = Visibility.Collapsed;
        }

        /// <summary>
        /// Procesa la notificacion de que la partida ha iniciado.
        /// </summary>
        public void NotificarPartidaIniciada()
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(() =>
            {
                if (MostrarEstadoRonda)
                {
                    return;
                }

                AplicarInicioVisualPartida(0);
                TextoContador = string.Empty;
                _sonidoManejador.ReproducirNotificacion();
            }));
        }

        /// <summary>
        /// Procesa la notificacion de inicio de una nueva ronda.
        /// </summary>
        /// <param name="ronda">Datos de la ronda.</param>
        /// <param name="totalJugadores">Numero total de jugadores.</param>
        public void NotificarInicioRonda(DTOs.RondaDTO ronda, int totalJugadores)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(() =>
            {
                if (ronda == null)
                {
                    return;
                }

                _totalJugadoresPendiente = totalJugadores;

                if (_alarmaActiva)
                {
                    _rondaPendiente = ronda;
                    return;
                }

                ProcesarInicioRonda(ronda);
            }));
        }

        private void ProcesarInicioRonda(DTOs.RondaDTO ronda)
        {
            _rondaPendiente = null;
            DetenerTemporizadoresYAlarma();
            PrepararEstadoVisualRonda();
            ConfigurarTiempoRonda(ronda.TiempoSegundos);
            ActualizarContadorRondas(_totalJugadoresPendiente);

            ResultadoOperacion<Cancion> resultadoCancion = _catalogoCanciones.ObtenerPorId(ronda.IdCancion);
            Cancion cancion = resultadoCancion.Exitoso ? resultadoCancion.Valor : Cancion.Vacia;
            AlmacenarDatosCancionActual(cancion);
            NotificarCambiosCancionYTiempo();
            ConfigurarTextoDibujante(ronda.NombreDibujante);

            bool esDibujanteRol = string.Equals(
                ronda.Rol,
                "Dibujante",
                StringComparison.OrdinalIgnoreCase);

            if (esDibujanteRol)
            {
                ConfigurarRolDibujante(cancion, ronda);
            }
            else
            {
                ConfigurarRolAdivinador(ronda);
            }
        }

        private void DetenerTemporizadoresYAlarma()
        {
            _temporizadores.DetenerTodos();
            _cancionManejador.Detener();
            _alarmaActiva = false;
            VisibilidadOverlayAlarma = Visibility.Collapsed;
            LimpiarTrazos?.Invoke();
        }

        private void PrepararEstadoVisualRonda()
        {
            ColorContador = Brushes.Black;
            VisibilidadCuadriculaDibujo = Visibility.Visible;
            MostrarEstadoRonda = true;
            ColorPalabraAdivinar = Brushes.Black;
        }

        private void ConfigurarTiempoRonda(int tiempoSegundos)
        {
            _tiempoRondaSegundos = tiempoSegundos;
            _contador = tiempoSegundos;
            TextoContador = _contador.ToString();
        }

        private void AlmacenarDatosCancionActual(Cancion cancion)
        {
            _nombreCancionActual = cancion?.Nombre ?? string.Empty;
            _archivoCancionActual = cancion?.Archivo ?? string.Empty;
        }

        private void NotificarCambiosCancionYTiempo()
        {
            NombreCancionCambiado?.Invoke(_nombreCancionActual);
            TiempoRestanteCambiado?.Invoke(_contador);
        }

        private void ConfigurarTextoDibujante(string nombreDibujante)
        {
            TextoDibujoDe = string.IsNullOrWhiteSpace(nombreDibujante)
                ? string.Empty
                : string.Format(Lang.partidaTextoDibujoDe, nombreDibujante);
        }

        private void ConfigurarRolDibujante(Cancion cancion, DTOs.RondaDTO ronda)
        {
            EsDibujante = true;
            EsDibujanteCambiado?.Invoke(true);
            PuedeEscribirCambiado?.Invoke(false);

            ConfigurarPalabraParaDibujante(cancion?.Nombre);
            ConfigurarPistas(ronda);
            ReproducirCancionSiExiste(cancion?.Archivo);
            MostrarOverlayDibujante();
        }

        private void ConfigurarPalabraParaDibujante(string nombreCancion)
        {
            PalabraAdivinar = string.IsNullOrWhiteSpace(nombreCancion)
                ? PalabraAdivinar
                : nombreCancion;
            VisibilidadPalabraAdivinar = Visibility.Visible;
        }

        private void ConfigurarPistas(DTOs.RondaDTO ronda)
        {
            TextoArtista = FormatearPistaArtista(ronda.PistaArtista);
            TextoGenero = FormatearPistaGenero(ronda.PistaGenero);
            VisibilidadArtista = DeterminarVisibilidadPista(TextoArtista);
            VisibilidadGenero = DeterminarVisibilidadPista(TextoGenero);
            VisibilidadInfoCancion = Visibility.Visible;
        }

        private void ReproducirCancionSiExiste(string archivoCancion)
        {
            if (!string.IsNullOrWhiteSpace(archivoCancion))
            {
                _cancionManejador.Reproducir(archivoCancion);
            }
        }

        private void MostrarOverlayDibujante()
        {
            VisibilidadOverlayAdivinador = Visibility.Collapsed;
            VisibilidadOverlayDibujante = Visibility.Visible;
            _temporizadores.DetenerOverlay();
            _temporizadores.IniciarOverlay();
        }

        private void ConfigurarRolAdivinador(DTOs.RondaDTO ronda)
        {
            EsDibujante = false;
            EsDibujanteCambiado?.Invoke(false);
            PuedeEscribirCambiado?.Invoke(false);

            OcultarPalabraParaAdivinador();
            ConfigurarPistas(ronda);
            _cancionManejador.Detener();
            MostrarOverlayAdivinador();
        }

        private void OcultarPalabraParaAdivinador()
        {
            PalabraAdivinar = string.Empty;
            VisibilidadPalabraAdivinar = Visibility.Collapsed;
        }

        private void MostrarOverlayAdivinador()
        {
            VisibilidadOverlayDibujante = Visibility.Collapsed;
            VisibilidadOverlayAdivinador = Visibility.Visible;
            _temporizadores.DetenerOverlay();
            _temporizadores.IniciarOverlay();
        }

        private static string FormatearPistaArtista(string pistaArtista)
        {
            return string.IsNullOrWhiteSpace(pistaArtista)
                ? string.Empty
                : string.Format("Artista: {0}", pistaArtista);
        }

        private static string FormatearPistaGenero(string pistaGenero)
        {
            return string.IsNullOrWhiteSpace(pistaGenero)
                ? string.Empty
                : string.Format("Genero: {0}", pistaGenero);
        }

        /// <summary>
        /// Procesa la notificacion de que un jugador adivino la cancion.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador que adivino.</param>
        /// <param name="puntos">Puntos obtenidos.</param>
        /// <param name="nombreUsuarioSesion">Nombre del usuario de la sesion actual.</param>
        public void NotificarJugadorAdivino(
            string nombreJugador,
            int puntos,
            string nombreUsuarioSesion)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(() =>
            {
                _sonidoManejador.ReproducirNotificacion();

                if (string.Equals(
                    nombreJugador,
                    nombreUsuarioSesion,
                    StringComparison.OrdinalIgnoreCase))
                {
                    PuedeEscribirCambiado?.Invoke(false);
                }
            }));
        }

        /// <summary>
        /// Procesa la recepcion de un trazo desde el servidor.
        /// </summary>
        /// <param name="trazo">Datos del trazo.</param>
        public void NotificarTrazoRecibido(DTOs.TrazoDTO trazo)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(() => TrazoRecibidoServidor?.Invoke(trazo)));
        }

        /// <summary>
        /// Procesa la notificacion de fin de ronda.
        /// </summary>
        public void NotificarFinRonda()
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(() =>
            {
                if (_alarmaActiva)
                {
                    return;
                }

                _temporizadores.DetenerTemporizador();
                _temporizadores.DetenerOverlay();
                _cancionManejador.Detener();
                LimpiarTrazos?.Invoke();
                PuedeEscribirCambiado?.Invoke(false);
                MostrarEstadoRonda = false;
                TextoContador = string.Empty;
                MostrarOverlayAlarma();
            }));
        }

        /// <summary>
        /// Procesa el fin de ronda temprano cuando todos los adivinadores acertaron.
        /// Muestra la cancion en azul y la reproduce durante 5 segundos.
        /// </summary>
        public void NotificarFinRondaTemprano()
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(() =>
            {
                _temporizadores.DetenerTemporizador();
                _temporizadores.DetenerOverlay();
                LimpiarTrazos?.Invoke();
                PuedeEscribirCambiado?.Invoke(false);
                MostrarEstadoRonda = false;
                TextoContador = string.Empty;

                if (!string.IsNullOrWhiteSpace(_nombreCancionActual))
                {
                    PalabraAdivinar = _nombreCancionActual;
                }

                VisibilidadPalabraAdivinar = Visibility.Visible;
                ColorPalabraAdivinar = Brushes.Blue;

                if (!string.IsNullOrWhiteSpace(_archivoCancionActual))
                {
                    _cancionManejador.Reproducir(_archivoCancionActual);
                }

                _alarmaActiva = true;
                _temporizadores.DetenerAlarma();
                _temporizadores.IniciarAlarma();
            }));
        }

        /// <summary>
        /// Procesa la notificacion de fin de partida.
        /// </summary>
        public void NotificarFinPartida()
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(new Action(() =>
            {
                _temporizadores.DetenerTodos();
                _cancionManejador.Detener();
                JuegoIniciado = false;
                MostrarEstadoRonda = false;
                TextoContador = string.Empty;
                NumeroRondaActual = 0;
                PuedeEscribirCambiado?.Invoke(false);
                _alarmaActiva = false;
                _rondaPendiente = null;
                _nombreCancionActual = string.Empty;
                _archivoCancionActual = string.Empty;
                VisibilidadOverlayAlarma = Visibility.Collapsed;
                VisibilidadOverlayDibujante = Visibility.Collapsed;
                VisibilidadOverlayAdivinador = Visibility.Collapsed;
                RestablecerPalabraTrasAlarma();
            }));
        }

        /// <summary>
        /// Ajusta el progreso de ronda despues de un cambio en jugadores.
        /// </summary>
        /// <param name="totalJugadores">Numero total de jugadores.</param>
        public void AjustarProgresoRondaTrasCambioJugadores(int totalJugadores)
        {
            if (totalJugadores <= 0)
            {
                _turnosCompletadosEnCiclo = 0;
                NumeroRondaActual = 0;
                return;
            }

            _turnosCompletadosEnCiclo = Math.Min(_turnosCompletadosEnCiclo, totalJugadores);
        }

        private void ActualizarContadorRondas(int totalJugadores)
        {
            if (totalJugadores <= 0)
            {
                NumeroRondaActual = 0;
                _turnosCompletadosEnCiclo = 0;
                return;
            }

            _turnosCompletadosEnCiclo = (_turnosCompletadosEnCiclo % totalJugadores) + 1;

            if (NumeroRondaActual == 0)
            {
                NumeroRondaActual = 1;
            }
            else if (_turnosCompletadosEnCiclo == 1)
            {
                NumeroRondaActual++;
            }
        }

        /// <summary>
        /// Detiene todos los temporizadores y libera recursos.
        /// </summary>
        public void Detener()
        {
            _temporizadores.DetenerTodos();
            _cronometroRonda.Stop();
            _cancionManejador.Detener();
        }

        /// <summary>
        /// Reinicia el estado visual para mostrar la sala cancelada.
        /// </summary>
        public void ReiniciarEstadoVisualSalaCancelada()
        {
            _temporizadores.DetenerTodos();
            _cancionManejador.Detener();

            JuegoIniciado = false;
            MostrarEstadoRonda = false;
            TextoContador = string.Empty;
            NumeroRondaActual = 0;
            VisibilidadCuadriculaDibujo = Visibility.Collapsed;
            VisibilidadOverlayAdivinador = Visibility.Collapsed;
            VisibilidadOverlayDibujante = Visibility.Collapsed;
            VisibilidadOverlayAlarma = Visibility.Collapsed;
        }
    }
}
