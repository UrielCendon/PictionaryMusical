using System.Collections.Generic;

namespace PictionaryMusicalServidor.Datos.Utilidades
{
    public interface IGeneradorAleatorio
    {
        int ObtenerIndiceAleatorio(int tamanoColeccion);
        T SeleccionarAleatorio<T>(IList<T> lista);
        void MezclarLista<T>(IList<T> lista);
    }
}