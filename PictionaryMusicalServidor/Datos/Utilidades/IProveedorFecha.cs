using System;

namespace PictionaryMusicalServidor.Datos.DAL.Interfaces
{
    /// <summary>
    /// Define un contrato para obtener la fecha actual.
    /// Permite inyectar la dependencia de fecha para facilitar pruebas unitarias.
    /// </summary>
    public interface IProveedorFecha
    {
        /// <summary>
        /// Obtiene la fecha y hora actual en formato UTC.
        /// </summary>
        /// <returns>La fecha y hora actual en UTC.</returns>
        DateTime ObtenerFechaActualUtc();
    }
}