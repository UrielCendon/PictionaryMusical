using System;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;

namespace PictionaryMusicalServidor.Datos.Utilidades
{
    /// <summary>
    /// Implementacion del proveedor de fecha que utiliza DateTime del sistema.
    /// </summary>
    public class ProveedorFecha : IProveedorFecha
    {
        /// <summary>
        /// Obtiene la fecha y hora actual del sistema en formato UTC.
        /// </summary>
        /// <returns>La fecha y hora actual en UTC.</returns>
        public DateTime ObtenerFechaActualUtc()
        {
            return DateTime.UtcNow;
        }
    }
}