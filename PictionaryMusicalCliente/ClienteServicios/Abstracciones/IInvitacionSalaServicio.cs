using PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante;
using System;
using System.Threading.Tasks;

namespace PictionaryMusicalCliente.ClienteServicios.Abstracciones
{
    /// <summary>
    /// Coordina las acciones de invitacion dentro de una sala, abstrayendo el origen de datos
    /// para facilitar las pruebas y la inyeccion de dependencias.
    /// </summary>
    public interface IInvitacionSalaServicio : IDisposable
    {
        /// <summary>
        /// Envia una invitacion por correo a la sala indicada.
        /// </summary>
        Task<InvitacionCorreoResultado> InvitarPorCorreoAsync(string codigoSala, string correo);

        /// <summary>
        /// Obtiene la informacion necesaria para mostrar el dialogo de invitar amigos.
        /// </summary>
        /// <param name="parametros">Parametros que contienen el codigo de sala, usuario y
        /// amigos invitados.</param>
        /// <returns>Resultado con el ViewModel preparado o mensaje de error.</returns>
        Task<InvitacionAmigosResultado> ObtenerInvitacionAmigosAsync(
            InvitacionAmigosParametros parametros);
    }
}