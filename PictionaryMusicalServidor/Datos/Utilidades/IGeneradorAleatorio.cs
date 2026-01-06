using System.Collections.Generic;

namespace PictionaryMusicalServidor.Datos.Utilidades
{
    /// <summary>
    /// Define operaciones para generar valores aleatorios y manipular colecciones.
    /// </summary>
    public interface IGeneradorAleatorio
    {
        /// <summary>
        /// Genera un indice aleatorio valido para una coleccion.
        /// </summary>
        int ObtenerIndiceAleatorio(int tamanoColeccion);

        /// <summary>
        /// Selecciona un elemento aleatorio de una lista.
        /// </summary>
        T SeleccionarAleatorio<T>(IList<T> lista);

        /// <summary>
        /// Mezcla aleatoriamente los elementos de una lista.
        /// </summary>
        void MezclarLista<T>(IList<T> lista);
    }
}