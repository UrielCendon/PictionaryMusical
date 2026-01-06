using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Partida;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Partida
{
    [TestClass]
    public class NotificadorPartidaPruebas
    {
        private const string IdSalaPrueba = "123456";
        private const string NombreUsuarioPrueba = "Usuario1";

        private NotificadorPartida _notificador;
        private Mock<ICursoPartidaManejadorCallback> _mockCallback;
        private Dictionary<string, ICursoPartidaManejadorCallback> _callbacks;

        [TestInitialize]
        public void Inicializar()
        {
            _notificador = new NotificadorPartida();
            _mockCallback = new Mock<ICursoPartidaManejadorCallback>();
            _callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { NombreUsuarioPrueba, _mockCallback.Object }
            };
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_LlamaCallbackDeTodosLosJugadores()
        {
            _notificador.NotificarPartidaIniciada(IdSalaPrueba, _callbacks);

            _mockCallback.Verify(
                callback => callback.NotificarPartidaIniciada(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_NoFallaConCallbacksNulo()
        {
            _notificador.NotificarPartidaIniciada(IdSalaPrueba, null);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Prueba_NotificarJugadorAdivino_LlamaCallbackConParametrosCorrectos()
        {
            var parametros = new NotificacionJugadorAdivinoParametros
            {
                IdSala = IdSalaPrueba,
                Callbacks = _callbacks,
                NombreJugador = NombreUsuarioPrueba,
                Puntos = 100
            };

            _notificador.NotificarJugadorAdivino(parametros);

            _mockCallback.Verify(
                callback => callback.NotificarJugadorAdivino(NombreUsuarioPrueba, 100),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarMensajeChat_LlamaCallbackConMensaje()
        {
            var parametros = new NotificacionMensajeChatParametros
            {
                IdSala = IdSalaPrueba,
                Callbacks = _callbacks,
                NombreJugador = NombreUsuarioPrueba,
                Mensaje = "Mensaje de prueba"
            };

            _notificador.NotificarMensajeChat(parametros);

            _mockCallback.Verify(
                callback => callback.NotificarMensajeChat(NombreUsuarioPrueba, "Mensaje de prueba"),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarTrazoRecibido_LlamaCallbackConTrazo()
        {
            var trazo = new TrazoDTO();

            _notificador.NotificarTrazoRecibido(IdSalaPrueba, _callbacks, trazo);

            _mockCallback.Verify(
                callback => callback.NotificarTrazoRecibido(trazo),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarFinRonda_LlamaCallbackConTiempoAgotado()
        {
            _notificador.NotificarFinRonda(IdSalaPrueba, _callbacks, true);

            _mockCallback.Verify(
                callback => callback.NotificarFinRonda(true),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarFinPartida_LlamaCallbackConResultado()
        {
            var resultado = new ResultadoPartidaDTO();

            _notificador.NotificarFinPartida(IdSalaPrueba, _callbacks, resultado);

            _mockCallback.Verify(
                callback => callback.NotificarFinPartida(resultado),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLimpiarLienzo_LlamaCallbackConTrazoLimpiar()
        {
            _notificador.NotificarLimpiarLienzo(IdSalaPrueba, _callbacks);

            _mockCallback.Verify(
                callback => callback.NotificarTrazoRecibido(
                    It.Is<TrazoDTO>(trazo => trazo.EsLimpiarTodo)),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_CallbackInvalido_SeDisparaAlFallarNotificacion()
        {
            var callbackInvalidoDisparado = false;
            var mockCallbackFallido = new Mock<ICursoPartidaManejadorCallback>();
            mockCallbackFallido
                .Setup(callback => callback.NotificarMensajeChat(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new CommunicationException());

            var callbacksFallidos = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { NombreUsuarioPrueba, mockCallbackFallido.Object }
            };

            _notificador.CallbackInvalido += (idSala, nombreUsuario) => callbackInvalidoDisparado = true;

            var parametros = new NotificacionMensajeChatParametros
            {
                IdSala = IdSalaPrueba,
                Callbacks = callbacksFallidos,
                NombreJugador = NombreUsuarioPrueba,
                Mensaje = "Mensaje"
            };
            _notificador.NotificarMensajeChat(parametros);

            Assert.IsTrue(callbackInvalidoDisparado);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_ManejaExcepcionTimeout()
        {
            var mockCallbackTimeout = new Mock<ICursoPartidaManejadorCallback>();
            mockCallbackTimeout
                .Setup(callback => callback.NotificarPartidaIniciada())
                .Throws(new TimeoutException());

            var callbacksTimeout = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { NombreUsuarioPrueba, mockCallbackTimeout.Object }
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacksTimeout);

            Assert.IsTrue(true);
        }
    }
}
