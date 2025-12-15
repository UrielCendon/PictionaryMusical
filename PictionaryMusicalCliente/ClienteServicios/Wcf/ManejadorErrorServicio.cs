using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;
using System.Reflection;
using System.ServiceModel;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Ayudante para extraer mensajes amigables de excepciones WCF.
    /// </summary>
    public class ManejadorErrorServicio : IManejadorErrorServicio
    {
        private readonly ILocalizadorServicio _localizador;

        /// <summary>
        /// Inicializa una nueva instancia del manejador de errores con el servicio de 
        /// localizacion necesario.
        /// </summary>
        /// <param name="localizador">El servicio encargado de traducir los mensajes extraidos.
        /// </param>
        /// <exception cref="ArgumentNullException">Se lanza si el localizador es nulo.</exception>
        public ManejadorErrorServicio(ILocalizadorServicio localizador)
        {
            _localizador = localizador ?? 
                throw new ArgumentNullException(nameof(localizador));
        }

        /// <summary>
        /// Obtiene un mensaje localizado a partir de una excepcion FaultException.
        /// </summary>
        /// <param name="excepcion">La excepcion capturada.</param>
        /// <param name="mensajePredeterminado">Mensaje a retornar si no se encuentra uno 
        /// especifico.</param>
        /// <returns>Mensaje localizado listo para mostrar.</returns>
        public string ObtenerMensaje(
            FaultException excepcion,
            string mensajePredeterminado)
        {
            string mensajeDetalle = ObtenerMensajeDetalle(excepcion);
            string mensajeExcepcion = excepcion?.Message ?? string.Empty;

            return DeterminarMensajeFinal(
                mensajeDetalle,
                mensajeExcepcion,
                mensajePredeterminado);
        }

        private string DeterminarMensajeFinal(
            string detalle,
            string mensajeBase,
            string predeterminado)
        {
            if (!string.IsNullOrWhiteSpace(detalle))
            {
                return _localizador.Localizar(detalle, predeterminado);
            }

            if (!string.IsNullOrWhiteSpace(mensajeBase))
            {
                return _localizador.Localizar(mensajeBase, predeterminado);
            }

            return _localizador.Localizar(string.Empty, predeterminado);
        }

        private static string ObtenerMensajeDetalle(FaultException excepcion)
        {
            if (excepcion == null)
            {
                return string.Empty;
            }

            Type tipoExcepcion = excepcion.GetType();

            if (!EsFaultExceptionGenerica(tipoExcepcion))
            {
                return string.Empty;
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
                return string.Empty;
            }

            PropertyInfo mensajePropiedad = detalle.GetType().GetRuntimeProperty("Mensaje");
            return mensajePropiedad?.GetValue(detalle) as string ?? string.Empty;
        }
    }
}