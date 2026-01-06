using System;
using System.Collections.Generic;
using System.Linq;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Implementacion del almacen de clientes de chat.
    /// Gestiona los clientes conectados por sala de manera thread-safe.
    /// </summary>
    public class AlmacenClientesChat : IAlmacenClientesChat
    {
        private readonly Dictionary<string, List<ClienteChat>> _clientesPorSala;
        private readonly object _sincronizacion = new object();

        /// <summary>
        /// Constructor que inicializa el almacen con un diccionario nuevo.
        /// </summary>
        public AlmacenClientesChat()
            : this(new Dictionary<string, List<ClienteChat>>(StringComparer.OrdinalIgnoreCase))
        {
        }

        /// <summary>
        /// Constructor con inyeccion de diccionario para pruebas.
        /// </summary>
        /// <param name="clientesPorSala">Diccionario de clientes por sala.</param>
        public AlmacenClientesChat(Dictionary<string, List<ClienteChat>> clientesPorSala)
        {
            _clientesPorSala = clientesPorSala 
                ?? new Dictionary<string, List<ClienteChat>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Obtiene la lista de clientes de una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <returns>Lista de clientes o null si la sala no existe.</returns>
        public List<ClienteChat> ObtenerClientesSala(string idSala)
        {
            lock (_sincronizacion)
            {
                List<ClienteChat> clientesSala;
                if (_clientesPorSala.TryGetValue(idSala, out clientesSala))
                {
                    return clientesSala.ToList();
                }

                return new List<ClienteChat>();
            }
        }

        /// <summary>
        /// Obtiene o crea la lista de clientes para una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <returns>Lista de clientes de la sala.</returns>
        public List<ClienteChat> ObtenerOCrearListaClientes(string idSala)
        {
            lock (_sincronizacion)
            {
                List<ClienteChat> clientesSala;
                if (!_clientesPorSala.TryGetValue(idSala, out clientesSala))
                {
                    clientesSala = new List<ClienteChat>();
                    _clientesPorSala[idSala] = clientesSala;
                }
                return clientesSala;
            }
        }

        /// <summary>
        /// Registra o actualiza un cliente en una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugador">Nombre del jugador.</param>
        /// <param name="callback">Callback del cliente.</param>
        public void RegistrarOActualizarCliente(
            string idSala,
            string nombreJugador,
            IChatManejadorCallback callback)
        {
            lock (_sincronizacion)
            {
                var clientesSala = ObtenerOCrearListaClientesInterno(idSala);
                var clienteExistente = clientesSala.FirstOrDefault(clienteActual =>
                    string.Equals(
                        clienteActual.NombreJugador,
                        nombreJugador,
                        StringComparison.OrdinalIgnoreCase));

                if (clienteExistente != null)
                {
                    clienteExistente.Callback = callback;
                }
                else
                {
                    clientesSala.Add(new ClienteChat(nombreJugador, callback));
                }
            }
        }

        /// <summary>
        /// Remueve un cliente de una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugador">Nombre del jugador a remover.</param>
        /// <returns>True si el cliente fue removido, false en caso contrario.</returns>
        public bool RemoverCliente(string idSala, string nombreJugador)
        {
            lock (_sincronizacion)
            {
                List<ClienteChat> clientesSala;
                if (!_clientesPorSala.TryGetValue(idSala, out clientesSala))
                {
                    return false;
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

                if (clientesSala.Count == 0)
                {
                    _clientesPorSala.Remove(idSala);
                }

                return cantidadRemovida > 0;
            }
        }

        /// <summary>
        /// Obtiene los clientes de una sala excluyendo a un jugador especifico.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugadorExcluir">Nombre del jugador a excluir.</param>
        /// <returns>Lista de clientes excluyendo al jugador especificado.</returns>
        public List<ClienteChat> ObtenerClientesExcluyendo(
            string idSala, 
            string nombreJugadorExcluir)
        {
            lock (_sincronizacion)
            {
                List<ClienteChat> clientesSala;
                if (!_clientesPorSala.TryGetValue(idSala, out clientesSala))
                {
                    return new List<ClienteChat>();
                }

                return clientesSala
                    .Where(clienteActual => !string.Equals(
                        clienteActual.NombreJugador,
                        nombreJugadorExcluir,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        /// <summary>
        /// Verifica si existe una sala con clientes.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <returns>True si la sala existe, false en caso contrario.</returns>
        public bool ExisteSala(string idSala)
        {
            lock (_sincronizacion)
            {
                return _clientesPorSala.ContainsKey(idSala);
            }
        }

        /// <summary>
        /// Elimina una sala si no tiene clientes.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        public void LimpiarSalaVacia(string idSala)
        {
            lock (_sincronizacion)
            {
                List<ClienteChat> clientesSala;
                bool existeSala = _clientesPorSala.TryGetValue(idSala, out clientesSala);
                if (existeSala && clientesSala.Count == 0)
                {
                    _clientesPorSala.Remove(idSala);
                }
            }
        }

        private List<ClienteChat> ObtenerOCrearListaClientesInterno(string idSala)
        {
            List<ClienteChat> clientesSala;
            if (!_clientesPorSala.TryGetValue(idSala, out clientesSala))
            {
                clientesSala = new List<ClienteChat>();
                _clientesPorSala[idSala] = clientesSala;
            }
            return clientesSala;
        }
    }
}
