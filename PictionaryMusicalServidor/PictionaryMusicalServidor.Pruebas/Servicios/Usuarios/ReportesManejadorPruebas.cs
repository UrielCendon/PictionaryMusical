using System;
using System.ServiceModel;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class ReportesManejadorPruebas
    {
        private const int IdUsuarioReportante = 1;
        private const int IdUsuarioReportado = 2;
        private const int TotalReportesCero = 0;
        private const int TotalReportesUno = 1;
        private const string NombreUsuarioReportante = "Reportante";
        private const string NombreUsuarioReportado = "Reportado";
        private const string MotivoValido = "Motivo de prueba para el reporte";

        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<IExpulsionPorReportesServicio> _mockExpulsionServicio;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
        private Mock<IReporteRepositorio> _mockReporteRepositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockExpulsionServicio = new Mock<IExpulsionPorReportesServicio>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();
            _mockReporteRepositorio = new Mock<IReporteRepositorio>();

            _mockContextoFactoria.Setup(factoria => factoria.CrearContexto())
                .Returns(_mockContexto.Object);
            _mockRepositorioFactoria.Setup(factoria => 
                factoria.CrearUsuarioRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_mockUsuarioRepositorio.Object);
            _mockRepositorioFactoria.Setup(factoria => 
                factoria.CrearReporteRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_mockReporteRepositorio.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ReportesManejador(
                    null, 
                    _mockRepositorioFactoria.Object, 
                    _mockExpulsionServicio.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ReportesManejador(
                    _mockContextoFactoria.Object, 
                    null, 
                    _mockExpulsionServicio.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionExpulsionServicioNula()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ReportesManejador(
                    _mockContextoFactoria.Object, 
                    _mockRepositorioFactoria.Object, 
                    null));
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaReporteNulo()
        {
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaReportanteVacio()
        {
            var reporte = CrearReporteValido();
            reporte.NombreUsuarioReportante = string.Empty;
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaReportadoVacio()
        {
            var reporte = CrearReporteValido();
            reporte.NombreUsuarioReportado = string.Empty;
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaMotivoVacio()
        {
            var reporte = CrearReporteValido();
            reporte.Motivo = string.Empty;
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaUsuariosNoEncontrados()
        {
            ConfigurarMocksUsuariosNoEncontrados();
            var reporte = CrearReporteValido();
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaMismoUsuario()
        {
            ConfigurarMocksMismoUsuario();
            var reporte = CrearReporteValido();
            reporte.NombreUsuarioReportante = NombreUsuarioReportante;
            reporte.NombreUsuarioReportado = NombreUsuarioReportante;
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaReporteDuplicado()
        {
            ConfigurarMocksUsuariosExistentes();
            _mockReporteRepositorio.Setup(repositorio => 
                repositorio.ExisteReporte(IdUsuarioReportante, IdUsuarioReportado))
                .Returns(true);
            var reporte = CrearReporteValido();
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaVerdaderoParaReporteValido()
        {
            ConfigurarMocksReporteExitoso();
            var reporte = CrearReporteValido();
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_CreaReporteEnRepositorio()
        {
            ConfigurarMocksReporteExitoso();
            var reporte = CrearReporteValido();
            var manejador = CrearManejador();

            manejador.ReportarJugador(reporte);

            _mockReporteRepositorio.Verify(repositorio => 
                repositorio.CrearReporte(It.IsAny<Reporte>()), Times.Once);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_LlamaExpulsionServicio()
        {
            ConfigurarMocksReporteExitoso();
            var reporte = CrearReporteValido();
            var manejador = CrearManejador();

            manejador.ReportarJugador(reporte);

            _mockExpulsionServicio.Verify(servicio => 
                servicio.ExpulsarSiAlcanzaLimite(
                    IdUsuarioReportado, 
                    NombreUsuarioReportado, 
                    TotalReportesUno), Times.Once);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaErrorBaseDatos()
        {
            ConfigurarMocksUsuariosExistentes();
            _mockReporteRepositorio.Setup(repositorio => 
                repositorio.ExisteReporte(IdUsuarioReportante, IdUsuarioReportado))
                .Throws(new System.Data.Entity.Core.EntityException());
            var reporte = CrearReporteValido();
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_RetornaFalsoParaExcepcionGeneral()
        {
            ConfigurarMocksUsuariosExistentes();
            _mockReporteRepositorio.Setup(repositorio => 
                repositorio.ExisteReporte(IdUsuarioReportante, IdUsuarioReportado))
                .Throws(new Exception());
            var reporte = CrearReporteValido();
            var manejador = CrearManejador();

            var resultado = manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        private ReportesManejador CrearManejador()
        {
            return new ReportesManejador(
                _mockContextoFactoria.Object, 
                _mockRepositorioFactoria.Object,
                _mockExpulsionServicio.Object);
        }

        private ReporteJugadorDTO CrearReporteValido()
        {
            return new ReporteJugadorDTO
            {
                NombreUsuarioReportante = NombreUsuarioReportante,
                NombreUsuarioReportado = NombreUsuarioReportado,
                Motivo = MotivoValido
            };
        }

        private void ConfigurarMocksUsuariosNoEncontrados()
        {
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorNombreUsuario(It.IsAny<string>()))
                .Returns((Usuario)null);
        }

        private void ConfigurarMocksMismoUsuario()
        {
            var usuario = new Usuario 
            { 
                idUsuario = IdUsuarioReportante, 
                Nombre_Usuario = NombreUsuarioReportante 
            };
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorNombreUsuario(It.IsAny<string>()))
                .Returns(usuario);
        }

        private void ConfigurarMocksUsuariosExistentes()
        {
            var reportante = new Usuario 
            { 
                idUsuario = IdUsuarioReportante, 
                Nombre_Usuario = NombreUsuarioReportante 
            };
            var reportado = new Usuario 
            { 
                idUsuario = IdUsuarioReportado, 
                Nombre_Usuario = NombreUsuarioReportado 
            };

            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorNombreUsuario(NombreUsuarioReportante))
                .Returns(reportante);
            _mockUsuarioRepositorio.Setup(repositorio => 
                repositorio.ObtenerPorNombreUsuario(NombreUsuarioReportado))
                .Returns(reportado);
        }

        private void ConfigurarMocksReporteExitoso()
        {
            ConfigurarMocksUsuariosExistentes();
            _mockReporteRepositorio.Setup(repositorio => 
                repositorio.ExisteReporte(IdUsuarioReportante, IdUsuarioReportado))
                .Returns(false);
            _mockReporteRepositorio.Setup(repositorio => 
                repositorio.ContarReportesRecibidos(IdUsuarioReportado))
                .Returns(TotalReportesUno);
        }
    }
}
