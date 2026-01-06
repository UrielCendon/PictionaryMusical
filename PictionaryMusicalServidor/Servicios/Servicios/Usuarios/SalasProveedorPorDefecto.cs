using System.Collections.Generic;
using System.Linq;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Implementacion por defecto del proveedor de salas.
    /// Utiliza AlmacenSalasEstatico para obtener la lista de salas.
    /// </summary>
    public class SalasProveedorPorDefecto : ISalasProveedor
    {
        private readonly IAlmacenSalas _almacenSalas;

        /// <summary>
        /// Constructor por defecto que usa la instancia singleton.
        /// </summary>
        public SalasProveedorPorDefecto() : this(AlmacenSalasEstatico.Instancia)
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="almacenSalas">Almacen de salas a utilizar.</param>
        public SalasProveedorPorDefecto(IAlmacenSalas almacenSalas)
        {
            _almacenSalas = almacenSalas;
        }

        /// <summary>
        /// Obtiene la lista de salas activas.
        /// </summary>
        /// <returns>Lista de salas disponibles.</returns>
        public IList<SalaDTO> ObtenerListaSalas()
        {
            return _almacenSalas.Valores
                .Select(sala => sala.ConvertirADto())
                .ToList();
        }
    }
}
