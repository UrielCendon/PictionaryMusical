using Datos.Modelo;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Implementacion concreta de la factoria de repositorios.
    /// Crea instancias de repositorios utilizando un contexto de base de datos proporcionado.
    /// Esta clase facilita las pruebas unitarias al permitir mockear la creacion de repositorios.
    /// </summary>
    public class RepositorioFactoria : IRepositorioFactoria
    {
        /// <summary>
        /// Crea una instancia del repositorio de usuarios.
        /// </summary>
        /// <param name="contexto">Contexto de base de datos a utilizar.</param>
        /// <returns>Instancia del repositorio de usuarios.</returns>
        public IUsuarioRepositorio CrearUsuarioRepositorio(BaseDatosPruebaEntities contexto)
        {
            return new UsuarioRepositorio(contexto);
        }

        /// <summary>
        /// Crea una instancia del repositorio de amigos.
        /// </summary>
        /// <param name="contexto">Contexto de base de datos a utilizar.</param>
        /// <returns>Instancia del repositorio de amigos.</returns>
        public IAmigoRepositorio CrearAmigoRepositorio(BaseDatosPruebaEntities contexto)
        {
            return new AmigoRepositorio(contexto);
        }

        /// <summary>
        /// Crea una instancia del repositorio de clasificaciones.
        /// </summary>
        /// <param name="contexto">Contexto de base de datos a utilizar.</param>
        /// <returns>Instancia del repositorio de clasificaciones.</returns>
        public IClasificacionRepositorio CrearClasificacionRepositorio(
            BaseDatosPruebaEntities contexto)
        {
            return new ClasificacionRepositorio(contexto);
        }

        /// <summary>
        /// Crea una instancia del repositorio de reportes.
        /// </summary>
        /// <param name="contexto">Contexto de base de datos a utilizar.</param>
        /// <returns>Instancia del repositorio de reportes.</returns>
        public IReporteRepositorio CrearReporteRepositorio(BaseDatosPruebaEntities contexto)
        {
            return new ReporteRepositorio(contexto);
        }

        /// <summary>
        /// Crea una instancia del repositorio de jugadores.
        /// </summary>
        /// <param name="contexto">Contexto de base de datos a utilizar.</param>
        /// <returns>Instancia del repositorio de jugadores.</returns>
        public IJugadorRepositorio CrearJugadorRepositorio(BaseDatosPruebaEntities contexto)
        {
            return new JugadorRepositorio(contexto);
        }
    }
}
