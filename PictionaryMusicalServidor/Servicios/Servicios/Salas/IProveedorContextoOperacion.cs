using System.ServiceModel;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Interfaz para obtener el contexto de operacion WCF.
    /// Permite abstraer el acceso a OperationContext para facilitar pruebas unitarias.
    /// </summary>
    public interface IProveedorContextoOperacion
    {
        /// <summary>
        /// Obtiene el canal de callback del contexto actual.
        /// </summary>
        /// <returns>Canal de callback para notificaciones de sala.</returns>
        ISalasManejadorCallback ObtenerCallback();

        /// <summary>
        /// Obtiene el canal de comunicacion del contexto actual.
        /// </summary>
        /// <returns>Canal de comunicacion contextual o null si no existe.</returns>
        IContextChannel ObtenerCanal();
    }
}
