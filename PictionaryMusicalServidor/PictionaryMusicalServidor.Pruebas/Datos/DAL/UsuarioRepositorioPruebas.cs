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
    public class UsuarioRepositorioPruebas
    {
        private const int IdUsuarioValido = 10;
        private const int IdUsuarioInexistente = 99;
        private const int IdUsuarioInvalido = 0;
        private const string NombreUsuarioValido = "UsuarioPrueba";
        private const string NombreUsuarioDiferente = "OtroUsuario";
        private const string NombreUsuarioEspacios = " UsuarioPrueba ";
        private const string NombreUsuarioVacio = "";
        private const string CorreoValido = "correo@prueba.com";
        private const string CorreoInexistente = "inexistente@prueba.com";
        private const string ContrasenaOriginal = "HashOriginal";
        private const string ContrasenaNueva = "HashNuevo";

        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<DbSet<Usuario>> _usuarioDbSetMock;
        private UsuarioRepositorio _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _usuarioDbSetMock = CrearDbSetMock(new List<Usuario>());

            _contextoMock
                .Setup(contexto => contexto.Usuario)
                .Returns(_usuarioDbSetMock.Object);

            _repositorio = new UsuarioRepositorio(_contextoMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new UsuarioRepositorio(null));
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_NombreNuloRetornaFalse()
        {
            bool resultado = _repositorio.ExisteNombreUsuario(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_UsuarioExisteRetornaTrue()
        {
            var datos = new List<Usuario>
            {
                new Usuario { Nombre_Usuario = NombreUsuarioValido }
            }.AsQueryable();

            ConfigurarDbSet(_usuarioDbSetMock, datos);

            bool resultado = _repositorio.ExisteNombreUsuario(NombreUsuarioEspacios);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_UsuarioNoExisteRetornaFalse()
        {
            var datos = new List<Usuario>
            {
                new Usuario { Nombre_Usuario = NombreUsuarioValido }
            }.AsQueryable();

            ConfigurarDbSet(_usuarioDbSetMock, datos);

            bool resultado = _repositorio.ExisteNombreUsuario(NombreUsuarioDiferente);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_ErrorBaseDatosLanzaExcepcionPersonalizada()
        {
            _usuarioDbSetMock.As<IQueryable<Usuario>>()
                .Setup(consulta => consulta.Provider)
                .Throws(new EntityException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ExisteNombreUsuario(NombreUsuarioValido));
        }

        [TestMethod]
        public void Prueba_CrearUsuario_UsuarioNuloLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _repositorio.CrearUsuario(null));
        }

        [TestMethod]
        public void Prueba_CrearUsuario_UsuarioValidoGuardaCambios()
        {
            var nuevoUsuario = new Usuario { Nombre_Usuario = NombreUsuarioValido };

            _usuarioDbSetMock
                .Setup(conjunto => conjunto.Add(It.IsAny<Usuario>()))
                .Returns<Usuario>(entidad => entidad);

            var resultado = _repositorio.CrearUsuario(nuevoUsuario);

            _usuarioDbSetMock.Verify(
                conjuntoUsuarios => conjuntoUsuarios.Add(It.IsAny<Usuario>()),
                Times.Once);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
            Assert.AreEqual(NombreUsuarioValido, resultado.Nombre_Usuario);
        }

        [TestMethod]
        public void Prueba_CrearUsuario_ErrorGuardarLanzaExcepcionPersonalizada()
        {
            var nuevoUsuario = new Usuario { Nombre_Usuario = NombreUsuarioValido };

            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new DbUpdateException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.CrearUsuario(nuevoUsuario));
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_NombreVacioLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _repositorio.ObtenerPorNombreUsuario(NombreUsuarioVacio));
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_UsuarioExisteRetornaEntidad()
        {
            var datos = new List<Usuario>
            {
                new Usuario { Nombre_Usuario = NombreUsuarioValido }
            }.AsQueryable();

            ConfigurarDbSet(_usuarioDbSetMock, datos);

            var resultado = _repositorio.ObtenerPorNombreUsuario(NombreUsuarioEspacios);

            Assert.AreEqual(NombreUsuarioValido, resultado.Nombre_Usuario);
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_UsuarioNoExisteLanzaExcepcion()
        {
            var datos = new List<Usuario>().AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ObtenerPorNombreUsuario(NombreUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_CorreoVacioLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _repositorio.ObtenerPorCorreo(NombreUsuarioVacio));
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_UsuarioEncontradoRetornaEntidad()
        {
            var usuario = new Usuario
            {
                Jugador = new Jugador { Correo = CorreoValido }
            };
            var datos = new List<Usuario> { usuario }.AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            var resultado = _repositorio.ObtenerPorCorreo(CorreoValido);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(CorreoValido, resultado.Jugador.Correo);
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_UsuarioNoEncontradoLanzaExcepcion()
        {
            var usuario = new Usuario
            {
                Jugador = new Jugador { Correo = CorreoValido }
            };
            var datos = new List<Usuario> { usuario }.AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ObtenerPorCorreo(CorreoInexistente));
        }

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSociales_IdInvalidoLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _repositorio.ObtenerPorIdConRedesSociales(IdUsuarioInvalido));
        }

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSociales_UsuarioExisteRetornaEntidad()
        {
            var datos = new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = IdUsuarioValido,
                    Jugador = new Jugador { RedSocial = new List<RedSocial>() }
                }
            }.AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            var resultado = _repositorio.ObtenerPorIdConRedesSociales(IdUsuarioValido);

            Assert.AreEqual(IdUsuarioValido, resultado.idUsuario);
            Assert.IsNotNull(resultado.Jugador.RedSocial);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_UsuarioExisteActualizaYGuarda()
        {
            var usuario = new Usuario
            {
                idUsuario = IdUsuarioValido,
                Contrasena = ContrasenaOriginal
            };
            var datos = new List<Usuario> { usuario }.AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            _repositorio.ActualizarContrasena(IdUsuarioValido, ContrasenaNueva);

            Assert.AreEqual(ContrasenaNueva, usuario.Contrasena);
            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_UsuarioNoExisteNoGuardaCambios()
        {
            var datos = new List<Usuario>().AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            _repositorio.ActualizarContrasena(IdUsuarioInexistente, ContrasenaNueva);

            _contextoMock.Verify(contexto => contexto.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_ErrorBaseDatosLanzaExcepcionPersonalizada()
        {
            var usuario = new Usuario { idUsuario = IdUsuarioValido };
            var datos = new List<Usuario> { usuario }.AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            _contextoMock.Setup(contexto => contexto.SaveChanges()).Throws(new DbUpdateException());

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ActualizarContrasena(IdUsuarioValido, ContrasenaNueva));
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_NombreVacioLanzaExcepcion()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _repositorio.ObtenerPorNombreConJugador(string.Empty));
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_UsuarioExisteRetornaEntidad()
        {
            var datos = new List<Usuario>
            {
                new Usuario
                {
                    Nombre_Usuario = NombreUsuarioValido,
                    Jugador = new Jugador()
                }
            }.AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            var resultado = _repositorio.ObtenerPorNombreConJugador(NombreUsuarioValido);

            Assert.AreEqual(NombreUsuarioValido, resultado.Nombre_Usuario);
            Assert.IsNotNull(resultado.Jugador);
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_UsuarioNoExisteLanzaExcepcion()
        {
            var datos = new List<Usuario>().AsQueryable();
            ConfigurarDbSet(_usuarioDbSetMock, datos);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
                _repositorio.ObtenerPorNombreConJugador(NombreUsuarioValido));
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