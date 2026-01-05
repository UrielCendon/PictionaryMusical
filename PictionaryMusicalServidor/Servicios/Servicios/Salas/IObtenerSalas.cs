using System.Collections.Generic;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Interfaz para obtener la coleccion de salas internas.
    /// </summary>
    public interface IObtenerSalas
    {
        /// <summary>
        /// Obtiene todas las salas internas activas.
        /// </summary>
        /// <returns>Coleccion de salas internas.</returns>
        IEnumerable<SalaInternaManejador> ObtenerSalasInternas();
    }
}
