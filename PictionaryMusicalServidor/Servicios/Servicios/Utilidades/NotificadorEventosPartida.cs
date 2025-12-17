using System;
using System.Collections.Generic;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Implementacion del notificador de eventos de partida.
    /// Maneja la distribucion de notificaciones a los jugadores de una partida.
    /// </summary>
    internal sealed class NotificadorEventosPartida : INotificadorEventosPartida
    {
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(NotificadorEventosPartida));

        private readonly IGestorCallbacksPartida _gestorCallbacks;

        /// <summary>
        /// Inicializa una nueva instancia del notificador de eventos.
        /// </summary>
        /// <param name="gestorCallbacks">Gestor de callbacks de partida.</param>
        public NotificadorEventosPartida(IGestorCallbacksPartida gestorCallbacks)
        {
            _gestorCallbacks = gestorCallbacks ?? 
                throw new ArgumentNullException(nameof(gestorCallbacks));
        }

        /// <inheritdoc/>
        public void NotificarPartidaIniciada(string idSala)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                _gestorCallbacks.EjecutarCallbackSeguro(
                    par.Value, 
                    par.Key, 
                    idSala,
                    cb => cb.NotificarPartidaIniciada());
            }
        }

        /// <inheritdoc/>
        public void NotificarJugadorAdivino(string idSala, string nombreJugador, int puntos)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                _gestorCallbacks.EjecutarCallbackSeguro(
                    par.Value, 
                    par.Key, 
                    idSala,
                    cb => cb.NotificarJugadorAdivino(nombreJugador, puntos));
            }
        }

        /// <inheritdoc/>
        public void NotificarMensajeChat(string idSala, string nombreJugador, string mensaje)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                _gestorCallbacks.EjecutarCallbackSeguro(
                    par.Value, 
                    par.Key, 
                    idSala,
                    cb => cb.NotificarMensajeChat(nombreJugador, mensaje));
            }
        }

        /// <inheritdoc/>
        public void NotificarTrazoRecibido(string idSala, TrazoDTO trazo)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                _gestorCallbacks.EjecutarCallbackSeguro(
                    par.Value, 
                    par.Key, 
                    idSala,
                    cb => cb.NotificarTrazoRecibido(trazo));
            }
        }

        /// <inheritdoc/>
        public void NotificarFinRonda(string idSala)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                _gestorCallbacks.EjecutarCallbackSeguro(
                    par.Value, 
                    par.Key, 
                    idSala,
                    cb => cb.NotificarFinRonda());
            }
        }

        /// <inheritdoc/>
        public void NotificarFinPartida(string idSala, ResultadoPartidaDTO resultado)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                _gestorCallbacks.EjecutarCallbackSeguro(
                    par.Value, 
                    par.Key, 
                    idSala,
                    cb => cb.NotificarFinPartida(resultado));
            }
        }

        /// <inheritdoc/>
        public void NotificarInicioRondaAJugador(
            ICursoPartidaManejadorCallback callback,
            RondaDTO ronda,
            string idSala,
            string idJugador)
        {
            _gestorCallbacks.EjecutarCallbackSeguro(
                callback, 
                idJugador, 
                idSala,
                cb => cb.NotificarInicioRonda(ronda));
        }
    }
}
