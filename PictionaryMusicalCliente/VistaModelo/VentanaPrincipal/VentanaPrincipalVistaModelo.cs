using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using PictionaryMusicalCliente.VistaModelo.VentanaPrincipal.Auxiliares;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.ClienteServicios.Wcf;

namespace PictionaryMusicalCliente.VistaModelo.VentanaPrincipal
{
    /// <summary>
    /// ViewModel principal que gestiona la pantalla de inicio del usuario autenticado.
    /// Coordina la creacion de salas, union a partidas y gestion de amigos.
    /// </summary>
    public class VentanaPrincipalVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _nombreUsuario;
        private string _codigoSala;
        private OpcionEntero _numeroRondasSeleccionada;
        private OpcionEntero _tiempoRondaSeleccionada;
        private IdiomaOpcion _idiomaSeleccionado;
        private OpcionTexto _dificultadSeleccionada;

        private readonly string _nombreUsuarioSesion;
        private readonly ILocalizacionServicio _localizacion;
        private readonly IAmigosServicio _amigosServicio;
        private readonly ISalasServicio _salasServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IUsuarioAutenticado _usuarioSesion;
        private readonly OpcionesPartidaManejador _opcionesPartida;
        private readonly GestorListaAmigos _gestorAmigos;

        private bool _desconexionInternetProcesada;

