using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class NotificadorAmigosPruebas
    {
        private const string NombreUsuarioPrueba = "UsuarioPrueba";
        private const int IdUsuarioPrueba = 1;

        private Mock<IManejadorCallback<IAmigosManejadorCallback>> _mockManejadorCallback;
        private Mock<IAmistadServicio> _mockAmistadServicio;
        private Mock<IAmigosManejadorCallback> _mockCallback;
        private NotificadorAmigos _notificador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockManejadorCallback = new Mock<IManejadorCallback<IAmigosManejadorCallback>>();
            _mockAmistadServicio = new Mock<IAmistadServicio>();
            _mockCallback = new Mock<IAmigosManejadorCallback>();

            _notificador = new NotificadorAmigos(
                _mockManejadorCallback.Object, 
                _mockAmistadServicio.Object);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudActualizada_LlamaCallbackCuandoExiste()
        {
            var solicitud = CrearSolicitudAmistadPrueba();
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_mockCallback.Object);

            _notificador.NotificarSolicitudActualizada(NombreUsuarioPrueba, solicitud);

            _mockCallback.Verify(
                callback => callback.NotificarSolicitudActualizada(solicitud), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudActualizada_NoFallaSiCallbackNulo()
        {
            var solicitud = CrearSolicitudAmistadPrueba();
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns((IAmigosManejadorCallback)null);

            _notificador.NotificarSolicitudActualizada(NombreUsuarioPrueba, solicitud);

            _mockCallback.Verify(
                callback => callback.NotificarSolicitudActualizada(It.IsAny<SolicitudAmistadDTO>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudActualizada_ManejaExcepcionCallback()
        {
            var solicitud = CrearSolicitudAmistadPrueba();
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_mockCallback.Object);
            _mockCallback
                .Setup(callback => callback.NotificarSolicitudActualizada(It.IsAny<SolicitudAmistadDTO>()))
                .Throws(new Exception("Error de prueba"));

            _notificador.NotificarSolicitudActualizada(NombreUsuarioPrueba, solicitud);

            _mockManejadorCallback.Verify(
                manejador => manejador.ObtenerCallback(NombreUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarAmistadEliminada_LlamaCallbackCuandoExiste()
        {
            var solicitud = CrearSolicitudAmistadPrueba();
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_mockCallback.Object);

            _notificador.NotificarAmistadEliminada(NombreUsuarioPrueba, solicitud);

            _mockCallback.Verify(
                callback => callback.NotificarAmistadEliminada(solicitud), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarAmistadEliminada_NoFallaSiCallbackNulo()
        {
            var solicitud = CrearSolicitudAmistadPrueba();
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns((IAmigosManejadorCallback)null);

            _notificador.NotificarAmistadEliminada(NombreUsuarioPrueba, solicitud);

            _mockCallback.Verify(
                callback => callback.NotificarAmistadEliminada(It.IsAny<SolicitudAmistadDTO>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarAmistadEliminada_ManejaExcepcionCallback()
        {
            var solicitud = CrearSolicitudAmistadPrueba();
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_mockCallback.Object);
            _mockCallback
                .Setup(callback => callback.NotificarAmistadEliminada(It.IsAny<SolicitudAmistadDTO>()))
                .Throws(new Exception("Error de prueba"));

            _notificador.NotificarAmistadEliminada(NombreUsuarioPrueba, solicitud);

            _mockManejadorCallback.Verify(
                manejador => manejador.ObtenerCallback(NombreUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudesPendientesAlSuscribir_LlamaSolicitudesPendientes()
        {
            var solicitudes = new List<SolicitudAmistadDTO> { CrearSolicitudAmistadPrueba() };
            _mockAmistadServicio
                .Setup(servicio => servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioPrueba))
                .Returns(solicitudes);
            _mockManejadorCallback
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_mockCallback.Object);

            _notificador.NotificarSolicitudesPendientesAlSuscribir(
                NombreUsuarioPrueba, 
                IdUsuarioPrueba);

            _mockCallback.Verify(
                callback => callback.NotificarSolicitudActualizada(It.IsAny<SolicitudAmistadDTO>()), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudesPendientesAlSuscribir_NoNotificaSiListaVacia()
        {
            _mockAmistadServicio
                .Setup(servicio => servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioPrueba))
                .Returns(new List<SolicitudAmistadDTO>());

            _notificador.NotificarSolicitudesPendientesAlSuscribir(
                NombreUsuarioPrueba, 
                IdUsuarioPrueba);

            _mockCallback.Verify(
                callback => callback.NotificarSolicitudActualizada(It.IsAny<SolicitudAmistadDTO>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudesPendientesAlSuscribir_NoNotificaSiListaNula()
        {
            _mockAmistadServicio
                .Setup(servicio => servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioPrueba))
                .Returns((List<SolicitudAmistadDTO>)null);

            _notificador.NotificarSolicitudesPendientesAlSuscribir(
                NombreUsuarioPrueba, 
                IdUsuarioPrueba);

            _mockCallback.Verify(
                callback => callback.NotificarSolicitudActualizada(It.IsAny<SolicitudAmistadDTO>()), 
                Times.Never);
        }

        private static SolicitudAmistadDTO CrearSolicitudAmistadPrueba()
        {
            return new SolicitudAmistadDTO
            {
                UsuarioEmisor = "Emisor",
                UsuarioReceptor = "Receptor",
                SolicitudAceptada = false
            };
        }
    }
}
