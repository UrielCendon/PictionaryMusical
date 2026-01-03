using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Implementacion del servicio de chat entre jugadores.
    /// Gestiona la comunicacion en tiempo real entre jugadores conectados a una sala.
    /// Utiliza un diccionario estatico para administrar los clientes conectados por sala.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChatManejador : IChatManejador
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ChatManejador));

        private static readonly Dictionary<string, List<ClienteChat>> _clientesPorSala =
            new Dictionary<string, List<ClienteChat>>(StringComparer.OrdinalIgnoreCase);

        private static readonly object _sincronizacion = new object();

        /// <summary>
        /// Constructor por defecto para WCF.
        /// </summary>
        public ChatManejador()
        {
        }

        /// <summary>
        /// Permite a un jugador unirse al chat de una sala especifica.
        /// Registra el callback del cliente y notifica a los demas participantes que alguien entro.
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
        /// Distribuye el mensaje invocando el callback RecibirMensaje de cada cliente conectado.
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
        /// Elimina el callback y notifica a los demas participantes que el jugador salio.
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

        private static List<ClienteChat> GestionarIngresoSala(
            string idSala,
            string nombreJugador,
            IChatManejadorCallback callback)
        {
            lock (_sincronizacion)
            {
                var clientesSala = ObtenerOCrearListaClientes(idSala);
                RegistrarOActualizarCliente(clientesSala, nombreJugador, callback);
                return ObtenerClientesExcluyendo(clientesSala, nombreJugador);
            }
        }

        private static List<ClienteChat> ObtenerOCrearListaClientes(string idSala)
        {
            List<ClienteChat> clientesSala;
            if (!_clientesPorSala.TryGetValue(idSala, out clientesSala))
            {
                clientesSala = new List<ClienteChat>();
                _clientesPorSala[idSala] = clientesSala;
            }
            return clientesSala;
        }

        private static void RegistrarOActualizarCliente(
            List<ClienteChat> clientesSala,
            string nombreJugador,
            IChatManejadorCallback callback)
        {
            var clienteExistente = clientesSala.FirstOrDefault(c =>
                string.Equals(c.NombreJugador, nombreJugador, StringComparison.OrdinalIgnoreCase));

            if (clienteExistente != null)
            {
                clienteExistente.Callback = callback;
            }
            else
            {
                clientesSala.Add(new ClienteChat(nombreJugador, callback));
            }
        }

        private static List<ClienteChat> ObtenerClientesExcluyendo(
            List<ClienteChat> clientesSala,
            string nombreJugador)
        {
            return clientesSala
                .Where(c => !string.Equals(
                    c.NombreJugador, 
                    nombreJugador, 
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private static void NotificarIngresoMasivo(
            string idSala,
            string nombreJugador,
            List<ClienteChat> destinatarios)
        {
            foreach (var cliente in destinatarios)
            {
                NotificarJugadorUnidoSeguro(idSala, cliente, nombreJugador);
            }
        }

        private static void NotificarJugadorUnidoSeguro(
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
                _logger.Warn("Error de comunicacion con cliente. Removiendo callback.", excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn("Timeout al notificar cliente. Removiendo callback.", excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(
                    "Operacion invalida en canal WCF. Removiendo callback.",
                    excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    "Error inesperado al notificar cliente. Removiendo callback.",
                    excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
        }

        private static void RemoverCliente(string idSala, string nombreJugador)
        {
            var clientesANotificar = ProcesarSalidaSala(idSala, nombreJugador);

            if (clientesANotificar != null && clientesANotificar.Count > 0)
            {
                NotificarSalidaMasiva(idSala, nombreJugador, clientesANotificar);
            }
        }

        private static List<ClienteChat> ProcesarSalidaSala(string idSala, string nombreJugador)
        {
            lock (_sincronizacion)
            {
                List<ClienteChat> clientesSala;
                if (!_clientesPorSala.TryGetValue(idSala, out clientesSala))
                {
                    return new List<ClienteChat>();
                }

                int cantidadRemovida = 0;
                for (int i = clientesSala.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(
                        clientesSala[i].NombreJugador,
                        nombreJugador,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        clientesSala.RemoveAt(i);
                        cantidadRemovida++;
                    }
                }

                bool clienteRemovido = cantidadRemovida > 0;

                if (!clienteRemovido)
                {
                    return new List<ClienteChat>();
                }

                var remanentes = clientesSala.ToList();

                if (clientesSala.Count == 0)
                {
                    _clientesPorSala.Remove(idSala);
                }

                return remanentes;
            }
        }

        private static void NotificarSalidaMasiva(
            string idSala,
            string nombreJugador,
            List<ClienteChat> destinatarios)
        {
            foreach (var cliente in destinatarios)
            {
                NotificarJugadorSalioSeguro(idSala, cliente, nombreJugador);
            }
        }

        private static void NotificarJugadorSalioSeguro(
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
                _logger.Warn("Error de comunicacion con cliente. Removiendo callback.", excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn("Timeout al notificar cliente. Removiendo callback.", excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(
                    "Operacion invalida en canal WCF. Removiendo callback.",
                    excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    "Error inesperado al notificar cliente. Removiendo callback.",
                    excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
        }

        private static IChatManejadorCallback ObtenerCallbackActual()
        {
            var contexto = OperationContext.Current;
            if (contexto == null)
            {
                throw new FaultException(MensajesError.Cliente.ErrorContextoOperacion);
            }

            var callback = contexto.GetCallbackChannel<IChatManejadorCallback>();
            if (callback == null)
            {
                throw new FaultException(MensajesError.Cliente.ErrorObtenerCallback);
            }

            return callback;
        }

        private static void ConfigurarEventosCanal(string idSala, string nombreJugador)
        {
            var canal = OperationContext.Current?.Channel;
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

        private static void NotificarMensajeATodos(
            string idSala,
            string nombreJugador,
            string mensaje)
        {
            List<ClienteChat> clientesANotificar;

            lock (_sincronizacion)
            {
                List<ClienteChat> clientesSala;
                if (!_clientesPorSala.TryGetValue(idSala, out clientesSala))
                {
                    return;
                }

                clientesANotificar = clientesSala.ToList();
            }

            foreach (var cliente in clientesANotificar)
            {
                NotificarMensajeSeguro(idSala, cliente, nombreJugador, mensaje);
            }
        }

        private static void NotificarMensajeSeguro(
            string idSala,
            ClienteChat cliente,
            string nombreJugador,
            string mensaje)
        {
            try
            {
                cliente.Callback.RecibirMensaje(nombreJugador, mensaje);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn("Error de comunicacion con cliente. Removiendo callback.", excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn("Timeout al notificar cliente. Removiendo callback.", excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(
                    "Operacion invalida en canal WCF. Removiendo callback.",
                    excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    "Error inesperado al notificar cliente. Removiendo callback.",
                    excepcion);
                RemoverClienteSinNotificar(idSala, cliente.NombreJugador);
            }
        }

        private static void RemoverClienteSinNotificar(string idSala, string nombreJugador)
        {
            lock (_sincronizacion)
            {
                List<ClienteChat> clientesSala;
                if (_clientesPorSala.TryGetValue(idSala, out clientesSala))
                {
                    for (int i = clientesSala.Count - 1; i >= 0; i--)
                    {
                        if (string.Equals(
                            clientesSala[i].NombreJugador,
                            nombreJugador,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            clientesSala.RemoveAt(i);
                        }
                    }

                    if (clientesSala.Count == 0)
                    {
                        _clientesPorSala.Remove(idSala);
                    }
                }
            }
        }
    }
}
