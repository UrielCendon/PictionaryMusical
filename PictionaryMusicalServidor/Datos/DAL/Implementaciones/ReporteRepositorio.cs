using System;
using System.Linq;
using log4net;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
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
                return _contexto.Reporte.Any(r => r.idReportante == idReportante
                    && r.idReportado == idReportado);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(
                    "Error al verificar existencia del reporte entre {0} y {1}.",
                    idReportante,
                    idReportado,
                    ex);
                throw;
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
                var ex = new ArgumentNullException(nameof(reporte));
                _logger.Error("Se intento crear un reporte nulo.", ex);
                throw ex;
            }

            try
            {
                var entidad = _contexto.Reporte.Add(reporte);
                _contexto.SaveChanges();
                return entidad;
            }
            catch (Exception ex)
            {
                _logger.Error("Error al guardar el reporte en la base de datos.", ex);
                throw;
            }
        }

        /// <summary>
        /// Cuenta el numero de reportes recibidos por un usuario.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario reportado.</param>
        /// <returns>Numero de reportes recibidos por el usuario.</returns>
        public int ContarReportesRecibidos(int idUsuario)
        {
            try
            {
                return _contexto.Reporte.Count(r => r.idReportado == idUsuario);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(
                    "Error al contar reportes recibidos del usuario {0}.",
                    idUsuario,
                    ex);
                throw;
            }
        }
    }
}