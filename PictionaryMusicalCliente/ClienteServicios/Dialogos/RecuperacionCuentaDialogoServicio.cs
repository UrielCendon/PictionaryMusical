using System;
using System.Threading.Tasks;
using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Vista;
using PictionaryMusicalCliente.VistaModelo.Perfil;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Dialogos
{
    /// <summary>
    /// Gestiona el flujo visual completo para la recuperacion de una cuenta de usuario.
    /// Orquesta la solicitud de codigo, verificacion y cambio de contrasena.
    /// </summary>
    public class RecuperacionCuentaDialogoServicio : IRecuperacionCuentaServicio
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(RecuperacionCuentaDialogoServicio));

        private readonly IVerificacionCodigoDialogoServicio _verificarCodigoDialogoServicio;
        private readonly IAvisoServicio _avisoServicio;
        private readonly SonidoManejador _sonidoManejador;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de recuperacion de cuenta.
        /// </summary>
        /// <param name="verificarCodigoDialogoServicio">
        /// Servicio para mostrar el dialogo de verificacion de codigo.
        /// </param>
        /// <param name="avisoServicio">Servicio para mostrar avisos al usuario.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si alguno de los parametros es nulo.
        /// </exception>
        public RecuperacionCuentaDialogoServicio(
            IVerificacionCodigoDialogoServicio verificarCodigoDialogoServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
        {
            _verificarCodigoDialogoServicio = verificarCodigoDialogoServicio ??
                throw new ArgumentNullException(nameof(verificarCodigoDialogoServicio));
            _avisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
        }

        /// <summary>
        /// Ejecuta el proceso de recuperacion de cuenta paso a paso.
        /// Incluye solicitud de codigo, verificacion y cambio de contrasena.
        /// </summary>
        /// <param name="identificador">
        /// Correo electronico o nombre de usuario de la cuenta a recuperar.
        /// </param>
        /// <param name="cambioContrasenaServicio">
        /// Servicio que ejecuta las operaciones de cambio de contrasena.
        /// </param>
        /// <returns>
        /// Resultado de la operacion indicando exito o fallo con mensaje descriptivo.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="cambioContrasenaServicio"/> es nulo.
        /// </exception>
        public async Task<DTOs.ResultadoOperacionDTO> RecuperarCuentaAsync(
            string identificador,
            ICambioContrasenaServicio cambioContrasenaServicio)
        {
            if (cambioContrasenaServicio == null)
            {
                throw new ArgumentNullException(nameof(cambioContrasenaServicio));
            }

            var resultadoSolicitud = await SolicitarCodigoRecuperacionAsync(
                identificador,
                cambioContrasenaServicio).ConfigureAwait(true);

            if (!resultadoSolicitud.Exito)
            {
                return resultadoSolicitud.Error;
            }

            NotificarCodigoEnviado();

            var resultadoVerificacion = await VerificarCodigoAsync(
                resultadoSolicitud.Token,
                cambioContrasenaServicio).ConfigureAwait(true);

            if (!resultadoVerificacion.Exito)
            {
                return resultadoVerificacion.Error;
            }

            NotificarCodigoVerificado();

            return await MostrarDialogoCambioContrasena(
                resultadoSolicitud.Token,
                cambioContrasenaServicio).ConfigureAwait(true);
        }

        private async static Task<(bool Exito, string Token, DTOs.ResultadoOperacionDTO Error)>
            SolicitarCodigoRecuperacionAsync(
                string identificador,
                ICambioContrasenaServicio servicio)
        {
            var respuesta = await servicio
                .SolicitarCodigoRecuperacionAsync(identificador)
                .ConfigureAwait(true);

            var validacion = ValidarRespuestaSolicitud(respuesta);
            if (!validacion.Exito)
            {
                return (false, null, validacion.Error);
            }

            return (true, respuesta.TokenCodigo, null);
        }

        private void NotificarCodigoEnviado()
        {
            _avisoServicio.Mostrar(Lang.avisoTextoCodigoEnviado);
        }

        private static (bool Exito, DTOs.ResultadoOperacionDTO Error) ValidarRespuestaSolicitud(
            DTOs.ResultadoSolicitudRecuperacionDTO respuesta)
        {
            if (respuesta == null)
            {
                return (false, CrearError(null, Lang.errorTextoServidorNoDisponible));
            }

            if (!respuesta.CuentaEncontrada)
            {
                return (false, CrearError(
                    respuesta.Mensaje,
                    Lang.errorTextoCuentaNoRegistrada));
            }

            if (!respuesta.CodigoEnviado)
            {
                return (false, CrearError(
                    respuesta.Mensaje,
                    Lang.errorTextoServidorSolicitudCambioContrasena));
            }

            return (true, null);
        }

        private async Task<(bool Exito, DTOs.ResultadoOperacionDTO Error)>
            VerificarCodigoAsync(string token, ICambioContrasenaServicio servicio)
        {
            var adaptador = CrearAdaptadorVerificacion(servicio);
            var respuestaDialogo = await MostrarDialogoVerificacionAsync(token, adaptador)
                .ConfigureAwait(true);

            return ValidarResultadoVerificacion(respuestaDialogo);
        }

        private static ICodigoVerificacionServicio CrearAdaptadorVerificacion(
            ICambioContrasenaServicio servicio)
        {
            return new CodigoRecuperacionServicioAdaptador(servicio);
        }

        private async Task<DTOs.ResultadoRegistroCuentaDTO> MostrarDialogoVerificacionAsync(
            string token,
            ICodigoVerificacionServicio adaptador)
        {
            var parametros = new VerificacionDialogoParametros(
                Lang.cambiarContrasenaTextoCodigoVerificacion,
                token,
                adaptador);

            return await _verificarCodigoDialogoServicio.MostrarDialogoAsync(
                parametros,
                _avisoServicio,
                _sonidoManejador).ConfigureAwait(true);
        }

        private static (bool Exito, DTOs.ResultadoOperacionDTO Error) ValidarResultadoVerificacion(
            DTOs.ResultadoRegistroCuentaDTO respuesta)
        {
            if (respuesta == null)
            {
                return (false, null);
            }

            if (!respuesta.RegistroExitoso)
            {
                _logger.Warn("Verificacion de codigo fallida.");
                return (false, CrearError(
                    respuesta.Mensaje,
                    Lang.errorTextoCodigoIncorrecto));
            }

            return (true, null);
        }

        private void NotificarCodigoVerificado()
        {
            _avisoServicio.Mostrar(Lang.avisoTextoCodigoVerificadoCambio);
        }

        private Task<DTOs.ResultadoOperacionDTO> MostrarDialogoCambioContrasena(
            string token,
            ICambioContrasenaServicio servicio)
        {
            var finalizacion = new TaskCompletionSource<DTOs.ResultadoOperacionDTO>();
            var ventana = CrearVentanaCambioContrasena();
            var vistaModelo = CrearVistaModeloCambioContrasena(token, servicio);

            ConfigurarCierreVentana(ventana, finalizacion);
            ConfigurarCallbackFinalizacion(vistaModelo, finalizacion);
            MostrarVentana(ventana, vistaModelo);

            return finalizacion.Task;
        }

        private static CambioContrasena CrearVentanaCambioContrasena()
        {
            return new CambioContrasena();
        }

        private CambioContrasenaVistaModelo CrearVistaModeloCambioContrasena(
            string token,
            ICambioContrasenaServicio servicio)
        {
            return new CambioContrasenaVistaModelo(
                App.VentanaServicio,
                App.Localizador,
                token,
                servicio,
                _avisoServicio,
                _sonidoManejador);
        }

        private static void ConfigurarCierreVentana(
            CambioContrasena ventana,
            TaskCompletionSource<DTOs.ResultadoOperacionDTO> tareaFinalizada)
        {
            ventana.Closed += (_, __) =>
            {
                if (!tareaFinalizada.Task.IsCompleted)
                {
                    tareaFinalizada.TrySetResult(null);
                }
            };
        }

        private static void ConfigurarCallbackFinalizacion(
            CambioContrasenaVistaModelo vistaModelo,
            TaskCompletionSource<DTOs.ResultadoOperacionDTO> finalizacion)
        {
            vistaModelo.CambioContrasenaFinalizada = resultado =>
            {
                if (finalizacion.Task.IsCompleted)
                {
                    return;
                }

                finalizacion.TrySetResult(resultado);
            };
        }

        private static void MostrarVentana(
            CambioContrasena ventana,
            CambioContrasenaVistaModelo vistaModelo)
        {
            ventana.DataContext = vistaModelo;
            ventana.ShowDialog();
        }

        private static DTOs.ResultadoOperacionDTO CrearError(
            string mensajeServidor,
            string mensajeContingencia)
        {
            string mensajeFinal = App.Localizador.Localizar(
                mensajeServidor,
                mensajeContingencia);

            return new DTOs.ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = mensajeFinal
            };
        }
    }
}