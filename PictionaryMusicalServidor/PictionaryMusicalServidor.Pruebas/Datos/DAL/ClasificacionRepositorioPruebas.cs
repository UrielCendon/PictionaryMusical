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

namespace PictionaryMusicalServidor.Pruebas.Datos.DAL
{
    [TestClass]
    public class ClasificacionRepositorioPruebas
    {
        private const int IdJugadorValido = 10;
        private const int IdJugadorSinClasificacion = 20;
        private const int IdJugadorInexistente = 99;
        private const int PuntosAIncrementar = 50;
        private const int PuntosIniciales = 10;
        private const int RondasIniciales = 2;
        private const int CantidadMejoresJugadores = 3;
        private const bool GanoPartidaTrue = true;
        private const bool GanoPartidaFalse = false;
        private const string NombreUsuarioUno = "JugadorUno";
        private const string NombreUsuarioDos = "JugadorDos";

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Clasificacion>> _clasificacionDbSetMock;
        private Mock<DbSet<Jugador>> _jugadorDbSetMock;
        private Mock<DbSet<Usuario>> _usuarioDbSetMock;
        private ClasificacionRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _clasificacionDbSetMock = CrearDbSetMock(new List<Clasificacion>());
            _jugadorDbSetMock = CrearDbSetMock(new List<Jugador>());
            _usuarioDbSetMock = CrearDbSetMock(new List<Usuario>());

