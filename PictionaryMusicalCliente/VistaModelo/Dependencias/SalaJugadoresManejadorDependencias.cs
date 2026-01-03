using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Utilidades;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Contiene las dependencias de servicios para 
    /// <see cref="SalaJugadoresManejador"/>.
    /// </summary>
    public sealed class SalaJugadoresManejadorDependencias
    {
        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="SalaJugadoresManejadorDependencias"/>.
        /// </summary>
        public SalaJugadoresManejadorDependencias(
            ISalasServicio salasServicio,
            IReportesServicio reportesServicio,
            SonidoManejador sonidoManejador,
            IAvisoServicio avisoServicio,
            ILocalizadorServicio localizadorServicio)
        {
            SalasServicio = salasServicio ?? 
                throw new ArgumentNullException(nameof(salasServicio));
            ReportesServicio = reportesServicio ?? 
                throw new ArgumentNullException(nameof(reportesServicio));
            SonidoManejador = sonidoManejador ?? 
                throw new ArgumentNullException(nameof(sonidoManejador));
            AvisoServicio = avisoServicio ?? 
                throw new ArgumentNullException(nameof(avisoServicio));
            LocalizadorServicio = localizadorServicio ??
                throw new ArgumentNullException(nameof(localizadorServicio));
        }

        /// <summary>
        /// Servicio para operaciones de salas.
        /// </summary>
        public ISalasServicio SalasServicio { get; }

        /// <summary>
        /// Servicio para operaciones de reportes.
        /// </summary>
        public IReportesServicio ReportesServicio { get; }

        /// <summary>
        /// Manejador de sonidos.
        /// </summary>
        public SonidoManejador SonidoManejador { get; }

        /// <summary>
        /// Servicio para mostrar avisos.
        /// </summary>
        public IAvisoServicio AvisoServicio { get; }

        /// <summary>
        /// Servicio de localizacion de mensajes.
        /// </summary>
        public ILocalizadorServicio LocalizadorServicio { get; }
    }
}
