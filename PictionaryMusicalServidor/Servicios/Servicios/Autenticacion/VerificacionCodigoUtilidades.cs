using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Utilidades comunes para los servicios de verificacion de codigos.
    /// Proporciona metodos de validacion y creacion de resultados reutilizables.
    /// </summary>
    internal static class VerificacionCodigoUtilidades
    {
        /// <summary>
        /// Valida los datos de confirmacion de un codigo.
        /// </summary>
        /// <param name="confirmacion">Datos de confirmacion a validar.</param>
        /// <returns>True si los datos son validos, false en caso contrario.</returns>
        public static bool ValidarDatosConfirmacion(ConfirmacionCodigoDTO confirmacion)
        {
            if (confirmacion == null)
            {
                return false;
            }

            string token = EntradaComunValidador.NormalizarTexto(confirmacion.TokenCodigo);
            string codigo = EntradaComunValidador.NormalizarTexto(confirmacion.CodigoIngresado);

            return EntradaComunValidador.EsTokenValido(token) &&
                   EntradaComunValidador.EsCodigoVerificacionValido(codigo);
        }

        /// <summary>
        /// Valida que un token de reenvio sea valido.
        /// </summary>
        /// <param name="token">Token a validar.</param>
        /// <returns>True si el token es valido, false en caso contrario.</returns>
        public static bool ValidarToken(string token)
        {
            string tokenNormalizado = EntradaComunValidador.NormalizarTexto(token);
            return EntradaComunValidador.EsTokenValido(tokenNormalizado);
        }

        /// <summary>
        /// Crea un resultado de fallo para reenvio de codigo.
        /// </summary>
        /// <param name="mensaje">Mensaje de error.</param>
        /// <returns>DTO con resultado de fallo.</returns>
        public static ResultadoSolicitudCodigoDTO CrearFalloReenvio(string mensaje)
        {
            return new ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = false,
                Mensaje = mensaje
            };
        }

        /// <summary>
        /// Crea un resultado de operacion fallida.
        /// </summary>
        /// <param name="mensaje">Mensaje de error.</param>
        /// <returns>DTO con resultado de fallo.</returns>
        public static ResultadoOperacionDTO CrearFalloOperacion(string mensaje)
        {
            return new ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = mensaje
            };
        }
    }
}
