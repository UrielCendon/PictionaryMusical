using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using log4net;
using System;
using System.Threading.Tasks;
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
        private readonly IInicioSesionServicio _inicioSesionServicio;

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
        /// <param name="inicioSesionServicio">Servicio de inicio de sesion.</param>
        public TerminacionSesionVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            IUsuarioAutenticado usuarioSesion,
            IInicioSesionServicio inicioSesionServicio)
            : base(ventana, localizador)
        {
            _usuarioSesion = usuarioSesion 
                ?? throw new ArgumentNullException(nameof(usuarioSesion));
            _inicioSesionServicio = inicioSesionServicio
                ?? throw new ArgumentNullException(nameof(inicioSesionServicio));

            AceptarComando = new ComandoAsincrono(EjecutarComandoAceptarAsync);
            CancelarComando = new ComandoDelegado(EjecutarComandoCancelar);
        }

        /// <summary>
        /// Comando para confirmar el cierre de sesion.
        /// </summary>
        public ICommand AceptarComando { get; }

        /// <summary>
        /// Comando para cancelar la operacion y mantener la sesion activa.
        /// </summary>
        public ICommand CancelarComando { get; }

        private async Task EjecutarComandoAceptarAsync(object parametro)
        {
            await EjecutarAceptarAsync().ConfigureAwait(true);
        }

        private void EjecutarComandoCancelar(object parametro)
        {
            EjecutarCancelar();
        }

        private async Task EjecutarAceptarAsync()
        {
            RegistrarConfirmacionCierreSesion();
            await CerrarSesionEnServidorAsync().ConfigureAwait(true);
            _usuarioSesion.Limpiar();
            EjecutarCierreSesionYNavegacion?.Invoke();
            _ventana.CerrarVentana(this);
        }

        private async Task CerrarSesionEnServidorAsync()
        {
            string nombreUsuario = _usuarioSesion.NombreUsuario;

            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                await _inicioSesionServicio.CerrarSesionAsync(nombreUsuario)
                    .ConfigureAwait(false);
            }
        }

        private static void RegistrarConfirmacionCierreSesion()
        {
            _logger.Info("Usuario confirmo el cierre de sesion.");
        }

        private void EjecutarCancelar()
        {
            _ventana.CerrarVentana(this);
        }
    }
}