using System;

namespace PictionaryMusicalServidor.Servicios.Excepciones
{
    /// <summary>
    /// Excepcion lanzada cuando ocurre un error en la comunicacion WCF entre cliente y servidor.
    /// Se utiliza para encapsular errores de CommunicationException y TimeoutException.
    /// </summary>
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
    }
}
