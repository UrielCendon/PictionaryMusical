using System;

namespace PictionaryMusicalServidor.Servicios.Excepciones
{
    /// <summary>
    /// Excepcion lanzada cuando ocurre un error al notificar a los clientes via callbacks WCF.
    /// Se utiliza para encapsular errores durante el envio de notificaciones en tiempo real.
    /// </summary>
    public class NotificacionCallbackExcepcion : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotificacionCallbackExcepcion"/>.
        /// </summary>
        public NotificacionCallbackExcepcion()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotificacionCallbackExcepcion"/> 
        /// con un mensaje especifico.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        public NotificacionCallbackExcepcion(string mensaje)
            : base(mensaje)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotificacionCallbackExcepcion"/> 
        /// con un mensaje y una excepcion interna.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        /// <param name="excepcionInterna">Excepcion que origino el error actual.</param>
        public NotificacionCallbackExcepcion(string mensaje, Exception excepcionInterna)
            : base(mensaje, excepcionInterna)
        {
        }
    }
}
