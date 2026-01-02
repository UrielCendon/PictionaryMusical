using System;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Clase interna que representa una solicitud de codigo de verificacion pendiente para 
    /// registro.
    /// Almacena los datos temporales durante el proceso de verificacion de nueva cuenta.
    /// </summary>
    internal class SolicitudCodigoPendiente
    {
        /// <summary>
        /// Datos de la nueva cuenta que se esta verificando.
        /// </summary>
        public NuevaCuentaDTO DatosCuenta { get; set; }

        /// <summary>
        /// Codigo de verificacion generado.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Fecha y hora de expiracion del codigo.
        /// </summary>
        public DateTime Expira { get; set; }
    }
}
