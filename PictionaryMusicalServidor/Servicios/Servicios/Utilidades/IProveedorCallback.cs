namespace PictionaryMusicalServidor.Servicios.Servicios.Utilidades
{
    /// <summary>
    /// Interfaz generica para obtener el callback actual del contexto de operacion WCF.
    /// Permite abstraer la obtencion de callbacks para facilitar pruebas unitarias.
    /// </summary>
    /// <typeparam name="T">Tipo de callback a obtener.</typeparam>
    public interface IProveedorCallback<out T> where T : class
    {
        /// <summary>
        /// Obtiene el callback actual del contexto de operacion.
        /// </summary>
        /// <returns>El callback del contexto actual.</returns>
        T ObtenerCallbackActual();
    }
}
