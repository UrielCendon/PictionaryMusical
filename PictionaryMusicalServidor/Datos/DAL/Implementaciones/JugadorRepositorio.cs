using System;
using System.Data;
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
    /// Repositorio encargado de las operaciones CRUD y validaciones relacionadas con la entidad 
    /// Jugador.
    /// </summary>
    public class JugadorRepositorio : IJugadorRepositorio
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(JugadorRepositorio));
        private readonly BaseDatosPruebaEntities _contexto;
        
        /// <summary>
        /// Inicializa una nueva instancia del repositorio de jugadores.
        /// </summary>
        /// <param name="contexto">Contexto de la base de datos.</param>
        /// <exception cref="ArgumentNullException">Se lanza si el contexto es nulo.</exception>
        public JugadorRepositorio(BaseDatosPruebaEntities contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        /// <summary>
        /// Verifica si existe algun jugador registrado con el correo electronico proporcionado.
        /// </summary>
        /// <param name="correo">Correo electronico a verificar.</param>
        /// <returns>True si el correo ya existe, False en caso contrario.</returns>
        public bool ExisteCorreo(string correo)
        {
            try
            {
                return _contexto.Jugador.Any(
                    jugadorEntidad => jugadorEntidad.Correo == correo);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Jugador.ErrorVerificarExistenciaCorreo, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Jugador.ErrorVerificarExistenciaCorreo, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Jugador.ErrorVerificarExistenciaCorreo, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Jugador.ErrorVerificarExistenciaCorreo, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Jugador.ErrorVerificarExistenciaCorreo, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Jugador.ErrorVerificarExistenciaCorreo, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Jugador.ErrorVerificarExistenciaCorreo, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Jugador.ErrorVerificarExistenciaCorreo, 
                    excepcion);
            }
        }

        /// <summary>
        /// Crea un nuevo jugador en la base de datos.
        /// </summary>
        /// <param name="jugador">Entidad Jugador a persistir.</param>
        /// <returns>La entidad Jugador creada y persistida.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el objeto jugador es nulo.
        /// </exception>
        public Jugador CrearJugador(Jugador jugador)
        {
            if (jugador == null)
            {
                var excepcion = new ArgumentNullException(nameof(jugador));
                _logger.Error(MensajesErrorDatos.Jugador.IntentarCrearJugadorNulo, excepcion);
                throw excepcion;
            }

            try
            {
                var jugadorCreado = _contexto.Jugador.Add(jugador);
                _contexto.SaveChanges();

                return jugadorCreado;
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Jugador.ErrorGuardarJugador, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Jugador.ErrorGuardarJugador, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Jugador.ErrorGuardarJugador, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Jugador.ErrorGuardarJugador, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Jugador.ErrorGuardarJugador, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Jugador.ErrorGuardarJugador, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Jugador.ErrorGuardarJugador, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Jugador.ErrorGuardarJugador, 
                    excepcion);
            }
        }
    }
}
