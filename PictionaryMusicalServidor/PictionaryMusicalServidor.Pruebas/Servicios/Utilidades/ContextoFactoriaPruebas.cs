using System;
using Datos.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PictionaryMusicalServidor.Datos.Utilidades;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Utilidades
{
    [TestClass]
    public class ContextoFactoriaPruebas
    {
        private const string CadenaConexionValida = "metadata=res://*/Modelo.BaseDatosPictionaryMusical.csdl|res://*/Modelo.BaseDatosPictionaryMusical.ssdl|res://*/Modelo.BaseDatosPictionaryMusical.msl;provider=System.Data.SqlClient;provider connection string=\"data source=localhost;initial catalog=TestDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework\"";

        [TestMethod]
        public void Prueba_Constructor_LanzaExcepcionProveedorNulo()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ContextoFactoria(null));
        }

        [TestMethod]
        public void Prueba_Constructor_NoLanzaExcepcionProveedorValido()
        {
            var mockProveedor = new Mock<IProveedorConexion>();

            var factoria = new ContextoFactoria(mockProveedor.Object);

            Assert.IsInstanceOfType(factoria, typeof(ContextoFactoria));
        }

        [TestMethod]
        public void Prueba_CrearContexto_RetornaContextoCuandoConexionVacia()
        {
            var mockProveedor = new Mock<IProveedorConexion>();
            mockProveedor.Setup(proveedor => proveedor.ObtenerConexion())
                .Returns(string.Empty);
            var factoria = new ContextoFactoria(mockProveedor.Object);

            var contexto = factoria.CrearContexto();

            Assert.IsInstanceOfType(contexto, typeof(BaseDatosPruebaEntities));
            contexto.Dispose();
        }

        [TestMethod]
        public void Prueba_CrearContexto_RetornaContextoCuandoConexionNula()
        {
            var mockProveedor = new Mock<IProveedorConexion>();
            mockProveedor.Setup(proveedor => proveedor.ObtenerConexion())
                .Returns((string)null);
            var factoria = new ContextoFactoria(mockProveedor.Object);

            var contexto = factoria.CrearContexto();

            Assert.IsInstanceOfType(contexto, typeof(BaseDatosPruebaEntities));
            contexto.Dispose();
        }
    }
}
