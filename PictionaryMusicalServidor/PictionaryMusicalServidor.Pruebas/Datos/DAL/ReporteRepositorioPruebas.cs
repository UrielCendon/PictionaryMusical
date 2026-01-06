using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL
{
    [TestClass]
    public class ReporteRepositorioPruebas
    {
        private const int IdReportantePrueba = 1;
        private const int IdReportadoPrueba = 2;
        private const int IdReportadoInvalido = 0;
        private const int IdReportadoNegativo = -1;
        private const int CantidadReportesPrueba = 3;
        private const string MotivoPrueba = "Comportamiento inapropiado";

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ReporteRepositorio(null));
        }

        [TestMethod]
        public void Prueba_ExisteReporte_RetornaVerdaderoReporteExistente()
        {
            var datosReportes = new List<Reporte>
            {
                new Reporte
                {
                    idReporte = 1,
                    idReportante = IdReportantePrueba,
                    idReportado = IdReportadoPrueba,
                    Motivo = MotivoPrueba,
                    Fecha_Reporte = DateTime.Now
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosReportes);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Reporte).Returns(mockDbSet.Object);

            var repositorio = new ReporteRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteReporte(IdReportantePrueba, IdReportadoPrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_RetornaFalsoReporteInexistente()
        {
            var datosReportes = new List<Reporte>().AsQueryable();

            var mockDbSet = CrearMockDbSet(datosReportes);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Reporte).Returns(mockDbSet.Object);

            var repositorio = new ReporteRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteReporte(IdReportantePrueba, IdReportadoPrueba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_RetornaFalsoReportanteDiferente()
        {
            int otroReportante = 99;
            var datosReportes = new List<Reporte>
            {
                new Reporte
                {
                    idReporte = 1,
                    idReportante = otroReportante,
                    idReportado = IdReportadoPrueba,
                    Motivo = MotivoPrueba,
                    Fecha_Reporte = DateTime.Now
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosReportes);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Reporte).Returns(mockDbSet.Object);

            var repositorio = new ReporteRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteReporte(IdReportantePrueba, IdReportadoPrueba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_CrearReporte_LanzaExcepcionReporteNulo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new ReporteRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => repositorio.CrearReporte(null));
        }

        [TestMethod]
        public void Prueba_CrearReporte_AgregaReporteAlContexto()
        {
            var reporteNuevo = new Reporte
            {
                idReportante = IdReportantePrueba,
                idReportado = IdReportadoPrueba,
                Motivo = MotivoPrueba,
                Fecha_Reporte = DateTime.Now
            };

            var listaReportes = new List<Reporte>();
            var mockDbSet = CrearMockDbSetConAdd(listaReportes);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Reporte).Returns(mockDbSet.Object);

            var repositorio = new ReporteRepositorio(mockContexto.Object);

            repositorio.CrearReporte(reporteNuevo);

            mockDbSet.Verify(
                dbSet => dbSet.Add(It.IsAny<Reporte>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearReporte_GuardaCambiosEnContexto()
        {
            var reporteNuevo = new Reporte
            {
                idReportante = IdReportantePrueba,
                idReportado = IdReportadoPrueba,
                Motivo = MotivoPrueba,
                Fecha_Reporte = DateTime.Now
            };

            var listaReportes = new List<Reporte>();
            var mockDbSet = CrearMockDbSetConAdd(listaReportes);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Reporte).Returns(mockDbSet.Object);

            var repositorio = new ReporteRepositorio(mockContexto.Object);

            repositorio.CrearReporte(reporteNuevo);

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearReporte_RetornaReporteCreado()
        {
            var reporteNuevo = new Reporte
            {
                idReportante = IdReportantePrueba,
                idReportado = IdReportadoPrueba,
                Motivo = MotivoPrueba,
                Fecha_Reporte = DateTime.Now
            };

            var mockDbSet = new Mock<DbSet<Reporte>>();
            mockDbSet.Setup(conjunto => conjunto.Add(It.IsAny<Reporte>()))
                .Returns<Reporte>(reporte => reporte);

            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Reporte).Returns(mockDbSet.Object);

            var repositorio = new ReporteRepositorio(mockContexto.Object);

            var resultado = repositorio.CrearReporte(reporteNuevo);

            Assert.AreEqual(IdReportantePrueba, resultado.idReportante);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_LanzaExcepcionIdInvalido()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new ReporteRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => repositorio.ContarReportesRecibidos(IdReportadoInvalido));
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_LanzaExcepcionIdNegativo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new ReporteRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => repositorio.ContarReportesRecibidos(IdReportadoNegativo));
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_RetornaCantidadCorrecta()
        {
            var datosReportes = new List<Reporte>
            {
                new Reporte
                {
                    idReporte = 1,
                    idReportante = 1,
                    idReportado = IdReportadoPrueba,
                    Motivo = MotivoPrueba,
                    Fecha_Reporte = DateTime.Now
                },
                new Reporte
                {
                    idReporte = 2,
                    idReportante = 2,
                    idReportado = IdReportadoPrueba,
                    Motivo = MotivoPrueba,
                    Fecha_Reporte = DateTime.Now
                },
                new Reporte
                {
                    idReporte = 3,
                    idReportante = 3,
                    idReportado = IdReportadoPrueba,
                    Motivo = MotivoPrueba,
                    Fecha_Reporte = DateTime.Now
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosReportes);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Reporte).Returns(mockDbSet.Object);

            var repositorio = new ReporteRepositorio(mockContexto.Object);

            int resultado = repositorio.ContarReportesRecibidos(IdReportadoPrueba);

            Assert.AreEqual(CantidadReportesPrueba, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_RetornaCeroSinReportes()
        {
            var datosReportes = new List<Reporte>().AsQueryable();

            var mockDbSet = CrearMockDbSet(datosReportes);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Reporte).Returns(mockDbSet.Object);

            var repositorio = new ReporteRepositorio(mockContexto.Object);

            int resultado = repositorio.ContarReportesRecibidos(IdReportadoPrueba);

            int valorEsperadoCero = 0;
            Assert.AreEqual(valorEsperadoCero, resultado);
        }

        private static Mock<DbSet<Reporte>> CrearMockDbSet(IQueryable<Reporte> datos)
        {
            var mockDbSet = new Mock<DbSet<Reporte>>();
            mockDbSet.As<IQueryable<Reporte>>()
                .Setup(conjunto => conjunto.Provider)
                .Returns(datos.Provider);
            mockDbSet.As<IQueryable<Reporte>>()
                .Setup(conjunto => conjunto.Expression)
                .Returns(datos.Expression);
            mockDbSet.As<IQueryable<Reporte>>()
                .Setup(conjunto => conjunto.ElementType)
                .Returns(datos.ElementType);
            mockDbSet.As<IQueryable<Reporte>>()
                .Setup(conjunto => conjunto.GetEnumerator())
                .Returns(datos.GetEnumerator());
            return mockDbSet;
        }

        private static Mock<DbSet<Reporte>> CrearMockDbSetConAdd(List<Reporte> listaReportes)
        {
            var datos = listaReportes.AsQueryable();
            var mockDbSet = CrearMockDbSet(datos);
            mockDbSet.Setup(conjunto => conjunto.Add(It.IsAny<Reporte>()))
                .Callback<Reporte>(reporte => listaReportes.Add(reporte))
                .Returns<Reporte>(reporte => reporte);
            return mockDbSet;
        }
    }
}
