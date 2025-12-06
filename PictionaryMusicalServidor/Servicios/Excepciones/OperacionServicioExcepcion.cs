using System;
using System.Runtime.Serialization;

namespace PictionaryMusicalServidor.Servicios.Excepciones
{
    /// <summary>
    /// Excepcion lanzada cuando ocurre un error inesperado durante la ejecucion 
    /// de una operacion de servicio que no puede ser manejado de manera especifica.
    /// </summary>
    [Serializable]
    public class OperacionServicioExcepcion : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OperacionServicioExcepcion"/>.
        /// </summary>
        public OperacionServicioExcepcion()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OperacionServicioExcepcion"/> 
        /// con un mensaje especifico.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        public OperacionServicioExcepcion(string mensaje)
            : base(mensaje)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OperacionServicioExcepcion"/> 
        /// con un mensaje y una excepcion interna.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        /// <param name="excepcionInterna">Excepcion que origino el error actual.</param>
        public OperacionServicioExcepcion(string mensaje, Exception excepcionInterna)
            : base(mensaje, excepcionInterna)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="OperacionServicioExcepcion"/> 
        /// con datos serializados.
        /// </summary>
        /// <param name="info">Datos de serializacion.</param>
        /// <param name="context">Contexto de streaming.</param>
        protected OperacionServicioExcepcion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
