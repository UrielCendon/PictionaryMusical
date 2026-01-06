using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares
{
    /// <summary>
    /// Encapsula los parametros necesarios para crear un GestorSesionPartida.
    /// </summary>
    public sealed class GestorSesionPartidaParametros
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GestorSesionPartidaParametros"/>.
        /// </summary>
        /// <param name="fabricaClientes">Fabrica de clientes WCF.</param>
        /// <param name="codigoSala">Codigo de la sala.</param>
        /// <param name="idJugador">Identificador del jugador.</param>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        /// <param name="esHost">Indica si es el anfitrion.</param>
        public GestorSesionPartidaParametros(
            IWcfClienteFabrica fabricaClientes,
            string codigoSala,
            string idJugador,
            string nombreUsuario,
            bool esHost)
        {
            FabricaClientes = fabricaClientes 
                ?? throw new ArgumentNullException(nameof(fabricaClientes));
            CodigoSala = codigoSala 
                ?? throw new ArgumentNullException(nameof(codigoSala));
            IdJugador = idJugador 
                ?? throw new ArgumentNullException(nameof(idJugador));
            NombreUsuario = nombreUsuario ?? string.Empty;
            EsHost = esHost;
        }

        /// <summary>
        /// Fabrica para crear clientes WCF.
        /// </summary>
        public IWcfClienteFabrica FabricaClientes { get; }

        /// <summary>
        /// Codigo de la sala de juego.
        /// </summary>
        public string CodigoSala { get; }

        /// <summary>
        /// Identificador unico del jugador.
        /// </summary>
        public string IdJugador { get; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string NombreUsuario { get; }

        /// <summary>
        /// Indica si el jugador es el anfitrion de la sala.
        /// </summary>
        public bool EsHost { get; }
    }
}
