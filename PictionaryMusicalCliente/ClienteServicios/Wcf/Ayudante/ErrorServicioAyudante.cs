using System;
using System.Reflection;
using System.ServiceModel;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante
{
    /// <summary>
    /// Ayudante para extraer mensajes amigables de excepciones WCF.
    /// </summary>
    public static class ErrorServicioAyudante
    {
        /// <summary>
        /// Obtiene un mensaje localizado a partir de una excepcion FaultException.
        /// </summary>
        /// <param name="excepcion">La excepcion capturada.</param>
        /// <param name="mensajePredeterminado">Mensaje a retornar si no se encuentra uno 
        /// especifico.</param>
        /// <returns>Mensaje localizado listo para mostrar.</returns>
        public static string ObtenerMensaje(
            FaultException excepcion,
            string mensajePredeterminado)
        {
            string mensajeDetalle = ObtenerMensajeDetalle(excepcion);
            string mensajeExcepcion = excepcion?.Message;

            return DeterminarMensajeFinal(
                mensajeDetalle,
                mensajeExcepcion,
                mensajePredeterminado);
        }

        private static string DeterminarMensajeFinal(
            string detalle,
            string mensajeBase,
            string predeterminado)
        {
            if (!string.IsNullOrWhiteSpace(detalle))
            {
                return MensajeServidorAyudante.Localizar(detalle, predeterminado);
            }

            if (!string.IsNullOrWhiteSpace(mensajeBase))
            {
                return MensajeServidorAyudante.Localizar(mensajeBase, predeterminado);
            }

            return MensajeServidorAyudante.Localizar(null, predeterminado);
        }

        private static string ObtenerMensajeDetalle(FaultException excepcion)
        {
            if (excepcion == null)
            {
                return null;
            }

            Type tipoExcepcion = excepcion.GetType();

            if (!EsFaultExceptionGenerica(tipoExcepcion))
            {
                return null;
            }

            object detalle = ObtenerObjetoDetalle(excepcion, tipoExcepcion);
            return ObtenerTextoMensajeDeDetalle(detalle);
        }

        private static bool EsFaultExceptionGenerica(Type tipo)
        {
            return tipo.GetTypeInfo().IsGenericType &&
                   tipo.GetGenericTypeDefinition() == typeof(FaultException<>);
        }

        private static object ObtenerObjetoDetalle(FaultException excepcion, Type tipo)
        {
            PropertyInfo detallePropiedad = tipo.GetRuntimeProperty("Detail");
            return detallePropiedad?.GetValue(excepcion);
        }

        private static string ObtenerTextoMensajeDeDetalle(object detalle)
        {
            if (detalle == null)
            {
                return null;
            }

            PropertyInfo mensajePropiedad = detalle.GetType().GetRuntimeProperty("Mensaje");
            return mensajePropiedad?.GetValue(detalle) as string;
        }
    }
}