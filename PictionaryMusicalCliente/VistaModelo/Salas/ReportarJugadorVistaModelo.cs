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
    /// Controla la logica de la ventana para reportar a un jugador.
    /// </summary>
    public class ReportarJugadorVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SonidoManejador _sonidoManejador;
        private readonly string _nombreJugador;
        private string _motivo;
        private string _mensajeError;

        /// <summary>
        /// Inicializa el ViewModel para reportar a un jugador.
        /// </summary>
        /// <param name="ventana">Servicio para gestionar ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <param name="nombreJugador">Nombre del jugador a reportar.</param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="sonidoManejador"/> es nulo.
        /// </exception>
        public ReportarJugadorVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            SonidoManejador sonidoManejador,
            string nombreJugador)
            : base(ventana, localizador)
        {
            _nombreJugador = nombreJugador;
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));

            ReportarComando = new ComandoDelegado(_ => EjecutarReportar());
            CancelarComando = new ComandoDelegado(_ => EjecutarCancelar());
        }

        /// <summary>
        /// Nombre del jugador que sera reportado.
        /// </summary>
        public string NombreJugador => _nombreJugador;

        /// <summary>
        /// Descripcion formateada del reporte.
        /// </summary>
        public string DescripcionReporte => string.Format(
            Lang.reportarJugadorTextoDescripcion,
            _nombreJugador);

        /// <summary>
        /// Motivo del reporte ingresado por el usuario.
        /// </summary>
        public string Motivo
        {
            get => _motivo;
            set => EstablecerPropiedad(ref _motivo, value);
        }

        /// <summary>
        /// Mensaje de error a mostrar si la validacion falla.
        /// </summary>
        public string MensajeError
        {
            get => _mensajeError;
            private set => EstablecerPropiedad(ref _mensajeError, value);
        }

        /// <summary>
        /// Comando para confirmar el reporte.
        /// </summary>
        public ICommand ReportarComando { get; }

        /// <summary>
        /// Comando para cancelar el reporte.
        /// </summary>
        public ICommand CancelarComando { get; }

        /// <summary>
        /// Resultado del dialogo (true si confirmo, false si cancelo).
        /// </summary>
        public bool? DialogResult { get; private set; }

        /// <summary>
        /// Motivo del reporte para uso externo.
        /// </summary>
        public string MotivoReporte => Motivo;

        private void EjecutarReportar()
        {
            _sonidoManejador.ReproducirClick();

            if (!ValidarMotivo())
            {
                return;
            }

            ConfirmarReporte();
        }

        private bool ValidarMotivo()
        {
            if (string.IsNullOrWhiteSpace(Motivo))
            {
                MensajeError = Lang.reportarJugadorTextoMotivoRequerido;
                return false;
            }

            return true;
        }

        private void ConfirmarReporte()
        {
            MensajeError = string.Empty;
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