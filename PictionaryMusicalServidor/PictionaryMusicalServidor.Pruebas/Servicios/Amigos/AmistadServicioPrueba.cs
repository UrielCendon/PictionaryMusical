using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Amigos;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Amigos
{
    [TestClass]
    public class AmistadServicioPrueba
    {
        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IAmigoRepositorio> _amigoRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private AmistadServicio _amistadServicio;

        private const int IdEmisorPrueba = 1;
        private const int IdReceptorPrueba = 2;

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _amigoRepositorioMock = new Mock<IAmigoRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock.Setup(contextoFactoria => contextoFactoria.CrearContexto())
                .Returns(_contextoMock.Object);
            _repositorioFactoriaMock.Setup(repositorioFactoria => repositorioFactoria.CrearAmigoRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_amigoRepositorioMock.Object);

            _amistadServicio = new AmistadServicio(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object);
        }

        [TestMethod]
        public void Prueba_CrearSolicitudMismoUsuario_LanzaInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _amistadServicio.CrearSolicitud(IdEmisorPrueba, IdEmisorPrueba));
        }

        [TestMethod]
        public void Prueba_CrearSolicitudRelacionExistente_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ExisteRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns(true);

            Assert.ThrowsException<InvalidOperationException>(() =>
                _amistadServicio.CrearSolicitud(IdEmisorPrueba, IdReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_FlujoExitoso()
        {
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ExisteRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns(false);

            _amistadServicio.CrearSolicitud(IdEmisorPrueba, IdReceptorPrueba);

            _amigoRepositorioMock.Verify(amigoRepositorio => amigoRepositorio.CrearSolicitud(IdEmisorPrueba, IdReceptorPrueba), Times.Once);
        }

        [TestMethod]
        public void Prueba_AceptarSolicitudInexistente_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ObtenerRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns((Amigo)null);

            Assert.ThrowsException<InvalidOperationException>(() =>
                _amistadServicio.AceptarSolicitud(IdEmisorPrueba, IdReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_AceptarSolicitudYaAceptada_LanzaInvalidOperationException()
        {
            var relacion = new Amigo { UsuarioReceptor = IdReceptorPrueba, Estado = true };
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ObtenerRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns(relacion);

            Assert.ThrowsException<InvalidOperationException>(() =>
                _amistadServicio.AceptarSolicitud(IdEmisorPrueba, IdReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_AceptarSolicitud_FlujoExitoso()
        {
            var relacion = new Amigo { UsuarioReceptor = IdReceptorPrueba, Estado = false };
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ObtenerRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns(relacion);

            _amistadServicio.AceptarSolicitud(IdEmisorPrueba, IdReceptorPrueba);

            _amigoRepositorioMock.Verify(amigoRepositorio => amigoRepositorio.ActualizarEstado(relacion, true), Times.Once);
        }

        [TestMethod]
        public void Prueba_EliminarAmistad_FlujoExitoso()
        {
            var relacion = new Amigo();
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ObtenerRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns(relacion);

            var resultado = _amistadServicio.EliminarAmistad(IdEmisorPrueba, IdReceptorPrueba);

            Assert.AreEqual(relacion, resultado);
            _amigoRepositorioMock.Verify(amigoRepositorio => amigoRepositorio.EliminarRelacion(relacion), Times.Once);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesDTO_FlujoExitoso()
        {
            var solicitudesPendientes = new List<Amigo>();
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ObtenerSolicitudesPendientes(IdReceptorPrueba))
                .Returns(solicitudesPendientes);

            var resultado = _amistadServicio.ObtenerSolicitudesPendientesDTO(IdReceptorPrueba);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigosDTO_FlujoExitoso()
        {
            var listaAmigos = new List<Usuario>
            {
                new Usuario { idUsuario = IdEmisorPrueba, Nombre_Usuario = "Amigo1" }
            };

            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ObtenerAmigos(IdReceptorPrueba))
                .Returns(listaAmigos);

            var resultado = _amistadServicio.ObtenerAmigosDTO(IdReceptorPrueba);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Amigo1", resultado[0].NombreUsuario);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigosDTO_SinAmigosRetornaListaVacia()
        {
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ObtenerAmigos(IdReceptorPrueba))
                .Returns((List<Usuario>)null);

            var resultado = _amistadServicio.ObtenerAmigosDTO(IdReceptorPrueba);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_EliminarAmistadMismoUsuario_LanzaInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _amistadServicio.EliminarAmistad(IdEmisorPrueba, IdEmisorPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarAmistadRelacionNoExiste_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock.Setup(amigoRepositorio => amigoRepositorio.ObtenerRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns((Amigo)null);

            Assert.ThrowsException<InvalidOperationException>(() =>
                _amistadServicio.EliminarAmistad(IdEmisorPrueba, IdReceptorPrueba));
        }
    }
}