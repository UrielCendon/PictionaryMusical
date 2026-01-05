using System;
using System.Collections.Generic;
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
        private const string NombreCreadorSala = "CreadorSala";

        private Mock<IObtenerSalas> _proveedorSalasMock;
        private Mock<ISalasManejadorCallback> _callbackMock;
        private Mock<ISalasManejadorCallback> _callbackSecundarioMock;
        private NotificadorSalas _notificador;

        [TestInitialize]
        public void Inicializar()
        {
            _proveedorSalasMock = new Mock<IObtenerSalas>();
            _callbackMock = new Mock<ISalasManejadorCallback>();
            _callbackSecundarioMock = new Mock<ISalasManejadorCallback>();

            _notificador = new NotificadorSalas(_proveedorSalasMock.Object);
        }

        [TestMethod]
        public void Prueba_Suscribir_RetornaGuidValido()
        {
            Guid sesionId = _notificador.Suscribir(_callbackMock.Object);

            Assert.AreNotEqual(Guid.Empty, sesionId);
        }

        [TestMethod]
        public void Prueba_Suscribir_MultiplesCallbacksRetornanGuidsDistintos()
        {
            Guid sesionId1 = _notificador.Suscribir(_callbackMock.Object);
            Guid sesionId2 = _notificador.Suscribir(_callbackSecundarioMock.Object);

            Assert.AreNotEqual(sesionId1, sesionId2);
        }

        [TestMethod]
        public void Prueba_Desuscribir_EliminaSuscripcionExistente()
        {
            ConfigurarProveedorSalasVacio();
            Guid sesionId = _notificador.Suscribir(_callbackMock.Object);

            _notificador.Desuscribir(sesionId);
            _notificador.NotificarListaSalasATodos();

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_Desuscribir_GuidInexistenteNoLanzaExcepcion()
        {
            Guid guidInexistente = Guid.NewGuid();
            ConfigurarProveedorSalasVacio();

            _notificador.Desuscribir(guidInexistente);
            _notificador.NotificarListaSalasATodos();

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_DesuscribirPorCallback_EliminaTodasLasSuscripcionesDelCallback()
        {
            ConfigurarProveedorSalasVacio();
            _notificador.Suscribir(_callbackMock.Object);
            _notificador.Suscribir(_callbackMock.Object);
            _notificador.Suscribir(_callbackSecundarioMock.Object);

            _notificador.DesuscribirPorCallback(_callbackMock.Object);
            _notificador.NotificarListaSalasATodos();

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Never);
            _callbackSecundarioMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_DesuscribirPorCallback_CallbackNoRegistradoNoLanzaExcepcion()
        {
            ConfigurarProveedorSalasVacio();

            _notificador.DesuscribirPorCallback(_callbackMock.Object);
            _notificador.NotificarListaSalasATodos();

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalas_EnviaListaAlCallback()
        {
            var salasInternas = CrearListaSalasInternas();
            _proveedorSalasMock
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(salasInternas);

            _notificador.NotificarListaSalas(_callbackMock.Object);

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalas_ExcepcionComunicacionNoSePropaga()
        {
            _proveedorSalasMock
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(new List<SalaInternaManejador>());
            _callbackMock
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new System.ServiceModel.CommunicationException());

            _notificador.NotificarListaSalas(_callbackMock.Object);

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalas_ExcepcionTimeoutNoSePropaga()
        {
            _proveedorSalasMock
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(new List<SalaInternaManejador>());
            _callbackMock
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new TimeoutException());

            _notificador.NotificarListaSalas(_callbackMock.Object);

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalas_ExcepcionObjectDisposedNoSePropaga()
        {
            _proveedorSalasMock
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(new List<SalaInternaManejador>());
            _callbackMock
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new ObjectDisposedException("callback"));

            _notificador.NotificarListaSalas(_callbackMock.Object);

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalasATodos_NotificaATodosLosSuscriptores()
        {
            ConfigurarProveedorSalasVacio();
            _notificador.Suscribir(_callbackMock.Object);
            _notificador.Suscribir(_callbackSecundarioMock.Object);

            _notificador.NotificarListaSalasATodos();

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
            _callbackSecundarioMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalasATodos_SinSuscriptoresNoLanzaExcepcion()
        {
            ConfigurarProveedorSalasVacio();

            _notificador.NotificarListaSalasATodos();

            _proveedorSalasMock.Verify(
                proveedor => proveedor.ObtenerSalasInternas(),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalasATodos_ExcepcionEnUnCallbackNoAfectaAOtros()
        {
            ConfigurarProveedorSalasVacio();
            _notificador.Suscribir(_callbackMock.Object);
            _notificador.Suscribir(_callbackSecundarioMock.Object);
            _callbackMock
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new System.ServiceModel.CommunicationException());

            _notificador.NotificarListaSalasATodos();

            _callbackSecundarioMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalasATodos_CommunicationExceptionRemoveSuscripcion()
        {
            ConfigurarProveedorSalasVacio();
            _notificador.Suscribir(_callbackMock.Object);
            _callbackMock
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new System.ServiceModel.CommunicationException());

            _notificador.NotificarListaSalasATodos();
            _callbackMock.Invocations.Clear();
            _notificador.NotificarListaSalasATodos();

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_NotificarListaSalasATodos_TimeoutExceptionRemoveSuscripcion()
        {
            ConfigurarProveedorSalasVacio();
            _notificador.Suscribir(_callbackMock.Object);
            _callbackMock
                .Setup(callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()))
                .Throws(new TimeoutException());

            _notificador.NotificarListaSalasATodos();
            _callbackMock.Invocations.Clear();
            _notificador.NotificarListaSalasATodos();

            _callbackMock.Verify(
                callback => callback.NotificarListaSalasActualizada(It.IsAny<SalaDTO[]>()),
                Times.Never);
        }

        private void ConfigurarProveedorSalasVacio()
        {
            _proveedorSalasMock
                .Setup(proveedor => proveedor.ObtenerSalasInternas())
                .Returns(new List<SalaInternaManejador>());
        }

        private static List<SalaInternaManejador> CrearListaSalasInternas()
        {
            var gestorMock = new Mock<IGestorNotificacionesSalaInterna>();
            var configuracion = new ConfiguracionPartidaDTO
            {
                NumeroRondas = 3,
                TiempoPorRondaSegundos = 60,
                Dificultad = "media"
            };

            var sala = new SalaInternaManejador(
                CodigoSalaPrueba,
                NombreCreadorSala,
                configuracion,
                gestorMock.Object);

            return new List<SalaInternaManejador> { sala };
        }
    }
}
