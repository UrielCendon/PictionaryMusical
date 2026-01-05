using System;
using System.Collections.Generic;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Implementacion del servicio de chat entre jugadores.
    /// Gestiona la comunicacion en tiempo real entre jugadores conectados a una sala.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChatManejador : IChatManejador
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ChatManejador));

        private static readonly IAlmacenClientesChat _almacenGlobal = new AlmacenClientesChat();

        private readonly IAlmacenClientesChat _almacenClientes;
        private readonly IProveedorContextoOperacion _proveedorContexto;

        /// <summary>
        /// Constructor por defecto para WCF.
        /// </summary>
        public ChatManejador()
            : this(_almacenGlobal, new ProveedorContextoOperacion())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="almacenClientes">Almacen de clientes de chat.</param>
        /// <param name="proveedorContexto">Proveedor de contexto de operacion.</param>
        public ChatManejador(
            IAlmacenClientesChat almacenClientes,
            IProveedorContextoOperacion proveedorContexto)
        {
            _almacenClientes = almacenClientes
                ?? throw new ArgumentNullException(nameof(almacenClientes));
            _proveedorContexto = proveedorContexto
                ?? throw new ArgumentNullException(nameof(proveedorContexto));
        }

        /// <summary>
        /// Permite a un jugador unirse al chat de una sala especifica.
        /// Registra el callback del cliente y notifica a los demas participantes.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugador">Nombre del jugador que se une.</param>
        public void UnirseChatSala(string idSala, string nombreJugador)
        {
            try
            {
                EntradaComunValidador.ValidarEntradaSalaChat(idSala, nombreJugador);

                var callback = ObtenerCallbackActual();
                var idSalaNormalizado = idSala.Trim();
                var nombreNormalizado = nombreJugador.Trim();

                var clientesANotificar = GestionarIngresoSala(
                    idSalaNormalizado,
                    nombreNormalizado,
                    callback);

                NotificarIngresoMasivo(
                    idSalaNormalizado,
                    nombreNormalizado,
                    clientesANotificar);

                ConfigurarEventosCanal(idSalaNormalizado, nombreNormalizado);
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorValidacionChat, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.DatosInvalidosChat, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorComunicacionChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorUnirseChat);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorTimeoutChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorUnirseChat);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCanalCerradoChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorUnirseChat);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorUnirseChat);
            }
        }

        /// <summary>
        /// Envia un mensaje a todos los participantes del chat de una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="mensaje">Contenido del mensaje a enviar.</param>
        /// <param name="nombreJugador">Nombre del jugador que envia el mensaje.</param>
        public void EnviarMensaje(string idSala, string mensaje, string nombreJugador)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(nombreJugador, nameof(nombreJugador));

                if (!EntradaComunValidador.EsCodigoSalaValido(idSala))
                {
                    throw new FaultException(MensajesError.Cliente.CodigoSalaObligatorio);
                }

                if (!EntradaComunValidador.EsMensajeValido(mensaje))
                {
                    return;
                }

                var idSalaNormalizado = idSala.Trim();
                var nombreNormalizado = nombreJugador.Trim();
                var mensajeNormalizado = mensaje.Trim();

                NotificarMensajeATodos(
                    idSalaNormalizado,
                    nombreNormalizado,
                    mensajeNormalizado);
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorValidacionChat, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.DatosInvalidosChat, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorComunicacionChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorEnviarMensaje);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorTimeoutChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorEnviarMensaje);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCanalCerradoChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorEnviarMensaje);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorEnviarMensaje);
            }
        }

        /// <summary>
        /// Permite a un jugador salir del chat de una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugador">Nombre del jugador que sale.</param>
        public void SalirChatSala(string idSala, string nombreJugador)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(nombreJugador, nameof(nombreJugador));

                if (!EntradaComunValidador.EsCodigoSalaValido(idSala))
                {
                    throw new FaultException(MensajesError.Cliente.CodigoSalaObligatorio);
                }

                var idSalaNormalizado = idSala.Trim();
                var nombreNormalizado = nombreJugador.Trim();

                RemoverCliente(idSalaNormalizado, nombreNormalizado);
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorValidacionChat, excepcion);
                throw;
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.DatosInvalidosChat, excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorComunicacionChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorSalirChat);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorTimeoutChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorSalirChat);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorCanalCerradoChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorSalirChat);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoChat, excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorSalirChat);
            }
        }

        private List<ClienteChat> GestionarIngresoSala(
            string idSala,
            string nombreJugador,
            IChatManejadorCallback callback)
        {
            _almacenClientes.RegistrarOActualizarCliente(idSala, nombreJugador, callback);
            return _almacenClientes.ObtenerClientesExcluyendo(idSala, nombreJugador);
        }

        private void NotificarIngresoMasivo(
            string idSala,
            string nombreJugador,
            List<ClienteChat> destinatarios)
        {
            foreach (var cliente in destinatarios)
            {
                NotificarJugadorUnidoSeguro(idSala, cliente, nombreJugador);
            }
        }

        private void NotificarJugadorUnidoSeguro(
            string idSala,
            ClienteChat cliente,
            string nombreJugador)
        {
            try
            {
                cliente.Callback.NotificarJugadorUnido(nombreJugador);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorComunicacionChat, excepcion);
                RemoverClienteInalcanzable(idSala, cliente.NombreJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorTimeoutChat, excepcion);
                RemoverClienteInalcanzable(idSala, cliente.NombreJugador);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorCanalCerradoChat, excepcion);
                RemoverClienteInalcanzable(idSala, cliente.NombreJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorInesperadoChat, excepcion);
                RemoverClienteInalcanzable(idSala, cliente.NombreJugador);
            }
        }

        private void RemoverCliente(string idSala, string nombreJugador)
        {
            var clientesANotificar = _almacenClientes.ObtenerClientesExcluyendo(idSala, nombreJugador);
            _almacenClientes.RemoverCliente(idSala, nombreJugador);

            if (clientesANotificar != null && clientesANotificar.Count > 0)
            {
                NotificarSalidaMasiva(idSala, nombreJugador, clientesANotificar);
            }
        }

        private void NotificarSalidaMasiva(
            string idSala,
            string nombreJugador,
            List<ClienteChat> destinatarios)
        {
            foreach (var cliente in destinatarios)
            {
                NotificarJugadorSalioSeguro(idSala, cliente, nombreJugador);
            }
        }

        private void NotificarJugadorSalioSeguro(
            string idSala,
            ClienteChat cliente,
            string nombreJugador)
        {
            try
            {
                cliente.Callback.NotificarJugadorSalio(nombreJugador);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorComunicacionChat, excepcion);
                RemoverClienteInalcanzable(idSala, cliente.NombreJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorTimeoutChat, excepcion);
                RemoverClienteInalcanzable(idSala, cliente.NombreJugador);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorCanalCerradoChat, excepcion);
                RemoverClienteInalcanzable(idSala, cliente.NombreJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorInesperadoChat, excepcion);
                RemoverClienteInalcanzable(idSala, cliente.NombreJugador);
            }
        }

        private IChatManejadorCallback ObtenerCallbackActual()
        {
            if (!_proveedorContexto.ExisteContexto)
            {
                throw new FaultException(MensajesError.Cliente.ErrorContextoOperacion);
            }

            var callback = _proveedorContexto.ObtenerCallbackChannel<IChatManejadorCallback>();
            if (callback == null)
            {
                throw new FaultException(MensajesError.Cliente.ErrorObtenerCallback);
            }

            return callback;
        }

        private void ConfigurarEventosCanal(string idSala, string nombreJugador)
        {
            var canal = _proveedorContexto.ObtenerCanalActual();
            if (canal != null)
            {
                EventHandler manejadorClosed = null;
                EventHandler manejadorFaulted = null;

                manejadorClosed = delegate(object remitente, EventArgs argumentos)
                {
                    RemoverCliente(idSala, nombreJugador);
                };

                manejadorFaulted = delegate(object remitente, EventArgs argumentos)
                {
                    RemoverCliente(idSala, nombreJugador);
                };

                canal.Closed += manejadorClosed;
                canal.Faulted += manejadorFaulted;
            }
        }

        private void NotificarMensajeATodos(
            string idSala,
            string nombreJugador,
            string mensaje)
        {
            var clientesANotificar = _almacenClientes.ObtenerClientesSala(idSala);
            if (clientesANotificar == null)
            {
                return;
            }

            foreach (var cliente in clientesANotificar)
            {
                var parametrosMensaje = new NotificacionMensajeSeguroParametros
                {
                    IdSala = idSala,
                    Cliente = cliente,
                    NombreJugador = nombreJugador,
                    Mensaje = mensaje
                };
                NotificarMensajeSeguro(parametrosMensaje);
            }
        }

        private void NotificarMensajeSeguro(NotificacionMensajeSeguroParametros parametros)
        {
            try
            {
                parametros.Cliente.Callback.RecibirMensaje(
                    parametros.NombreJugador,
                    parametros.Mensaje);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorComunicacionChat, excepcion);
                RemoverClienteInalcanzable(parametros.IdSala, parametros.Cliente.NombreJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorTimeoutChat, excepcion);
                RemoverClienteInalcanzable(parametros.IdSala, parametros.Cliente.NombreJugador);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorCanalCerradoChat, excepcion);
                RemoverClienteInalcanzable(parametros.IdSala, parametros.Cliente.NombreJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorInesperadoChat, excepcion);
                RemoverClienteInalcanzable(parametros.IdSala, parametros.Cliente.NombreJugador);
            }
        }

        private void RemoverClienteInalcanzable(string idSala, string nombreJugador)
        {
            _almacenClientes.RemoverCliente(idSala, nombreJugador);

            _logger.Info("Eliminando sesion por cliente inalcanzable en chat.");
            SesionUsuarioManejador.Instancia.EliminarSesionPorNombre(nombreJugador);
        }
    }
}
