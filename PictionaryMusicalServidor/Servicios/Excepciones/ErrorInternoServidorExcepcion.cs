using System;

namespace PictionaryMusicalServidor.Servicios.Excepciones
{
    /// <summary>
    /// Excepcion lanzada cuando ocurre un error interno inesperado en el servidor
    /// que no puede ser clasificado en otras categorias especificas.
    /// </summary>
    public class ErrorInternoServidorExcepcion : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ErrorInternoServidorExcepcion"/>.
        /// </summary>
        public ErrorInternoServidorExcepcion()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ErrorInternoServidorExcepcion"/> 
        /// con un mensaje especifico.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        public ErrorInternoServidorExcepcion(string mensaje)
            : base(mensaje)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ErrorInternoServidorExcepcion"/> 
        /// con un mensaje y una excepcion interna.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        /// <param name="excepcionInterna">Excepcion que origino el error actual.</param>
        public ErrorInternoServidorExcepcion(string mensaje, Exception excepcionInterna)
            : base(mensaje, excepcionInterna)
        {
        }
    }
}
