namespace PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares
{
    /// <summary>
    /// Contiene informacion del contexto de sala para 
    /// <see cref="SalaJugadoresManejador"/>.
    /// </summary>
    public sealed class ContextoSalaJugador
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ContextoSalaJugador"/>.
        /// </summary>
        public ContextoSalaJugador(
            string nombreUsuarioSesion,
            string creadorSala,
            string codigoSala,
            bool esHost,
            bool esInvitado)
        {
            NombreUsuarioSesion = nombreUsuarioSesion ?? string.Empty;
            CreadorSala = creadorSala ?? string.Empty;
            CodigoSala = codigoSala ?? string.Empty;
            EsHost = esHost;
            EsInvitado = esInvitado;
        }

        /// <summary>
        /// Nombre del usuario en sesion.
        /// </summary>
        public string NombreUsuarioSesion { get; }

        /// <summary>
        /// Nombre del creador de la sala.
        /// </summary>
        public string CreadorSala { get; }

        /// <summary>
        /// Codigo de la sala.
        /// </summary>
        public string CodigoSala { get; }

        /// <summary>
        /// Indica si el usuario es el host de la sala.
        /// </summary>
        public bool EsHost { get; }

        /// <summary>
        /// Indica si el usuario es un invitado.
        /// </summary>
        public bool EsInvitado { get; }
    }
}
