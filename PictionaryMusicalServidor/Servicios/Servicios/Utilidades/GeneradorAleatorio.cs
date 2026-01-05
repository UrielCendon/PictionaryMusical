using System;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Clase utilitaria centralizada para generar valores aleatorios, codigos y tokens.
    /// Proporciona metodos thread-safe para generar codigos numericos, tokens de sesion,
    /// codigos de sala y mezclar listas.
    /// </summary>
    internal static class GeneradorAleatorio
    {
        private const int LongitudCodigoPorDefecto = 6;
        private const int BaseNumerica = 10;

        private static readonly Random _generadorNumeros = new Random();
        private static readonly object _objetoBloqueo = new object();

        /// <summary>
        /// Genera un token unico de sesion basado en GUID.
        /// El token generado es una cadena hexadecimal de 32 caracteres sin guiones.
        /// </summary>
        /// <returns>Token unico como cadena hexadecimal.</returns>
        public static string GenerarToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Genera un codigo de verificacion numerico aleatorio de la longitud especificada.
        /// El codigo generado no contiene ceros iniciales y es thread-safe.
        /// </summary>
        /// <param name="longitud">Longitud del codigo a generar (por defecto 6 digitos).</param>
        /// <returns>Codigo de verificacion numerico como cadena.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si longitud es menor o 
        /// igual a 0.</exception>
        public static string GenerarCodigoVerificacion(int longitud = LongitudCodigoPorDefecto)
        {
            if (longitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(longitud));
            }

            lock (_objetoBloqueo)
            {
                int valorMaximo = (int)Math.Pow(BaseNumerica, longitud) - 1;
                int valorMinimo = (int)Math.Pow(BaseNumerica, longitud - 1);
                int codigoGenerado = _generadorNumeros.Next(valorMinimo, valorMaximo);
                return codigoGenerado.ToString();
            }
        }

        /// <summary>
        /// Genera un codigo de sala aleatorio formateado con ceros a la izquierda.
        /// </summary>
        /// <param name="longitud">Longitud del codigo a generar (por defecto 6 digitos).</param>
        /// <returns>Codigo de sala formateado como cadena con ceros iniciales si es necesario.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza si longitud es menor o 
        /// igual a 0.</exception>
        public static string GenerarCodigoSala(int longitud = LongitudCodigoPorDefecto)
        {
            if (longitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(longitud));
            }

            lock (_objetoBloqueo)
            {
                int valorMaximo = (int)Math.Pow(BaseNumerica, longitud);
                int codigoGenerado = _generadorNumeros.Next(0, valorMaximo);
                return codigoGenerado.ToString("D" + longitud);
            }
        }

        /// <summary>
        /// Mezcla aleatoriamente los elementos de una lista usando el algoritmo Fisher-Yates.
        /// La operacion modifica la lista original.
        /// </summary>
        /// <typeparam name="T">Tipo de elementos en la lista.</typeparam>
        /// <param name="lista">Lista a mezclar.</param>
        /// <exception cref="ArgumentNullException">Se lanza si la lista es null.</exception>
        public static void MezclarLista<T>(IList<T> lista)
        {
            if (lista == null)
            {
                throw new ArgumentNullException(nameof(lista));
            }

            lock (_objetoBloqueo)
            {
                for (int indiceActual = lista.Count - 1; indiceActual > 0; indiceActual--)
                {
                    int indiceAleatorio = _generadorNumeros.Next(indiceActual + 1);
                    T elementoTemporal = lista[indiceActual];
                    lista[indiceActual] = lista[indiceAleatorio];
                    lista[indiceAleatorio] = elementoTemporal;
                }
            }
        }
    }
}
