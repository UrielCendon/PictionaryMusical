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
        private const int SegundosIntervaloOverlay = 5;
        private const int SegundosIntervaloAlarma = 5;
        private const int SegundosIntervaloPrincipal = 1;

        private DispatcherTimer _capaMinutero;
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
            _capaMinutero?.Start();
        }

        /// <summary>
        /// Detiene el temporizador de overlay.
        /// </summary>
        public void DetenerOverlay()
        {
            _capaMinutero?.Stop();
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
            _capaMinutero = null;
            _temporizadorAlarma = null;
            _temporizador = null;
        }

        private void InicializarTemporizadores()
        {
            var uiDispatcher = Application.Current?.Dispatcher
                ?? Dispatcher.CurrentDispatcher;

            _capaMinutero = new DispatcherTimer(
                DispatcherPriority.Normal, 
                uiDispatcher)
            {
                Interval = TimeSpan.FromSeconds(SegundosIntervaloOverlay)
            };
            _capaMinutero.Tick += ManejarOverlayTick;

            _temporizadorAlarma = new DispatcherTimer(
                DispatcherPriority.Normal, 
                uiDispatcher)
            {
                Interval = TimeSpan.FromSeconds(SegundosIntervaloAlarma)
            };
            _temporizadorAlarma.Tick += ManejarAlarmaTick;

            _temporizador = new DispatcherTimer(
                DispatcherPriority.Normal, 
                uiDispatcher)
            {
                Interval = TimeSpan.FromSeconds(SegundosIntervaloPrincipal)
            };
            _temporizador.Tick += ManejarTemporizadorTick;
        }

        private void ManejarOverlayTick(object remitente, EventArgs argumentosEvento)
        {
            OverlayTick?.Invoke(remitente, argumentosEvento);
        }

        private void ManejarAlarmaTick(object remitente, EventArgs argumentosEvento)
        {
            AlarmaTick?.Invoke(remitente, argumentosEvento);
        }

        private void ManejarTemporizadorTick(object remitente, EventArgs argumentosEvento)
        {
            TemporizadorTick?.Invoke(remitente, argumentosEvento);
        }
    }
}
