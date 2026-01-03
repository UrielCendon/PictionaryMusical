using System.Threading.Tasks;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Interfaz para el servicio de envio de invitaciones por correo electronico.
    /// </summary>
    public interface ICorreoInvitacionNotificador
    {
        /// <summary>
        /// Envia una invitacion a una partida de forma asincrona.
        /// </summary>
        /// <param name="parametros">Objeto con los datos necesarios para la invitacion.</param>
        /// <returns>True si el envio fue exitoso, False en caso contrario.</returns>
        Task<bool> EnviarInvitacionAsync(InvitacionCorreoParametros parametros);
    }
}