using System;
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
        private const string NombreUsuarioPrueba = "UsuarioPrueba";
        private const string NombreUsuarioDosPrueba = "UsuarioDosPrueba";
        private const string CodigoSalaPrueba = "SALA01";

        private Mock<ISalasManejadorCallback> _mockCallback;
        private GestorNotificacionesSalaInterna _gestor;

        [TestInitialize]
        public void Inicializar()
        {
            _mockCallback = new Mock<ISalasManejadorCallback>();
            _gestor = new GestorNotificacionesSalaInterna();
        }

        [TestMethod]
        public void Prueba_Registrar_AgregaCallbackAlGestor()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);

            var callback = _gestor.ObtenerCallback(NombreUsuarioPrueba);

            Assert.IsNotNull(callback);
        }

        [TestMethod]
        public void Prueba_Registrar_ReemplazaCallbackExistente()
        {
            var mockCallbackNuevo = new Mock<ISalasManejadorCallback>();
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);

            _gestor.Registrar(NombreUsuarioPrueba, mockCallbackNuevo.Object);

            var callback = _gestor.ObtenerCallback(NombreUsuarioPrueba);
            Assert.AreEqual(mockCallbackNuevo.Object, callback);
        }

        [TestMethod]
        public void Prueba_Remover_EliminaCallbackDelGestor()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);

            _gestor.Remover(NombreUsuarioPrueba);

            var callback = _gestor.ObtenerCallback(NombreUsuarioPrueba);
            Assert.AreNotEqual(_mockCallback.Object, callback);
        }

        [TestMethod]
        public void Prueba_Remover_NoFallaConUsuarioInexistente()
        {
            _gestor.Remover(NombreUsuarioPrueba);

            var callback = _gestor.ObtenerCallback(NombreUsuarioPrueba);
            Assert.IsNotNull(callback);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_RetornaCallbackNuloSiNoExiste()
        {
            var callback = _gestor.ObtenerCallback(NombreUsuarioPrueba);

            Assert.IsNotNull(callback);
            Assert.AreNotEqual(_mockCallback.Object, callback);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_RetornaCallbackRegistrado()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);

            var callback = _gestor.ObtenerCallback(NombreUsuarioPrueba);

            Assert.AreEqual(_mockCallback.Object, callback);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_NoDistingueMayusculasMinusculas()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);

            var callback = _gestor.ObtenerCallback(NombreUsuarioPrueba.ToUpper());

            Assert.AreEqual(_mockCallback.Object, callback);
        }

        [TestMethod]
        public void Prueba_Limpiar_EliminaTodosLosCallbacks()
        {
            var mockCallbackDos = new Mock<ISalasManejadorCallback>();
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);
            _gestor.Registrar(NombreUsuarioDosPrueba, mockCallbackDos.Object);

            _gestor.Limpiar();

            var callback1 = _gestor.ObtenerCallback(NombreUsuarioPrueba);
            var callback2 = _gestor.ObtenerCallback(NombreUsuarioDosPrueba);
            Assert.AreNotEqual(_mockCallback.Object, callback1);
            Assert.AreNotEqual(mockCallbackDos.Object, callback2);
        }

        [TestMethod]
        public void Prueba_NotificarIngreso_NotificaATodosExceptoNuevoJugador()
        {
            var mockCallbackDos = new Mock<ISalasManejadorCallback>();
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);
            _gestor.Registrar(NombreUsuarioDosPrueba, mockCallbackDos.Object);
            var salaActualizada = CrearSalaDTOPrueba();

            _gestor.NotificarIngreso(CodigoSalaPrueba, NombreUsuarioDosPrueba, salaActualizada);

            _mockCallback.Verify(
                callback => callback.NotificarJugadorSeUnio(
                    CodigoSalaPrueba, 
                    NombreUsuarioDosPrueba), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarSalida_NotificaATodosLosJugadores()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);
            var salaActualizada = CrearSalaDTOPrueba();

            _gestor.NotificarSalida(CodigoSalaPrueba, NombreUsuarioDosPrueba, salaActualizada);

            _mockCallback.Verify(
                callback => callback.NotificarJugadorSalio(
                    CodigoSalaPrueba, 
                    NombreUsuarioDosPrueba), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarSalida_NotificaSalaActualizadaATodos()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);
            var salaActualizada = CrearSalaDTOPrueba();

            _gestor.NotificarSalida(CodigoSalaPrueba, NombreUsuarioDosPrueba, salaActualizada);

            _mockCallback.Verify(
                callback => callback.NotificarSalaActualizada(salaActualizada), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarAccionJugador_NotificaExpulsionATodos()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);
            var parametros = new AccionJugadorSalaParametros
            {
                CodigoSala = CodigoSalaPrueba,
                NombreJugadorAfectado = NombreUsuarioDosPrueba,
                TipoAccion = TipoAccionJugador.Expulsion,
                SalaActualizada = CrearSalaDTOPrueba()
            };

            _gestor.NotificarAccionJugador(parametros);

            _mockCallback.Verify(
                callback => callback.NotificarJugadorExpulsado(
                    CodigoSalaPrueba, 
                    NombreUsuarioDosPrueba), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarAccionJugador_NotificaBaneoATodos()
        {
            _gestor.Registrar(NombreUsuarioPrueba, _mockCallback.Object);
            var parametros = new AccionJugadorSalaParametros
            {
                CodigoSala = CodigoSalaPrueba,
                NombreJugadorAfectado = NombreUsuarioDosPrueba,
                TipoAccion = TipoAccionJugador.Baneo,
                SalaActualizada = CrearSalaDTOPrueba()
            };

            _gestor.NotificarAccionJugador(parametros);

            _mockCallback.Verify(
                callback => callback.NotificarJugadorBaneado(
                    CodigoSalaPrueba, 
                    NombreUsuarioDosPrueba), 
                Times.Once);
        }

        private static SalaDTO CrearSalaDTOPrueba()
        {
            return new SalaDTO
            {
                Codigo = CodigoSalaPrueba,
                Creador = "Creador"
            };
        }
    }
}
