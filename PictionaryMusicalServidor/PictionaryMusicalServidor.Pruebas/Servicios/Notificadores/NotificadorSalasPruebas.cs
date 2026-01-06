using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class NotificadorSalasPruebas
    {
        private const string CodigoSalaPrueba = "SALA01";
        private const string NombreCreadorPrueba = "Creador";
        private const int MaximoJugadoresPrueba = 4;

        private Mock<IObtenerSalas> _mockProveedorSalas;
        private Mock<ISalasManejadorCallback> _mockCallback;
        private NotificadorSalas _notificador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockProveedorSalas = new Mock<IObtenerSalas>();
            _mockCallback = new Mock<ISalasManejadorCallback>();
            _notificador = new NotificadorSalas(_mockProveedorSalas.Object);
        }

        [TestMethod]
        public void Prueba_Suscribir_RetornaGuidValido()
        {
            Guid sesionId = _notificador.Suscribir(_mockCallback.Object);

            Assert.AreNotEqual(Guid.Empty, sesionId);
        }

        [TestMethod]
        public void Prueba_Suscribir_RetornaGuidsDiferentes()
        {
            var mockCallback2 = new Mock<ISalasManejadorCallback>();

            Guid sesionId1 = _notificador.Suscribir(_mockCallback.Object);
            Guid sesionId2 = _notificador.Suscribir(mockCallback2.Object);

            Assert.AreNotEqual(sesionId1, sesionId2);
        }

        [TestMethod]
        public void Prueba_Desuscribir_EliminaSuscripcionExistente()
        {
            Guid sesionId = _notificador.Suscribir(_mockCallback.Object);
            _mockProveedorSalas
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(Array.Empty<SalaInternaManejador>());

            _notificador.Desuscribir(sesionId);
            _notificador.NotificarListaSalasATodos();

            _mockCallback.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_Desuscribir_NoFallaConSesionInexistente()
        {
            Guid sesionInvalida = Guid.NewGuid();

            _notificador.Desuscribir(sesionInvalida);

            Assert.AreNotEqual(Guid.Empty, sesionInvalida);
        }

        [TestMethod]
        public void Prueba_DesuscribirPorCallback_EliminaSuscripcionPorReferencia()
        {
            _notificador.Suscribir(_mockCallback.Object);
            _mockProveedorSalas
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(Array.Empty<SalaInternaManejador>());

            _notificador.DesuscribirPorCallback(_mockCallback.Object);
            _notificador.NotificarListaSalasATodos();

            _mockCallback.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()), 
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalas_LlamaCallbackConLista()
        {
            _mockProveedorSalas
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(Array.Empty<SalaInternaManejador>());

            _notificador.NotificarListaSalas(_mockCallback.Object);

            _mockCallback.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalas_ManejaExcepcionComunicacion()
        {
            _mockProveedorSalas
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(Array.Empty<SalaInternaManejador>());
            _mockCallback
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new System.ServiceModel.CommunicationException());

            _notificador.NotificarListaSalas(_mockCallback.Object);

            _mockCallback.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalas_ManejaExcepcionTimeout()
        {
            _mockProveedorSalas
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(Array.Empty<SalaInternaManejador>());
            _mockCallback
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new TimeoutException());

            _notificador.NotificarListaSalas(_mockCallback.Object);

            _mockCallback.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalasATodos_NotificaTodosSuscriptores()
        {
            var mockCallback2 = new Mock<ISalasManejadorCallback>();
            _notificador.Suscribir(_mockCallback.Object);
            _notificador.Suscribir(mockCallback2.Object);
            _mockProveedorSalas
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(Array.Empty<SalaInternaManejador>());

            _notificador.NotificarListaSalasATodos();

            _mockCallback.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()), 
                Times.Once);
            mockCallback2.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()), 
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalasATodos_NoFallaSinSuscriptores()
        {
            _mockProveedorSalas
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(Array.Empty<SalaInternaManejador>());

            _notificador.NotificarListaSalasATodos();

            _mockProveedorSalas.Verify(
                proveedor => proveedor.ObtenerSalasInternas(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalasATodos_EliminaSuscriptorConErrorComunicacion()
        {
            _notificador.Suscribir(_mockCallback.Object);
            _mockProveedorSalas
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(Array.Empty<SalaInternaManejador>());
            _mockCallback
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new System.ServiceModel.CommunicationException());

            _notificador.NotificarListaSalasATodos();
            _mockCallback.Reset();
            _mockCallback.Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()));
            _notificador.NotificarListaSalasATodos();

            _mockCallback.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()), 
                Times.Never);
        }
    }
}
