using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Datos.Modelo;
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
        private const int IdUsuarioPrueba = 1;

        private Mock<IManejadorCallback<IListaAmigosManejadorCallback>> _mockManejadorCallback;
        private Mock<IAmistadServicio> _mockAmistadServicio;
        private Mock<IContextoFactoria> _mockContextoFactoria;
        private Mock<IRepositorioFactoria> _mockRepositorioFactoria;
        private Mock<BaseDatosPruebaEntities> _mockContexto;
        private Mock<IUsuarioRepositorio> _mockUsuarioRepositorio;
        private Mock<IListaAmigosManejadorCallback> _mockCallback;
        private NotificadorListaAmigos _notificador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockManejadorCallback = new Mock<IManejadorCallback<IListaAmigosManejadorCallback>>();
            _mockAmistadServicio = new Mock<IAmistadServicio>();
            _mockContextoFactoria = new Mock<IContextoFactoria>();
            _mockRepositorioFactoria = new Mock<IRepositorioFactoria>();
            _mockContexto = new Mock<BaseDatosPruebaEntities>();
            _mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();
            _mockCallback = new Mock<IListaAmigosManejadorCallback>();

            _mockContextoFactoria
                .Setup(factoria => factoria.CrearContexto())
                .Returns(_mockContexto.Object);

            _mockRepositorioFactoria
                .Setup(factoria => factoria.CrearUsuarioRepositorio(_mockContexto.Object))
                .Returns(_mockUsuarioRepositorio.Object);

            _notificador = new NotificadorListaAmigos(
                _mockManejadorCallback.Object,
                _mockAmistadServicio.Object,
                _mockContextoFactoria.Object,
                _mockRepositorioFactoria.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionContextoFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new NotificadorListaAmigos(
                    _mockManejadorCallback.Object,
                    _mockAmistadServicio.Object,
                    null,
                    _mockRepositorioFactoria.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionRepositorioFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new NotificadorListaAmigos(
                    _mockManejadorCallback.Object,
                    _mockAmistadServicio.Object,
                    _mockContextoFactoria.Object,
                    null));
        }

        [TestMethod]
        public void Prueba_NotificarLista_LlamaCallbackCuandoExiste()
        {
            var amigos = new List<AmigoDTO> { CrearAmigoDTOPrueba() };
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_mockCallback.Object);

            _notificador.NotificarLista(NombreUsuarioPrueba, amigos);

            _mockCallback.Verify(
                callback => callback.NotificarListaAmigosActualizada(amigos), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLista_NoFallaSiCallbackNulo()
        {
            var amigos = new List<AmigoDTO> { CrearAmigoDTOPrueba() };
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns((IListaAmigosManejadorCallback)null);

            _notificador.NotificarLista(NombreUsuarioPrueba, amigos);

            _mockCallback.Verify(
                callback => callback.NotificarListaAmigosActualizada(It.IsAny<List<AmigoDTO>>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarLista_ManejaExcepcionCallback()
        {
            var amigos = new List<AmigoDTO> { CrearAmigoDTOPrueba() };
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_mockCallback.Object);
            _mockCallback
                .Setup(callback => callback.NotificarListaAmigosActualizada(It.IsAny<List<AmigoDTO>>()))
                .Throws(new Exception("Error de prueba"));

            _notificador.NotificarLista(NombreUsuarioPrueba, amigos);

            _mockManejadorCallback.Verify(
                manejador => manejador.ObtenerCallback(NombreUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_IgnoraNombreVacio()
        {
            _notificador.NotificarCambioAmistad(string.Empty);

            _mockManejadorCallback.Verify(
                manejador => manejador.ObtenerCallback(It.IsAny<string>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_IgnoraNombreNulo()
        {
            _notificador.NotificarCambioAmistad(null);

            _mockManejadorCallback.Verify(
                manejador => manejador.ObtenerCallback(It.IsAny<string>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarCambioAmistad_IgnoraNombreSoloEspacios()
        {
            _notificador.NotificarCambioAmistad("   ");

            _mockManejadorCallback.Verify(
                manejador => manejador.ObtenerCallback(It.IsAny<string>()), 
                Times.Never);
        }

        private static AmigoDTO CrearAmigoDTOPrueba()
        {
            return new AmigoDTO
            {
                NombreUsuario = "Amigo",
                UsuarioId = 2
            };
        }
    }
}
