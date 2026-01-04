using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;
using PictionaryMusicalServidor.Datos.Excepciones;

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL.Implementaciones
{
    [TestClass]
    public class AmigoRepositorioPruebas
    {
        private const int IdUsuarioEmisor = 1;
        private const int IdUsuarioReceptor = 2;
        private const int IdInvalido = -1;
        private const int IdCero = 0;
        private const int IdInexistente = -5;

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Amigo>> _amigoDbSetMock;
        private AmigoRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _amigoDbSetMock = new Mock<DbSet<Amigo>>();
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
                new AmigoRepositorio(null);
            });
        }

        #endregion

        #region ExisteRelacion

        [TestMethod]
        public void Prueba_ExisteRelacionExistente_RetornaTrue()
        {
            var amigos = new List<Amigo>
            {
                new Amigo 
                { 
                    UsuarioEmisor = IdUsuarioEmisor, 
                    UsuarioReceptor = IdUsuarioReceptor, 
                    Estado = false 
                }
            }.AsQueryable();

            ConfigurarDbSetMock(_amigoDbSetMock, amigos);
            _contextoMock.Setup(c => c.Amigo).Returns(_amigoDbSetMock.Object);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteRelacion(IdUsuarioEmisor, IdUsuarioReceptor);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteRelacionInexistente_RetornaFalse()
        {
            var amigos = new List<Amigo>().AsQueryable();

            ConfigurarDbSetMock(_amigoDbSetMock, amigos);
            _contextoMock.Setup(c => c.Amigo).Returns(_amigoDbSetMock.Object);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteRelacion(IdUsuarioEmisor, IdUsuarioReceptor);

            Assert.IsFalse(resultado);
        }

        #endregion

        #region ObtenerRelacion

        [TestMethod]
        public void Prueba_ObtenerRelacionInexistente_LanzaBaseDatosExcepcion()
        {
            var amigos = new List<Amigo>().AsQueryable();

            ConfigurarDbSetMock(_amigoDbSetMock, amigos);
            _contextoMock.Setup(c => c.Amigo).Returns(_amigoDbSetMock.Object);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerRelacionExistente_RetornaRelacionCorrecta()
        {
            var amigoEsperado = new Amigo 
            { 
                UsuarioEmisor = IdUsuarioEmisor, 
                UsuarioReceptor = IdUsuarioReceptor, 
                Estado = true 
            };
            var amigos = new List<Amigo> { amigoEsperado }.AsQueryable();

            ConfigurarDbSetMock(_amigoDbSetMock, amigos);
            _contextoMock.Setup(c => c.Amigo).Returns(_amigoDbSetMock.Object);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Amigo resultado = _repositorio.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor);

            Assert.AreEqual(IdUsuarioEmisor, resultado.UsuarioEmisor);
        }

        #endregion

        #region ObtenerSolicitudesPendientes

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesIdCero_LanzaArgumentOutOfRangeException()
        {
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerSolicitudesPendientes(IdCero);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesIdNegativo_LanzaArgumentOutOfRangeException()
        {
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerSolicitudesPendientes(IdInvalido);
            });
        }

        #endregion

        #region ActualizarEstado

        [TestMethod]
        public void Prueba_ActualizarEstadoRelacionNula_LanzaArgumentNullException()
        {
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.ActualizarEstado(null, true);
            });
        }

        #endregion

        #region EliminarRelacion

        [TestMethod]
        public void Prueba_EliminarRelacionNula_LanzaArgumentNullException()
        {
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.EliminarRelacion(null);
            });
        }

        #endregion

        #region ObtenerAmigos

        [TestMethod]
        public void Prueba_ObtenerAmigosIdCero_LanzaArgumentOutOfRangeException()
        {
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerAmigos(IdCero);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerAmigosIdNegativo_LanzaArgumentOutOfRangeException()
        {
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerAmigos(IdInexistente);
            });
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
