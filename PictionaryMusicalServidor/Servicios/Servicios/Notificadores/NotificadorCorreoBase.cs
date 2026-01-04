using log4net;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Runtime.Caching;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Clase base abstracta para los notificadores que envian correos electronicos.
    /// Proporciona la funcionalidad comun de configuracion SMTP y envio de correos.
    /// Incluye proteccion contra envios frecuentes mediante limite de frecuencia.
    /// </summary>
    public abstract class NotificadorCorreoBase
    {
        private static readonly MemoryCache _cacheEnviosRecientes = 
            new MemoryCache("EnviosCorreoRecientes");

        private static readonly object _lockCache = new object();

        /// <summary>
        /// Tiempo en segundos que debe esperar un usuario para enviar otro correo.
        /// </summary>
        internal const int SegundosEsperaEntreEnvios = 60;

        /// <summary>
        /// Logger para registrar eventos y errores.
        /// </summary>
        protected abstract ILog Logger { get; }

        /// <summary>
        /// Obtiene la configuracion SMTP desde los ajustes de la aplicacion.
        /// </summary>
        /// <returns>Objeto con la configuracion SMTP.</returns>
        protected ConfiguracionSmtp ObtenerConfiguracionSmtp()
        {
            var configuracionSmtp = new ConfiguracionSmtp
            {
                Remitente = ObtenerConfiguracion("CorreoRemitente", "Correo.Remitente.Direccion"),
                Contrasena = ObtenerConfiguracion("CorreoPassword", "Correo.Smtp.Contrasena"),
                Host = ObtenerConfiguracion("CorreoHost", "Correo.Smtp.Host"),
                Usuario = ObtenerConfiguracion("CorreoUsuario", "Correo.Smtp.Usuario"),
                PuertoString = ObtenerConfiguracion("CorreoPuerto", "Correo.Smtp.Puerto"),
                SslString = ObtenerConfiguracion("CorreoSsl", "Correo.Smtp.HabilitarSsl")
            };

            if (string.IsNullOrWhiteSpace(configuracionSmtp.Usuario))
            {
                configuracionSmtp.Usuario = configuracionSmtp.Remitente;
            }

            return configuracionSmtp;
        }

        /// <summary>
        /// Valida que no se haya enviado un correo reciente a la misma direccion.
        /// Si se ha enviado uno en el ultimo minuto, lanza una excepcion FaultException.
        /// </summary>
        /// <param name="correoDestino">Direccion de correo a validar.</param>
        /// <exception cref="FaultException">
        /// Si se intenta enviar un correo antes de que transcurra el tiempo de espera.
        /// </exception>
        protected void ValidarLimiteFrecuenciaEnvio(string correoDestino)
        {
            if (string.IsNullOrWhiteSpace(correoDestino))
            {
                return;
            }

            string claveCache = correoDestino.Trim().ToLowerInvariant();

            lock (_lockCache)
            {
                if (_cacheEnviosRecientes.Contains(claveCache))
                {
                    Logger.Warn(
                        MensajesError.Bitacora.IntentoPrecozEnvioCorreo
                        );
                    throw new FaultException(MensajesError.Cliente.LimiteFrecuenciaCorreo);
                }
            }
        }

        /// <summary>
        /// Registra un envio de correo exitoso en el cache con expiracion automatica.
        /// El registro expira automaticamente despues de SegundosEsperaEntreEnvios.
        /// </summary>
        /// <param name="correoDestino">Direccion de correo enviada.</param>
        protected void RegistrarEnvioExitoso(string correoDestino)
        {
            if (string.IsNullOrWhiteSpace(correoDestino))
            {
                return;
            }

            string claveCache = correoDestino.Trim().ToLowerInvariant();

            var politicaExpiracion = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow
                    .AddSeconds(SegundosEsperaEntreEnvios)
            };

            lock (_lockCache)
            {
                _cacheEnviosRecientes.Set(claveCache, DateTime.UtcNow, politicaExpiracion);
            }
        }

        /// <summary>
        /// Ejecuta el envio de un correo electronico de forma asincrona.
        /// </summary>
        /// <param name="destinatario">Direccion de correo del destinatario.</param>
        /// <param name="asunto">Asunto del correo.</param>
        /// <param name="cuerpo">Cuerpo HTML del correo.</param>
        /// <param name="config">Configuracion SMTP a utilizar.</param>
        /// <returns>True si el envio fue exitoso, False en caso contrario.</returns>
        protected async Task<bool> EjecutarEnvioSmtpAsync(
            string destinatario,
            string asunto,
            string cuerpo,
            ConfiguracionSmtp config)
        {
            try
            {
                using (var mensaje = new MailMessage(
                    config.Remitente,
                    destinatario,
                    asunto,
                    cuerpo))
                {
                    mensaje.IsBodyHtml = true;
                    mensaje.BodyEncoding = Encoding.UTF8;
                    mensaje.SubjectEncoding = Encoding.UTF8;

                    using (var clienteSmtp = new SmtpClient(config.Host, config.Puerto))
                    {
                        clienteSmtp.EnableSsl = config.HabilitarSsl;

                        if (!string.IsNullOrWhiteSpace(config.Contrasena))
                        {
                            clienteSmtp.Credentials =
                                new NetworkCredential(config.Usuario, config.Contrasena);
                        }

                        await clienteSmtp
                            .SendMailAsync(mensaje)
                            .ConfigureAwait(false);
                    }
                }

                return true;
            }
            catch (SmtpException excepcion)
            {
                Logger.Error(MensajesError.Bitacora.ErrorSmtpEnviarCorreo, excepcion);
                return false;
            }
            catch (InvalidOperationException excepcion)
            {
                Logger.Error(MensajesError.Bitacora.OperacionInvalidaEnviarCorreo, excepcion);
                return false;
            }
            catch (ArgumentException excepcion)
            {
                Logger.Error(MensajesError.Bitacora.ArgumentosInvalidosCorreo, excepcion);
                return false;
            }
            catch (FormatException excepcion)
            {
                Logger.Error(MensajesError.Bitacora.FormatoCorreoInvalido, excepcion);
                return false;
            }
            catch (Exception excepcion)
            {
                Logger.Error(MensajesError.Bitacora.ErrorCriticoEnviarNotificacionCodigo, excepcion);
                return false;
            }
        }

        /// <summary>
        /// Obtiene un valor de configuracion buscando en multiples claves.
        /// </summary>
        /// <param name="claves">Claves de configuracion a buscar en orden de prioridad.</param>
        /// <returns>El primer valor encontrado o cadena vacia si no existe.</returns>
        protected static string ObtenerConfiguracion(params string[] claves)
        {
            if (claves == null)
            {
                return string.Empty;
            }

            foreach (string clave in claves)
            {
                if (string.IsNullOrWhiteSpace(clave))
                {
                    continue;
                }

                string valor = ConfigurationManager.AppSettings[clave];

                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Normaliza el codigo de idioma a "es" o "en".
        /// </summary>
        /// <param name="idioma">Codigo de idioma a normalizar.</param>
        /// <returns>"en" si el idioma comienza con "en", "es" en caso contrario.</returns>
        protected static string NormalizarIdioma(string idioma)
        {
            if (string.IsNullOrWhiteSpace(idioma))
            {
                return "es";
            }

            return idioma.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? "en" : "es";
        }

        /// <summary>
        /// Clase interna para almacenar la configuracion del servidor SMTP.
        /// </summary>
        protected sealed class ConfiguracionSmtp
        {
            /// <summary>
            /// Direccion de correo del remitente.
            /// </summary>
            public string Remitente { get; set; }

            /// <summary>
            /// Contrasena para autenticacion SMTP.
            /// </summary>
            public string Contrasena { get; set; }

            /// <summary>
            /// Host del servidor SMTP.
            /// </summary>
            public string Host { get; set; }

            /// <summary>
            /// Usuario para autenticacion SMTP.
            /// </summary>
            public string Usuario { get; set; }

            /// <summary>
            /// Puerto del servidor SMTP como cadena.
            /// </summary>
            public string PuertoString { get; set; }

            /// <summary>
            /// Valor SSL como cadena.
            /// </summary>
            public string SslString { get; set; }

            /// <summary>
            /// Puerto del servidor SMTP. Por defecto 587.
            /// </summary>
            public int Puerto
            {
                get
                {
                    int puerto;
                    if (int.TryParse(PuertoString, out puerto))
                    {
                        return puerto;
                    }
                    return 587;
                }
            }

            /// <summary>
            /// Indica si SSL esta habilitado.
            /// </summary>
            public bool HabilitarSsl
            {
                get
                {
                    bool ssl;
                    if (bool.TryParse(SslString, out ssl))
                    {
                        return ssl;
                    }
                    return false;
                }
            }

            /// <summary>
            /// Indica si la configuracion es valida para enviar correos.
            /// </summary>
            public bool EsValida
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(Remitente)
                        && !string.IsNullOrWhiteSpace(Host)
                        && HabilitarSsl;
                }
            }
        }
    }
}
