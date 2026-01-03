using PictionaryMusicalServidor.Datos;
using System;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio
{
    /// <summary>
    /// Gestiona la logica de verificacion y registro de adivinanzas dentro de una partida.
    /// </summary>
    public class ValidadorAdivinanza
    {
        private readonly ICatalogoCanciones _catalogoCanciones;
        private readonly GestorTiemposPartida _gestorTiempos;

        /// <summary>
        /// Inicializa una nueva instancia del validador de adivinanzas.
        /// </summary>
        /// <param name="catalogoCanciones">Servicio de catalogo para validar respuestas.</param>
        /// <param name="gestorTiempos">Gestor de tiempos para calcular puntos.</param>
        public ValidadorAdivinanza(
            ICatalogoCanciones catalogoCanciones, 
            GestorTiemposPartida gestorTiempos)
        {
            _catalogoCanciones = catalogoCanciones ?? 
                throw new ArgumentNullException(nameof(catalogoCanciones));
            _gestorTiempos = gestorTiempos ?? 
                throw new ArgumentNullException(nameof(gestorTiempos));
        }

        /// <summary>
        /// Verifica si un jugador puede intentar adivinar en el estado actual.
        /// </summary>
        /// <param name="jugador">Jugador a verificar.</param>
        /// <param name="estadoPartida">Estado actual de la partida.</param>
        /// <returns>True si el jugador puede adivinar, False en caso contrario.</returns>
        public static bool JugadorPuedeAdivinar(JugadorPartida jugador, EstadoPartida estadoPartida)
        {
            return estadoPartida == EstadoPartida.Jugando 
                && !jugador.EsDibujante 
                && !jugador.YaAdivino;
        }

        /// <summary>
        /// Verifica si el mensaje es una respuesta correcta y calcula los puntos.
        /// </summary>
        /// <param name="cancionId">ID de la cancion actual.</param>
        /// <param name="mensaje">Mensaje con el intento de adivinanza.</param>
        /// <param name="puntos">Puntos obtenidos si es correcto.</param>
        /// <returns>True si la respuesta es correcta, False en caso contrario.</returns>
        public bool VerificarAcierto(int cancionId, string mensaje, out int puntos)
        {
            puntos = 0;
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
        /// <param name="puntos">Puntos a agregar.</param>
        public static void RegistrarAcierto(JugadorPartida jugador, int puntos)
        {
            jugador.YaAdivino = true;
            jugador.PuntajeTotal += puntos;
        }

        /// <summary>
        /// Verifica si el mensaje sigue el protocolo de acierto y extrae los puntos.
        /// </summary>
        /// <param name="mensaje">Mensaje a verificar.</param>
        /// <param name="puntos">Puntos extraidos del mensaje.</param>
        /// <returns>True si es un mensaje de protocolo valido, False en caso contrario.</returns>
        public static bool EsMensajeAciertoProtocolo(string mensaje, out int puntos)
        {
            puntos = 0;
            if (!mensaje.StartsWith("ACIERTO:", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var partes = mensaje.Split(':');
            return partes.Length >= 3 && int.TryParse(partes[2], out puntos) && puntos > 0;
        }
    }
}
