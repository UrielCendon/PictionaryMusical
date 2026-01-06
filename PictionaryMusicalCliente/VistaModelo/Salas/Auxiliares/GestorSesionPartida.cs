using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.PictionaryServidorServicioCursoPartida;
using PictionaryMusicalCliente.Properties.Langs;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares
{
    /// <summary>
    /// Gestiona la sesion de partida y los callbacks del servicio WCF.
    /// Desacopla la logica de comunicacion de red de la capa de presentacion.
    /// </summary>
    [CallbackBehavior(
        ConcurrencyMode = ConcurrencyMode.Reentrant,
        UseSynchronizationContext = false)]
    public sealed class GestorSesionPartida : ICursoPartidaManejadorCallback, IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int SegundosCierreCanal = 2;

        private readonly IWcfClienteFabrica _fabricaClientes;
        private readonly string _codigoSala;
        private readonly string _idJugador;
        private readonly string _nombreUsuario;
        private readonly bool _esHost;

        private ICursoPartidaManejador _proxyJuego;
        private bool _dispuesto;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GestorSesionPartida"/>.
        /// </summary>
        /// <param name="parametros">Parametros de configuracion del gestor.</param>
        public GestorSesionPartida(GestorSesionPartidaParametros parametros)
        {
            if (parametros == null)
            {
                throw new ArgumentNullException(nameof(parametros));
            }

            _fabricaClientes = parametros.FabricaClientes;
            _codigoSala = parametros.CodigoSala;
            _idJugador = parametros.IdJugador;
            _nombreUsuario = parametros.NombreUsuario;
            _esHost = parametros.EsHost;
        }

        /// <summary>
        /// Se dispara cuando la partida ha sido iniciada.
        /// </summary>
        public event Action PartidaIniciada;

        /// <summary>
        /// Se dispara cuando inicia una nueva ronda.
        /// </summary>
        public event Action<DTOs.RondaDTO> RondaIniciada;

        /// <summary>
        /// Se dispara cuando un jugador adivina correctamente.
        /// </summary>
        public event Action<string, int> JugadorAdivino;

        /// <summary>
        /// Se dispara cuando se recibe un mensaje de chat.
        /// </summary>
        public event Action<string, string> MensajeChatRecibido;

        /// <summary>
        /// Se dispara cuando se recibe un trazo de dibujo.
        /// </summary>
        public event Action<DTOs.TrazoDTO> TrazoRecibido;

        /// <summary>
        /// Se dispara cuando finaliza una ronda.
        /// </summary>
        public event Action<bool> RondaFinalizada;

        /// <summary>
        /// Se dispara cuando finaliza la partida.
        /// </summary>
        public event Action<DTOs.ResultadoPartidaDTO> PartidaFinalizada;

        /// <summary>
        /// Se dispara cuando el canal entra en estado fallido.
        /// </summary>
        public event Action CanalFallido;

        /// <summary>
        /// Se dispara cuando el canal es cerrado.
        /// </summary>
        public event Action CanalCerrado;

        /// <summary>
        /// Se dispara cuando ocurre un error de comunicacion.
        /// </summary>
        public event Action<string> ErrorComunicacion;

        /// <summary>
        /// Indica si el proxy de juego esta disponible.
        /// </summary>
        public bool ProxyDisponible => _proxyJuego != null;

        /// <summary>
        /// Inicializa la conexion con el servidor y suscribe al jugador.
        /// </summary>
        /// <returns>True si la inicializacion fue exitosa.</returns>
        public bool Inicializar()
        {
            try
            {
                var contexto = new InstanceContext(this);
                _proxyJuego = _fabricaClientes.CrearClienteCursoPartida(contexto);

                SuscribirEventosCanal();
                SuscribirJugador();
                return true;
            }
            catch (FaultException excepcion)
            {
                _logger.Error(
                    "Fallo del servicio al suscribir al jugador en la partida.",
                    excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoErrorProcesarSolicitud);
                return false;
            }
            catch (CommunicationObjectFaultedException excepcion)
            {
                _logger.Error(
                    "Canal en estado fallido al suscribir al jugador.",
                    excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoConexionInterrumpida);
                return false;
            }
            catch (CommunicationObjectAbortedException excepcion)
            {
                _logger.Error(
                    "Canal abortado al suscribir al jugador.",
                    excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoConexionInterrumpida);
                return false;
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(
                    "Error de comunicacion al suscribir al jugador en la partida.",
                    excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoDesconexionServidor);
                return false;
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(
                    "Se agoto el tiempo para inicializar el proxy de partida.",
                    excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoTiempoAgotadoConexion);
                return false;
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(
                    "Operacion invalida al inicializar el proxy de partida.",
                    excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoErrorProcesarSolicitud);
                return false;
            }
        }

        private void SuscribirEventosCanal()
        {
            if (_proxyJuego is ICommunicationObject canal)
            {
                canal.Faulted += Canal_Faulted;
                canal.Closed += Canal_Closed;
            }
        }

        private void DesuscribirEventosCanal()
        {
            if (_proxyJuego is ICommunicationObject canal)
            {
                canal.Faulted -= Canal_Faulted;
                canal.Closed -= Canal_Closed;
            }
        }

        private void SuscribirJugador()
        {
            var suscripcion = new DTOs.SuscripcionJugadorDTO
            {
                IdSala = _codigoSala,
                IdJugador = _idJugador,
                NombreUsuario = _nombreUsuario,
                EsHost = _esHost
            };
            _proxyJuego.SuscribirJugador(suscripcion);
        }

        private void Canal_Faulted(object remitente, EventArgs argumentosEvento)
        {
            _logger.Error("El canal de comunicacion con el servidor entro en estado Faulted.");
            EjecutarEnDispatcher(() => CanalFallido?.Invoke());
        }

        private void Canal_Closed(object remitente, EventArgs argumentosEvento)
        {
            _logger.Info("El canal de comunicacion con el servidor fue cerrado.");
            EjecutarEnDispatcher(() => CanalCerrado?.Invoke());
        }

        /// <summary>
        /// Envia un mensaje de chat al servidor.
        /// </summary>
        /// <param name="mensaje">Contenido del mensaje.</param>
        public async Task EnviarMensajeAsync(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return;
            }

            if (_proxyJuego == null)
            {
                _logger.Warn("Proxy de juego no disponible para enviar mensaje.");
                return;
            }

            try
            {
                await _proxyJuego.EnviarMensajeJuegoAsync(mensaje, _codigoSala, _idJugador)
                    .ConfigureAwait(false);
            }
            catch (FaultException excepcion)
            {
                _logger.Error("Fallo del servicio al enviar mensaje de juego.", excepcion);
                throw;
            }
            catch (CommunicationObjectFaultedException excepcion)
            {
                _logger.Error("Canal fallido al enviar mensaje de juego.", excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoConexionInterrumpida);
                throw;
            }
            catch (CommunicationObjectAbortedException excepcion)
            {
                _logger.Error("Canal abortado al enviar mensaje de juego.", excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoConexionInterrumpida);
                throw;
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("No se pudo enviar el mensaje de juego.", excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoServidorNoDisponible);
                throw;
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error("Tiempo agotado al enviar mensaje de juego.", excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoServidorNoDisponible);
                throw;
            }
        }

        /// <summary>
        /// Envia un trazo de dibujo al servidor.
        /// </summary>
        /// <param name="trazo">Datos del trazo.</param>
        public void EnviarTrazo(DTOs.TrazoDTO trazo)
        {
            if (trazo == null)
            {
                return;
            }

            try
            {
                _proxyJuego?.EnviarTrazo(trazo, _codigoSala, _idJugador);
            }
            catch (FaultException excepcion)
            {
                _logger.Error("Fallo del servicio al enviar trazo al servidor.", excepcion);
            }
            catch (CommunicationObjectFaultedException excepcion)
            {
                _logger.Error("Canal fallido al enviar trazo.", excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoConexionInterrumpida);
            }
            catch (CommunicationObjectAbortedException excepcion)
            {
                _logger.Error("Canal abortado al enviar trazo.", excepcion);
                ErrorComunicacion?.Invoke(Lang.errorTextoConexionInterrumpida);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("No se pudo enviar el trazo al servidor.", excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error("Tiempo agotado al enviar trazo al servidor.", excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Operacion invalida al enviar trazo al servidor.", excepcion);
            }
        }

        /// <summary>
        /// Solicita al servidor iniciar la partida.
        /// </summary>
        public async Task IniciarPartidaAsync()
        {
            if (_proxyJuego == null)
            {
                throw new InvalidOperationException("Proxy de juego no disponible.");
            }

            await _proxyJuego.IniciarPartidaAsync(_codigoSala, _idJugador)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Aborta el canal de comunicacion inmediatamente.
        /// </summary>
        public void AbortarCanal()
        {
            try
            {
                if (_proxyJuego is ICommunicationObject canal)
                {
                    canal.Abort();
                    _proxyJuego = null;
                }
            }
            catch (Exception excepcion)
            {
                _logger.Warn("Error al abortar canal de partida.", excepcion);
            }
        }

        /// <summary>
        /// Cierra el canal de comunicacion de forma ordenada.
        /// </summary>
        public async Task CerrarCanalAsync()
        {
            if (!(_proxyJuego is ICommunicationObject canal))
            {
                return;
            }

            try
            {
                if (canal.State == CommunicationState.Faulted)
                {
                    canal.Abort();
                    return;
                }

                var cierreCompletado = await Task.Run(() =>
                {
                    try
                    {
                        canal.Close(TimeSpan.FromSeconds(SegundosCierreCanal));
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }).ConfigureAwait(false);

                if (!cierreCompletado)
                {
                    canal.Abort();
                }
            }
            catch (Exception excepcion)
            {
                _logger.Warn("Error al cerrar canal de partida, abortando.", excepcion);
                try
                {
                    canal.Abort();
                }
                catch (Exception excepcionAbortar)
                {
                    _logger.Warn("Error al abortar canal de partida.", excepcionAbortar);
                }
            }
            finally
            {
                _proxyJuego = null;
            }
        }

        void ICursoPartidaManejadorCallback.NotificarPartidaIniciada()
        {
            EjecutarEnDispatcher(() => PartidaIniciada?.Invoke());
        }

        void ICursoPartidaManejadorCallback.NotificarInicioRonda(DTOs.RondaDTO ronda)
        {
            EjecutarEnDispatcher(() => RondaIniciada?.Invoke(ronda));
        }

        void ICursoPartidaManejadorCallback.NotificarJugadorAdivino(
            string nombreJugador,
            int puntos)
        {
            EjecutarEnDispatcher(() => JugadorAdivino?.Invoke(nombreJugador, puntos));
        }

        void ICursoPartidaManejadorCallback.NotificarMensajeChat(
            string nombreJugador,
            string mensaje)
        {
            EjecutarEnDispatcher(() => MensajeChatRecibido?.Invoke(nombreJugador, mensaje));
        }

        void ICursoPartidaManejadorCallback.NotificarTrazoRecibido(DTOs.TrazoDTO trazo)
        {
            EjecutarEnDispatcher(() => TrazoRecibido?.Invoke(trazo));
        }

        void ICursoPartidaManejadorCallback.NotificarFinRonda(bool tiempoAgotado)
        {
            EjecutarEnDispatcher(() => RondaFinalizada?.Invoke(tiempoAgotado));
        }

        void ICursoPartidaManejadorCallback.NotificarFinPartida(
            DTOs.ResultadoPartidaDTO resultado)
        {
            EjecutarEnDispatcher(() => PartidaFinalizada?.Invoke(resultado));
        }

        private static void EjecutarEnDispatcher(Action accion)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.BeginInvoke(accion);
        }

        /// <summary>
        /// Libera los recursos utilizados por el gestor.
        /// </summary>
        public void Dispose()
        {
            if (_dispuesto)
            {
                return;
            }

            _dispuesto = true;
            DesuscribirEventosCanal();
            AbortarCanal();
        }
    }
}
