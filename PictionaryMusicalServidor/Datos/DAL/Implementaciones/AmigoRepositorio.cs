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
    /// Repositorio para la gestion de relaciones de amistad, solicitudes y consultas de amigos 
    /// entre usuarios.
    /// </summary>
    public class AmigoRepositorio : IAmigoRepositorio
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AmigoRepositorio));
        private readonly BaseDatosPruebaEntities _contexto;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de amigos.
        /// </summary>
        /// <param name="contexto">Contexto de la base de datos.</param>
        /// <exception cref="ArgumentNullException">Se lanza si el contexto es nulo.</exception>
        public AmigoRepositorio(BaseDatosPruebaEntities contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        /// <summary>
        /// Verifica si existe una relacion (solicitud o amistad) entre dos usuarios.
        /// </summary>
        /// <param name="usuarioAId">ID del primer usuario.</param>
        /// <param name="usuarioBId">ID del segundo usuario.</param>
        /// <returns>True si existe registro en la tabla Amigo, False en caso contrario.</returns>
        public bool ExisteRelacion(int usuarioAId, int usuarioBId)
        {
            try
            {
                return _contexto.Amigo.Any(relacionAmistad =>
                    (relacionAmistad.UsuarioEmisor == usuarioAId 
                        && relacionAmistad.UsuarioReceptor == usuarioBId) ||
                    (relacionAmistad.UsuarioEmisor == usuarioBId 
                        && relacionAmistad.UsuarioReceptor == usuarioAId));
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesErrorDatos.Amigo.ErrorVerificarRelacion, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Amigo.ErrorVerificarRelacion, 
                    excepcion);
            }
        }

        /// <summary>
        /// Crea una nueva solicitud de amistad entre un emisor y un receptor.
        /// </summary>
        /// <param name="usuarioEmisorId">ID del usuario que envia la solicitud.</param>
        /// <param name="usuarioReceptorId">ID del usuario que recibe la solicitud.</param>
        /// <returns>La entidad Amigo creada.</returns>
        public Amigo CrearSolicitud(int usuarioEmisorId, int usuarioReceptorId)
        {
            var nuevaSolicitudAmistad = new Amigo
            {
                UsuarioEmisor = usuarioEmisorId,
                UsuarioReceptor = usuarioReceptorId,
                Estado = false
            };

            _contexto.Amigo.Add(nuevaSolicitudAmistad);
            _contexto.SaveChanges();

            return nuevaSolicitudAmistad;
        }

        /// <summary>
        /// Obtiene la entidad de relacion de amistad existente entre dos usuarios.
        /// </summary>
        /// <param name="usuarioAId">ID del primer usuario.</param>
        /// <param name="usuarioBId">ID del segundo usuario.</param>
        /// <returns>La entidad Amigo encontrada.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe relacion entre los 
        /// usuarios.</exception>
        public Amigo ObtenerRelacion(int usuarioAId, int usuarioBId)
        {
            try
            {
                var relacionEncontrada = _contexto.Amigo.FirstOrDefault(relacionAmistad =>
                    (relacionAmistad.UsuarioEmisor == usuarioAId 
                        && relacionAmistad.UsuarioReceptor == usuarioBId) ||
                    (relacionAmistad.UsuarioEmisor == usuarioBId 
                        && relacionAmistad.UsuarioReceptor == usuarioAId));

                if (relacionEncontrada == null)
                {
                    _logger.WarnFormat(
                        "No se encontro relacion de amistad entre usuarios con ids {0} y {1}.",
                        usuarioAId,
                        usuarioBId);
                    throw new KeyNotFoundException(
                        MensajesErrorDatos.Amigo.ErrorObtenerRelacion);
                }

                return relacionEncontrada;
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesErrorDatos.Amigo.ErrorObtenerRelacion, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Amigo.ErrorObtenerRelacion, 
                    excepcion);
            }
        }

        /// <summary>
        /// Obtiene una lista de las solicitudes de amistad pendientes donde el usuario esta 
        /// involucrado.
        /// </summary>
        /// <param name="usuarioId">ID del usuario a consultar.</param>
        /// <returns>Lista de entidades Amigo pendientes.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID de usuario es menor o 
        /// igual a cero.</exception>
        public IList<Amigo> ObtenerSolicitudesPendientes(int usuarioId)
        {
            if (usuarioId <= 0)
            {
                var excepcion = new ArgumentOutOfRangeException(
                    nameof(usuarioId), 
                    MensajesErrorDatos.Amigo.IdUsuarioInvalido);
                _logger.Error(
                    MensajesErrorDatos.Amigo.IntentarObtenerSolicitudesIdInvalido, 
                    excepcion);
                throw excepcion;
            }

            try
            {
                return _contexto.Amigo
                    .Where(solicitudAmistad => !solicitudAmistad.Estado 
                        && (solicitudAmistad.UsuarioEmisor == usuarioId
                            || solicitudAmistad.UsuarioReceptor == usuarioId))
                    .Include(solicitudAmistad => solicitudAmistad.Usuario)
                    .Include(solicitudAmistad => solicitudAmistad.Usuario1)
                    .ToList();
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Amigo.ErrorObtenerSolicitudesPendientes,
                    usuarioId);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Amigo.ErrorObtenerSolicitudesPendientes,
                    usuarioId);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
        }

        /// <summary>
        /// Actualiza el estado de aceptacion de una relacion de amistad.
        /// </summary>
        /// <param name="relacion">Entidad Amigo a modificar.</param>
        /// <param name="estado">Nuevo estado (True para aceptado).</param>
        /// <exception cref="ArgumentNullException">Se lanza si la relacion proporcionada es nula.
        /// </exception>
        public void ActualizarEstado(Amigo relacion, bool estado)
        {
            if (relacion == null)
            {
                var excepcion = new ArgumentNullException(nameof(relacion));
                _logger.Error(MensajesErrorDatos.Amigo.IntentarActualizarRelacionNula, excepcion);
                throw excepcion;
            }

            try
            {
                relacion.Estado = estado;
                _contexto.SaveChanges();
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Amigo.ErrorActualizarEstado,
                    relacion.UsuarioEmisor, 
                    relacion.UsuarioReceptor);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Amigo.ErrorActualizarEstado,
                    relacion.UsuarioEmisor, 
                    relacion.UsuarioReceptor);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
        }

        /// <summary>
        /// Elimina permanentemente un registro de relacion de amistad de la base de datos.
        /// </summary>
        /// <param name="relacion">Entidad Amigo a eliminar.</param>
        /// <exception cref="ArgumentNullException">Se lanza si la relacion proporcionada es nula.
        /// </exception>
        public void EliminarRelacion(Amigo relacion)
        {
            if (relacion == null)
            {
                var excepcion = new ArgumentNullException(nameof(relacion));
                _logger.Error(MensajesErrorDatos.Amigo.IntentarEliminarRelacionNula, excepcion);
                throw excepcion;
            }

            int identificadorEmisor = relacion.UsuarioEmisor;
            int identificadorReceptor = relacion.UsuarioReceptor;

            try
            {
                _contexto.Amigo.Remove(relacion);
                _contexto.SaveChanges();
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Amigo.ErrorEliminarRelacion,
                    identificadorEmisor, 
                    identificadorReceptor);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Amigo.ErrorEliminarRelacion,
                    identificadorEmisor, 
                    identificadorReceptor);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
        }

        /// <summary>
        /// Obtiene la lista de usuarios que son amigos confirmados del usuario especificado.
        /// </summary>
        /// <param name="usuarioId">ID del usuario a consultar.</param>
        /// <returns>Lista de entidades Usuario que son amigos.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el ID de usuario es menor o 
        /// igual a cero.</exception>
        public IList<Usuario> ObtenerAmigos(int usuarioId)
        {
            if (usuarioId <= 0)
            {
                var excepcion = new ArgumentOutOfRangeException(
                    nameof(usuarioId), 
                    MensajesErrorDatos.Amigo.IdUsuarioInvalido);
                _logger.Error(
                    MensajesErrorDatos.Amigo.IdUsuarioInvalidoObtenerAmigos, 
                    excepcion);
                throw excepcion;
            }

            try
            {
                var identificadoresAmigos = _contexto.Amigo
                    .Where(relacionAmistad => relacionAmistad.Estado 
                        && (relacionAmistad.UsuarioEmisor == usuarioId
                            || relacionAmistad.UsuarioReceptor == usuarioId))
                    .Select(relacionAmistad => relacionAmistad.UsuarioEmisor == usuarioId 
                        ? relacionAmistad.UsuarioReceptor
                        : relacionAmistad.UsuarioEmisor)
                    .Distinct()
                    .ToList();

                if (identificadoresAmigos.Count == 0)
                {
                    return new List<Usuario>();
                }

                return _contexto.Usuario
                    .Where(usuarioEntidad => 
                        identificadoresAmigos.Contains(usuarioEntidad.idUsuario))
                    .ToList();
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Amigo.ErrorObtenerAmigos,
                    usuarioId);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Amigo.ErrorObtenerAmigos,
                    usuarioId);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
        }
    }
}
