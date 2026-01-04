using System;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces
{
    /// <summary>
    /// Define el contrato para gestionar los temporizadores de una partida.
    /// </summary>
    public interface IGestorTiemposPartida : IDisposable
    {
        /// <summary>
        /// Evento disparado cuando el tiempo de ronda se agota.
        /// </summary>
        event Action TiempoRondaAgotado;

        /// <summary>
        /// Evento disparado cuando el tiempo de transicion termina.
        /// </summary>
        event Action TiempoTransicionAgotado;

        /// <summary>
        /// Inicia el temporizador de ronda.
        /// </summary>
        void IniciarRonda();

        /// <summary>
        /// Inicia el periodo de transicion entre rondas.
        /// </summary>
        void IniciarTransicion();

        /// <summary>
        /// Detiene todos los temporizadores activos.
        /// </summary>
        void DetenerTodo();

        /// <summary>
        /// Calcula los puntos disponibles segun el tiempo restante.
        /// </summary>
        int CalcularPuntosPorTiempo();
    }
}