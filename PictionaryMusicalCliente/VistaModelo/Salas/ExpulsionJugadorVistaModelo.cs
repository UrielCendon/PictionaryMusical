using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using System;
using System.Windows.Input;

namespace PictionaryMusicalCliente.VistaModelo.Salas
{
    /// <summary>
    /// Controla la logica de la ventana de confirmacion para expulsar un jugador.
    /// </summary>
    public class ExpulsionJugadorVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SonidoManejador _sonidoManejador;

        /// <summary>
        /// Inicializa el ViewModel para confirmar la expulsion de un jugador.
        /// </summary>
        /// <param name="ventana">Servicio para gestionar ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <param name="mensajeConfirmacion">Mensaje a mostrar en el dialogo.</param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="sonidoManejador"/> es nulo.
        /// </exception>
        public ExpulsionJugadorVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            SonidoManejador sonidoManejador,
            string mensajeConfirmacion)
            : base(ventana, localizador)
        {
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));

            MensajeConfirmacion = ObtenerMensajeConfirmacion(mensajeConfirmacion);

            ConfirmarComando = new ComandoDelegado(_ => EjecutarConfirmar());
            CancelarComando = new ComandoDelegado(_ => EjecutarCancelar());
        }

        /// <summary>
        /// Mensaje de confirmacion a mostrar al usuario.
        /// </summary>
        public string MensajeConfirmacion { get; }

        /// <summary>
        /// Comando para confirmar la expulsion.
        /// </summary>
        public ICommand ConfirmarComando { get; }

        /// <summary>
        /// Comando para cancelar la expulsion.
        /// </summary>
        public ICommand CancelarComando { get; }

        /// <summary>
        /// Resultado del dialogo (true si confirmo, false si cancelo).
        /// </summary>
        public bool? DialogResult { get; private set; }

        private static string ObtenerMensajeConfirmacion(string mensaje)
        {
            return string.IsNullOrWhiteSpace(mensaje)
                ? Lang.expulsarTextoConfirmacion
                : mensaje;
        }

        private void EjecutarConfirmar()
        {
            _sonidoManejador.ReproducirClick();
            DialogResult = true;
            _ventana.CerrarVentana(this);
        }

        private void EjecutarCancelar()
        {
            _sonidoManejador.ReproducirClick();
            DialogResult = false;
            _ventana.CerrarVentana(this);
        }
    }
}