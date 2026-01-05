using System;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace PictionaryMusicalServidor.Datos.Utilidades
{
    /// <summary>
    /// Implementacion del proveedor de conexion que utiliza variables de entorno.
    /// </summary>
    public class ProveedorConexion : IProveedorConexion
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

        /// <inheritdoc/>
        public string ObtenerConexion()
        {
            var constructorSql = new SqlConnectionStringBuilder
            {
                DataSource = Environment.GetEnvironmentVariable(VariableEntornoServidor) 
                    ?? ServidorPorDefecto,
                InitialCatalog = NombreBaseDatos,
                UserID = Environment.GetEnvironmentVariable(VariableEntornoUsuario),
                Password = Environment.GetEnvironmentVariable(VariableEntornoContrasena),
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
