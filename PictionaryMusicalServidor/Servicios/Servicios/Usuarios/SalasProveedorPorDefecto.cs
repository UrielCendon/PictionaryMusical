using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Implementacion por defecto del proveedor de salas.
    /// Utiliza SalasManejador para obtener la lista de salas.
    /// </summary>
    public class SalasProveedorPorDefecto : ISalasProveedor
    {
        /// <summary>
        /// Obtiene la lista de salas activas usando SalasManejador.
        /// </summary>
        /// <returns>Lista de salas disponibles.</returns>
        public IList<SalaDTO> ObtenerListaSalas()
        {
            return SalasManejador.ObtenerListaSalas();
        }
    }
}
