using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using PictionaryMusicalCliente.VistaModelo.VentanaPrincipal.Auxiliares;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private ObservableCollection<DTOs.AmigoDTO> _amigos;
        private DTOs.AmigoDTO _amigoSeleccionado;

        private readonly string _nombreUsuarioSesion;
        private readonly ILocalizacionServicio _localizacion;
        private readonly IListaAmigosServicio _listaAmigosServicio;
        private readonly IAmigosServicio _amigosServicio;
        private readonly ISalasServicio _salasServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IUsuarioAutenticado _usuarioSesion;
        private readonly OpcionesPartidaManejador _opcionesPartida;

        private bool _suscripcionActiva;
        private bool _canalAmigosDisponible;
        private bool _desconexionProcesada;
        private bool _desconexionInternetProcesada;
        private bool _actualizacionAmigosEnProgreso;

        public VentanaPrincipalVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            ILocalizacionServicio localizacionServicio,
            IListaAmigosServicio listaAmigosServicio,
            IAmigosServicio amigosServicio,
            ISalasServicio salasServicio,
            SonidoManejador sonidoManejador,
            IUsuarioAutenticado usuarioSesion)
            : base(ventana, localizador)
        {

            _localizacion = localizacionServicio ??
                throw new ArgumentNullException(nameof(localizacionServicio));
            _listaAmigosServicio = listaAmigosServicio ??
                throw new ArgumentNullException(nameof(listaAmigosServicio));
            _amigosServicio = amigosServicio ??
                throw new ArgumentNullException(nameof(amigosServicio));
            _salasServicio = salasServicio ??
                throw new ArgumentNullException(nameof(salasServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            _usuarioSesion = usuarioSesion ??
                throw new ArgumentNullException(nameof(usuarioSesion));
            _listaAmigosServicio.ListaActualizada += ListaActualizada;
            _amigosServicio.SolicitudesActualizadas += SolicitudesAmistadActualizadas;
            _listaAmigosServicio.CanalDesconectado += CanalAmigos_Desconectado;
            _amigosServicio.CanalDesconectado += CanalAmigos_Desconectado;
            ConectividadRedMonitor.Instancia.ConexionPerdida += OnConexionInternetPerdida;
            DesconexionDetectada += ManejarDesconexion;

            _nombreUsuarioSesion = _usuarioSesion.NombreUsuario ?? string.Empty;
            _opcionesPartida = new OpcionesPartidaManejador();

            CargarDatosUsuario();
            InicializarOpcionesSeleccionadas();
            InicializarComandos();
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

        private bool ValidarParametroAmigoDTO(object parametro)
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
        public ObservableCollection<DTOs.AmigoDTO> Amigos
        {
            get => _amigos;
            private set => EstablecerPropiedad(ref _amigos, value);
        }

        /// <summary>
        /// Obtiene o establece el amigo seleccionado en la lista.
        /// </summary>
        public DTOs.AmigoDTO AmigoSeleccionado
        {
            get => _amigoSeleccionado;
            set
            {
                EstablecerPropiedad(ref _amigoSeleccionado, value);
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
            if (!ValidarCondicionesInicializacion())
            {
                return;
            }

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                await SuscribirAServiciosAsync();
                MarcarSuscripcionActiva();
                await CargarListaAmigosInicialAsync();
            });
        }

        private bool ValidarCondicionesInicializacion()
        {
            return !_suscripcionActiva && !string.IsNullOrWhiteSpace(_nombreUsuarioSesion);
        }

        private async Task SuscribirAServiciosAsync()
        {
            _desconexionProcesada = false;
            _actualizacionAmigosEnProgreso = false;
            await _listaAmigosServicio.SuscribirAsync(_nombreUsuarioSesion).
                ConfigureAwait(false);
            await _amigosServicio.SuscribirAsync(_nombreUsuarioSesion).ConfigureAwait(false);
            _canalAmigosDisponible = true;
        }

        private void MarcarSuscripcionActiva()
        {
            _suscripcionActiva = true;
        }

        private async Task CargarListaAmigosInicialAsync()
        {
            IReadOnlyList<DTOs.AmigoDTO> listaActual = _listaAmigosServicio.ListaActual;
            EjecutarEnDispatcher(ActualizarAmigosConListaActual);
            await Task.CompletedTask;
        }

        private void ActualizarAmigosConListaActual()
        {
            ActualizarAmigos(_listaAmigosServicio.ListaActual);
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
                await CancelarSuscripcionesAsync();
                MarcarSuscripcionInactiva();
            });
        }

        private void DesuscribirEventos()
        {
            _listaAmigosServicio.ListaActualizada -= ListaActualizada;
            _amigosServicio.SolicitudesActualizadas -= SolicitudesAmistadActualizadas;
            _listaAmigosServicio.CanalDesconectado -= CanalAmigos_Desconectado;
            _amigosServicio.CanalDesconectado -= CanalAmigos_Desconectado;
            ConectividadRedMonitor.Instancia.ConexionPerdida -= OnConexionInternetPerdida;
            DesconexionDetectada -= ManejarDesconexion;
        }

        private void CanalAmigos_Desconectado(object remitente, EventArgs argumentosEvento)
        {
            if (_desconexionProcesada)
            {
                return;
            }

            _desconexionProcesada = true;
            _logger.Error("Se detecto desconexion del canal de amigos.");
            _canalAmigosDisponible = false;
            
            DesuscribirEventosCanalesAmigos();
            EjecutarEnDispatcher(ManejarDesconexionCanalAmigos);
        }

        private void DesuscribirEventosCanalesAmigos()
        {
            _listaAmigosServicio.CanalDesconectado -= CanalAmigos_Desconectado;
            _amigosServicio.CanalDesconectado -= CanalAmigos_Desconectado;
        }

        private void ManejarDesconexionCanalAmigos()
        {
            _sonidoManejador.ReproducirError();
            ReiniciarAplicacion();
            _ventana.MostrarError(Lang.errorTextoServidorCerrado);
        }

        private async Task CancelarSuscripcionesAsync()
        {
            await _listaAmigosServicio.CancelarSuscripcionAsync(
                _nombreUsuarioSesion).ConfigureAwait(false);
            await _amigosServicio.CancelarSuscripcionAsync(
                _nombreUsuarioSesion).ConfigureAwait(false);
        }

        private void MarcarSuscripcionInactiva()
        {
            _suscripcionActiva = false;
        }

        private void CargarDatosUsuario()
        {
            CodigoSala = string.Empty;
            Amigos = new ObservableCollection<DTOs.AmigoDTO>();
            NombreUsuario = _nombreUsuarioSesion;
        }

        private void InicializarOpcionesSeleccionadas()
        {
            NumeroRondasSeleccionada = _opcionesPartida.NumeroRondasPredeterminado;
            TiempoRondaSeleccionada = _opcionesPartida.TiempoRondaPredeterminado;
            IdiomaSeleccionado = _opcionesPartida.IdiomaPredeterminado;
            DificultadSeleccionada = _opcionesPartida.DificultadPredeterminada;
        }

        private void ListaActualizada(object remitente, IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            EjecutarEnDispatcher(() => ActualizarAmigos(amigos));
        }

        private void SolicitudesAmistadActualizadas(
            object remitente,
            IReadOnlyCollection<DTOs.SolicitudAmistadDTO> solicitudes)
        {
            if (!_canalAmigosDisponible || _desconexionProcesada || _actualizacionAmigosEnProgreso)
            {
                return;
            }

            _ = ActualizarListaAmigosDesdeServidorAsync();
        }

        private void ActualizarAmigos(IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            InicializarListaAmigos();
            LimpiarListaAmigos();
            AgregarAmigosValidos(amigos);
            ValidarAmigoSeleccionado(amigos);
        }

        private void InicializarListaAmigos()
        {
            if (Amigos == null)
            {
                Amigos = new ObservableCollection<DTOs.AmigoDTO>();
            }
        }

        private void LimpiarListaAmigos()
        {
            Amigos.Clear();
        }

        private void AgregarAmigosValidos(IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            if (amigos != null)
            {
                foreach (var amigo in amigos.Where(a =>
                    !string.IsNullOrWhiteSpace(a?.NombreUsuario)))
                {
                    Amigos.Add(amigo);
                }
            }
        }

        private void ValidarAmigoSeleccionado(IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            if (AmigoSeleccionado != null
                && (amigos == null || !amigos.Any(a => string.Equals(
                    a.NombreUsuario,
                    AmigoSeleccionado.NombreUsuario,
                    StringComparison.OrdinalIgnoreCase))))
            {
                AmigoSeleccionado = null;
            }
        }

        private async Task ActualizarListaAmigosDesdeServidorAsync()
        {
            if (!ValidarSesionParaActualizarAmigos() || _actualizacionAmigosEnProgreso)
            {
                return;
            }

            _actualizacionAmigosEnProgreso = true;
            try
            {
                await EjecutarOperacionConDesconexionAsync(async () =>
                {
                    var amigos = await ObtenerAmigosDelServidorAsync();
                    ActualizarListaEnDispatcher(amigos);
                });
            }
            finally
            {
                _actualizacionAmigosEnProgreso = false;
            }
        }

        private bool ValidarSesionParaActualizarAmigos()
        {
            return !string.IsNullOrWhiteSpace(_nombreUsuarioSesion);
        }

        private async Task<IReadOnlyList<DTOs.AmigoDTO>> ObtenerAmigosDelServidorAsync()
        {
            return await _listaAmigosServicio.ObtenerAmigosAsync(
                _nombreUsuarioSesion).ConfigureAwait(false);
        }

        private void ActualizarListaEnDispatcher(IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            EjecutarEnDispatcher(() => ActualizarAmigos(amigos));
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
                await EliminarAmigoEnServidorAsync(amigo.NombreUsuario);
                await ActualizarListaTrasEliminacion();
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

        private async Task EliminarAmigoEnServidorAsync(string nombreAmigo)
        {
            await _amigosServicio.EliminarAmigoAsync(
                _nombreUsuarioSesion,
                nombreAmigo).ConfigureAwait(true);
        }

        private async Task ActualizarListaTrasEliminacion()
        {
            await ActualizarListaAmigosDesdeServidorAsync().ConfigureAwait(true);
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
            if (_desconexionProcesada)
            {
                return;
            }

            _desconexionProcesada = true;
            ReiniciarAplicacion();

            if (!esVoluntario)
            {
                _sonidoManejador.ReproducirError();
                _ventana.MostrarError(Lang.errorTextoDesconexionServidor);
            }
        }

        private void ReiniciarAplicacion()
        {
            DesuscribirEventos();
            AbortarCanalesAmigos();
            _usuarioSesion.Limpiar();

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

        private void AbortarCanalesAmigos()
        {
            try
            {
                _listaAmigosServicio.AbortarConexion();
            }
            catch (Exception excepcion)
            {
                _logger.Warn("Error al abortar canal de lista de amigos.", excepcion);
            }

            try
            {
                _amigosServicio.AbortarConexion();
            }
            catch (Exception excepcion)
            {
                _logger.Warn("Error al abortar canal de amigos.", excepcion);
            }
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
                App.AvisoServicio,
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
            if (!_canalAmigosDisponible || _amigosServicio.HuboErrorCargaSolicitudes)
            {
                App.AvisoServicio.Mostrar(Lang.amigosErrorObtenerSolicitudes);
                return;
            }

            var solicitudesPendientes = _amigosServicio?.SolicitudesPendientes;

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
                _listaAmigosServicio,
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
                _listaAmigosServicio,
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
            if (_desconexionProcesada || _desconexionInternetProcesada)
            {
                return;
            }

            _desconexionProcesada = true;
            _logger.WarnFormat("Desconexion detectada: {0}", mensaje);
            _sonidoManejador.ReproducirError();
            ReiniciarAplicacion();
            _ventana.MostrarError(mensaje);
        }

        private void OnConexionInternetPerdida(object remitente, EventArgs argumentos)
        {
            if (_desconexionInternetProcesada || _desconexionProcesada)
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