            _contextoMock
                .Setup(contexto => contexto.Clasificacion)
                .Returns(_clasificacionDbSetMock.Object);
            _contextoMock
                .Setup(contexto => contexto.Jugador)
                .Returns(_jugadorDbSetMock.Object);
            _contextoMock
                .Setup(contexto => contexto.Usuario)
                .Returns(_usuarioDbSetMock.Object);

            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ClasificacionRepositorio(null));
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_CorrectoGuardaCambios()
        {
            _clasificacionDbSetMock
                .Setup(conjunto => conjunto.Add(It.IsAny<Clasificacion>()))
                .Callback<Clasificacion>((clasificacionAgregada) => { });

            var resultado = _repositorio.CrearClasificacionInicial();

            _clasificacionDbSetMock.Verify(
                conjunto => conjunto.Add(It.IsAny<Clasificacion>()),
                Times.Once);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Puntos_Ganados);
            Assert.AreEqual(0, resultado.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_ErrorBaseDatosLanzaExcepcion()
        {
            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new DbUpdateException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.CrearClasificacionInicial());
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_JugadorNoExisteRetornaFalse()
        {
            var datosJugadores = new List<Jugador>().AsQueryable();
            ConfigurarDbSet(_jugadorDbSetMock, datosJugadores);

            var resultado = _repositorio.ActualizarEstadisticas(
                IdJugadorInexistente,
                PuntosAIncrementar,
                GanoPartidaTrue);

            Assert.IsFalse(resultado);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_ClasificacionNulaRetornaFalse()
        {
            var datosJugadores = new List<Jugador>
            {
                new Jugador { idJugador = IdJugadorSinClasificacion, Clasificacion = null }
            }.AsQueryable();
            ConfigurarDbSet(_jugadorDbSetMock, datosJugadores);

            var resultado = _repositorio.ActualizarEstadisticas(
                IdJugadorSinClasificacion,
                PuntosAIncrementar,
                GanoPartidaTrue);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_GanoPartidaActualizaPuntosYRondas()
        {
            var clasificacion = new Clasificacion
            {
                Puntos_Ganados = PuntosIniciales,
                Rondas_Ganadas = RondasIniciales
            };
            var jugador = new Jugador
            {
                idJugador = IdJugadorValido,
                Clasificacion = clasificacion
            };

            var datosJugadores = new List<Jugador> { jugador }.AsQueryable();
            ConfigurarDbSet(_jugadorDbSetMock, datosJugadores);

            var resultado = _repositorio.ActualizarEstadisticas(
                IdJugadorValido,
                PuntosAIncrementar,
                GanoPartidaTrue);

            Assert.IsTrue(resultado);
            Assert.AreEqual(PuntosIniciales + PuntosAIncrementar, clasificacion.Puntos_Ganados);
            Assert.AreEqual(RondasIniciales + 1, clasificacion.Rondas_Ganadas);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_PerdioPartidaSoloActualizaPuntos()
        {
            var clasificacion = new Clasificacion
            {
                Puntos_Ganados = PuntosIniciales,
                Rondas_Ganadas = RondasIniciales
            };
            var jugador = new Jugador
            {
                idJugador = IdJugadorValido,
                Clasificacion = clasificacion
            };

            var datosJugadores = new List<Jugador> { jugador }.AsQueryable();
            ConfigurarDbSet(_jugadorDbSetMock, datosJugadores);

            var resultado = _repositorio.ActualizarEstadisticas(
                IdJugadorValido,
                PuntosAIncrementar,
                GanoPartidaFalse);

            Assert.IsTrue(resultado);
            Assert.AreEqual(PuntosIniciales + PuntosAIncrementar, clasificacion.Puntos_Ganados);
            Assert.AreEqual(RondasIniciales, clasificacion.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_ErrorEntityLanzaExcepcion()
        {
            var jugador = new Jugador
            {
                idJugador = IdJugadorValido,
                Clasificacion = new Clasificacion()
            };
            var datosJugadores = new List<Jugador> { jugador }.AsQueryable();
            ConfigurarDbSet(_jugadorDbSetMock, datosJugadores);

            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ActualizarEstadisticas(
                    IdJugadorValido,
                    PuntosAIncrementar,
                    GanoPartidaTrue));
        }

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_RetornaListaOrdenada()
        {
            var clasificacionBaja = new Clasificacion
            {
                Puntos_Ganados = 10,
                Rondas_Ganadas = 1
            };
            var clasificacionAlta = new Clasificacion
            {
                Puntos_Ganados = 100,
                Rondas_Ganadas = 5
            };

            var usuarioBajo = new Usuario
            {
                Nombre_Usuario = NombreUsuarioDos,
                Jugador = new Jugador { Clasificacion = clasificacionBaja }
            };
            var usuarioAlto = new Usuario
            {
                Nombre_Usuario = NombreUsuarioUno,
                Jugador = new Jugador { Clasificacion = clasificacionAlta }
            };

            var datosUsuarios = new List<Usuario> { usuarioBajo, usuarioAlto }.AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datosUsuarios);

            var resultados = _repositorio.ObtenerMejoresJugadores(CantidadMejoresJugadores);

            Assert.AreEqual(2, resultados.Count);
            Assert.AreEqual(NombreUsuarioUno, resultados[0].Nombre_Usuario);
            Assert.AreEqual(NombreUsuarioDos, resultados[1].Nombre_Usuario);
        }

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_FiltraJugadoresNulos()
        {
            var usuarioValido = new Usuario
            {
                Jugador = new Jugador { Clasificacion = new Clasificacion() }
            };
            var usuarioSinJugador = new Usuario { Jugador = null };
            var usuarioSinClasificacion = new Usuario
            {
                Jugador = new Jugador { Clasificacion = null }
            };

            var datos = new List<Usuario>
            {
                usuarioValido,
                usuarioSinJugador,
                usuarioSinClasificacion
            }.AsQueryable();

            ConfigurarDbSet(_usuarioDbSetMock, datos);

            var resultados = _repositorio.ObtenerMejoresJugadores(CantidadMejoresJugadores);

            Assert.AreEqual(1, resultados.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_ErrorConexionLanzaExcepcion()
        {
            _usuarioDbSetMock.As<IQueryable<Usuario>>()
                .Setup(consulta => consulta.Provider)
                .Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ObtenerMejoresJugadores(CantidadMejoresJugadores));
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
            mockSet.Setup(dbSet => dbSet.Include(It.IsAny<string>())).Returns(mockSet.Object);
        }
    }
}