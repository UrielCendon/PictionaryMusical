using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Factoria para la creacion de salas internas.
    /// </summary>
    public class SalaInternaFactoria : ISalaInternaFactoria
    {
        /// <inheritdoc/>
        public SalaInternaManejador Crear(
            string codigo,
            string creador,
            ConfiguracionPartidaDTO configuracion)
        {
            var gestorNotificaciones = new GestorNotificacionesSalaInterna();
            return new SalaInternaManejador(
                codigo,
                creador,
                configuracion,
                gestorNotificaciones);
        }
    }
}
