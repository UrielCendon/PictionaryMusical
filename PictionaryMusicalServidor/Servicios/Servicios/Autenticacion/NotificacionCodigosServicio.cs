using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using log4net;
using System;
using System.ServiceModel;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Servicio interno para el envio de notificaciones de codigos de verificacion.
    /// Gestiona el envio de codigos por correo electronico a usuarios.
    /// </summary>
    public class NotificacionCodigosServicio : INotificacionCodigosServicio
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(NotificacionCodigosServicio));

        private readonly ICodigoVerificacionNotificador _notificador;

        /// <summary>
        /// Constructor por defecto para WCF.
        /// </summary>
        public NotificacionCodigosServicio() : this(new CorreoCodigoVerificacionNotificador())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias.
        /// </summary>
        /// <param name="notificador">Instancia del notificador a usar.</param>
        public NotificacionCodigosServicio(ICodigoVerificacionNotificador notificador)
        {
            _notificador = notificador ?? new CorreoCodigoVerificacionNotificador();
        }

        /// <summary>
        /// Envia un codigo de verificacion por correo electronico a un usuario.
        /// Valida que el correo y codigo no esten vacios antes de enviar.
        /// </summary>
        /// <param name="parametros">Objeto con los datos necesarios para la notificacion.</param>
        /// <returns>True si el codigo fue enviado exitosamente, false en caso contrario.</returns>
        /// <exception cref="FaultException">
        /// Si se intenta enviar un correo antes de que transcurra el tiempo de espera.
        /// </exception>
        public bool EnviarNotificacion(NotificacionCodigoParametros parametros)
        {
            if (parametros == null ||
                string.IsNullOrWhiteSpace(parametros.CorreoDestino) || 
                string.IsNullOrWhiteSpace(parametros.Codigo))
            {
                return false;
            }

            try
            {
                var tarea = _notificador?.NotificarAsync(parametros);

                if (tarea == null)
                {
                    return false;
                }

                return tarea.GetAwaiter().GetResult();
            }
            catch (FaultException)
            {
                throw;
            }
            catch (AggregateException excepcion)
            {
                ManejarExcepcionAgregada(excepcion);
                return false;
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.OperacionInvalidaEnviarCorreo,
                    excepcion);
                return false;
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorCriticoEnviarNotificacionCodigo,
                    excepcion);
                return false;
            }
        }

        private static void ManejarExcepcionAgregada(AggregateException excepcion)
        {
            foreach (var innerExcepcion in excepcion.InnerExceptions)
            {
                if (innerExcepcion is FaultException faultExcepcion)
                {
                    throw faultExcepcion;
                }
            }

            _logger.Error(
                MensajesError.Bitacora.ErrorCriticoEnviarNotificacionCodigo,
                excepcion);
        }
    }
}
