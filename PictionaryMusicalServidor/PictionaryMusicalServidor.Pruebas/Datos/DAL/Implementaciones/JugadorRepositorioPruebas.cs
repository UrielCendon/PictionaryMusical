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
    /// Contiene las pruebas unitarias para la clase JugadorRepositorio.
    /// Verifica flujos normales, alternos y de excepcion para la gestion de jugadores.
    /// </summary>
    [TestClass]
    public class JugadorRepositorioPruebas
    {
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private JugadorRepositorio _repositorio;

        /// <summary>
        /// Inicializa los mocks y el repositorio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = ContextoMockFabrica.CrearContextoMock();
            _repositorio = new JugadorRepositorio(_contextoMock.Object);
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
                var repositorio = new JugadorRepositorio(null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoValido_CreaInstancia()
        {
            var contexto = ContextoMockFabrica.CrearContextoMock();

            var repositorio = new JugadorRepositorio(contexto.Object);

            Assert.IsNotNull(repositorio);
        }

        #endregion

        #region ExisteCorreo - Flujos Normales

        [TestMethod]
        public void Prueba_ExisteCorreo_CorreoExiste_RetornaTrue()
        {
            var jugadores = new List<Jugador>
            {
                EntidadesPruebaFabrica.CrearJugador(1)
            };
            jugadores[0].Correo = "existente@ejemplo.com";
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo("existente@ejemplo.com");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_CorreoNoExiste_RetornaFalse()
        {
            var jugadores = new List<Jugador>
            {
                EntidadesPruebaFabrica.CrearJugador(1)
            };
            jugadores[0].Correo = "otro@ejemplo.com";
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo("inexistente@ejemplo.com");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_ListaVacia_RetornaFalse()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo("cualquier@ejemplo.com");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_CorreoDiferenteDominio_RetornaFalse()
        {
            var jugadores = new List<Jugador>
            {
                EntidadesPruebaFabrica.CrearJugador(1)
            };
            jugadores[0].Correo = "usuario@ejemplo.com";
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo("usuario@otrodominio.com");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_MultiplesJugadoresCorreoDiferente_RetornaFalse()
        {
            var jugadores = new List<Jugador>
            {
                EntidadesPruebaFabrica.CrearJugador(1),
                EntidadesPruebaFabrica.CrearJugador(2),
                EntidadesPruebaFabrica.CrearJugador(3)
            };
            jugadores[0].Correo = "usuario1@ejemplo.com";
            jugadores[1].Correo = "usuario2@ejemplo.com";
            jugadores[2].Correo = "usuario3@ejemplo.com";
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo("usuariobuscado@ejemplo.com");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_MultiplesJugadoresCorreoExiste_RetornaTrue()
        {
            var jugadores = new List<Jugador>
            {
                EntidadesPruebaFabrica.CrearJugador(1),
                EntidadesPruebaFabrica.CrearJugador(2)
            };
            jugadores[0].Correo = "usuario1@ejemplo.com";
            jugadores[1].Correo = "usuariobuscado@ejemplo.com";
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo("usuariobuscado@ejemplo.com");

            Assert.IsTrue(resultado);
        }

        #endregion

        #region ExisteCorreo - Flujos Alternos

        [TestMethod]
        public void Prueba_ExisteCorreo_CorreoNulo_RetornaFalse()
        {
            var jugadores = new List<Jugador>
            {
                EntidadesPruebaFabrica.CrearJugador(1)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteCorreo_CorreoVacio_RetornaFalse()
        {
            var jugadores = new List<Jugador>
            {
                EntidadesPruebaFabrica.CrearJugador(1)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteCorreo("");

            Assert.IsFalse(resultado);
        }

        #endregion

        #region ExisteCorreo - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ExisteCorreo_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Jugador)
                .Throws(new DbUpdateException("Error de base de datos"));
            _repositorio = new JugadorRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ExisteCorreo("correo@ejemplo.com");
            });
        }

        #endregion

        #region CrearJugador - Flujos Normales

        [TestMethod]
        public void Prueba_CrearJugador_JugadorValido_RetornaJugadorCreado()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);
            var nuevoJugador = EntidadesPruebaFabrica.CrearJugador(1);
            nuevoJugador.Nombre = "NuevoJugador";

            var resultado = _repositorio.CrearJugador(nuevoJugador);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("NuevoJugador", resultado.Nombre);
        }

        [TestMethod]
        public void Prueba_CrearJugador_JugadorValido_LlamaSaveChanges()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);
            var nuevoJugador = EntidadesPruebaFabrica.CrearJugador(1);

            _repositorio.CrearJugador(nuevoJugador);

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearJugador_JugadorValido_AgregaAlDbSet()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);
            var nuevoJugador = EntidadesPruebaFabrica.CrearJugador(1);

            _repositorio.CrearJugador(nuevoJugador);

            Assert.AreEqual(1, jugadores.Count);
        }

        [TestMethod]
        public void Prueba_CrearJugador_ConDatosCompletos_RetornaConDatos()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);
            var nuevoJugador = new Jugador
            {
                idJugador = 0,
                Nombre = "Juan",
                Apellido = "Perez",
                Correo = "juan.perez@ejemplo.com",
                Id_Avatar = 5,
                Clasificacion_idClasificacion = 1
            };

            var resultado = _repositorio.CrearJugador(nuevoJugador);

            Assert.AreEqual("Juan", resultado.Nombre);
            Assert.AreEqual("Perez", resultado.Apellido);
            Assert.AreEqual("juan.perez@ejemplo.com", resultado.Correo);
            Assert.AreEqual(5, resultado.Id_Avatar);
        }

        [TestMethod]
        public void Prueba_CrearJugador_MultiplesJugadores_CadaUnoSeAgrega()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _repositorio = new JugadorRepositorio(_contextoMock.Object);
            var jugador1 = EntidadesPruebaFabrica.CrearJugador(1);
            var jugador2 = EntidadesPruebaFabrica.CrearJugador(2);

            _repositorio.CrearJugador(jugador1);
            _repositorio.CrearJugador(jugador2);

            Assert.AreEqual(2, jugadores.Count);
        }

        #endregion

        #region CrearJugador - Flujos de Excepcion

        [TestMethod]
        public void Prueba_CrearJugador_JugadorNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.CrearJugador(null);
            });
        }

        [TestMethod]
        public void Prueba_CrearJugador_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new DbUpdateException("Error al guardar"));
            _repositorio = new JugadorRepositorio(_contextoMock.Object);
            var nuevoJugador = EntidadesPruebaFabrica.CrearJugador(1);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.CrearJugador(nuevoJugador);
            });
        }

        [TestMethod]
        public void Prueba_CrearJugador_ExcepcionGeneral_LanzaBaseDatosExcepcion()
        {
            var jugadores = new List<Jugador>();
            _contextoMock = ContextoMockFabrica.CrearContextoConJugadores(jugadores);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new Exception("Error inesperado"));
            _repositorio = new JugadorRepositorio(_contextoMock.Object);
            var nuevoJugador = EntidadesPruebaFabrica.CrearJugador(1);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.CrearJugador(nuevoJugador);
            });
        }

        #endregion
    }
}
