using System;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Implementacion singleton de callbacks compartidos entre servicios.
    /// Permite que servicios como AmigosManejador y ListaAmigosManejador
    /// compartan los mismos callbacks de notificacion.
    /// </summary>
    public sealed class CallbacksCompartidos : ICallbacksCompartidos
    {
        private static readonly Lazy<CallbacksCompartidos> _instancia =
            new Lazy<CallbacksCompartidos>(() => new CallbacksCompartidos());

        private readonly ManejadorCallback<IListaAmigosManejadorCallback> _listaAmigosCallback;

        /// <summary>
        /// Obtiene la instancia singleton de CallbacksCompartidos.
        /// </summary>
        public static CallbacksCompartidos Instancia => _instancia.Value;

        /// <summary>
        /// Constructor privado para patron singleton.
        /// </summary>
        private CallbacksCompartidos()
        {
            _listaAmigosCallback = new ManejadorCallback<IListaAmigosManejadorCallback>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Obtiene el manejador de callbacks compartido para la lista de amigos.
        /// Esta instancia es utilizada por AmigosManejador y ListaAmigosManejador
        /// para asegurar que las notificaciones lleguen a todos los clientes suscritos.
        /// </summary>
        public IManejadorCallback<IListaAmigosManejadorCallback> ListaAmigos => 
            _listaAmigosCallback;

        /// <summary>
        /// Acceso estatico al manejador de lista de amigos para compatibilidad.
        /// </summary>
        public static IManejadorCallback<IListaAmigosManejadorCallback> ListaAmigosCompartido =>
            Instancia.ListaAmigos;
    }
}
