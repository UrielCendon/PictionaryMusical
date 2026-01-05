using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Interfaz para el almacenamiento de salas en memoria.
    /// Permite abstraer el diccionario concurrente para facilitar pruebas unitarias.
    /// </summary>
    public interface IAlmacenSalas
    {
        /// <summary>
        /// Intenta agregar una sala al almacen.
        /// </summary>
        /// <param name="codigo">Codigo unico de la sala.</param>
        /// <param name="sala">Instancia de la sala a agregar.</param>
        /// <returns>True si se agrego correctamente, False si ya existe.</returns>
        bool TryAdd(string codigo, SalaInternaManejador sala);

        /// <summary>
        /// Intenta obtener una sala del almacen.
        /// </summary>
        /// <param name="codigo">Codigo de la sala a buscar.</param>
        /// <param name="sala">Sala encontrada si existe.</param>
        /// <returns>True si se encontro la sala.</returns>
        bool TryGetValue(string codigo, out SalaInternaManejador sala);

        /// <summary>
        /// Intenta remover una sala del almacen.
        /// </summary>
        /// <param name="codigo">Codigo de la sala a remover.</param>
        /// <param name="sala">Sala removida si existia.</param>
        /// <returns>True si se removio correctamente.</returns>
        bool TryRemove(string codigo, out SalaInternaManejador sala);

        /// <summary>
        /// Verifica si existe una sala con el codigo especificado.
        /// </summary>
        /// <param name="codigo">Codigo de la sala a verificar.</param>
        /// <returns>True si la sala existe.</returns>
        bool ContainsKey(string codigo);

        /// <summary>
        /// Obtiene todas las salas almacenadas.
        /// </summary>
        IEnumerable<SalaInternaManejador> Values { get; }

        /// <summary>
        /// Limpia todas las salas del almacen.
        /// </summary>
        void Clear();
    }
}
