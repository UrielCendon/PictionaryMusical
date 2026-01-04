using System.Collections.Generic;

namespace PictionaryMusicalServidor.Datos.DAL.Interfaces
{
    public interface IGeneradorAleatorio
    {
        int ObtenerIndiceAleatorio(int tamanoColeccion);
        T SeleccionarAleatorio<T>(IList<T> lista);
        void MezclarLista<T>(IList<T> lista);
    }
}