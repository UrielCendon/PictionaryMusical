using log4net;
using System;
using System.Net.NetworkInformation;

namespace PictionaryMusicalCliente.Utilidades
{
    /// <summary>
    /// Monitor de conectividad de red que detecta cambios en la disponibilidad 
    /// de conexión a internet de forma reactiva.
    /// </summary>
    public sealed class ConectividadRedMonitor : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Lazy<ConectividadRedMonitor> _instanciaLazy =
            new Lazy<ConectividadRedMonitor>(() => new ConectividadRedMonitor());

        private bool _ultimoEstadoConectado;
        private bool _dispuesto;

        /// <summary>
        /// Evento disparado cuando se detecta pérdida de conexión a internet.
        /// </summary>
        public event EventHandler ConexionPerdida;

        /// <summary>
        /// Evento disparado cuando se restaura la conexión a internet.
        /// </summary>
        public event EventHandler ConexionRestaurada;

        /// <summary>
        /// Obtiene la instancia única del monitor de conectividad.
        /// </summary>
        public static ConectividadRedMonitor Instancia => _instanciaLazy.Value;

        private ConectividadRedMonitor()
        {
            _ultimoEstadoConectado = NetworkInterface.GetIsNetworkAvailable();
            NetworkChange.NetworkAvailabilityChanged += OnCambioDisponibilidadRed;
            _logger.InfoFormat(
                "Monitor de conectividad iniciado. Estado inicial: {0}",
                _ultimoEstadoConectado ? "Conectado" : "Desconectado");
        }

        /// <summary>
        /// Indica si actualmente hay conexión de red disponible.
        /// </summary>
        public static bool HayConexion => NetworkInterface.GetIsNetworkAvailable();

        private void OnCambioDisponibilidadRed(
            object remitente, 
            NetworkAvailabilityEventArgs argumentos)
        {
            bool conectadoAhora = argumentos.IsAvailable;

            _logger.InfoFormat(
                "Cambio de estado de red detectado. Conectado: {0}", 
                conectadoAhora);

            if (_ultimoEstadoConectado && !conectadoAhora)
            {
                _logger.Warn("Se perdió la conexión a internet.");
                _ultimoEstadoConectado = false;
                ConexionPerdida?.Invoke(this, EventArgs.Empty);
            }
            else if (!_ultimoEstadoConectado && conectadoAhora)
            {
                _logger.Info("Se restauró la conexión a internet.");
                _ultimoEstadoConectado = true;
                ConexionRestaurada?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Verifica manualmente el estado de la conexión y dispara el evento si
        /// hay una diferencia con el último estado conocido.
        /// </summary>
        public void VerificarEstadoConexion()
        {
            bool conectadoAhora = NetworkInterface.GetIsNetworkAvailable();

            if (_ultimoEstadoConectado && !conectadoAhora)
            {
                _logger.Warn("Verificación manual: Se detectó pérdida de conexión.");
                _ultimoEstadoConectado = false;
                ConexionPerdida?.Invoke(this, EventArgs.Empty);
            }
            else if (!_ultimoEstadoConectado && conectadoAhora)
            {
                _logger.Info("Verificación manual: Se detectó restauración de conexión.");
                _ultimoEstadoConectado = true;
                ConexionRestaurada?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Libera los recursos del monitor de conectividad.
        /// </summary>
        public void Dispose()
        {
            if (!_dispuesto)
            {
                NetworkChange.NetworkAvailabilityChanged -= OnCambioDisponibilidadRed;
                _dispuesto = true;
                _logger.Info("Monitor de conectividad liberado.");
            }
        }
    }
}
