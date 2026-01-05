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
    public class JugadorRepositorioPruebas
    {
        private const string CorreoExistente = "existente@correo.com";
        private const string CorreoNuevo = "nuevo@correo.com";
        private const string NombreJugador = "JugadorPrueba";

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Jugador>> _jugadorDbSetMock;
        private JugadorRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _jugadorDbSetMock = CrearDbSetMock(new List<Jugador>());

            _contextoMock
                .Setup(contexto => contexto.Jugador)
                .Returns(_jugadorDbSetMock.Object);

            _repositorio = new JugadorRepositorio(_contextoMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JugadorRepositorio(null));
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_CorreoExistenteRetornaTrue()
        {
            var datos = new List<Jugador>
            {
                new Jugador { Correo = CorreoExistente }
            }.AsQueryable();

            ConfigurarDbSet(_jugadorDbSetMock, datos);

            bool resultado = _repositorio.ExisteCorreo(CorreoExistente);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_CorreoNoExistenteRetornaFalse()
        {
            var datos = new List<Jugador>
            {
                new Jugador { Correo = CorreoExistente }
            }.AsQueryable();

            ConfigurarDbSet(_jugadorDbSetMock, datos);

            bool resultado = _repositorio.ExisteCorreo(CorreoNuevo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_ErrorBaseDatosLanzaExcepcionPersonalizada()
        {
            _jugadorDbSetMock.As<IQueryable<Jugador>>()
                .Setup(consulta => consulta.Provider)
                .Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ExisteCorreo(CorreoExistente));
        }

        [TestMethod]
        public void Prueba_CrearJugador_JugadorNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _repositorio.CrearJugador(null));
        }

        [TestMethod]
        public void Prueba_CrearJugador_JugadorValidoGuardaCambios()
        {
            var nuevoJugador = new Jugador
            {
                Correo = CorreoNuevo,
                Nombre = NombreJugador
            };

            _jugadorDbSetMock
                .Setup(conjunto => conjunto.Add(It.IsAny<Jugador>()))
                .Returns<Jugador>(entidad => entidad);

            var resultado = _repositorio.CrearJugador(nuevoJugador);

            _jugadorDbSetMock.Verify(
                conjuntoJugadores => conjuntoJugadores.Add(It.IsAny<Jugador>()),
                Times.Once);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
            Assert.AreEqual(CorreoNuevo, resultado.Correo);
            Assert.AreEqual(NombreJugador, resultado.Nombre);
        }

        [TestMethod]
        public void Prueba_CrearJugador_ErrorGuardarLanzaExcepcionPersonalizada()
        {
            var nuevoJugador = new Jugador
            {
                Correo = CorreoNuevo
            };

            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new DbUpdateException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.CrearJugador(nuevoJugador));
        }

        [TestMethod]
        public void Prueba_CrearJugador_ErrorEntityLanzaExcepcionPersonalizada()
        {
            var nuevoJugador = new Jugador
            {
                Correo = CorreoNuevo
            };

            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.CrearJugador(nuevoJugador));
        }

        private static Mock<DbSet<T>> CrearDbSetMock<T>(List<T> datos) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            ConfigurarDbSet(mockSet, datos.AsQueryable());
            
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
            mockSet.Setup(dbSet => dbSet.Include(It.IsAny<string>())).Returns(mockSet.Object);
        }
    }
}