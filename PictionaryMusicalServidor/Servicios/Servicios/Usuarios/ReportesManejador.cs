using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Linq;
using System.ServiceModel;
using Datos.Modelo;
using log4net;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Servicio encargado de registrar reportes de jugadores.
    /// Valida la informacion recibida y evita duplicados por jugador reportante y reportado.
    /// </summary>
    public class ReportesManejador : IReportesManejador
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReportesManejador));

        private const int LimiteReportesParaExpulsion = 3;

        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;

        /// <summary>
        /// Constructor por defecto para uso en WCF.
        /// </summary>
        public ReportesManejador() : this(
            new ContextoFactoria(), 
            new RepositorioFactoria())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        public ReportesManejador(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria)
        {
            _contextoFactoria = contextoFactoria ??
                throw new ArgumentNullException(nameof(contextoFactoria));
            _repositorioFactoria = repositorioFactoria ??
                throw new ArgumentNullException(nameof(repositorioFactoria));
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
                        return CrearResultadoFallo(
                            MensajesError.Cliente.ReporteMismoUsuario);
                    }

                    if (reporteRepositorio.ExisteReporte(
                        idsUsuarios.IdReportante,
                        idsUsuarios.IdReportado))
                    {
                        return CrearResultadoFallo(
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
                    ExpulsarJugadorPorReportesSiAlcanzaLimite(
                        reporteRepositorio,
                        idsUsuarios.IdReportado,
                        reporte.NombreUsuarioReportado);

                    return new ResultadoOperacionDTO
                    {
                        OperacionExitosa = true,
                        Mensaje = MensajesError.Cliente.ReporteRegistrado
                    };
                }
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Log.ValidacionFallidaReporte, excepcion);
                return CrearResultadoFallo(excepcion.Message);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Log.DatosInvalidosReporte, excepcion);
                return CrearResultadoFallo(excepcion.Message);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Log.OperacionInvalidaReporte, excepcion);
                return CrearResultadoFallo(excepcion.Message);
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Warn(MensajesError.Log.UsuariosNoEncontradosReporte, excepcion);
                return CrearResultadoFallo(MensajesError.Cliente.UsuariosNoEncontrados);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorBaseDatosReporte, excepcion);
                return CrearResultadoFallo(MensajesError.Cliente.ErrorCrearReporte);
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorDatosReporte, excepcion);
                return CrearResultadoFallo(MensajesError.Cliente.ErrorCrearReporte);
            }
            catch (Exception excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorInesperadoReporte, excepcion);
                return CrearResultadoFallo(MensajesError.Cliente.ErrorCrearReporte);
            }
        }

        private (int IdReportante, int IdReportado) ObtenerIdentificadoresUsuarios(
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

                return (reportante.idUsuario, reportado.idUsuario);
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Warn(MensajesError.Log.ReporteUsuariosNoRegistrados, excepcion);
                throw new FaultException(MensajesError.Cliente.UsuariosEspecificadosNoExisten);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorBaseDatosReporte, excepcion);
                throw;
            }
            catch (DataException excepcion)
            {
                _logger.Error(MensajesError.Log.ErrorDatosReporte, excepcion);
                throw;
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Log.ErrorObtenerIdUsuariosReporte, excepcion);
                throw;
            }
        }

        private void ExpulsarJugadorPorReportesSiAlcanzaLimite(
            IReporteRepositorio reporteRepositorio,
            int idReportado,
            string nombreUsuarioReportado)
        {
            if (reporteRepositorio == null || idReportado <= 0)
            {
                return;
            }

            int totalReportes = reporteRepositorio.ContarReportesRecibidos(idReportado);
            if (totalReportes < LimiteReportesParaExpulsion)
            {
                return;
            }

            string nombreNormalizado = EntradaComunValidador.NormalizarTexto(
                nombreUsuarioReportado);
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                return;
            }

            _logger.WarnFormat(
                "Usuario {0} alcanzo el limite de reportes ({1}). Iniciando expulsion de salas activas.",
                nombreNormalizado,
                LimiteReportesParaExpulsion);

            try
            {
                ExpulsarDeSalasActivas(nombreNormalizado);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    "Error al intentar expulsar jugador con limite de reportes.",
                    excepcion);
            }
        }

        private void ExpulsarDeSalasActivas(string nombreUsuario)
        {
            var salas = SalasManejador.ObtenerListaSalas();
            if (salas == null || salas.Count == 0)
            {
                return;
            }

            foreach (var sala in salas)
            {
                bool jugadorEnSala = sala?.Jugadores?.Any(jugador =>
                {
                    string jugadorNormalizado = EntradaComunValidador.NormalizarTexto(jugador);
                    return string.Equals(
                        jugadorNormalizado,
                        EntradaComunValidador.NormalizarTexto(nombreUsuario),
                        StringComparison.OrdinalIgnoreCase);
                }) == true;

                if (!jugadorEnSala)
                {
                    continue;
                }

                try
                {
                    var salasManejador = new SalasManejador();

                    string creadorNormalizado = EntradaComunValidador.NormalizarTexto(sala?.Creador);
                    string usuarioNormalizado = EntradaComunValidador.NormalizarTexto(nombreUsuario);

                    bool esCreador = !string.IsNullOrWhiteSpace(creadorNormalizado) &&
                        string.Equals(
                            creadorNormalizado,
                            usuarioNormalizado,
                            StringComparison.OrdinalIgnoreCase);

                    if (esCreador)
                    {
                        salasManejador.AbandonarSala(sala.Codigo, nombreUsuario);
                    }
                    else
                    {
                        salasManejador.ExpulsarJugador(
                            sala.Codigo,
                            sala.Creador,
                            nombreUsuario);
                    }
                }
                catch (FaultException excepcion)
                {
                    _logger.Warn(string.Format(
                        "No se pudo expulsar a {0} de la sala {1}.",
                        nombreUsuario,
                        sala?.Codigo), excepcion);
                }
                catch (Exception excepcion)
                {
                    _logger.Warn(string.Format(
                        "Error inesperado al expulsar a {0} de la sala {1}.",
                        nombreUsuario,
                        sala?.Codigo), excepcion);
                }
            }
        }

        private ResultadoOperacionDTO CrearResultadoFallo(string mensaje)
        {
            return new ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = mensaje
            };
        }
    }
}
