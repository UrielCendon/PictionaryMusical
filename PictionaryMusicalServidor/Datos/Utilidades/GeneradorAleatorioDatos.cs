using System;
using System.Collections.Generic;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;

namespace PictionaryMusicalServidor.Datos.Utilidades
{
    /// <summary>
    /// Implementacion utilitaria para generar valores aleatorios de forma thread-safe.
    /// </summary>
    public class GeneradorAleatorioDatos : IGeneradorAleatorio
    {
        private readonly Random _random;
        private readonly object _bloqueo;

        public GeneradorAleatorioDatos()
        {
            _random = new Random();
            _bloqueo = new object();
        }

        /// <summary>
        /// Genera un indice aleatorio valido para una coleccion del tamano especificado.
        /// </summary>
        /// <param name="tamanoColeccion">Tamano de la coleccion.</param>
        /// <returns>Indice aleatorio entre 0 y tamanoColeccion - 1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si tamanoColeccion es menor o 
        /// igual a 0.</exception>
        public int ObtenerIndiceAleatorio(int tamanoColeccion)
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
        public T SeleccionarAleatorio<T>(IList<T> lista)
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

        /// <summary>
        /// Mezcla aleatoriamente los elementos de una lista (Algoritmo Fisher-Yates).
        /// </summary>
        /// <typeparam name="T">Tipo de elementos.</typeparam>
        /// <param name="lista">Lista a mezclar.</param>
        /// <exception cref="ArgumentNullException">Se lanza si la lista es nula.</exception>
        public void MezclarLista<T>(IList<T> lista)
        {
            if (lista == null)
            {
                throw new ArgumentNullException(nameof(lista));
            }

            lock (_bloqueo)
            {
                int indiceActual = lista.Count;
                while (indiceActual > 1)
                {
                    indiceActual--;
                    int indiceAleatorio = _random.Next(indiceActual + 1);
                    T valorTemporal = lista[indiceAleatorio];
                    lista[indiceAleatorio] = lista[indiceActual];
                    lista[indiceActual] = valorTemporal;
                }
            }
        }
    }
}