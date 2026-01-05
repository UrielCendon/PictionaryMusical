using System.ServiceModel;
using PictionaryMusicalServidor.Servicios.Contratos;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Implementacion del proveedor de contexto de operacion WCF.
    /// </summary>
    public class ProveedorContextoOperacion : IProveedorContextoOperacion
    {
        /// <inheritdoc/>
        public ISalasManejadorCallback ObtenerCallback()
        {
            return OperationContext.Current.GetCallbackChannel<ISalasManejadorCallback>();
        }

        /// <inheritdoc/>
        public IContextChannel ObtenerCanal()
        {
            return OperationContext.Current?.Channel;
        }
    }
}
