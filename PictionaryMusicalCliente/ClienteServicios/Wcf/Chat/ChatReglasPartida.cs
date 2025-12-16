using System;
using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Chat
{
    /// <summary>
    /// Implementa las reglas de negocio para el chat durante la partida.
    /// </summary>
    public class ChatReglasPartida : IChatReglasPartida
    {
        private const double PorcentajePuntosDibujante = 0.2;
        private readonly IChatAciertosServicio _chatAciertosServicio;

        /// <summary>
        /// Inicializa las reglas de partida con el servicio de aciertos necesario.
        /// </summary>
        /// <param name="chatAciertosServicio">Servicio para registrar puntuaciones.</param>
        /// <exception cref="ArgumentNullException">
        /// Si el servicio de aciertos es nulo.
        /// </exception>
        public ChatReglasPartida(IChatAciertosServicio chatAciertosServicio)
        {
            _chatAciertosServicio = chatAciertosServicio
                ?? throw new ArgumentNullException(nameof(chatAciertosServicio));
        }

        /// <inheritdoc />
        public string NombreCancionCorrecta { get; set; } = string.Empty;

        /// <inheritdoc />
        public int TiempoRestante { get; set; }

        /// <inheritdoc />
        public async Task<ChatDecision> EvaluarMensajeAsync(
            string mensaje,
            bool esPartidaIniciada,
            bool esDibujante)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return ChatDecision.IntentoFallido;
            }

            ChatDecision? decisionPorEstado = ObtenerDecisionPorEstado(
                esPartidaIniciada,
                esDibujante);

            if (decisionPorEstado.HasValue)
            {
                return decisionPorEstado.Value;
            }

            if (!EsRespuestaCorrecta(mensaje))
            {
                return ChatDecision.IntentoFallido;
            }

            await RegistrarAciertoConPuntajeAsync().ConfigureAwait(false);
            return ChatDecision.AciertoRegistrado;
        }

        private bool EsRespuestaCorrecta(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(NombreCancionCorrecta))
            {
                return false;
            }

            return string.Equals(
                mensaje?.Trim(),
                NombreCancionCorrecta.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        private async Task RegistrarAciertoConPuntajeAsync()
        {
            string nombreJugador = _chatAciertosServicio.ObtenerNombreJugadorActual();
            (int puntosAdivinador, int puntosDibujante) = CalcularPuntos(TiempoRestante);

            await _chatAciertosServicio.RegistrarAciertoAsync(
                nombreJugador,
                puntosAdivinador,
                puntosDibujante)
                .ConfigureAwait(false);
        }

        private static ChatDecision? ObtenerDecisionPorEstado(
            bool esPartidaIniciada,
            bool esDibujante)
        {
            if (!esPartidaIniciada)
            {
                return ChatDecision.CanalLibre;
            }

            if (esDibujante)
            {
                return ChatDecision.MensajeBloqueado;
            }

            return null;
        }

        private static (int Adivinador, int Dibujante) CalcularPuntos(int tiempoRestante)
        {
            int puntosAdivinador = tiempoRestante;
            int puntosDibujante = (int)(puntosAdivinador * PorcentajePuntosDibujante);
            return (puntosAdivinador, puntosDibujante);
        }
    }
}
