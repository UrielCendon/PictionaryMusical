using System.Collections.Generic;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces
{
    /// <summary>
    /// Define el contrato para gestionar los jugadores de una partida.
    /// </summary>
    public interface IGestorJugadoresPartida
    {
        /// <summary>
        /// Indica si hay suficientes jugadores para iniciar la partida.
        /// </summary>
        bool HaySuficientesJugadores { get; }

        /// <summary>
        /// Agrega un jugador a la partida.
        /// </summary>
        void Agregar(string id, string nombre, bool esHost);

        /// <summary>
        /// Obtiene un jugador por su identificador.
        /// </summary>
        JugadorPartida Obtener(string id);

        /// <summary>
        /// Remueve un jugador de la partida.
        /// </summary>
        bool Remover(string id, out bool eraDibujante, out string nombreUsuario);

        /// <summary>
        /// Verifica si un jugador es el anfitrion.
        /// </summary>
        bool EsHost(string id);

        /// <summary>
        /// Prepara la cola de dibujantes para la partida.
        /// </summary>
        void PrepararColaDibujantes();

        /// <summary>
        /// Selecciona al siguiente dibujante de la cola.
        /// </summary>
        bool SeleccionarSiguienteDibujante();

        /// <summary>
        /// Verifica si todos los jugadores adivinaron.
        /// </summary>
        bool TodosAdivinaron();

        /// <summary>
        /// Genera la clasificacion ordenada por puntaje.
        /// </summary>
        List<ClasificacionUsuarioDTO> GenerarClasificacion();

        /// <summary>
        /// Obtiene una copia de la lista de jugadores.
        /// </summary>
        IReadOnlyCollection<JugadorPartida> ObtenerCopiaLista();

        /// <summary>
        /// Indica si quedan dibujantes pendientes en la cola.
        /// </summary>
        bool QuedanDibujantesPendientes();
    }
}