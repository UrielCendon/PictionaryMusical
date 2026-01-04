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
    /// Contiene las pruebas unitarias para la clase AmigoRepositorio.
    /// Verifica flujos normales, alternos y de excepcion para la gestion de amistades.
    /// </summary>
    [TestClass]
    public class AmigoRepositorioPruebas
    {
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private AmigoRepositorio _repositorio;

        /// <summary>
        /// Inicializa los mocks y el repositorio antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _contextoMock = ContextoMockFabrica.CrearContextoMock();
            _repositorio = new AmigoRepositorio(_contextoMock.Object);
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
                var repositorio = new AmigoRepositorio(null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoValido_CreaInstancia()
        {
            var contexto = ContextoMockFabrica.CrearContextoMock();

            var repositorio = new AmigoRepositorio(contexto.Object);

            Assert.IsNotNull(repositorio);
        }

        #endregion

        #region ExisteRelacion - Flujos Normales

        [TestMethod]
        public void Prueba_ExisteRelacion_RelacionExisteComoEmisor_RetornaTrue()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteRelacion(1, 2);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_RelacionExisteComoReceptor_RetornaTrue()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(2, 1, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteRelacion(1, 2);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_RelacionNoExiste_RetornaFalse()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(3, 4, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteRelacion(1, 2);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_ListaVacia_RetornaFalse()
        {
            var amigos = new List<Amigo>();
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteRelacion(1, 2);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ExisteRelacion_RelacionAceptada_RetornaTrue()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, true)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            bool resultado = _repositorio.ExisteRelacion(1, 2);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region ExisteRelacion - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ExisteRelacion_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            _contextoMock.Setup(c => c.Amigo)
                .Throws(new DbUpdateException("Error de base de datos"));
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ExisteRelacion(1, 2);
            });
        }

        #endregion

        #region CrearSolicitud - Flujos Normales

        [TestMethod]
        public void Prueba_CrearSolicitud_DatosValidos_RetornaSolicitudCreada()
        {
            var amigos = new List<Amigo>();
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.CrearSolicitud(1, 2);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.UsuarioEmisor);
            Assert.AreEqual(2, resultado.UsuarioReceptor);
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_DatosValidos_EstadoEsPendiente()
        {
            var amigos = new List<Amigo>();
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.CrearSolicitud(1, 2);

            Assert.IsFalse(resultado.Estado);
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_DatosValidos_LlamaSaveChanges()
        {
            var amigos = new List<Amigo>();
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            _repositorio.CrearSolicitud(1, 2);

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_DatosValidos_AgregaAlDbSet()
        {
            var amigos = new List<Amigo>();
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            _repositorio.CrearSolicitud(1, 2);

            Assert.AreEqual(1, amigos.Count);
        }

        #endregion

        #region ObtenerRelacion - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerRelacion_RelacionExisteComoEmisor_RetornaRelacion()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerRelacion(1, 2);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.UsuarioEmisor);
        }

        [TestMethod]
        public void Prueba_ObtenerRelacion_RelacionExisteComoReceptor_RetornaRelacion()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(2, 1, true)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerRelacion(1, 2);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.UsuarioEmisor);
        }

        #endregion

        #region ObtenerRelacion - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerRelacion_RelacionNoExiste_LanzaBaseDatosExcepcion()
        {
            var amigos = new List<Amigo>();
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ObtenerRelacion(1, 2);
            });
        }

        #endregion

        #region ObtenerSolicitudesPendientes - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_TieneSolicitudes_RetornaLista()
        {
            var usuarioEmisor = EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "Emisor");
            var usuarioReceptor = EntidadesPruebaFabrica.CrearUsuarioSinJugador(2, "Receptor");
            var amigos = new List<Amigo>
            {
                new Amigo
                {
                    UsuarioEmisor = 1,
                    UsuarioReceptor = 2,
                    Estado = false,
                    Usuario = usuarioEmisor,
                    Usuario1 = usuarioReceptor
                }
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerSolicitudesPendientes(2);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_SinSolicitudes_RetornaListaVacia()
        {
            var amigos = new List<Amigo>();
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerSolicitudesPendientes(1);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_SoloAceptadas_RetornaListaVacia()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, true)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerSolicitudesPendientes(2);

            Assert.AreEqual(0, resultado.Count);
        }

        #endregion

        #region ObtenerSolicitudesPendientes - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_IdCero_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerSolicitudesPendientes(0);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientes_IdNegativo_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerSolicitudesPendientes(-1);
            });
        }

        #endregion

        #region ActualizarEstado - Flujos Normales

        [TestMethod]
        public void Prueba_ActualizarEstado_RelacionValida_ActualizaEstado()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            _repositorio.ActualizarEstado(amigos[0], true);

            Assert.IsTrue(amigos[0].Estado);
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_RelacionValida_LlamaSaveChanges()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            _repositorio.ActualizarEstado(amigos[0], true);

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_CambiarAFalse_ActualizaCorrectamente()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, true)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            _repositorio.ActualizarEstado(amigos[0], false);

            Assert.IsFalse(amigos[0].Estado);
        }

        #endregion

        #region ActualizarEstado - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ActualizarEstado_RelacionNula_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.ActualizarEstado(null, true);
            });
        }

        [TestMethod]
        public void Prueba_ActualizarEstado_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new DbUpdateException("Error al actualizar"));
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.ActualizarEstado(amigos[0], true);
            });
        }

        #endregion

        #region EliminarRelacion - Flujos Normales

        [TestMethod]
        public void Prueba_EliminarRelacion_RelacionValida_EliminaDelDbSet()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            _repositorio.EliminarRelacion(amigos[0]);

            Assert.AreEqual(0, amigos.Count);
        }

        [TestMethod]
        public void Prueba_EliminarRelacion_RelacionValida_LlamaSaveChanges()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            _repositorio.EliminarRelacion(amigos[0]);

            _contextoMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        #endregion

        #region EliminarRelacion - Flujos de Excepcion

        [TestMethod]
        public void Prueba_EliminarRelacion_RelacionNula_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _repositorio.EliminarRelacion(null);
            });
        }

        [TestMethod]
        public void Prueba_EliminarRelacion_DbUpdateException_LanzaBaseDatosExcepcion()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _contextoMock.Setup(c => c.SaveChanges())
                .Throws(new DbUpdateException("Error al eliminar"));
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            Assert.ThrowsException<BaseDatosExcepcion>(() =>
            {
                _repositorio.EliminarRelacion(amigos[0]);
            });
        }

        #endregion

        #region ObtenerAmigos - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerAmigos_TieneAmigos_RetornaListaUsuarios()
        {
            var usuarios = new List<Usuario>
            {
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(1, "Usuario1"),
                EntidadesPruebaFabrica.CrearUsuarioSinJugador(2, "Usuario2")
            };
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, true)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            var usuarioDbSetMock = DbSetMockExtensiones.CrearDbSetMock(usuarios);
            _contextoMock.Setup(c => c.Usuario).Returns(usuarioDbSetMock.Object);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerAmigos(1);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_SinAmigos_RetornaListaVacia()
        {
            var amigos = new List<Amigo>();
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerAmigos(1);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_SoloPendientes_RetornaListaVacia()
        {
            var amigos = new List<Amigo>
            {
                EntidadesPruebaFabrica.CrearRelacionAmistad(1, 2, false)
            };
            _contextoMock = ContextoMockFabrica.CrearContextoConAmigos(amigos);
            _repositorio = new AmigoRepositorio(_contextoMock.Object);

            var resultado = _repositorio.ObtenerAmigos(1);

            Assert.AreEqual(0, resultado.Count);
        }

        #endregion

        #region ObtenerAmigos - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerAmigos_IdCero_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerAmigos(0);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerAmigos_IdNegativo_LanzaArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _repositorio.ObtenerAmigos(-5);
            });
        }

        #endregion
    }
}
