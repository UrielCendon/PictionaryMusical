using System;
using System.Runtime.Serialization;

namespace PictionaryMusicalServidor.Datos.Excepciones
{
    /// <summary>
    /// Excepcion personalizada para errores de acceso a la base de datos.
    /// Permite encapsular excepciones de bajo nivel con contexto adicional.
    /// </summary>
    [Serializable]
    public class BaseDatosExcepcion : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la excepcion.
        /// </summary>
        public BaseDatosExcepcion()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia con un mensaje de error.
        /// </summary>
        /// <param name="mensaje">Mensaje que describe el error.</param>
        public BaseDatosExcepcion(string mensaje) : base(mensaje)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia con un mensaje y la excepcion interna.
        /// </summary>
        /// <param name="mensaje">Mensaje que describe el error.</param>
        /// <param name="innerException">Excepcion interna que causo este error.</param>
        public BaseDatosExcepcion(string mensaje, Exception innerException)
            : base(mensaje, innerException)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia con datos de serializacion.
        /// </summary>
        /// <param name="info">Informacion de serializacion.</param>
        /// <param name="context">Contexto de streaming.</param>
        protected BaseDatosExcepcion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
