using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using System;
using System.ServiceModel;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    [TestClass]
    public class ManejadorErrorServicioPruebas
    {
        private Mock<ILocalizadorServicio> _localizadorMock;
        private ManejadorErrorServicio _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _localizadorMock = new Mock<ILocalizadorServicio>();

            _localizadorMock
                .Setup(localizador => localizador.Localizar(It.IsAny<string>(), 
                It.IsAny<string>()))
                .Returns((string mensaje, string predeterminado) => 
                    string.IsNullOrWhiteSpace(mensaje) ? predeterminado : mensaje);

            _manejador = new ManejadorErrorServicio(_localizadorMock.Object);
        }

        [TestMethod]
        public void Prueba_Constructor_LocalizadorNulo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var manejador = new ManejadorErrorServicio(null);
            });
        }

        [TestMethod]
        public void Prueba_Constructor_LocalizadorValido_CreaInstancia()
        {
            var manejador = new ManejadorErrorServicio(_localizadorMock.Object);

            Assert.IsInstanceOfType(manejador, typeof(ManejadorErrorServicio));
        }

        [TestMethod]
        public void Prueba_ObtenerMensaje_ExcepcionNula_RetornaPredeterminado()
        {
            string mensajePredeterminado = "Error predeterminado";

            var resultado = _manejador.ObtenerMensaje(null, mensajePredeterminado);

            Assert.AreEqual(mensajePredeterminado, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerMensaje_ExcepcionConMensaje_RetornaMensajeLocalizado()
        {
            string mensajeOriginal = "Error del servidor";
            string mensajePredeterminado = "Error predeterminado";
            var excepcion = new FaultException(mensajeOriginal);

            _localizadorMock
                .Setup(localizador => localizador.Localizar(mensajeOriginal, 
                mensajePredeterminado))
                .Returns("Error localizado");

            var resultado = _manejador.ObtenerMensaje(excepcion, mensajePredeterminado);

            Assert.AreEqual("Error localizado", resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerMensaje_ExcepcionSinMensaje_RetornaPredeterminado()
        {
            string mensajePredeterminado = "Error predeterminado";
            var excepcion = new FaultException(string.Empty);

            var resultado = _manejador.ObtenerMensaje(excepcion, mensajePredeterminado);

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_ObtenerMensaje_InvocaLocalizador()
        {
            string mensajePredeterminado = "Error predeterminado";
            var excepcion = new FaultException("Error del servidor");

            _manejador.ObtenerMensaje(excepcion, mensajePredeterminado);

            _localizadorMock.Verify(
                l => l.Localizar(It.IsAny<string>(), It.IsAny<string>()),
                Times.AtLeastOnce);
        }

        [TestMethod]
        public void Prueba_ObtenerMensaje_ExcepcionNoGenerica_RetornaMensajeLocalizado()
        {
            string mensajeOriginal = "Falla en el servicio";
            string mensajePredeterminado = "Error predeterminado";
            var excepcion = new FaultException(mensajeOriginal);

            var resultado = _manejador.ObtenerMensaje(excepcion, mensajePredeterminado);

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }
    }
}
