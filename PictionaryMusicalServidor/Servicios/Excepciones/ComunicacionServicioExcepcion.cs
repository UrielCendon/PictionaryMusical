using System;
using System.Runtime.Serialization;

namespace PictionaryMusicalServidor.Servicios.Excepciones
{
    /// <summary>
    /// Excepcion lanzada cuando ocurre un error de comunicacion en los servicios WCF.
    /// Incluye timeouts, canales cerrados y otras fallas de comunicacion.
    /// </summary>
    [Serializable]
    public class ComunicacionServicioExcepcion : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ComunicacionServicioExcepcion"/>.
        /// </summary>
        public ComunicacionServicioExcepcion()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ComunicacionServicioExcepcion"/> 
        /// con un mensaje especifico.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        public ComunicacionServicioExcepcion(string mensaje)
            : base(mensaje)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ComunicacionServicioExcepcion"/> 
        /// con un mensaje y una excepcion interna.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        /// <param name="excepcionInterna">Excepcion que origino el error actual.</param>
        public ComunicacionServicioExcepcion(string mensaje, Exception excepcionInterna)
            : base(mensaje, excepcionInterna)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ComunicacionServicioExcepcion"/> 
        /// con datos serializados.
        /// </summary>
        /// <param name="info">Datos de serializacion.</param>
        /// <param name="context">Contexto de streaming.</param>
        protected ComunicacionServicioExcepcion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
