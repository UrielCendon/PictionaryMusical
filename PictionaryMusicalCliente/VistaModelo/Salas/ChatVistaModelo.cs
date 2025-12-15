using log4net;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using System;
using System.Threading.Tasks;
using System.Windows;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;

namespace PictionaryMusicalCliente.VistaModelo.Salas
{
    /// <summary>
    /// Gestiona la logica del chat durante una partida.
    /// </summary>
    public class ChatVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IChatMensajeria _chatMensajeria;
        private readonly IChatReglasPartida _chatReglasPartida;

        private bool _puedeEscribir;
        private bool _esPartidaIniciada;
        private bool _esDibujante;
        private string _nombreCancionCorrecta;
        private int _tiempoRestante;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ChatVistaModelo"/>.
        /// </summary>
        /// <param name="ventana">Servicio de ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="chatMensajeria">Servicio de mensajeria del chat.</param>
        /// <param name="chatReglasPartida">Servicio de reglas del chat.</param>
        public ChatVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            IChatMensajeria chatMensajeria,
            IChatReglasPartida chatReglasPartida)
            : base(ventana, localizador)
        {
            _chatMensajeria = chatMensajeria
                ?? throw new ArgumentNullException(nameof(chatMensajeria));
            _chatReglasPartida = chatReglasPartida
                ?? throw new ArgumentNullException(nameof(chatReglasPartida));
            InicializarEstadoChat();
        }

        private void InicializarEstadoChat()
        {
            _puedeEscribir = true;
            _esPartidaIniciada = false;
            _esDibujante = false;
            _nombreCancionCorrecta = string.Empty;
            _tiempoRestante = 0;
        }

        /// <summary>
        /// Indica si el jugador puede escribir mensajes en el chat.
        /// </summary>
        public bool PuedeEscribir
        {
            get => _puedeEscribir;
            set => EstablecerPropiedad(ref _puedeEscribir, value);
        }

        /// <summary>
        /// Indica si la partida ha iniciado.
        /// </summary>
        public bool EsPartidaIniciada
        {
            get => _esPartidaIniciada;
            set => EstablecerPropiedad(ref _esPartidaIniciada, value);
        }

        /// <summary>
        /// Indica si el jugador actual es el dibujante.
        /// </summary>
        public bool EsDibujante
        {
            get => _esDibujante;
            set => EstablecerPropiedad(ref _esDibujante, value);
        }

        /// <summary>
        /// Nombre de la cancion correcta para la ronda actual.
        /// </summary>
        public string NombreCancionCorrecta
        {
            get => _nombreCancionCorrecta;
            set
            {
                if (EstablecerPropiedad(ref _nombreCancionCorrecta, value))
                {
                    _chatReglasPartida.NombreCancionCorrecta = value;
                }
            }
        }

        /// <summary>
        /// Tiempo restante en segundos de la ronda actual.
        /// </summary>
        public int TiempoRestante
        {
            get => _tiempoRestante;
            set
            {
                if (EstablecerPropiedad(ref _tiempoRestante, value))
                {
                    _chatReglasPartida.TiempoRestante = value;
                }
            }
        }

        /// <summary>
        /// Evento disparado al recibir un mensaje de chat normal.
        /// </summary>
        public event Action<string, string> MensajeChatRecibido;

        /// <summary>
        /// Evento disparado al recibir un mensaje dorado (acierto).
        /// </summary>
        public event Action<string, string> MensajeDoradoRecibido;

        /// <summary>
        /// Envia un mensaje al chat aplicando las reglas de la partida.
        /// </summary>
        /// <param name="mensaje">Mensaje a enviar.</param>
        public async Task EnviarMensaje(string mensaje)
        {
            if (!ValidarMensaje(mensaje))
            {
                return;
            }

            var decision = await EvaluarMensajeAsync(mensaje).ConfigureAwait(false);
            ProcesarDecisionChat(decision, mensaje);
        }

        private static bool ValidarMensaje(string mensaje)
        {
            return !string.IsNullOrWhiteSpace(mensaje);
        }

        private async Task<ChatDecision> EvaluarMensajeAsync(string mensaje)
        {
            return await _chatReglasPartida
                .EvaluarMensajeAsync(mensaje, EsPartidaIniciada, EsDibujante)
                .ConfigureAwait(false);
        }

        private void ProcesarDecisionChat(ChatDecision decision, string mensaje)
        {
            switch (decision)
            {
                case ChatDecision.CanalLibre:
                    EnviarMensajeCanalLibre(mensaje);
                    break;
                case ChatDecision.IntentoFallido:
                    EnviarMensajeIntentoFallido(mensaje);
                    break;
                case ChatDecision.MensajeBloqueado:
                    RegistrarMensajeBloqueado();
                    break;
                case ChatDecision.AciertoRegistrado:
                    MarcarAciertoRegistrado();
                    break;
            }
        }

        private void EnviarMensajeCanalLibre(string mensaje)
        {
            RegistrarEnvioMensajeCanalLibre(mensaje);
            _chatMensajeria.Enviar(mensaje);
        }

        private static void RegistrarEnvioMensajeCanalLibre(string mensaje)
        {
            _logger.InfoFormat(
                "Enviando mensaje de chat (partida no iniciada): {0}",
                mensaje);
        }

        private void EnviarMensajeIntentoFallido(string mensaje)
        {
            RegistrarEnvioIntentoFallido(mensaje);
            _chatMensajeria.Enviar(mensaje);
        }

        private static void RegistrarEnvioIntentoFallido(string mensaje)
        {
            _logger.InfoFormat(
                "Enviando mensaje de chat (intento fallido): {0}",
                mensaje);
        }

        private static void RegistrarMensajeBloqueado()
        {
            _logger.Info("El dibujante no puede enviar mensajes durante su turno.");
        }

        private void MarcarAciertoRegistrado()
        {
            PuedeEscribir = false;
        }

        /// <summary>
        /// Notifica la recepcion de un mensaje de chat.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador que envio el mensaje.</param>
        /// <param name="mensaje">Contenido del mensaje.</param>
        public void NotificarMensajeChat(string nombreJugador, string mensaje)
        {
            EjecutarEnDispatcher(
                () => MensajeChatRecibido?.Invoke(nombreJugador, mensaje));
        }

        /// <summary>
        /// Notifica que un jugador adivino la cancion correcta.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador que adivino.</param>
        public void NotificarJugadorAdivinoEnChat(string nombreJugador)
        {
            string mensajeDorado = CrearMensajeAdivinacion(nombreJugador);
            EjecutarEnDispatcher(
                () => MensajeDoradoRecibido?.Invoke(string.Empty, mensajeDorado));
        }

        private static string CrearMensajeAdivinacion(string nombreJugador)
        {
            return string.Format(Lang.chatTextoJugadorAdivino, nombreJugador);
        }

        private static void EjecutarEnDispatcher(Action accion)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            if (dispatcher.CheckAccess())
            {
                accion();
            }
            else
            {
                dispatcher.BeginInvoke(accion);
            }
        }
    }
}
