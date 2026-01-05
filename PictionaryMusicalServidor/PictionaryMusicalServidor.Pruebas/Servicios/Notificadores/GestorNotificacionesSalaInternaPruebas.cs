using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class GestorNotificacionesSalaInternaPruebas
    {
        private const string NombreUsuarioPrueba = "JugadorPrueba";
        private const string NombreUsuarioSecundario = "JugadorSecundario";
        private const string CodigoSalaPrueba = "SALA01";
        private const string NombreJugadorExpulsado = "JugadorExpulsado";
        private const string NombreJugadorBaneado = "JugadorBaneado";
        private const string NombreCreadorSala = "CreadorSala";
        private const string NombreUsuarioMinusculas = "jugadorprueba";
        private const string NombreUsuarioMayusculas = "JUGADORPRUEBA";

        private GestorNotificacionesSalaInterna _gestor;
        private Mock<ISalasManejadorCallback> _callbackMock;
        private Mock<ISalasManejadorCallback> _callbackSecundarioMock;

        [TestInitialize]
        public void Inicializar()
        {
            _gestor = new GestorNotificacionesSalaInterna();
            _callbackMock = new Mock<ISalasManejadorCallback>();
            _callbackSecundarioMock = new Mock<ISalasManejadorCallback>();
        }

        [TestMethod]
        public void Prueba_Registrar_CallbackSeAlmacenaCorrectamente()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);

            ISalasManejadorCallback callbackObtenido = 
                _gestor.ObtenerCallback(NombreUsuarioPrueba);

            Assert.AreEqual(_callbackMock.Object, callbackObtenido);
        }

        [TestMethod]
        public void Prueba_Registrar_SobrescribeCallbackExistente()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            _gestor.Registrar(NombreUsuarioPrueba, _callbackSecundarioMock.Object);

            ISalasManejadorCallback callbackObtenido = 
                _gestor.ObtenerCallback(NombreUsuarioPrueba);

            Assert.AreEqual(_callbackSecundarioMock.Object, callbackObtenido);
        }

        [TestMethod]
        public void Prueba_Remover_EliminaCallbackRegistrado()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);

            _gestor.Remover(NombreUsuarioPrueba);
            ISalasManejadorCallback callbackObtenido = 
                _gestor.ObtenerCallback(NombreUsuarioPrueba);

            Assert.AreNotEqual(_callbackMock.Object, callbackObtenido);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioNoRegistradoRetornaCallbackNulo()
        {
            ISalasManejadorCallback callbackObtenido = 
                _gestor.ObtenerCallback(NombreUsuarioPrueba);

            Assert.IsFalse(callbackObtenido == null);
        }

        [TestMethod]
        public void Prueba_Limpiar_EliminaTodosLosCallbacks()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            _gestor.Registrar(NombreUsuarioSecundario, _callbackSecundarioMock.Object);

            _gestor.Limpiar();

            ISalasManejadorCallback callbackPrimero = 
                _gestor.ObtenerCallback(NombreUsuarioPrueba);
            ISalasManejadorCallback callbackSegundo = 
                _gestor.ObtenerCallback(NombreUsuarioSecundario);

            Assert.AreNotEqual(_callbackMock.Object, callbackPrimero);
            Assert.AreNotEqual(_callbackSecundarioMock.Object, callbackSegundo);
        }

        [TestMethod]
        public void Prueba_NotificarIngreso_NotificaATodosLosDestinatarios()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            _gestor.Registrar(NombreUsuarioSecundario, _callbackSecundarioMock.Object);
            SalaDTO salaActualizada = CrearSalaDtoPrueba();

            _gestor.NotificarIngreso(CodigoSalaPrueba, NombreUsuarioPrueba, salaActualizada);

            _callbackSecundarioMock.Verify(
                callback => callback.NotificarJugadorSeUnio(
                    CodigoSalaPrueba, 
                    NombreUsuarioPrueba),
                Times.Once);
            _callbackMock.Verify(
                callback => callback.NotificarSalaActualizada(salaActualizada),
                Times.Once);
            _callbackSecundarioMock.Verify(
                callback => callback.NotificarSalaActualizada(salaActualizada),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarIngreso_ExcluyeAlUsuarioQueIngresa()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            SalaDTO salaActualizada = CrearSalaDtoPrueba();

            _gestor.NotificarIngreso(CodigoSalaPrueba, NombreUsuarioPrueba, salaActualizada);

            _callbackMock.Verify(
                callback => callback.NotificarJugadorSeUnio(
                    It.IsAny<string>(), 
                    It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarSalida_NotificaATodosLosDestinatarios()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            _gestor.Registrar(NombreUsuarioSecundario, _callbackSecundarioMock.Object);
            SalaDTO salaActualizada = CrearSalaDtoPrueba();

            _gestor.NotificarSalida(CodigoSalaPrueba, NombreUsuarioPrueba, salaActualizada);

            _callbackMock.Verify(
                callback => callback.NotificarJugadorSalio(
                    CodigoSalaPrueba, 
                    NombreUsuarioPrueba),
                Times.Once);
            _callbackSecundarioMock.Verify(
                callback => callback.NotificarJugadorSalio(
                    CodigoSalaPrueba, 
                    NombreUsuarioPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarExpulsion_NotificaAlExpulsadoYActualizaDemas()
        {
            var callbackExpulsadoMock = new Mock<ISalasManejadorCallback>();
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            _gestor.Registrar(NombreJugadorExpulsado, callbackExpulsadoMock.Object);
            SalaDTO salaActualizada = CrearSalaDtoPrueba();

            var parametros = new ExpulsionNotificacionParametros
            {
                CodigoSala = CodigoSalaPrueba,
                NombreExpulsado = NombreJugadorExpulsado,
                CallbackExpulsado = callbackExpulsadoMock.Object,
                SalaActualizada = salaActualizada
            };

            _gestor.NotificarExpulsion(parametros);

            callbackExpulsadoMock.Verify(
                callback => callback.NotificarJugadorExpulsado(
                    CodigoSalaPrueba, 
                    NombreJugadorExpulsado),
                Times.AtLeastOnce);
            _callbackMock.Verify(
                callback => callback.NotificarSalaActualizada(salaActualizada),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarExpulsion_CallbackNuloNoLanzaExcepcion()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            SalaDTO salaActualizada = CrearSalaDtoPrueba();

            var parametros = new ExpulsionNotificacionParametros
            {
                CodigoSala = CodigoSalaPrueba,
                NombreExpulsado = NombreJugadorExpulsado,
                CallbackExpulsado = null,
                SalaActualizada = salaActualizada
            };

            _gestor.NotificarExpulsion(parametros);

            _callbackMock.Verify(
                callback => callback.NotificarJugadorExpulsado(
                    CodigoSalaPrueba, 
                    NombreJugadorExpulsado),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarCancelacion_NotificaATodosLosDestinatarios()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            _gestor.Registrar(NombreUsuarioSecundario, _callbackSecundarioMock.Object);

            _gestor.NotificarCancelacion(CodigoSalaPrueba);

            _callbackMock.Verify(
                callback => callback.NotificarSalaCancelada(CodigoSalaPrueba),
                Times.Once);
            _callbackSecundarioMock.Verify(
                callback => callback.NotificarSalaCancelada(CodigoSalaPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarBaneo_NotificaAlBaneadoYActualizaDemas()
        {
            var callbackBaneadoMock = new Mock<ISalasManejadorCallback>();
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            _gestor.Registrar(NombreJugadorBaneado, callbackBaneadoMock.Object);
            SalaDTO salaActualizada = CrearSalaDtoPrueba();

            var parametros = new BaneoNotificacionParametros
            {
                CodigoSala = CodigoSalaPrueba,
                NombreBaneado = NombreJugadorBaneado,
                CallbackBaneado = callbackBaneadoMock.Object,
                SalaActualizada = salaActualizada
            };

            _gestor.NotificarBaneo(parametros);

            callbackBaneadoMock.Verify(
                callback => callback.NotificarJugadorBaneado(
                    CodigoSalaPrueba, 
                    NombreJugadorBaneado),
                Times.AtLeastOnce);
            _callbackMock.Verify(
                callback => callback.NotificarSalaActualizada(salaActualizada),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarBaneo_CallbackNuloNoLanzaExcepcion()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _callbackMock.Object);
            SalaDTO salaActualizada = CrearSalaDtoPrueba();

            var parametros = new BaneoNotificacionParametros
            {
                CodigoSala = CodigoSalaPrueba,
                NombreBaneado = NombreJugadorBaneado,
                CallbackBaneado = null,
                SalaActualizada = salaActualizada
            };

            _gestor.NotificarBaneo(parametros);

            _callbackMock.Verify(
                callback => callback.NotificarJugadorBaneado(
                    CodigoSalaPrueba, 
                    NombreJugadorBaneado),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_IgnoraMayusculasEnNombreUsuario()
        {
            _gestor.Registrar(NombreUsuarioMinusculas, _callbackMock.Object);

            ISalasManejadorCallback callbackObtenido = 
                _gestor.ObtenerCallback(NombreUsuarioMayusculas);

            Assert.AreEqual(_callbackMock.Object, callbackObtenido);
        }

        private static SalaDTO CrearSalaDtoPrueba()
        {
            return new SalaDTO
            {
                Codigo = CodigoSalaPrueba,
                Creador = NombreCreadorSala,
                Jugadores = new List<string> { NombreCreadorSala }
            };
        }
    }
}
