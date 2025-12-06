using System;
using System.Globalization;
using System.Threading.Tasks;
using PictionaryMusicalCliente.ClienteServicios.Idiomas;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante
{
    /// <summary>
    /// Facilita la creacion y consumo de servicios WCF relacionados con la verificacion de 
    /// codigos.
    /// </summary>
    public static class VerificacionCodigoServicioAyudante
    {
        private const string CodigoVerificacionEndpoint =
            "BasicHttpBinding_ICodigoVerificacionManejador";
        private const string CuentaEndpoint =
            "BasicHttpBinding_ICuentaManejador";
        private const string CambioContrasenaEndpoint =
            "BasicHttpBinding_ICambioContrasenaManejador";

        /// <summary>
        /// Consume el servicio para solicitar un código de registro para una nueva cuenta.
        /// </summary>
        /// <param name="solicitud">El DTO con la información de la nueva cuenta.</param>
        /// <returns>El resultado de la solicitud del código.</returns>
        public static Task<DTOs.ResultadoSolicitudCodigoDTO> SolicitarCodigoRegistroAsync(
            DTOs.NuevaCuentaDTO solicitud)
        {
            if (solicitud == null)
            {
                throw new ArgumentNullException(nameof(solicitud));
            }

            PrepararSolicitudConIdioma(solicitud);

            var cliente = new PictionaryServidorServicioCodigoVerificacion
                .CodigoVerificacionManejadorClient(CodigoVerificacionEndpoint);

            return WcfClienteAyudante.UsarAsincronoAsync(
                cliente,
                c => c.SolicitarCodigoVerificacionAsync(solicitud));
        }

        /// <summary>
        /// Consume el servicio para solicitar un código de recuperación de cuenta dado un 
        /// identificador (correo o usuario).
        /// </summary>
        /// <param name="identificador">El correo electrónico o nombre de usuario.</param>
        /// <returns>El resultado de la solicitud de recuperación.</returns>
        public static Task<DTOs.ResultadoSolicitudRecuperacionDTO>
            SolicitarCodigoRecuperacionAsync(string identificador)
        {
            var solicitud = ConstruirSolicitudRecuperacion(identificador);
            var cliente = new PictionaryServidorServicioCodigoVerificacion
                .CodigoVerificacionManejadorClient(CodigoVerificacionEndpoint);

            return WcfClienteAyudante.UsarAsincronoAsync(
                cliente,
                c => c.SolicitarCodigoRecuperacionAsync(solicitud));
        }

        /// <summary>
        /// Envía el código ingresado por el usuario para validar el registro de la cuenta.
        /// </summary>
        /// <param name="tokenCodigo">El token asociado al código enviado previamente.</param>
        /// <param name="codigoIngresado">El código numérico ingresado por el usuario.</param>
        /// <returns>El resultado de la validación del registro.</returns>
        public static Task<DTOs.ResultadoRegistroCuentaDTO> ConfirmarCodigoRegistroAsync(
            string tokenCodigo,
            string codigoIngresado)
        {
            var solicitud = ConstruirConfirmacionCodigo(tokenCodigo, codigoIngresado);
            var cliente = new PictionaryServidorServicioCodigoVerificacion
                .CodigoVerificacionManejadorClient(CodigoVerificacionEndpoint);

            return WcfClienteAyudante.UsarAsincronoAsync(
                cliente,
                c => c.ConfirmarCodigoVerificacionAsync(solicitud));
        }

        /// <summary>
        /// Envía el código ingresado para validar la recuperación de una cuenta existente.
        /// </summary>
        /// <param name="tokenCodigo">El token asociado al código de recuperación.</param>
        /// <param name="codigoIngresado">El código numérico ingresado por el usuario.</param>
        /// <returns>El resultado de la operación de confirmación.</returns>
        public static Task<DTOs.ResultadoOperacionDTO> ConfirmarCodigoRecuperacionAsync(
            string tokenCodigo,
            string codigoIngresado)
        {
            var solicitud = ConstruirConfirmacionCodigo(tokenCodigo, codigoIngresado);
            var cliente = new PictionaryServidorServicioCodigoVerificacion
                .CodigoVerificacionManejadorClient(CodigoVerificacionEndpoint);

            return WcfClienteAyudante.UsarAsincronoAsync(
                cliente,
                c => c.ConfirmarCodigoRecuperacionAsync(solicitud));
        }

        /// <summary>
        /// Solicita el reenvío del código de verificación para el registro.
        /// </summary>
        /// <param name="tokenCodigo">El token del código previamente solicitado.</param>
        /// <returns>El resultado de la solicitud de reenvío.</returns>
        public static Task<DTOs.ResultadoSolicitudCodigoDTO> ReenviarCodigoRegistroAsync(
            string tokenCodigo)
        {
            var solicitud = new DTOs.ReenvioCodigoVerificacionDTO
            {
                TokenCodigo = tokenCodigo?.Trim()
            };

            var cliente = new PictionaryServidorServicioCuenta
                .CuentaManejadorClient(CuentaEndpoint);

            return WcfClienteAyudante.UsarAsincronoAsync(
                cliente,
                c => c.ReenviarCodigoVerificacionAsync(solicitud));
        }

        /// <summary>
        /// Solicita el reenvío del código para la recuperación de contraseña.
        /// </summary>
        /// <param name="tokenCodigo">El token del código previamente solicitado.</param>
        /// <returns>El resultado de la solicitud de reenvío.</returns>
        public static Task<DTOs.ResultadoSolicitudCodigoDTO> ReenviarCodigoRecuperacionAsync(
            string tokenCodigo)
        {
            var solicitud = new DTOs.ReenvioCodigoDTO
            {
                TokenCodigo = tokenCodigo?.Trim()
            };

            var cliente = new PictionaryServidorServicioCambioContrasena
                .CambioContrasenaManejadorClient(CambioContrasenaEndpoint);

            return WcfClienteAyudante.UsarAsincronoAsync(
                cliente,
                c => c.ReenviarCodigoRecuperacionAsync(solicitud));
        }

        private static void PrepararSolicitudConIdioma(DTOs.NuevaCuentaDTO solicitud)
        {
            solicitud.Idioma ??= ObtenerCodigoIdiomaActual();
        }

        private static DTOs.SolicitudRecuperarCuentaDTO ConstruirSolicitudRecuperacion(
            string identificador)
        {
            return new DTOs.SolicitudRecuperarCuentaDTO
            {
                Identificador = identificador?.Trim(),
                Idioma = ObtenerCodigoIdiomaActual()
            };
        }

        private static DTOs.ConfirmacionCodigoDTO ConstruirConfirmacionCodigo(
            string tokenCodigo,
            string codigoIngresado)
        {
            return new DTOs.ConfirmacionCodigoDTO
            {
                TokenCodigo = tokenCodigo,
                CodigoIngresado = codigoIngresado?.Trim()
            };
        }

        private static string ObtenerCodigoIdiomaActual()
        {
            return LocalizacionServicio.Instancia.CulturaActual?.Name
                ?? CultureInfo.CurrentUICulture?.Name;
        }
    }
}