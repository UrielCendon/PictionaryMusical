using System;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Interfaz para la gestion de callbacks de partida.
    /// Permite registrar, remover y verificar el estado de los callbacks de los jugadores.
    /// </summary>
    internal interface IGestorCallbacksPartida
    {
        /// <summary>
        /// Registra un callback para un jugador en una sala especifica.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idJugador">Identificador del jugador.</param>
        /// <param name="callback">Callback del jugador.</param>
        void RegistrarCallback(string idSala, string idJugador, 
            ICursoPartidaManejadorCallback callback);

        /// <summary>
        /// Remueve el callback de un jugador de una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idJugador">Identificador del jugador.</param>
        void RemoverCallback(string idSala, string idJugador);

        /// <summary>
        /// Obtiene el callback de un jugador en una sala.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="idJugador">Identificador del jugador.</param>
        /// <returns>Callback del jugador o null si no existe.</returns>
        ICursoPartidaManejadorCallback ObtenerCallback(string idSala, string idJugador);

        /// <summary>
        /// Verifica si el canal de un callback esta activo.
        /// </summary>
        /// <param name="callback">Callback a verificar.</param>
        /// <returns>True si el canal esta activo.</returns>
        bool EsCanalActivo(ICursoPartidaManejadorCallback callback);

        /// <summary>
        /// Ejecuta una accion de forma segura sobre un callback.
        /// </summary>
        /// <param name="callback">Callback sobre el que ejecutar.</param>
        /// <param name="idJugador">Identificador del jugador.</param>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="accion">Accion a ejecutar.</param>
        void EjecutarCallbackSeguro(
            ICursoPartidaManejadorCallback callback,
            string idJugador,
            string idSala,
            Action<ICursoPartidaManejadorCallback> accion);

        /// <summary>
        /// Obtiene todos los callbacks de una sala como pares clave-valor.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <returns>Lista de pares (idJugador, callback).</returns>
        System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, 
            ICursoPartidaManejadorCallback>> ObtenerCallbacksSala(string idSala);
    }
}
