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
    /// Contiene las pruebas unitarias para la clase UsuarioRepositorio.
    /// Verifica flujos normales, alternos y de excepcion.
    /// </summary>
    [TestClass]
    public class UsuarioRepositorioPruebas
    {
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private UsuarioRepositorio _repositorio;

        /// <summary>
        /// Inicializa los mocks y el repositorio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = ContextoMockFabrica.CrearContextoMock();
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);
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
                var repositorio = new UsuarioRepositorio(null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoValido_CreaInstancia()
        {
            var contexto = ContextoMockFabrica.CrearContextoMock();

            var repositorio = new UsuarioRepositorio(contexto.Object);

            Assert.IsNotNull(repositorio);
        }

        #endregion

        #region ExisteNombreUsuario - Flujos Normales

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_UsuarioExiste_RetornaTrue()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "UsuarioExistente")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario("UsuarioExistente");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_UsuarioNoExiste_RetornaFalse()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "OtroUsuario")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario("UsuarioInexistente");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_ListaVacia_RetornaFalse()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario("CualquierUsuario");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_ConEspaciosAlrededor_NormalizaYEncuentra()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "UsuarioTrim")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario("  UsuarioTrim  ");

            Assert.IsTrue(resultado);
        }

        #endregion

        #region ExisteNombreUsuario - Flujos Alternos

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_NombreNulo_RetornaFalse()
        {
            bool resultado = _repositorio.ExisteNombreUsuario(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_NombreVacio_RetornaFalse()
        {
            bool resultado = _repositorio.ExisteNombreUsuario("");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_SoloEspacios_RetornaFalse()
        {
            bool resultado = _repositorio.ExisteNombreUsuario("   ");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_CaseSensitive_DiferenciaMinusculas()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "UsuarioMayusculas")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteNombreUsuario("usuariomayusculas");

            Assert.IsFalse(resultado);
        }

        #endregion

        #region ExisteNombreUsuario - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ExisteNombreUsuario_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Usuario)
                .Throws(new DbUpdateException("Error de base de datos"));
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ExisteNombreUsuario("Usuario");
            });
        }

        #endregion

        #region CrearUsuario - Flujos Normales

        [TestMethod]
        public void Prueba_CrearUsuario_UsuarioValido_RetornaUsuarioCreado()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);
            var nuevoUsuario = EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "NuevoUsuario");

            var resultado = _repositorio.CrearUsuario(nuevoUsuario);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("NuevoUsuario", resultado.Nombre_Usuario);
        }

        [TestMethod]
        public void Prueba_CrearUsuario_UsuarioValido_LlamaSaveChanges()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);
            var nuevoUsuario = EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "NuevoUsuario");

            _repositorio.CrearUsuario(nuevoUsuario);

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearUsuario_UsuarioValido_AgregaAlDbSet()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);
            var nuevoUsuario = EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "NuevoUsuario");

            _repositorio.CrearUsuario(nuevoUsuario);

            Assert.AreEqual(1, usuarios.Count);
        }

        #endregion

        #region CrearUsuario - Flujos de Excepcion

        [TestMethod]
        public void Prueba_CrearUsuario_UsuarioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.CrearUsuario(null);
            });
        }

        [TestMethod]
        public void Prueba_CrearUsuario_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new DbUpdateException("Error al guardar"));
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);
            var nuevoUsuario = EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "NuevoUsuario");

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.CrearUsuario(nuevoUsuario);
            });
        }

        #endregion

        #region ObtenerPorNombreUsuario - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_UsuarioExiste_RetornaUsuario()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "UsuarioBuscado")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerPorNombreUsuario("UsuarioBuscado");

            Assert.IsNotNull(resultado);
            Assert.AreEqual("UsuarioBuscado", resultado.Nombre_Usuario);
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_ConEspacios_NormalizaNombre()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "UsuarioTrim")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerPorNombreUsuario("  UsuarioTrim  ");

            Assert.IsNotNull(resultado);
            Assert.AreEqual("UsuarioTrim", resultado.Nombre_Usuario);
        }

        #endregion

        #region ObtenerPorNombreUsuario - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_UsuarioNoExiste_LanzaExcepcion()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "OtroUsuario")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ObtenerPorNombreUsuario("UsuarioInexistente");
            });
        }

        #endregion

        #region ObtenerPorNombreUsuario - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_NombreNulo_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreUsuario(null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_NombreVacio_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreUsuario("");
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreUsuario_SoloEspacios_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreUsuario("   ");
            });
        }

        #endregion

        #region ObtenerPorCorreo - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_CorreoExiste_RetornaUsuario()
        {
            var clasificacion = EntidadesPruebaFabrica.CrearClasificacion(1);
            var jugador = EntidadesPruebaFabrica.CrearJugador(1, clasificacion);
            jugador.Correo = "correo@ejemplo.com";
            var usuario = new Usuario
            {
                idUsuario = 1,
                Nombre_Usuario = "UsuarioConCorreo",
                Contrasena = "hash123",
                Jugador_idJugador = 1,
                Jugador = jugador
            };
            var usuarios = new List<Usuario> { usuario };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerPorCorreo("correo@ejemplo.com");

            Assert.IsNotNull(resultado);
            Assert.AreEqual("correo@ejemplo.com", resultado.Jugador.Correo);
        }

        #endregion

        #region ObtenerPorCorreo - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_CorreoNoExiste_LanzaExcepcion()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ObtenerPorCorreo("inexistente@ejemplo.com");
            });
        }

        #endregion

        #region ObtenerPorCorreo - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_CorreoNulo_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorCorreo(null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_CorreoVacio_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorCorreo("");
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorCorreo_SoloEspacios_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorCorreo("   ");
            });
        }

        #endregion

        #region ObtenerPorIdConRedesSociales - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSociales_IdValido_RetornaUsuario()
        {
            var usuario = EntidadesPruebaFabrica.CrearUsuario(1, "UsuarioConRedes");
            var usuarios = new List<Usuario> { usuario };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerPorIdConRedesSociales(1);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.idUsuario);
        }

        #endregion

        #region ObtenerPorIdConRedesSociales - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSociales_IdNoExiste_LanzaExcepcion()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuario(1, "Usuario1")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ObtenerPorIdConRedesSociales(999);
            });
        }

        #endregion

        #region ObtenerPorIdConRedesSociales - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSociales_IdCero_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerPorIdConRedesSociales(0);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorIdConRedesSociales_IdNegativo_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerPorIdConRedesSociales(-1);
            });
        }

        #endregion

        #region ActualizarContrasena - Flujos Normales

        [TestMethod]
        public void Prueba_ActualizarContrasena_UsuarioExiste_ActualizaContrasena()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "UsuarioActualizar")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);
            string nuevaContrasena = "nuevaContrasenaHash";

            _repositorio.ActualizarContrasena(1, nuevaContrasena);

            Assert.AreEqual(nuevaContrasena, usuarios[0].Contrasena);
        }

        [TestMethod]
        public void Prueba_ActualizarContrasena_UsuarioExiste_LlamaSaveChanges()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "UsuarioActualizar")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            _repositorio.ActualizarContrasena(1, "nuevaContrasena");

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        #endregion

        #region ActualizarContrasena - Flujos Alternos

        [TestMethod]
        public void Prueba_ActualizarContrasena_UsuarioNoExiste_NoLanzaExcepcion()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            _repositorio.ActualizarContrasena(999, "nuevaContrasena");

            _contextoMock.Verify(c => c.SaveChanges(), Times.Never);
        }

        #endregion

        #region ActualizarContrasena - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ActualizarContrasena_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "UsuarioError")
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new DbUpdateException("Error al actualizar"));
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ActualizarContrasena(1, "nuevaContrasena");
            });
        }

        #endregion

        #region ObtenerPorNombreConJugador - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_UsuarioExiste_RetornaUsuarioConJugador()
        {
            var usuario = EntidadesPruebaFabrica.CrearUsuario(1, "UsuarioConJugador");
            var usuarios = new List<Usuario> { usuario };
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerPorNombreConJugador("UsuarioConJugador");

            Assert.IsNotNull(resultado);
            Assert.IsNotNull(resultado.Jugador);
        }

        #endregion

        #region ObtenerPorNombreConJugador - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_UsuarioNoExiste_LanzaExcepcion()
        {
            var usuarios = new List<Usuario>();
            _contextoMock = ContextoMockFabrica.CrearContextoConUsuarios(usuarios);
            _repositorio = new UsuarioRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ObtenerPorNombreConJugador("UsuarioInexistente");
            });
        }

        #endregion

        #region ObtenerPorNombreConJugador - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_NombreNulo_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreConJugador(null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_NombreVacio_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreConJugador("");
            });
        }

        [TestMethod]
        public void Prueba_ObtenerPorNombreConJugador_SoloEspacios_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _repositorio.ObtenerPorNombreConJugador("   ");
            });
        }

        #endregion
    }
}
