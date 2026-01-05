using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Interfaz para proveer listas de salas.
    /// Permite abstraer el acceso a las salas para facilitar pruebas unitarias.
    /// </summary>
    public interface ISalasProveedor
    {
        /// <summary>
        /// Obtiene la lista de salas activas.
        /// </summary>
        /// <returns>Lista de salas disponibles.</returns>
        IList<SalaDTO> ObtenerListaSalas();
    }
}
