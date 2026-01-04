using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL.Implementaciones
{
    [TestClass]
    public class ReporteRepositorioPruebas
    {
        private const int IdReporteUno = 1;
        private const int IdReporteDos = 2;
        private const int IdReporteTres = 3;
        private const int IdUsuarioReportante = 1;
        private const int IdUsuarioReportanteDos = 2;
        private const int IdUsuarioReportanteTres = 3;
        private const int IdUsuarioReportanteCuatro = 4;
        private const int IdUsuarioReportado = 2;
        private const int IdUsuarioReportadoUno = 1;
        private const int IdUsuarioReportadoOtro = 5;
        private const int IdCero = 0;
        private const int IdNegativo = -1;
        private const int CantidadEsperada = 2;
        private const int CantidadCero = 0;

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Reporte>> _reporteDbSetMock;
        private ReporteRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _reporteDbSetMock = new Mock<DbSet<Reporte>>();
        }

        [TestCleanup]
        public void Limpiar()
        {
            _repositorio = null;
            _contextoMock = null;
        }

        #region Constructor

        [TestMethod]
        public void Prueba_ConstructorContextoNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ReporteRepositorio(null);
            });
        }

        #endregion

        #region ExisteReporte

        [TestMethod]
        public void Prueba_ExisteReporteExistente_RetornaTrue()
        {
            var reportes = new List<Reporte>
            {
                new Reporte 
                { 
                    idReporte = IdReporteUno, 
                    idReportante = IdUsuarioReportante, 
                    idReportado = IdUsuarioReportado 
                }
            }.AsQueryable();

            ConfigurarDbSetMock(_reporteDbSetMock, reportes);
            _contextoMock.Setup(c => c.Reporte).Returns(_reporteDbSetMock.Object);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteReporte(IdUsuarioReportante, IdUsuarioReportado);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporteInexistente_RetornaFalse()
        {
            var reportes = new List<Reporte>().AsQueryable();

            ConfigurarDbSetMock(_reporteDbSetMock, reportes);
            _contextoMock.Setup(c => c.Reporte).Returns(_reporteDbSetMock.Object);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteReporte(IdUsuarioReportante, IdUsuarioReportado);

            Assert.IsFalse(resultado);
        }

        #endregion

        #region CrearReporte

        [TestMethod]
        public void Prueba_CrearReporteNulo_LanzaArgumentNullException()
        {
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.CrearReporte(null);
            });
        }

        #endregion

        #region ContarReportesRecibidos

        [TestMethod]
        public void Prueba_ContarReportesRecibidosIdCero_LanzaArgumentOutOfRangeException()
        {
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ContarReportesRecibidos(IdCero);
            });
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidosIdNegativo_LanzaArgumentOutOfRangeException()
        {
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ContarReportesRecibidos(IdNegativo);
            });
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidosSinReportes_RetornaCero()
        {
            var reportes = new List<Reporte>().AsQueryable();

            ConfigurarDbSetMock(_reporteDbSetMock, reportes);
            _contextoMock.Setup(c => c.Reporte).Returns(_reporteDbSetMock.Object);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            int resultado = _repositorio.ContarReportesRecibidos(IdUsuarioReportante);

            Assert.AreEqual(CantidadCero, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidosConReportes_RetornaCantidadCorrecta()
        {
            var reportes = new List<Reporte>
            {
                new Reporte { idReporte = IdReporteUno, idReportante = IdUsuarioReportanteDos, idReportado = IdUsuarioReportadoUno },
                new Reporte { idReporte = IdReporteDos, idReportante = IdUsuarioReportanteTres, idReportado = IdUsuarioReportadoUno },
                new Reporte { idReporte = IdReporteTres, idReportante = IdUsuarioReportanteCuatro, idReportado = IdUsuarioReportadoOtro }
            }.AsQueryable();

            ConfigurarDbSetMock(_reporteDbSetMock, reportes);
            _contextoMock.Setup(c => c.Reporte).Returns(_reporteDbSetMock.Object);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            int resultado = _repositorio.ContarReportesRecibidos(IdUsuarioReportadoUno);

            Assert.AreEqual(CantidadEsperada, resultado);
        }

        #endregion

        #region Metodos Auxiliares

        private static void ConfigurarDbSetMock<T>(Mock<DbSet<T>> dbSetMock, IQueryable<T> datos) 
            where T : class
        {
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(datos.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(datos.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(datos.ElementType);
            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(datos.GetEnumerator());
        }

        #endregion
    }
}
