using System;
using System.Timers;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio
{
    /// <summary>
    /// Gestiona los temporizadores de la partida y el control del tiempo restante para rondas y 
    /// transiciones.
    /// Implementa IDisposable para asegurar la liberacion correcta de los recursos del 
    /// temporizador.
    /// </summary>
    public class GestorTiemposPartida : IDisposable
    {
        private readonly Timer _temporizadorRonda;
        private readonly Timer _temporizadorTransicion;
        private DateTime _inicioRonda;
        private readonly int _duracionRondaSegundos;
        private bool _disposed = false;

        /// <summary>
        /// Evento que se dispara cuando el tiempo de la ronda de juego ha finalizado.
        /// </summary>
        public event Action TiempoRondaAgotado;

        /// <summary>
        /// Evento que se dispara cuando el tiempo de espera (transicion) entre rondas ha 
        /// finalizado.
        /// </summary>
        public event Action TiempoTransicionAgotado;

        /// <summary>
        /// Inicializa una nueva instancia del gestor de tiempos.
        /// </summary>
        /// <param name="duracionRondaSegundos">Duracion en segundos para cada ronda de juego.
        /// </param>
        /// <param name="duracionTransicionSegundos">Duracion en segundos para las pausas entre 
        /// rondas.</param>
        public GestorTiemposPartida(int duracionRondaSegundos, int duracionTransicionSegundos)
        {
            _duracionRondaSegundos = duracionRondaSegundos;

            _temporizadorRonda = new Timer 
            { 
                AutoReset = false 
            };

            _temporizadorRonda.Elapsed += ManejarTiempoRondaAgotado;

            _temporizadorTransicion = new Timer
            {
                AutoReset = false,
                Interval = duracionTransicionSegundos * 1000
            };

            _temporizadorTransicion.Elapsed += ManejarTiempoTransicionAgotado;
        }

        private void ManejarTiempoRondaAgotado(object sender, ElapsedEventArgs argumentos)
        {
            TiempoRondaAgotado?.Invoke();
        }

        private void ManejarTiempoTransicionAgotado(object sender, ElapsedEventArgs argumentos)
        {
            TiempoTransicionAgotado?.Invoke();
        }

        /// <summary>
        /// Inicia el temporizador principal de la ronda y registra la marca de tiempo de inicio.
        /// </summary>
        public void IniciarRonda()
        {
            _temporizadorTransicion.Stop();
            _temporizadorRonda.Interval = _duracionRondaSegundos * 1000;
            _inicioRonda = DateTime.UtcNow;
            _temporizadorRonda.Start();
        }

        /// <summary>
        /// Detiene la ronda actual e inicia el temporizador de transicion.
        /// </summary>
        public void IniciarTransicion()
        {
            DetenerRonda();
            _temporizadorTransicion.Start();
        }

        /// <summary>
        /// Detiene inmediatamente todos los temporizadores activos.
        /// </summary>
        public void DetenerTodo()
        {
            DetenerRonda();
            _temporizadorTransicion.Stop();
        }

        /// <summary>
        /// Calcula los puntos a otorgar basandose en el tiempo restante de la ronda.
        /// </summary>
        /// <returns>Cantidad de segundos restantes como puntos, o cero si el tiempo se agoto.
        /// </returns>
        public int CalcularPuntosPorTiempo()
        {
            if (!_temporizadorRonda.Enabled)
            {
                return 0;
            }

            var transcurrido = (int)(DateTime.UtcNow - _inicioRonda).TotalSeconds;
            var restante = _duracionRondaSegundos - transcurrido;
            return Math.Max(0, restante);
        }

        /// <summary>
        /// Libera los recursos utilizados por los temporizadores.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _temporizadorRonda?.Dispose();
                _temporizadorTransicion?.Dispose();
            }

            _disposed = true;
        }

        private void DetenerRonda()
        {
            _temporizadorRonda.Stop();
            _inicioRonda = default;
        }
    }
}