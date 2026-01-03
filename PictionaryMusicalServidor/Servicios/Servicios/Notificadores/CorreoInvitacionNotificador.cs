using System.Text;
using System.Threading.Tasks;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Implementacion del notificador que envia invitaciones a partidas por correo electronico.
    /// </summary>
    public class CorreoInvitacionNotificador : NotificadorCorreoBase, ICorreoInvitacionNotificador
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(CorreoInvitacionNotificador));

        private const string AsuntoPredeterminadoEs = "Invitacion a partida de Pictionary Musical";
        private const string AsuntoPredeterminadoEn = "Pictionary Musical Game Invitation";

        /// <summary>
        /// Logger para registrar eventos y errores.
        /// </summary>
        protected override ILog Logger => _logger;

        /// <summary>
        /// Envia una invitacion a una partida al correo indicado de forma asincrona.
        /// </summary>
        /// <param name="parametros">Objeto con los datos necesarios para la invitacion.</param>
        /// <returns>True si el correo se envio correctamente.</returns>
        public async Task<bool> EnviarInvitacionAsync(InvitacionCorreoParametros parametros)
        {
            if (parametros == null ||
                !EntradaComunValidador.EsMensajeValido(parametros.CorreoDestino) || 
                !EntradaComunValidador.EsCodigoSalaValido(parametros.CodigoSala))
            {
                return false;
            }

            var configuracion = ObtenerConfiguracionSmtp();
            if (!configuracion.EsValida)
            {
                _logger.Error(MensajesError.Bitacora.ConfiguracionCorreoInvalida);
                return false;
            }

            string idiomaNormalizado = NormalizarIdioma(parametros.Idioma);
            string asunto = ObtenerAsunto(idiomaNormalizado);
            string cuerpoHtml = ConstruirCuerpoMensaje(
                parametros.CodigoSala, 
                parametros.Creador, 
                idiomaNormalizado);

            return await EjecutarEnvioSmtpAsync(
                parametros.CorreoDestino, 
                asunto, 
                cuerpoHtml, 
                configuracion);
        }

        private static string ObtenerAsunto(string idiomaNormalizado)
        {
            string asuntoConfigurado = ObtenerConfiguracion(
                string.Format("CorreoAsuntoInvitacion.{0}", idiomaNormalizado),
                string.Format("Correo.Invitacion.Asunto.{0}", idiomaNormalizado),
                "CorreoAsuntoInvitacion",
                "Correo.Invitacion.Asunto");

            return string.IsNullOrWhiteSpace(asuntoConfigurado)
                ? ObtenerAsuntoPredeterminado(idiomaNormalizado)
                : asuntoConfigurado;
        }

        internal static string ConstruirCuerpoMensaje(string codigoSala, string creador, 
            string idioma)
        {
            string idiomaNormalizado = NormalizarIdioma(idioma);
            bool esIngles = idiomaNormalizado == "en";

            string saludo = esIngles
                ? "Hello!"
                : "¡Hola!";

            string mensajeBienvenida = esIngles
                ? "You have been invited to a Musical Pictionary game."
                : "Has sido invitado a una partida de Pictionary Musical.";

            string mensajeInvitacion = esIngles
                ? $"{creador} has invited you to their room."
                : $"{creador} te ha invitado a su sala.";

            string mensajeInstruccion = esIngles
                ? "Use the following code to join:"
                : "Utiliza el siguiente codigo para unirte:";

            string mensajeDespedida = esIngles
                ? "See you in the game!"
                : "¡Nos vemos en el juego!";

            var cuerpoHtml = new StringBuilder();

            cuerpoHtml.Append("<html><body style='font-family: Arial, sans-serif;'>");
            cuerpoHtml.Append($"<h2>{saludo}</h2>");
            cuerpoHtml.Append($"<p>{mensajeBienvenida}</p>");
            cuerpoHtml.Append($"<p>{mensajeInvitacion}</p>");
            cuerpoHtml.Append($"<p>{mensajeInstruccion}</p>");
            cuerpoHtml.Append
                ($"<h1 style='color:#4CAF50; letter-spacing: 2px;'>{codigoSala}</h1>");
            cuerpoHtml.Append($"<p>{mensajeDespedida}</p>");
            cuerpoHtml.Append("</body></html>");

            return cuerpoHtml.ToString();
        }

        private static string ObtenerAsuntoPredeterminado(string idiomaNormalizado)
        {
            return idiomaNormalizado == "en" ? AsuntoPredeterminadoEn : AsuntoPredeterminadoEs;
        }
    }
}
