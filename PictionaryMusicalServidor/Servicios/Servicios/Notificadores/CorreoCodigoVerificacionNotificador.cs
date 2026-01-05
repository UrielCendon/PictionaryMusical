using log4net;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using System.Text;
using System.Threading.Tasks;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Servicio para el envio de codigos de verificacion por correo electronico.
    /// Implementa la interfaz ICodigoVerificacionNotificador.
    /// </summary>
    public class CorreoCodigoVerificacionNotificador : NotificadorCorreoBase, 
        ICodigoVerificacionNotificador
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(CorreoCodigoVerificacionNotificador));

        private const string AsuntoPredeterminadoEs = "Codigo de verificacion";
        private const string AsuntoPredeterminadoEn = "Verification code";

        /// <summary>
        /// Logger para registrar eventos y errores.
        /// </summary>
        protected override ILog Logger => _logger;

        /// <summary>
        /// Envia un codigo de verificacion al usuario especificado de forma asincrona.
        /// </summary>
        /// <param name="parametros">Objeto con los datos necesarios para la notificacion.</param>
        /// <returns>True si el envio fue exitoso, False en caso contrario.</returns>
        public async Task<bool> NotificarAsync(NotificacionCodigoParametros parametros)
        {
            if (parametros == null ||
                string.IsNullOrWhiteSpace(parametros.CorreoDestino) || 
                string.IsNullOrWhiteSpace(parametros.Codigo))
            {
                return false;
            }

            ValidarYReservarEnvio(parametros.CorreoDestino);

            var configuracionSmtp = ObtenerConfiguracionSmtp();
            if (!configuracionSmtp.EsValida)
            {
                _logger.Error(MensajesError.Bitacora.ConfiguracionCorreoInvalida);
                LiberarReservaEnvio(parametros.CorreoDestino);
                return false;
            }

            string idiomaNormalizado = NormalizarIdioma(parametros.Idioma);
            string asunto = ObtenerAsunto(idiomaNormalizado);
            string cuerpoHtml = ConstruirCuerpoMensaje(
                parametros.UsuarioDestino, 
                parametros.Codigo, 
                idiomaNormalizado);

            bool enviado = await EjecutarEnvioSmtpAsync(
                parametros.CorreoDestino, 
                asunto, 
                cuerpoHtml, 
                configuracionSmtp);

            if (!enviado)
            {
                LiberarReservaEnvio(parametros.CorreoDestino);
            }

            return enviado;
        }

        /// <summary>
        /// Construye el cuerpo HTML del mensaje de correo con el codigo de verificacion.
        /// Metodo interno para facilitar pruebas unitarias.
        /// </summary>
        /// <param name="usuarioDestino">Nombre del usuario al que se dirige el correo.</param>
        /// <param name="codigo">Codigo numerico de verificacion.</param>
        /// <param name="idioma">Idioma para el texto del mensaje.</param>
        /// <returns>Cadena con el HTML del cuerpo del correo.</returns>
        internal static string ConstruirCuerpoMensaje(string usuarioDestino, string codigo,
            string idioma)
        {
            string idiomaNormalizado = NormalizarIdioma(idioma);
            bool esIngles = idiomaNormalizado == "en";

            string saludo = esIngles ? "Hello" : "Hola";
            string mensajeCodigo = esIngles
                ? "Your verification code is:"
                : "Tu codigo de verificacion es:";

            string mensajeIgnorar = esIngles
                ? "If you did not request this code you can ignore this message."
                : "Si no solicitaste este codigo puedes ignorar este mensaje.";

            var cuerpoHtml = new StringBuilder();

            cuerpoHtml.Append("<html><body style='font-family: Arial, sans-serif;'>");

            if (!string.IsNullOrWhiteSpace(usuarioDestino))
            {
                cuerpoHtml.Append($"<h2>{saludo} {usuarioDestino},</h2>");
            }
            else
            {
                cuerpoHtml.Append($"<h2>{saludo},</h2>");
            }

            cuerpoHtml.Append($"<p>{mensajeCodigo}</p>");
            cuerpoHtml.Append($"<h1 style='color:#0078D7; letter-spacing: 5px;'>{codigo}</h1>");
            cuerpoHtml.Append($"<p style='color: gray; font-size: 0.9em;'>{mensajeIgnorar}</p>");
            cuerpoHtml.Append("</body></html>");

            return cuerpoHtml.ToString();
        }

        private static string ObtenerAsunto(string idiomaNormalizado)
        {
            string asuntoConfigurado = ObtenerConfiguracion(
                string.Format("CorreoAsunto.{0}", idiomaNormalizado),
                string.Format("Correo.Codigo.Asunto.{0}", idiomaNormalizado),
                "CorreoAsunto",
                "Correo.Codigo.Asunto");

            return string.IsNullOrWhiteSpace(asuntoConfigurado)
                ? ObtenerAsuntoPredeterminado(idiomaNormalizado)
                : asuntoConfigurado;
        }

        private static string ObtenerAsuntoPredeterminado(string idiomaNormalizado)
        {
            return idiomaNormalizado == "en" ? AsuntoPredeterminadoEn : AsuntoPredeterminadoEs;
        }
    }
}
