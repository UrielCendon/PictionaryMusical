using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias requeridas por IngresoPartidaInvitadoVistaModelo.
    /// </summary>
    public sealed class IngresoPartidaInvitadoDependencias
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="IngresoPartidaInvitadoDependencias"/>.
        /// </summary>
        /// <param name="localizacionServicio">Servicio de localizacion cultural.</param>
        /// <param name="salasServicio">Servicio de salas.</param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        /// <param name="nombreInvitadoGenerador">Generador de nombres para invitados.</param>
        public IngresoPartidaInvitadoDependencias(
            ILocalizacionServicio localizacionServicio,
            ISalasServicio salasServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador,
            INombreInvitadoGenerador nombreInvitadoGenerador)
        {
            LocalizacionServicio = localizacionServicio ??
                throw new ArgumentNullException(nameof(localizacionServicio));
            SalasServicio = salasServicio ??
                throw new ArgumentNullException(nameof(salasServicio));
            AvisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            SonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            NombreInvitadoGenerador = nombreInvitadoGenerador ??
                throw new ArgumentNullException(nameof(nombreInvitadoGenerador));
        }

        /// <summary>
        /// Obtiene el servicio de localizacion cultural.
        /// </summary>
        public ILocalizacionServicio LocalizacionServicio { get; }

        /// <summary>
        /// Obtiene el servicio de salas.
        /// </summary>
        public ISalasServicio SalasServicio { get; }

        /// <summary>
        /// Obtiene el servicio de avisos.
        /// </summary>
        public IAvisoServicio AvisoServicio { get; }

        /// <summary>
        /// Obtiene el manejador de sonidos.
        /// </summary>
        public SonidoManejador SonidoManejador { get; }

        /// <summary>
        /// Obtiene el generador de nombres para invitados.
        /// </summary>
        public INombreInvitadoGenerador NombreInvitadoGenerador { get; }
    }
}
