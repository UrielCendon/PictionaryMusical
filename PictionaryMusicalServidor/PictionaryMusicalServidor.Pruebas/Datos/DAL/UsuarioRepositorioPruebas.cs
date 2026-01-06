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
    public class UsuarioRepositorioPruebas
    {
        private const string NombreUsuarioPrueba = "UsuarioPrueba";
        private const string NombreUsuarioInexistente = "UsuarioInexistente";
        private const string ContrasenaPrueba = "Contrasena123!";
        private const string NuevaContrasenaPrueba = "NuevaContrasena456!";
        private const int IdUsuarioPrueba = 1;
        private const int IdUsuarioInvalido = 0;
        private const int IdUsuarioNegativo = -1;
        private const int IdJugadorPrueba = 1;

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new UsuarioRepositorio(null));
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_RetornaVerdaderoNombreExistente()
        {
            var datosUsuarios = new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = IdUsuarioPrueba,
                    Nombre_Usuario = NombreUsuarioPrueba,
                    Contrasena = ContrasenaPrueba,
                    Jugador_idJugador = IdJugadorPrueba
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosUsuarios);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Usuario).Returns(mockDbSet.Object);

            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteNombreUsuario(NombreUsuarioPrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_RetornaFalsoNombreInexistente()
        {
            var datosUsuarios = new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = IdUsuarioPrueba,
                    Nombre_Usuario = NombreUsuarioPrueba,
                    Contrasena = ContrasenaPrueba,
                    Jugador_idJugador = IdJugadorPrueba
                }
            }.AsQueryable();

            var mockDbSet = CrearMockDbSet(datosUsuarios);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Usuario).Returns(mockDbSet.Object);

            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteNombreUsuario(NombreUsuarioInexistente);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_RetornaFalsoNombreNulo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteNombreUsuario(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_RetornaFalsoNombreVacio()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteNombreUsuario(string.Empty);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_RetornaFalsoNombreSoloEspacios()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            bool resultado = repositorio.ExisteNombreUsuario("   ");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_CrearUsuario_LanzaExcepcionUsuarioNulo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => repositorio.CrearUsuario(null));
        }

        [TestMethod]
        public void Prueba_CrearUsuario_AgregaUsuarioAlContexto()
        {
            var usuarioNuevo = new Usuario
            {
                Nombre_Usuario = NombreUsuarioPrueba,
                Contrasena = ContrasenaPrueba,
                Jugador_idJugador = IdJugadorPrueba
            };

            var listaUsuarios = new List<Usuario>();
            var mockDbSet = CrearMockDbSetConAdd(listaUsuarios);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Usuario).Returns(mockDbSet.Object);

            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            repositorio.CrearUsuario(usuarioNuevo);

            mockDbSet.Verify(
                dbSet => dbSet.Add(It.IsAny<Usuario>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearUsuario_GuardaCambiosEnContexto()
        {
            var usuarioNuevo = new Usuario
            {
                Nombre_Usuario = NombreUsuarioPrueba,
                Contrasena = ContrasenaPrueba,
                Jugador_idJugador = IdJugadorPrueba
            };

            var listaUsuarios = new List<Usuario>();
            var mockDbSet = CrearMockDbSetConAdd(listaUsuarios);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Usuario).Returns(mockDbSet.Object);

            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            repositorio.CrearUsuario(usuarioNuevo);

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearUsuario_RetornaUsuarioCreado()
        {
            var usuarioNuevo = new Usuario
            {
                Nombre_Usuario = NombreUsuarioPrueba,
                Contrasena = ContrasenaPrueba,
                Jugador_idJugador = IdJugadorPrueba
            };

            var mockDbSet = new Mock<DbSet<Usuario>>();
            mockDbSet.Setup(dbSet => dbSet.Add(It.IsAny<Usuario>()))
                .Returns<Usuario>(usuario => usuario);

            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Usuario).Returns(mockDbSet.Object);

            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            var resultado = repositorio.CrearUsuario(usuarioNuevo);

            Assert.AreEqual(NombreUsuarioPrueba, resultado.Nombre_Usuario);
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_LanzaExcepcionNombreNulo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorNombreUsuario(null));
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_LanzaExcepcionNombreVacio()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorNombreUsuario(string.Empty));
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_LanzaExcepcionNombreSoloEspacios()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorNombreUsuario("   "));
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_LanzaExcepcionCorreoNulo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorCorreo(null));
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_LanzaExcepcionCorreoVacio()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorCorreo(string.Empty));
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_LanzaExcepcionCorreoSoloEspacios()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorCorreo("   "));
        }

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSociales_LanzaExcepcionIdInvalido()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioInvalido));
        }

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSociales_LanzaExcepcionIdNegativo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => repositorio.ObtenerPorIdConRedesSociales(IdUsuarioNegativo));
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_GuardaCambiosEnContexto()
        {
            var usuario = new Usuario
            {
                idUsuario = IdUsuarioPrueba,
                Nombre_Usuario = NombreUsuarioPrueba,
                Contrasena = ContrasenaPrueba,
                Jugador_idJugador = IdJugadorPrueba
            };

            var datosUsuarios = new List<Usuario> { usuario }.AsQueryable();
            var mockDbSet = CrearMockDbSet(datosUsuarios);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Usuario).Returns(mockDbSet.Object);

            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            repositorio.ActualizarContrasena(IdUsuarioPrueba, NuevaContrasenaPrueba);

            mockContexto.Verify(contexto => contexto.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_CambiaContrasenaUsuario()
        {
            var usuario = new Usuario
            {
                idUsuario = IdUsuarioPrueba,
                Nombre_Usuario = NombreUsuarioPrueba,
                Contrasena = ContrasenaPrueba,
                Jugador_idJugador = IdJugadorPrueba
            };

            var datosUsuarios = new List<Usuario> { usuario }.AsQueryable();
            var mockDbSet = CrearMockDbSet(datosUsuarios);
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            mockContexto.Setup(contexto => contexto.Usuario).Returns(mockDbSet.Object);

            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            repositorio.ActualizarContrasena(IdUsuarioPrueba, NuevaContrasenaPrueba);

            Assert.AreEqual(NuevaContrasenaPrueba, usuario.Contrasena);
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_LanzaExcepcionNombreNulo()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorNombreConJugador(null));
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_LanzaExcepcionNombreVacio()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorNombreConJugador(string.Empty));
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_LanzaExcepcionNombreSoloEspacios()
        {
            var mockContexto = new Mock<BaseDatosPruebaEntities>();
            var repositorio = new UsuarioRepositorio(mockContexto.Object);

            Assert.ThrowsException<ArgumentException>(
                () => repositorio.ObtenerPorNombreConJugador("   "));
        }

        private static Mock<DbSet<Usuario>> CrearMockDbSet(IQueryable<Usuario> datos)
        {
            var mockDbSet = new Mock<DbSet<Usuario>>();
            mockDbSet.As<IQueryable<Usuario>>()
                .Setup(dbSet => dbSet.Provider)
                .Returns(datos.Provider);
            mockDbSet.As<IQueryable<Usuario>>()
                .Setup(dbSet => dbSet.Expression)
                .Returns(datos.Expression);
            mockDbSet.As<IQueryable<Usuario>>()
                .Setup(dbSet => dbSet.ElementType)
                .Returns(datos.ElementType);
            mockDbSet.As<IQueryable<Usuario>>()
                .Setup(dbSet => dbSet.GetEnumerator())
                .Returns(datos.GetEnumerator());
            return mockDbSet;
        }

        private static Mock<DbSet<Usuario>> CrearMockDbSetConAdd(List<Usuario> listaUsuarios)
        {
            var datos = listaUsuarios.AsQueryable();
            var mockDbSet = CrearMockDbSet(datos);
            mockDbSet.Setup(dbSet => dbSet.Add(It.IsAny<Usuario>()))
                .Callback<Usuario>(usuario => listaUsuarios.Add(usuario))
                .Returns<Usuario>(usuario => usuario);
            return mockDbSet;
        }
    }
}
