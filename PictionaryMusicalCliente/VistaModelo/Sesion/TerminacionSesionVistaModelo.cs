using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using log4net;
using System;
using System.Windows.Input;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;

namespace PictionaryMusicalCliente.VistaModelo.Sesion
{
    /// <summary>
    /// Gestiona la logica para la confirmacion del cierre de sesion del usuario.
    /// </summary>
    public class TerminacionSesionVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUsuarioAutenticado _usuarioSesion;

        /// <summary>
        /// Obtiene o establece la accion a ejecutar para cerrar sesion y navegar.
        /// </summary>
        public Action EjecutarCierreSesionYNavegacion { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="TerminacionSesionVistaModelo"/>.
        /// </summary>
        /// <param name="ventana">Servicio de ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="usuarioSesion">Usuario autenticado actual.</param>
        public TerminacionSesionVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            IUsuarioAutenticado usuarioSesion)
            : base(ventana, localizador)
        {
            _usuarioSesion = usuarioSesion 
                ?? throw new ArgumentNullException(nameof(usuarioSesion));

            AceptarComando = new ComandoDelegado(_ => EjecutarAceptar());
            CancelarComando = new ComandoDelegado(_ => EjecutarCancelar());
        }

        /// <summary>
        /// Comando para confirmar el cierre de sesion.
        /// </summary>
        public ICommand AceptarComando { get; }

        /// <summary>
        /// Comando para cancelar la operacion y mantener la sesion activa.
        /// </summary>
        public ICommand CancelarComando { get; }

        private void EjecutarAceptar()
        {
            RegistrarConfirmacionCierreSesion();
            _usuarioSesion.Limpiar();
            EjecutarCierreSesionYNavegacion?.Invoke();
            _ventana.CerrarVentana(this);
        }

        private static void RegistrarConfirmacionCierreSesion()
        {
            _logger.Info("Usuario confirmo el cierre de sesion.");
        }

        private void EjecutarCancelar()
        {
            RegistrarCancelacionCierreSesion();
            _ventana.CerrarVentana(this);
        }

        private static void RegistrarCancelacionCierreSesion()
        {
            _logger.Info("Usuario cancelo el cierre de sesion.");
        }
    }
}