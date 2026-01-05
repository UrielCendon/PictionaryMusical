using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.Utilidades;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Data;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Utilidades
{
    [TestClass]
    public class ContextoFactoriaPruebas
    {
        private const string CadenaConexionVacia = "";
        private const string CadenaConexionNula = null;
        private const string CadenaConexionEspacios = "   ";

        private Mock<IProveedorConexion> _proveedorConexionMock;

        [TestInitialize]
        public void Inicializar()
        {
            _proveedorConexionMock = new Mock<IProveedorConexion>();
        }

        [TestMethod]
        public void Prueba_Constructor_ProveedorConexionNulo_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ContextoFactoria(null));
        }

        [TestMethod]
        public void Prueba_Constructor_ProveedorConexionValido_CreaInstancia()
        {
            var factoria = new ContextoFactoria(_proveedorConexionMock.Object);

            Assert.IsInstanceOfType(factoria, typeof(IContextoFactoria));
        }

        [TestMethod]
        public void Prueba_Constructor_SinParametros_CreaInstanciaConProveedorPorDefecto()
        {
            var factoria = new ContextoFactoria();

            Assert.IsInstanceOfType(factoria, typeof(IContextoFactoria));
        }

        [TestMethod]
        public void Prueba_CrearContexto_ConexionVacia_RetornaContextoPorDefecto()
        {
            _proveedorConexionMock
                .Setup(proveedor => proveedor.ObtenerConexion())
                .Returns(CadenaConexionVacia);
            var factoria = new ContextoFactoria(_proveedorConexionMock.Object);

            using (var contexto = factoria.CrearContexto())
            {
                Assert.IsNotNull(contexto);
            }
        }

        [TestMethod]
        public void Prueba_CrearContexto_ConexionNula_RetornaContextoPorDefecto()
        {
            _proveedorConexionMock
                .Setup(proveedor => proveedor.ObtenerConexion())
                .Returns(CadenaConexionNula);
            var factoria = new ContextoFactoria(_proveedorConexionMock.Object);

            using (var contexto = factoria.CrearContexto())
            {
                Assert.IsNotNull(contexto);
            }
        }

        [TestMethod]
        public void Prueba_CrearContexto_ConexionSoloEspacios_RetornaContextoPorDefecto()
        {
            _proveedorConexionMock
                .Setup(proveedor => proveedor.ObtenerConexion())
                .Returns(CadenaConexionEspacios);
            var factoria = new ContextoFactoria(_proveedorConexionMock.Object);

            using (var contexto = factoria.CrearContexto())
            {
                Assert.IsNotNull(contexto);
            }
        }

        [TestMethod]
        public void Prueba_CrearContexto_ProveedorLanzaDataException_PropagaDataException()
        {
            _proveedorConexionMock
                .Setup(proveedor => proveedor.ObtenerConexion())
                .Throws(new DataException("Error de datos"));
            var factoria = new ContextoFactoria(_proveedorConexionMock.Object);

            Assert.ThrowsException<DataException>(() => factoria.CrearContexto());
        }

        [TestMethod]
        public void Prueba_CrearContexto_ProveedorLanzaExcepcionGenerica_PropagaDataException()
        {
            _proveedorConexionMock
                .Setup(proveedor => proveedor.ObtenerConexion())
                .Throws(new InvalidOperationException("Error inesperado"));
            var factoria = new ContextoFactoria(_proveedorConexionMock.Object);

            Assert.ThrowsException<DataException>(() => factoria.CrearContexto());
        }

        [TestMethod]
        public void Prueba_CrearContexto_InvocaProveedorConexion()
        {
            _proveedorConexionMock
                .Setup(proveedor => proveedor.ObtenerConexion())
                .Returns(CadenaConexionVacia);
            var factoria = new ContextoFactoria(_proveedorConexionMock.Object);

            factoria.CrearContexto();

            _proveedorConexionMock.Verify(
                proveedor => proveedor.ObtenerConexion(), 
                Times.Once);
        }
    }
}
