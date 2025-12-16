using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias de servicios para el perfil de usuario.
    /// </summary>
    /// <remarks>
    /// Incluye servicios de perfil, avatar, contrasena y catalogos.
    /// </remarks>
    public class DependenciasPerfil
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias de perfil.
        /// </summary>
        /// <param name="perfilServicio">Servicio de perfil.</param>
        /// <param name="seleccionarAvatarServicio">
        /// Servicio de seleccion de avatar.
        /// </param>
        /// <param name="cambioContrasenaServicio">
        /// Servicio de cambio de contrasena.
        /// </param>
        /// <param name="recuperacionCuentaServicio">
        /// Servicio de recuperacion de cuenta.
        /// </param>
        /// <param name="usuarioSesion">Usuario autenticado actual.</param>
        /// <param name="catalogoAvatares">Catalogo de avatares.</param>
        /// <param name="catalogoPerfil">Catalogo de imagenes de perfil.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        public DependenciasPerfil(
            IPerfilServicio perfilServicio,
            ISeleccionarAvatarServicio seleccionarAvatarServicio,
            ICambioContrasenaServicio cambioContrasenaServicio,
            IRecuperacionCuentaServicio recuperacionCuentaServicio,
            IUsuarioAutenticado usuarioSesion,
            ICatalogoAvatares catalogoAvatares,
            ICatalogoImagenesPerfil catalogoPerfil)
        {
            PerfilServicio = perfilServicio ?? 
                throw new ArgumentNullException(nameof(perfilServicio));
            SeleccionarAvatarServicio = seleccionarAvatarServicio ?? 
                throw new ArgumentNullException(nameof(seleccionarAvatarServicio));
            CambioContrasenaServicio = cambioContrasenaServicio ?? 
                throw new ArgumentNullException(nameof(cambioContrasenaServicio));
            RecuperacionCuentaServicio = recuperacionCuentaServicio ?? 
                throw new ArgumentNullException(nameof(recuperacionCuentaServicio));
            UsuarioSesion = usuarioSesion ?? 
                throw new ArgumentNullException(nameof(usuarioSesion));
            CatalogoAvatares = catalogoAvatares ?? 
                throw new ArgumentNullException(nameof(catalogoAvatares));
            CatalogoPerfil = catalogoPerfil ?? 
                throw new ArgumentNullException(nameof(catalogoPerfil));
        }

        /// <summary>
        /// Servicio para gestionar el perfil del usuario.
        /// </summary>
        public IPerfilServicio PerfilServicio { get; }

        /// <summary>
        /// Servicio para seleccionar avatares.
        /// </summary>
        public ISeleccionarAvatarServicio SeleccionarAvatarServicio { get; }

        /// <summary>
        /// Servicio para cambiar la contrasena.
        /// </summary>
        public ICambioContrasenaServicio CambioContrasenaServicio { get; }

        /// <summary>
        /// Servicio de dialogo para recuperacion de cuenta.
        /// </summary>
        public IRecuperacionCuentaServicio RecuperacionCuentaServicio { get; }

        /// <summary>
        /// Datos del usuario autenticado actualmente.
        /// </summary>
        public IUsuarioAutenticado UsuarioSesion { get; }

        /// <summary>
        /// Catalogo de avatares disponibles.
        /// </summary>
        public ICatalogoAvatares CatalogoAvatares { get; }

        /// <summary>
        /// Catalogo de imagenes de perfil.
        /// </summary>
        public ICatalogoImagenesPerfil CatalogoPerfil { get; }
    }
}
