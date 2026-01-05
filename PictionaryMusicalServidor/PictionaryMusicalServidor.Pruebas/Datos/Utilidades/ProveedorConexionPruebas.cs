using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Datos.Utilidades
{
    [TestClass]
    public class ProveedorConexionPruebas
    {
        private const string ProveedorEsperado = "System.Data.SqlClient";
        private const string MetadatosEsperados = "res://*/Modelo.BaseDatosPictionaryMusical";
        private const string CatalogoInicialEsperado = "BaseDatosPrueba";
        private const int LongitudMinimaConexion = 50;

        private ProveedorConexion _proveedor;

        [TestInitialize]
        public void Inicializar()
        {
            _proveedor = new ProveedorConexion();
        }

        #region Pruebas ObtenerConexion

        [TestMethod]
        public void Prueba_ObtenerConexion_RetornaCadenaNoVacia()
        {
            string conexion = _proveedor.ObtenerConexion();

            Assert.IsTrue(conexion.Length > LongitudMinimaConexion);
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ContieneProveedorSqlClient()
        {
            string conexion = _proveedor.ObtenerConexion();

            Assert.IsTrue(conexion.Contains(ProveedorEsperado));
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ContieneMetadatosModelo()
        {
            string conexion = _proveedor.ObtenerConexion();

            Assert.IsTrue(conexion.Contains(MetadatosEsperados));
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ContieneCatalogoInicial()
        {
            string conexion = _proveedor.ObtenerConexion();

            Assert.IsTrue(conexion.Contains(CatalogoInicialEsperado));
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_LlamadasConsecutivasRetornanMismaEstructura()
        {
            string conexion1 = _proveedor.ObtenerConexion();
            string conexion2 = _proveedor.ObtenerConexion();

            Assert.AreEqual(conexion1, conexion2);
        }

        #endregion
    }
}
