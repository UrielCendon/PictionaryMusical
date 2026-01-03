using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Auxiliares;
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

            var dependencias = new VentanaPrincipalDependencias(
                _localizacionServicio,
                App.ListaAmigosServicio,
                App.AmigosServicio,
                App.SalasServicio,
                _sonidoManejador,
                _usuarioSesion);

            var principalVistaModelo = new VentanaPrincipal.VentanaPrincipalVistaModelo(
                _ventana,
                _localizador,
                dependencias);

            _ventana.MostrarVentana(principalVistaModelo);
            _ventana.CerrarVentana(vistaModeloActual);
        }

        /// <summary>
        /// Navega a una sala de juego.
        /// </summary>
        /// <param name="parametros">Parámetros de navegación a la sala.</param>
        public void NavegarAVentanaSala(NavegacionSalaParametros parametros)
        {
            if (parametros == null)
            {
                throw new ArgumentNullException(nameof(parametros));
            }

            App.MusicaManejador.Detener();

            var comunicacion = new ComunicacionSalaDependencias(
                parametros.Servicio,
                App.InvitacionesServicio,
                new ClienteServicios.Wcf.InvitacionSalaServicio(
                    App.InvitacionesServicio,
                    App.ListaAmigosServicio,
                    App.PerfilServicio,
                    _sonidoManejador,
                    _avisoServicio,
                    _localizador),
                App.WcfFabrica);

            var perfiles = new PerfilesSalaDependencias(
                App.ListaAmigosServicio,
                App.PerfilServicio,
                App.ReportesServicio,
                _usuarioSesion);

            var audio = new AudioSalaDependencias(
                _sonidoManejador,
                new CancionManejador(),
                App.CatalogoCanciones);

            var dependenciasSala = new SalaVistaModeloDependencias(
                comunicacion,
                perfiles,
                audio,
                _avisoServicio);

            var salaVistaModelo = new SalaVistaModelo(
                _ventana,
                _localizador,
                parametros.Sala,
                dependenciasSala,
                parametros.NombreJugador,
                parametros.EsInvitado);

            _ventana.MostrarVentana(salaVistaModelo);
            _ventana.CerrarVentana(parametros.VistaModeloActual);
        }
    }
}
