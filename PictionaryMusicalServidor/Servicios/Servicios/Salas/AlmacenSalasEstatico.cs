using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Implementacion singleton del almacen de salas que usa un diccionario estatico compartido.
    /// Esta clase mantiene compatibilidad con el comportamiento original del servicio WCF.
    /// </summary>
    public sealed class AlmacenSalasEstatico : IAlmacenSalas
    {
        private static readonly Lazy<AlmacenSalasEstatico> _instancia =
            new Lazy<AlmacenSalasEstatico>(() => new AlmacenSalasEstatico());

        private readonly ConcurrentDictionary<string, SalaInternaManejador> _salas;

        /// <summary>
        /// Obtiene la instancia singleton del almacen de salas.
        /// </summary>
        public static AlmacenSalasEstatico Instancia => _instancia.Value;

        /// <summary>
        /// Constructor privado para patron singleton.
        /// </summary>
        private AlmacenSalasEstatico()
        {
            _salas = new ConcurrentDictionary<string, SalaInternaManejador>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Intenta agregar una sala al almacen.
        /// </summary>
        /// <param name="codigo">Codigo unico de la sala.</param>
        /// <param name="sala">Instancia de la sala a agregar.</param>
        /// <returns>True si se agrego correctamente, False si ya existe.</returns>
        public bool IntentarAgregar(string codigo, SalaInternaManejador sala)
        {
            return _salas.TryAdd(codigo, sala);
        }

        /// <summary>
        /// Intenta obtener una sala del almacen.
        /// </summary>
        /// <param name="codigo">Codigo de la sala a buscar.</param>
        /// <param name="sala">Sala encontrada si existe.</param>
        /// <returns>True si se encontro la sala.</returns>
        public bool IntentarObtener(string codigo, out SalaInternaManejador sala)
        {
            return _salas.TryGetValue(codigo, out sala);
        }

        /// <summary>
        /// Intenta remover una sala del almacen.
        /// </summary>
        /// <param name="codigo">Codigo de la sala a remover.</param>
        /// <param name="sala">Sala removida si existia.</param>
        /// <returns>True si se removio correctamente.</returns>
        public bool IntentarRemover(string codigo, out SalaInternaManejador sala)
        {
            return _salas.TryRemove(codigo, out sala);
        }

        /// <summary>
        /// Verifica si existe una sala con el codigo especificado.
        /// </summary>
        /// <param name="codigo">Codigo de la sala a verificar.</param>
        /// <returns>True si la sala existe.</returns>
        public bool ContieneCodigo(string codigo)
        {
            return _salas.ContainsKey(codigo);
        }

        /// <summary>
        /// Obtiene todas las salas almacenadas.
        /// </summary>
        public IEnumerable<SalaInternaManejador> Valores => _salas.Values;

        /// <summary>
        /// Limpia todas las salas del almacen.
        /// </summary>
        public void Limpiar()
        {
            _salas.Clear();
        }
    }
}
