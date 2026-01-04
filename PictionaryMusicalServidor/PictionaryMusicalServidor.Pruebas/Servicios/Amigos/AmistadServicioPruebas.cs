using System;
using System.Collections.Generic;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class AmistadServicioPruebas
    {
        private const int IdUsuarioEmisor = 1;
        private const int IdUsuarioReceptor = 2;
        private const string NombreUsuarioEmisor = "EmisorTest";
        private const string NombreUsuarioReceptor = "ReceptorTest";

        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<IAmigoRepositorio> _amigoRepositorioMock;
        private AmistadServicio _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _amigoRepositorioMock = new Mock<IAmigoRepositorio>();

            _contextoFactoriaMock
                .Setup(f => f.CrearContexto())
                .Returns(_contextoMock.Object);

            _repositorioFactoriaMock
                .Setup(f => f.CrearAmigoRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_amigoRepositorioMock.Object);

            _servicio = new AmistadServicio(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object);
        }

        [TestCleanup]
        public void Limpiar()
        {
            _servicio = null;
            _contextoFactoriaMock = null;
            _repositorioFactoriaMock = null;
        }

        #region Constructor

        [TestMethod]
        public void Prueba_ConstructorContextoFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new AmistadServicio(null, _repositorioFactoriaMock.Object);
            });
        }

        [TestMethod]
        public void Prueba_ConstructorRepositorioFactoriaNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new AmistadServicio(_contextoFactoriaMock.Object, null);
            });
        }

        #endregion

        #region ObtenerSolicitudesPendientesDTO

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesSinSolicitudes_RetornaListaVacia()
        {
            _amigoRepositorioMock
                .Setup(r => r.ObtenerSolicitudesPendientes(IdUsuarioEmisor))
                .Returns(new List<Amigo>());

            List<SolicitudAmistadDTO> resultado = 
                _servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioEmisor);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesNulas_RetornaListaVacia()
        {
            _amigoRepositorioMock
                .Setup(r => r.ObtenerSolicitudesPendientes(IdUsuarioEmisor))
                .Returns((IList<Amigo>)null);

            List<SolicitudAmistadDTO> resultado = 
                _servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioEmisor);

            Assert.AreEqual(0, resultado.Count);
        }

        #endregion

        #region CrearSolicitud

        [TestMethod]
        public void Prueba_CrearSolicitudMismoUsuario_LanzaInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _servicio.CrearSolicitud(IdUsuarioEmisor, IdUsuarioEmisor);
            });
        }

        [TestMethod]
        public void Prueba_CrearSolicitudRelacionExistente_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock
                .Setup(r => r.ExisteRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(true);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _servicio.CrearSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);
            });
        }

        [TestMethod]
        public void Prueba_CrearSolicitudValida_InvocaRepositorio()
        {
            _amigoRepositorioMock
                .Setup(r => r.ExisteRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(false);

            _servicio.CrearSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);

            _amigoRepositorioMock.Verify(
                r => r.CrearSolicitud(IdUsuarioEmisor, IdUsuarioReceptor),
                Times.Once);
        }

        #endregion

        #region AceptarSolicitud

        [TestMethod]
        public void Prueba_AceptarSolicitudInexistente_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns((Amigo)null);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _servicio.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);
            });
        }

        [TestMethod]
        public void Prueba_AceptarSolicitudYaAceptada_LanzaInvalidOperationException()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor,
                Estado = true
            };

            _amigoRepositorioMock
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _servicio.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);
            });
        }

        [TestMethod]
        public void Prueba_AceptarSolicitudReceptorIncorrecto_LanzaInvalidOperationException()
        {
            const int otroUsuarioId = 99;
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = otroUsuarioId,
                Estado = false
            };

            _amigoRepositorioMock
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _servicio.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);
            });
        }

        [TestMethod]
        public void Prueba_AceptarSolicitudValida_ActualizaEstado()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor,
                Estado = false
            };

            _amigoRepositorioMock
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            _servicio.AceptarSolicitud(IdUsuarioEmisor, IdUsuarioReceptor);

            _amigoRepositorioMock.Verify(
                r => r.ActualizarEstado(relacion, true),
                Times.Once);
        }

        #endregion

        #region EliminarAmistad

        [TestMethod]
        public void Prueba_EliminarAmistadMismoUsuario_LanzaInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _servicio.EliminarAmistad(IdUsuarioEmisor, IdUsuarioEmisor);
            });
        }

        [TestMethod]
        public void Prueba_EliminarAmistadInexistente_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns((Amigo)null);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _servicio.EliminarAmistad(IdUsuarioEmisor, IdUsuarioReceptor);
            });
        }

        [TestMethod]
        public void Prueba_EliminarAmistadValida_RetornaRelacionEliminada()
        {
            var relacion = new Amigo
            {
                UsuarioEmisor = IdUsuarioEmisor,
                UsuarioReceptor = IdUsuarioReceptor,
                Estado = true
            };

            _amigoRepositorioMock
                .Setup(r => r.ObtenerRelacion(IdUsuarioEmisor, IdUsuarioReceptor))
                .Returns(relacion);

            Amigo resultado = _servicio.EliminarAmistad(IdUsuarioEmisor, IdUsuarioReceptor);

            Assert.AreEqual(IdUsuarioEmisor, resultado.UsuarioEmisor);
        }

        #endregion

        #region ObtenerAmigosDTO

        [TestMethod]
        public void Prueba_ObtenerAmigosDTOSinAmigos_RetornaListaVacia()
        {
            _amigoRepositorioMock
                .Setup(r => r.ObtenerAmigos(IdUsuarioEmisor))
                .Returns((IList<Usuario>)null);

            List<AmigoDTO> resultado = _servicio.ObtenerAmigosDTO(IdUsuarioEmisor);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigosDTOConAmigos_RetornaListaCorrecta()
        {
            var amigos = new List<Usuario>
            {
                new Usuario { idUsuario = IdUsuarioReceptor, Nombre_Usuario = NombreUsuarioReceptor }
            };

            _amigoRepositorioMock
                .Setup(r => r.ObtenerAmigos(IdUsuarioEmisor))
                .Returns(amigos);

            List<AmigoDTO> resultado = _servicio.ObtenerAmigosDTO(IdUsuarioEmisor);

            Assert.AreEqual(1, resultado.Count);
        }

        #endregion
    }
}
