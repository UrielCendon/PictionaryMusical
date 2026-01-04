using System.Threading.Tasks;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces
{
    /// <summary>
    /// Define el contrato para proveer operaciones de tiempo y retrasos.
    /// </summary>
    public interface IProveedorTiempo
    {
        /// <summary>
        /// Crea un retraso asincrono por el tiempo especificado.
        /// </summary>
        /// <param name="milisegundos">Duracion del retraso en milisegundos.</param>
        /// <returns>Tarea que se completa cuando transcurre el tiempo.</returns>
        Task Retrasar(int milisegundos);
    }
}