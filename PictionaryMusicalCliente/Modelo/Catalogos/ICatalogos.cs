using System.Collections.Generic;
using System.Windows.Media;

namespace PictionaryMusicalCliente.Modelo.Catalogos
{
    /// <summary>
    /// Define el contrato para obtener la coleccion de avatares disponibles.
    /// </summary>
    public interface ICatalogoAvatares
    {
        /// <summary>
        /// Obtiene la lista completa de avatares cargados.
        /// </summary>
        /// <returns>Lista de lectura de objetos avatar.</returns>
        IReadOnlyList<ObjetoAvatar> ObtenerAvatares();

        /// <summary>
        /// Obtiene un avatar especifico por su identificador unico.
        /// </summary>
        /// <param name="id">Identificador del avatar.</param>
        /// <returns>Resultado con el avatar si existe, o fallo si no se encuentra.</returns>
        ResultadoOperacion<ObjetoAvatar> ObtenerPorId(int id);
    }

    /// <summary>
    /// Define el contrato para acceder a recursos graficos del perfil.
    /// </summary>
    public interface ICatalogoImagenesPerfil
    {
        /// <summary>
        /// Recupera el icono asociado a una red social especifica.
        /// </summary>
        /// <param name="nombre">Nombre clave de la red social.</param>
        /// <returns>
        /// El recurso grafico correspondiente, o valor vacio si no existe.
        /// </returns>
        ImageSource ObtenerIconoRedSocial(string nombre);
    }

    /// <summary>
    /// Define el contrato para acceder al catalogo de canciones de la partida.
    /// </summary>
    public interface ICatalogoCanciones
    {
        /// <summary>
        /// Obtiene una cancion por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la cancion.</param>
        /// <returns>Resultado con la cancion si existe, o fallo si no se encuentra.</returns>
        ResultadoOperacion<Cancion> ObtenerPorId(int id);

        /// <summary>
        /// Obtiene todas las canciones del catalogo.
        /// </summary>
        /// <returns>Coleccion de todas las canciones disponibles.</returns>
        IReadOnlyList<Cancion> ObtenerTodas();
    }
}