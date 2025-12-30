using PictionaryMusicalCliente.Utilidades;
using System;

namespace PictionaryMusicalCliente.ClienteServicios.Abstracciones
{
    /// <summary>
    /// Agrupa los parámetros necesarios para mostrar el diálogo de verificación de código.
    /// </summary>
    public class VerificacionDialogoParametros
    {
        /// <summary>
        /// Inicializa una nueva instancia de los parámetros de verificación.
        /// </summary>
        /// <param name="descripcion">Texto descriptivo que se muestra al usuario.</param>
        /// <param name="tokenCodigo">Token asociado al código de verificación.</param>
        /// <param name="codigoVerificacionServicio">Servicio que valida el código.</param>
        /// <exception cref="ArgumentNullException">
        /// Si el servicio de verificación es nulo.
        /// </exception>
        public VerificacionDialogoParametros(
            string descripcion,
            string tokenCodigo,
            ICodigoVerificacionServicio codigoVerificacionServicio)
        {
            Descripcion = descripcion;
            TokenCodigo = tokenCodigo;
            CodigoVerificacionServicio = codigoVerificacionServicio ??
                throw new ArgumentNullException(nameof(codigoVerificacionServicio));
        }

        /// <summary>
        /// Texto descriptivo que se muestra al usuario en el diálogo.
        /// </summary>
        public string Descripcion { get; }

        /// <summary>
        /// Token asociado al código de verificación enviado al usuario.
        /// </summary>
        public string TokenCodigo { get; }

        /// <summary>
        /// Servicio que valida el código ingresado por el usuario.
        /// </summary>
        public ICodigoVerificacionServicio CodigoVerificacionServicio { get; }
    }
}
