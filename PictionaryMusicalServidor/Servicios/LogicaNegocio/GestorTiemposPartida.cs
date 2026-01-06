using System;
using System.Timers;
using PictionaryMusicalServidor.Datos.Utilidades;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio
{
    /// <summary>
    /// Gestiona los temporizadores de ronda y transicion de una partida.
    /// Controla el inicio, detencion y calculo de puntos basados en tiempo restante.
    /// </summary>
    public class GestorTiemposPartida : IGestorTiemposPartida
    {
        private const int MilisegundosPorSegundo = 1000;

        private readonly Timer _temporizadorRonda;
        private readonly Timer _temporizadorTransicion;
        private readonly IProveedorFecha _proveedorFecha;
        private DateTime _inicioRonda;
        private readonly int _duracionRondaSegundos;
        private bool _desechado = false;

        /// <summary>
        /// Evento que se dispara cuando el tiempo de la ronda se agota.
        /// </summary>
        public event Action TiempoRondaAgotado;

        /// <summary>
        /// Evento que se dispara cuando el tiempo de transicion termina.
        /// </summary>
        public event Action TiempoTransicionAgotado;

        /// <summary>
        /// Inicializa una nueva instancia del gestor de tiempos.
        /// </summary>
        /// <param name="duracionRondaSegundos">Duracion de cada ronda en segundos.</param>
        /// <param name="duracionTransicionSegundos">Duracion de la transicion entre rondas 
        /// en segundos.</param>
        /// <param name="proveedorFecha">Proveedor de fecha para calculos de tiempo.</param>
        /// <exception cref="ArgumentNullException">Se lanza si proveedorFecha es nulo.</exception>
        public GestorTiemposPartida(
            int duracionRondaSegundos,
            int duracionTransicionSegundos,
            IProveedorFecha proveedorFecha)
        {
            _duracionRondaSegundos = duracionRondaSegundos;
            _proveedorFecha = proveedorFecha ??
                throw new ArgumentNullException(nameof(proveedorFecha));

            _temporizadorRonda = new Timer { AutoReset = false };
            _temporizadorRonda.Elapsed += ManejarTiempoRondaAgotado;

            _temporizadorTransicion = new Timer
            {
                AutoReset = false,
                Interval = duracionTransicionSegundos * MilisegundosPorSegundo
            };
            _temporizadorTransicion.Elapsed += ManejarTiempoTransicionAgotado;
        }

        private void ManejarTiempoRondaAgotado(object emisor, ElapsedEventArgs argumentos)
        {
            TiempoRondaAgotado?.Invoke();
        }

        private void ManejarTiempoTransicionAgotado(object emisor, ElapsedEventArgs argumentos)
        {
            TiempoTransicionAgotado?.Invoke();
        }

        /// <summary>
        /// Inicia el temporizador de ronda y registra el momento de inicio.
        /// </summary>
        public void IniciarRonda()
        {
            _temporizadorTransicion.Stop();
            _temporizadorRonda.Interval = _duracionRondaSegundos * MilisegundosPorSegundo;
            _inicioRonda = _proveedorFecha.ObtenerFechaActualUtc();
            _temporizadorRonda.Start();
        }

        /// <summary>
        /// Inicia el periodo de transicion entre rondas.
        /// </summary>
        public void IniciarTransicion()
        {
            DetenerRonda();
            _temporizadorTransicion.Start();
        }

        /// <summary>
        /// Detiene todos los temporizadores activos.
        /// </summary>
        public void DetenerTodo()
        {
            DetenerRonda();
            _temporizadorTransicion.Stop();
        }

        /// <summary>
        /// Calcula los puntos disponibles basados en el tiempo restante de la ronda.
        /// </summary>
        /// <returns>Puntos restantes, o cero si la ronda no esta activa o el tiempo expiro.
        /// </returns>
        public int CalcularPuntosPorTiempo()
        {
            if (!_temporizadorRonda.Enabled)
            {
                return 0;
            }

            var fechaActual = _proveedorFecha.ObtenerFechaActualUtc();
            var transcurrido = (int)(fechaActual - _inicioRonda).TotalSeconds;
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

        /// <summary>
        /// Libera los recursos gestionados y no gestionados.
        /// </summary>
        /// <param name="liberando">Indica si se estan liberando recursos gestionados.</param>
        protected virtual void Dispose(bool liberando)
        {
            if (_desechado)
            {
                return;
            }

            if (liberando)
            {
                _temporizadorRonda?.Dispose();
                _temporizadorTransicion?.Dispose();
            }

            _desechado = true;
        }

        private void DetenerRonda()
        {
            _temporizadorRonda.Stop();
            _inicioRonda = default;
        }
    }
}