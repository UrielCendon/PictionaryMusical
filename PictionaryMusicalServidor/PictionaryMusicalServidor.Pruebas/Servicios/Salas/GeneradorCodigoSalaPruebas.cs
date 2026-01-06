using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Servicios.Servicios.Salas;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Salas
{
    [TestClass]
    public class GeneradorCodigoSalaPruebas
    {
        private const int LongitudCodigoEsperada = 6;

        private Mock<IAlmacenSalas> _mockAlmacenSalas;
        private GeneradorCodigoSala _generador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockAlmacenSalas = new Mock<IAlmacenSalas>();
            _generador = new GeneradorCodigoSala(_mockAlmacenSalas.Object);
        }

        [TestMethod]
        public void Prueba_GenerarCodigo_RetornaCodigoNoVacio()
        {
            _mockAlmacenSalas
                .Setup(almacen => almacen.ContieneCodigo(It.IsAny<string>()))
                .Returns(false);

            var codigo = _generador.GenerarCodigo();

            Assert.IsFalse(string.IsNullOrEmpty(codigo));
        }

        [TestMethod]
        public void Prueba_GenerarCodigo_RetornaCodigoConLongitudCorrecta()
        {
            _mockAlmacenSalas
                .Setup(almacen => almacen.ContieneCodigo(It.IsAny<string>()))
                .Returns(false);

            var codigo = _generador.GenerarCodigo();

            Assert.AreEqual(LongitudCodigoEsperada, codigo.Length);
        }

        [TestMethod]
        public void Prueba_GenerarCodigo_VerificaExistenciaEnAlmacen()
        {
            _mockAlmacenSalas
                .Setup(almacen => almacen.ContieneCodigo(It.IsAny<string>()))
                .Returns(false);

            _generador.GenerarCodigo();

            _mockAlmacenSalas.Verify(
                almacen => almacen.ContieneCodigo(It.IsAny<string>()),
                Times.AtLeastOnce);
        }

        [TestMethod]
        public void Prueba_GenerarCodigo_RegeneraCodigoSiYaExiste()
        {
            var llamadas = 0;
            _mockAlmacenSalas
                .Setup(almacen => almacen.ContieneCodigo(It.IsAny<string>()))
                .Returns(() =>
                {
                    llamadas++;
                    return llamadas < 3;
                });

            _generador.GenerarCodigo();

            _mockAlmacenSalas.Verify(
                almacen => almacen.ContieneCodigo(It.IsAny<string>()),
                Times.AtLeast(3));
        }

        [TestMethod]
        public void Prueba_GenerarCodigo_LanzaExcepcionSiNoEncuentraCodigoUnico()
        {
            _mockAlmacenSalas
                .Setup(almacen => almacen.ContieneCodigo(It.IsAny<string>()))
                .Returns(true);

            Assert.ThrowsException<FaultException>(
                () => _generador.GenerarCodigo());
        }
    }
}
