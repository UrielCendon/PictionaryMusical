using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Utilidades;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias requeridas por VentanaPrincipalVistaModelo.
    /// </summary>
    public sealed class VentanaPrincipalDependencias
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="VentanaPrincipalDependencias"/>.
        /// </summary>
        /// <param name="localizacionServicio">Servicio de localizacion cultural.</param>
        /// <param name="listaAmigosServicio">Servicio de lista de amigos.</param>
        /// <param name="amigosServicio">Servicio de operaciones de amigos.</param>
        /// <param name="salasServicio">Servicio de salas.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        /// <param name="usuarioSesion">Usuario autenticado actual.</param>
        public VentanaPrincipalDependencias(
            ILocalizacionServicio localizacionServicio,
            IListaAmigosServicio listaAmigosServicio,
            IAmigosServicio amigosServicio,
            ISalasServicio salasServicio,
            SonidoManejador sonidoManejador,
            IUsuarioAutenticado usuarioSesion)
        {
            LocalizacionServicio = localizacionServicio ??
                throw new ArgumentNullException(nameof(localizacionServicio));
            ListaAmigosServicio = listaAmigosServicio ??
                throw new ArgumentNullException(nameof(listaAmigosServicio));
            AmigosServicio = amigosServicio ??
                throw new ArgumentNullException(nameof(amigosServicio));
            SalasServicio = salasServicio ??
                throw new ArgumentNullException(nameof(salasServicio));
            SonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            UsuarioSesion = usuarioSesion ??
                throw new ArgumentNullException(nameof(usuarioSesion));
        }

        /// <summary>
        /// Obtiene el servicio de localizacion cultural.
        /// </summary>
        public ILocalizacionServicio LocalizacionServicio { get; }

        /// <summary>
        /// Obtiene el servicio de lista de amigos.
        /// </summary>
        public IListaAmigosServicio ListaAmigosServicio { get; }

        /// <summary>
        /// Obtiene el servicio de operaciones de amigos.
        /// </summary>
        public IAmigosServicio AmigosServicio { get; }

        /// <summary>
        /// Obtiene el servicio de salas.
        /// </summary>
        public ISalasServicio SalasServicio { get; }

        /// <summary>
        /// Obtiene el manejador de sonidos.
        /// </summary>
        public SonidoManejador SonidoManejador { get; }

        /// <summary>
        /// Obtiene el usuario autenticado actual.
        /// </summary>
        public IUsuarioAutenticado UsuarioSesion { get; }
    }
}
