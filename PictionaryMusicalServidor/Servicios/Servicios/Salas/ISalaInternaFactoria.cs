using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Interfaz para la creacion de salas internas.
    /// Permite abstraer la creacion de instancias para facilitar pruebas unitarias.
    /// </summary>
    public interface ISalaInternaFactoria
    {
        /// <summary>
        /// Crea una nueva instancia de sala interna.
        /// </summary>
        /// <param name="codigo">Codigo unico de la sala.</param>
        /// <param name="creador">Nombre del creador de la sala.</param>
        /// <param name="configuracion">Configuracion de la partida.</param>
        /// <returns>Nueva instancia de sala interna.</returns>
        SalaInternaManejador Crear(
            string codigo,
            string creador,
            ConfiguracionPartidaDTO configuracion);
    }
}
