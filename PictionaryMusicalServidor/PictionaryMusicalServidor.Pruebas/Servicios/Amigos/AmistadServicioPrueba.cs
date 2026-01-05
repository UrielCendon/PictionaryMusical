using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
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
        private const string NombreUsuarioReceptor = "UsuarioReceptor";

        [TestInitialize]
        public void Inicializar()
        {
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _amigoRepositorioMock = new Mock<IAmigoRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();

            _contextoFactoriaMock
                .Setup(factoria => factoria.CrearContexto())
                .Returns(_contextoMock.Object);
            _repositorioFactoriaMock
                .Setup(factoria => factoria.CrearAmigoRepositorio(
                    It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_amigoRepositorioMock.Object);

            _amistadServicio = new AmistadServicio(
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object);
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_MismoUsuario_LanzaInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => _amistadServicio.CrearSolicitud(IdEmisorPrueba, IdEmisorPrueba));
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_RelacionExistente_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ExisteRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns(true);

            Assert.ThrowsException<InvalidOperationException>(
                () => _amistadServicio.CrearSolicitud(IdEmisorPrueba, IdReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_CrearSolicitud_FlujoExitoso()
        {
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ExisteRelacion(IdEmisorPrueba, IdReceptorPrueba))
                .Returns(false);

            _amistadServicio.CrearSolicitud(IdEmisorPrueba, IdReceptorPrueba);

            _amigoRepositorioMock.Verify(
                repositorio => repositorio.CrearSolicitud(IdEmisorPrueba, IdReceptorPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_AceptarSolicitud_SolicitudInexistente_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ObtenerRelacion(
                    IdEmisorPrueba,
                    IdReceptorPrueba))
                .Returns((Amigo)null);

            Assert.ThrowsException<InvalidOperationException>(
                () => _amistadServicio.AceptarSolicitud(IdEmisorPrueba, IdReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_AceptarSolicitud_YaAceptada_LanzaInvalidOperationException()
        {
            var relacion = new Amigo
            {
                UsuarioReceptor = IdReceptorPrueba,
                Estado = true
            };
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ObtenerRelacion(
                    IdEmisorPrueba,
                    IdReceptorPrueba))
                .Returns(relacion);

            Assert.ThrowsException<InvalidOperationException>(
                () => _amistadServicio.AceptarSolicitud(IdEmisorPrueba, IdReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_AceptarSolicitud_FlujoExitoso()
        {
            var relacion = new Amigo
            {
                UsuarioReceptor = IdReceptorPrueba,
                Estado = false
            };
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ObtenerRelacion(
                    IdEmisorPrueba,
                    IdReceptorPrueba))
                .Returns(relacion);

            _amistadServicio.AceptarSolicitud(IdEmisorPrueba, IdReceptorPrueba);

            _amigoRepositorioMock.Verify(
                repositorio => repositorio.ActualizarEstado(relacion, true),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_EliminarAmistad_MismoUsuario_LanzaInvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => _amistadServicio.EliminarAmistad(IdEmisorPrueba, IdEmisorPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarAmistad_RelacionNoExiste_LanzaInvalidOperationException()
        {
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ObtenerRelacion(
                    IdEmisorPrueba,
                    IdReceptorPrueba))
                .Returns((Amigo)null);

            Assert.ThrowsException<InvalidOperationException>(
                () => _amistadServicio.EliminarAmistad(IdEmisorPrueba, IdReceptorPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarAmistad_FlujoExitoso()
        {
            var relacion = new Amigo();
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ObtenerRelacion(
                    IdEmisorPrueba,
                    IdReceptorPrueba))
                .Returns(relacion);

            var resultado = _amistadServicio.EliminarAmistad(IdEmisorPrueba, IdReceptorPrueba);

            Assert.AreEqual(relacion, resultado);
            _amigoRepositorioMock.Verify(
                repositorio => repositorio.EliminarRelacion(relacion),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigosDTO_SinAmigos_RetornaListaVacia()
        {
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ObtenerAmigos(IdEmisorPrueba))
                .Returns((List<Usuario>)null);

            var resultado = _amistadServicio.ObtenerAmigosDTO(IdEmisorPrueba);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerAmigosDTO_ConAmigos_RetornaListaAmigos()
        {
            var listaAmigos = new List<Usuario>
            {
                new Usuario
                {
                    idUsuario = IdReceptorPrueba,
                    Nombre_Usuario = NombreUsuarioReceptor
                }
            };
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ObtenerAmigos(IdEmisorPrueba))
                .Returns(listaAmigos);

            var resultado = _amistadServicio.ObtenerAmigosDTO(IdEmisorPrueba);

            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual(NombreUsuarioReceptor, resultado[0].NombreUsuario);
        }

        [TestMethod]
        public void Prueba_ObtenerSolicitudesPendientesDTO_SinSolicitudes_RetornaListaVacia()
        {
            _amigoRepositorioMock
                .Setup(repositorio => repositorio.ObtenerSolicitudesPendientes(IdReceptorPrueba))
                .Returns(new List<Amigo>());

            var resultado = _amistadServicio.ObtenerSolicitudesPendientesDTO(IdReceptorPrueba);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }
    }
}