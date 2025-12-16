using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias comunes requeridas por todos los ViewModels.
    /// </summary>
    /// <remarks>
    /// Incluye servicios de UI basicos como ventana, localizacion, sonido y avisos.
    /// </remarks>
    public class DependenciasVistaModeloBase
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias comunes.
        /// </summary>
        /// <param name="ventana">Servicio para gestionar ventanas.</param>
        /// <param name="localizador">Servicio de localizacion de mensajes.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <param name="avisoServicio">Servicio para mostrar avisos al usuario.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        public DependenciasVistaModeloBase(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            SonidoManejador sonidoManejador,
            IAvisoServicio avisoServicio)
        {
            Ventana = ventana ?? throw new ArgumentNullException(nameof(ventana));
            Localizador = localizador ?? 
                throw new ArgumentNullException(nameof(localizador));
            SonidoManejador = sonidoManejador ?? 
                throw new ArgumentNullException(nameof(sonidoManejador));
            AvisoServicio = avisoServicio ?? 
                throw new ArgumentNullException(nameof(avisoServicio));
        }

        /// <summary>
        /// Servicio para gestionar ventanas y dialogos.
        /// </summary>
        public IVentanaServicio Ventana { get; }

        /// <summary>
        /// Servicio para localizar mensajes y recursos.
        /// </summary>
        public ILocalizadorServicio Localizador { get; }

        /// <summary>
        /// Manejador de efectos de sonido de la aplicacion.
        /// </summary>
        public SonidoManejador SonidoManejador { get; }

        /// <summary>
        /// Servicio para mostrar avisos y notificaciones al usuario.
        /// </summary>
        public IAvisoServicio AvisoServicio { get; }
    }
}
