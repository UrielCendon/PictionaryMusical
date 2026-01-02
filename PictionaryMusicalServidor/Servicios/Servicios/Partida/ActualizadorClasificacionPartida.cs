using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using log4net;
using PictionaryMusicalServidor.Datos.Constantes;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Partida
{
    /// <summary>
    /// Servicio encargado de actualizar las clasificaciones de los jugadores 
    /// al finalizar una partida.
    /// </summary>
    public class ActualizadorClasificacionPartida : IActualizadorClasificacionPartida
    {
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(ActualizadorClasificacionPartida));

        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;

        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public ActualizadorClasificacionPartida()
            : this(new ContextoFactoria(), new RepositorioFactoria())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        public ActualizadorClasificacionPartida(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria)
        {
            _contextoFactoria = contextoFactoria
                ?? throw new ArgumentNullException(nameof(contextoFactoria));
            _repositorioFactoria = repositorioFactoria
                ?? throw new ArgumentNullException(nameof(repositorioFactoria));
        }

        /// <summary>
        /// Actualiza las clasificaciones de los jugadores basandose en el resultado de la partida.
        /// </summary>
        /// <param name="jugadores">Lista de jugadores de la partida.</param>
        /// <param name="resultado">Resultado de la partida.</param>
        public void ActualizarClasificaciones(
            IList<JugadorPartida> jugadores,
            ResultadoPartidaDTO resultado)
        {
            if (jugadores == null || jugadores.Count == 0)
            {
                return;
            }

            if (resultado?.Clasificacion == null || !resultado.Clasificacion.Any())
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
            {
                _logger.Info(MensajesError.Log.PartidaFinalizadaSinClasificacion);
                return;
            }

            var ganadores = CalcularGanadores(jugadores);

            try
            {
                PersistirClasificaciones(jugadores, ganadores);
            }
            catch (Datos.Excepciones.BaseDatosExcepcion excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorActualizarClasificaciones, excepcion);
                resultado.Mensaje = MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion;
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorActualizarClasificaciones, excepcion);
                resultado.Mensaje = MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion;
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorActualizarClasificaciones, excepcion);
                resultado.Mensaje = MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion;
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorDatosActualizarClasificaciones, excepcion);
                resultado.Mensaje = MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion;
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Log.ErrorInesperadoActualizarClasificaciones, 
                    excepcion);
                resultado.Mensaje = MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion;
            }
        }

        private static HashSet<string> CalcularGanadores(IList<JugadorPartida> jugadores)
        {
            int puntajeMaximo = jugadores.Max(j => j.PuntajeTotal);

            return new HashSet<string>(
                jugadores
                    .Where(j => j.PuntajeTotal == puntajeMaximo)
                    .Select(j => j.IdConexion),
                StringComparer.OrdinalIgnoreCase);
        }

        private void PersistirClasificaciones(
            IList<JugadorPartida> jugadores,
            HashSet<string> ganadores)
        {
            using (var contexto = _contextoFactoria.CrearContexto())
            {
                var clasificacionRepositorio =
                    _repositorioFactoria.CrearClasificacionRepositorio(contexto);

                foreach (var jugador in jugadores)
                {
                    PersistirEstadisticasJugador(
                        clasificacionRepositorio,
                        jugador,
                        ganadores);
                }
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

            repositorio.ActualizarEstadisticas(
                jugadorId,
                jugador.PuntajeTotal,
                ganoPartida);
        }
    }
}
