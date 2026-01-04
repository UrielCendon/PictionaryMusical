using System;
using System.ServiceModel;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class OperacionAmistadServicioPruebas
    {
        private const int IdUsuarioEmisor = 1;
        private const int IdUsuarioReceptor = 2;
        private const string NombreUsuarioEmisor = "EmisorTest";
        private const string NombreUsuarioReceptor = "ReceptorTest";

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IAmistadServicio> _amistadServicioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private OperacionAmistadServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _amistadServicioMock = new Mock<IAmistadServicio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();

            _contextoFactoriaMock
                .Setup(f => f.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(f => f.CrearUsuarioRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _servicio = new OperacionAmistadServicio(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object,
                _amistadServicioMock.Object);
        }

        [TestCleanup]
        public void Limpiar()
        {
            _servicio = null;
            _contextoFactoriaMock = null;
            _repositorioFactoriaMock = null;
            _amistadServicioMock = null;
        }

        #region Constructor

        [TestMethod]
        public void Prueba_ConstructorContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new OperacionAmistadServicio(
                    null,
                    _repositorioFactoriaMock.Object,
                    _amistadServicioMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_ConstructorRepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new OperacionAmistadServicio(
                    _contextoFactoriaMock.Object,
                    null,
                    _amistadServicioMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_ConstructorAmistadServicioNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new OperacionAmistadServicio(
                    _contextoFactoriaMock.Object,
                    _repositorioFactoriaMock.Object,
                    null);
            });
        }

        #endregion

        #region ObtenerDatosUsuarioSuscripcion

        [TestMethod]
        public void Prueba_ObtenerDatosUsuarioSuscripcionUsuarioNoExiste_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(() =>
            {
                _servicio.ObtenerDatosUsuarioSuscripcion(NombreUsuarioEmisor);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerDatosUsuarioSuscripcionValido_RetornaDatosCorrectos()
        {
            var usuario = new Usuario
            {
                idUsuario = IdUsuarioEmisor,
                Nombre_Usuario = NombreUsuarioEmisor
            };

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(usuario);

            DatosSuscripcionUsuario resultado = 
                _servicio.ObtenerDatosUsuarioSuscripcion(NombreUsuarioEmisor);

            Assert.AreEqual(IdUsuarioEmisor, resultado.IdUsuario);
        }

        #endregion

        #region EjecutarCreacionSolicitud

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitudEmisorNoExiste_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(() =>
            {
                _servicio.EjecutarCreacionSolicitud(NombreUsuarioEmisor, NombreUsuarioReceptor);
            });
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitudReceptorNoExiste_LanzaFaultException()
        {
            var emisor = new Usuario
            {
                idUsuario = IdUsuarioEmisor,
                Nombre_Usuario = NombreUsuarioEmisor
            };

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(emisor);
            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioReceptor))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(() =>
            {
                _servicio.EjecutarCreacionSolicitud(NombreUsuarioEmisor, NombreUsuarioReceptor);
            });
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitudValida_RetornaResultadoConUsuarios()
        {
            var emisor = new Usuario
            {
                idUsuario = IdUsuarioEmisor,
                Nombre_Usuario = NombreUsuarioEmisor
            };
            var receptor = new Usuario
            {
                idUsuario = IdUsuarioReceptor,
                Nombre_Usuario = NombreUsuarioReceptor
            };

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(emisor);
            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioReceptor))
                .Returns(receptor);

            ResultadoCreacionSolicitud resultado = 
                _servicio.EjecutarCreacionSolicitud(NombreUsuarioEmisor, NombreUsuarioReceptor);

            Assert.AreEqual(IdUsuarioEmisor, resultado.Emisor.idUsuario);
        }

        #endregion

        #region EjecutarAceptacionSolicitud

        [TestMethod]
        public void Prueba_EjecutarAceptacionSolicitudUsuariosValidos_RetornaNombresNormalizados()
        {
            var emisor = new Usuario
            {
                idUsuario = IdUsuarioEmisor,
                Nombre_Usuario = NombreUsuarioEmisor
            };
            var receptor = new Usuario
            {
                idUsuario = IdUsuarioReceptor,
                Nombre_Usuario = NombreUsuarioReceptor
            };

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(emisor);
            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioReceptor))
                .Returns(receptor);

            ResultadoAceptacionSolicitud resultado = 
                _servicio.EjecutarAceptacionSolicitud(NombreUsuarioEmisor, NombreUsuarioReceptor);

            Assert.AreEqual(NombreUsuarioEmisor, resultado.NombreNormalizadoEmisor);
        }

        #endregion

        #region EjecutarEliminacion

        [TestMethod]
        public void Prueba_EjecutarEliminacionUsuariosValidos_RetornaResultadoConRelacion()
        {
            var usuario1 = new Usuario
            {
                idUsuario = IdUsuarioEmisor,
                Nombre_Usuario = NombreUsuarioEmisor
            };
            var usuario2 = new Usuario
            {
                idUsuario = IdUsuarioReceptor,
                Nombre_Usuario = NombreUsuarioReceptor
            };
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor
            };

            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioEmisor))
                .Returns(usuario1);
            _usuarioRepositorioMock
                .Setup(r => r.ObtenerPorNombreUsuario(NombreUsuarioReceptor))
                .Returns(usuario2);
            _amistadServicioMock
                .Setup(s => s.EliminarAmistad(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            ResultadoEliminacionAmistad resultado = 
                _servicio.EjecutarEliminacion(NombreUsuarioEmisor, NombreUsuarioReceptor);

            Assert.AreEqual(IdUsuarioEmisor, resultado.Relacion.UsuarioEmisor);
        }

        #endregion
    }
}
