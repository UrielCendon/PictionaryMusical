using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Datos.Modelo;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;
using PictionaryMusicalServidor.Datos.Excepciones;
using PictionaryMusicalServidor.Pruebas.DAL.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.DAL.Implementaciones
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase ReporteRepositorio.
    /// Verifica flujos normales, alternos y de excepcion para la gestion de reportes.
    /// </summary>
    [TestClass]
    public class ReporteRepositorioPruebas
    {
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private ReporteRepositorio _repositorio;

        /// <summary>
        /// Inicializa los mocks y el repositorio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = ContextoMockFabrica.CrearContextoMock();
            _repositorio = new ReporteRepositorio(_contextoMock.Object);
        }

        /// <summary>
        /// Limpia los recursos despues de cada prueba.
        /// </summary>
        [TestCleanup]
        public void Limpiar()
        {
            _repositorio = null;
            _contextoMock = null;
        }

        #region Constructor

        [TestMethod]
        public void Prueba_Constructor_ContextoNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var repositorio = new ReporteRepositorio(null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoValido_CreaInstancia()
        {
            var contexto = ContextoMockFabrica.CrearContextoMock();

            var repositorio = new ReporteRepositorio(contexto.Object);

            Assert.IsNotNull(repositorio);
        }

        #endregion

        #region ExisteReporte - Flujos Normales

        [TestMethod]
        public void Prueba_ExisteReporte_ReporteExiste_RetornaTrue()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 1, 2, "Motivo")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteReporte(1, 2);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_ReporteNoExiste_RetornaFalse()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 3, 4, "Motivo")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteReporte(1, 2);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_ListaVacia_RetornaFalse()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteReporte(1, 2);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_ReporteInverso_RetornaFalse()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 2, 1, "Motivo")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteReporte(1, 2);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_MismosIds_RetornaTrue()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 5, 5, "Autoreporte")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteReporte(5, 5);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteReporte_MultiplesReportes_EncuentraCorrectamente()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 1, 3, "Motivo1"),
                EntidadesPruebaFabrica.CrearReporte(2, 2, 4, "Motivo2"),
                EntidadesPruebaFabrica.CrearReporte(3, 5, 6, "Motivo3")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteReporte(2, 4);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region ExisteReporte - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ExisteReporte_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Reporte)
                .Throws(new DbUpdateException("Error de base de datos"));
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ExisteReporte(1, 2);
            });
        }

        [TestMethod]
        public void Prueba_ExisteReporte_ExcepcionGeneral_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Reporte)
                .Throws(new Exception("Error inesperado"));
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ExisteReporte(1, 2);
            });
        }

        #endregion

        #region CrearReporte - Flujos Normales

        [TestMethod]
        public void Prueba_CrearReporte_ReporteValido_RetornaReporteCreado()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);
            var nuevoReporte = EntidadesPruebaFabrica.CrearReporte(0, 1, 2, "Motivo de prueba");

            var resultado = _repositorio.CrearReporte(nuevoReporte);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Motivo de prueba", resultado.Motivo);
        }

        [TestMethod]
        public void Prueba_CrearReporte_ReporteValido_LlamaSaveChanges()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);
            var nuevoReporte = EntidadesPruebaFabrica.CrearReporte(0, 1, 2, "Motivo");

            _repositorio.CrearReporte(nuevoReporte);

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearReporte_ReporteValido_AgregaAlDbSet()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);
            var nuevoReporte = EntidadesPruebaFabrica.CrearReporte(0, 1, 2, "Motivo");

            _repositorio.CrearReporte(nuevoReporte);

            Assert.AreEqual(1, reportes.Count);
        }

        [TestMethod]
        public void Prueba_CrearReporte_ConDatosCompletos_RetornaConTodosLosDatos()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);
            var fechaReporte = DateTime.Now;
            var nuevoReporte = new Reporte
            {
                idReportante = 10,
                idReportado = 20,
                Motivo = "Lenguaje inapropiado",
                Fecha_Reporte = fechaReporte
            };

            var resultado = _repositorio.CrearReporte(nuevoReporte);

            Assert.AreEqual(10, resultado.idReportante);
            Assert.AreEqual(20, resultado.idReportado);
            Assert.AreEqual("Lenguaje inapropiado", resultado.Motivo);
            Assert.AreEqual(fechaReporte, resultado.Fecha_Reporte);
        }

        [TestMethod]
        public void Prueba_CrearReporte_MultiplesReportes_TodosSeAgregan()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);
            var reporte1 = EntidadesPruebaFabrica.CrearReporte(0, 1, 2, "Motivo1");
            var reporte2 = EntidadesPruebaFabrica.CrearReporte(0, 3, 4, "Motivo2");

            _repositorio.CrearReporte(reporte1);
            _repositorio.CrearReporte(reporte2);

            Assert.AreEqual(2, reportes.Count);
        }

        #endregion

        #region CrearReporte - Flujos de Excepcion

        [TestMethod]
        public void Prueba_CrearReporte_ReporteNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.CrearReporte(null);
            });
        }

        [TestMethod]
        public void Prueba_CrearReporte_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new DbUpdateException("Error al guardar"));
            _repositorio = new ReporteRepositorio(_contextoMock.Object);
            var nuevoReporte = EntidadesPruebaFabrica.CrearReporte(0, 1, 2, "Motivo");

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.CrearReporte(nuevoReporte);
            });
        }

        [TestMethod]
        public void Prueba_CrearReporte_ExcepcionGeneral_LanzaBaseDatosExcepcion()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new Exception("Error inesperado"));
            _repositorio = new ReporteRepositorio(_contextoMock.Object);
            var nuevoReporte = EntidadesPruebaFabrica.CrearReporte(0, 1, 2, "Motivo");

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.CrearReporte(nuevoReporte);
            });
        }

        #endregion

        #region ContarReportesRecibidos - Flujos Normales

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_TieneReportes_RetornaCantidadCorrecta()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 1, 5, "Motivo1"),
                EntidadesPruebaFabrica.CrearReporte(2, 2, 5, "Motivo2"),
                EntidadesPruebaFabrica.CrearReporte(3, 3, 5, "Motivo3")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            int resultado = _repositorio.ContarReportesRecibidos(5);

            Assert.AreEqual(3, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_SinReportes_RetornaCero()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 1, 2, "Motivo")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            int resultado = _repositorio.ContarReportesRecibidos(10);

            Assert.AreEqual(0, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_ListaVacia_RetornaCero()
        {
            var reportes = new List<Reporte>();
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            int resultado = _repositorio.ContarReportesRecibidos(1);

            Assert.AreEqual(0, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_UnSoloReporte_RetornaUno()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 1, 3, "Motivo")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            int resultado = _repositorio.ContarReportesRecibidos(3);

            Assert.AreEqual(1, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_NoCuentaReportesComoReportante()
        {
            var reportes = new List<Reporte>
            {
                EntidadesPruebaFabrica.CrearReporte(1, 5, 2, "Motivo1"),
                EntidadesPruebaFabrica.CrearReporte(2, 5, 3, "Motivo2"),
                EntidadesPruebaFabrica.CrearReporte(3, 1, 5, "Motivo3")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            int resultado = _repositorio.ContarReportesRecibidos(5);

            Assert.AreEqual(1, resultado);
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_MuchosReportes_ConteoExacto()
        {
            var reportes = new List<Reporte>();
            int usuarioReportado = 10;
            for (int i = 1; i <= 15; i++)
            {
                reportes.Add(EntidadesPruebaFabrica.CrearReporte(i, i, usuarioReportado, $"Motivo{i}"));
            }
            _contextoMock = ContextoMockFabrica.CrearContextoConReportes(reportes);
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            int resultado = _repositorio.ContarReportesRecibidos(usuarioReportado);

            Assert.AreEqual(15, resultado);
        }

        #endregion

        #region ContarReportesRecibidos - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_IdCero_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ContarReportesRecibidos(0);
            });
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_IdNegativo_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ContarReportesRecibidos(-1);
            });
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Reporte)
                .Throws(new DbUpdateException("Error de base de datos"));
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ContarReportesRecibidos(1);
            });
        }

        [TestMethod]
        public void Prueba_ContarReportesRecibidos_ExcepcionGeneral_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Reporte)
                .Throws(new Exception("Error inesperado"));
            _repositorio = new ReporteRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ContarReportesRecibidos(1);
            });
        }

        #endregion
    }
}
