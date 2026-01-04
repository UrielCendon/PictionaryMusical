using System;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Datos
{
    /// <summary>
    /// Contiene pruebas unitarias para la clase <see cref="Conexion"/>.
    /// Valida la construccion correcta de cadenas de conexion a partir de variables de entorno.
    /// </summary>
    [TestClass]
    public class ConexionPruebas
    {
        private const string VariableServidor = "BD_SERVIDOR";
        private const string VariableUsuario = "BD_USUARIO";
        private const string VariableContrasena = "BD_CONTRASENA";
        private const string ValorServidor = "ServidorPrueba";
        private const string ValorUsuario = "UsuarioPrueba";
        private const string ValorContrasena = "ContrasenaPrueba";
        private const string ValorLocalhost = "localhost";
        private const string NombreCatalogo = "BaseDatosPrueba";
        private const string ProveedorDatos = "System.Data.SqlClient";
        private const string MetadatosModelo = 
            "res://*/Modelo.BaseDatosPictionaryMusical.csdl|" +
            "res://*/Modelo.BaseDatosPictionaryMusical.ssdl|" +
            "res://*/Modelo.BaseDatosPictionaryMusical.msl";

        [TestCleanup]
        public void LimpiarVariablesEntorno()
        {
            Environment.SetEnvironmentVariable(VariableServidor, null);
            Environment.SetEnvironmentVariable(VariableUsuario, null);
            Environment.SetEnvironmentVariable(VariableContrasena, null);
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_VariablesConfiguradasRetornaCadenaCorrecta()
        {
            Environment.SetEnvironmentVariable(VariableServidor, ValorServidor);
            Environment.SetEnvironmentVariable(VariableUsuario, ValorUsuario);
            Environment.SetEnvironmentVariable(VariableContrasena, ValorContrasena);

            var resultado = Conexion.ObtenerConexion();
            var constructorEntidad = new EntityConnectionStringBuilder(resultado);
            var cadenaProveedor = constructorEntidad.ProviderConnectionString;
            var constructorSql = new SqlConnectionStringBuilder(cadenaProveedor);

            Assert.AreEqual(ProveedorDatos, constructorEntidad.Provider);
            Assert.AreEqual(MetadatosModelo, constructorEntidad.Metadata);
            Assert.AreEqual(ValorServidor, constructorSql.DataSource);
            Assert.AreEqual(NombreCatalogo, constructorSql.InitialCatalog);
            Assert.AreEqual(ValorUsuario, constructorSql.UserID);
            Assert.AreEqual(ValorContrasena, constructorSql.Password);
            Assert.IsTrue(constructorSql.MultipleActiveResultSets);
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ServidorNoConfiguradoUsaLocalhost()
        {
            Environment.SetEnvironmentVariable(VariableServidor, null);
            Environment.SetEnvironmentVariable(VariableUsuario, ValorUsuario);
            Environment.SetEnvironmentVariable(VariableContrasena, ValorContrasena);

            var resultado = Conexion.ObtenerConexion();
            var constructorEntidad = new EntityConnectionStringBuilder(resultado);
            var cadenaProveedor = constructorEntidad.ProviderConnectionString;
            var constructorSql = new SqlConnectionStringBuilder(cadenaProveedor);

            Assert.AreEqual(ValorLocalhost, constructorSql.DataSource);
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_CredencialesNulasLanzaExcepcion()
        {
            Environment.SetEnvironmentVariable(VariableServidor, ValorServidor);
            Environment.SetEnvironmentVariable(VariableUsuario, null);
            Environment.SetEnvironmentVariable(VariableContrasena, null);

            Assert.ThrowsException<ArgumentNullException>(() => Conexion.ObtenerConexion());
        }
    }
}