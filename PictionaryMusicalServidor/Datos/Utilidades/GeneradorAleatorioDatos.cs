using System;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Datos.Utilidades
{
    /// <summary>
    /// Clase utilitaria para generar valores aleatorios de forma thread-safe.
    /// Proporciona metodos para generar indices aleatorios y seleccionar elementos de colecciones.
    /// </summary>
    internal static class GeneradorAleatorioDatos
    {
        private static readonly Random _random = new Random();
        private static readonly object _bloqueo = new object();

        /// <summary>
        /// Genera un indice aleatorio valido para una coleccion del tamano especificado.
        /// </summary>
        /// <param name="tamanoColeccion">Tamano de la coleccion.</param>
        /// <returns>Indice aleatorio entre 0 y tamanoColeccion - 1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si tamanoColeccion es menor o 
        /// igual a 0.</exception>
        public static int ObtenerIndiceAleatorio(int tamanoColeccion)
        {
            if (tamanoColeccion <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tamanoColeccion));
            }

            lock (_bloqueo)
            {
                return _random.Next(tamanoColeccion);
            }
        }

        /// <summary>
        /// Selecciona un elemento aleatorio de una lista.
        /// </summary>
        /// <typeparam name="T">Tipo de elementos en la lista.</typeparam>
        /// <param name="lista">Lista de la cual seleccionar.</param>
        /// <returns>Elemento aleatorio de la lista.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si la lista es null.</exception>
        /// <exception cref="ArgumentException">Se lanza si la lista esta vacia.</exception>
        public static T SeleccionarAleatorio<T>(IList<T> lista)
        {
            if (lista == null)
            {
                throw new ArgumentNullException(nameof(lista));
            }

            if (lista.Count == 0)
            {
                throw new ArgumentException("La lista no puede estar vacia.", nameof(lista));
            }

            int indice = ObtenerIndiceAleatorio(lista.Count);
            return lista[indice];
        }
    }
}
