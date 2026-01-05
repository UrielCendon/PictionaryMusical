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
        private const string IdSalaPrueba = "SALA001";
        private const string IdJugadorUno = "1";
        private const string IdJugadorDos = "2";
        private const string IdJugadorTres = "3";
        private const string NombreJugadorUno = "JugadorUno";
        private const string MensajePrueba = "Mensaje de prueba";
        private const int PuntosPrueba = 100;
        private const int GrosorTrazo = 5;
        private const string ColorHex = "#FF0000";
        private const double CoordenadaX = 10.0;
        private const double CoordenadaY = 20.0;

        private NotificadorPartida _notificador;
        private Mock<ICursoPartidaManejadorCallback> _callbackMock;

        [TestInitialize]
        public void Inicializar()
        {
            _notificador = new NotificadorPartida();
            _callbackMock = new Mock<ICursoPartidaManejadorCallback>();
        }

        private TrazoDTO CrearTrazoPrueba()
        {
            return new TrazoDTO
            {
                PuntosX = new double[] { CoordenadaX },
                PuntosY = new double[] { CoordenadaY },
                ColorHex = ColorHex,
                Grosor = GrosorTrazo,
                EsBorrado = false,
                EsLimpiarTodo = false
            };
        }

        private ResultadoPartidaDTO CrearResultadoPartidaPrueba()
        {
            return new ResultadoPartidaDTO
            {
                Clasificacion = new List<ClasificacionUsuarioDTO>
                {
                    new ClasificacionUsuarioDTO
                    {
                        Usuario = NombreJugadorUno,
                        Puntos = PuntosPrueba
                    }
                },
                Mensaje = null
            };
        }

        private Mock<ICursoPartidaManejadorCallback> CrearCallbackConCanalAbierto()
        {
            var mockCallback = new Mock<ICursoPartidaManejadorCallback>();
            var mockCanal = mockCallback.As<ICommunicationObject>();
            mockCanal.Setup(canal => canal.State).Returns(CommunicationState.Opened);
            return mockCallback;
        }

        private Mock<ICursoPartidaManejadorCallback> CrearCallbackConCanalCerrado()
        {
            var mockCallback = new Mock<ICursoPartidaManejadorCallback>();
            var mockCanal = mockCallback.As<ICommunicationObject>();
            mockCanal.Setup(canal => canal.State).Returns(CommunicationState.Closed);
            return mockCallback;
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_CallbacksNulo_NoLanzaExcepcion()
        {
            _notificador.NotificarPartidaIniciada(IdSalaPrueba, null);

            _callbackMock.Verify(
                callback => callback.NotificarPartidaIniciada(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_CallbacksVacios_NoLanzaExcepcion()
        {
            var callbacksVacios = new Dictionary<string, ICursoPartidaManejadorCallback>();

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacksVacios);

            _callbackMock.Verify(
                callback => callback.NotificarPartidaIniciada(),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_UnCallback_LlamaNotificarPartidaIniciada()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacks);

            callbackAbierto.Verify(
                callback => callback.NotificarPartidaIniciada(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_MultiplesCallbacks_LlamaATodos()
        {
            var callbackUno = CrearCallbackConCanalAbierto();
            var callbackDos = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackUno.Object },
                { IdJugadorDos, callbackDos.Object }
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacks);

            callbackUno.Verify(callback => callback.NotificarPartidaIniciada(), Times.Once);
            callbackDos.Verify(callback => callback.NotificarPartidaIniciada(), Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarJugadorAdivino_CallbacksValidos_NotificaCorrectamente()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };
            var parametros = new NotificacionJugadorAdivinoParametros
            {
                IdSala = IdSalaPrueba,
                Callbacks = callbacks,
                NombreJugador = NombreJugadorUno,
                Puntos = PuntosPrueba
            };

            _notificador.NotificarJugadorAdivino(parametros);

            callbackAbierto.Verify(
                callback => callback.NotificarJugadorAdivino(NombreJugadorUno, PuntosPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarJugadorAdivino_CallbacksNulo_NoLanzaExcepcion()
        {
            var parametros = new NotificacionJugadorAdivinoParametros
            {
                IdSala = IdSalaPrueba,
                Callbacks = null,
                NombreJugador = NombreJugadorUno,
                Puntos = PuntosPrueba
            };

            _notificador.NotificarJugadorAdivino(parametros);

            _callbackMock.Verify(
                callback => callback.NotificarJugadorAdivino(
                    It.IsAny<string>(),
                    It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarMensajeChat_CallbacksValidos_NotificaCorrectamente()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };
            var parametros = new NotificacionMensajeChatParametros
            {
                IdSala = IdSalaPrueba,
                Callbacks = callbacks,
                NombreJugador = NombreJugadorUno,
                Mensaje = MensajePrueba
            };

            _notificador.NotificarMensajeChat(parametros);

            callbackAbierto.Verify(
                callback => callback.NotificarMensajeChat(NombreJugadorUno, MensajePrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarMensajeChat_CallbacksNulo_NoLanzaExcepcion()
        {
            var parametros = new NotificacionMensajeChatParametros
            {
                IdSala = IdSalaPrueba,
                Callbacks = null,
                NombreJugador = NombreJugadorUno,
                Mensaje = MensajePrueba
            };

            _notificador.NotificarMensajeChat(parametros);

            _callbackMock.Verify(
                callback => callback.NotificarMensajeChat(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarTrazoRecibido_CallbacksValidos_NotificaTrazo()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };
            var trazo = CrearTrazoPrueba();

            _notificador.NotificarTrazoRecibido(IdSalaPrueba, callbacks, trazo);

            callbackAbierto.Verify(
                callback => callback.NotificarTrazoRecibido(trazo),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarTrazoRecibido_CallbacksNulo_NoLanzaExcepcion()
        {
            var trazo = CrearTrazoPrueba();

            _notificador.NotificarTrazoRecibido(IdSalaPrueba, null, trazo);

            _callbackMock.Verify(
                callback => callback.NotificarTrazoRecibido(It.IsAny<TrazoDTO>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarFinRonda_TiempoAgotadoTrue_NotificaCorrectamente()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };

            _notificador.NotificarFinRonda(IdSalaPrueba, callbacks, true);

            callbackAbierto.Verify(
                callback => callback.NotificarFinRonda(true),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarFinRonda_TiempoAgotadoFalse_NotificaCorrectamente()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };

            _notificador.NotificarFinRonda(IdSalaPrueba, callbacks, false);

            callbackAbierto.Verify(
                callback => callback.NotificarFinRonda(false),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarFinRonda_CallbacksNulo_NoLanzaExcepcion()
        {
            _notificador.NotificarFinRonda(IdSalaPrueba, null, true);

            _callbackMock.Verify(
                callback => callback.NotificarFinRonda(It.IsAny<bool>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarFinPartida_CallbacksValidos_NotificaResultado()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };
            var resultado = CrearResultadoPartidaPrueba();

            _notificador.NotificarFinPartida(IdSalaPrueba, callbacks, resultado);

            callbackAbierto.Verify(
                callback => callback.NotificarFinPartida(resultado),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarFinPartida_CallbacksNulo_NoLanzaExcepcion()
        {
            var resultado = CrearResultadoPartidaPrueba();

            _notificador.NotificarFinPartida(IdSalaPrueba, null, resultado);

            _callbackMock.Verify(
                callback => callback.NotificarFinPartida(It.IsAny<ResultadoPartidaDTO>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarLimpiarLienzo_CallbacksValidos_NotificaTrazoLimpiar()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };

            _notificador.NotificarLimpiarLienzo(IdSalaPrueba, callbacks);

            callbackAbierto.Verify(
                callback => callback.NotificarTrazoRecibido(
                    It.Is<TrazoDTO>(trazo =>
                        trazo.EsLimpiarTodo == true &&
                        trazo.EsBorrado == true)),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLimpiarLienzo_CallbacksNulo_NoLanzaExcepcion()
        {
            _notificador.NotificarLimpiarLienzo(IdSalaPrueba, null);

            _callbackMock.Verify(
                callback => callback.NotificarTrazoRecibido(It.IsAny<TrazoDTO>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_CanalCerrado_DisparaCallbackInvalido()
        {
            var callbackCerrado = CrearCallbackConCanalCerrado();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackCerrado.Object }
            };
            string salaRecibida = null;
            string jugadorRecibido = null;

            _notificador.CallbackInvalido += (sala, jugador) =>
            {
                salaRecibida = sala;
                jugadorRecibido = jugador;
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacks);

            Assert.AreEqual(IdSalaPrueba, salaRecibida);
            Assert.AreEqual(IdJugadorUno, jugadorRecibido);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_CommunicationException_DisparaCallbackInvalido()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            callbackAbierto
                .Setup(callback => callback.NotificarPartidaIniciada())
                .Throws(new CommunicationException("Error de comunicacion"));

            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };
            string salaRecibida = null;
            string jugadorRecibido = null;

            _notificador.CallbackInvalido += (sala, jugador) =>
            {
                salaRecibida = sala;
                jugadorRecibido = jugador;
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacks);

            Assert.AreEqual(IdSalaPrueba, salaRecibida);
            Assert.AreEqual(IdJugadorUno, jugadorRecibido);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_TimeoutException_DisparaCallbackInvalido()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            callbackAbierto
                .Setup(callback => callback.NotificarPartidaIniciada())
                .Throws(new TimeoutException("Tiempo agotado"));

            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };
            string salaRecibida = null;

            _notificador.CallbackInvalido += (sala, jugador) =>
            {
                salaRecibida = sala;
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacks);

            Assert.AreEqual(IdSalaPrueba, salaRecibida);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_ObjectDisposed_DisparaCallbackInvalido()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            callbackAbierto
                .Setup(callback => callback.NotificarPartidaIniciada())
                .Throws(new ObjectDisposedException("Canal"));

            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };
            string jugadorRecibido = null;

            _notificador.CallbackInvalido += (sala, jugador) =>
            {
                jugadorRecibido = jugador;
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacks);

            Assert.AreEqual(IdJugadorUno, jugadorRecibido);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_ExcepcionGeneral_DisparaCallbackInvalido()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            callbackAbierto
                .Setup(callback => callback.NotificarPartidaIniciada())
                .Throws(new Exception("Error inesperado"));

            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };
            bool eventoDisparado = false;

            _notificador.CallbackInvalido += (sala, jugador) =>
            {
                eventoDisparado = true;
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacks);

            Assert.IsTrue(eventoDisparado);
        }

        [TestMethod]
        public void Prueba_NotificarPartidaIniciada_CallbackFallido_ContinuaConSiguiente()
        {
            var callbackFallido = CrearCallbackConCanalAbierto();
            callbackFallido
                .Setup(callback => callback.NotificarPartidaIniciada())
                .Throws(new CommunicationException("Error"));

            var callbackExitoso = CrearCallbackConCanalAbierto();

            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackFallido.Object },
                { IdJugadorDos, callbackExitoso.Object }
            };

            _notificador.NotificarPartidaIniciada(IdSalaPrueba, callbacks);

            callbackExitoso.Verify(
                callback => callback.NotificarPartidaIniciada(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarLimpiarLienzo_TrazoTienePuntosVacios_ArregloCorrecto()
        {
            var callbackAbierto = CrearCallbackConCanalAbierto();
            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackAbierto.Object }
            };

            _notificador.NotificarLimpiarLienzo(IdSalaPrueba, callbacks);

            callbackAbierto.Verify(
                callback => callback.NotificarTrazoRecibido(
                    It.Is<TrazoDTO>(trazo =>
                        trazo.PuntosX.Length == 0 &&
                        trazo.PuntosY.Length == 0)),
                Times.Once);
        }



        [TestMethod]
        public void Prueba_NotificarTrazoRecibido_MultiplesCallbacks_NotificaTodos()
        {
            var callbackUno = CrearCallbackConCanalAbierto();
            var callbackDos = CrearCallbackConCanalAbierto();
            var callbackTres = CrearCallbackConCanalAbierto();

            var callbacks = new Dictionary<string, ICursoPartidaManejadorCallback>
            {
                { IdJugadorUno, callbackUno.Object },
                { IdJugadorDos, callbackDos.Object },
                { IdJugadorTres, callbackTres.Object }
            };
            var trazo = CrearTrazoPrueba();

            _notificador.NotificarTrazoRecibido(IdSalaPrueba, callbacks, trazo);

            callbackUno.Verify(
                callback => callback.NotificarTrazoRecibido(trazo),
                Times.Once);
            callbackDos.Verify(
                callback => callback.NotificarTrazoRecibido(trazo),
                Times.Once);
            callbackTres.Verify(
                callback => callback.NotificarTrazoRecibido(trazo),
                Times.Once);
        }
    }
}
