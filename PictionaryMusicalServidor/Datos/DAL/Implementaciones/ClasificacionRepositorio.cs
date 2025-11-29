using System;
using System.Data.Entity;
using System.Linq;
using log4net;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using Datos.Modelo;

namespace PictionaryMusicalServidor.Datos.DAL.Implementaciones
{
    /// <summary>
    /// Repositorio encargado de gestionar la persistencia y actualizacion de las clasificaciones
    /// y estadisticas de juego de los usuarios.
    /// </summary>
    public class ClasificacionRepositorio : IClasificacionRepositorio
    {
        private static readonly ILog _logger = LogManager.
            GetLogger(typeof(ClasificacionRepositorio));
        private readonly BaseDatosPruebaEntities _contexto;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de clasificaciones.
        /// </summary>
        /// <param name="contexto">Contexto de la base de datos.</param>
        /// <exception cref="ArgumentNullException">Se lanza si el contexto es nulo.</exception>
        public ClasificacionRepositorio(BaseDatosPruebaEntities contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        /// <summary>
        /// Crea un registro de clasificacion inicial con contadores en cero para un nuevo jugador.
        /// </summary>
        /// <returns>La entidad de clasificacion creada.</returns>
        public Clasificacion CrearClasificacionInicial()
        {
            try
            {
                var clasificacion = new Clasificacion
                {
                    Puntos_Ganados = 0,
                    Rondas_Ganadas = 0
                };

                _contexto.Clasificacion.Add(clasificacion);
                _contexto.SaveChanges();

                return clasificacion;
            }
            catch (Exception ex)
            {
                _logger.Error("Error al crear la clasificación inicial.", ex);
                throw;
            }
        }

        /// <summary>
        /// Actualiza las estadisticas de puntos y partidas ganadas de un jugador especifico.
        /// </summary>
        /// <param name="jugadorId">Identificador del jugador a actualizar.</param>
        /// <param name="puntosObtenidos">Cantidad de puntos a sumar.</param>
        /// <param name="ganoPartida">Indica si el jugador gano la partida para incrementar el 
        /// contador.</param>
        /// <returns>True si la actualizacion fue exitosa, False si no se encontro la 
        /// clasificacion.</returns>
        public bool ActualizarEstadisticas(int jugadorId, int puntosObtenidos, bool ganoPartida)
        {
            try
            {
                var jugador = _contexto.Jugador
                    .Include(j => j.Clasificacion)
                    .FirstOrDefault(j => j.idJugador == jugadorId);

                if (jugador?.Clasificacion == null)
                {
                    _logger.WarnFormat("No se encontró clasificación para el jugador con ID {0}.",
                        jugadorId);
                    return false;
                }

                jugador.Clasificacion.Puntos_Ganados = (jugador.Clasificacion.Puntos_Ganados ?? 0)
                    + puntosObtenidos;

                if (ganoPartida)
                {
                    jugador.Clasificacion.Rondas_Ganadas = (jugador.Clasificacion.Rondas_Ganadas 
                        ?? 0) + 1;
                }

                _contexto.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(
                    "Error al actualizar la clasificación del jugador con ID {0}.", jugadorId, ex);
                throw;
            }
        }
    }
}