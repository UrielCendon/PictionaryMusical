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
    public class UsuarioRepositorioPruebas
    {
        private const int IdUsuario = 1;
        private const int IdCero = 0;
        private const int IdNegativo = -1;
        private const string NombreUsuarioPrueba = "TestUser";
        private const string NombreUsuarioAlterno = "OtroUser";
        private const string EspaciosEnBlanco = "   ";
        private const string CadenaVacia = "";

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Usuario>> _usuarioDbSetMock;
        private UsuarioRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _usuarioDbSetMock = new Mock<DbSet<Usuario>>();
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
                new UsuarioRepositorio(null);
            });
        }

        #endregion

        #region ExisteNombreUsuario

        [TestMethod]
        public void Prueba_ExisteNombreUsuarioNulo_RetornaFalse()
        {
            var usuarios = new List<Usuario>().AsQueryable();
            ConfigurarDbSetMock(_usuarioDbSetMock, usuarios);
            _contextoMock.Setup(c => c.Usuario).Returns(_usuarioDbSetMock.Object);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuarioVacio_RetornaFalse()
        {
            var usuarios = new List<Usuario>().AsQueryable();
            ConfigurarDbSetMock(_usuarioDbSetMock, usuarios);
            _contextoMock.Setup(c => c.Usuario).Returns(_usuarioDbSetMock.Object);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario(EspaciosEnBlanco);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuarioExistente_RetornaTrue()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { idUsuario = IdUsuario, Nombre_Usuario = NombreUsuarioPrueba }
            }.AsQueryable();

            ConfigurarDbSetMock(_usuarioDbSetMock, usuarios);
            _contextoMock.Setup(c => c.Usuario).Returns(_usuarioDbSetMock.Object);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario(NombreUsuarioPrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuarioInexistente_RetornaFalse()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { idUsuario = IdUsuario, Nombre_Usuario = NombreUsuarioAlterno }
            }.AsQueryable();

            ConfigurarDbSetMock(_usuarioDbSetMock, usuarios);
            _contextoMock.Setup(c => c.Usuario).Returns(_usuarioDbSetMock.Object);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario(NombreUsuarioPrueba);

            Assert.IsFalse(resultado);
        }

        #endregion

        #region CrearUsuario

        [TestMethod]
        public void Prueba_CrearUsuarioNulo_LanzaArgumentNullException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.CrearUsuario(null);
            });
        }

        #endregion

        #region ObtenerPorNombreUsuario

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuarioNulo_LanzaArgumentException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreUsuario(null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuarioVacio_LanzaArgumentException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreUsuario(EspaciosEnBlanco);
            });
        }

        #endregion

        #region ObtenerPorCorreo

        [TestMethod]
        public void Prueba_ObtenerPorCorreoNulo_LanzaArgumentException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorCorreo(null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreoVacio_LanzaArgumentException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorCorreo(CadenaVacia);
            });
        }

        #endregion

        #region ObtenerPorCorreoAsync

        [TestMethod]
        public void Prueba_ObtenerPorCorreoAsyncNulo_LanzaArgumentException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await _repositorio.ObtenerPorCorreoAsync(null);
            });
        }

        #endregion

        #region ObtenerPorIdConRedesSociales

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSocialesIdCero_LanzaArgumentOutOfRangeException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerPorIdConRedesSociales(IdCero);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSocialesIdNegativo_LanzaArgumentOutOfRangeException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerPorIdConRedesSociales(IdNegativo);
            });
        }

        #endregion

        #region ObtenerPorNombreConJugador

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugadorNulo_LanzaArgumentException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreConJugador(null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugadorVacio_LanzaArgumentException()
        {
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreConJugador(EspaciosEnBlanco);
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
