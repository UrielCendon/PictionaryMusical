using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Usuarios;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Data;
using System.Data.Entity.Core;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Usuarios
{
    [TestClass]
    public class ReportesManejadorPruebas
    {
        private const int IdUsuarioReportante = 1;
        private const int IdUsuarioReportado = 2;
        private const int TotalReportesNormal = 1;
        private const int TotalReportesLimite = 3;
        private const string NombreUsuarioReportante = "UsuarioReportante";
        private const string NombreUsuarioReportado = "UsuarioReportado";
        private const string MotivoValido = "Comportamiento inapropiado";
        private const string CadenaVacia = "";
        private const string CadenaSoloEspacios = "   ";
        private const string MotivoMuyLargo = 
            "Este motivo es demasiado largo y excede el limite de cien caracteres " +
            "permitidos para un reporte de jugador en el sistema";

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IExpulsionPorReportesServicio> _expulsionServicioMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private Mock<IReporteRepositorio> _reporteRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private ReportesManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _expulsionServicioMock = new Mock<IExpulsionPorReportesServicio>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _reporteRepositorioMock = new Mock<IReporteRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock
                .Setup(fabrica => fabrica.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearUsuarioRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _repositorioFactoriaMock
                .Setup(fabrica => fabrica.CrearReporteRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_reporteRepositorioMock.Object);

            _manejador = new ReportesManejador(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object,
                _expulsionServicioMock.Object);
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

        private void ConfigurarUsuariosValidos()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(
                    It.Is<string>(nombre => nombre == NombreUsuarioReportante)))
                .Returns(new Usuario { idUsuario = IdUsuarioReportante });

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(
                    It.Is<string>(nombre => nombre == NombreUsuarioReportado)))
                .Returns(new Usuario { idUsuario = IdUsuarioReportado });
        }

        private void ConfigurarReporteNoDuplicado()
        {
            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ExisteReporte(
                    IdUsuarioReportante, IdUsuarioReportado))
                .Returns(false);
        }

        private void ConfigurarReporteDuplicado()
        {
            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ExisteReporte(
                    IdUsuarioReportante, IdUsuarioReportado))
                .Returns(true);
        }

        private void ConfigurarConteoReportes(int totalReportes)
        {
            _reporteRepositorioMock
                .Setup(repositorio => repositorio.ContarReportesRecibidos(IdUsuarioReportado))
                .Returns(totalReportes);
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ReportesManejador(
                    null, 
                    _repositorioFactoriaMock.Object, 
                    _expulsionServicioMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ReportesManejador(
                    _contextoFactoriaMock.Object, 
                    null, 
                    _expulsionServicioMock.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_ExpulsionServicioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ReportesManejador(
                    _contextoFactoriaMock.Object, 
                    _repositorioFactoriaMock.Object, 
                    null));
        }

        [TestMethod]
        public void Prueba_ReportarJugador_ReporteNulo_RetornaOperacionFallida()
        {
            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_NombreReportanteInvalido_RetornaOperacionFallida()
        {
            var reporteVacio = CrearReporteValido();
            reporteVacio.NombreUsuarioReportante = CadenaVacia;
            var reporteEspacios = CrearReporteValido();
            reporteEspacios.NombreUsuarioReportante = CadenaSoloEspacios;
            var reporteNulo = CrearReporteValido();
            reporteNulo.NombreUsuarioReportante = null;

            Assert.IsFalse(_manejador.ReportarJugador(reporteVacio).OperacionExitosa);
            Assert.IsFalse(_manejador.ReportarJugador(reporteEspacios).OperacionExitosa);
            Assert.IsFalse(_manejador.ReportarJugador(reporteNulo).OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_NombreReportadoInvalido_RetornaOperacionFallida()
        {
            var reporteVacio = CrearReporteValido();
            reporteVacio.NombreUsuarioReportado = CadenaVacia;
            var reporteNulo = CrearReporteValido();
            reporteNulo.NombreUsuarioReportado = null;

            Assert.IsFalse(_manejador.ReportarJugador(reporteVacio).OperacionExitosa);
            Assert.IsFalse(_manejador.ReportarJugador(reporteNulo).OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_MotivoInvalido_RetornaOperacionFallida()
        {
            var reporteMotivoVacio = CrearReporteValido();
            reporteMotivoVacio.Motivo = CadenaVacia;
            var reporteMotivoNulo = CrearReporteValido();
            reporteMotivoNulo.Motivo = null;
            var reporteMotivoLargo = CrearReporteValido();
            reporteMotivoLargo.Motivo = MotivoMuyLargo;

            Assert.IsFalse(_manejador.ReportarJugador(reporteMotivoVacio).OperacionExitosa);
            Assert.IsFalse(_manejador.ReportarJugador(reporteMotivoNulo).OperacionExitosa);
            Assert.IsFalse(_manejador.ReportarJugador(reporteMotivoLargo).OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_MismoUsuario_RetornaOperacionFallida()
        {
            ConfigurarUsuariosValidos();
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(
                    It.IsAny<string>()))
                .Returns(new Usuario { idUsuario = IdUsuarioReportante });

            var reporte = CrearReporteValido();
            reporte.NombreUsuarioReportado = NombreUsuarioReportante;

            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_ReporteDuplicado_RetornaOperacionFallida()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteDuplicado();

            var reporte = CrearReporteValido();

            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_DatosValidos_RetornaOperacionExitosa()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            ConfigurarConteoReportes(TotalReportesNormal);

            var reporte = CrearReporteValido();

            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(reporte);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_DatosValidos_CreaReporteEnRepositorio()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            ConfigurarConteoReportes(TotalReportesNormal);

            var reporte = CrearReporteValido();

            _manejador.ReportarJugador(reporte);

            _reporteRepositorioMock.Verify(
                repositorio => repositorio.CrearReporte(It.IsAny<Reporte>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_AlcanzaLimiteReportes_LlamaExpulsionServicio()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            ConfigurarConteoReportes(TotalReportesLimite);

            var reporte = CrearReporteValido();

            _manejador.ReportarJugador(reporte);

            _expulsionServicioMock.Verify(
                servicio => servicio.ExpulsarSiAlcanzaLimite(
                    IdUsuarioReportado,
                    NombreUsuarioReportado,
                    TotalReportesLimite),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_NoAlcanzaLimite_LlamaExpulsionServicio()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            ConfigurarConteoReportes(TotalReportesNormal);

            var reporte = CrearReporteValido();

            _manejador.ReportarJugador(reporte);

            _expulsionServicioMock.Verify(
                servicio => servicio.ExpulsarSiAlcanzaLimite(
                    IdUsuarioReportado,
                    NombreUsuarioReportado,
                    TotalReportesNormal),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_UsuarioReportanteNoExiste_RetornaOperacionFallida()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(
                    It.Is<string>(nombre => nombre == NombreUsuarioReportante)))
                .Returns((Usuario)null);
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(
                    It.Is<string>(nombre => nombre == NombreUsuarioReportado)))
                .Returns(new Usuario { idUsuario = IdUsuarioReportado });

            var reporte = CrearReporteValido();

            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_UsuarioReportadoNoExiste_RetornaOperacionFallida()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(
                    It.Is<string>(nombre => nombre == NombreUsuarioReportante)))
                .Returns(new Usuario { idUsuario = IdUsuarioReportante });
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(
                    It.Is<string>(nombre => nombre == NombreUsuarioReportado)))
                .Returns((Usuario)null);

            var reporte = CrearReporteValido();

            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_EntityException_RetornaOperacionFallida()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            _reporteRepositorioMock
                .Setup(repositorio => repositorio.CrearReporte(It.IsAny<Reporte>()))
                .Throws(new EntityException());

            var reporte = CrearReporteValido();

            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_DataException_RetornaOperacionFallida()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            _reporteRepositorioMock
                .Setup(repositorio => repositorio.CrearReporte(It.IsAny<Reporte>()))
                .Throws(new DataException());

            var reporte = CrearReporteValido();

            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_ExcepcionGenerica_RetornaOperacionFallida()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            _reporteRepositorioMock
                .Setup(repositorio => repositorio.CrearReporte(It.IsAny<Reporte>()))
                .Throws(new Exception());

            var reporte = CrearReporteValido();

            ResultadoOperacionDTO resultado = _manejador.ReportarJugador(reporte);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_ReporteValido_VerificaExistenciaReporte()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            ConfigurarConteoReportes(TotalReportesNormal);

            var reporte = CrearReporteValido();

            _manejador.ReportarJugador(reporte);

            _reporteRepositorioMock.Verify(
                repositorio => repositorio.ExisteReporte(
                    IdUsuarioReportante, IdUsuarioReportado),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ReportarJugador_ReporteValido_ContabilizaReportes()
        {
            ConfigurarUsuariosValidos();
            ConfigurarReporteNoDuplicado();
            ConfigurarConteoReportes(TotalReportesNormal);

            var reporte = CrearReporteValido();

            _manejador.ReportarJugador(reporte);

            _reporteRepositorioMock.Verify(
                repositorio => repositorio.ContarReportesRecibidos(IdUsuarioReportado),
                Times.Once);
        }
    }
}
