using System;
using System.Runtime.Serialization;

namespace PictionaryMusicalServidor.Servicios.Excepciones
{
    /// <summary>
    /// Excepcion lanzada cuando ocurre un error al interactuar con servicios externos
    /// como servicios de correo electronico, APIs de terceros, etc.
    /// </summary>
    [Serializable]
    public class ServicioExternoExcepcion : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServicioExternoExcepcion"/>.
        /// </summary>
        public ServicioExternoExcepcion()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServicioExternoExcepcion"/> 
        /// con un mensaje especifico.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        public ServicioExternoExcepcion(string mensaje)
            : base(mensaje)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServicioExternoExcepcion"/> 
        /// con un mensaje y una excepcion interna.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        /// <param name="excepcionInterna">Excepcion que origino el error actual.</param>
        public ServicioExternoExcepcion(string mensaje, Exception excepcionInterna)
            : base(mensaje, excepcionInterna)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServicioExternoExcepcion"/> 
        /// con datos serializados.
        /// </summary>
        /// <param name="info">Datos de serializacion.</param>
        /// <param name="context">Contexto de streaming.</param>
        protected ServicioExternoExcepcion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
