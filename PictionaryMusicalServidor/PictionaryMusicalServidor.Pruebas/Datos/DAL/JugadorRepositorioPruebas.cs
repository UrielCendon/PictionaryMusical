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
    public class JugadorRepositorioPruebas
    {
        private const string CorreoExistentePrueba = "jugador@correo.com";
        private const string CorreoInexistentePrueba = "inexistente@correo.com";
        private const string NombrePrueba = "JugadorPrueba";
        private const string ApellidoPrueba = "ApellidoPrueba";
        private const int IdJugadorPrueba = 1;
        private const int IdClasificacionPrueba = 1;

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new JugadorRepositorio(null));
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_RetornaVerdaderoCorreoExistente()
        {
            var datosJugadores = new List<Jugador>
            {
                new Jugador
                {
                    idJugador = IdJugadorPrueba,
                    Correo = CorreoExistentePrueba,
                    Nombre = NombrePrueba,
                    Apellido = ApellidoPrueba
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSet.Object);

            var repositorio = new JugadorRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteCorreo(CorreoExistentePrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_RetornaFalsoCorreoInexistente()
        {
            var datosJugadores = new List<Jugador>
            {
                new Jugador
                {
                    idJugador = IdJugadorPrueba,
                    Correo = CorreoExistentePrueba,
                    Nombre = NombrePrueba,
                    Apellido = ApellidoPrueba
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSet.Object);

            var repositorio = new JugadorRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteCorreo(CorreoInexistentePrueba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_RetornaFalsoListaVacia()
        {
            var datosJugadores = new List<Jugador>().AsQueryable();

            var mockDbSet = CrearMockDbSet(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSet.Object);

            var repositorio = new JugadorRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteCorreo(CorreoExistentePrueba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_CrearJugador_LanzaExcepcionJugadorNulo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new JugadorRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => repositorio.CrearJugador(null));
        }

        [TestMethod]
        public void Prueba_CrearJugador_AgregaJugadorAlContexto()
        {
            var jugadorNuevo = new Jugador
            {
                Correo = CorreoExistentePrueba,
                Nombre = NombrePrueba,
                Apellido = ApellidoPrueba,
                Clasificacion_idClasificacion = IdClasificacionPrueba
            };

            var listaJugadores = new List<Jugador>();
            var mockDbSet = CrearMockDbSetConAdd(listaJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSet.Object);

            var repositorio = new JugadorRepositorio(mockContexto.Object);

            repositorio.CrearJugador(jugadorNuevo);

            mockDbSet.Verify(
                dbSet => dbSet.Add(It.IsAny<Jugador>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearJugador_GuardaCambiosEnContexto()
        {
            var jugadorNuevo = new Jugador
            {
                Correo = CorreoExistentePrueba,
                Nombre = NombrePrueba,
                Apellido = ApellidoPrueba,
                Clasificacion_idClasificacion = IdClasificacionPrueba
            };

            var listaJugadores = new List<Jugador>();
            var mockDbSet = CrearMockDbSetConAdd(listaJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSet.Object);

            var repositorio = new JugadorRepositorio(mockContexto.Object);

            repositorio.CrearJugador(jugadorNuevo);

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearJugador_RetornaJugadorCreado()
        {
            var jugadorNuevo = new Jugador
            {
                Correo = CorreoExistentePrueba,
                Nombre = NombrePrueba,
                Apellido = ApellidoPrueba,
                Clasificacion_idClasificacion = IdClasificacionPrueba
            };

            var mockDbSet = new Mock<DbSet<Jugador>>();
            mockDbSet.Setup(dbSet => dbSet.Add(It.IsAny<Jugador>()))
                .Returns<Jugador>(jugador => jugador);

            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSet.Object);

            var repositorio = new JugadorRepositorio(mockContexto.Object);

            var resultado = repositorio.CrearJugador(jugadorNuevo);

            Assert.AreEqual(CorreoExistentePrueba, resultado.Correo);
        }

        private Mock<DbSet<Jugador>> CrearMockDbSet(IQueryable<Jugador> datos)
        {
            var mockDbSet = new Mock<DbSet<Jugador>>();
            mockDbSet.As<IQueryable<Jugador>>()
                .Setup(dbSet => dbSet.Provider)
                .Returns(datos.Provider);
            mockDbSet.As<IQueryable<Jugador>>()
                .Setup(dbSet => dbSet.Expression)
                .Returns(datos.Expression);
            mockDbSet.As<IQueryable<Jugador>>()
                .Setup(dbSet => dbSet.ElementType)
                .Returns(datos.ElementType);
            mockDbSet.As<IQueryable<Jugador>>()
                .Setup(dbSet => dbSet.GetEnumerator())
                .Returns(datos.GetEnumerator());
            return mockDbSet;
        }

        private Mock<DbSet<Jugador>> CrearMockDbSetConAdd(List<Jugador> listaJugadores)
        {
            var datos = listaJugadores.AsQueryable();
            var mockDbSet = CrearMockDbSet(datos);
            mockDbSet.Setup(dbSet => dbSet.Add(It.IsAny<Jugador>()))
                .Callback<Jugador>(jugador => listaJugadores.Add(jugador))
                .Returns<Jugador>(jugador => jugador);
            return mockDbSet;
        }
    }
}
