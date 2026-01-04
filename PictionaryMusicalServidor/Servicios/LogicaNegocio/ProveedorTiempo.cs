using System.Threading.Tasks;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Implementacion del proveedor de tiempo usando Task.Delay.
    /// </summary>
    public class ProveedorTiempo : IProveedorTiempo
    {
        /// <summary>
        /// Crea un retraso asincrono por el tiempo especificado.
        /// </summary>
        /// <param name="milisegundos">Duracion del retraso en milisegundos.</param>
        /// <returns>Tarea que se completa cuando transcurre el tiempo.</returns>
        public Task Retrasar(int milisegundos)
        {
            return Task.Delay(milisegundos);
        }
    }
}