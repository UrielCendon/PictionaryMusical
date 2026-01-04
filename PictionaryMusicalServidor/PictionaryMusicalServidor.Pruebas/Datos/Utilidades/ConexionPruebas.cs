using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Utilidades
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase Conexion.
    /// Verifica flujos normales, alternos y de excepcion para la construccion de cadenas de conexion.
    /// </summary>
    [TestClass]
    public class ConexionPruebas
    {
        private string _servidorOriginal;
        private string _usuarioOriginal;
        private string _contrasenaOriginal;

        /// <summary>
        /// Guarda las variables de entorno originales antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _servidorOriginal = Environment.GetEnvironmentVariable("BD_SERVIDOR");
            _usuarioOriginal = Environment.GetEnvironmentVariable("BD_USUARIO");
            _contrasenaOriginal = Environment.GetEnvironmentVariable("BD_CONTRASENA");
        }

        /// <summary>
        /// Restaura las variables de entorno originales despues de cada prueba.
        /// </summary>
        [TestCleanup]
        public void Limpiar()
        {
            RestaurarVariableEntorno("BD_SERVIDOR", _servidorOriginal);
            RestaurarVariableEntorno("BD_USUARIO", _usuarioOriginal);
            RestaurarVariableEntorno("BD_CONTRASENA", _contrasenaOriginal);
        }

        #region ObtenerConexion - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerConexion_ConVariablesEntorno_RetornaCadenaValida()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor-test");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario-test");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "contrasena-test");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Length > 0);
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConVariablesEntorno_ContieneServidor()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "mi-servidor-sql");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsTrue(resultado.Contains("mi-servidor-sql"), 
                "La cadena de conexion debe contener el servidor especificado");
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConVariablesEntorno_ContieneBaseDatos()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsTrue(resultado.Contains("BaseDatosPrueba"), 
                "La cadena de conexion debe contener el nombre de la base de datos");
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConVariablesEntorno_ContieneUsuario()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario-especifico");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsTrue(resultado.Contains("usuario-especifico"), 
                "La cadena de conexion debe contener el usuario especificado");
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConVariablesEntorno_ContieneMetadata()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsTrue(resultado.Contains("metadata="), 
                "La cadena de conexion debe contener la informacion de metadata");
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConVariablesEntorno_ContieneProvider()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsTrue(resultado.Contains("provider=System.Data.SqlClient"), 
                "La cadena de conexion debe especificar el provider SqlClient");
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConVariablesEntorno_ContieneMars()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsTrue(resultado.Contains("MultipleActiveResultSets=True"), 
                "La cadena de conexion debe tener MARS habilitado");
        }

        #endregion

        #region ObtenerConexion - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerConexion_SinVariableServidor_UsaLocalhost()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", null);
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsTrue(resultado.Contains("localhost"), 
                "Sin BD_SERVIDOR debe usar localhost como valor predeterminado");
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_SinUsuario_LanzaArgumentNullException()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", null);
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Conexion.ObtenerConexion();
            });
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_SinContrasena_LanzaArgumentNullException()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", null);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Conexion.ObtenerConexion();
            });
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_SinNingunaVariable_LanzaArgumentNullException()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", null);
            Environment.SetEnvironmentVariable("BD_USUARIO", null);
            Environment.SetEnvironmentVariable("BD_CONTRASENA", null);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Conexion.ObtenerConexion();
            });
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConServidorVacio_UsaVacio()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_MultiplesLlamadas_RetornaMismaCadena()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor-consistente");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado1 = Conexion.ObtenerConexion();
            string resultado2 = Conexion.ObtenerConexion();

            Assert.AreEqual(resultado1, resultado2, 
                "Multiples llamadas deben retornar la misma cadena de conexion");
        }

        #endregion

        #region ObtenerConexion - Casos Especiales

        [TestMethod]
        public void Prueba_ObtenerConexion_ConCaracteresEspecialesEnServidor_GeneraCadena()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor\\instancia");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Contains("servidor\\instancia") || 
                          resultado.Contains("servidor\\\\instancia"));
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConPuertoEnServidor_GeneraCadena()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor,1433");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_ConEspaciosEnCredenciales_GeneraCadena()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario con espacios");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password con espacios");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerConexion_RetornaFormatoEntityConnection()
        {
            Environment.SetEnvironmentVariable("BD_SERVIDOR", "servidor");
            Environment.SetEnvironmentVariable("BD_USUARIO", "usuario");
            Environment.SetEnvironmentVariable("BD_CONTRASENA", "password");

            string resultado = Conexion.ObtenerConexion();

            Assert.IsTrue(resultado.StartsWith("metadata=") || resultado.Contains("metadata="), 
                "La cadena debe tener formato de EntityConnection");
            Assert.IsTrue(resultado.Contains("provider="), 
                "La cadena debe contener la especificacion del provider");
            Assert.IsTrue(resultado.Contains("provider connection string="), 
                "La cadena debe contener la cadena de conexion del provider");
        }

        #endregion

        private void RestaurarVariableEntorno(string nombre, string valor)
        {
            if (valor != null)
            {
                Environment.SetEnvironmentVariable(nombre, valor);
            }
            else
            {
                Environment.SetEnvironmentVariable(nombre, null);
            }
        }
    }
}
