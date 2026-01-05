using System;
using System.Collections.Generic;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class NotificadorListaAmigosPruebas
    {
        private const string NombreUsuarioPrueba = "UsuarioPrueba";
        private const string NombreUsuarioInvalido = "";
        private const int IdUsuarioPrueba = 100;
        private const string NombreAmigoPrueba = "AmigoPrueba";
        private const int IdAmigoPrueba = 200;

        private Mock<IManejadorCallback<IListaAmigosManejadorCallback>> _manejadorCallbackMock;
        private Mock<IAmistadServicio> _amistadServicioMock;
        private Mock<IContextoFactoria> _contextoFactoriaMock;
        private Mock<IRepositorioFactoria> _repositorioFactoriaMock;
        private Mock<IUsuarioRepositorio> _usuarioRepositorioMock;
        private Mock<BaseDatosPruebaEntities> _contextoMock;
        private Mock<IListaAmigosManejadorCallback> _callbackMock;
        private NotificadorListaAmigos _notificador;

        [TestInitialize]
        public void Inicializar()
        {
            _manejadorCallbackMock = new Mock<IManejadorCallback<IListaAmigosManejadorCallback>>();
            _amistadServicioMock = new Mock<IAmistadServicio>();
            _contextoFactoriaMock = new Mock<IContextoFactoria>();
            _repositorioFactoriaMock = new Mock<IRepositorioFactoria>();
            _usuarioRepositorioMock = new Mock<IUsuarioRepositorio>();
            _contextoMock = new Mock<BaseDatosPruebaEntities>();
            _callbackMock = new Mock<IListaAmigosManejadorCallback>();

            _contextoFactoriaMock
                .Setup(factoria => factoria.CrearContexto())
                .Returns(_contextoMock.Object);
            _repositorioFactoriaMock
                .Setup(factoria => factoria.CrearUsuarioRepositorio(It.IsAny<BaseDatosPruebaEntities>()))
                .Returns(_usuarioRepositorioMock.Object);

            _notificador = new NotificadorListaAmigos(
                _manejadorCallbackMock.Object,
                _amistadServicioMock.Object,
                _contextoFactoriaMock.Object,
                _repositorioFactoriaMock.Object);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_NombreUsuarioInvalidoNoNotifica()
        {
            _notificador.NotificarCambioAmistad(NombreUsuarioInvalido);

            _contextoFactoriaMock.Verify(
                factoria => factoria.CrearContexto(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_NombreUsuarioNuloNoNotifica()
        {
            _notificador.NotificarCambioAmistad(null);

            _contextoFactoriaMock.Verify(
                factoria => factoria.CrearContexto(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_UsuarioExistenteNotificaLista()
        {
            var usuario = CrearUsuarioPrueba();
            var listaAmigos = CrearListaAmigosPrueba();
            ConfigurarUsuarioExistente(usuario);
            ConfigurarListaAmigos(listaAmigos);
            ConfigurarCallbackExistente();

            _notificador.NotificarCambioAmistad(NombreUsuarioPrueba);

            _callbackMock.Verify(
                callback => callback.NotificarListaAmigosActualizada(listaAmigos),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_ExcepcionFaultExceptionNoSePropaga()
        {
            var usuario = CrearUsuarioPrueba();
            ConfigurarUsuarioExistente(usuario);
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Throws(new System.ServiceModel.FaultException());

            _notificador.NotificarCambioAmistad(NombreUsuarioPrueba);

            _amistadServicioMock.Verify(
                servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_ExcepcionArgumentOutOfRangeNoSePropaga()
        {
            var usuario = CrearUsuarioPrueba();
            ConfigurarUsuarioExistente(usuario);
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Throws(new ArgumentOutOfRangeException("idUsuario", "ID fuera de rango"));

            _notificador.NotificarCambioAmistad(NombreUsuarioPrueba);

            _amistadServicioMock.Verify(
                servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_ExcepcionArgumentExceptionNoSePropaga()
        {
            var usuario = CrearUsuarioPrueba();
            ConfigurarUsuarioExistente(usuario);
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Throws(new ArgumentException("Argumento invalido"));

            _notificador.NotificarCambioAmistad(NombreUsuarioPrueba);

            _amistadServicioMock.Verify(
                servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_ExcepcionDataExceptionNoSePropaga()
        {
            var usuario = CrearUsuarioPrueba();
            ConfigurarUsuarioExistente(usuario);
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Throws(new System.Data.DataException());

            _notificador.NotificarCambioAmistad(NombreUsuarioPrueba);

            _amistadServicioMock.Verify(
                servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_ExcepcionInvalidOperationNoSePropaga()
        {
            var usuario = CrearUsuarioPrueba();
            ConfigurarUsuarioExistente(usuario);
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Throws(new InvalidOperationException());

            _notificador.NotificarCambioAmistad(NombreUsuarioPrueba);

            _amistadServicioMock.Verify(
                servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_ExcepcionGeneralNoSePropaga()
        {
            var usuario = CrearUsuarioPrueba();
            ConfigurarUsuarioExistente(usuario);
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Throws(new Exception());

            _notificador.NotificarCambioAmistad(NombreUsuarioPrueba);

            _amistadServicioMock.Verify(
                servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLista_CallbackExistenteNotifica()
        {
            var listaAmigos = CrearListaAmigosPrueba();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_callbackMock.Object);

            _notificador.NotificarLista(NombreUsuarioPrueba, listaAmigos);

            _callbackMock.Verify(
                callback => callback.NotificarListaAmigosActualizada(listaAmigos),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLista_CallbackNuloNoLanzaExcepcion()
        {
            var listaAmigos = CrearListaAmigosPrueba();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns((IListaAmigosManejadorCallback)null);

            _notificador.NotificarLista(NombreUsuarioPrueba, listaAmigos);

            _callbackMock.Verify(
                callback => callback.NotificarListaAmigosActualizada(It.IsAny<List<AmigoDTO>>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarLista_ExcepcionEnCallbackNoSePropaga()
        {
            var listaAmigos = CrearListaAmigosPrueba();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_callbackMock.Object);
            _callbackMock
                .Setup(callback => callback.NotificarListaAmigosActualizada(It.IsAny<List<AmigoDTO>>()))
                .Throws(new Exception());

            _notificador.NotificarLista(NombreUsuarioPrueba, listaAmigos);

            _callbackMock.Verify(
                callback => callback.NotificarListaAmigosActualizada(listaAmigos),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLista_ListaVaciaNotificaCorrectamente()
        {
            var listaVacia = new List<AmigoDTO>();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_callbackMock.Object);

            _notificador.NotificarLista(NombreUsuarioPrueba, listaVacia);

            _callbackMock.Verify(
                callback => callback.NotificarListaAmigosActualizada(listaVacia),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_Constructor_RepositorioFactoriaNuloLanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new NotificadorListaAmigos(
                    _manejadorCallbackMock.Object,
                    _amistadServicioMock.Object,
                    _contextoFactoriaMock.Object,
                    null));
        }

        [TestMethod]
        public void Prueba_Constructor_ContextoFactoriaNuloLanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new NotificadorListaAmigos(
                    _manejadorCallbackMock.Object,
                    _amistadServicioMock.Object,
                    null,
                    _repositorioFactoriaMock.Object));
        }

        private void ConfigurarUsuarioExistente(Usuario usuario)
        {
            _usuarioRepositorioMock
                .Setup(repositorio => repositorio.ObtenerPorNombreUsuario(NombreUsuarioPrueba))
                .Returns(usuario);
        }

        private void ConfigurarListaAmigos(List<AmigoDTO> listaAmigos)
        {
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerAmigosDTO(IdUsuarioPrueba))
                .Returns(listaAmigos);
        }

        private void ConfigurarCallbackExistente()
        {
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_callbackMock.Object);
        }

        private static Usuario CrearUsuarioPrueba()
        {
            return new Usuario
            {
                idUsuario = IdUsuarioPrueba,
                Nombre_Usuario = NombreUsuarioPrueba
            };
        }

        private static List<AmigoDTO> CrearListaAmigosPrueba()
        {
            return new List<AmigoDTO>
            {
                new AmigoDTO
                {
                    UsuarioId = IdAmigoPrueba,
                    NombreUsuario = NombreAmigoPrueba
                }
            };
        }
    }
}
