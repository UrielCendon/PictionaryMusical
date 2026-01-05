namespace PictionaryMusicalServidor.Datos.Utilidades
{
    /// <summary>
    /// Interfaz para proveer cadenas de conexion a la base de datos.
    /// Permite abstraer la obtencion de conexiones para facilitar pruebas unitarias.
    /// </summary>
    public interface IProveedorConexion
    {
        /// <summary>
        /// Obtiene la cadena de conexion para la base de datos.
        /// </summary>
        /// <returns>Cadena de conexion formateada.</returns>
        string ObtenerConexion();
    }
}
