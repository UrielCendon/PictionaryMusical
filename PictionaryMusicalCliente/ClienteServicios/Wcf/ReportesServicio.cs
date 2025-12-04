using System;
using System.ServiceModel;
using System.Threading.Tasks;
using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Ayudante;
using PictionaryMusicalCliente.Properties.Langs;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Gestiona el envío de reportes de jugadores hacia el servidor.
    /// </summary>
    public class ReportesServicio : IReportesServicio
    {
        private const string ReportesEndpoint = "BasicHttpBinding_IReportesManejador";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReportesServicio));

        /// <inheritdoc />
        public async Task<DTOs.ResultadoOperacionDTO> ReportarJugadorAsync(
            DTOs.ReporteJugadorDTO reporte)
        {
            if (reporte == null)
            {
                throw new ArgumentNullException(nameof(reporte));
            }

            var cliente = new PictionaryServidorServicioReportes.ReportesManejadorClient(
                ReportesEndpoint);

            try
            {
                DTOs.ResultadoOperacionDTO resultado = await WcfClienteAyudante
                    .UsarAsincronoAsync(cliente, c => c.ReportarJugadorAsync(reporte))
                    .ConfigureAwait(false);

                if (resultado != null)
                {
                    resultado.Mensaje = MensajeServidorAyudante.Localizar(
                        resultado.Mensaje,
                        Lang.errorTextoReportarJugador);
                }

                if (resultado?.OperacionExitosa == true)
                {
                    Logger.InfoFormat(
                        "Reporte enviado por {0} contra {1}.",
                        reporte.NombreUsuarioReportante,
                        reporte.NombreUsuarioReportado);
                }
                else
                {
                    Logger.WarnFormat(
                        "No se pudo completar el reporte para {0}. Mensaje: {1}",
                        reporte.NombreUsuarioReportado,
                        resultado?.Mensaje);
                }

                return resultado;
            }
            catch (FaultException ex)
            {
                Logger.Warn("Fallo al reportar jugador (FaultException).", ex);
                string mensaje = ErrorServicioAyudante.ObtenerMensaje(
                    ex,
                    Lang.errorTextoReportarJugador);
                throw new ServicioExcepcion(TipoErrorServicio.FallaServicio, mensaje, ex);
            }
            catch (EndpointNotFoundException ex)
            {
                Logger.Error("Endpoint de reportes no encontrado.", ex);
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.avisoTextoComunicacionServidorSesion,
                    ex);
            }
            catch (TimeoutException ex)
            {
                Logger.Error("Timeout al enviar reporte de jugador.", ex);
                throw new ServicioExcepcion(
                    TipoErrorServicio.TiempoAgotado,
                    Lang.avisoTextoServidorTiempoSesion,
                    ex);
            }
            catch (CommunicationException ex)
            {
                Logger.Error("Error de comunicacion al enviar reporte de jugador.", ex);
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.avisoTextoComunicacionServidorSesion,
                    ex);
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error("Operacion invalida al enviar reporte de jugador.", ex);
                throw new ServicioExcepcion(
                    TipoErrorServicio.OperacionInvalida,
                    Lang.errorTextoReportarJugador,
                    ex);
            }
        }
    }
}