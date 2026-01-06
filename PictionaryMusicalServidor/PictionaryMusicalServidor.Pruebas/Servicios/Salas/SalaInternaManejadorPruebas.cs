using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Salas
{
    [TestClass]
    public class SalaInternaManejadorPruebas
    {
        private const string CodigoSalaPrueba = "123456";
        private const string CreadorPrueba = "Creador1";
        private const string JugadorPrueba = "Jugador1";

        private Mock<IGestorNotificacionesSalaInterna> _mockGestorNotificaciones;
        private SalaInternaManejador _salaInterna;

        [TestInitialize]
        public void Inicializar()
        {
            _mockGestorNotificaciones = new Mock<IGestorNotificacionesSalaInterna>();
            _salaInterna = new SalaInternaManejador(
                CodigoSalaPrueba,
                CreadorPrueba,
                new ConfiguracionPartidaDTO(),
                _mockGestorNotificaciones.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionGestorNotificacionesNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SalaInternaManejador(
                    CodigoSalaPrueba,
                    CreadorPrueba,
                    new ConfiguracionPartidaDTO(),
                    null));
        }

        [TestMethod]
        public void Prueba_Constructor_AsignaCodigoCorrecto()
        {
            Assert.AreEqual(CodigoSalaPrueba, _salaInterna.Codigo);
        }

        [TestMethod]
        public void Prueba_Constructor_AsignaCreadorCorrecto()
        {
            Assert.AreEqual(CreadorPrueba, _salaInterna.Creador);
        }

        [TestMethod]
        public void Prueba_Constructor_PartidaNoIniciada()
        {
            Assert.IsFalse(_salaInterna.PartidaIniciada);
        }

        [TestMethod]
        public void Prueba_Constructor_PartidaNoFinalizada()
        {
            Assert.IsFalse(_salaInterna.PartidaFinalizada);
        }

        [TestMethod]
        public void Prueba_Constructor_NoDebeEliminarse()
        {
            Assert.IsFalse(_salaInterna.DebeEliminarse);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaCodigo()
        {
            var dto = _salaInterna.ConvertirADto();

            Assert.AreEqual(CodigoSalaPrueba, dto.Codigo);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaCreador()
        {
            var dto = _salaInterna.ConvertirADto();

            Assert.AreEqual(CreadorPrueba, dto.Creador);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_RegistraCallbackEnGestor()
        {
            var mockCallback = new Mock<ISalasManejadorCallback>();

            _salaInterna.AgregarJugador(JugadorPrueba, mockCallback.Object, true);

            _mockGestorNotificaciones.Verify(
                gestor => gestor.Registrar(JugadorPrueba, mockCallback.Object),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_IncluyeJugadorEnLista()
        {
            var mockCallback = new Mock<ISalasManejadorCallback>();

            var dto = _salaInterna.AgregarJugador(JugadorPrueba, mockCallback.Object, false);

            Assert.IsTrue(dto.Jugadores.Contains(JugadorPrueba));
        }

        [TestMethod]
        public void Prueba_AgregarJugador_NotificaIngresoSiSeIndica()
        {
            var mockCallback = new Mock<ISalasManejadorCallback>();

            _salaInterna.AgregarJugador(JugadorPrueba, mockCallback.Object, true);

            _mockGestorNotificaciones.Verify(
                gestor => gestor.NotificarIngreso(
                    CodigoSalaPrueba,
                    JugadorPrueba,
                    It.IsAny<SalaDTO>()),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_NoNotificaIngresoSiNoSeIndica()
        {
            var mockCallback = new Mock<ISalasManejadorCallback>();

            _salaInterna.AgregarJugador(JugadorPrueba, mockCallback.Object, false);

            _mockGestorNotificaciones.Verify(
                gestor => gestor.NotificarIngreso(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SalaDTO>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_AgregarJugador_ActualizaCallbackSiJugadorYaExiste()
        {
            var mockCallback1 = new Mock<ISalasManejadorCallback>();
            var mockCallback2 = new Mock<ISalasManejadorCallback>();
            _salaInterna.AgregarJugador(JugadorPrueba, mockCallback1.Object, false);

            _salaInterna.AgregarJugador(JugadorPrueba, mockCallback2.Object, false);

            _mockGestorNotificaciones.Verify(
                gestor => gestor.Registrar(JugadorPrueba, mockCallback2.Object),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_RemueveCallbackDelGestor()
        {
            var mockCallback = new Mock<ISalasManejadorCallback>();
            _salaInterna.AgregarJugador(JugadorPrueba, mockCallback.Object, false);

            _salaInterna.RemoverJugador(JugadorPrueba);

            _mockGestorNotificaciones.Verify(
                gestor => gestor.Remover(JugadorPrueba),
                Times.Once);
        }

        [TestMethod]
        public void Prueba_RemoverJugador_NoHaceNadaSiJugadorNoExiste()
        {
            _salaInterna.RemoverJugador("JugadorInexistente");

            _mockGestorNotificaciones.Verify(
                gestor => gestor.Remover(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Prueba_PartidaIniciada_PermiteCambiarValor()
        {
            _salaInterna.PartidaIniciada = true;

            Assert.IsTrue(_salaInterna.PartidaIniciada);
        }

        [TestMethod]
        public void Prueba_PartidaFinalizada_PermiteCambiarValor()
        {
            _salaInterna.PartidaFinalizada = true;

            Assert.IsTrue(_salaInterna.PartidaFinalizada);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaConfiguracion()
        {
            var dto = _salaInterna.ConvertirADto();

            Assert.IsNotNull(dto.Configuracion);
        }

        [TestMethod]
        public void Prueba_ConvertirADto_RetornaListaJugadoresVacia()
        {
            var dto = _salaInterna.ConvertirADto();

            Assert.AreEqual(0, dto.Jugadores.Count);
        }
    }
}
