using System.Threading.Tasks;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Define las operaciones para enviar notificaciones relacionadas con codigos de verificacion
    /// a un usuario mediante correo electronico.
    /// </summary>
    public interface ICodigoVerificacionNotificador
    {
        /// <summary>
        /// Envia una notificacion de codigo de verificacion al correo destino especificado.
        /// </summary>
        /// <param name="parametros">
        /// Objeto que contiene los datos necesarios para enviar la notificacion:
        /// correo destino, codigo de verificacion, usuario destino e idioma.
        /// </param>
        /// <returns>
        /// Retorna true si el mensaje fue enviado correctamente; de lo contrario, retorna false.
        /// </returns>
        Task<bool> NotificarAsync(NotificacionCodigoParametros parametros);
    }
}
