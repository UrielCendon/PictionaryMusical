using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using PictionaryMusicalCliente.VistaModelo.Salas;
using System;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.InicioSesion.Auxiliares
{
    /// <summary>
    /// Gestiona la navegacion desde la pantalla de inicio de sesion.
    /// </summary>
    public sealed class InicioSesionNavegacion
    {
        private readonly IVentanaServicio _ventana;
        private readonly ILocalizadorServicio _localizador;
        private readonly ILocalizacionServicio _localizacionServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _avisoServicio;
        private readonly IUsuarioAutenticado _usuarioSesion;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="InicioSesionNavegacion"/>.
        /// </summary>
        public InicioSesionNavegacion(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            ILocalizacionServicio localizacionServicio,
            SonidoManejador sonidoManejador,
            IAvisoServicio avisoServicio,
            IUsuarioAutenticado usuarioSesion)
        {
            _ventana = ventana ?? 
                throw new ArgumentNullException(nameof(ventana));
            _localizador = localizador ?? 
                throw new ArgumentNullException(nameof(localizador));
            _localizacionServicio = localizacionServicio;
            _sonidoManejador = sonidoManejador ?? 
                throw new ArgumentNullException(nameof(sonidoManejador));
            _avisoServicio = avisoServicio ?? 
                throw new ArgumentNullException(nameof(avisoServicio));
            _usuarioSesion = usuarioSesion ?? 
                throw new ArgumentNullException(nameof(usuarioSesion));
        }

        /// <summary>
        /// Navega a la ventana principal de la aplicacion.
        /// </summary>
        /// <param name="vistaModeloActual">
        /// VistaModelo actual para cerrar.
        /// </param>
        public void NavegarAVentanaPrincipal(object vistaModeloActual)
        {
            App.MusicaManejador.Detener();

            var principalVistaModelo = new VentanaPrincipal.VentanaPrincipalVistaModelo(
                _ventana,
                _localizador,
                _localizacionServicio,
                App.ListaAmigosServicio,
                App.AmigosServicio,
                App.SalasServicio,
                _sonidoManejador,
                _usuarioSesion);

            _ventana.MostrarVentana(principalVistaModelo);
            _ventana.CerrarVentana(vistaModeloActual);
        }

        /// <summary>
        /// Navega a una sala de juego.
        /// </summary>
        /// <param name="sala">Datos de la sala.</param>
        /// <param name="servicio">Servicio de salas.</param>
        /// <param name="nombreJugador">Nombre del jugador.</param>
        /// <param name="esInvitado">Indica si es invitado.</param>
        /// <param name="vistaModeloActual">
        /// VistaModelo actual para cerrar.
        /// </param>
        public void NavegarAVentanaSala(
            DTOs.SalaDTO sala,
            ISalasServicio servicio,
            string nombreJugador,
            bool esInvitado,
            object vistaModeloActual)
        {
            App.MusicaManejador.Detener();

            var comunicacion = new DependenciasComunicacionSala(
                servicio,
                App.InvitacionesServicio,
                new ClienteServicios.Wcf.InvitacionSalaServicio(
                    App.InvitacionesServicio,
                    App.ListaAmigosServicio,
                    App.PerfilServicio,
                    _sonidoManejador,
                    _avisoServicio),
                App.WcfFabrica);

            var perfiles = new DependenciasPerfilesSala(
                App.ListaAmigosServicio,
                App.PerfilServicio,
                App.ReportesServicio,
                _usuarioSesion);

            var audio = new DependenciasAudioSala(
                _sonidoManejador,
                new CancionManejador(),
                App.CatalogoCanciones);

            var dependenciasSala = new DependenciasSalaVistaModelo(
                comunicacion,
                perfiles,
                audio,
                _avisoServicio);

            var salaVistaModelo = new SalaVistaModelo(
                _ventana,
                _localizador,
                sala,
                dependenciasSala,
                nombreJugador,
                esInvitado);

            _ventana.MostrarVentana(salaVistaModelo);
            _ventana.CerrarVentana(vistaModeloActual);
        }
    }
}
