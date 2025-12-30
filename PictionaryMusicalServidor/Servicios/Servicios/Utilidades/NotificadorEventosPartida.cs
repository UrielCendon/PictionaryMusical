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
                var parametros = new CallbackEjecucionParametros
                {
                    Callback = par.Value,
                    IdJugador = par.Key,
                    IdSala = idSala,
                    Accion = cb => cb.NotificarPartidaIniciada()
                };
                _gestorCallbacks.EjecutarCallbackSeguro(parametros);
            }
        }

        /// <inheritdoc/>
        public void NotificarJugadorAdivino(string idSala, string nombreJugador, int puntos)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                var parametros = new CallbackEjecucionParametros
                {
                    Callback = par.Value,
                    IdJugador = par.Key,
                    IdSala = idSala,
                    Accion = cb => cb.NotificarJugadorAdivino(nombreJugador, puntos)
                };
                _gestorCallbacks.EjecutarCallbackSeguro(parametros);
            }
        }

        /// <inheritdoc/>
        public void NotificarMensajeChat(string idSala, string nombreJugador, string mensaje)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                var parametros = new CallbackEjecucionParametros
                {
                    Callback = par.Value,
                    IdJugador = par.Key,
                    IdSala = idSala,
                    Accion = cb => cb.NotificarMensajeChat(nombreJugador, mensaje)
                };
                _gestorCallbacks.EjecutarCallbackSeguro(parametros);
            }
        }

        /// <inheritdoc/>
        public void NotificarTrazoRecibido(string idSala, TrazoDTO trazo)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                var parametros = new CallbackEjecucionParametros
                {
                    Callback = par.Value,
                    IdJugador = par.Key,
                    IdSala = idSala,
                    Accion = cb => cb.NotificarTrazoRecibido(trazo)
                };
                _gestorCallbacks.EjecutarCallbackSeguro(parametros);
            }
        }

        /// <inheritdoc/>
        public void NotificarFinRonda(string idSala)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                var parametros = new CallbackEjecucionParametros
                {
                    Callback = par.Value,
                    IdJugador = par.Key,
                    IdSala = idSala,
                    Accion = cb => cb.NotificarFinRonda()
                };
                _gestorCallbacks.EjecutarCallbackSeguro(parametros);
            }
        }

        /// <inheritdoc/>
        public void NotificarFinPartida(string idSala, ResultadoPartidaDTO resultado)
        {
            var callbacks = _gestorCallbacks.ObtenerCallbacksSala(idSala);
            foreach (var par in callbacks)
            {
                var parametros = new CallbackEjecucionParametros
                {
                    Callback = par.Value,
                    IdJugador = par.Key,
                    IdSala = idSala,
                    Accion = cb => cb.NotificarFinPartida(resultado)
                };
                _gestorCallbacks.EjecutarCallbackSeguro(parametros);
            }
        }

        /// <inheritdoc/>
        public void NotificarInicioRondaAJugador(NotificacionRondaParametros parametros)
        {
            var callbackParams = new CallbackEjecucionParametros
            {
                Callback = parametros.Callback,
                IdJugador = parametros.IdJugador,
                IdSala = parametros.IdSala,
                Accion = cb => cb.NotificarInicioRonda(parametros.Ronda)
            };
            _gestorCallbacks.EjecutarCallbackSeguro(callbackParams);
        }
    }
}
