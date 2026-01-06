using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Factoria para la creacion de salas internas.
    /// </summary>
    public class SalaInternaFactoria : ISalaInternaFactoria
    {
        /// <summary>
        /// Crea una nueva instancia de sala interna.
        /// </summary>
        /// <param name="codigo">Codigo unico de la sala.</param>
        /// <param name="creador">Nombre del creador de la sala.</param>
        /// <param name="configuracion">Configuracion de la partida.</param>
        /// <returns>Nueva instancia de sala interna.</returns>
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
