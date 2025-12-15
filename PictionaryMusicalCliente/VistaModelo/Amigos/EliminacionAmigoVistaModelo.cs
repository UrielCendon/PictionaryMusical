using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using log4net;
using System;
using System.Windows.Input;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Utilidades;

namespace PictionaryMusicalCliente.VistaModelo.Amigos
{
    /// <summary>
    /// ViewModel para el cuadro de dialogo de confirmacion de eliminacion de amigo.
    /// </summary>
    public class EliminacionAmigoVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SonidoManejador _sonidoManejador;
        private readonly string _nombreAmigo;

        /// <summary>
        /// Inicializa el ViewModel para confirmar la eliminacion de un amigo.
        /// </summary>
        /// <param name="ventana">Servicio para gestionar ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <param name="nombreAmigo">Nombre del amigo a eliminar.</param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="sonidoManejador"/> es nulo.
        /// </exception>
        public EliminacionAmigoVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            SonidoManejador sonidoManejador,
            string nombreAmigo)
            : base(ventana, localizador)
        {
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            _nombreAmigo = nombreAmigo;

            MensajeConfirmacion = CrearMensajeConfirmacion(nombreAmigo);

            AceptarComando = new ComandoDelegado(_ => EjecutarAceptar());
            CancelarComando = new ComandoDelegado(_ => EjecutarCancelar());
        }

        /// <summary>
        /// Mensaje de confirmacion a mostrar al usuario.
        /// </summary>
        public string MensajeConfirmacion { get; }

        /// <summary>
        /// Comando para confirmar la eliminacion del amigo.
        /// </summary>
        public ICommand AceptarComando { get; }

        /// <summary>
        /// Comando para cancelar la eliminacion.
        /// </summary>
        public ICommand CancelarComando { get; }

        /// <summary>
        /// Resultado del dialogo (true si confirmo, false si cancelo).
        /// </summary>
        public bool? DialogResult { get; private set; }

        private static string CrearMensajeConfirmacion(string nombreAmigo)
        {
            return string.IsNullOrWhiteSpace(nombreAmigo)
                ? Lang.eliminarAmigoTextoConfirmacion
                : string.Concat(
                    Lang.eliminarAmigoTextoConfirmacion, 
                    nombreAmigo, 
                    "?");
        }

        private void EjecutarAceptar()
        {
            _sonidoManejador.ReproducirClick();
            RegistrarConfirmacion();
            ConfirmarYCerrar();
        }

        private void RegistrarConfirmacion()
        {
            _logger.InfoFormat(
                "Usuario confirmo la eliminacion del amigo: {0}",
                _nombreAmigo);
        }

        private void ConfirmarYCerrar()
        {
            DialogResult = true;
            _ventana.CerrarVentana(this);
        }

        private void EjecutarCancelar()
        {
            _sonidoManejador.ReproducirClick();
            RegistrarCancelacion();
            CancelarYCerrar();
        }

        private void RegistrarCancelacion()
        {
            _logger.InfoFormat(
                "Usuario cancelo la eliminacion del amigo: {0}",
                _nombreAmigo);
        }

        private void CancelarYCerrar()
        {
            DialogResult = false;
            _ventana.CerrarVentana(this);
        }
    }
}