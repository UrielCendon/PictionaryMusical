using System;
using System.Windows.Input;
using log4net;
using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Properties.Langs;

namespace PictionaryMusicalCliente.VistaModelo.Salas
{
    /// <summary>
    /// Controla la lógica de la ventana para reportar a un jugador.
    /// </summary>
    public class ReportarJugadorVistaModelo : BaseVistaModelo
    {
        private static readonly ILog Logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _motivo;
        private string _mensajeError;

        /// <summary>
        /// Inicializa la instancia con el nombre del jugador a reportar.
        /// </summary>
        public ReportarJugadorVistaModelo(string nombreJugador)
        {
            NombreJugador = nombreJugador;

            ReportarComando = new ComandoDelegado(_ =>
            {
                SonidoManejador.ReproducirClick();
                if (string.IsNullOrWhiteSpace(Motivo))
                {
                    MensajeError = Lang.reportarJugadorTextoMotivoRequerido;
                    return;
                }

                if (Motivo.Trim().Length > 100)
                {
                    MensajeError = Lang.reportarJugadorTextoMotivoLongitud;
                    return;
                }

                Logger.InfoFormat("Usuario confirmó reporte contra: {0}", NombreJugador);
                MensajeError = string.Empty;
                Cerrar?.Invoke(true);
            });

            CancelarComando = new ComandoDelegado(_ =>
            {
                SonidoManejador.ReproducirClick();
                Logger.Info("Usuario canceló el reporte de jugador.");
                Cerrar?.Invoke(false);
            });
        }

        /// <summary>
        /// Nombre del jugador que será reportado.
        /// </summary>
        public string NombreJugador { get; }

        /// <summary>
        /// Mensaje descriptivo que indica a quién se reportará.
        /// </summary>
        public string DescripcionReporte => string.Format(
            Lang.reportarJugadorTextoDescripcion,
            NombreJugador);

        /// <summary>
        /// Motivo descrito por el usuario.
        /// </summary>
        public string Motivo
        {
            get => _motivo;
            set => EstablecerPropiedad(ref _motivo, value);
        }

        /// <summary>
        /// Mensaje de error a mostrar al usuario.
        /// </summary>
        public string MensajeError
        {
            get => _mensajeError;
            private set => EstablecerPropiedad(ref _mensajeError, value);
        }

        /// <summary>
        /// Comando para confirmar y enviar el reporte.
        /// </summary>
        public ICommand ReportarComando { get; }

        /// <summary>
        /// Comando para cancelar y cerrar la ventana.
        /// </summary>
        public ICommand CancelarComando { get; }

        /// <summary>
        /// Acción para cerrar la ventana y retornar el resultado.
        /// </summary>
        public Action<bool?> Cerrar { get; set; }
    }
}