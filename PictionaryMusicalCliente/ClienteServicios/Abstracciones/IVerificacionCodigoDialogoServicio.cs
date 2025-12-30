using PictionaryMusicalCliente.Utilidades;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Abstracciones
{
    /// <summary>
    /// Abstrae la logica de interfaz de usuario para mostrar ventanas emergentes de verificacion.
    /// </summary>
    public interface IVerificacionCodigoDialogoServicio
    {
        /// <summary>
        /// Despliega un dialogo modal para ingresar el codigo de verificacion enviado por correo.
        /// </summary>
        /// <param name="parametros">Parametros que contienen la descripcion, token y servicio
        /// de verificacion.</param>
        /// <param name="avisoServicio">Servicio para mostrar avisos al usuario.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <returns>El resultado de la operacion de verificacion.</returns>
        Task<DTOs.ResultadoRegistroCuentaDTO> MostrarDialogoAsync(
            VerificacionDialogoParametros parametros,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador);
    }
}