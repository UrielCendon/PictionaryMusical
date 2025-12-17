using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Servicios.Servicios
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

        private static readonly ConcurrentDictionary<string, SalaInternaManejador> _salas =
            new ConcurrentDictionary<string, SalaInternaManejador>(
                StringComparer.OrdinalIgnoreCase);

        private readonly INotificadorSalas _notificador;

        /// <summary>
        /// Constructor por defecto que inicializa las dependencias.
        /// </summary>
        public SalasManejador()
        {
            _notificador = new NotificadorSalas(this);
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias.
        /// </summary>
        public SalasManejador(INotificadorSalas notificador)
        {
            _notificador = notificador ??
                throw new ArgumentNullException(nameof(notificador));
        }

        /// <summary>
        /// Obtiene la coleccion de salas internas para uso interno.
        /// </summary>
        /// <returns>Coleccion de salas internas.</returns>
        IEnumerable<SalaInternaManejador> IObtenerSalas.ObtenerSalasInternas()
        {
            return _salas.Values;
        }

        /// <summary>
        /// Obtiene la lista de todas las salas de juego disponibles como DTOs.
        /// </summary>
        /// <returns>Lista de salas disponibles.</returns>
        public IList<SalaDTO> ObtenerSalas()
        {
            var resultado = new List<SalaDTO>();
            foreach (var sala in _salas.Values)
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
                ValidarConfiguracion(configuracion);

                string codigo = GenerarCodigoSala();
                var callback = OperationContext.Current.GetCallbackChannel
                    <ISalasManejadorCallback>();

                var gestorNotificacionesSala = new GestorNotificacionesSalaInterna();

                var sala = new SalaInternaManejador(
                    codigo,
                    nombreCreador.Trim(),
                    configuracion,
                    gestorNotificacionesSala);

                sala.AgregarJugador(nombreCreador.Trim(), callback, notificar: false);

                if (!_salas.TryAdd(codigo, sala))
                {
                    _logger.WarnFormat(
                        MensajesError.Log.ErrorConcurrenciaCrearSala,
                        codigo);
                    throw new FaultException(MensajesError.Cliente.ErrorCrearSala);
                }

                _logger.InfoFormat(
                    MensajesError.Log.SalaCreadaExito,
                    codigo);

                _notificador.NotificarListaSalasATodos();

                return sala.ConvertirADto();
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorValidacionCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorComunicacionCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoCrearSala);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorTimeoutCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoCrearSala);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorCanalCerradoCrearSala, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoCrearSala);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorInesperadoCrearSala, excepcion);
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
                if (!_salas.TryGetValue(codigoSala.Trim(), out sala))
                {
                    throw new FaultException(MensajesError.Cliente.SalaNoEncontrada);
                }

                if (sala.PartidaIniciada)
                {
                    throw new FaultException(MensajesError.Cliente.PartidaComenzo);
                }

                var callback = OperationContext.Current.GetCallbackChannel
                    <ISalasManejadorCallback>();

                var resultado = sala.AgregarJugador(
                    nombreUsuario.Trim(),
                    callback,
                    notificar: true);

                _logger.InfoFormat(
                    MensajesError.Log.UsuarioUnidoSala,
                    codigoSala.Trim());

                _notificador.NotificarListaSalasATodos();

                return resultado;
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorValidacionUnirse, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorOperacionUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorComunicacionUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoUnirse);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorTimeoutUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoUnirse);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorCanalCerradoUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoUnirse);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorInesperadoUnirse, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoUnirse);
            }
        }

        /// <summary>
        /// Obtiene la lista de todas las salas de juego disponibles.
        /// </summary>
        /// <returns> Lista de salas disponibles.</returns>
        public static IList<SalaDTO> ObtenerListaSalas()
        {
            try
            {
                return _salas.Values
                    .Select(sala => sala.ConvertirADto())
                    .ToList();
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorOperacionInvalidaObtenerSalas, excepcion);
                return new List<SalaDTO>();
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorInesperadoObtenerSalas, excepcion);
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
                if (!_salas.TryGetValue(codigoSala.Trim(), out sala))
                {
                    throw new FaultException(MensajesError.Cliente.SalaNoEncontrada);
                }

                sala.RemoverJugador(nombreUsuario.Trim());
                _notificador.NotificarListaSalasATodos();
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorValidacionAbandonar, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorOperacionAbandonar, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorOperacionAbandonar, excepcion);
                throw new FaultException(
                    excepcion.Message ?? MensajesError.Cliente.ErrorOperacionInvalida);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorInesperadoAbandonar, excepcion);
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
                var callback = OperationContext.Current.GetCallbackChannel
                    <ISalasManejadorCallback>();
                var sesionId = _notificador.Suscribir(callback);

                var canal = OperationContext.Current?.Channel;
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
                _logger.Error(MensajesError.Log.ErrorSuscripcionListaSalas, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoSuscripcion);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorComunicacionSuscripcion, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoSuscripcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorTimeoutSuscripcion, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoSuscripcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorInesperadoSuscripcion, excepcion);
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
                var callback = OperationContext.Current.GetCallbackChannel
                    <ISalasManejadorCallback>();
                _notificador.DesuscribirPorCallback(callback);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorCancelarSuscripcion, excepcion);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorCancelarSuscripcion, excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorCancelarSuscripcion, excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorCancelarSuscripcion, excepcion);
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
                if (!_salas.TryGetValue(codigoSala.Trim(), out sala))
                {
                    throw new FaultException(MensajesError.Cliente.SalaNoEncontrada);
                }

                _logger.InfoFormat(
                    MensajesError.Log.ExpulsandoJugador,
                    codigoSala.Trim());

                sala.ExpulsarJugador(nombreHost.Trim(), nombreJugadorAExpulsar.Trim());

                _logger.InfoFormat(
                    MensajesError.Log.JugadorExpulsadoExito,
                    codigoSala.Trim());

                if (sala.DebeEliminarse)
                {
                    SalaInternaManejador salaRemovida;
                    _salas.TryRemove(codigoSala.Trim(), out salaRemovida);
                }

                _notificador.NotificarListaSalasATodos();
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorValidacionExpulsar, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorOperacionExpulsar, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorOperacionExpulsar, excepcion);
                throw new FaultException(
                    excepcion.Message ?? MensajesError.Cliente.ErrorInesperadoExpulsar);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorInesperadoExpulsar, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorInesperadoExpulsar);
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
            if (_salas.TryGetValue(codigoSala.Trim(), out sala))
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
            if (_salas.TryGetValue(codigoSala, out sala))
            {
                sala.PartidaIniciada = true;
            }
        }

        public void MarcarPartidaComoFinalizada(string codigoSala)
        {
            SalaInternaManejador sala;
            if (_salas.TryGetValue(codigoSala, out sala))
            {
                sala.PartidaFinalizada = true;
            }
        }


        private static string GenerarCodigoSala()
        {
            const int maxIntentos = 1000;

            for (int i = 0; i < maxIntentos; i++)
            {
                string codigo = GeneradorAleatorio.GenerarCodigoSala(6);
                if (!_salas.ContainsKey(codigo))
                {
                    return codigo;
                }
            }

            _logger.Error(MensajesError.Log.ErrorGenerarCodigoSala);
            throw new FaultException(MensajesError.Cliente.ErrorGenerarCodigo);
        }

        private static void ValidarConfiguracion(ConfiguracionPartidaDTO configuracion)
        {
            if (configuracion == null)
            {
                throw new FaultException(MensajesError.Cliente.ConfiguracionObligatoria);
            }

            if (configuracion.NumeroRondas <= 0 || 
                configuracion.NumeroRondas > EntradaComunValidador.NumeroRondasMaximo)
            {
                throw new FaultException(MensajesError.Cliente.NumeroRondasInvalido);
            }

            if (configuracion.TiempoPorRondaSegundos <= 0 ||
                configuracion.TiempoPorRondaSegundos > EntradaComunValidador.TiempoRondaMaximoSegundos)
            {
                throw new FaultException(MensajesError.Cliente.TiempoRondaInvalido);
            }

            if (!EntradaComunValidador.EsIdiomaValido(configuracion.IdiomaCanciones))
            {
                throw new FaultException(MensajesError.Cliente.IdiomaObligatorio);
            }

            if (!EntradaComunValidador.EsDificultadValida(configuracion.Dificultad))
            {
                throw new FaultException(MensajesError.Cliente.DificultadObligatoria);
            }
        }
    }
}
