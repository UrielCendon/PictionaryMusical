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
            string usuario = Environment.GetEnvironmentVariable(VariableEntornoUsuario);
            string contrasena = Environment.GetEnvironmentVariable(VariableEntornoContrasena);

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                // Se permite continuar si no hay credenciales, asumiendo que podria usarse 
                // seguridad integrada o que el manejo de errores ocurrira al intentar conectar.
                // Sin embargo, para consistencia con la clase estatica Conexion, se podria validar.
                // Dado que esta clase implementa una interfaz, el comportamiento depende del contrato.
                // Por ahora mantenemos el comportamiento original pero aseguramos que no falle 
                // la construccion del builder.
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
