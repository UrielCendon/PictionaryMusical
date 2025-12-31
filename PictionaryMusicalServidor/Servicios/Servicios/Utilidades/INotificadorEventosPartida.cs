using System;
using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Interfaz para notificacion de eventos durante una partida.
    /// Centraliza la logica de notificacion a los jugadores conectados.
    /// </summary>
    internal interface INotificadorEventosPartida
    {
        /// <summary>
        /// Notifica a todos los jugadores que la partida ha iniciado.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        void NotificarPartidaIniciada(string idSala);

        /// <summary>
        /// Notifica a todos los jugadores que un jugador adivino correctamente.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugador">Nombre del jugador que adivino.</param>
        /// <param name="puntos">Puntos obtenidos.</param>
        void NotificarJugadorAdivino(string idSala, string nombreJugador, int puntos);

        /// <summary>
        /// Notifica a todos los jugadores un mensaje de chat.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="nombreJugador">Nombre del jugador que envio el mensaje.</param>
        /// <param name="mensaje">Contenido del mensaje.</param>
        void NotificarMensajeChat(string idSala, string nombreJugador, string mensaje);

        /// <summary>
        /// Notifica a todos los jugadores un trazo de dibujo.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="trazo">Datos del trazo.</param>
        void NotificarTrazoRecibido(string idSala, TrazoDTO trazo);

        /// <summary>
        /// Notifica a todos los jugadores que la ronda ha finalizado.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="tiempoAgotado">Indica si la ronda termino por tiempo agotado.</param>
        void NotificarFinRonda(string idSala, bool tiempoAgotado);

        /// <summary>
        /// Notifica a todos los jugadores que la partida ha finalizado.
        /// </summary>
        /// <param name="idSala">Identificador de la sala.</param>
        /// <param name="resultado">Resultado final de la partida.</param>
        void NotificarFinPartida(string idSala, ResultadoPartidaDTO resultado);

        /// <summary>
        /// Notifica el inicio de una ronda a un jugador especifico.
        /// </summary>
        /// <param name="parametros">Objeto con los datos necesarios para la notificacion.</param>
        void NotificarInicioRondaAJugador(NotificacionRondaParametros parametros);
    }
}
