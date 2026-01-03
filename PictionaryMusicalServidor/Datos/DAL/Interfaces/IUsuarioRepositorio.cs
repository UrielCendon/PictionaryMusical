using Datos.Modelo;
using System.Threading.Tasks;

namespace PictionaryMusicalServidor.Datos.DAL.Interfaces
{

    /// <summary>
    /// Interfaz de repositorio para la gestion de usuarios en la capa de acceso a datos.
    /// Define operaciones de consulta y creacion de usuarios.
    /// </summary>
    public interface IUsuarioRepositorio
    {
        /// <summary>
        /// Verifica si existe un usuario con el nombre de usuario especificado.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a verificar.</param>
        /// <returns>True si el nombre de usuario ya existe.</returns>
        bool ExisteNombreUsuario(string nombreUsuario);

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="usuario">Entidad de usuario a crear.</param>
        /// <returns>Usuario creado con su identificador asignado.</returns>
        Usuario CrearUsuario(Usuario usuario);

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a buscar.</param>
        /// <returns>Usuario encontrado.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si el usuario no existe.</exception>
        Usuario ObtenerPorNombreUsuario(string nombreUsuario);

        /// <summary>
        /// Obtiene un usuario buscando por el correo electronico de su jugador asociado.
        /// </summary>
        /// <param name="correo">Correo electronico a buscar.</param>
        /// <returns>Usuario encontrado.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe usuario con ese correo.
        /// </exception>
        Usuario ObtenerPorCorreo(string correo);

        /// <summary>
        /// Obtiene un usuario de forma asincrona buscando por su correo electronico.
        /// </summary>
        /// <param name="correo">Correo electronico a buscar.</param>
        /// <returns>Tarea con el usuario encontrado.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe usuario con ese correo.
        /// </exception>
        Task<Usuario> ObtenerPorCorreoAsync(string correo);

        /// <summary>
        /// Obtiene un usuario por su identificador, incluyendo datos de jugador y redes sociales.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario.</param>
        /// <returns>Usuario encontrado con relaciones cargadas.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe el usuario.</exception>
        Usuario ObtenerPorIdConRedesSociales(int idUsuario);

        /// <summary>
        /// Actualiza la contrasena de un usuario especifico.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        /// <param name="nuevaContrasenaHash">Hash de la nueva contrasena.</param>
        void ActualizarContrasena(int usuarioId, string nuevaContrasenaHash);

        /// <summary>
        /// Obtiene un usuario y sus datos de jugador asociados buscando por nombre de usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a buscar.</param>
        /// <returns>Usuario con datos de jugador cargados.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si no existe el usuario.</exception>
        Usuario ObtenerPorNombreConJugador(string nombreUsuario);
    }
}
