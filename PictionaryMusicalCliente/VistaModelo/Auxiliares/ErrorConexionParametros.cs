using System;

namespace PictionaryMusicalCliente.VistaModelo.Auxiliares
{
    /// <summary>
    /// Agrupa los parámetros necesarios para manejar errores de conexión.
    /// </summary>
    public sealed class ErrorConexionParametros
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ErrorConexionParametros"/>.
        /// </summary>
        /// <param name="excepcion">La excepción que ocurrió.</param>
        /// <param name="mensajeLog">Mensaje para el registro de logs.</param>
        /// <param name="mensajeDesconexion">
        /// Mensaje a mostrar cuando se redirige por desconexión.
        /// </param>
        /// <param name="mensajeError">
        /// Mensaje de error a mostrar cuando no se redirige.
        /// </param>
        /// <param name="redirigirEnDesconexion">
        /// Indica si se debe redirigir al inicio de sesión.
        /// </param>
        public ErrorConexionParametros(
            Exception excepcion,
            string mensajeLog,
            string mensajeDesconexion,
            string mensajeError,
            bool redirigirEnDesconexion)
        {
            Excepcion = excepcion;
            MensajeLog = mensajeLog ?? string.Empty;
            MensajeDesconexion = mensajeDesconexion ?? string.Empty;
            MensajeError = mensajeError ?? string.Empty;
            RedirigirEnDesconexion = redirigirEnDesconexion;
        }

        /// <summary>
        /// Obtiene la excepción que ocurrió.
        /// </summary>
        public Exception Excepcion { get; }

        /// <summary>
        /// Obtiene el mensaje para el registro de logs.
        /// </summary>
        public string MensajeLog { get; }

        /// <summary>
        /// Obtiene el mensaje a mostrar cuando se redirige por desconexión.
        /// </summary>
        public string MensajeDesconexion { get; }

        /// <summary>
        /// Obtiene el mensaje de error a mostrar cuando no se redirige.
        /// </summary>
        public string MensajeError { get; }

        /// <summary>
        /// Obtiene un valor que indica si se debe redirigir al inicio de sesión.
        /// </summary>
        public bool RedirigirEnDesconexion { get; }
    }
}
