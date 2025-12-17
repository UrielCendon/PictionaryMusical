using log4net;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace PictionaryMusicalServidor.Servicios.Servicios
{
    /// <summary>
    /// Implementa una fachada para administrar el curso de las partidas activas.
    /// Gestiona los callbacks y delega la logica al ControladorPartida por sala.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CursoPartidaManejador : ICursoPartidaManejador
    {
        private const int TiempoRondaPorDefectoSegundos = 90;
        private const int NumeroRondasPorDefecto = 3;
        private const string DificultadPorDefecto = "Media";
        private const int LimiteCaracteresMensaje = 150;

        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(CursoPartidaManejador));

        private static readonly Dictionary<string, ControladorPartida> _partidasActivas =
            new Dictionary<string, ControladorPartida>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string,
            Dictionary<string, ICursoPartidaManejadorCallback>> _callbacksPorSala =
            new Dictionary<string, Dictionary<string, ICursoPartidaManejadorCallback>>(
                StringComparer.OrdinalIgnoreCase);

        private static readonly object _sincronizacion = new object();

        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;
        private readonly ISalasManejador _salasManejador;
        private readonly ICatalogoCanciones _catalogoCanciones;

        /// <summary>
        /// Constructor por defecto para uso en WCF.
        /// </summary>
        public CursoPartidaManejador() : this(
            new ContextoFactoria(),
            new RepositorioFactoria(),
            new SalasManejador(),
            new CatalogoCanciones())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        /// <param name="salasManejador">Manejador de salas.</param>
        /// <param name="catalogoCanciones">Catalogo de canciones.</param>
        public CursoPartidaManejador(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria,
            ISalasManejador salasManejador,
            ICatalogoCanciones catalogoCanciones)
        {
            _contextoFactoria = contextoFactoria
                ?? throw new ArgumentNullException(nameof(contextoFactoria));

            _repositorioFactoria = repositorioFactoria
                ?? throw new ArgumentNullException(nameof(repositorioFactoria));

            _salasManejador = salasManejador
                ?? throw new ArgumentNullException(nameof(salasManejador));

            _catalogoCanciones = catalogoCanciones
                ?? throw new ArgumentNullException(nameof(catalogoCanciones));
        }

        /// <summary>
        /// Registra a un jugador para recibir notificaciones de la partida.
        /// </summary>
        public void SuscribirJugador(string idSala, string idJugador, string nombreUsuario,
            bool esHost)
        {
            if (string.IsNullOrWhiteSpace(idSala))
            {
                throw new FaultException(MensajesError.Cliente.IdSalaObligatorio);
            }

            if (string.IsNullOrWhiteSpace(idJugador))
            {
                throw new FaultException(MensajesError.Cliente.IdJugadorObligatorio);
            }

            var callback = ObtenerCallbackActual();
            var controlador = ObtenerOCrearControlador(idSala.Trim());

            controlador.AgregarJugador(
                idJugador.Trim(),
                nombreUsuario?.Trim() ?? string.Empty,
                esHost);

            RegistrarCallback(idSala.Trim(), idJugador.Trim(), callback);

            _logger.InfoFormat(
                MensajesError.Log.JugadorSuscritoPartida,
                idJugador.Trim(),
                idSala.Trim());
        }

        /// <summary>
        /// Inicia la partida de la sala indicada.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idJugadorSolicitante">Identificador del jugador que solicita el inicio.
        /// </param>
        public void IniciarPartida(string idSala, string idJugadorSolicitante)
        {
            if (string.IsNullOrWhiteSpace(idSala))
            {
                throw new FaultException(MensajesError.Cliente.IdSalaObligatorio);
            }

            _logger.InfoFormat(
                MensajesError.Log.InicioPartidaSolicitado,
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
            if (string.IsNullOrWhiteSpace(idSala))
            {
                throw new FaultException(MensajesError.Cliente.IdSalaObligatorio);
            }

            if (SuperaLimiteCaracteres(mensaje))
            {
                throw new FaultException(MensajesError.Cliente.MensajeSuperaLimiteCaracteres);
            }

            var controlador = ObtenerOCrearControlador(idSala.Trim());
            controlador.ProcesarMensaje(idJugador?.Trim(), mensaje);
        }

        private static bool SuperaLimiteCaracteres(string mensaje)
        {
            return !string.IsNullOrWhiteSpace(mensaje) && mensaje.Length > LimiteCaracteresMensaje;
        }

        /// <summary>
        /// Procesa un trazo dibujado por el jugador actual de la sala.
        /// </summary>
        public void EnviarTrazo(TrazoDTO trazo, string idSala, string idJugador)
        {
            if (string.IsNullOrWhiteSpace(idSala))
            {
                throw new FaultException(MensajesError.Cliente.IdSalaObligatorio);
            }

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
                var gestorJugadores = new GestorJugadoresPartida();

                var controlador = new ControladorPartida(
                    configuracion?.TiempoPorRondaSegundos ?? TiempoRondaPorDefectoSegundos,
                    configuracion?.Dificultad ?? DificultadPorDefecto,
                    configuracion?.NumeroRondas ?? NumeroRondasPorDefecto,
                    _catalogoCanciones,
                    gestorJugadores);

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

            controlador.FinRonda += delegate()
            {
                ManejarFinRonda(idSala);
            };

            controlador.FinPartida += delegate(ResultadoPartidaDTO resultado)
            {
                ManejarFinPartida(idSala, resultado, controlador);
            };
        }

        private static void ManejarPartidaIniciada(string idSala)
        {
            NotificarCallbacksPartidaIniciada(idSala);
        }

        private void ManejarInicioRonda(
            string idSala,
            RondaDTO rondaBase,
            ControladorPartida controlador)
        {
            var jugadoresEstado = controlador.ObtenerJugadores();
            var dibujante = jugadoresEstado.FirstOrDefault(j => j.EsDibujante);
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
            var datosJugador = jugadoresEstado.FirstOrDefault(j =>
                string.Equals(j.IdConexion, idJugador, StringComparison.OrdinalIgnoreCase));

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
                _logger.WarnFormat(MensajesError.Log.ErrorNotificandoInicioRonda, idJugador, excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.WarnFormat(MensajesError.Log.ErrorNotificandoInicioRonda, idJugador, excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.WarnFormat(MensajesError.Log.ErrorNotificandoInicioRonda, idJugador, excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    "Error inesperado al notificar jugador {0}: {1}",
                    idJugador,
                    excepcion);
                RemoverCallback(idSala, idJugador);
            }
        }

        private static void ManejarJugadorAdivino(string idSala, string jugador, int puntos)
        {
            NotificarCallbacksJugadorAdivino(idSala, jugador, puntos);
        }

        private static void ManejarMensajeChat(string idSala, string jugador, string mensaje)
        {
            NotificarCallbacksMensajeChat(idSala, jugador, mensaje);
        }

        private static void ManejarTrazoRecibido(string idSala, TrazoDTO trazo)
        {
            NotificarCallbacksTrazoRecibido(idSala, trazo);
        }

        private static void ManejarFinRonda(string idSala)
        {
            NotificarCallbacksFinRonda(idSala);
        }

        private void ManejarFinPartida(
            string idSala,
            ResultadoPartidaDTO resultado,
            ControladorPartida controlador)
        {
            _salasManejador.MarcarPartidaComoFinalizada(idSala);
            Task.Run(delegate()
            {
                ActualizarClasificacionPartida(controlador, resultado);
            });
            NotificarCallbacksFinPartida(idSala, resultado);
        }

        private void ActualizarClasificacionPartida(
            ControladorPartida controlador,
            ResultadoPartidaDTO resultado)
        {
            if (controlador == null ||
                resultado?.Clasificacion == null ||
                !resultado.Clasificacion.Any())
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
            {
                _logger.Info(MensajesError.Log.PartidaFinalizadaSinClasificacion);
                return;
            }

            var jugadoresFinales = ObtenerJugadoresFinales(controlador);

            if (jugadoresFinales == null || jugadoresFinales.Count == 0)
            {
                return;
            }

            var ganadores = CalcularGanadores(jugadoresFinales);

            try
            {
                using (var contexto = _contextoFactoria.CrearContexto())
                {
                    var clasificacionRepositorio = 
                        _repositorioFactoria.CrearClasificacionRepositorio(contexto);

                    foreach (var jugador in jugadoresFinales)
                    {
                        PersistirEstadisticasJugador(
                            clasificacionRepositorio,
                            jugador,
                            ganadores);
                    }
                }
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorActualizarClasificaciones, excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorActualizarClasificaciones, excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorDatosActualizarClasificaciones, excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorInesperadoActualizarClasificaciones, excepcion);
            }
        }

        private static List<JugadorPartida> ObtenerJugadoresFinales(ControladorPartida controlador)
        {
            try
            {
                var jugadores = controlador.ObtenerJugadores();
                if (jugadores == null)
                {
                    return new List<JugadorPartida>();
                }
                return new List<JugadorPartida>(jugadores);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesError.Log.ErrorObtenerJugadoresClasificacion,
                    excepcion);
                return new List<JugadorPartida>();
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesError.Log.ErrorObtenerJugadoresClasificacion,
                    excepcion);
                return new List<JugadorPartida>();
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesError.Log.ErrorDatosObtenerJugadoresClasificacion,
                    excepcion);
                return new List<JugadorPartida>();
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Log.ErrorInesperadoObtenerJugadoresClasificacion,
                    excepcion);
                return new List<JugadorPartida>();
            }
        }

        private static HashSet<string> CalcularGanadores(List<JugadorPartida> jugadores)
        {
            int puntajeMaximo = jugadores.Max(j => j.PuntajeTotal);

            return new HashSet<string>(
                jugadores
                    .Where(j => j.PuntajeTotal == puntajeMaximo)
                    .Select(j => j.IdConexion),
                StringComparer.OrdinalIgnoreCase);
        }

        private static void PersistirEstadisticasJugador(
            IClasificacionRepositorio repositorio,
            JugadorPartida jugador,
            HashSet<string> ganadores)
        {
            int jugadorId;
            if (!int.TryParse(jugador.IdConexion, out jugadorId) || jugadorId <= 0)
            {
                return;
            }

            bool ganoPartida = ganadores.Contains(jugador.IdConexion);

            try
            {
                repositorio.ActualizarEstadisticas(
                    jugadorId,
                    jugador.PuntajeTotal,
                    ganoPartida);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesError.Log.ErrorActualizarClasificacionJugador,
                    jugadorId,
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesError.Log.ErrorActualizarClasificacionJugador,
                    jugadorId,
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.ErrorFormat(
                    "Error de datos al actualizar clasificacion del jugador id {0}.",
                    jugadorId,
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    "Error inesperado al actualizar clasificacion del jugador id {0}.",
                    jugadorId,
                    excepcion);
            }
        }

        private static void NotificarCallbacksPartidaIniciada(string idSala)
        {
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

            foreach (var par in callbacks)
            {
                NotificarCallbackSeguro(par.Value, par.Key, idSala,
                    delegate(ICursoPartidaManejadorCallback cb) { cb.NotificarPartidaIniciada(); });
            }
        }

        private static void NotificarCallbacksJugadorAdivino(
            string idSala, 
            string jugador, 
            int puntos)
        {
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

            foreach (var par in callbacks)
            {
                NotificarCallbackSeguro(par.Value, par.Key, idSala,
                    delegate(ICursoPartidaManejadorCallback cb) 
                    { 
                        cb.NotificarJugadorAdivino(jugador, puntos); 
                    });
            }
        }

        private static void NotificarCallbacksMensajeChat(
            string idSala, 
            string jugador, 
            string mensaje)
        {
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

            foreach (var par in callbacks)
            {
                NotificarCallbackSeguro(par.Value, par.Key, idSala,
                    delegate(ICursoPartidaManejadorCallback cb) 
                    { 
                        cb.NotificarMensajeChat(jugador, mensaje); 
                    });
            }
        }

        private static void NotificarCallbacksTrazoRecibido(string idSala, TrazoDTO trazo)
        {
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

            foreach (var par in callbacks)
            {
                NotificarCallbackSeguro(par.Value, par.Key, idSala,
                    delegate(ICursoPartidaManejadorCallback cb) 
                    { 
                        cb.NotificarTrazoRecibido(trazo); 
                    });
            }
        }

        private static void NotificarCallbacksFinRonda(string idSala)
        {
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

            foreach (var par in callbacks)
            {
                NotificarCallbackSeguro(par.Value, par.Key, idSala,
                    delegate(ICursoPartidaManejadorCallback cb) { cb.NotificarFinRonda(); });
            }
        }

        private static void NotificarCallbacksFinPartida(
            string idSala, 
            ResultadoPartidaDTO resultado)
        {
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

            foreach (var par in callbacks)
            {
                NotificarCallbackSeguro(par.Value, par.Key, idSala,
                    delegate(ICursoPartidaManejadorCallback cb) 
                    { 
                        cb.NotificarFinPartida(resultado); 
                    });
            }
        }

        private static void NotificarCallbackSeguro(
            ICursoPartidaManejadorCallback callback,
            string idJugador,
            string idSala,
            Action<ICursoPartidaManejadorCallback> accion)
        {
            try
            {
                if (!CanalActivo(callback))
                {
                    _logger.WarnFormat(
                        "Canal inactivo para jugador {0} en sala {1}. Removiendo.",
                        idJugador,
                        idSala);

                    RemoverCallback(idSala, idJugador);
                    return;
                }
                accion(callback);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    string.Format("Canal desechado para jugador {0} en sala {1}. Removiendo.", 
                        idJugador, idSala),
                    excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (CommunicationObjectFaultedException excepcion)
            {
                _logger.Warn(
                    string.Format("Canal en falta para jugador {0} en sala {1}. Removiendo.", 
                        idJugador, idSala),
                    excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    string.Format("Error comunicacion con jugador {0} en sala {1}. Removiendo.", 
                        idJugador, idSala),
                    excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    string.Format("Timeout con jugador {0} en sala {1}. Removiendo.", 
                        idJugador, idSala),
                    excepcion);
                RemoverCallback(idSala, idJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    string.Format("Error inesperado con jugador {0} en sala {1}. Removiendo.", 
                        idJugador, idSala),
                    excepcion);
                RemoverCallback(idSala, idJugador);
            }
        }

        private static bool CanalActivo(ICursoPartidaManejadorCallback callback)
        {
            var canal = callback as ICommunicationObject;
            if (canal != null)
            {
                return canal.State == CommunicationState.Opened;
            }

            return true;
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
            lock (_sincronizacion)
            {
                Dictionary<string, ICursoPartidaManejadorCallback> callbacks;
                if (_callbacksPorSala.TryGetValue(idSala, out callbacks))
                {
                    callbacks.Remove(idJugador);
                }

                ControladorPartida controlador;
                if (_partidasActivas.TryGetValue(idSala, out controlador)
                    && !controlador.EstaFinalizada)
                {
                    controlador.RemoverJugador(idJugador);
                }
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
                    string.Format("Error de comunicacion al obtener configuracion de sala {0}.", idSala),
                    excepcion);
                return CrearConfiguracionPorDefecto();
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    string.Format("Timeout al obtener configuracion de sala {0}.", idSala),
                    excepcion);
                return CrearConfiguracionPorDefecto();
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    string.Format("Canal cerrado al obtener configuracion de sala {0}.", idSala),
                    excepcion);
                return CrearConfiguracionPorDefecto();
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    string.Format("Error inesperado al obtener configuracion de sala {0}.", idSala),
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
