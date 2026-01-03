using System;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using log4net;
using PictionaryMusicalServidor.Datos.Constantes;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Datos.Excepciones;
using Datos.Modelo;

namespace PictionaryMusicalServidor.Datos.DAL.Implementaciones
{
    /// <summary>
    /// Repositorio encargado de gestionar la persistencia de reportes de jugadores.
    /// Permite verificar reportes duplicados y crear nuevos registros.
    /// </summary>
    public class ReporteRepositorio : IReporteRepositorio
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReporteRepositorio));
        private readonly BaseDatosPruebaEntities _contexto;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de reportes.
        /// </summary>
        /// <param name="contexto">Contexto de base de datos a utilizar.</param>
        public ReporteRepositorio(BaseDatosPruebaEntities contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        /// <summary>
        /// Verifica si ya existe un reporte del reportante hacia el usuario reportado.
        /// </summary>
        public bool ExisteReporte(int idReportante, int idReportado)
        {
            try
            {
                return _contexto.Reporte.Any(reporteEntidad => 
                    reporteEntidad.idReportante == idReportante
                    && reporteEntidad.idReportado == idReportado);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Reporte.ErrorVerificarExistencia,
                    idReportante,
                    idReportado);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Reporte.ErrorVerificarExistencia,
                    idReportante,
                    idReportado);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Reporte.ErrorVerificarExistencia,
                    idReportante,
                    idReportado);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Reporte.ErrorVerificarExistencia,
                    idReportante,
                    idReportado);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Reporte.ErrorVerificarExistencia,
                    idReportante,
                    idReportado);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Reporte.ErrorVerificarExistencia,
                    idReportante,
                    idReportado);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Reporte.ErrorVerificarExistencia,
                    idReportante,
                    idReportado);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Reporte.ErrorVerificarExistencia,
                    idReportante,
                    idReportado);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
        }

        /// <summary>
        /// Almacena un nuevo reporte.
        /// </summary>
        /// <param name="reporte">Entidad con los datos del reporte a guardar.</param>
        /// <returns>Entidad almacenada con su identificador.</returns>
        public Reporte CrearReporte(Reporte reporte)
        {
            if (reporte == null)
            {
                var excepcion = new ArgumentNullException(nameof(reporte));
                _logger.Error(MensajesErrorDatos.Reporte.IntentarCrearReporteNulo, excepcion);
                throw excepcion;
            }

            try
            {
                var reporteCreado = _contexto.Reporte.Add(reporte);
                _contexto.SaveChanges();
                return reporteCreado;
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Reporte.ErrorGuardarReporte, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Reporte.ErrorGuardarReporte, 
                    excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Reporte.ErrorGuardarReporte, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Reporte.ErrorGuardarReporte, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesErrorDatos.Reporte.ErrorGuardarReporte, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Reporte.ErrorGuardarReporte, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesErrorDatos.Reporte.ErrorGuardarReporte, excepcion);
                throw new BaseDatosExcepcion(
                    MensajesErrorDatos.Reporte.ErrorGuardarReporte, 
                    excepcion);
            }
        }

        /// <summary>
        /// Cuenta la cantidad de reportes que ha recibido un usuario.
        /// </summary>
        /// <param name="idReportado">Identificador del usuario reportado.</param>
        /// <returns>Total de reportes registrados en su contra.</returns>
        public int ContarReportesRecibidos(int idReportado)
        {
            if (idReportado <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(idReportado),
                    MensajesErrorDatos.Reporte.IdReportadoMayorCero);
            }

            try
            {
                return _contexto.Reporte.Count(
                    reporteEntidad => reporteEntidad.idReportado == idReportado);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Reporte.ErrorContarReportes, 
                    idReportado);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Reporte.ErrorContarReportes, 
                    idReportado);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (EntityException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Reporte.ErrorContarReportes, 
                    idReportado);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Reporte.ErrorContarReportes, 
                    idReportado);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Reporte.ErrorContarReportes, 
                    idReportado);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Reporte.ErrorContarReportes, 
                    idReportado);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.ErrorFormat(
                    MensajesErrorDatos.Reporte.ErrorContarReportes, 
                    idReportado);
                string mensajeExcepcion = string.Format(
                    MensajesErrorDatos.Reporte.ErrorContarReportes, 
                    idReportado);
                throw new BaseDatosExcepcion(mensajeExcepcion, excepcion);
            }
        }
    }
}
