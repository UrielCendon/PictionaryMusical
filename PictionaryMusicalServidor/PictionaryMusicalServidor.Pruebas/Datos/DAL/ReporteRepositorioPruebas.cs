using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;
using PictionaryMusicalServidor.Datos.Excepciones;
using Datos.Modelo;

namespace PictionaryMusicalServidor.Pruebas.Datos
{
    [TestClass]
    public class ReporteRepositorioPruebas
    {
        private const int IdReportanteValido = 10;
        private const int IdReportadoValido = 20;
        private const int IdReportadoOtro = 30;
        private const int IdInvalido = 0;
        private const int CantidadEsperadaCero = 0;
        private const int CantidadEsperadaUno = 1;
        private const string MotivoReporte = "Comportamiento inadecuado";

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Reporte>> _reporteDbSetMock;
        private ReporteRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _reporteDbSetMock = CrearDbSetMock(new List<Reporte>());

            _contextoMock
                .Setup(contexto => contexto.Reporte)
                .Returns(_reporteDbSetMock.Object);

            _repositorio = new ReporteRepositorio(_contextoMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ReporteRepositorio(null));
        }

        [TestMethod]
        public void Prueba_ExisteReporte_ReporteExisteRetornaTrue()
        {
            var datos = new List<Reporte>
            {
                new Reporte
                {
                    idReportante = IdReportanteValido,
                    idReportado = IdReportadoValido
                }
            }.AsQueryable();

            ConfigurarDbSet(_reporteDbSetMock, datos);

            bool resultado = _repositorio.ExisteReporte(IdReportanteValido, IdReportadoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_ReporteNoExisteRetornaFalse()
        {
            var datos = new List<Reporte>
            {
                new Reporte
                {
                    idReportante = IdReportanteValido,
                    idReportado = IdReportadoOtro
                }
            }.AsQueryable();

            ConfigurarDbSet(_reporteDbSetMock, datos);

            bool resultado = _repositorio.ExisteReporte(IdReportanteValido, IdReportadoValido);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_ErrorBaseDatosLanzaExcepcionPersonalizada()
        {
            _reporteDbSetMock.As<IQueryable<Reporte>>()
                .Setup(consulta => consulta.Provider)
                .Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ExisteReporte(IdReportanteValido, IdReportadoValido));
        }

        [TestMethod]
        public void Prueba_CrearReporte_ReporteNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _repositorio.CrearReporte(null));
        }

        [TestMethod]
        public void Prueba_CrearReporte_DatosValidosGuardaCambios()
        {
            var nuevoReporte = new Reporte
            {
                idReportante = IdReportanteValido,
                idReportado = IdReportadoValido,
                Motivo = MotivoReporte
            };

            _reporteDbSetMock
                .Setup(conjunto => conjunto.Add(It.IsAny<Reporte>()))
                .Returns<Reporte>(entidad => entidad);

            var resultado = _repositorio.CrearReporte(nuevoReporte);

            _reporteDbSetMock.Verify(
                conjuntoReportes => conjuntoReportes.Add(It.IsAny<Reporte>()),
                Times.Once);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
            Assert.AreEqual(IdReportanteValido, resultado.idReportante);
            Assert.AreEqual(IdReportadoValido, resultado.idReportado);
        }

        [TestMethod]
        public void Prueba_CrearReporte_ErrorGuardarLanzaExcepcionPersonalizada()
        {
            var nuevoReporte = new Reporte
            {
                idReportante = IdReportanteValido,
                idReportado = IdReportadoValido
            };

            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new DbUpdateException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.CrearReporte(nuevoReporte));
        }

        [TestMethod]
        public void Prueba_CrearReporte_ErrorEntityLanzaExcepcionPersonalizada()
        {
            var nuevoReporte = new Reporte
            {
                idReportante = IdReportanteValido,
                idReportado = IdReportadoValido
            };

            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.CrearReporte(nuevoReporte));
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_IdInvalidoLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _repositorio.ContarReportesRecibidos(IdInvalido));
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_UsuarioSinReportesRetornaCero()
        {
            var datos = new List<Reporte>().AsQueryable();
            ConfigurarDbSet(_reporteDbSetMock, datos);

            int resultado = _repositorio.ContarReportesRecibidos(IdReportadoValido);

            Assert.AreEqual(CantidadEsperadaCero, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_UsuarioConReportesRetornaCantidadCorrecta()
        {
            var datos = new List<Reporte>
            {
                new Reporte { idReportado = IdReportadoValido },
                new Reporte { idReportado = IdReportadoOtro }
            }.AsQueryable();

            ConfigurarDbSet(_reporteDbSetMock, datos);

            int resultado = _repositorio.ContarReportesRecibidos(IdReportadoValido);

            Assert.AreEqual(CantidadEsperadaUno, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_ErrorBaseDatosLanzaExcepcionPersonalizada()
        {
            _reporteDbSetMock.As<IQueryable<Reporte>>()
                .Setup(consulta => consulta.Provider)
                .Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ContarReportesRecibidos(IdReportadoValido));
        }

        private static Mock<DbSet<T>> CrearDbSetMock<T>(List<T> datos) where T : class
        {
            var queryable = datos.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>()
                .Setup(consulta => consulta.Provider)
                .Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>()
                .Setup(consulta => consulta.Expression)
                .Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>()
                .Setup(consulta => consulta.ElementType)
                .Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>()
                .Setup(consulta => consulta.GetEnumerator())
                .Returns(() => queryable.GetEnumerator());

            mockSet.Setup(dbSet => dbSet.Include(It.IsAny<string>())).Returns(mockSet.Object);
            mockSet.Setup(dbSet => dbSet.Add(It.IsAny<T>())).Returns<T>(entidad => entidad);

            return mockSet;
        }

        private static void ConfigurarDbSet<T>(
            Mock<DbSet<T>> mockSet,
            IQueryable<T> datos) where T : class
        {
            mockSet.As<IQueryable<T>>().Setup(consulta => consulta.Provider)
                .Returns(datos.Provider);
            mockSet.As<IQueryable<T>>().Setup(consulta => consulta.Expression)
                .Returns(datos.Expression);
            mockSet.As<IQueryable<T>>().Setup(consulta => consulta.ElementType)
                .Returns(datos.ElementType);
            mockSet.As<IQueryable<T>>().Setup(consulta => consulta.GetEnumerator())
                .Returns(() => datos.GetEnumerator());
        }
    }
}