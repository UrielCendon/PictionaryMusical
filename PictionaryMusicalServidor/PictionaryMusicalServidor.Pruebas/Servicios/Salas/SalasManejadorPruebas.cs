using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Salas
{
    [TestClass]
    public class SalasManejadorPruebas
    {
        private const string CodigoSalaPrueba = "123456";
        private const string NombreJugadorPrueba = "Jugador1";

        private Mock<INotificadorSalas> _mockNotificadorSalas;
        private Mock<IAlmacenSalas> _mockAlmacenSalas;
        private Mock<IProveedorContextoOperacion> _mockProveedorContexto;
        private Mock<ISalaInternaFactoria> _mockSalaFactoria;
        private Mock<IGeneradorCodigoSala> _mockGeneradorCodigo;
        private SalasManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockNotificadorSalas = new Mock<INotificadorSalas>();
            _mockAlmacenSalas = new Mock<IAlmacenSalas>();
            _mockProveedorContexto = new Mock<IProveedorContextoOperacion>();
            _mockSalaFactoria = new Mock<ISalaInternaFactoria>();
            _mockGeneradorCodigo = new Mock<IGeneradorCodigoSala>();
            _manejador = new SalasManejador(
                _mockNotificadorSalas.Object,
                _mockAlmacenSalas.Object,
                _mockProveedorContexto.Object,
                _mockSalaFactoria.Object,
                _mockGeneradorCodigo.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionNotificadorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SalasManejador(
                    null,
                    _mockAlmacenSalas.Object,
                    _mockProveedorContexto.Object,
                    _mockSalaFactoria.Object,
                    _mockGeneradorCodigo.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionAlmacenSalasNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SalasManejador(
                    _mockNotificadorSalas.Object,
                    null,
                    _mockProveedorContexto.Object,
                    _mockSalaFactoria.Object,
                    _mockGeneradorCodigo.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionProveedorContextoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SalasManejador(
                    _mockNotificadorSalas.Object,
                    _mockAlmacenSalas.Object,
                    null,
                    _mockSalaFactoria.Object,
                    _mockGeneradorCodigo.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionSalaFactoriaNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SalasManejador(
                    _mockNotificadorSalas.Object,
                    _mockAlmacenSalas.Object,
                    _mockProveedorContexto.Object,
                    null,
                    _mockGeneradorCodigo.Object));
        }

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionGeneradorCodigoNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new SalasManejador(
                    _mockNotificadorSalas.Object,
                    _mockAlmacenSalas.Object,
                    _mockProveedorContexto.Object,
                    _mockSalaFactoria.Object,
                    null));
        }

        [TestMethod]
        public void Prueba_ObtenerSalas_RetornaListaVaciaSinSalas()
        {
            _mockAlmacenSalas
                .Setup(almacen => almacen.Valores)
                .Returns(new System.Collections.Generic.List<SalaInternaManejador>());

            var salas = _manejador.ObtenerSalas();

            Assert.AreEqual(0, salas.Count);
        }

        [TestMethod]
        public void Prueba_ObtenerListaSalas_RetornaListaVaciaSinSalas()
        {
            _mockAlmacenSalas
                .Setup(almacen => almacen.Valores)
                .Returns(new System.Collections.Generic.List<SalaInternaManejador>());

            var salas = _manejador.ObtenerListaSalas();

            Assert.AreEqual(0, salas.Count);
        }

        [TestMethod]
        public void Prueba_AbandonarSala_LanzaExcepcionSalaNoEncontrada()
        {
            SalaInternaManejador salaOut;
            _mockAlmacenSalas
                .Setup(almacen => almacen.IntentarObtener(It.IsAny<string>(), out salaOut))
                .Returns(false);

            Assert.ThrowsException<FaultException>(
                () => _manejador.AbandonarSala(CodigoSalaPrueba, NombreJugadorPrueba));
        }

        [TestMethod]
        public void Prueba_UnirseSala_LanzaExcepcionSalaNoEncontrada()
        {
            SalaInternaManejador salaOut;
            _mockAlmacenSalas
                .Setup(almacen => almacen.IntentarObtener(It.IsAny<string>(), out salaOut))
                .Returns(false);
            _mockProveedorContexto
                .Setup(proveedor => proveedor.ObtenerCallbackChannel<ISalasManejadorCallback>())
                .Returns(new Mock<ISalasManejadorCallback>().Object);

            Assert.ThrowsException<FaultException>(
                () => _manejador.UnirseSala(CodigoSalaPrueba, NombreJugadorPrueba));
        }
    }
}
