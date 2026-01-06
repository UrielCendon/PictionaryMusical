using System;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Clase base para solicitudes de verificacion pendientes.
    /// Contiene las propiedades comunes entre verificacion de registro y recuperacion de cuenta.
    /// </summary>
    internal abstract class SolicitudPendienteBase
    {
        /// <summary>
        /// Codigo de verificacion generado.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Fecha y hora de expiracion del codigo.
        /// </summary>
        public DateTime Expira { get; set; }

        /// <summary>
        /// Idioma preferido del usuario para las notificaciones.
        /// </summary>
        public string Idioma { get; set; }

        /// <summary>
        /// Verifica si el codigo ha expirado.
        /// </summary>
        public bool EstaExpirado => Expira < DateTime.UtcNow;
    }
}
