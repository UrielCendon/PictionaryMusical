using System.ServiceModel;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Implementacion del proveedor de callbacks que obtiene el callback actual 
    /// del contexto de operacion WCF.
    /// </summary>
    /// <typeparam name="T">Tipo de callback a obtener.</typeparam>
    internal class ProveedorCallback<T> : IProveedorCallback<T> where T : class
    {
        /// <summary>
        /// Obtiene el callback actual del contexto de operacion WCF.
        /// </summary>
        /// <returns>El callback del contexto actual.</returns>
        /// <exception cref="FaultException">Se lanza si no se puede obtener el callback.
        /// </exception>
        public T ObtenerCallbackActual()
        {
            var contexto = OperationContext.Current;
            if (contexto != null)
            {
                var callback = contexto.GetCallbackChannel<T>();
                if (callback != null)
                {
                    return callback;
                }
                
                throw new FaultException(MensajesError.Cliente.ErrorObtenerCallback);
            }

            throw new FaultException(MensajesError.Cliente.ErrorContextoOperacion);
        }
    }
}
