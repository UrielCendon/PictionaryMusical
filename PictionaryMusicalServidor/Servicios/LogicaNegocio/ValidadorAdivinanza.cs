using System;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio
{
    /// <summary>
    /// Valida los intentos de adivinanza de los jugadores y gestiona el registro de aciertos.
    /// </summary>
    public class ValidadorAdivinanza : IValidadorAdivinanza
    {
        private const string PrefijoAciertoProtocolo = "ACIERTO:";
        private const char SeparadorProtocolo = ':';
        private const int MinimoPartesProtocolo = 3;
        private const int IndicePuntosProtocolo = 2;

        private readonly ICatalogoCanciones _catalogoCanciones;
        private readonly IGestorTiemposPartida _gestorTiempos;

        /// <summary>
        /// Inicializa una nueva instancia del validador de adivinanzas.
        /// </summary>
        /// <param name="catalogoCanciones">Catalogo para validar respuestas.</param>
        /// <param name="gestorTiempos">Gestor para calcular puntos por tiempo.</param>
        /// <exception cref="ArgumentNullException">Se lanza si alguna dependencia es nula.
        /// </exception>
        public ValidadorAdivinanza(
            ICatalogoCanciones catalogoCanciones,
            IGestorTiemposPartida gestorTiempos)
        {
            _catalogoCanciones = catalogoCanciones ??
                throw new ArgumentNullException(nameof(catalogoCanciones));
            _gestorTiempos = gestorTiempos ??
                throw new ArgumentNullException(nameof(gestorTiempos));
        }

        /// <summary>
        /// Determina si un jugador puede realizar un intento de adivinanza.
        /// </summary>
        /// <param name="jugador">Jugador que intenta adivinar.</param>
        /// <param name="estadoPartida">Estado actual de la partida.</param>
        /// <returns>True si el jugador puede adivinar, false en caso contrario.</returns>
        public bool JugadorPuedeAdivinar(JugadorPartida jugador, EstadoPartida estadoPartida)
        {
            if (jugador == null)
            {
                return false;
            }

            return estadoPartida == EstadoPartida.Jugando
                && !jugador.EsDibujante
                && !jugador.YaAdivino;
        }

        /// <summary>
        /// Verifica si el mensaje corresponde a un acierto y calcula los puntos.
        /// </summary>
        /// <param name="cancionId">Identificador de la cancion actual.</param>
        /// <param name="mensaje">Mensaje enviado por el jugador.</param>
        /// <param name="puntos">Puntos obtenidos si es acierto.</param>
        /// <returns>True si el mensaje es correcto, false en caso contrario.</returns>
        public bool VerificarAcierto(int cancionId, string mensaje, out int puntos)
        {
            puntos = 0;
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return false;
            }

            bool esCorrecto = _catalogoCanciones.ValidarRespuesta(cancionId, mensaje);

            if (!esCorrecto && EsMensajeAciertoProtocolo(mensaje, out int puntosProtocolo))
            {
                esCorrecto = true;
                puntos = puntosProtocolo;
            }

            if (esCorrecto && puntos == 0)
            {
                puntos = _gestorTiempos.CalcularPuntosPorTiempo();
            }

            return esCorrecto;
        }

        /// <summary>
        /// Registra un acierto para el jugador, actualizando su estado y puntaje.
        /// </summary>
        /// <param name="jugador">Jugador que acerto.</param>
        /// <param name="puntos">Puntos a sumar al puntaje total.</param>
        /// <exception cref="ArgumentNullException">Se lanza si el jugador es nulo.</exception>
        public void RegistrarAcierto(JugadorPartida jugador, int puntos)
        {
            if (jugador == null)
            {
                throw new ArgumentNullException(nameof(jugador));
            }

            jugador.YaAdivino = true;
            jugador.PuntajeTotal += puntos;
        }

        private static bool EsMensajeAciertoProtocolo(string mensaje, out int puntos)
        {
            puntos = 0;
            if (!mensaje.StartsWith(PrefijoAciertoProtocolo, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var partes = mensaje.Split(SeparadorProtocolo);
            return partes.Length >= MinimoPartesProtocolo 
                && int.TryParse(partes[IndicePuntosProtocolo], out puntos) 
                && puntos > 0;
        }
    }
}