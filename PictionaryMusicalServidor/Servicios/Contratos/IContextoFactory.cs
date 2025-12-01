using Datos.Modelo;

namespace PictionaryMusicalServidor.Servicios.Contratos
{
    /// <summary>
    /// Interfaz para la factoria de creacion de contextos de base de datos.
    /// Permite inyectar y mockear la creacion de contextos en pruebas unitarias.
    /// </summary>
    public interface IContextoFactory
    {
        /// <summary>
        /// Crea una nueva instancia del contexto de base de datos.
        /// </summary>
        /// <returns>Instancia del contexto de base de datos configurada.</returns>
        BaseDatosPruebaEntities CrearContexto();
    }
}
