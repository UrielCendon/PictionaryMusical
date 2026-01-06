using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Implementacion del servicio de gestion de salas de juego.
    /// Maneja creacion, union, abandono y expulsion de salas con notificaciones en tiempo real
    /// via callbacks.
    /// Utiliza un diccionario concurrente para almacenar salas activas en memoria.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SalasManejador : ISalasManejador, IObtenerSalas
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SalasManejador));

        private readonly INotificadorSalas _notificador;
        private readonly IAlmacenSalas _almacenSalas;
        private readonly IProveedorContextoOperacion _proveedorContexto;
        private readonly ISalaInternaFactoria _salaFactoria;
        private readonly IGeneradorCodigoSala _generadorCodigo;
        private readonly ISesionUsuarioManejador _sesionManejador;

        /// <summary>
        /// Constructor por defecto que inicializa las dependencias.
        /// </summary>
        public SalasManejador()
        {
            _almacenSalas = AlmacenSalasEstatico.Instancia;
            _proveedorContexto = new ProveedorContextoOperacion();
            _salaFactoria = new SalaInternaFactoria();
            _generadorCodigo = new GeneradorCodigoSala(_almacenSalas);
            _notificador = new NotificadorSalas(this);
            _sesionManejador = SesionUsuarioManejador.Instancia;
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        public SalasManejador(
            INotificadorSalas notificador,
            IAlmacenSalas almacenSalas,
            IProveedorContextoOperacion proveedorContexto,
            ISalaInternaFactoria salaFactoria,
            IGeneradorCodigoSala generadorCodigo,
            ISesionUsuarioManejador sesionManejador = null)
        {
            _notificador = notificador ??
                throw new ArgumentNullException(nameof(notificador));
            _almacenSalas = almacenSalas ??
                throw new ArgumentNullException(nameof(almacenSalas));
            _proveedorContexto = proveedorContexto ??
                throw new ArgumentNullException(nameof(proveedorContexto));
            _salaFactoria = salaFactoria ??
                throw new ArgumentNullException(nameof(salaFactoria));
            _generadorCodigo = generadorCodigo ??
                throw new ArgumentNullException(nameof(generadorCodigo));
            _sesionManejador = sesionManejador ?? SesionUsuarioManejador.Instancia;
        }

        /// <summary>
        /// Obtiene la coleccion de salas internas para uso interno.
        /// </summary>
        /// <returns>Coleccion de salas internas.</returns>
        IEnumerable<SalaInternaManejador> IObtenerSalas.ObtenerSalasInternas()
        {
            return _almacenSalas.Valores;
        }

        /// <summary>
        /// Obtiene la lista de todas las salas de juego disponibles como DTOs.
        /// </summary>
        /// <returns>Lista de salas disponibles.</returns>
        public IList<SalaDTO> ObtenerSalas()
        {
            var resultado = new List<SalaDTO>();
            foreach (var sala in _almacenSalas.Valores)
            {
                resultado.Add(sala.ConvertirADto());
            }
            return resultado;
        }

        /// <summary>
        /// Crea una nueva sala de juego con la configuracion especificada.
        /// Genera un codigo unico, registra el callback del creador y notifica a todos.
        /// </summary>
        /// <param name="nombreCreador">Nombre del usuario que crea la sala.</param>
        /// <param name="configuracion">Configuracion de la partida para la sala.</param>
        /// <returns>Datos de la sala creada.</returns>
        public SalaDTO CrearSala(string nombreCreador, ConfiguracionPartidaDTO configuracion)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(nombreCreador, nameof(nombreCreador));
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion);

                string codigo = _generadorCodigo.GenerarCodigo();
                var callback = _proveedorContexto.ObtenerCallbackChannel<ISalasManejadorCallback>();

                var sala = _salaFactoria.Crear(
                    codigo,
                    nombreCreador.Trim(),
                    configuracion);

                sala.AgregarJugador(nombreCreador.Trim(), callback, notificar: false);

                if (!_almacenSalas.IntentarAgregar(codigo, sala))
                {
                    _logger.WarnFormat(
                        MensajesError.Bitacora.ErrorConcurrenciaCrearSala,
                        codigo);
                    throw new FaultException(MensajesError.Cliente.ErrorCrearSala);
                }

                SuscribirEventosDesconexionCanal(codigo, nombreCreador.Trim());

                _logger.InfoFormat(
                    MensajesError.Bitacora.SalaCreadaExito,
                    codigo);

                _notificador.NotificarListaSalasATodos();

                return sala.ConvertirADto();
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorValidacionCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorComunicacionCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoCrearSala);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorTimeoutCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoCrearSala);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCanalCerradoCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoCrearSala);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoCrearSala);
            }
        }

        /// <summary>
        /// Une un usuario a una sala de juego existente.
        /// Valida sala, agrega jugador, registra callback y notifica.
        /// </summary>
        /// <param name="codigoSala">Codigo identificador de la sala.</param>
        /// <param name="nombreUsuario">Nombre del usuario que se une a la sala.</param>
        /// <returns>Datos actualizados de la sala a la que se unio.</returns>
        public SalaDTO UnirseSala(string codigoSala, string nombreUsuario)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(nombreUsuario, nameof(nombreUsuario));

                if (!EntradaComunValidador.EsCodigoSalaValido(codigoSala))
                {
                    throw new FaultException(MensajesError.Cliente.CodigoSalaObligatorio);
                }

                SalaInternaManejador sala;
                if (!_almacenSalas.IntentarObtener(codigoSala.Trim(), out sala))
                {
                    throw new FaultException(MensajesError.Cliente.SalaNoEncontrada);
                }

                if (sala.PartidaIniciada)
                {
                    throw new FaultException(MensajesError.Cliente.PartidaComenzo);
                }

                var callback = _proveedorContexto.ObtenerCallbackChannel<ISalasManejadorCallback>();

                var resultado = sala.AgregarJugador(
                    nombreUsuario.Trim(),
                    callback,
                    notificar: true);

                SuscribirEventosDesconexionCanal(codigoSala.Trim(), nombreUsuario.Trim());

                _logger.InfoFormat(
                    MensajesError.Bitacora.UsuarioUnidoSala,
                    codigoSala.Trim());

                _notificador.NotificarListaSalasATodos();

                return resultado;
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorValidacionUnirse, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorOperacionUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorComunicacionUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoUnirse);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorTimeoutUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoUnirse);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCanalCerradoUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoUnirse);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoUnirse);
            }
        }

        /// <summary>
        /// Obtiene la lista de todas las salas de juego disponibles.
        /// </summary>
        /// <returns> Lista de salas disponibles.</returns>
        public IList<SalaDTO> ObtenerListaSalas()
        {
            try
            {
                return _almacenSalas.Valores
                    .Select(sala => sala.ConvertirADto())
                    .ToList();
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorOperacionInvalidaObtenerSalas, excepcion);
                return new List<SalaDTO>();
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoObtenerSalas, excepcion);
                return new List<SalaDTO>();
            }
        }

        /// <summary>
        /// Permite a un usuario abandonar una sala de juego.
        /// Remueve jugador, elimina sala si vacia y notifica.
        /// </summary>
        /// <param name="codigoSala">Codigo identificador de la sala.</param>
        /// <param name="nombreUsuario">Nombre del usuario que abandona la sala.</param>
        public void AbandonarSala(string codigoSala, string nombreUsuario)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(nombreUsuario, nameof(nombreUsuario));

                if (!EntradaComunValidador.EsCodigoSalaValido(codigoSala))
                {
                    throw new FaultException(MensajesError.Cliente.CodigoSalaObligatorio);
                }

                SalaInternaManejador sala;
                if (!_almacenSalas.IntentarObtener(codigoSala.Trim(), out sala))
                {
                    throw new FaultException(MensajesError.Cliente.SalaNoEncontrada);
                }

                sala.RemoverJugador(nombreUsuario.Trim());

                if (sala.DebeEliminarse)
                {
                    SalaInternaManejador salaRemovida;
                    _almacenSalas.IntentarRemover(codigoSala.Trim(), out salaRemovida);
                }

                _notificador.NotificarListaSalasATodos();
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorValidacionAbandonar, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorOperacionAbandonar, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorOperacionAbandonar, excepcion);
                throw new FaultException(
                    excepcion.Message ?? MensajesError.Cliente.ErrorOperacionInvalida);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorInesperadoAbandonar, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoAbandonar);
            }
        }

        /// <summary>
        /// Suscribe al cliente para recibir notificaciones sobre cambios en la lista de salas.
        /// </summary>
        public void SuscribirListaSalas()
        {
            try
            {
                var callback = _proveedorContexto.ObtenerCallbackChannel<ISalasManejadorCallback>();
                var sesionId = _notificador.Suscribir(callback);

                var canal = _proveedorContexto.ObtenerCanalActual();
                if (canal != null)
                {
                    EventHandler manejadorClosed = null;
                    EventHandler manejadorFaulted = null;

                    manejadorClosed = delegate(object remitente, EventArgs argumentos)
                    {
                        _notificador.Desuscribir(sesionId);
                    };

                    manejadorFaulted = delegate(object remitente, EventArgs argumentos)
                    {
                        _notificador.Desuscribir(sesionId);
                    };

                    canal.Closed += manejadorClosed;
                    canal.Faulted += manejadorFaulted;
                }

                _notificador.NotificarListaSalas(callback);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorSuscripcionListaSalas, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoSuscripcion);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorComunicacionSuscripcion, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoSuscripcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorTimeoutSuscripcion, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoSuscripcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoSuscripcion, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoSuscripcion);
            }
        }

        /// <summary>
        /// Cancela la suscripcion del cliente de notificaciones de la lista de salas.
        /// </summary>
        public void CancelarSuscripcionListaSalas()
        {
            try
            {
                var callback = _proveedorContexto.ObtenerCallbackChannel<ISalasManejadorCallback>();
                _notificador.DesuscribirPorCallback(callback);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCancelarSuscripcion, excepcion);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCancelarSuscripcion, excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCancelarSuscripcion, excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCancelarSuscripcion, excepcion);
            }
        }

        /// <summary>
        /// Expulsa un jugador de una sala de juego.
        /// Valida permisos de anfitrion, remueve jugador y notifica.
        /// </summary>
        /// <param name="codigoSala">Codigo identificador de la sala.</param>
        /// <param name="nombreHost">Nombre del usuario anfitrion que expulsa.</param>
        /// <param name="nombreJugadorAExpulsar">Nombre del jugador a expulsar.</param>
        public void ExpulsarJugador(
            string codigoSala,
            string nombreHost,
            string nombreJugadorAExpulsar)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(nombreHost, nameof(nombreHost));
                EntradaComunValidador.ValidarNombreUsuario(
                    nombreJugadorAExpulsar,
                    nameof(nombreJugadorAExpulsar));

                if (!EntradaComunValidador.EsCodigoSalaValido(codigoSala))
                {
                    throw new FaultException(MensajesError.Cliente.CodigoSalaObligatorio);
                }

                SalaInternaManejador sala;
                if (!_almacenSalas.IntentarObtener(codigoSala.Trim(), out sala))
                {
                    throw new FaultException(MensajesError.Cliente.SalaNoEncontrada);
                }

                _logger.InfoFormat(
                    MensajesError.Bitacora.ExpulsandoJugador,
                    codigoSala.Trim());

                sala.ExpulsarJugador(nombreHost.Trim(), nombreJugadorAExpulsar.Trim());

                _logger.InfoFormat(
                    MensajesError.Bitacora.JugadorExpulsadoExito,
                    codigoSala.Trim());

                if (sala.DebeEliminarse)
                {
                    SalaInternaManejador salaRemovida;
                    _almacenSalas.IntentarRemover(codigoSala.Trim(), out salaRemovida);
                }

                _notificador.NotificarListaSalasATodos();
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorValidacionExpulsar, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorOperacionExpulsar, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorOperacionExpulsar, excepcion);
                throw new FaultException(
                    excepcion.Message ?? MensajesError.Cliente.ErrorInesperadoExpulsar);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorInesperadoExpulsar, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoExpulsar);
            }
        }

        /// <summary>
        /// Banea a un jugador de una sala por exceso de reportes.
        /// Es una accion del sistema, no requiere permisos de anfitrion.
        /// </summary>
        /// <param name="codigoSala">Codigo identificador de la sala.</param>
        /// <param name="nombreJugadorABanear">Nombre del jugador a banear.</param>
        public void BanearJugador(string codigoSala, string nombreJugadorABanear)
        {
            try
            {
                if (!EntradaComunValidador.EsCodigoSalaValido(codigoSala))
                {
                    return;
                }

                SalaInternaManejador sala;
                if (!_almacenSalas.IntentarObtener(codigoSala.Trim(), out sala))
                {
                    return;
                }

                _logger.InfoFormat(
                    "Baneando jugador de sala '{0}' por exceso de reportes.",
                    codigoSala.Trim());

                sala.BanearJugador(nombreJugadorABanear.Trim());

                if (sala.DebeEliminarse)
                {
                    SalaInternaManejador salaRemovida;
                    _almacenSalas.IntentarRemover(codigoSala.Trim(), out salaRemovida);
                }

                _notificador.NotificarListaSalasATodos();
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Error al banear jugador de sala '{0}'.",
                        codigoSala),
                    excepcion);
            }
        }

        /// <summary>
        /// Obtiene una sala por su codigo identificador.
        /// </summary>
        /// <param name="codigoSala">Codigo identificador de la sala.</param>
        /// <returns>Datos de la sala como DTO.</returns>
        public SalaDTO ObtenerSalaPorCodigo(string codigoSala)
        {
            if (!EntradaComunValidador.EsCodigoSalaValido(codigoSala))
            {
                throw new InvalidOperationException(MensajesError.Cliente.CodigoSalaObligatorio);
            }

            SalaInternaManejador sala;
            if (_almacenSalas.IntentarObtener(codigoSala.Trim(), out sala))
            {
                return sala.ConvertirADto();
            }

            throw new InvalidOperationException(MensajesError.Cliente.SalaNoEncontrada);
        }

        /// <summary>
        /// Marca una sala como iniciada para prevenir que nuevos jugadores se unan.
        /// </summary>
        public void MarcarPartidaComoIniciada(string codigoSala)
        {
            SalaInternaManejador sala;
            if (_almacenSalas.IntentarObtener(codigoSala, out sala))
            {
                sala.PartidaIniciada = true;
            }
        }

        public void MarcarPartidaComoFinalizada(string codigoSala)
        {
            SalaInternaManejador sala;
            if (_almacenSalas.IntentarObtener(codigoSala, out sala))
            {
                sala.PartidaFinalizada = true;
            }

            SalaInternaManejador salaRemovida;
            _almacenSalas.IntentarRemover(codigoSala, out salaRemovida);

            _logger.InfoFormat(
                "Sala '{0}' eliminada por finalizacion de partida.",
                codigoSala);

            _notificador.NotificarListaSalasATodos();
        }


        private void SuscribirEventosDesconexionCanal(string codigoSala, string nombreUsuario)
        {
            var canal = _proveedorContexto.ObtenerCanalActual();
            if (canal == null)
            {
                return;
            }

            EventHandler manejadorClosed = null;
            EventHandler manejadorFaulted = null;

            manejadorClosed = delegate(object remitente, EventArgs argumentos)
            {
                canal.Closed -= manejadorClosed;
                canal.Faulted -= manejadorFaulted;
                ManejarSalidaJugadorSala(codigoSala, nombreUsuario);
            };

            manejadorFaulted = delegate(object remitente, EventArgs argumentos)
            {
                canal.Closed -= manejadorClosed;
                canal.Faulted -= manejadorFaulted;
                ManejarDesconexionJugador(codigoSala, nombreUsuario);
            };

            canal.Closed += manejadorClosed;
            canal.Faulted += manejadorFaulted;
        }

        /// <summary>
        /// Notifica que un jugador se desconecto durante la partida y debe ser removido de la sala.
        /// Este metodo es llamado desde CursoPartidaManejador cuando detecta una desconexion.
        /// No elimina la sesion global ya que puede ser una salida voluntaria.
        /// </summary>
        /// <param name="codigoSala">Codigo de la sala.</param>
        /// <param name="nombreUsuario">Nombre del usuario desconectado.</param>
        public void NotificarDesconexionJugador(string codigoSala, string nombreUsuario)
        {
            ManejarSalidaJugadorSala(codigoSala, nombreUsuario);
        }

        /// <summary>
        /// Notifica que un jugador perdio la conexion (error de comunicacion, timeout, etc.)
        /// y debe ser removido de la sala. Tambien elimina su sesion global porque el cliente
        /// ya no es alcanzable.
        /// </summary>
        /// <param name="codigoSala">Codigo de la sala.</param>
        /// <param name="nombreUsuario">Nombre del usuario desconectado.</param>
        public void NotificarClienteInalcanzable(string codigoSala, string nombreUsuario)
        {
            ManejarDesconexionJugador(codigoSala, nombreUsuario);
        }

        /// <summary>
        /// Remueve al jugador de la sala sin eliminar su sesion global.
        /// Se usa para cierres de canal normales (navegacion, salida voluntaria).
        /// </summary>
        private void ManejarSalidaJugadorSala(string codigoSala, string nombreUsuario)
        {
            try
            {
                SalaInternaManejador sala;
                if (!_almacenSalas.IntentarObtener(codigoSala, out sala))
                {
                    return;
                }

                sala.RemoverJugador(nombreUsuario);

                if (sala.DebeEliminarse)
                {
                    SalaInternaManejador salaRemovida;
                    _almacenSalas.IntentarRemover(codigoSala, out salaRemovida);
                }

                _notificador.NotificarListaSalasATodos();
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Error al remover jugador de sala '{0}'.",
                        codigoSala),
                    excepcion);
            }
        }

        /// <summary>
        /// Maneja la desconexion inesperada de un jugador (canal Faulted).
        /// Elimina la sesion global y remueve al jugador de la sala.
        /// </summary>
        private void ManejarDesconexionJugador(string codigoSala, string nombreUsuario)
        {
            try
            {
                _sesionManejador.EliminarSesionPorNombre(nombreUsuario);

                SalaInternaManejador sala;
                if (!_almacenSalas.IntentarObtener(codigoSala, out sala))
                {
                    return;
                }

                sala.RemoverJugador(nombreUsuario);

                if (sala.DebeEliminarse)
                {
                    SalaInternaManejador salaRemovida;
                    _almacenSalas.IntentarRemover(codigoSala, out salaRemovida);
                }

                _notificador.NotificarListaSalasATodos();
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    string.Format(
                        "Error al remover jugador de sala '{0}'.",
                        codigoSala),
                    excepcion);
            }
        }
    }
}
