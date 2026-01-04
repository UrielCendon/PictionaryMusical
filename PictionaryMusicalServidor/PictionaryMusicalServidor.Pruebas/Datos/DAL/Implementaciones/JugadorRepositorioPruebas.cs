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
    public class JugadorRepositorioPruebas
    {
        private const int IdJugadorValido = 1;
        private const string CorreoPrueba = "test@ejemplo.com";
        private const string CorreoOtro = "otro@ejemplo.com";

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Jugador>> _jugadorDbSetMock;
        private JugadorRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _jugadorDbSetMock = new Mock<DbSet<Jugador>>();
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
                new JugadorRepositorio(null);
            });
        }

        #endregion

        #region ExisteCorreo

        [TestMethod]
        public void Prueba_ExisteCorreoExistente_RetornaTrue()
        {
            var jugadores = new List<Jugador>
            {
                new Jugador { idJugador = IdJugadorValido, Correo = CorreoPrueba }
            }.AsQueryable();

            ConfigurarDbSetMock(_jugadorDbSetMock, jugadores);
            _contextoMock.Setup(c => c.Jugador).Returns(_jugadorDbSetMock.Object);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo(CorreoPrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreoInexistente_RetornaFalse()
        {
            var jugadores = new List<Jugador>
            {
                new Jugador { idJugador = IdJugadorValido, Correo = CorreoOtro }
            }.AsQueryable();

            ConfigurarDbSetMock(_jugadorDbSetMock, jugadores);
            _contextoMock.Setup(c => c.Jugador).Returns(_jugadorDbSetMock.Object);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo(CorreoPrueba);

            Assert.IsFalse(resultado);
        }

        #endregion

        #region CrearJugador

        [TestMethod]
        public void Prueba_CrearJugadorNulo_LanzaArgumentNullException()
        {
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.CrearJugador(null);
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
