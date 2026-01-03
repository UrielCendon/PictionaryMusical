using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using log4net;
using PictionaryMusicalServidor.Datos.Constantes;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Datos.Excepciones;
using Datos.Modelo;

namespace PictionaryMusicalServidor.Datos.DAL.Implementaciones
{
    /// <summary>
    /// Repositorio encargado de gestionar la persistencia y actualizacion de las clasificaciones
    /// y estadisticas de juego de los usuarios.
    /// </summary>
    public class ClasificacionRepositorio : IClasificacionRepositorio
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(ClasificacionRepositorio));

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
        public Clasificacion CrearClasificacionInicial()
        {
            try
            {
                var nuevaClasificacion = new Clasificacion
                {
                    Puntos_Ganados = 0,
                    Rondas_Ganadas = 0
                };

                _contexto.Clasificacion.Add(nuevaClasificacion);
                _contexto.SaveChanges();

                return nuevaClasificacion;
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Clasificacion.ErrorCrearInicial, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorCrearInicial, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Clasificacion.ErrorCrearInicial, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorCrearInicial, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Clasificacion.ErrorCrearInicial, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorCrearInicial, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesErrorDatos.Clasificacion.ErrorCrearInicial, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorCrearInicial, 
                    excepcion);
            }
        }

        /// <summary>
        /// Actualiza las estadisticas de puntos y partidas ganadas de un jugador especifico.
        /// </summary>
        public bool ActualizarEstadisticas(
            int jugadorId,
            int puntosObtenidos,
            bool ganoPartida)
        {
            try
            {
                var jugadorEncontrado = _contexto.Jugador
                    .Include(jugadorEntidad => jugadorEntidad.Clasificacion)
                    .FirstOrDefault(jugadorEntidad => jugadorEntidad.idJugador == jugadorId);

                if (jugadorEncontrado?.Clasificacion == null)
                {
                    _logger.WarnFormat(
                        MensajesErrorDatos.Clasificacion.ClasificacionNoEncontrada,
                        jugadorId);
                    return false;
                }

                jugadorEncontrado.Clasificacion.Puntos_Ganados =
                    (jugadorEncontrado.Clasificacion.Puntos_Ganados ?? 0) + puntosObtenidos;

                if (ganoPartida)
                {
                    jugadorEncontrado.Clasificacion.Rondas_Ganadas =
                        (jugadorEncontrado.Clasificacion.Rondas_Ganadas ?? 0) + 1;
                }

                _contexto.SaveChanges();
                return true;
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorActualizarClasificacion, 
                    excepcion);
            }
        }

        /// <summary>
        /// Obtiene la lista de usuarios con sus clasificaciones ordenadas por puntuacion.
        /// </summary>
        public IList<Usuario> ObtenerMejoresJugadores(int cantidad)
        {
            try
            {
                return _contexto.Usuario
                    .Include(usuario => usuario.Jugador.Clasificacion)
                    .Where(usuario => usuario.Jugador != null 
                        && usuario.Jugador.Clasificacion != null)
                    .OrderByDescending(usuario => usuario.Jugador.Clasificacion.Puntos_Ganados)
                    .ThenByDescending(usuario => usuario.Jugador.Clasificacion.Rondas_Ganadas)
                    .ThenBy(usuario => usuario.Nombre_Usuario)
                    .Take(cantidad)
                    .ToList();
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Clasificacion.ErrorConsultarMejores, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorConsultarMejores, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Clasificacion.ErrorConsultarMejores, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorConsultarMejores, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Clasificacion.ErrorConsultarMejores, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorConsultarMejores, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesErrorDatos.Clasificacion.ErrorConsultarMejores, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Clasificacion.ErrorConsultarMejores, 
                    excepcion);
            }
        }
    }
}
