using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class OperacionAmistadServicioPrueba
    {
        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IAmistadServicio> _amistadServicioMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private OperacionAmistadServicio _operacionServicio;

        private const string NombreEmisorPrueba = "EmisorPrueba";
        private const string NombreReceptorPrueba = "ReceptorPrueba";
        private const int IdEmisorPrueba = 100;
        private const int IdReceptorPrueba = 200;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _amistadServicioMock = new Mock<IAmistadServicio>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock
                .Setup(factoria => factoria.CrearContexto())
                .Returns(_contextoMock.Object);
            _repositorioFactoriaMock
                .Setup(factoria => factoria.CrearUsuarioRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _operacionServicio = new OperacionAmistadServicio(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object,
                _amistadServicioMock.Object);
        }

        [TestMethod]
        public void Prueba_ObtenerDatosUsuarioSuscripcion_UsuarioNoExiste_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _operacionServicio.ObtenerDatosUsuarioSuscripcion(NombreEmisorPrueba));
        }

        [TestMethod]
        public void Prueba_ObtenerDatosUsuarioSuscripcion_FlujoExitoso()
        {
            var usuario = new Usuario
            {
                idUsuario = IdEmisorPrueba,
                Nombre_Usuario = NombreEmisorPrueba
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns(usuario);

            var resultado = _operacionServicio.ObtenerDatosUsuarioSuscripcion(NombreEmisorPrueba);

            Assert.AreEqual(IdEmisorPrueba, resultado.IdUsuario);
            Assert.IsNotNull(resultado.NombreNormalizado);
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitud_FlujoExitoso()
        {
            var emisor = new Usuario
            {
                idUsuario = IdEmisorPrueba,
                Nombre_Usuario = NombreEmisorPrueba
            };
            var receptor = new Usuario
            {
                idUsuario = IdReceptorPrueba,
                Nombre_Usuario = NombreReceptorPrueba
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns(emisor);
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreReceptorPrueba))
                .Returns(receptor);

            var resultado = _operacionServicio.EjecutarCreacionSolicitud(
                NombreEmisorPrueba,
                NombreReceptorPrueba);

            Assert.AreEqual(emisor, resultado.Emisor);
            Assert.AreEqual(receptor, resultado.Receptor);
            _amistadServicioMock.Verify(
                servicio => servicio.CrearSolicitud(IdEmisorPrueba, IdReceptorPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitud_EmisorNoExiste_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _operacionServicio.EjecutarCreacionSolicitud(
                    NombreEmisorPrueba,
                    NombreReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_EjecutarCreacionSolicitud_ReceptorNoExiste_LanzaFaultException()
        {
            var emisor = new Usuario
            {
                idUsuario = IdEmisorPrueba,
                Nombre_Usuario = NombreEmisorPrueba
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns(emisor);
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreReceptorPrueba))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _operacionServicio.EjecutarCreacionSolicitud(
                    NombreEmisorPrueba,
                    NombreReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_EjecutarAceptacionSolicitud_FlujoExitoso()
        {
            var emisor = new Usuario
            {
                idUsuario = IdEmisorPrueba,
                Nombre_Usuario = NombreEmisorPrueba
            };
            var receptor = new Usuario
            {
                idUsuario = IdReceptorPrueba,
                Nombre_Usuario = NombreReceptorPrueba
            };

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns(emisor);
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreReceptorPrueba))
                .Returns(receptor);

            var resultado = _operacionServicio.EjecutarAceptacionSolicitud(
                NombreEmisorPrueba,
                NombreReceptorPrueba);

            Assert.IsNotNull(resultado.NombreNormalizadoEmisor);
            Assert.IsNotNull(resultado.NombreNormalizadoReceptor);
            _amistadServicioMock.Verify(
                servicio => servicio.AceptarSolicitud(IdEmisorPrueba, IdReceptorPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EjecutarAceptacionSolicitud_UsuarioNoExiste_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _operacionServicio.EjecutarAceptacionSolicitud(
                    NombreEmisorPrueba,
                    NombreReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_EjecutarEliminacion_FlujoExitoso()
        {
            var emisor = new Usuario
            {
                idUsuario = IdEmisorPrueba,
                Nombre_Usuario = NombreEmisorPrueba
            };
            var receptor = new Usuario
            {
                idUsuario = IdReceptorPrueba,
                Nombre_Usuario = NombreReceptorPrueba
            };
            var relacion = new Amigo();

            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns(emisor);
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreReceptorPrueba))
                .Returns(receptor);
            _amistadServicioMock
                .Setup(servicio => servicio.EliminarAmistad(IdEmisorPrueba, IdReceptorPrueba))
                .Returns(relacion);

            var resultado = _operacionServicio.EjecutarEliminacion(
                NombreEmisorPrueba,
                NombreReceptorPrueba);

            Assert.AreEqual(relacion, resultado.Relacion);
            Assert.IsNotNull(resultado.NombrePrimerUsuarioNormalizado);
            Assert.IsNotNull(resultado.NombreSegundoUsuarioNormalizado);
        }

        [TestMethod]
        public void Prueba_EjecutarEliminacion_UsuarioNoExiste_LanzaFaultException()
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreEmisorPrueba))
                .Returns((Usuario)null);

            Assert.ThrowsException<FaultException>(
                () => _operacionServicio.EjecutarEliminacion(
                    NombreEmisorPrueba,
                    NombreReceptorPrueba));
        }
    }
}