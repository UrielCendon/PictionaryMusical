using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Contratos
{
    /// <summary>
    /// Interfaz que define las operaciones para el envio de notificaciones de codigos.
    /// </summary>
    public interface INotificacionCodigosServicio
    {
        /// <summary>
        /// Envia un codigo de verificacion por correo electronico.
        /// </summary>
        /// <param name="parametros">Objeto con los datos necesarios para la notificacion.</param>
        /// <returns>True si el codigo fue enviado exitosamente, false en caso contrario.</returns>
        bool EnviarNotificacion(NotificacionCodigoParametros parametros);
    }
}