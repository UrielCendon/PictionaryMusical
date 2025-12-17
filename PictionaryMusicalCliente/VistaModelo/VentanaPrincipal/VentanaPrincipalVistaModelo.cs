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
using System.Windows;
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
            DesconexionDetectada += ManejarDesconexion;

            _nombreUsuarioSesion = _usuarioSesion.NombreUsuario ?? string.Empty;
            _opcionesPartida = new OpcionesPartidaManejador();

            CargarDatosUsuario();
            InicializarOpcionesSeleccionadas();

            AbrirPerfilComando = new ComandoDelegado(_ =>
            {
                _sonidoManejador.ReproducirClick();
                EjecutarAbrirPerfil();
            });
            AbrirAjustesComando = new ComandoDelegado(_ =>
            {
                _sonidoManejador.ReproducirClick();
                EjecutarAbrirAjustes();
            });
            AbrirComoJugarComando = new ComandoDelegado(_ =>
            {
                _sonidoManejador.ReproducirClick();
                EjecutarAbrirComoJugar();
            });
            AbrirClasificacionComando = new ComandoDelegado(_ =>
            {
                _sonidoManejador.ReproducirClick();
                EjecutarAbrirClasificacion();
            });
            AbrirBuscarAmigoComando = new ComandoDelegado(_ =>
            {
                _sonidoManejador.ReproducirClick();
                EjecutarAbrirBuscarAmigo();
            });
            AbrirSolicitudesComando = new ComandoDelegado(_ =>
            {
                _sonidoManejador.ReproducirClick();
                EjecutarAbrirSolicitudes();
            });

            EliminarAmigoComando = new ComandoAsincrono(async param =>
            {
                _sonidoManejador.ReproducirClick();
                await EjecutarEliminarAmigoAsync(param as DTOs.AmigoDTO);
            }, param => param is DTOs.AmigoDTO);

            UnirseSalaComando = new ComandoAsincrono(async _ =>
            {
                _sonidoManejador.ReproducirClick();
                await UnirseSalaInternoAsync();
            });

            IniciarJuegoComando = new ComandoAsincrono(async _ =>
            {
                _sonidoManejador.ReproducirClick();
                await IniciarJuegoInternoAsync();
            }, _ => PuedeIniciarJuego());
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
        public ICommand AbrirPerfilComando { get; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de ajustes.
        /// </summary>
        public ICommand AbrirAjustesComando { get; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de instrucciones del juego.
        /// </summary>
        public ICommand AbrirComoJugarComando { get; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de clasificacion.
        /// </summary>
        public ICommand AbrirClasificacionComando { get; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de busqueda de amigos.
        /// </summary>
        public ICommand AbrirBuscarAmigoComando { get; }

        /// <summary>
        /// Obtiene el comando para abrir la ventana de solicitudes de amistad.
        /// </summary>
        public ICommand AbrirSolicitudesComando { get; }

        /// <summary>
        /// Obtiene el comando asincrono para eliminar un amigo de la lista.
        /// </summary>
        public IComandoAsincrono EliminarAmigoComando { get; }

        /// <summary>
        /// Obtiene el comando asincrono para unirse a una sala existente.
        /// </summary>
        public IComandoAsincrono UnirseSalaComando { get; }

        /// <summary>
        /// Obtiene el comando asincrono para iniciar una nueva partida.
        /// </summary>
        public IComandoAsincrono IniciarJuegoComando { get; }

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
            await _listaAmigosServicio.SuscribirAsync(_nombreUsuarioSesion).
                ConfigureAwait(false);
            await _amigosServicio.SuscribirAsync(_nombreUsuarioSesion).ConfigureAwait(false);
        }

        private void MarcarSuscripcionActiva()
        {
            _suscripcionActiva = true;
        }

        private async Task CargarListaAmigosInicialAsync()
        {
            IReadOnlyList<DTOs.AmigoDTO> listaActual = _listaAmigosServicio.ListaActual;
            EjecutarEnDispatcher(() => ActualizarAmigos(listaActual));
            await Task.CompletedTask;
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
        }

        private void CanalAmigos_Desconectado(object sender, EventArgs e)
        {
            _logger.Error("Se detecto desconexion del canal de amigos.");
            EjecutarEnDispatcher(() =>
            {
                ManejarDesconexionCritica(Lang.errorTextoConexionInterrumpida);
            });
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
            if (!ValidarSesionParaActualizarAmigos())
            {
                return;
            }

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                var amigos = await ObtenerAmigosDelServidorAsync();
                ActualizarListaEnDispatcher(amigos);
            });
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

            perfilVistaModelo.SolicitarReinicioSesion = () => ReiniciarAplicacion();
            _ventana.MostrarVentanaDialogo(perfilVistaModelo);
        }

        private void ReiniciarAplicacion()
        {
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
            _ventana.MostrarVentanaDialogo(busquedaAmigoVistaModelo);
        }

        private void EjecutarAbrirSolicitudes()
        {
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
            _ventana.MostrarVentanaDialogo(solicitudesVistaModelo);
        }

        private async Task UnirseSalaInternoAsync()
        {
            string codigo = ValidarCodigoSala();
            if (codigo == null)
            {
                ManejarErrorCodigoInvalido();
                return;
            }

            if (!ValidarSesionActivaParaUnirse())
            {
                ManejarError();
                return;
            }

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                var sala = await UnirseSalaEnServidorAsync(codigo);
                NavegarASala(sala);
            });
        }

        private string ValidarCodigoSala()
        {
            string codigo = CodigoSala?.Trim();
            return string.IsNullOrWhiteSpace(codigo) ? null : codigo;
        }

        private void ManejarErrorCodigoInvalido()
        {
            _sonidoManejador.ReproducirError();
            App.AvisoServicio.Mostrar(Lang.unirseSalaTextoVacio);
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
                App.AvisoServicio);

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
            _logger.WarnFormat("Desconexion detectada: {0}", mensaje);
            _sonidoManejador.ReproducirError();
            ReiniciarAplicacion();
        }
    }
}