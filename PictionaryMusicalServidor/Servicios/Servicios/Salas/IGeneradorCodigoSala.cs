namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Interfaz para la generacion de codigos de sala.
    /// Permite abstraer la generacion para facilitar pruebas unitarias.
    /// </summary>
    public interface IGeneradorCodigoSala
    {
        /// <summary>
        /// Genera un codigo unico para una sala.
        /// </summary>
        /// <returns>Codigo generado.</returns>
        string GenerarCodigo();
    }
}
