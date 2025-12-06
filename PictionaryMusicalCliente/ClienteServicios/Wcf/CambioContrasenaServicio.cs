using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante;
using PictionaryMusicalCliente.Properties.Langs;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Provee servicios para el flujo de recuperacion y cambio de contraseña.
    /// </summary>
    public class CambioContrasenaServicio : ICambioContrasenaServicio
    {
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(CambioContrasenaServicio));
        private const string Endpoint = "BasicHttpBinding_ICambioContrasenaManejador";

        /// <summary>
        /// Inicia el proceso de recuperacion solicitando un codigo al identificador dado.
        /// </summary>
        public async Task<DTOs.ResultadoSolicitudRecuperacionDTO> SolicitarCodigoRecuperacionAsync(
            string identificador)
        {
            ValidarTextoObligatorio(
                identificador,
                Lang.errorTextoIdentificadorRecuperacionRequerido,
                nameof(identificador));

            DTOs.ResultadoSolicitudRecuperacionDTO resultado =
                await EjecutarConManejoDeErroresAsync(
                () => VerificacionCodigoServicioAyudante.SolicitarCodigoRecuperacionAsync
                (identificador),
                Lang.errorTextoServidorSolicitudCambioContrasena
            ).ConfigureAwait(false);

            return MapearResultadoSolicitudRecuperacion(resultado);
        }

        /// <summary>
        /// Solicita el reenvio del codigo de recuperacion en caso de no haberlo recibido.
        /// </summary>
        public async Task<DTOs.ResultadoSolicitudCodigoDTO> ReenviarCodigoRecuperacionAsync(
            string tokenCodigo)
        {
            ValidarTextoObligatorio(
                tokenCodigo,
                Lang.errorTextoTokenCodigoObligatorio,
                nameof(tokenCodigo));

            DTOs.ResultadoSolicitudCodigoDTO resultado = await EjecutarConManejoDeErroresAsync(
                () => VerificacionCodigoServicioAyudante.ReenviarCodigoRecuperacionAsync
                (tokenCodigo),
                Lang.errorTextoServidorReenviarCodigo
            ).ConfigureAwait(false);

            return MapearResultadoSolicitudCodigo(resultado);
        }

        /// <summary>
        /// Verifica que el codigo ingresado para recuperacion sea valido.
        /// </summary>
        public async Task<DTOs.ResultadoOperacionDTO> ConfirmarCodigoRecuperacionAsync(
            string tokenCodigo,
            string codigoIngresado)
        {
            ValidarTextoObligatorio(
                tokenCodigo,
                Lang.errorTextoTokenCodigoObligatorio,
                nameof(tokenCodigo));

            ValidarTextoObligatorio(
                codigoIngresado,
                Lang.errorTextoCodigoVerificacionRequerido,
                nameof(codigoIngresado));

            DTOs.ResultadoOperacionDTO resultado = await EjecutarConManejoDeErroresAsync(
                () => VerificacionCodigoServicioAyudante.ConfirmarCodigoRecuperacionAsync(
                    tokenCodigo,
                    codigoIngresado),
                Lang.errorTextoServidorValidarCodigo
            ).ConfigureAwait(false);

            return MapearResultadoOperacion(resultado);
        }

        /// <summary>
        /// Establece la nueva contraseña despues de haber verificado el codigo.
        /// </summary>
        public async Task<DTOs.ResultadoOperacionDTO> ActualizarContrasenaAsync(
            string tokenCodigo,
            string nuevaContrasena)
        {
            ValidarTextoObligatorio(
                tokenCodigo,
                Lang.errorTextoTokenCodigoObligatorio,
                nameof(tokenCodigo));

            ValidarContrasena(nuevaContrasena);

            var cliente = new PictionaryServidorServicioCambioContrasena
                .CambioContrasenaManejadorClient(Endpoint);

            DTOs.ActualizacionContrasenaDTO solicitud = CrearSolicitudCambioContrasena(
                tokenCodigo,
                nuevaContrasena);

            DTOs.ResultadoOperacionDTO resultado = await EjecutarConManejoDeErroresAsync(
                () => WcfClienteAyudante.UsarAsincronoAsync(
                    cliente,
                    c => c.ActualizarContrasenaAsync(solicitud)),
                Lang.errorTextoServidorActualizarContrasena
            ).ConfigureAwait(false);

            RegistrarActualizacionExitosa(resultado);

            return MapearResultadoOperacion(resultado);
        }

        private static void ValidarTextoObligatorio(
            string valor,
            string mensajeError,
            string nombreParametro)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException(mensajeError, nombreParametro);
            }
        }

        private static void ValidarContrasena(string nuevaContrasena)
        {
            if (string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                throw new ArgumentNullException(nameof(nuevaContrasena));
            }
        }

        private static DTOs.ActualizacionContrasenaDTO CrearSolicitudCambioContrasena(
            string tokenCodigo,
            string nuevaContrasena)
        {
            return new DTOs.ActualizacionContrasenaDTO
            {
                TokenCodigo = tokenCodigo,
                NuevaContrasena = nuevaContrasena
            };
        }

        private static DTOs.ResultadoSolicitudRecuperacionDTO MapearResultadoSolicitudRecuperacion(
            DTOs.ResultadoSolicitudRecuperacionDTO resultado)
        {
            return resultado == null ? null : new DTOs.ResultadoSolicitudRecuperacionDTO
            {
                CuentaEncontrada = resultado.CuentaEncontrada,
                CodigoEnviado = resultado.CodigoEnviado,
                CorreoDestino = resultado.CorreoDestino,
                Mensaje = resultado.Mensaje,
                TokenCodigo = resultado.TokenCodigo
            };
        }

        private static DTOs.ResultadoSolicitudCodigoDTO MapearResultadoSolicitudCodigo(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            return resultado == null ? null : new DTOs.ResultadoSolicitudCodigoDTO
            {
                CodigoEnviado = resultado.CodigoEnviado,
                Mensaje = resultado.Mensaje,
                TokenCodigo = resultado.TokenCodigo
            };
        }

        private static DTOs.ResultadoOperacionDTO MapearResultadoOperacion(
            DTOs.ResultadoOperacionDTO resultado)
        {
            return resultado == null ? null : new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = resultado.OperacionExitosa,
                Mensaje = resultado.Mensaje
            };
        }

        private void RegistrarActualizacionExitosa(DTOs.ResultadoOperacionDTO resultado)
        {
            if (resultado != null && resultado.OperacionExitosa)
            {
                _logger.Info("Contraseña actualizada mediante recuperación.");
            }
        }

        private static async Task<TResult> EjecutarConManejoDeErroresAsync<TResult>(
            Func<Task<TResult>> operacion,
            string mensajeFallaPredeterminado)
        {
            try
            {
                return await operacion().ConfigureAwait(false);
            }
            catch (FaultException ex)
            {
                _logger.Warn("Error de servidor controlado en flujo de cambio de contrasena.", ex);
                string mensaje = ErrorServicioAyudante.ObtenerMensaje(
                    ex,
                    mensajeFallaPredeterminado);
                throw new ServicioExcepcion(TipoErrorServicio.FallaServicio, mensaje, ex);
            }
            catch (EndpointNotFoundException ex)
            {
                _logger.Error("Endpoint no encontrado en flujo de cambio de contrasena.", ex);
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible,
                    ex);
            }
            catch (TimeoutException ex)
            {
                _logger.Error("Timeout en flujo de cambio de contrasena.", ex);
                throw new ServicioExcepcion(
                    TipoErrorServicio.TiempoAgotado,
                    Lang.errorTextoServidorTiempoAgotado,
                    ex);
            }
            catch (CommunicationException ex)
            {
                _logger.Error("Error de comunicación en flujo de cambio de contrasena.", ex);
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible,
                    ex);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error("Operación inválida en flujo de cambio de contrasena.", ex);
                throw new ServicioExcepcion(
                    TipoErrorServicio.OperacionInvalida,
                    Lang.errorTextoPrepararSolicitudCambioContrasena,
                    ex);
            }
        }
    }
}