using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Interfaz para el almacenamiento de clientes de chat por sala.
    /// Permite abstraer el almacenamiento para facilitar pruebas unitarias.
    /// </summary>
    public interface IAlmacenClientesChat
    {
        /// <summary>
        /// Obtiene la lista de clientes de una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <returns>Lista de clientes o null si la sala no existe.</returns>
        List<ClienteChat> ObtenerClientesSala(string idSala);

        /// <summary>
        /// Obtiene o crea la lista de clientes para una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <returns>Lista de clientes de la sala.</returns>
        List<ClienteChat> ObtenerOCrearListaClientes(string idSala);

        /// <summary>
        /// Registra o actualiza un cliente en una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugador">Nombre del jugador.</param>
        /// <param name="callback">Callback del cliente.</param>
        void RegistrarOActualizarCliente(
            string idSala,
            string nombreJugador,
            IChatManejadorCallback callback);

        /// <summary>
        /// Remueve un cliente de una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugador">Nombre del jugador a remover.</param>
        /// <returns>True si el cliente fue removido, false en caso contrario.</returns>
        bool RemoverCliente(string idSala, string nombreJugador);

        /// <summary>
        /// Obtiene los clientes de una sala excluyendo a un jugador especifico.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugadorExcluir">Nombre del jugador a excluir.</param>
        /// <returns>Lista de clientes excluyendo al jugador especificado.</returns>
        List<ClienteChat> ObtenerClientesExcluyendo(string idSala, string nombreJugadorExcluir);

        /// <summary>
        /// Verifica si existe una sala con clientes.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <returns>True si la sala existe, false en caso contrario.</returns>
        bool ExisteSala(string idSala);

        /// <summary>
        /// Elimina una sala si no tiene clientes.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        void LimpiarSalaVacia(string idSala);
    }
}
