using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Interfaz para el servicio de actualizacion de clasificaciones al finalizar una partida.
    /// </summary>
    public interface IActualizadorClasificacionPartida
    {
        /// <summary>
        /// Actualiza las clasificaciones de los jugadores basandose en el resultado de la partida.
        /// </summary>
        /// <param name="jugadores">Lista de jugadores de la partida.</param>
        /// <param name="resultado">Resultado de la partida.</param>
        void ActualizarClasificaciones(
            IList<JugadorPartida> jugadores,
            ResultadoPartidaDTO resultado);
    }
}
