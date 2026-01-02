namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Contiene los identificadores de usuarios involucrados en un reporte.
    /// </summary>
    public class IdentificadoresUsuarios
    {
        /// <summary>
        /// Identificador del usuario que realiza el reporte.
        /// </summary>
        public int IdReportante { get; set; }

        /// <summary>
        /// Identificador del usuario que es reportado.
        /// </summary>
        public int IdReportado { get; set; }
    }
}
