using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Implementacion del almacen de salas que usa el diccionario estatico compartido.
    /// Esta clase mantiene compatibilidad con el comportamiento original del servicio WCF.
    /// </summary>
    public class AlmacenSalasEstatico : IAlmacenSalas
    {
        private static readonly ConcurrentDictionary<string, SalaInternaManejador> _salas =
            new ConcurrentDictionary<string, SalaInternaManejador>(
                StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public bool IntentarAgregar(string codigo, SalaInternaManejador sala)
        {
            return _salas.TryAdd(codigo, sala);
        }

        /// <inheritdoc/>
        public bool IntentarObtener(string codigo, out SalaInternaManejador sala)
        {
            return _salas.TryGetValue(codigo, out sala);
        }

        /// <inheritdoc/>
        public bool IntentarRemover(string codigo, out SalaInternaManejador sala)
        {
            return _salas.TryRemove(codigo, out sala);
        }

        /// <inheritdoc/>
        public bool ContieneCodigo(string codigo)
        {
            return _salas.ContainsKey(codigo);
        }

        /// <inheritdoc/>
        public IEnumerable<SalaInternaManejador> Valores => _salas.Values;

        /// <inheritdoc/>
        public void Limpiar()
        {
            _salas.Clear();
        }

        /// <summary>
        /// Obtiene las salas almacenadas de forma estatica.
        /// Permite acceso desde metodos estaticos que necesitan consultar las salas.
        /// </summary>
        /// <returns>Coleccion de valores del diccionario de salas.</returns>
        public static IEnumerable<SalaInternaManejador> ObtenerSalasEstaticas()
        {
            return _salas.Values;
        }
    }
}
