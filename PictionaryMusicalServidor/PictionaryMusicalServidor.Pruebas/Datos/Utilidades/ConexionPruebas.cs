using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Datos.Utilidades
{
    /// <summary>
    /// Pruebas unitarias para la clase Conexion.
    /// Verifica la construccion de cadenas de conexion para Entity Framework.
    /// </summary>
    [TestClass]
    public class ConexionPruebas
    {
        private const string NombreVariableServidor = "BD_SERVIDOR";
        private const string NombreVariableUsuario = "BD_USUARIO";
        private const string NombreVariableContrasena = "BD_CONTRASENA";
        private const string ServidorLocalhost = "localhost";
        private const string ServidorEjemplo = "miservidor.ejemplo.com";
        private const string MetadataCsdl = "BaseDatosPictionaryMusical.csdl";
        private const string NombreCatalogo = "BaseDatosPrueba";
        private const string ProveedorSqlClient = "System.Data.SqlClient";

        private string _servidorOriginal;
        private string _usuarioOriginal;
        private string _contrasenaOriginal;

        [TestInitialize]
        public void Inicializar()
        {
            _servidorOriginal = Environment.GetEnvironmentVariable(NombreVariableServidor);
            _usuarioOriginal = Environment.GetEnvironmentVariable(NombreVariableUsuario);
            _contrasenaOriginal = Environment.GetEnvironmentVariable(NombreVariableContrasena);
        }

        [TestCleanup]
        public void Limpiar()
        {
            RestaurarVariableEntorno(NombreVariableServidor, _servidorOriginal);
            RestaurarVariableEntorno(NombreVariableUsuario, _usuarioOriginal);
            RestaurarVariableEntorno(NombreVariableContrasena, _contrasenaOriginal);
        }

        #region ObtenerConexion

        [TestMethod]
        public void Prueba_ObtenerConexionSinVariables_UsaLocalhost()
        {
            Environment.SetEnvironmentVariable(NombreVariableServidor, null);

            string conexion = Conexion.ObtenerConexion();

            StringAssert.Contains(conexion, ServidorLocalhost);
        }

        [TestMethod]
        public void Prueba_ObtenerConexionConServidor_UsaServidorConfigurado()
        {
            Environment.SetEnvironmentVariable(NombreVariableServidor, ServidorEjemplo);

            string conexion = Conexion.ObtenerConexion();

            StringAssert.Contains(conexion, ServidorEjemplo);
        }

        [TestMethod]
        public void Prueba_ObtenerConexionSiempre_ContieneMetadataEntityFramework()
        {
            string conexion = Conexion.ObtenerConexion();

            StringAssert.Contains(conexion, MetadataCsdl);
        }

        [TestMethod]
        public void Prueba_ObtenerConexionSiempre_ContieneCatalogoCorrecto()
        {
            string conexion = Conexion.ObtenerConexion();

            StringAssert.Contains(conexion, NombreCatalogo);
        }

        [TestMethod]
        public void Prueba_ObtenerConexionSiempre_UsaProveedorSqlClient()
        {
            string conexion = Conexion.ObtenerConexion();

            StringAssert.Contains(conexion, ProveedorSqlClient);
        }

        #endregion

        #region Metodos Auxiliares

        private static void RestaurarVariableEntorno(string nombre, string valorOriginal)
        {
            if (valorOriginal != null)
            {
                Environment.SetEnvironmentVariable(nombre, valorOriginal);
            }
            else
            {
                Environment.SetEnvironmentVariable(nombre, null);
            }
        }

        #endregion
    }
}
