using System;
using System.Threading.Tasks;
using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Vista;
using PictionaryMusicalCliente.VistaModelo.Perfil;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Dialogos
{
    /// <summary>
    /// Servicio de dialogo para manejar la interfaz de verificacion de codigo.
    /// </summary>
    public class VerificacionCodigoDialogoServicio : IVerificacionCodigoDialogoServicio
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(VerificacionCodigoDialogoServicio));

        /// <summary>
        /// Muestra el dialogo de verificacion de codigo y retorna el resultado.
        /// </summary>
        /// <param name="descripcion">
        /// Texto descriptivo que se muestra al usuario en el dialogo.
        /// </param>
        /// <param name="tokenCodigo">
        /// Token asociado al codigo de verificacion enviado al usuario.
        /// </param>
        /// <param name="codigoVerificacionServicio">
        /// Servicio que valida el codigo ingresado por el usuario.
        /// </param>
        /// <param name="avisoServicio">Servicio para mostrar avisos al usuario.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <returns>
        /// Resultado de la verificacion, o null si el usuario cancelo el dialogo.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="codigoVerificacionServicio"/>,
        /// <paramref name="avisoServicio"/> o <paramref name="sonidoManejador"/> es nulo.
        /// </exception>
        public Task<DTOs.ResultadoRegistroCuentaDTO> MostrarDialogoAsync(
            string descripcion,
            string tokenCodigo,
            ICodigoVerificacionServicio codigoVerificacionServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
        {
            ValidarParametros(codigoVerificacionServicio, avisoServicio, sonidoManejador);

            var finalizacion = new TaskCompletionSource<DTOs.ResultadoRegistroCuentaDTO>();
            var ventana = CrearVentanaVerificacion();
            var vistaModelo = CrearVistaModelo(
                descripcion,
                tokenCodigo,
                codigoVerificacionServicio,
                avisoServicio,
                sonidoManejador);

            ConfigurarEventoVerificacionCompletada(vistaModelo, ventana, finalizacion);
            ConfigurarEventoCancelacion(vistaModelo, ventana, finalizacion);
            ConfigurarEventoCierreVentana(ventana, finalizacion);
            MostrarVentana(ventana, vistaModelo);

            return finalizacion.Task;
        }

        private static void ValidarParametros(
            ICodigoVerificacionServicio codigoVerificacionServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
        {
            if (codigoVerificacionServicio == null)
            {
                throw new ArgumentNullException(nameof(codigoVerificacionServicio));
            }

            if (avisoServicio == null)
            {
                throw new ArgumentNullException(nameof(avisoServicio));
            }

            if (sonidoManejador == null)
            {
                throw new ArgumentNullException(nameof(sonidoManejador));
            }
        }

        private static VerificacionCodigo CrearVentanaVerificacion()
        {
            return new VerificacionCodigo();
        }

        private static VerificacionCodigoVistaModelo CrearVistaModelo(
            string descripcion,
            string tokenCodigo,
            ICodigoVerificacionServicio codigoVerificacionServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
        {
            return new VerificacionCodigoVistaModelo(
                App.VentanaServicio,
                App.Localizador,
                descripcion,
                tokenCodigo,
                codigoVerificacionServicio,
                avisoServicio,
                sonidoManejador);
        }

        private static void ConfigurarEventoVerificacionCompletada(
            VerificacionCodigoVistaModelo vistaModelo,
            VerificacionCodigo ventana,
            TaskCompletionSource<DTOs.ResultadoRegistroCuentaDTO> finalizacion)
        {
            vistaModelo.VerificacionCompletada = resultado =>
            {
                RegistrarVerificacionExitosa();
                finalizacion.TrySetResult(resultado);
                ventana.Close();
            };
        }

        private static void RegistrarVerificacionExitosa()
        {
            _logger.Info("Verificacion de codigo completada exitosamente.");
        }

        private static void ConfigurarEventoCancelacion(
            VerificacionCodigoVistaModelo vistaModelo,
            VerificacionCodigo ventana,
            TaskCompletionSource<DTOs.ResultadoRegistroCuentaDTO> finalizacion)
        {
            vistaModelo.Cancelado = () =>
            {
                finalizacion.TrySetResult(null);
                ventana.Close();
            };
        }

        private static void ConfigurarEventoCierreVentana(
            VerificacionCodigo ventana,
            TaskCompletionSource<DTOs.ResultadoRegistroCuentaDTO> finalizacion)
        {
            ventana.Closed += (_, __) =>
            {
                if (!finalizacion.Task.IsCompleted)
                {
                    finalizacion.TrySetResult(null);
                }
            };
        }

        private static void MostrarVentana(
            VerificacionCodigo ventana,
            VerificacionCodigoVistaModelo vistaModelo)
        {
            ventana.DataContext = vistaModelo;
            ventana.ShowDialog();
        }
    }
}