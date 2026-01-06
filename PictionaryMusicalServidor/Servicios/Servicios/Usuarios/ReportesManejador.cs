using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.ServiceModel;
using Datos.Modelo;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Servicio encargado de registrar reportes de jugadores.
    /// Valida la informacion recibida y evita duplicados por jugador reportante y reportado.
    /// </summary>
    public class ReportesManejador : IReportesManejador
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReportesManejador));

        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;
        private readonly IExpulsionPorReportesServicio _expulsionServicio;

        /// <summary>
        /// Constructor por defecto para uso en WCF.
        /// </summary>
        public ReportesManejador() : this(
            new ContextoFactoria(), 
            new RepositorioFactoria(),
            new ExpulsionPorReportesServicio())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        /// <param name="expulsionServicio">Servicio de expulsion por reportes.</param>
        public ReportesManejador(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria,
            IExpulsionPorReportesServicio expulsionServicio)
        {
            _contextoFactoria = contextoFactoria ??
                throw new ArgumentNullException(nameof(contextoFactoria));
            _repositorioFactoria = repositorioFactoria ??
                throw new ArgumentNullException(nameof(repositorioFactoria));
            _expulsionServicio = expulsionServicio ??
                throw new ArgumentNullException(nameof(expulsionServicio));
        }

        /// <summary>
        /// Registra un reporte de un jugador hacia otro.
        /// Valida la informacion, verifica duplicados y almacena el reporte.
        /// </summary>
        /// <param name="reporte">Datos del reporte enviado por el cliente.</param>
        /// <returns>Resultado de la operacion.</returns>
        public ResultadoOperacionDTO ReportarJugador(ReporteJugadorDTO reporte)
        {
            try
            {
                EntradaComunValidador.ValidarReporteJugador(reporte);
                string motivoNormalizado = EntradaComunValidador.NormalizarTexto(reporte.Motivo);

                using (var contexto = _contextoFactoria.CrearContexto())
                {
                    var idsUsuarios = ObtenerIdentificadoresUsuarios(contexto, reporte);
                    var reporteRepositorio = 
                        _repositorioFactoria.CrearReporteRepositorio(contexto);

                    if (idsUsuarios.IdReportante == idsUsuarios.IdReportado)
                    {
                        return CreadorResultado.CrearResultadoFallo(
                            MensajesError.Cliente.ReporteMismoUsuario);
                    }

                    if (reporteRepositorio.ExisteReporte(
                        idsUsuarios.IdReportante,
                        idsUsuarios.IdReportado))
                    {
                        return CreadorResultado.CrearResultadoFallo(
                            MensajesError.Cliente.ReporteDuplicado);
                    }

                    var nuevoReporte = new Reporte
                    {
                        idReportante = idsUsuarios.IdReportante,
                        idReportado = idsUsuarios.IdReportado,
                        Motivo = motivoNormalizado,
                        Fecha_Reporte = DateTime.UtcNow
                    };

                    reporteRepositorio.CrearReporte(nuevoReporte);
                    
                    int totalReportes = reporteRepositorio.ContarReportesRecibidos(
                        idsUsuarios.IdReportado);
                    _expulsionServicio.ExpulsarSiAlcanzaLimite(
                        idsUsuarios.IdReportado,
                        reporte.NombreUsuarioReportado,
                        totalReportes);

                    return new ResultadoOperacionDTO
                    {
                        OperacionExitosa = true,
                        Mensaje = MensajesError.Cliente.ReporteRegistrado
                    };
                }
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ValidacionFallidaReporte, excepcion);
                return CreadorResultado.CrearResultadoFallo(excepcion.Message);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.DatosInvalidosReporte, excepcion);
                return CreadorResultado.CrearResultadoFallo(excepcion.Message);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.OperacionInvalidaReporte, excepcion);
                return CreadorResultado.CrearResultadoFallo(excepcion.Message);
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.UsuariosNoEncontradosReporte, excepcion);
                return CreadorResultado.CrearResultadoFallo(MensajesError.Cliente.UsuariosNoEncontrados);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorBaseDatosReporte, excepcion);
                return CreadorResultado.CrearResultadoFallo(MensajesError.Cliente.ErrorCrearReporte);
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorDatosReporte, excepcion);
                return CreadorResultado.CrearResultadoFallo(MensajesError.Cliente.ErrorCrearReporte);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorInesperadoReporte, excepcion);
                return CreadorResultado.CrearResultadoFallo(MensajesError.Cliente.ErrorCrearReporte);
            }
        }

        private IdentificadoresUsuarios ObtenerIdentificadoresUsuarios(
           BaseDatosPruebaEntities contexto,
           ReporteJugadorDTO reporte)
        {
            try
            {
                var usuarioRepositorio = 
                    _repositorioFactoria.CrearUsuarioRepositorio(contexto);
                var reportante = usuarioRepositorio.ObtenerPorNombreUsuario(
                    EntradaComunValidador.NormalizarTexto(reporte.NombreUsuarioReportante));
                var reportado = usuarioRepositorio.ObtenerPorNombreUsuario(
                    EntradaComunValidador.NormalizarTexto(reporte.NombreUsuarioReportado));

                if (reportante == null || reportado == null)
                {
                    throw new FaultException(
                        MensajesError.Cliente.UsuariosEspecificadosNoExisten);
                }

                return new IdentificadoresUsuarios
                {
                    IdReportante = reportante.idUsuario,
                    IdReportado = reportado.idUsuario
                };
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ReporteUsuariosNoRegistrados, excepcion);
                throw new FaultException(MensajesError.Cliente.UsuariosEspecificadosNoExisten);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorBaseDatosReporte, excepcion);
                throw;
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesError.Bitacora.ErrorDatosReporte, excepcion);
                throw;
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorObtenerIdUsuariosReporte, excepcion);
                throw;
            }
        }
    }
}
