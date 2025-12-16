using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Utilidades;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias de comunicacion de sala de juego.
    /// </summary>
    /// <remarks>
    /// Incluye servicios de sala, invitaciones, chat y comunicacion WCF.
    /// </remarks>
    public class DependenciasComunicacionSala
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias de comunicacion.
        /// </summary>
        /// <param name="salasServicio">Servicio de gestion de salas.</param>
        /// <param name="invitacionesServicio">Servicio de invitaciones.</param>
        /// <param name="invitacionSalaServicio">
        /// Servicio de invitacion en sala.
        /// </param>
        /// <param name="fabricaClientes">Fabrica de clientes WCF.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        public DependenciasComunicacionSala(
            ISalasServicio salasServicio,
            IInvitacionesServicio invitacionesServicio,
            IInvitacionSalaServicio invitacionSalaServicio,
            IWcfClienteFabrica fabricaClientes)
        {
            SalasServicio = salasServicio ?? 
                throw new ArgumentNullException(nameof(salasServicio));
            InvitacionesServicio = invitacionesServicio ?? 
                throw new ArgumentNullException(nameof(invitacionesServicio));
            InvitacionSalaServicio = invitacionSalaServicio ?? 
                throw new ArgumentNullException(nameof(invitacionSalaServicio));
            FabricaClientes = fabricaClientes ?? 
                throw new ArgumentNullException(nameof(fabricaClientes));
        }

        /// <summary>
        /// Servicio para gestionar salas de juego.
        /// </summary>
        public ISalasServicio SalasServicio { get; }

        /// <summary>
        /// Servicio para enviar invitaciones a jugadores.
        /// </summary>
        public IInvitacionesServicio InvitacionesServicio { get; }

        /// <summary>
        /// Servicio para manejar invitaciones dentro de la sala.
        /// </summary>
        public IInvitacionSalaServicio InvitacionSalaServicio { get; }

        /// <summary>
        /// Fabrica para crear clientes WCF.
        /// </summary>
        public IWcfClienteFabrica FabricaClientes { get; }
    }

    /// <summary>
    /// Agrupa las dependencias de perfiles y reportes para la sala.
    /// </summary>
    public class DependenciasPerfilesSala
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias de perfiles.
        /// </summary>
        /// <param name="listaAmigosServicio">Servicio de lista de amigos.</param>
        /// <param name="perfilServicio">Servicio de perfil de usuario.</param>
        /// <param name="reportesServicio">Servicio de reportes.</param>
        /// <param name="usuarioSesion">Datos del usuario autenticado.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        public DependenciasPerfilesSala(
            IListaAmigosServicio listaAmigosServicio,
            IPerfilServicio perfilServicio,
            IReportesServicio reportesServicio,
            IUsuarioAutenticado usuarioSesion)
        {
            ListaAmigosServicio = listaAmigosServicio ?? 
                throw new ArgumentNullException(nameof(listaAmigosServicio));
            PerfilServicio = perfilServicio ?? 
                throw new ArgumentNullException(nameof(perfilServicio));
            ReportesServicio = reportesServicio ?? 
                throw new ArgumentNullException(nameof(reportesServicio));
            UsuarioSesion = usuarioSesion ?? 
                throw new ArgumentNullException(nameof(usuarioSesion));
        }

        /// <summary>
        /// Servicio para obtener la lista de amigos.
        /// </summary>
        public IListaAmigosServicio ListaAmigosServicio { get; }

        /// <summary>
        /// Servicio para obtener perfiles de usuario.
        /// </summary>
        public IPerfilServicio PerfilServicio { get; }

        /// <summary>
        /// Servicio para generar y enviar reportes.
        /// </summary>
        public IReportesServicio ReportesServicio { get; }

        /// <summary>
        /// Datos del usuario autenticado actualmente.
        /// </summary>
        public IUsuarioAutenticado UsuarioSesion { get; }
    }

    /// <summary>
    /// Agrupa las dependencias de audio para la sala de juego.
    /// </summary>
    public class DependenciasAudioSala
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias de audio.
        /// </summary>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <param name="cancionManejador">Manejador de reproduccion de canciones.</param>
        /// <param name="catalogoCanciones">Catalogo de canciones disponibles.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        public DependenciasAudioSala(
            SonidoManejador sonidoManejador,
            CancionManejador cancionManejador,
            ICatalogoCanciones catalogoCanciones)
        {
            SonidoManejador = sonidoManejador ?? 
                throw new ArgumentNullException(nameof(sonidoManejador));
            CancionManejador = cancionManejador ?? 
                throw new ArgumentNullException(nameof(cancionManejador));
            CatalogoCanciones = catalogoCanciones ?? 
                throw new ArgumentNullException(nameof(catalogoCanciones));
        }

        /// <summary>
        /// Manejador de efectos de sonido de la aplicacion.
        /// </summary>
        public SonidoManejador SonidoManejador { get; }

        /// <summary>
        /// Manejador para reproducir canciones durante el juego.
        /// </summary>
        public CancionManejador CancionManejador { get; }

        /// <summary>
        /// Catalogo de canciones disponibles para el juego.
        /// </summary>
        public ICatalogoCanciones CatalogoCanciones { get; }
    }

    /// <summary>
    /// Agrupa todas las dependencias requeridas para el ViewModel de sala.
    /// </summary>
    /// <remarks>
    /// Combina dependencias de comunicacion, perfiles y audio en un solo objeto.
    /// </remarks>
    public class DependenciasSalaVistaModelo
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias de sala.
        /// </summary>
        /// <param name="comunicacion">Dependencias de comunicacion.</param>
        /// <param name="perfiles">Dependencias de perfiles y reportes.</param>
        /// <param name="audio">Dependencias de audio.</param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        public DependenciasSalaVistaModelo(
            DependenciasComunicacionSala comunicacion,
            DependenciasPerfilesSala perfiles,
            DependenciasAudioSala audio,
            IAvisoServicio avisoServicio)
        {
            Comunicacion = comunicacion ?? 
                throw new ArgumentNullException(nameof(comunicacion));
            Perfiles = perfiles ?? 
                throw new ArgumentNullException(nameof(perfiles));
            Audio = audio ?? 
                throw new ArgumentNullException(nameof(audio));
            AvisoServicio = avisoServicio ?? 
                throw new ArgumentNullException(nameof(avisoServicio));
        }

        /// <summary>
        /// Dependencias relacionadas con comunicacion de sala.
        /// </summary>
        public DependenciasComunicacionSala Comunicacion { get; }

        /// <summary>
        /// Dependencias relacionadas con perfiles y reportes.
        /// </summary>
        public DependenciasPerfilesSala Perfiles { get; }

        /// <summary>
        /// Dependencias relacionadas con audio y canciones.
        /// </summary>
        public DependenciasAudioSala Audio { get; }

        /// <summary>
        /// Servicio para mostrar avisos y notificaciones al usuario.
        /// </summary>
        public IAvisoServicio AvisoServicio { get; }
    }
}
