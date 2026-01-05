using System;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Implementacion por defecto del proveedor de contexto de operacion.
    /// Accede directamente a OperationContext.Current de WCF.
    /// </summary>
    public class ProveedorContextoOperacion : IProveedorContextoOperacion
    {
        /// <summary>
        /// Indica si existe un contexto de operacion activo.
        /// </summary>
        public bool ExisteContexto
        {
            get { return OperationContext.Current != null; }
        }

        /// <summary>
        /// Obtiene el canal de callback del tipo especificado desde el contexto actual.
        /// </summary>
        /// <typeparam name="T">Tipo del contrato de callback.</typeparam>
        /// <returns>Instancia del canal de callback.</returns>
        public T ObtenerCallbackChannel<T>()
        {
            var contexto = OperationContext.Current;
            if (contexto == null)
            {
                return default(T);
            }

            return contexto.GetCallbackChannel<T>();
        }

        /// <summary>
        /// Obtiene el canal de comunicacion actual.
        /// </summary>
        /// <returns>Canal de comunicacion o null si no hay contexto.</returns>
        public IContextChannel ObtenerCanalActual()
        {
            return OperationContext.Current?.Channel;
        }
    }
}
