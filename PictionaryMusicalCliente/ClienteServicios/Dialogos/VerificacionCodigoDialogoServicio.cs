using System;
using System.Threading.Tasks;
using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.VistaModelo.Perfil;
using ICodigoVerificacionCli = PictionaryMusicalCliente.ClienteServicios.
    Abstracciones.ICodigoVerificacionServicio;
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
        /// Muestra el dialogo de verificacion y retorna el resultado.
        /// </summary>
        public Task<DTOs.ResultadoRegistroCuentaDTO> MostrarDialogoAsync(
            string descripcion,
            string tokenCodigo,
            ICodigoVerificacionCli codigoVerificacionServicio)
        {
            ValidarServicio(codigoVerificacionServicio);

            var finalizacion = new TaskCompletionSource<DTOs.ResultadoRegistroCuentaDTO>();
            var ventana = new VerificacionCodigo();

            var vistaModelo = CrearVistaModelo(
                descripcion,
                tokenCodigo,
                codigoVerificacionServicio);

            ConfigurarEventos(vistaModelo, ventana, finalizacion);
            ConfigurarCierreVentana(ventana, finalizacion);

            ventana.ConfigurarVistaModelo(vistaModelo);
            ventana.ShowDialog();

            return finalizacion.Task;
        }

        private void ValidarServicio(ICodigoVerificacionCli servicio)
        {
            if (servicio == null)
            {
                var ex = new ArgumentNullException(nameof(servicio));
                _logger.Error("Intento de abrir dialogo de verificacion con servicio nulo.", ex);
                throw ex;
            }
        }

        private VerificacionCodigoVistaModelo CrearVistaModelo(
            string descripcion,
            string token,
            ICodigoVerificacionCli servicio)
        {
            return new VerificacionCodigoVistaModelo(descripcion, token, servicio);
        }

        private void ConfigurarEventos(
            VerificacionCodigoVistaModelo vistaModelo,
            VerificacionCodigo ventana,
            TaskCompletionSource<DTOs.ResultadoRegistroCuentaDTO> finalizacion)
        {
            vistaModelo.VerificacionCompletada = resultado =>
            {
                _logger.Info("Verificacion de codigo completada exitosamente.");
                finalizacion.TrySetResult(resultado);
                ventana.Close();
            };

            vistaModelo.Cancelado = () =>
            {
                finalizacion.TrySetResult(null);
                ventana.Close();
            };
        }

        private void ConfigurarCierreVentana(
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
    }
}