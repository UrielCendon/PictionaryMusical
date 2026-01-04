using log4net;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.Utilidades;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Implementa una fachada para administrar el curso de las partidas activas.
    /// Gestiona los callbacks y delega la logica al ControladorPartida por sala.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CursoPartidaManejador : ICursoPartidaManejador
    {
        /// <summary>
        /// Maneja la desconexión de un jugador notificando a SalasManejador.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreUsuario">Nombre del usuario desconectado.</param>
        private void ManejarJugadorDesconectado(string idSala, string nombreUsuario)
        {
            try
            {
                _salasManejador.NotificarDesconexionJugador(idSala, nombreUsuario);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    string.Format(
                        "Error al notificar desconexion de jugador en sala '{0}'.",
                        idSala),
                    excepcion);
            }
        }
        private const int TiempoRondaPorDefectoSegundos = 90;
        private const int TiempoTransicionPorDefectoSegundos = 5;
        private const int NumeroRondasPorDefecto = 3;
        private const string DificultadPorDefecto = "Media";

        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(CursoPartidaManejador));

        private static readonly Dictionary<string, ControladorPartida> _partidasActivas =
            new Dictionary<string, ControladorPartida>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string,
            Dictionary<string, ICursoPartidaManejadorCallback>> _callbacksPorSala =
            new Dictionary<string, Dictionary<string, ICursoPartidaManejadorCallback>>(
                StringComparer.OrdinalIgnoreCase);

        private static readonly object _sincronizacion = new object();

        private readonly ISalasManejador _salasManejador;
        private readonly ICatalogoCanciones _catalogoCanciones;
        private readonly INotificadorPartida _notificadorPartida;
        private readonly IActualizadorClasificacionPartida _actualizadorClasificacion;

        /// <summary>
        /// Constructor por defecto para uso en WCF.
        /// </summary>
        public CursoPartidaManejador() : this(
            new SalasManejador(),
            new CatalogoCanciones(new GeneradorAleatorioDatos()),
            new NotificadorPartida(),
            new ActualizadorClasificacionPartida())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="salasManejador">Manejador de salas.</param>
        /// <param name="catalogoCanciones">Catalogo de canciones.</param>
        /// <param name="notificadorPartida">Servicio de notificaciones de partida.</param>
        /// <param name="actualizadorClasificacion">Servicio de actualizacion de clasificaciones.
        /// </param>
        public CursoPartidaManejador(
            ISalasManejador salasManejador,
            ICatalogoCanciones catalogoCanciones,
            INotificadorPartida notificadorPartida,
            IActualizadorClasificacionPartida actualizadorClasificacion)
        {
            _salasManejador = salasManejador
                ?? throw new ArgumentNullException(nameof(salasManejador));

            _catalogoCanciones = catalogoCanciones
                ?? throw new ArgumentNullException(nameof(catalogoCanciones));

            _notificadorPartida = notificadorPartida
                ?? throw new ArgumentNullException(nameof(notificadorPartida));

            _actualizadorClasificacion = actualizadorClasificacion
                ?? throw new ArgumentNullException(nameof(actualizadorClasificacion));

            _notificadorPartida.CallbackInvalido += ManejarCallbackInvalido;
        }

        /// <summary>
        /// Registra a un jugador para recibir notificaciones de la partida.
        /// </summary>
        /// <param name="suscripcion">Datos de suscripcion del jugador.</param>
        public void SuscribirJugador(SuscripcionJugadorDTO suscripcion)
        {
            EntradaComunValidador.ValidarSuscripcionJugador(suscripcion);

            var callback = ObtenerCallbackActual();
            var controlador = ObtenerOCrearControlador(suscripcion.IdSala.Trim());

            controlador.AgregarJugador(
                suscripcion.IdJugador.Trim(),
                suscripcion.NombreUsuario?.Trim() ?? string.Empty,
                suscripcion.EsHost);

            RegistrarCallback(
                suscripcion.IdSala.Trim(), 
                suscripcion.IdJugador.Trim(), 
                callback);

            _logger.InfoFormat(
                MensajesError.Bitacora.JugadorSuscritoPartida,
                suscripcion.IdJugador.Trim(),
                suscripcion.IdSala.Trim());
        }

        /// <summary>
        /// Inicia la partida de la sala indicada.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idJugadorSolicitante">Identificador del jugador que solicita el inicio.
        /// </param>
        public void IniciarPartida(string idSala, string idJugadorSolicitante)
        {
            EntradaComunValidador.ValidarIdSala(idSala);

            _logger.InfoFormat(
                MensajesError.Bitacora.InicioPartidaSolicitado,
                idSala.Trim(),
                idJugadorSolicitante?.Trim());

            _salasManejador.MarcarPartidaComoIniciada(idSala.Trim());
            var controlador = ObtenerOCrearControlador(idSala.Trim());
            controlador.IniciarPartida(idJugadorSolicitante?.Trim());
        }

        /// <summary>
        /// Procesa un mensaje de chat enviado por un jugador durante la partida.
        /// </summary>
        /// <param name="mensaje">Mensaje a enviar.</param>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idJugador">Identificador del jugador.</param>
        public void EnviarMensajeJuego(string mensaje, string idSala, string idJugador)
        {
            EntradaComunValidador.ValidarMensajeJuego(mensaje, idSala);

            var controlador = ObtenerOCrearControlador(idSala.Trim());
            controlador.ProcesarMensaje(idJugador?.Trim(), mensaje);
        }

        /// <summary>
        /// Procesa un trazo dibujado por el jugador actual de la sala.
        /// </summary>
        public void EnviarTrazo(TrazoDTO trazo, string idSala, string idJugador)
        {
            EntradaComunValidador.ValidarIdSala(idSala);

            var controlador = ObtenerOCrearControlador(idSala.Trim());
            controlador.ProcesarTrazo(idJugador?.Trim(), trazo);
        }

        private ControladorPartida ObtenerOCrearControlador(string idSala)
        {
            lock (_sincronizacion)
            {
                ControladorPartida existente;
                if (_partidasActivas.TryGetValue(idSala, out existente))
                {
                    return existente;
                }

                var configuracion = ObtenerConfiguracionSala(idSala);
                var generadorAleatorio = new GeneradorAleatorioDatos();
                var gestorJugadores = new GestorJugadoresPartida(generadorAleatorio);
                var proveedorFecha = new ProveedorFecha();

                int tiempoRonda = configuracion?.TiempoPorRondaSegundos ?? TiempoRondaPorDefectoSegundos;
                var gestorTiempos = new GestorTiemposPartida(
                    tiempoRonda,
                    TiempoTransicionPorDefectoSegundos,
                    proveedorFecha);

                var validadorAdivinanza = new ValidadorAdivinanza(_catalogoCanciones, gestorTiempos);
                var proveedorTiempo = new ProveedorTiempo();

                var controlador = new ControladorPartida(
                    tiempoRonda,
                    configuracion?.Dificultad ?? DificultadPorDefecto,
                    configuracion?.NumeroRondas ?? NumeroRondasPorDefecto,
                    _catalogoCanciones,
                    gestorJugadores,
                    gestorTiempos,
                    validadorAdivinanza,
                    proveedorTiempo);

                if (!string.IsNullOrWhiteSpace(configuracion?.IdiomaCanciones))
                {
                    controlador.ConfigurarIdiomaCanciones(configuracion.IdiomaCanciones);
                }

                SuscribirEventos(controlador, idSala);
                _partidasActivas[idSala] = controlador;
                _callbacksPorSala[idSala] =
                    new Dictionary<string, ICursoPartidaManejadorCallback>(
                        StringComparer.OrdinalIgnoreCase);

                return controlador;
            }
        }

        private void SuscribirEventos(ControladorPartida controlador, string idSala)
        {
            controlador.PartidaIniciada += delegate()
            {
                ManejarPartidaIniciada(idSala);
            };

            controlador.InicioRonda += delegate(RondaDTO ronda)
            {
                Task.Run(delegate()
                {
                    ManejarInicioRonda(idSala, ronda, controlador);
                });
            };

            controlador.JugadorAdivino += delegate(string jugador, int puntos)
            {
                ManejarJugadorAdivino(idSala, jugador, puntos);
            };

            controlador.MensajeChatRecibido += delegate(string jugador, string mensaje)
            {
                ManejarMensajeChat(idSala, jugador, mensaje);
            };

            controlador.TrazoRecibido += delegate(TrazoDTO trazo)
            {
                ManejarTrazoRecibido(idSala, trazo);
            };

            controlador.FinRonda += delegate(bool tiempoAgotado)
            {
                ManejarFinRonda(idSala, tiempoAgotado);
            };

            controlador.FinPartida += delegate(ResultadoPartidaDTO resultado)
            {
                ManejarFinPartida(idSala, resultado, controlador);
            };

            controlador.LimpiarLienzo += delegate()
            {
                ManejarLimpiarLienzo(idSala);
            };

            controlador.JugadorDesconectado += delegate(string nombreUsuario)
            {
                ManejarJugadorDesconectado(idSala, nombreUsuario);
            };
        }

        private void ManejarPartidaIniciada(string idSala)
        {
            var callbacks = ObtenerCallbacksSala(idSala);
            if (callbacks != null)
            {
                _notificadorPartida.NotificarPartidaIniciada(idSala, callbacks);
            }
        }

        private void ManejarInicioRonda(
            string idSala,
            RondaDTO rondaBase,
            ControladorPartida controlador)
        {
            var jugadoresEstado = controlador.ObtenerJugadores();
            var dibujante = jugadoresEstado.FirstOrDefault(
                jugadorActual => jugadorActual.EsDibujante);
            string nombreDibujante = dibujante?.NombreUsuario ?? string.Empty;

            List<KeyValuePair<string, ICursoPartidaManejadorCallback>> callbacks;

            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacksSala;
                if (!_callbacksPorSala.TryGetValue(idSala, out callbacksSala))
                {
                    return;
                }
                callbacks = new List<KeyValuePair<string, ICursoPartidaManejadorCallback>>
                    (callbacksSala);
            }

            var cancionActual = _catalogoCanciones.ObtenerCancionPorId(rondaBase.IdCancion);

            foreach (var suscripcionJugador in callbacks)
            {
                NotificarInicioRondaIndividual(
                    idSala,
                    suscripcionJugador.Key,
                    suscripcionJugador.Value,
                    rondaBase,
                    jugadoresEstado,
                    nombreDibujante,
                    cancionActual);
            }
        }

        private static void NotificarInicioRondaIndividual(
            string idSala,
            string idJugador,
            ICursoPartidaManejadorCallback callback,
            RondaDTO rondaBase,
            IEnumerable<JugadorPartida> jugadoresEstado,
            string nombreDibujante,
            Datos.Entidades.Cancion cancionActual)
        {
            var datosJugador = jugadoresEstado.FirstOrDefault(jugadorActual =>
                string.Equals(
                    jugadorActual.IdConexion, 
                    idJugador, 
                    StringComparison.OrdinalIgnoreCase));

            bool esDibujante = datosJugador?.EsDibujante ?? false;

            var rondaPersonalizada = CrearRondaPersonalizada(
                rondaBase, 
                cancionActual, 
                esDibujante, 
                nombreDibujante);

            NotificarInicioRondaSeguro(callback, rondaPersonalizada, idSala, idJugador);
        }

        private static RondaDTO CrearRondaPersonalizada(
            RondaDTO rondaBase,
            Datos.Entidades.Cancion cancionActual,
            bool esDibujante,
            string nombreDibujante)
        {
            string pistaArtista = rondaBase.PistaArtista;
            string pistaGenero = rondaBase.PistaGenero;

            if (esDibujante && cancionActual != null)
            {
                pistaArtista = cancionActual.Artista;
                pistaGenero = cancionActual.Genero;
            }

            return new RondaDTO
            {
                IdCancion = rondaBase.IdCancion,
                PistaArtista = pistaArtista,
                PistaGenero = pistaGenero,
                TiempoSegundos = rondaBase.TiempoSegundos,
                Rol = esDibujante ? "Dibujante" : "Adivinador",
                NombreDibujante = nombreDibujante
            };
        }

        private static void NotificarInicioRondaSeguro(
            ICursoPartidaManejadorCallback callback,
            RondaDTO ronda,
            string idSala,
            string idJugador)
        {
            try
            {
                callback.NotificarInicioRonda(ronda);
            }
            catch (CommunicationException excepcion)
            {
                _logger.WarnFormat(MensajesError.Bitacora.ErrorNotificandoInicioRonda, idJugador, excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.WarnFormat(MensajesError.Bitacora.ErrorNotificandoInicioRonda, idJugador, excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.WarnFormat(MensajesError.Bitacora.ErrorNotificandoInicioRonda, idJugador, excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    string.Format(
                        MensajesError.Bitacora.ErrorNotificandoInicioRonda,
                        idJugador),
                    excepcion);
                RemoverCallback(idSala, idJugador);
            }
        }

        private void ManejarJugadorAdivino(string idSala, string jugador, int puntos)
        {
            var callbacks = ObtenerCallbacksSala(idSala);
            if (callbacks != null)
            {
                var parametrosNotificacion = new NotificacionJugadorAdivinoParametros
                {
                    IdSala = idSala,
                    Callbacks = callbacks,
                    NombreJugador = jugador,
                    Puntos = puntos
                };
                _notificadorPartida.NotificarJugadorAdivino(parametrosNotificacion);
            }
        }

        private void ManejarMensajeChat(string idSala, string jugador, string mensaje)
        {
            var callbacks = ObtenerCallbacksSala(idSala);
            if (callbacks != null)
            {
                var parametrosNotificacion = new NotificacionMensajeChatParametros
                {
                    IdSala = idSala,
                    Callbacks = callbacks,
                    NombreJugador = jugador,
                    Mensaje = mensaje
                };
                _notificadorPartida.NotificarMensajeChat(parametrosNotificacion);
            }
        }

        private void ManejarTrazoRecibido(string idSala, TrazoDTO trazo)
        {
            var callbacks = ObtenerCallbacksSala(idSala);
            if (callbacks != null)
            {
                _notificadorPartida.NotificarTrazoRecibido(idSala, callbacks, trazo);
            }
        }

        private void ManejarFinRonda(string idSala, bool tiempoAgotado)
        {
            var callbacks = ObtenerCallbacksSala(idSala);
            if (callbacks != null)
            {
                _notificadorPartida.NotificarFinRonda(idSala, callbacks, tiempoAgotado);
            }
        }

        private void ManejarLimpiarLienzo(string idSala)
        {
            var callbacks = ObtenerCallbacksSala(idSala);
            if (callbacks != null)
            {
                _notificadorPartida.NotificarLimpiarLienzo(idSala, callbacks);
            }
        }

        private void ManejarFinPartida(
            string idSala,
            ResultadoPartidaDTO resultado,
            ControladorPartida controlador)
        {
            _salasManejador.MarcarPartidaComoFinalizada(idSala);
            
            var jugadores = ObtenerJugadoresFinales(controlador);
            _actualizadorClasificacion.ActualizarClasificaciones(jugadores, resultado);
            
            var callbacks = ObtenerCallbacksSala(idSala);
            if (callbacks != null)
            {
                _notificadorPartida.NotificarFinPartida(idSala, callbacks, resultado);
            }
        }

        private static List<JugadorPartida> ObtenerJugadoresFinales(ControladorPartida controlador)
        {
            try
            {
                var jugadores = controlador?.ObtenerJugadores();
                if (jugadores == null)
                {
                    return new List<JugadorPartida>();
                }
                return new List<JugadorPartida>(jugadores);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorInesperadoObtenerJugadoresClasificacion,
                    excepcion);
                return new List<JugadorPartida>();
            }
        }

        private static Dictionary<string, ICursoPartidaManejadorCallback> ObtenerCallbacksSala(
            string idSala)
        {
            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacksSala;
                if (_callbacksPorSala.TryGetValue(idSala, out callbacksSala))
                {
                    return new Dictionary<string, ICursoPartidaManejadorCallback>(
                        callbacksSala,
                        StringComparer.OrdinalIgnoreCase);
                }
                return new Dictionary<string, ICursoPartidaManejadorCallback>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private void ManejarCallbackInvalido(string idSala, string idJugador)
        {
            string nombreUsuario = RemoverCallbackYObtenerNombre(idSala, idJugador);
            
            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                try
                {
                    _salasManejador.NotificarClienteInalcanzable(idSala, nombreUsuario);
                }
                catch (Exception excepcion)
                {
                    _logger.Error(
                        string.Format(
                            "Error al notificar cliente inalcanzable en sala '{0}'.",
                            idSala),
                        excepcion);
                }
            }
        }

        private static string RemoverCallbackYObtenerNombre(string idSala, string idJugador)
        {
            ControladorPartida controladorARemover = null;
            string nombreUsuario = null;
            bool callbackRemovido = false;
            bool partidaActiva = false;
            bool partidaFinalizada = false;

            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacks;
                if (_callbacksPorSala.TryGetValue(idSala, out callbacks))
                {
                    callbackRemovido = callbacks.Remove(idJugador);
                }

                if (callbackRemovido)
                {
                    ControladorPartida controlador;
                    partidaActiva = _partidasActivas.TryGetValue(idSala, out controlador);
                    if (partidaActiva)
                    {
                        partidaFinalizada = controlador.EstaFinalizada;
                        if (!partidaFinalizada)
                        {
                            controladorARemover = controlador;
                        }
                    }
                }
            }

            if (controladorARemover != null)
            {
                _logger.InfoFormat(
                    "Removiendo jugador {0} de sala {1} por cliente inalcanzable.",
                    idJugador,
                    idSala);
                nombreUsuario = controladorARemover.ObtenerNombreUsuarioPorId(idJugador);
                controladorARemover.RemoverJugador(idJugador);
            }

            return nombreUsuario;
        }

        private static void RegistrarCallback(
            string idSala,
            string idJugador,
            ICursoPartidaManejadorCallback callback)
        {
            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacks;
                if (!_callbacksPorSala.TryGetValue(idSala, out callbacks))
                {
                    callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>(
                        StringComparer.OrdinalIgnoreCase);

                    _callbacksPorSala[idSala] = callbacks;
                }

                callbacks[idJugador] = callback;
            }

            var canal = OperationContext.Current?.Channel;
            if (canal != null)
            {
                EventHandler manejadorClosed = null;
                EventHandler manejadorFaulted = null;

                manejadorClosed = delegate(object remitente, EventArgs argumentos)
                {
                    RemoverCallback(idSala, idJugador);
                };

                manejadorFaulted = delegate(object remitente, EventArgs argumentos)
                {
                    RemoverCallback(idSala, idJugador);
                };

                canal.Closed += manejadorClosed;
                canal.Faulted += manejadorFaulted;
            }
        }

        private static void RemoverCallback(string idSala, string idJugador)
        {
            ControladorPartida controladorARemover = null;
            bool callbackRemovido = false;
            bool partidaActiva = false;
            bool partidaFinalizada = false;

            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacks;
                if (_callbacksPorSala.TryGetValue(idSala, out callbacks))
                {
                    callbackRemovido = callbacks.Remove(idJugador);
                }

                if (callbackRemovido)
                {
                    ControladorPartida controlador;
                    partidaActiva = _partidasActivas.TryGetValue(idSala, out controlador);
                    if (partidaActiva)
                    {
                        partidaFinalizada = controlador.EstaFinalizada;
                        if (!partidaFinalizada)
                        {
                            controladorARemover = controlador;
                        }
                    }
                }
            }

            if (controladorARemover != null)
            {
                _logger.InfoFormat(
                    "Removiendo jugador {0} de sala {1}.",
                    idJugador,
                    idSala);
                controladorARemover.RemoverJugador(idJugador);
            }
        }

        private static ICursoPartidaManejadorCallback ObtenerCallbackActual()
        {
            var contexto = OperationContext.Current;
            if (contexto == null)
            {
                throw new FaultException(MensajesError.Cliente.ErrorContextoOperacion);
            }

            var callback = contexto.GetCallbackChannel<ICursoPartidaManejadorCallback>();
            if (callback == null)
            {
                throw new FaultException(MensajesError.Cliente.ErrorObtenerCallback);
            }

            return callback;
        }

        private ConfiguracionPartidaDTO ObtenerConfiguracionSala(string idSala)
        {
            try
            {
                return _salasManejador.ObtenerSalaPorCodigo(idSala)?.Configuracion;
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Error de comunicacion al obtener configuracion de sala {0}.",
                        idSala),
                    excepcion);
                return CrearConfiguracionPorDefecto();
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Timeout al obtener configuracion de sala {0}.",
                        idSala),
                    excepcion);
                return CrearConfiguracionPorDefecto();
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Canal cerrado al obtener configuracion de sala {0}.",
                        idSala),
                    excepcion);
                return CrearConfiguracionPorDefecto();
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Error inesperado al obtener configuracion de sala {0}.",
                        idSala),
                    excepcion);
                return CrearConfiguracionPorDefecto();
            }
        }

        private ConfiguracionPartidaDTO CrearConfiguracionPorDefecto()
        {
            return new ConfiguracionPartidaDTO
            {
                TiempoPorRondaSegundos = TiempoRondaPorDefectoSegundos,
                NumeroRondas = NumeroRondasPorDefecto,
                Dificultad = DificultadPorDefecto,
                IdiomaCanciones = "Espanol"
            };
        }
    }
}
