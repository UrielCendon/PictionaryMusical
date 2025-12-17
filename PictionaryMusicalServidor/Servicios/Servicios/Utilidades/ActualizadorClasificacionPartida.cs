using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using log4net;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Interfaz para la actualizacion de clasificaciones de partida.
    /// </summary>
    internal interface IActualizadorClasificacionPartida
    {
        /// <summary>
        /// Actualiza las clasificaciones de los jugadores al finalizar una partida.
        /// </summary>
        /// <param name="controlador">Controlador de la partida finalizada.</param>
        /// <param name="resultado">Resultado de la partida.</param>
        void ActualizarClasificacion(ControladorPartida controlador, ResultadoPartidaDTO resultado);
    }

    /// <summary>
    /// Implementacion del actualizador de clasificaciones de partida.
    /// Se encarga de persistir las estadisticas de los jugadores al finalizar una partida.
    /// </summary>
    internal sealed class ActualizadorClasificacionPartida : IActualizadorClasificacionPartida
    {
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(ActualizadorClasificacionPartida));

        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;

        /// <summary>
        /// Inicializa una nueva instancia del actualizador de clasificaciones.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        public ActualizadorClasificacionPartida(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria)
        {
            _contextoFactoria = contextoFactoria ?? 
                throw new ArgumentNullException(nameof(contextoFactoria));
            _repositorioFactoria = repositorioFactoria ?? 
                throw new ArgumentNullException(nameof(repositorioFactoria));
        }

        /// <inheritdoc/>
        public void ActualizarClasificacion(
            ControladorPartida controlador, 
            ResultadoPartidaDTO resultado)
        {
            if (!EsActualizacionValida(controlador, resultado))
            {
                return;
            }

            var jugadoresFinales = ObtenerJugadoresFinales(controlador);
            if (jugadoresFinales.Count == 0)
            {
                return;
            }

            var ganadores = CalcularGanadores(jugadoresFinales);
            PersistirEstadisticas(jugadoresFinales, ganadores);
        }

        private static bool EsActualizacionValida(
            ControladorPartida controlador, 
            ResultadoPartidaDTO resultado)
        {
            if (controlador == null)
            {
                return false;
            }

            if (resultado?.Clasificacion == null || !resultado.Clasificacion.Any())
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
            {
                _logger.Info("Partida finalizada sin clasificacion por mensaje de error.");
                return false;
            }

            return true;
        }

        private static List<JugadorPartida> ObtenerJugadoresFinales(ControladorPartida controlador)
        {
            try
            {
                var jugadores = controlador.ObtenerJugadores();
                return jugadores != null 
                    ? new List<JugadorPartida>(jugadores) 
                    : new List<JugadorPartida>();
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorObtenerJugadoresClasificacion, excepcion);
                return new List<JugadorPartida>();
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorObtenerJugadoresClasificacion, excepcion);
                return new List<JugadorPartida>();
            }
            catch (DataException excepcion)
            {
                _logger.Error("Error de datos al obtener jugadores para clasificacion.", excepcion);
                return new List<JugadorPartida>();
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Error inesperado al obtener jugadores para clasificacion.", 
                    excepcion);
                return new List<JugadorPartida>();
            }
        }

        private static HashSet<string> CalcularGanadores(List<JugadorPartida> jugadores)
        {
            int puntajeMaximo = jugadores.Max(j => j.PuntajeTotal);

            return new HashSet<string>(
                jugadores
                    .Where(j => j.PuntajeTotal == puntajeMaximo)
                    .Select(j => j.IdConexion),
                StringComparer.OrdinalIgnoreCase);
        }

        private void PersistirEstadisticas(
            List<JugadorPartida> jugadores, 
            HashSet<string> ganadores)
        {
            try
            {
                using (var contexto = _contextoFactoria.CrearContexto())
                {
                    var clasificacionRepositorio = 
                        _repositorioFactoria.CrearClasificacionRepositorio(contexto);

                    foreach (var jugador in jugadores)
                    {
                        PersistirEstadisticasJugador(clasificacionRepositorio, jugador, ganadores);
                    }
                }
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorActualizarClasificaciones, excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorActualizarClasificaciones, excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error("Error de datos al actualizar clasificaciones de partida.", excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Error inesperado al actualizar clasificaciones de partida.", 
                    excepcion);
            }
        }

        private static void PersistirEstadisticasJugador(
            IClasificacionRepositorio repositorio,
            JugadorPartida jugador,
            HashSet<string> ganadores)
        {
            int jugadorId;
            if (!int.TryParse(jugador.IdConexion, out jugadorId) || jugadorId <= 0)
            {
                return;
            }

            bool ganoPartida = ganadores.Contains(jugador.IdConexion);

            try
            {
                repositorio.ActualizarEstadisticas(jugadorId, jugador.PuntajeTotal, ganoPartida);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesError.Log.ErrorActualizarClasificacionJugador, 
                    jugadorId, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesError.Log.ErrorActualizarClasificacionJugador, 
                    jugadorId, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.ErrorFormat(
                    "Error de datos al actualizar clasificacion del jugador id {0}.", 
                    jugadorId, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    "Error inesperado al actualizar clasificacion del jugador id {0}.", 
                    jugadorId, 
                    excepcion);
            }
        }
    }
}
