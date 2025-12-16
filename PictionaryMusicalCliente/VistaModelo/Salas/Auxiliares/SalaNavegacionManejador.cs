using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using PictionaryMusicalCliente.VistaModelo.InicioSesion;
using PictionaryMusicalCliente.VistaModelo.VentanaPrincipal;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares
{
    /// <summary>
    /// Gestiona la navegacion desde la sala de juego hacia otras vistas.
    /// </summary>
    public sealed class SalaNavegacionManejador
    {
        private readonly IVentanaServicio _ventana;
        private readonly ILocalizadorServicio _localizador;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _avisoServicio;
        private readonly IUsuarioAutenticado _usuarioSesion;
        private readonly bool _esInvitado;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="SalaNavegacionManejador"/>.
        /// </summary>
        /// <param name="ventana">Servicio de ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <param name="usuarioSesion">Usuario autenticado actual.</param>
        /// <param name="esInvitado">Indica si el usuario es invitado.</param>
        public SalaNavegacionManejador(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            SonidoManejador sonidoManejador,
            IAvisoServicio avisoServicio,
            IUsuarioAutenticado usuarioSesion,
            bool esInvitado)
        {
            _ventana = ventana ?? 
                throw new ArgumentNullException(nameof(ventana));
            _localizador = localizador ?? 
                throw new ArgumentNullException(nameof(localizador));
            _sonidoManejador = sonidoManejador ?? 
                throw new ArgumentNullException(nameof(sonidoManejador));
            _avisoServicio = avisoServicio ?? 
                throw new ArgumentNullException(nameof(avisoServicio));
            _usuarioSesion = usuarioSesion ?? 
                throw new ArgumentNullException(nameof(usuarioSesion));
            _esInvitado = esInvitado;
        }

        /// <summary>
        /// Accion para cerrar la ventana actual.
        /// </summary>
        public Action CerrarVentana { get; set; }

        /// <summary>
        /// Obtiene el destino de navegacion segun el estado de la sesion.
        /// </summary>
        /// <returns>El destino de navegacion.</returns>
        public SalaVistaModelo.DestinoNavegacion ObtenerDestinoSegunSesion()
        {
            bool sesionActiva = _usuarioSesion?.EstaAutenticado == true && 
                !_esInvitado;
            return sesionActiva
                ? SalaVistaModelo.DestinoNavegacion.VentanaPrincipal
                : SalaVistaModelo.DestinoNavegacion.InicioSesion;
        }

        /// <summary>
        /// Navega al destino especificado.
        /// </summary>
        /// <param name="destino">Destino de navegacion.</param>
        public void Navegar(SalaVistaModelo.DestinoNavegacion destino)
        {
            if (destino == SalaVistaModelo.DestinoNavegacion.InicioSesion)
            {
                NavegarAInicioSesion();
            }
            else
            {
                NavegarAVentanaPrincipal();
            }

            CerrarVentana?.Invoke();
        }

        private void NavegarAInicioSesion()
        {
            _usuarioSesion.Limpiar();

            var dependenciasBase = new DependenciasVistaModeloBase(
                _ventana,
                _localizador,
                _sonidoManejador,
                _avisoServicio);

            var dependenciasInicioSesion = new DependenciasInicioSesion(
                App.InicioSesionServicio,
                App.CambioContrasenaServicio,
                App.RecuperacionCuentaServicio,
                App.ServicioIdioma,
                App.GeneradorNombres,
                _usuarioSesion,
                App.FabricaSalas);

            var inicioVistaModelo = new InicioSesionVistaModelo(
                dependenciasBase,
                dependenciasInicioSesion);
            _ventana.MostrarVentana(inicioVistaModelo);
        }

        private void NavegarAVentanaPrincipal()
        {
            var principalVistaModelo = new VentanaPrincipalVistaModelo(
                _ventana,
                _localizador,
                App.ServicioIdioma,
                App.ListaAmigosServicio,
                App.AmigosServicio,
                App.SalasServicio,
                _sonidoManejador,
                _usuarioSesion);
            _ventana.MostrarVentana(principalVistaModelo);
        }
    }
}
