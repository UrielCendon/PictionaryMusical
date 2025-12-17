using System;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Contenedor singleton para callbacks compartidos entre servicios.
    /// Permite que servicios como AmigosManejador y ListaAmigosManejador
    /// compartan los mismos callbacks de notificacion.
    /// </summary>
    internal static class CallbacksCompartidos
    {
        private static readonly Lazy<ManejadorCallback<IListaAmigosManejadorCallback>> 
            _listaAmigosCallback = new Lazy<ManejadorCallback<IListaAmigosManejadorCallback>>(
                () => new ManejadorCallback<IListaAmigosManejadorCallback>(
                    StringComparer.OrdinalIgnoreCase));

        /// <summary>
        /// Obtiene el manejador de callbacks compartido para la lista de amigos.
        /// Esta instancia es utilizada por AmigosManejador y ListaAmigosManejador
        /// para asegurar que las notificaciones lleguen a todos los clientes suscritos.
        /// </summary>
        public static ManejadorCallback<IListaAmigosManejadorCallback> ListaAmigos =>
            _listaAmigosCallback.Value;
    }
}
