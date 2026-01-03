using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Auxiliares
{
    /// <summary>
    /// Agrupa los parámetros necesarios para navegar a una sala de juego.
    /// </summary>
    public sealed class NavegacionSalaParametros
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="NavegacionSalaParametros"/>.
        /// </summary>
        /// <param name="sala">Datos de la sala.</param>
        /// <param name="servicio">Servicio de salas.</param>
        /// <param name="nombreJugador">Nombre del jugador.</param>
        /// <param name="esInvitado">Indica si es invitado.</param>
        /// <param name="vistaModeloActual">VistaModelo actual para cerrar.</param>
        /// <exception cref="ArgumentNullException">
        /// Si la sala o el servicio son nulos.
        /// </exception>
        public NavegacionSalaParametros(
            DTOs.SalaDTO sala,
            ISalasServicio servicio,
            string nombreJugador,
            bool esInvitado,
            object vistaModeloActual)
        {
            Sala = sala ?? throw new ArgumentNullException(nameof(sala));
            Servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
            NombreJugador = nombreJugador ?? string.Empty;
            EsInvitado = esInvitado;
            VistaModeloActual = vistaModeloActual;
        }

        /// <summary>
        /// Obtiene los datos de la sala.
        /// </summary>
        public DTOs.SalaDTO Sala { get; }

        /// <summary>
        /// Obtiene el servicio de salas.
        /// </summary>
        public ISalasServicio Servicio { get; }

        /// <summary>
        /// Obtiene el nombre del jugador.
        /// </summary>
        public string NombreJugador { get; }

        /// <summary>
        /// Obtiene un valor que indica si es invitado.
        /// </summary>
        public bool EsInvitado { get; }

        /// <summary>
        /// Obtiene el VistaModelo actual para cerrar.
        /// </summary>
        public object VistaModeloActual { get; }
    }
}
