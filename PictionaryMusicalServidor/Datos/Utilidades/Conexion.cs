using System;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace PictionaryMusicalServidor.Datos.Utilidades
{
    /// <summary>
    /// Clase de utilidad para la gestion y construccion de cadenas de conexion.
    /// </summary>
    public static class Conexion
    {
        private const string ServidorPorDefecto = "localhost";
        private const string NombreBaseDatos = "BaseDatosPrueba";
        private const string ProveedorSql = "System.Data.SqlClient";
        private const string MetadatosModelo = 
            "res://*/Modelo.BaseDatosPictionaryMusical.csdl|" +
            "res://*/Modelo.BaseDatosPictionaryMusical.ssdl|" +
            "res://*/Modelo.BaseDatosPictionaryMusical.msl";
        private const string VariableEntornoServidor = "BD_SERVIDOR";
        private const string VariableEntornoUsuario = "BD_USUARIO";
        private const string VariableEntornoContrasena = "BD_CONTRASENA";

        /// <summary>
        /// Construye y obtiene la cadena de conexion completa para Entity Framework.
        /// Utiliza variables de entorno para las credenciales y el servidor.
        /// </summary>
        /// <returns>Cadena de conexion formateada para EntityClient.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si las credenciales no estan 
        /// configuradas.</exception>
        public static string ObtenerConexion()
        {
            string usuario = Environment.GetEnvironmentVariable(VariableEntornoUsuario);
            string contrasena = Environment.GetEnvironmentVariable(VariableEntornoContrasena);

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                throw new InvalidOperationException(
                    "Las credenciales de base de datos no estan configuradas en el entorno.");
            }

            var constructorSql = new SqlConnectionStringBuilder
            {
                DataSource = Environment.GetEnvironmentVariable(VariableEntornoServidor) 
                    ?? ServidorPorDefecto,
                InitialCatalog = NombreBaseDatos,
                UserID = usuario,
                Password = contrasena,
                MultipleActiveResultSets = true
            };

            var constructorEntidad = new EntityConnectionStringBuilder
            {
                Provider = ProveedorSql,
                ProviderConnectionString = constructorSql.ToString(),
                Metadata = MetadatosModelo
            };

            return constructorEntidad.ToString();
        }
    }
}
