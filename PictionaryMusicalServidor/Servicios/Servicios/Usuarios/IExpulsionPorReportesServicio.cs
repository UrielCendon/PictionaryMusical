namespace PictionaryMusicalServidor.Servicios.Servicios.Usuarios
{
    /// <summary>
    /// Interfaz para el servicio de expulsion de jugadores por limite de reportes.
    /// </summary>
    public interface IExpulsionPorReportesServicio
    {
        /// <summary>
        /// Expulsa a un jugador de todas las salas activas si alcanza el limite de reportes.
        /// </summary>
        /// <param name="idReportado">Identificador del usuario reportado.</param>
        /// <param name="nombreUsuarioReportado">Nombre del usuario reportado.</param>
        /// <param name="totalReportes">Total de reportes recibidos.</param>
        void ExpulsarSiAlcanzaLimite(
            int idReportado, 
            string nombreUsuarioReportado, 
            int totalReportes);
    }
}
