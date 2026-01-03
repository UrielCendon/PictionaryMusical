using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Utilidades;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias requeridas por VerificacionCodigoVistaModelo.
    /// </summary>
    public sealed class VerificacionCodigoDependencias
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="VerificacionCodigoDependencias"/>.
        /// </summary>
        /// <param name="descripcion">Descripcion del dialogo de verificacion.</param>
        /// <param name="tokenCodigo">Token del codigo de verificacion.</param>
        /// <param name="codigoVerificacionServicio">Servicio de verificacion de codigos.</param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        public VerificacionCodigoDependencias(
            string descripcion,
            string tokenCodigo,
            ICodigoVerificacionServicio codigoVerificacionServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
        {
            Descripcion = descripcion ??
                throw new ArgumentNullException(nameof(descripcion));
            TokenCodigo = tokenCodigo ??
                throw new ArgumentNullException(nameof(tokenCodigo));
            CodigoVerificacionServicio = codigoVerificacionServicio ??
                throw new ArgumentNullException(nameof(codigoVerificacionServicio));
            AvisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            SonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
        }

        /// <summary>
        /// Obtiene la descripcion del dialogo de verificacion.
        /// </summary>
        public string Descripcion { get; }

        /// <summary>
        /// Obtiene el token del codigo de verificacion.
        /// </summary>
        public string TokenCodigo { get; }

        /// <summary>
        /// Obtiene el servicio de verificacion de codigos.
        /// </summary>
        public ICodigoVerificacionServicio CodigoVerificacionServicio { get; }

        /// <summary>
        /// Obtiene el servicio de avisos.
        /// </summary>
        public IAvisoServicio AvisoServicio { get; }

        /// <summary>
        /// Obtiene el manejador de sonidos.
        /// </summary>
        public SonidoManejador SonidoManejador { get; }
    }
}
