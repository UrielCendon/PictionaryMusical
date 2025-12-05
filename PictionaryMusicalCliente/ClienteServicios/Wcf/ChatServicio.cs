using System;
using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    public enum ChatDecision
    {
        CanalLibre,
        MensajeBloqueado,
        IntentoFallido,
        AciertoRegistrado
    }

    public interface IChatMensajeria
    {
        void Enviar(string mensaje);
    }

    public interface IChatAciertosServicio
    {
        string ObtenerNombreJugadorActual();

        Task RegistrarAciertoAsync(string nombreJugador, int puntosAdivinador, 
            int puntosDibujante);
    }

    public interface IChatReglasPartida
    {
        string NombreCancionCorrecta { get; set; }

        int TiempoRestante { get; set; }

        Task<ChatDecision> EvaluarMensajeAsync(string mensaje, bool esPartidaIniciada, 
            bool esDibujante);
    }

    public class ChatMensajeriaDelegado : IChatMensajeria
    {
        private readonly Action<string> _enviarMensaje;

        public ChatMensajeriaDelegado(Action<string> enviarMensaje)
        {
            _enviarMensaje = enviarMensaje ?? 
                throw new ArgumentNullException(nameof(enviarMensaje));
        }

        public void Enviar(string mensaje)
        {
            _enviarMensaje(mensaje);
        }
    }

    public class ChatAciertosDelegado : IChatAciertosServicio
    {
        private readonly Func<string> _obtenerNombreJugador;
        private readonly Func<string, int, int, Task> _registrarAcierto;

        public ChatAciertosDelegado(
            Func<string> obtenerNombreJugador,
            Func<string, int, int, Task> registrarAcierto)
        {
            _obtenerNombreJugador = obtenerNombreJugador
                ?? throw new ArgumentNullException(nameof(obtenerNombreJugador));
            _registrarAcierto = registrarAcierto
                ?? throw new ArgumentNullException(nameof(registrarAcierto));
        }

        public string ObtenerNombreJugadorActual()
        {
            return _obtenerNombreJugador();
        }

        public Task RegistrarAciertoAsync(string nombreJugador, int puntosAdivinador, 
            int puntosDibujante)
        {
            return _registrarAcierto(nombreJugador, puntosAdivinador, puntosDibujante);
        }
    }

    public class ChatReglasPartida : IChatReglasPartida
    {
        private const double PorcentajePuntosDibujante = 0.2;
        private readonly IChatAciertosServicio _chatAciertosServicio;

        public ChatReglasPartida(IChatAciertosServicio chatAciertosServicio)
        {
            _chatAciertosServicio = chatAciertosServicio
                ?? throw new ArgumentNullException(nameof(chatAciertosServicio));
        }

        public string NombreCancionCorrecta { get; set; } = string.Empty;

        public int TiempoRestante { get; set; }

        public async Task<ChatDecision> EvaluarMensajeAsync(
            string mensaje,
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

            if (!EsRespuestaCorrecta(mensaje))
            {
                return ChatDecision.IntentoFallido;
            }

            await RegistrarAciertoAsync().ConfigureAwait(false);
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

        private async Task RegistrarAciertoAsync()
        {
            string nombreJugador = _chatAciertosServicio.ObtenerNombreJugadorActual();
            int puntosAdivinador = TiempoRestante;
            int puntosDibujante = (int)(puntosAdivinador * PorcentajePuntosDibujante);

            await _chatAciertosServicio.RegistrarAciertoAsync(
                nombreJugador,
                puntosAdivinador,
                puntosDibujante)
                .ConfigureAwait(false);
        }
    }
}
