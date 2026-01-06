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
    public class ClasificacionRepositorioPruebas
    {
        private const int IdJugadorPrueba = 1;
        private const int IdJugadorInexistente = 999;
        private const int PuntosObtenidosPrueba = 100;
        private const int PuntosInicialesPrueba = 50;
        private const int RondasInicialesPrueba = 5;
        private const int ValorInicialCero = 0;

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ClasificacionRepositorio(null));
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_AgregaClasificacionAlContexto()
        {
            var listaClasificaciones = new List<Clasificacion>();
            var mockDbSet = CrearMockDbSetConAdd(listaClasificaciones);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Clasificacion).Returns(mockDbSet.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            repositorio.CrearClasificacionInicial();

            mockDbSet.Verify(
                dbSet => dbSet.Add(It.IsAny<Clasificacion>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_GuardaCambiosEnContexto()
        {
            var listaClasificaciones = new List<Clasificacion>();
            var mockDbSet = CrearMockDbSetConAdd(listaClasificaciones);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Clasificacion).Returns(mockDbSet.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            repositorio.CrearClasificacionInicial();

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_RetornaClasificacionConPuntosCero()
        {
            Clasificacion clasificacionAgregada = null;
            var mockDbSet = new Mock<DbSet<Clasificacion>>();
            mockDbSet.Setup(conjunto => conjunto.Add(It.IsAny<Clasificacion>()))
                .Callback<Clasificacion>(clasificacion => clasificacionAgregada = clasificacion)
                .Returns<Clasificacion>(clasificacion => clasificacion);

            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Clasificacion).Returns(mockDbSet.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            repositorio.CrearClasificacionInicial();

            Assert.AreEqual(ValorInicialCero, clasificacionAgregada.Puntos_Ganados);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_RetornaClasificacionConRondasCero()
        {
            Clasificacion clasificacionAgregada = null;
            var mockDbSet = new Mock<DbSet<Clasificacion>>();
            mockDbSet.Setup(conjunto => conjunto.Add(It.IsAny<Clasificacion>()))
                .Callback<Clasificacion>(clasificacion => clasificacionAgregada = clasificacion)
                .Returns<Clasificacion>(clasificacion => clasificacion);

            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Clasificacion).Returns(mockDbSet.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            repositorio.CrearClasificacionInicial();

            Assert.AreEqual(ValorInicialCero, clasificacionAgregada.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_RetornaFalsoJugadorNoEncontrado()
        {
            var datosJugadores = new List<Jugador>().AsQueryable();
            var mockDbSetJugador = CrearMockDbSetJugador(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSetJugador.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            bool resultado = repositorio.ActualizarEstadisticas(
                IdJugadorInexistente,
                PuntosObtenidosPrueba,
                false);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_RetornaVerdaderoJugadorEncontrado()
        {
            var clasificacion = new Clasificacion
            {
                idClasificacion = 1,
                Puntos_Ganados = PuntosInicialesPrueba,
                Rondas_Ganadas = RondasInicialesPrueba
            };

            var jugador = new Jugador
            {
                idJugador = IdJugadorPrueba,
                Clasificacion = clasificacion
            };

            var datosJugadores = new List<Jugador> { jugador }.AsQueryable();
            var mockDbSetJugador = CrearMockDbSetJugador(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSetJugador.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            bool resultado = repositorio.ActualizarEstadisticas(
                IdJugadorPrueba,
                PuntosObtenidosPrueba,
                false);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_SumaPuntosCorrectamente()
        {
            var clasificacion = new Clasificacion
            {
                idClasificacion = 1,
                Puntos_Ganados = PuntosInicialesPrueba,
                Rondas_Ganadas = RondasInicialesPrueba
            };

            var jugador = new Jugador
            {
                idJugador = IdJugadorPrueba,
                Clasificacion = clasificacion
            };

            var datosJugadores = new List<Jugador> { jugador }.AsQueryable();
            var mockDbSetJugador = CrearMockDbSetJugador(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSetJugador.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            repositorio.ActualizarEstadisticas(IdJugadorPrueba, PuntosObtenidosPrueba, false);

            int puntosEsperados = PuntosInicialesPrueba + PuntosObtenidosPrueba;
            Assert.AreEqual(puntosEsperados, clasificacion.Puntos_Ganados);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_IncrementaRondasSiGanoPartida()
        {
            var clasificacion = new Clasificacion
            {
                idClasificacion = 1,
                Puntos_Ganados = PuntosInicialesPrueba,
                Rondas_Ganadas = RondasInicialesPrueba
            };

            var jugador = new Jugador
            {
                idJugador = IdJugadorPrueba,
                Clasificacion = clasificacion
            };

            var datosJugadores = new List<Jugador> { jugador }.AsQueryable();
            var mockDbSetJugador = CrearMockDbSetJugador(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSetJugador.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            repositorio.ActualizarEstadisticas(IdJugadorPrueba, PuntosObtenidosPrueba, true);

            int rondasEsperadas = RondasInicialesPrueba + 1;
            Assert.AreEqual(rondasEsperadas, clasificacion.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_NoIncrementaRondasSiNoGanoPartida()
        {
            var clasificacion = new Clasificacion
            {
                idClasificacion = 1,
                Puntos_Ganados = PuntosInicialesPrueba,
                Rondas_Ganadas = RondasInicialesPrueba
            };

            var jugador = new Jugador
            {
                idJugador = IdJugadorPrueba,
                Clasificacion = clasificacion
            };

            var datosJugadores = new List<Jugador> { jugador }.AsQueryable();
            var mockDbSetJugador = CrearMockDbSetJugador(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSetJugador.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            repositorio.ActualizarEstadisticas(IdJugadorPrueba, PuntosObtenidosPrueba, false);

            Assert.AreEqual(RondasInicialesPrueba, clasificacion.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_GuardaCambiosEnContexto()
        {
            var clasificacion = new Clasificacion
            {
                idClasificacion = 1,
                Puntos_Ganados = PuntosInicialesPrueba,
                Rondas_Ganadas = RondasInicialesPrueba
            };

            var jugador = new Jugador
            {
                idJugador = IdJugadorPrueba,
                Clasificacion = clasificacion
            };

            var datosJugadores = new List<Jugador> { jugador }.AsQueryable();
            var mockDbSetJugador = CrearMockDbSetJugador(datosJugadores);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Jugador).Returns(mockDbSetJugador.Object);

            var repositorio = new ClasificacionRepositorio(mockContexto.Object);

            repositorio.ActualizarEstadisticas(IdJugadorPrueba, PuntosObtenidosPrueba, false);

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        private static Mock<DbSet<Clasificacion>> CrearMockDbSetConAdd(List<Clasificacion> lista)
        {
            var datos = lista.AsQueryable();
            var mockDbSet = new Mock<DbSet<Clasificacion>>();
            mockDbSet.As<IQueryable<Clasificacion>>()
                .Setup(conjunto => conjunto.Provider)
                .Returns(datos.Provider);
            mockDbSet.As<IQueryable<Clasificacion>>()
                .Setup(conjunto => conjunto.Expression)
                .Returns(datos.Expression);
            mockDbSet.As<IQueryable<Clasificacion>>()
                .Setup(conjunto => conjunto.ElementType)
                .Returns(datos.ElementType);
            mockDbSet.As<IQueryable<Clasificacion>>()
                .Setup(conjunto => conjunto.GetEnumerator())
                .Returns(datos.GetEnumerator());
            mockDbSet.Setup(conjunto => conjunto.Add(It.IsAny<Clasificacion>()))
                .Callback<Clasificacion>(item => lista.Add(item))
                .Returns<Clasificacion>(item => item);
            return mockDbSet;
        }

        private static Mock<DbSet<Jugador>> CrearMockDbSetJugador(IQueryable<Jugador> datos)
        {
            var mockDbSet = new Mock<DbSet<Jugador>>();
            mockDbSet.As<IQueryable<Jugador>>()
                .Setup(conjunto => conjunto.Provider)
                .Returns(datos.Provider);
            mockDbSet.As<IQueryable<Jugador>>()
                .Setup(conjunto => conjunto.Expression)
                .Returns(datos.Expression);
            mockDbSet.As<IQueryable<Jugador>>()
                .Setup(conjunto => conjunto.ElementType)
                .Returns(datos.ElementType);
            mockDbSet.As<IQueryable<Jugador>>()
                .Setup(conjunto => conjunto.GetEnumerator())
                .Returns(datos.GetEnumerator());
            mockDbSet.Setup(conjunto => conjunto.Include(It.IsAny<string>()))
                .Returns(mockDbSet.Object);
            return mockDbSet;
        }
    }
}
