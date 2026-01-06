using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;

namespace PictionaryMusicalCliente.VistaModelo.VentanaPrincipal.Auxiliares
{
    /// <summary>
    /// Encapsula los parametros necesarios para crear un GestorListaAmigos.
    /// </summary>
    public sealed class GestorListaAmigosParametros
    {
        /// <summary>
        /// Obtiene o establece el servicio de lista de amigos.
        /// </summary>
        public IListaAmigosServicio ListaAmigosServicio { get; set; }

        /// <summary>
        /// Obtiene o establece el servicio de amigos.
        /// </summary>
        public IAmigosServicio AmigosServicio { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de usuario de la sesion.
        /// </summary>
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Obtiene o establece la accion para ejecutar codigo en el dispatcher UI.
        /// </summary>
        public Action<Action> EjecutarEnDispatcher { get; set; }
    }
}
