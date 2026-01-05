using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Implementacion del almacen de salas utilizando un diccionario concurrente.
    /// </summary>
    public class AlmacenSalas : IAlmacenSalas
    {
        private readonly ConcurrentDictionary<string, SalaInternaManejador> _salas;

        /// <summary>
        /// Constructor que inicializa el diccionario con comparador insensible a mayusculas.
        /// </summary>
        public AlmacenSalas()
        {
            _salas = new ConcurrentDictionary<string, SalaInternaManejador>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public bool TryAdd(string codigo, SalaInternaManejador sala)
        {
            return _salas.TryAdd(codigo, sala);
        }

        /// <inheritdoc/>
        public bool TryGetValue(string codigo, out SalaInternaManejador sala)
        {
            return _salas.TryGetValue(codigo, out sala);
        }

        /// <inheritdoc/>
        public bool TryRemove(string codigo, out SalaInternaManejador sala)
        {
            return _salas.TryRemove(codigo, out sala);
        }

        /// <inheritdoc/>
        public bool ContainsKey(string codigo)
        {
            return _salas.ContainsKey(codigo);
        }

        /// <inheritdoc/>
        public IEnumerable<SalaInternaManejador> Values => _salas.Values;

        /// <inheritdoc/>
        public void Clear()
        {
            _salas.Clear();
        }
    }
}
