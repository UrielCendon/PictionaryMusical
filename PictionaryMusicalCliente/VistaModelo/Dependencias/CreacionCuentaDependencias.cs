using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo.Catalogos;
using System;

namespace PictionaryMusicalCliente.VistaModelo.Dependencias
{
    /// <summary>
    /// Agrupa las dependencias de servicios para la creacion de cuenta.
    /// </summary>
    /// <remarks>
    /// Incluye servicios de verificacion, cuenta, avatar y localizacion.
    /// </remarks>
    public class CreacionCuentaDependencias
    {
        /// <summary>
        /// Inicializa una nueva instancia con las dependencias de registro.
        /// </summary>
        /// <param name="codigoVerificacionServicio">
        /// Servicio para enviar codigos de verificacion.
        /// </param>
        /// <param name="cuentaServicio">Servicio para registrar cuentas.</param>
        /// <param name="seleccionarAvatarServicio">
        /// Servicio para seleccionar avatares.
        /// </param>
        /// <param name="verificacionCodigoDialogoServicio">
        /// Servicio para mostrar dialogo de verificacion.
        /// </param>
        /// <param name="catalogoAvatares">Catalogo de avatares disponibles.</param>
        /// <param name="localizacionServicio">
        /// Servicio de localizacion opcional.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia requerida es nula.
        /// </exception>
        public CreacionCuentaDependencias(
            ICodigoVerificacionServicio codigoVerificacionServicio,
            ICuentaServicio cuentaServicio,
            ISeleccionarAvatarServicio seleccionarAvatarServicio,
            IVerificacionCodigoDialogoServicio verificacionCodigoDialogoServicio,
            ICatalogoAvatares catalogoAvatares,
            ILocalizacionServicio localizacionServicio = null)
        {
            CodigoVerificacionServicio = codigoVerificacionServicio ?? 
                throw new ArgumentNullException(nameof(codigoVerificacionServicio));
            CuentaServicio = cuentaServicio ?? 
                throw new ArgumentNullException(nameof(cuentaServicio));
            SeleccionarAvatarServicio = seleccionarAvatarServicio ?? 
                throw new ArgumentNullException(nameof(seleccionarAvatarServicio));
            VerificacionCodigoDialogoServicio = verificacionCodigoDialogoServicio ?? 
                throw new ArgumentNullException(
                    nameof(verificacionCodigoDialogoServicio));
            CatalogoAvatares = catalogoAvatares ?? 
                throw new ArgumentNullException(nameof(catalogoAvatares));
            LocalizacionServicio = localizacionServicio;
        }

        /// <summary>
        /// Servicio para enviar codigos de verificacion por correo.
        /// </summary>
        public ICodigoVerificacionServicio CodigoVerificacionServicio { get; }

        /// <summary>
        /// Servicio para registrar nuevas cuentas de usuario.
        /// </summary>
        public ICuentaServicio CuentaServicio { get; }

        /// <summary>
        /// Servicio para seleccionar avatares.
        /// </summary>
        public ISeleccionarAvatarServicio SeleccionarAvatarServicio { get; }

        /// <summary>
        /// Servicio para mostrar el dialogo de verificacion de codigo.
        /// </summary>
        public IVerificacionCodigoDialogoServicio VerificacionCodigoDialogoServicio 
        { get; }

        /// <summary>
        /// Catalogo de avatares disponibles.
        /// </summary>
        public ICatalogoAvatares CatalogoAvatares { get; }

        /// <summary>
        /// Servicio de localizacion opcional.
        /// </summary>
        public ILocalizacionServicio LocalizacionServicio { get; }
    }
}
