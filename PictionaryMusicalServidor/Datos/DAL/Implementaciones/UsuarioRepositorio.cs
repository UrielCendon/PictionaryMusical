using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using PictionaryMusicalServidor.Datos.Constantes;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Datos.Excepciones;
using Datos.Modelo;

namespace PictionaryMusicalServidor.Datos.DAL.Implementaciones
{
    /// <summary>
    /// Implementacion del repositorio de usuarios para acceso a datos.
    /// Proporciona operaciones de consulta y creacion de usuarios con validaciones y 
    /// comparaciones exactas.
    /// Verifica que el nombre de usuario no este duplicado usando comparacion case-sensitive.
    /// </summary>
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(UsuarioRepositorio));
        private readonly BaseDatosPruebaEntities _contexto;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de usuarios.
        /// </summary>
        /// <param name="contexto">Contexto de base de datos.</param>
        /// <exception cref="ArgumentNullException">Se lanza si contexto es null.</exception>
        public UsuarioRepositorio(BaseDatosPruebaEntities contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        /// <summary>
        /// Verifica si existe un usuario con el nombre de usuario especificado.
        /// Usa comparacion exacta (case-sensitive) para garantizar unicidad.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a verificar.</param>
        /// <returns>True si el nombre de usuario ya existe.</returns>
        public bool ExisteNombreUsuario(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return false;
            }

            try
            {
                string nombreNormalizado = nombreUsuario.Trim();
                var usuarioEncontrado = _contexto.Usuario.FirstOrDefault(usuarioEntidad =>
                    usuarioEntidad.Nombre_Usuario == nombreNormalizado);

                return usuarioEncontrado != null
                    && string.Equals(
                        usuarioEncontrado.Nombre_Usuario, 
                        nombreNormalizado,
                        StringComparison.Ordinal);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorVerificarExistencia, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorVerificarExistencia, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorVerificarExistencia, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorVerificarExistencia, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorVerificarExistencia, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorVerificarExistencia, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorVerificarExistencia, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorVerificarExistencia, 
                    excepcion);
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// Agrega el usuario al contexto y persiste los cambios.
        /// </summary>
        /// <param name="usuario">Entidad de usuario a crear.</param>
        /// <returns>Usuario creado con su identificador asignado.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si usuario es null.</exception>
        public Usuario CrearUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                var excepcion = new ArgumentNullException(nameof(usuario));
                _logger.Error(MensajesErrorDatos.Usuario.IntentarCrearUsuarioNulo, excepcion);
                throw excepcion;
            }

            try
            {
                var usuarioCreado = _contexto.Usuario.Add(usuario);
                _contexto.SaveChanges();

                return usuarioCreado;
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorGuardarUsuario, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorGuardarUsuario, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorGuardarUsuario, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorGuardarUsuario, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorGuardarUsuario, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorGuardarUsuario, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorGuardarUsuario, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorGuardarUsuario, 
                    excepcion);
            }
        }

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario.
        /// Usa comparacion exacta (case-sensitive) para buscar el usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a buscar.</param>
        /// <returns>Usuario encontrado.</returns>
        /// <exception cref="ArgumentException">Se lanza si nombreUsuario es null o vacio.
        /// </exception>
        /// <exception cref="KeyNotFoundException">Se lanza si el usuario no existe.</exception>
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                var excepcion = new ArgumentException(
                    MensajesErrorDatos.Usuario.NombreUsuarioObligatorio,
                    nameof(nombreUsuario));
                _logger.Error(
                    MensajesErrorDatos.Usuario.IntentoBusquedaNombreVacio, 
                    excepcion);
                throw excepcion;
            }

            try
            {
                string nombreNormalizado = nombreUsuario.Trim();
                var usuarioEncontrado = _contexto.Usuario.FirstOrDefault(usuarioEntidad =>
                    usuarioEntidad.Nombre_Usuario == nombreNormalizado);

                if (usuarioEncontrado != null 
                    && string.Equals(
                        usuarioEncontrado.Nombre_Usuario, 
                        nombreNormalizado,
                        StringComparison.Ordinal))
                {
                    return usuarioEncontrado;
                }

                throw new KeyNotFoundException(
                    MensajesErrorDatos.Usuario.UsuarioNoEncontrado);
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Info(MensajesErrorDatos.Usuario.UsuarioNoEncontrado, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.UsuarioNoEncontrado, 
                    excepcion);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerUsuario, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerUsuario, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerUsuario, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerUsuario, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerUsuario, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerUsuario, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerUsuario, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerUsuario, 
                    excepcion);
            }
        }

        /// <summary>
        /// Obtiene un usuario buscando por el correo electronico de su jugador asociado.
        /// Realiza una busqueda exacta (case-sensitive) sobre el correo.
        /// </summary>
        /// <param name="correo">Correo electronico a buscar.</param>
        /// <returns>Usuario encontrado.</returns>
        /// <exception cref="ArgumentException">Se lanza si el correo es nulo o vacio.</exception>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe usuario con ese correo.
        /// </exception>
        public Usuario ObtenerPorCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                var excepcion = new ArgumentException(
                    MensajesErrorDatos.Usuario.CorreoObligatorio, 
                    nameof(correo));
                _logger.Error(MensajesErrorDatos.Usuario.IntentoBusquedaCorreoVacio, excepcion);
                throw excepcion;
            }

            try
            {
                var usuariosCandidatos = _contexto.Usuario
                    .Include("Jugador")
                    .Where(usuarioEntidad => usuarioEntidad.Jugador.Correo == correo)
                    .ToList();

                var usuarioEncontrado = usuariosCandidatos.FirstOrDefault(usuarioCandidato =>
                    string.Equals(
                        usuarioCandidato.Jugador?.Correo, 
                        correo, 
                        StringComparison.Ordinal));

                if (usuarioEncontrado == null)
                {
                    _logger.Info(
                        "Busqueda de usuario por correo no arrojo resultados.");
                    throw new KeyNotFoundException(
                        MensajesErrorDatos.Usuario.UsuarioNoEncontrado);
                }

                return usuarioEncontrado;
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Info(MensajesErrorDatos.Usuario.UsuarioNoEncontrado, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.UsuarioNoEncontrado, 
                    excepcion);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerPorCorreo, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerPorCorreo, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerPorCorreo, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerPorCorreo, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerPorCorreo, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerPorCorreo, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerPorCorreo, 
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerPorCorreo, 
                    excepcion);
            }
        }

        /// <summary>
        /// Obtiene un usuario de forma asincrona buscando por el correo electronico de su jugador
        /// asociado.
        /// </summary>
        /// <param name="correo">Correo electronico a buscar.</param>
        /// <returns>Una tarea que representa la operacion asincrona. El resultado contiene el
        /// usuario encontrado.</returns>
        /// <exception cref="ArgumentException">Se lanza si el correo es nulo o vacio.</exception>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe usuario con ese correo.
        /// </exception>
        public async Task<Usuario> ObtenerPorCorreoAsync(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                throw new ArgumentException(
                    MensajesErrorDatos.Usuario.CorreoObligatorioBusquedaAsincrona,
                    nameof(correo));
            }

            try
            {
                var usuarioEncontrado = await _contexto.Usuario
                    .Include(usuarioEntidad => usuarioEntidad.Jugador)
                    .FirstOrDefaultAsync(
                        usuarioEntidad => usuarioEntidad.Jugador.Correo == correo);

                if (usuarioEncontrado == null)
                {
                    _logger.Info(
                        "Busqueda asincrona de usuario por correo no arrojo resultados.");
                    throw new KeyNotFoundException(
                        MensajesErrorDatos.Usuario.UsuarioNoEncontrado);
                }

                return usuarioEncontrado;
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Info(MensajesErrorDatos.Usuario.UsuarioNoEncontrado, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.UsuarioNoEncontrado, 
                    excepcion);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorAsincronoObtenerPorCorreo,
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorAsincronoObtenerPorCorreo, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorAsincronoObtenerPorCorreo,
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorAsincronoObtenerPorCorreo, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorAsincronoObtenerPorCorreo,
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorAsincronoObtenerPorCorreo, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorAsincronoObtenerPorCorreo,
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorAsincronoObtenerPorCorreo, 
                    excepcion);
            }
        }

        /// <summary>
        /// Obtiene un usuario por su identificador, incluyendo la carga explicita de datos del
        /// jugador y sus redes sociales asociadas.
        /// </summary>
        /// <param name="idUsuario">Identificador unico del usuario.</param>
        /// <returns>El usuario encontrado con sus relaciones cargadas.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si el id es menor o igual a 
        /// cero.</exception>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe el usuario.</exception>
        public Usuario ObtenerPorIdConRedesSociales(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(idUsuario),
                    MensajesErrorDatos.Usuario.IdUsuarioMayorCero);
            }

            try
            {
                var usuarioEncontrado = _contexto.Usuario
                    .Include(usuarioEntidad => usuarioEntidad.Jugador.RedSocial)
                    .FirstOrDefault(usuarioEntidad => usuarioEntidad.idUsuario == idUsuario);

                if (usuarioEncontrado == null)
                {
                    _logger.InfoFormat(
                        "No se encontro usuario con id {0} al obtener con redes sociales.",
                        idUsuario);
                    throw new KeyNotFoundException(
                        MensajesErrorDatos.Usuario.UsuarioNoEncontrado);
                }

                return usuarioEncontrado;
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Info(MensajesErrorDatos.Usuario.UsuarioNoEncontrado, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.UsuarioNoEncontrado, 
                    excepcion);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Usuario.ErrorObtenerConRedesSociales,
                    idUsuario);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerConRedesSociales, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Usuario.ErrorObtenerConRedesSociales,
                    idUsuario);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerConRedesSociales, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Usuario.ErrorObtenerConRedesSociales,
                    idUsuario);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerConRedesSociales, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Usuario.ErrorObtenerConRedesSociales,
                    idUsuario);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerConRedesSociales, 
                    excepcion);
            }
        }

        /// <summary>
        /// Actualiza la contrasena de un usuario.
        /// </summary>
        /// <param name="usuarioId">Identificador unico del usuario.</param>
        /// <param name="nuevaContrasenaHash">Nueva contrasena hasheada.</param>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la actualizacion 
        /// a la base de datos.</exception>
        public void ActualizarContrasena(int usuarioId, string nuevaContrasenaHash)
        {
            try
            {
                var usuario = _contexto.Usuario.FirstOrDefault(usuarioEntidad =>
                    usuarioEntidad.idUsuario == usuarioId);
                if (usuario != null)
                {
                    usuario.Contrasena = nuevaContrasenaHash;
                    _contexto.SaveChanges();
                }
            }
            catch (DbUpdateException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Usuario.ErrorActualizarContrasena,
                    usuarioId);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Usuario.ErrorActualizarContrasena,
                    usuarioId);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Usuario.ErrorActualizarContrasena,
                    usuarioId);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Usuario.ErrorActualizarContrasena,
                    usuarioId);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Usuario.ErrorActualizarContrasena,
                    usuarioId);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Usuario.ErrorActualizarContrasena,
                    usuarioId);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Usuario.ErrorActualizarContrasena,
                    usuarioId);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Usuario.ErrorActualizarContrasena,
                    usuarioId);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
        }

        /// <summary>
        /// Busca los datos de un Jugador con el Nombre del Usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del Usuario.</param>
        /// <returns>Usuario encontrado con datos de jugador cargados.</returns>
        /// <exception cref="ArgumentException">Se lanza si el nombre es nulo o vacio.</exception>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe el usuario.</exception>
        public Usuario ObtenerPorNombreConJugador(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException(
                    MensajesErrorDatos.Usuario.NombreUsuarioObligatorioBusquedaJugador,
                    nameof(nombreUsuario));
            }

            try
            {
                string nombreNormalizado = nombreUsuario.Trim();

                var usuarioEncontrado = _contexto.Usuario
                    .Include(usuarioEntidad => usuarioEntidad.Jugador)
                    .FirstOrDefault(usuarioEntidad => 
                        usuarioEntidad.Nombre_Usuario == nombreNormalizado);

                if (usuarioEncontrado == null)
                {
                    _logger.Info(
                        "Busqueda de usuario con datos de jugador no arrojo resultados.");
                    throw new KeyNotFoundException(
                        MensajesErrorDatos.Usuario.UsuarioNoEncontrado);
                }

                return usuarioEncontrado;
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Info(MensajesErrorDatos.Usuario.UsuarioNoEncontrado, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.UsuarioNoEncontrado, 
                    excepcion);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerConJugadorPorNombre,
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerConJugadorPorNombre, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerConJugadorPorNombre,
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerConJugadorPorNombre, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerConJugadorPorNombre,
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerConJugadorPorNombre, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesErrorDatos.Usuario.ErrorObtenerConJugadorPorNombre,
                    excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Usuario.ErrorObtenerConJugadorPorNombre, 
                    excepcion);
            }
        }
    }
}
