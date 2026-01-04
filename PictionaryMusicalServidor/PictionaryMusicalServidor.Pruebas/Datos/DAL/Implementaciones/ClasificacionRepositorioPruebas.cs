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
    /// Contiene las pruebas unitarias para la clase ClasificacionRepositorio.
    /// Verifica flujos normales, alternos y de excepcion para la gestion de clasificaciones.
    /// </summary>
    [TestClass]
    public class ClasificacionRepositorioPruebas
    {
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private ClasificacionRepositorio _repositorio;

        /// <summary>
        /// Inicializa los mocks y el repositorio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = ContextoMockFabrica.CrearContextoMock();
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);
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
                var repositorio = new ClasificacionRepositorio(null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoValido_CreaInstancia()
        {
            var contexto = ContextoMockFabrica.CrearContextoMock();

            var repositorio = new ClasificacionRepositorio(contexto.Object);

            Assert.IsNotNull(repositorio);
        }

        #endregion

        #region CrearClasificacionInicial - Flujos Normales

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_CreaConPuntosCero()
        {
            var clasificaciones = new List<Clasificacion>();
            _contextoMock = ContextoMockFabrica.CrearContextoConClasificaciones(clasificaciones);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.CrearClasificacionInicial();

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Puntos_Ganados);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_CreaConRondasCero()
        {
            var clasificaciones = new List<Clasificacion>();
            _contextoMock = ContextoMockFabrica.CrearContextoConClasificaciones(clasificaciones);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.CrearClasificacionInicial();

            Assert.AreEqual(0, resultado.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_LlamaSaveChanges()
        {
            var clasificaciones = new List<Clasificacion>();
            _contextoMock = ContextoMockFabrica.CrearContextoConClasificaciones(clasificaciones);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            _repositorio.CrearClasificacionInicial();

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_AgregaAlDbSet()
        {
            var clasificaciones = new List<Clasificacion>();
            _contextoMock = ContextoMockFabrica.CrearContextoConClasificaciones(clasificaciones);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            _repositorio.CrearClasificacionInicial();

            Assert.AreEqual(1, clasificaciones.Count);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_RetornaClasificacionCreada()
        {
            var clasificaciones = new List<Clasificacion>();
            _contextoMock = ContextoMockFabrica.CrearContextoConClasificaciones(clasificaciones);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.CrearClasificacionInicial();

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_MultiplesCreaciones_TodasSeAgregan()
        {
            var clasificaciones = new List<Clasificacion>();
            _contextoMock = ContextoMockFabrica.CrearContextoConClasificaciones(clasificaciones);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            _repositorio.CrearClasificacionInicial();
            _repositorio.CrearClasificacionInicial();
            _repositorio.CrearClasificacionInicial();

            Assert.AreEqual(3, clasificaciones.Count);
        }

        #endregion

        #region CrearClasificacionInicial - Flujos de Excepcion

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            var clasificaciones = new List<Clasificacion>();
            _contextoMock = ContextoMockFabrica.CrearContextoConClasificaciones(clasificaciones);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new DbUpdateException("Error al guardar"));
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.CrearClasificacionInicial();
            });
        }

        [TestMethod]
        public void Prueba_CrearClasificacionInicial_ExcepcionGeneral_LanzaBaseDatosExcepcion()
        {
            var clasificaciones = new List<Clasificacion>();
            _contextoMock = ContextoMockFabrica.CrearContextoConClasificaciones(clasificaciones);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new Exception("Error inesperado"));
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.CrearClasificacionInicial();
            });
        }

        #endregion

        #region ActualizarEstadisticas - Flujos Normales

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_JugadorExiste_ActualizaPuntos()
        {
            var clasificacion = EntidadesPruebaFabrica.CrearClasificacion(1, 100, 5);
            var jugador = EntidadesPruebaFabrica.CrearJugador(1, clasificacion);
            var jugadores = new List<Jugador> { jugador };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ActualizarEstadisticas(1, 50, false);

            Assert.IsTrue(resultado);
            Assert.AreEqual(150, clasificacion.Puntos_Ganados);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_JugadorGanoPartida_IncrementaRondas()
        {
            var clasificacion = EntidadesPruebaFabrica.CrearClasificacion(1, 100, 5);
            var jugador = EntidadesPruebaFabrica.CrearJugador(1, clasificacion);
            var jugadores = new List<Jugador> { jugador };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            _repositorio.ActualizarEstadisticas(1, 50, true);

            Assert.AreEqual(6, clasificacion.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_JugadorNoGanoPartida_NoIncrementaRondas()
        {
            var clasificacion = EntidadesPruebaFabrica.CrearClasificacion(1, 100, 5);
            var jugador = EntidadesPruebaFabrica.CrearJugador(1, clasificacion);
            var jugadores = new List<Jugador> { jugador };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            _repositorio.ActualizarEstadisticas(1, 50, false);

            Assert.AreEqual(5, clasificacion.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_PuntosNulos_InicializaACero()
        {
            var clasificacion = new Clasificacion
            {
                idClasificacion = 1,
                Puntos_Ganados = null,
                Rondas_Ganadas = null
            };
            var jugador = EntidadesPruebaFabrica.CrearJugador(1, clasificacion);
            var jugadores = new List<Jugador> { jugador };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            _repositorio.ActualizarEstadisticas(1, 25, true);

            Assert.AreEqual(25, clasificacion.Puntos_Ganados);
            Assert.AreEqual(1, clasificacion.Rondas_Ganadas);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_LlamaSaveChanges()
        {
            var clasificacion = EntidadesPruebaFabrica.CrearClasificacion(1, 100, 5);
            var jugador = EntidadesPruebaFabrica.CrearJugador(1, clasificacion);
            var jugadores = new List<Jugador> { jugador };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            _repositorio.ActualizarEstadisticas(1, 50, false);

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        #endregion

        #region ActualizarEstadisticas - Flujos Alternos

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_JugadorNoExiste_RetornaFalse()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ActualizarEstadisticas(999, 50, true);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_JugadorSinClasificacion_RetornaFalse()
        {
            var jugador = new Jugador
            {
                idJugador = 1,
                Nombre = "Test",
                Apellido = "Usuario",
                Correo = "test@ejemplo.com",
                Clasificacion = null
            };
            var jugadores = new List<Jugador> { jugador };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ActualizarEstadisticas(1, 50, true);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_PuntosCero_NoModificaPuntos()
        {
            var clasificacion = EntidadesPruebaFabrica.CrearClasificacion(1, 100, 5);
            var jugador = EntidadesPruebaFabrica.CrearJugador(1, clasificacion);
            var jugadores = new List<Jugador> { jugador };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            _repositorio.ActualizarEstadisticas(1, 0, false);

            Assert.AreEqual(100, clasificacion.Puntos_Ganados);
        }

        #endregion

        #region ActualizarEstadisticas - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ActualizarEstadisticas_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            var clasificacion = EntidadesPruebaFabrica.CrearClasificacion(1, 100, 5);
            var jugador = EntidadesPruebaFabrica.CrearJugador(1, clasificacion);
            var jugadores = new List<Jugador> { jugador };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new DbUpdateException("Error al actualizar"));
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ActualizarEstadisticas(1, 50, true);
            });
        }

        #endregion

        #region ObtenerMejoresJugadores - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_ConDatos_RetornaLista()
        {
            var clasificacion1 = EntidadesPruebaFabrica.CrearClasificacion(1, 100, 5);
            var clasificacion2 = EntidadesPruebaFabrica.CrearClasificacion(2, 200, 10);
            var jugador1 = EntidadesPruebaFabrica.CrearJugador(1, clasificacion1);
            var jugador2 = EntidadesPruebaFabrica.CrearJugador(2, clasificacion2);
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = 1,
                    Nombre_Usuario = "Usuario1",
                    Jugador = jugador1
                },
                new Usuario
                {
                    idUsuario = 2,
                    Nombre_Usuario = "Usuario2",
                    Jugador = jugador2
                }
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerMejoresJugadores(10);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_LimiteCantidad_RetornaMaximoCantidad()
        {
            var usuarios = new List<Usuario>();
            for (int i = 1; i <= 5; i++)
            {
                var clasificacion = EntidadesPruebaFabrica.CrearClasificacion(i, i * 10, i);
                var jugador = EntidadesPruebaFabrica.CrearJugador(i, clasificacion);
                usuarios.Add(new Usuario
                {
                    idUsuario = i,
                    Nombre_Usuario = $"Usuario{i}",
                    Jugador = jugador
                });
            }
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerMejoresJugadores(3);

            Assert.AreEqual(3, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_OrdenadoPorPuntos_PrimerEsMayorPuntos()
        {
            var clasificacion1 = EntidadesPruebaFabrica.CrearClasificacion(1, 50, 5);
            var clasificacion2 = EntidadesPruebaFabrica.CrearClasificacion(2, 200, 10);
            var jugador1 = EntidadesPruebaFabrica.CrearJugador(1, clasificacion1);
            var jugador2 = EntidadesPruebaFabrica.CrearJugador(2, clasificacion2);
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = 1,
                    Nombre_Usuario = "UsuarioMenos",
                    Jugador = jugador1
                },
                new Usuario
                {
                    idUsuario = 2,
                    Nombre_Usuario = "UsuarioMas",
                    Jugador = jugador2
                }
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerMejoresJugadores(10);

            Assert.AreEqual("UsuarioMas", resultado[0].Nombre_Usuario);
        }

        #endregion

        #region ObtenerMejoresJugadores - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_ListaVacia_RetornaListaVacia()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerMejoresJugadores(10);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_UsuariosSinJugador_NoSeIncluyen()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "SinJugador")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerMejoresJugadores(10);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_CantidadCero_RetornaListaVacia()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuario(1, "Usuario1")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerMejoresJugadores(0);

            Assert.AreEqual(0, resultado.Count);
        }

        #endregion

        #region ObtenerMejoresJugadores - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Usuario)
                .Throws(new DbUpdateException("Error de base de datos"));
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ObtenerMejoresJugadores(10);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerMejoresJugadores_ExcepcionGeneral_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Usuario)
                .Throws(new Exception("Error inesperado"));
            _repositorio = new ClasificacionRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ObtenerMejoresJugadores(10);
            });
        }

        #endregion
    }
}