        public VentanaPrincipalVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            VentanaPrincipalDependencias dependencias)
            : base(ventana, localizador)
        {
            if (dependencias == null)
            {
                throw new ArgumentNullException(nameof(dependencias));
            }

            _localizacion = dependencias.LocalizacionServicio;
            _amigosServicio = dependencias.AmigosServicio;
            _salasServicio = dependencias.SalasServicio;
            _sonidoManejador = dependencias.SonidoManejador;
            _usuarioSesion = dependencias.UsuarioSesion;

            _nombreUsuarioSesion = _usuarioSesion.NombreUsuario ?? string.Empty;
            _opcionesPartida = new OpcionesPartidaManejador();

            _gestorAmigos = CrearGestorAmigos(dependencias);
            SuscribirEventosGestorAmigos();
            SuscribirEventosConectividad();

            CargarDatosUsuario();
            InicializarOpcionesSeleccionadas();
            InicializarComandos();
        }

        private GestorListaAmigos CrearGestorAmigos(
            VentanaPrincipalDependencias dependencias)
        {
            var parametros = new GestorListaAmigosParametros
            {
                ListaAmigosServicio = dependencias.ListaAmigosServicio,
                AmigosServicio = dependencias.AmigosServicio,
                NombreUsuario = _nombreUsuarioSesion,
                EjecutarEnDispatcher = EjecutarEnDispatcher
            };

            return new GestorListaAmigos(parametros);
        }

        private void SuscribirEventosGestorAmigos()
        {
            _gestorAmigos.CanalDesconectado += OnCanalAmigosDesconectado;
            _gestorAmigos.SolicitudesActualizadas += OnSolicitudesActualizadas;
        }

        private void SuscribirEventosConectividad()
        {
            ConectividadRedMonitor.Instancia.ConexionPerdida += EnConexionInternetPerdida;
            DesconexionDetectada += ManejarDesconexion;
        }

        private void InicializarComandos()
        {
            AbrirPerfilComando = new ComandoDelegado(EjecutarComandoAbrirPerfil);
            AbrirAjustesComando = new ComandoDelegado(EjecutarComandoAbrirAjustes);
            AbrirComoJugarComando = new ComandoDelegado(EjecutarComandoAbrirComoJugar);
            AbrirClasificacionComando = new ComandoDelegado(EjecutarComandoAbrirClasificacion);
            AbrirBuscarAmigoComando = new ComandoDelegado(EjecutarComandoAbrirBuscarAmigo);
            AbrirSolicitudesComando = new ComandoDelegado(EjecutarComandoAbrirSolicitudes);
            EliminarAmigoComando = new ComandoAsincrono(EjecutarComandoEliminarAmigoAsync, 
                ValidarParametroAmigoDTO);
            UnirseSalaComando = new ComandoAsincrono(EjecutarComandoUnirseSalaAsync);
            IniciarJuegoComando = new ComandoAsincrono(EjecutarComandoIniciarJuegoAsync, 
                ValidarPuedeIniciarJuego);
        }

        private void EjecutarComandoAbrirPerfil(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            EjecutarAbrirPerfil();
        }

        private void EjecutarComandoAbrirAjustes(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            EjecutarAbrirAjustes();
        }

        private void EjecutarComandoAbrirComoJugar(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            EjecutarAbrirComoJugar();
        }

        private void EjecutarComandoAbrirClasificacion(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            EjecutarAbrirClasificacion();
        }

        private void EjecutarComandoAbrirBuscarAmigo(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            EjecutarAbrirBuscarAmigo();
        }

        private void EjecutarComandoAbrirSolicitudes(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            EjecutarAbrirSolicitudes();
        }

        private async Task EjecutarComandoEliminarAmigoAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await EjecutarEliminarAmigoAsync(parametro as DTOs.AmigoDTO);
        }

        private static bool ValidarParametroAmigoDTO(object parametro)
        {
            return parametro is DTOs.AmigoDTO;
        }

        private async Task EjecutarComandoUnirseSalaAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await UnirseSalaInternoAsync();
        }

        private async Task EjecutarComandoIniciarJuegoAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await IniciarJuegoInternoAsync();
        }

        private bool ValidarPuedeIniciarJuego(object parametro)
        {
            return PuedeIniciarJuego();
        }

        /// <summary>
        /// Obtiene el nombre del usuario autenticado.
        /// </summary>
        public string NombreUsuario
        {
            get => _nombreUsuario;
            private set => EstablecerPropiedad(ref _nombreUsuario, value);
        }

        /// <summary>
        /// Obtiene o establece el codigo de sala para unirse.
        /// </summary>
        public string CodigoSala
        {
            get => _codigoSala;
            set => EstablecerPropiedad(ref _codigoSala, value);
        }

        /// <summary>
        /// Obtiene la coleccion de opciones disponibles para el numero de rondas.
        /// </summary>
        public ObservableCollection<OpcionEntero> NumeroRondasOpciones 
            => _opcionesPartida.NumeroRondasOpciones;

        /// <summary>
        /// Obtiene o establece el numero de rondas seleccionado para la partida.
        /// </summary>
        public OpcionEntero NumeroRondasSeleccionada
        {
            get => _numeroRondasSeleccionada;
            set
            {
                if (EstablecerPropiedad(ref _numeroRondasSeleccionada, value))
                {
                    ActualizarEstadoIniciarJuego();
                }
            }
        }

        /// <summary>
        /// Obtiene la coleccion de opciones disponibles para el tiempo de ronda.
        /// </summary>
        public ObservableCollection<OpcionEntero> TiempoRondaOpciones 
            => _opcionesPartida.TiempoRondaOpciones;

        /// <summary>
        /// Obtiene o establece el tiempo de ronda seleccionado en segundos.
        /// </summary>
        public OpcionEntero TiempoRondaSeleccionada
        {
            get => _tiempoRondaSeleccionada;
            set
            {
                if (EstablecerPropiedad(ref _tiempoRondaSeleccionada, value))
                {
                    ActualizarEstadoIniciarJuego();
                }
            }
        }

        /// <summary>
        /// Obtiene la coleccion de idiomas disponibles para las canciones.
        /// </summary>
        public ObservableCollection<IdiomaOpcion> IdiomasDisponibles 
            => _opcionesPartida.IdiomasDisponibles;

        /// <summary>
        /// Obtiene o establece el idioma seleccionado para las canciones de la partida.
        /// </summary>
        public IdiomaOpcion IdiomaSeleccionado
        {
            get => _idiomaSeleccionado;
            set
            {
                if (EstablecerPropiedad(ref _idiomaSeleccionado, value) && value != null)
                {
                    ActualizarEstadoIniciarJuego();
                }
            }
        }

        /// <summary>
        /// Obtiene la coleccion de niveles de dificultad disponibles.
        /// </summary>
        public ObservableCollection<OpcionTexto> DificultadesDisponibles 
            => _opcionesPartida.DificultadesDisponibles;

        /// <summary>
        /// Obtiene o establece el nivel de dificultad seleccionado para la partida.
        /// </summary>
        public OpcionTexto DificultadSeleccionada
        {
            get => _dificultadSeleccionada;
            set
            {
                if (EstablecerPropiedad(ref _dificultadSeleccionada, value))
                {
                    ActualizarEstadoIniciarJuego();
                }
            }
        }

        /// <summary>
        /// Obtiene la coleccion de amigos del usuario autenticado.
        /// </summary>
        public ObservableCollection<DTOs.AmigoDTO> Amigos => _gestorAmigos.Amigos;

        /// <summary>
        /// Obtiene o establece el amigo seleccionado en la lista.
        /// </summary>
        public DTOs.AmigoDTO AmigoSeleccionado
        {
            get => _gestorAmigos.AmigoSeleccionado;
            set
            {
                _gestorAmigos.SeleccionarAmigo(value);
                NotificarCambio(nameof(AmigoSeleccionado));
            }
        }

        /// <summary>
        /// Obtiene el comando para abrir la ventana del perfil del usuario.
        /// </summary>
        public ICommand AbrirPerfilComando { get; private set; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de ajustes.
        /// </summary>
        public ICommand AbrirAjustesComando { get; private set; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de instrucciones del juego.
        /// </summary>
        public ICommand AbrirComoJugarComando { get; private set; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de clasificacion.
        /// </summary>
        public ICommand AbrirClasificacionComando { get; private set; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de busqueda de amigos.
        /// </summary>
        public ICommand AbrirBuscarAmigoComando { get; private set; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de solicitudes de amistad.
        /// </summary>
        public ICommand AbrirSolicitudesComando { get; private set; }

        /// <summary>
        /// Obtiene el comando asincrono para eliminar un amigo de la lista.
        /// </summary>
        public IComandoAsincrono EliminarAmigoComando { get; private set; }

        /// <summary>
        /// Obtiene el comando asincrono para unirse a una sala existente.
        /// </summary>
        public IComandoAsincrono UnirseSalaComando { get; private set; }

        /// <summary>
        /// Obtiene el comando asincrono para iniciar una nueva partida.
        /// </summary>
        public IComandoAsincrono IniciarJuegoComando { get; private set; }

        /// <summary>
        /// Inicializa las suscripciones a servicios y carga la lista de amigos.
        /// </summary>
        /// <returns>Tarea que representa la operacion asincrona.</returns>
        public async Task InicializarAsync()
        {
            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                await _gestorAmigos.InicializarAsync().ConfigureAwait(false);
            });
        }

        private void OnCanalAmigosDesconectado(object remitente, EventArgs argumentosEvento)
        {
            EjecutarEnDispatcher(ManejarDesconexionCanalAmigos);
        }

        private void OnSolicitudesActualizadas(object remitente, EventArgs argumentosEvento)
        {
            _ = EjecutarOperacionConDesconexionAsync(async () =>
            {
                await _gestorAmigos.ActualizarListaAmigosDesdeServidorAsync()
                    .ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Finaliza las suscripciones a servicios y libera recursos.
        /// </summary>
        /// <returns>Tarea que representa la operacion asincrona.</returns>
        public async Task FinalizarAsync()
        {
            DesuscribirEventos();

            if (string.IsNullOrWhiteSpace(_nombreUsuarioSesion))
            {
                return;
            }

            await EjecutarOperacionAsync(async () =>
            {
                await _gestorAmigos.FinalizarAsync().ConfigureAwait(false);
            });
        }

        private void DesuscribirEventos()
        {
            _gestorAmigos.CanalDesconectado -= OnCanalAmigosDesconectado;
            _gestorAmigos.SolicitudesActualizadas -= OnSolicitudesActualizadas;
            ConectividadRedMonitor.Instancia.ConexionPerdida -= EnConexionInternetPerdida;
            DesconexionDetectada -= ManejarDesconexion;
        }

        private void ManejarDesconexionCanalAmigos()
        {
            _sonidoManejador.ReproducirError();
            ReiniciarAplicacion();
            
            string mensaje = ConectividadRedMonitor.HayConexion
                ? Lang.errorTextoSesionExpiradaGenerico
                : Lang.errorTextoPerdidaConexionInternet;
            _ventana.MostrarError(mensaje);
        }

        private void CargarDatosUsuario()
        {
            CodigoSala = string.Empty;
            NombreUsuario = _nombreUsuarioSesion;
        }

        private void InicializarOpcionesSeleccionadas()
        {
            NumeroRondasSeleccionada = _opcionesPartida.NumeroRondasPredeterminado;
            TiempoRondaSeleccionada = _opcionesPartida.TiempoRondaPredeterminado;
            IdiomaSeleccionado = _opcionesPartida.IdiomaPredeterminado;
            DificultadSeleccionada = _opcionesPartida.DificultadPredeterminada;
        }

        private async Task EjecutarEliminarAmigoAsync(DTOs.AmigoDTO amigo)
        {
            if (!ValidarAmigoParaEliminar(amigo))
            {
                return;
            }

            if (!SolicitarConfirmacionEliminacion(amigo.NombreUsuario))
            {
                return;
            }

            if (!ValidarSesionActivaParaEliminar())
            {
                ManejarErrorSesionInactiva();
                return;
            }

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                await _gestorAmigos.EliminarAmigoAsync(amigo.NombreUsuario)
                    .ConfigureAwait(true);
                MostrarExitoEliminacion();
            });
        }

        private static bool ValidarAmigoParaEliminar(DTOs.AmigoDTO amigo)
        {
            return amigo != null;
        }

        private bool SolicitarConfirmacionEliminacion(string nombreAmigo)
        {
            var eliminacionVistaModelo = new Amigos.EliminacionAmigoVistaModelo(
                _ventana,
                _localizador,
                _sonidoManejador,
                nombreAmigo);
            _ventana.MostrarVentanaDialogo(eliminacionVistaModelo);
            return eliminacionVistaModelo.DialogResult == true;
        }

        private bool ValidarSesionActivaParaEliminar()
        {
            return !string.IsNullOrWhiteSpace(_nombreUsuarioSesion);
        }

        private void ManejarErrorSesionInactiva()
        {
            _logger.Warn("Intento de eliminar amigo sin sesion activa.");
            _sonidoManejador.ReproducirError();
            App.AvisoServicio.Mostrar(Lang.errorTextoErrorProcesarSolicitud);
        }

        private void MostrarExitoEliminacion()
        {
            _sonidoManejador.ReproducirNotificacion();
            App.AvisoServicio.Mostrar(Lang.amigosTextoAmigoEliminado);
        }

        private void EjecutarAbrirPerfil()
        {
            var dependenciasBase = new VistaModeloBaseDependencias(
                _ventana,
                _localizador,
                _sonidoManejador,
                App.AvisoServicio);

            var dependenciasPerfil = new PerfilDependencias(
                App.PerfilServicio,
                new ClienteServicios.Dialogos.SeleccionAvatarDialogoServicio(
                    App.AvisoServicio, App.CatalogoAvatares, _sonidoManejador),
                App.CambioContrasenaServicio,
                App.RecuperacionCuentaServicio,
                _usuarioSesion,
                App.CatalogoAvatares,
                App.CatalogoImagenes);

            var perfilVistaModelo = new Perfil.PerfilVistaModelo(
                dependenciasBase,
                dependenciasPerfil);

            perfilVistaModelo.SolicitarReinicioSesion = EjecutarReinicioAplicacion;
            _ventana.MostrarVentanaDialogo(perfilVistaModelo);
        }

        private void EjecutarReinicioAplicacion(bool esVoluntario)
        {
            if (_gestorAmigos.DesconexionProcesada)
            {
                return;
            }

            _gestorAmigos.MarcarDesconexionProcesada();
            ReiniciarAplicacion();

            if (!esVoluntario)
            {
                _sonidoManejador.ReproducirError();
                _ventana.MostrarError(Lang.errorTextoDesconexionServidor);
            }
        }

        private void ReiniciarAplicacion()
        {
            _logger.Info(
                "Modulo: VentanaPrincipalVistaModelo - Reiniciando aplicacion tras " +
                "desconexion del servidor WCF. Causas probables: servidor cerrado, " +
                "timeout por inactividad del cliente, o fallo de red. " +
                "Reinicializando servicios de conexion.");
            
            DesuscribirEventos();
            _gestorAmigos.AbortarConexiones();
            IntentarCerrarSesionEnServidor();
            _usuarioSesion.Limpiar();
            App.ReinicializarServiciosConexion();

            var dependenciasBase = new VistaModeloBaseDependencias(
                _ventana,
                _localizador,
                _sonidoManejador,
                App.AvisoServicio);

            var dependenciasInicioSesion = new InicioSesionDependencias(
                App.InicioSesionServicio,
                App.CambioContrasenaServicio,
                App.RecuperacionCuentaServicio,
                _localizacion,
                App.GeneradorNombres,
                _usuarioSesion,
                App.FabricaSalas);

            var inicioVistaModelo = new InicioSesion.InicioSesionVistaModelo(
                dependenciasBase,
                dependenciasInicioSesion);
            _ventana.MostrarVentana(inicioVistaModelo);
            _ventana.CerrarTodasLasVentanas();
            _ventana.CerrarVentana(this);
        }

        private void IntentarCerrarSesionEnServidor()
        {
            string nombreUsuario = _usuarioSesion.NombreUsuario;

            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await App.InicioSesionServicio.CerrarSesionAsync(nombreUsuario)
                        .ConfigureAwait(false);
                    _logger.Info(
                        "Sesion cerrada en servidor durante reinicio por desconexion.");
                }
                catch (Exception excepcion)
                {
                    _logger.Warn(
                        "No se pudo cerrar la sesion en el servidor durante " +
                        "reinicio por desconexion. La sesion expirara por timeout.",
                        excepcion);
                }
            });
        }

        private void EjecutarAbrirAjustes()
        {
            var ajustesVistaModelo = new Ajustes.AjustesVistaModelo(
                _ventana,
                _localizador);
            _ventana.MostrarVentanaDialogo(ajustesVistaModelo);
        }

        private static void EjecutarAbrirComoJugar()
        {
            var comoJugar = new Vista.ComoJugar();
            comoJugar.ShowDialog();
        }

        private void EjecutarAbrirClasificacion()
        {
            var clasificacionVistaModelo = new ClasificacionVistaModelo(
                _ventana,
                _localizador,
                App.ClasificacionServicio,
                _sonidoManejador);
            clasificacionVistaModelo.SolicitarReinicioSesion = EjecutarReinicioAplicacion;
            _ventana.MostrarVentanaDialogo(clasificacionVistaModelo);
        }

        private void EjecutarAbrirBuscarAmigo()
        {
            var busquedaAmigoVistaModelo = new Amigos.BusquedaAmigoVistaModelo(
                _ventana,
                _localizador,
                _amigosServicio,
                _sonidoManejador,
                App.AvisoServicio,
                _usuarioSesion);
            busquedaAmigoVistaModelo.SolicitarReinicioSesion = EjecutarReinicioAplicacion;
            _ventana.MostrarVentanaDialogo(busquedaAmigoVistaModelo);
        }

        private void EjecutarAbrirSolicitudes()
        {
            if (!_gestorAmigos.CanalDisponible || _gestorAmigos.HuboErrorCargaSolicitudes)
            {
                App.AvisoServicio.Mostrar(Lang.amigosErrorObtenerSolicitudes);
                return;
            }

            var solicitudesPendientes = _gestorAmigos.SolicitudesPendientes;

            if (solicitudesPendientes == null || solicitudesPendientes.Count == 0)
            {
                App.AvisoServicio.Mostrar(Lang.amigosAvisoSinSolicitudesPendientes);
                return;
            }

            var solicitudesVistaModelo = new Amigos.SolicitudesVistaModelo(
                _ventana,
                _localizador,
                _amigosServicio,
                _sonidoManejador,
                App.AvisoServicio,
                _usuarioSesion);
            solicitudesVistaModelo.SolicitarReinicioSesion = EjecutarReinicioAplicacion;
            _ventana.MostrarVentanaDialogo(solicitudesVistaModelo);
        }

        private async Task UnirseSalaInternoAsync()
        {
            var resultadoValidacion = ValidadorEntrada.ValidarCodigoSala(CodigoSala);
            if (!resultadoValidacion.OperacionExitosa)
            {
                _sonidoManejador.ReproducirError();
                App.AvisoServicio.Mostrar(resultadoValidacion.Mensaje);
                return;
            }

            if (!ValidarSesionActivaParaUnirse())
            {
                ManejarError();
                return;
            }

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                var sala = await UnirseSalaEnServidorAsync(CodigoSala.Trim());
                NavegarASala(sala);
            });
        }

        private bool ValidarSesionActivaParaUnirse()
        {
            return !string.IsNullOrWhiteSpace(_nombreUsuarioSesion);
        }

        private async Task<DTOs.SalaDTO> UnirseSalaEnServidorAsync(string codigo)
        {
            _logger.InfoFormat("Intentando unirse a sala: {0}", codigo);
            return await _salasServicio.UnirseSalaAsync(
                codigo,
                _nombreUsuarioSesion).ConfigureAwait(true);
        }



        private async Task IniciarJuegoInternoAsync()
        {
            if (!ValidarConfiguracionJuego())
            {
                ManejarError();
                return;
            }

            if (!ValidarSesionActivaParaIniciar())
            {
                ManejarError();
                return;
            }

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                var configuracion = CrearConfiguracionPartida();
                var sala = await CrearSalaEnServidorAsync(configuracion);
                NavegarASala(sala);
            });
        }

        private bool ValidarConfiguracionJuego()
        {
            return PuedeIniciarJuego();
        }

        private void ManejarError()
        {
            _sonidoManejador.ReproducirError();
            App.AvisoServicio.Mostrar(Lang.errorTextoErrorProcesarSolicitud);
        }

        private bool ValidarSesionActivaParaIniciar()
        {
            return !string.IsNullOrWhiteSpace(_nombreUsuarioSesion);
        }

        private DTOs.ConfiguracionPartidaDTO CrearConfiguracionPartida()
        {
            return new DTOs.ConfiguracionPartidaDTO
            {
                NumeroRondas = NumeroRondasSeleccionada?.Valor ?? 0,
                TiempoPorRondaSegundos = TiempoRondaSeleccionada?.Valor ?? 0,
                IdiomaCanciones = IdiomaSeleccionado?.Codigo,
                Dificultad = DificultadSeleccionada?.Clave
            };
        }

        private async Task<DTOs.SalaDTO> CrearSalaEnServidorAsync(
            DTOs.ConfiguracionPartidaDTO configuracion)
        {
            _logger.Info("Creando nueva sala de juego.");
            return await _salasServicio.CrearSalaAsync(
                _nombreUsuarioSesion,
                configuracion).ConfigureAwait(true);
        }

        private void NavegarASala(DTOs.SalaDTO sala)
        {
            _sonidoManejador.ReproducirNotificacion();
            NavegarASala(sala, esInvitado: false);
        }

        private void NavegarASala(DTOs.SalaDTO sala, bool esInvitado)
        {
            App.MusicaManejador.Detener();

            var invitacionSalaServicio = new InvitacionSalaServicio(
                App.InvitacionesServicio,
                _gestorAmigos.ListaAmigosServicio,
                App.PerfilServicio,
                _sonidoManejador,
                App.AvisoServicio,
                App.Localizador);

            var comunicacion = new ComunicacionSalaDependencias(
                _salasServicio,
                App.InvitacionesServicio,
                invitacionSalaServicio,
                App.WcfFabrica);

            var perfiles = new PerfilesSalaDependencias(
                _gestorAmigos.ListaAmigosServicio,
                App.PerfilServicio,
                App.ReportesServicio,
                _usuarioSesion);

            var audio = new AudioSalaDependencias(
                _sonidoManejador,
                new CancionManejador(),
                App.CatalogoCanciones);

            var dependenciasSala = new SalaVistaModeloDependencias(
                comunicacion,
                perfiles,
                audio,
                App.AvisoServicio);

            var salaVistaModelo = new Salas.SalaVistaModelo(
                _ventana,
                _localizador,
                sala,
                dependenciasSala,
                _usuarioSesion.NombreUsuario,
                esInvitado);

            _ventana.MostrarVentana(salaVistaModelo);
            _ventana.CerrarVentana(this);
        }

        private bool PuedeIniciarJuego()
        {
            return NumeroRondasSeleccionada != null
                && TiempoRondaSeleccionada != null
                && IdiomaSeleccionado != null
                && DificultadSeleccionada != null;
        }

        private void ActualizarEstadoIniciarJuego()
        {
            IniciarJuegoComando?.NotificarPuedeEjecutar();
        }

        private void ManejarDesconexion(string mensaje)
        {
            if (_gestorAmigos.DesconexionProcesada || _desconexionInternetProcesada)
            {
                return;
            }

            _gestorAmigos.MarcarDesconexionProcesada();
            _logger.WarnFormat("Desconexion detectada: {0}", mensaje);
            _sonidoManejador.ReproducirError();
            ReiniciarAplicacion();
            _ventana.MostrarError(mensaje);
        }

        private void EnConexionInternetPerdida(object remitente, EventArgs argumentos)
        {
            if (_desconexionInternetProcesada || _gestorAmigos.DesconexionProcesada)
            {
                return;
            }

            _desconexionInternetProcesada = true;
            _logger.Warn("Se detectó pérdida de conexión a internet en ventana principal.");

            EjecutarEnDispatcher(() =>
            {
                _sonidoManejador.ReproducirError();
                ReiniciarAplicacion();
                _ventana.MostrarError(Lang.errorTextoPerdidaConexionInternet);
            });
        }
    }
}