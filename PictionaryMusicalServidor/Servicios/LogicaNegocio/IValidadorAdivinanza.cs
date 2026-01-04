using System;
using PictionaryMusicalServidor.Datos.Entidades; 

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces
{
    /// <summary>
    /// Define el contrato para validar adivinanzas de los jugadores.
    /// </summary>
    public interface IValidadorAdivinanza
    {
        /// <summary>
        /// Determina si un jugador puede realizar un intento de adivinanza.
        /// </summary>
        bool JugadorPuedeAdivinar(JugadorPartida jugador, EstadoPartida estadoPartida);

        /// <summary>
        /// Verifica si el mensaje corresponde a un acierto.
        /// </summary>
        bool VerificarAcierto(int cancionId, string mensaje, out int puntos);

        /// <summary>
        /// Registra un acierto para el jugador.
        /// </summary>
        void RegistrarAcierto(JugadorPartida jugador, int puntos);
    }
}