using System;
using System.Collections.Generic;
using System.Data;
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
        private const string NombreUsuarioNormalizado = "usuarioprueba";
        private const int IdUsuarioPrueba = 100;
        private const string NombreEmisor = "UsuarioEmisor";
        private const string NombreReceptor = "UsuarioReceptor";

        private Mock<IManejadorCallback<IAmigosManejadorCallback>> _manejadorCallbackMock;
        private Mock<IAmistadServicio> _amistadServicioMock;
        private Mock<IAmigosManejadorCallback> _callbackMock;
        private NotificadorAmigos _notificador;

        [TestInitialize]
        public void Inicializar()
        {
            _manejadorCallbackMock = new Mock<IManejadorCallback<IAmigosManejadorCallback>>();
            _amistadServicioMock = new Mock<IAmistadServicio>();
            _callbackMock = new Mock<IAmigosManejadorCallback>();

            _notificador = new NotificadorAmigos(
                _manejadorCallbackMock.Object,
                _amistadServicioMock.Object);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudActualizada_CallbackExistenteNotifica()
        {
            var solicitud = CrearSolicitudAmistadDto();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_callbackMock.Object);

            _notificador.NotificarSolicitudActualizada(NombreUsuarioPrueba, solicitud);

            _callbackMock.Verify(
                callback => callback.NotificarSolicitudActualizada(solicitud),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudActualizada_CallbackNuloNoLanzaExcepcion()
        {
            var solicitud = CrearSolicitudAmistadDto();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns((IAmigosManejadorCallback)null);

            _notificador.NotificarSolicitudActualizada(NombreUsuarioPrueba, solicitud);

            _callbackMock.Verify(
                callback => callback.NotificarSolicitudActualizada(It.IsAny<SolicitudAmistadDTO>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudActualizada_ExcepcionEnCallbackNoSePropaga()
        {
            var solicitud = CrearSolicitudAmistadDto();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_callbackMock.Object);
            _callbackMock
                .Setup(callback => callback.NotificarSolicitudActualizada(It.IsAny<SolicitudAmistadDTO>()))
                .Throws(new Exception());

            _notificador.NotificarSolicitudActualizada(NombreUsuarioPrueba, solicitud);

            _callbackMock.Verify(
                callback => callback.NotificarSolicitudActualizada(solicitud),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarAmistadEliminada_CallbackExistenteNotifica()
        {
            var solicitud = CrearSolicitudAmistadDto();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_callbackMock.Object);

            _notificador.NotificarAmistadEliminada(NombreUsuarioPrueba, solicitud);

            _callbackMock.Verify(
                callback => callback.NotificarAmistadEliminada(solicitud),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarAmistadEliminada_CallbackNuloNoLanzaExcepcion()
        {
            var solicitud = CrearSolicitudAmistadDto();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns((IAmigosManejadorCallback)null);

            _notificador.NotificarAmistadEliminada(NombreUsuarioPrueba, solicitud);

            _callbackMock.Verify(
                callback => callback.NotificarAmistadEliminada(It.IsAny<SolicitudAmistadDTO>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarAmistadEliminada_ExcepcionEnCallbackNoSePropaga()
        {
            var solicitud = CrearSolicitudAmistadDto();
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioPrueba))
                .Returns(_callbackMock.Object);
            _callbackMock
                .Setup(callback => callback.NotificarAmistadEliminada(It.IsAny<SolicitudAmistadDTO>()))
                .Throws(new Exception());

            _notificador.NotificarAmistadEliminada(NombreUsuarioPrueba, solicitud);

            _callbackMock.Verify(
                callback => callback.NotificarAmistadEliminada(solicitud),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudesPendientesAlSuscribir_SinSolicitudesNoNotifica()
        {
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioPrueba))
                .Returns(new List<SolicitudAmistadDTO>());

            _notificador.NotificarSolicitudesPendientesAlSuscribir(
                NombreUsuarioNormalizado, 
                IdUsuarioPrueba);

            _manejadorCallbackMock.Verify(
                manejador => manejador.ObtenerCallback(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudesPendientesAlSuscribir_ListaNulaNoNotifica()
        {
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioPrueba))
                .Returns((List<SolicitudAmistadDTO>)null);

            _notificador.NotificarSolicitudesPendientesAlSuscribir(
                NombreUsuarioNormalizado, 
                IdUsuarioPrueba);

            _manejadorCallbackMock.Verify(
                manejador => manejador.ObtenerCallback(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudesPendientesAlSuscribir_ConSolicitudesNotificaCadaUna()
        {
            var solicitudes = new List<SolicitudAmistadDTO>
            {
                CrearSolicitudAmistadDto(),
                CrearSolicitudAmistadDto()
            };
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioPrueba))
                .Returns(solicitudes);
            _manejadorCallbackMock
                .Setup(manejador => manejador.ObtenerCallback(NombreUsuarioNormalizado))
                .Returns(_callbackMock.Object);

            _notificador.NotificarSolicitudesPendientesAlSuscribir(
                NombreUsuarioNormalizado, 
                IdUsuarioPrueba);

            _callbackMock.Verify(
                callback => callback.NotificarSolicitudActualizada(It.IsAny<SolicitudAmistadDTO>()),
                Times.Exactly(2));
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudesPendientesAlSuscribir_DataExceptionSePropaga()
        {
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioPrueba))
                .Throws(new DataException());

            Assert.ThrowsException<DataException>(() =>
                _notificador.NotificarSolicitudesPendientesAlSuscribir(
                    NombreUsuarioNormalizado, 
                    IdUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_NotificarSolicitudesPendientesAlSuscribir_ExcepcionGeneralLanzaDataException()
        {
            _amistadServicioMock
                .Setup(servicio => servicio.ObtenerSolicitudesPendientesDTO(IdUsuarioPrueba))
                .Throws(new InvalidOperationException());

            Assert.ThrowsException<DataException>(() =>
                _notificador.NotificarSolicitudesPendientesAlSuscribir(
                    NombreUsuarioNormalizado, 
                    IdUsuarioPrueba));
        }

        private static SolicitudAmistadDTO CrearSolicitudAmistadDto()
        {
            return new SolicitudAmistadDTO
            {
                UsuarioEmisor = NombreEmisor,
                UsuarioReceptor = NombreReceptor,
                SolicitudAceptada = false
            };
        }
    }
}
