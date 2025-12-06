using System;

namespace PictionaryMusicalServidor.Datos.Excepciones
{
    /// <summary>
    /// Excepcion lanzada cuando ocurre un error durante el acceso a la base de datos.
    /// Se utiliza para encapsular errores de Entity Framework, SQL y otros problemas de persistencia.
    /// </summary>
    public class AccesoDatosExcepcion : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AccesoDatosExcepcion"/>.
        /// </summary>
        public AccesoDatosExcepcion()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AccesoDatosExcepcion"/> 
        /// con un mensaje especifico.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        public AccesoDatosExcepcion(string mensaje)
            : base(mensaje)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AccesoDatosExcepcion"/> 
        /// con un mensaje y una excepcion interna.
        /// </summary>
        /// <param name="mensaje">Mensaje descriptivo del error.</param>
        /// <param name="excepcionInterna">Excepcion que origino el error actual.</param>
        public AccesoDatosExcepcion(string mensaje, Exception excepcionInterna)
            : base(mensaje, excepcionInterna)
        {
        }
    }
}
