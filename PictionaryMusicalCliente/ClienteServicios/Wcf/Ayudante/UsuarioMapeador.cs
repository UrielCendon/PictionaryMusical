using PictionaryMusicalCliente.Sesiones;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante
{
    /// <summary>
    /// Expone operaciones auxiliares para mantener sincronizada la sesion del usuario.
    /// </summary>
    public class UsuarioMapeador : IUsuarioMapeador
    {
        /// <summary>
        /// Actualiza la sesion del usuario actual a partir del DTO recibido del servidor.
        /// </summary>
        /// <param name="dto">Datos del usuario autenticado.</param>
        public void ActualizarSesion(DTOs.UsuarioDTO dto)
        {
            if (dto == null)
            {
                SesionUsuarioActual.CerrarSesion();
                return;
            }
            SesionUsuarioActual.EstablecerUsuario(dto);
        }
    }
}