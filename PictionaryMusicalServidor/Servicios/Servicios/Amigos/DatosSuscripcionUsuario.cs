namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Contiene los datos de un usuario para suscripcion a notificaciones.
    /// </summary>
    public class DatosSuscripcionUsuario
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Nombre del usuario normalizado.
        /// </summary>
        public string NombreNormalizado { get; set; }
    }
}
