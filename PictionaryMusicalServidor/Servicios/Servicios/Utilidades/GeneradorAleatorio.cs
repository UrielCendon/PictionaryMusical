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
        private static readonly Random _random = new Random();
        private static readonly object _bloqueo = new object();

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
        public static string GenerarCodigoVerificacion(int longitud = 6)
        {
            if (longitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(longitud));
            }

            lock (_bloqueo)
            {
                int limiteSuperior = (int)Math.Pow(10, longitud) - 1;
                int limiteInferior = (int)Math.Pow(10, longitud - 1);
                int numero = _random.Next(limiteInferior, limiteSuperior);
                return numero.ToString();
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
        public static string GenerarCodigoSala(int longitud = 6)
        {
            if (longitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(longitud));
            }

            lock (_bloqueo)
            {
                int limiteSuperior = (int)Math.Pow(10, longitud);
                int numero = _random.Next(0, limiteSuperior);
                return numero.ToString("D" + longitud);
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

            lock (_bloqueo)
            {
                for (int i = lista.Count - 1; i > 0; i--)
                {
                    int j = _random.Next(i + 1);
                    T temp = lista[i];
                    lista[i] = lista[j];
                    lista[j] = temp;
                }
            }
        }
    }
}
