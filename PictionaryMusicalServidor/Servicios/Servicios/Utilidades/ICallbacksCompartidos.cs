using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Interfaz para proveer callbacks compartidos entre servicios.
    /// Permite la inyeccion de dependencias para pruebas unitarias.
    /// </summary>
    public interface ICallbacksCompartidos
    {
        /// <summary>
        /// Obtiene el manejador de callbacks para la lista de amigos.
        /// </summary>
        IManejadorCallback<IListaAmigosManejadorCallback> ListaAmigos { get; }
    }
}
