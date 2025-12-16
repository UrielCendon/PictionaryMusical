using System;
using System.Windows;
using System.Windows.Threading;

namespace PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares
{
    /// <summary>
    /// Gestiona los temporizadores de la partida.
    /// </summary>
    public sealed class PartidaTemporizadores : IDisposable
    {
        private DispatcherTimer _overlayTimer;
        private DispatcherTimer _temporizadorAlarma;
        private DispatcherTimer _temporizador;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="PartidaTemporizadores"/>.
        /// </summary>
        public PartidaTemporizadores()
        {
            InicializarTemporizadores();
        }

        /// <summary>
        /// Evento que se dispara en cada tick del overlay.
        /// </summary>
        public event EventHandler OverlayTick;

        /// <summary>
        /// Evento que se dispara en cada tick de alarma.
        /// </summary>
        public event EventHandler AlarmaTick;

        /// <summary>
        /// Evento que se dispara en cada tick del temporizador principal.
        /// </summary>
        public event EventHandler TemporizadorTick;

        /// <summary>
        /// Inicia el temporizador de overlay.
        /// </summary>
        public void IniciarOverlay()
        {
            _overlayTimer?.Start();
        }

        /// <summary>
        /// Detiene el temporizador de overlay.
        /// </summary>
        public void DetenerOverlay()
        {
            _overlayTimer?.Stop();
        }

        /// <summary>
        /// Inicia el temporizador de alarma.
        /// </summary>
        public void IniciarAlarma()
        {
            _temporizadorAlarma?.Start();
        }

        /// <summary>
        /// Detiene el temporizador de alarma.
        /// </summary>
        public void DetenerAlarma()
        {
            _temporizadorAlarma?.Stop();
        }

        /// <summary>
        /// Inicia el temporizador principal.
        /// </summary>
        public void IniciarTemporizador()
        {
            _temporizador?.Start();
        }

        /// <summary>
        /// Detiene el temporizador principal.
        /// </summary>
        public void DetenerTemporizador()
        {
            _temporizador?.Stop();
        }

        /// <summary>
        /// Detiene todos los temporizadores.
        /// </summary>
        public void DetenerTodos()
        {
            DetenerOverlay();
            DetenerAlarma();
            DetenerTemporizador();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DetenerTodos();
            _overlayTimer = null;
            _temporizadorAlarma = null;
            _temporizador = null;
        }

        private void InicializarTemporizadores()
        {
            var uiDispatcher = Application.Current?.Dispatcher
                ?? Dispatcher.CurrentDispatcher;

            _overlayTimer = new DispatcherTimer(
                DispatcherPriority.Normal, 
                uiDispatcher)
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _overlayTimer.Tick += (s, e) => OverlayTick?.Invoke(s, e);

            _temporizadorAlarma = new DispatcherTimer(
                DispatcherPriority.Normal, 
                uiDispatcher)
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _temporizadorAlarma.Tick += (s, e) => AlarmaTick?.Invoke(s, e);

            _temporizador = new DispatcherTimer(
                DispatcherPriority.Normal, 
                uiDispatcher)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _temporizador.Tick += (s, e) => TemporizadorTick?.Invoke(s, e);
        }
    }
}
