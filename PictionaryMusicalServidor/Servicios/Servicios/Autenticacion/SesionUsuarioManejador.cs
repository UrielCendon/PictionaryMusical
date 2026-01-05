using System;
using System.Collections.Concurrent;
using System.Linq;
using log4net;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Singleton thread-safe que gestiona las sesiones activas de usuarios.
    /// Permite registrar, verificar y eliminar sesiones para prevenir inicios de sesion 
    /// duplicados.
    /// </summary>
    internal sealed class SesionUsuarioManejador
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SesionUsuarioManejador));
        private static readonly Lazy<SesionUsuarioManejador> _instancia = 
            new Lazy<SesionUsuarioManejador>(() => new SesionUsuarioManejador());

        private readonly ConcurrentDictionary<int, SesionActiva> _sesionesActivas;

        private SesionUsuarioManejador()
        {
            _sesionesActivas = new ConcurrentDictionary<int, SesionActiva>();
        }

        /// <summary>
        /// Obtiene la instancia unica del manejador de sesiones.
        /// </summary>
        public static SesionUsuarioManejador Instancia => _instancia.Value;

        /// <summary>
        /// Verifica si un usuario tiene una sesion activa.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        /// <returns>True si el usuario tiene una sesion activa.</returns>
        public bool TieneSesionActiva(int usuarioId)
        {
            return _sesionesActivas.ContainsKey(usuarioId);
        }

        /// <summary>
        /// Intenta registrar una nueva sesion para el usuario.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        /// <param name="nombreUsuario">Nombre del usuario para registro.</param>
        /// <returns>True si la sesion fue registrada exitosamente, false si ya existe una sesion 
        /// activa.</returns>
        public bool IntentarRegistrarSesion(int usuarioId, string nombreUsuario)
        {
            var nuevaSesion = new SesionActiva(nombreUsuario, DateTime.UtcNow);

            bool registrada = _sesionesActivas.TryAdd(usuarioId, nuevaSesion);

            if (registrada)
            {
                _logger.InfoFormat(
                    MensajesError.Bitacora.SesionRegistradaUsuario,
                    usuarioId);
            }
            else
            {
                _logger.WarnFormat(
                    MensajesError.Bitacora.IntentoDuplicadoSesion,
                    usuarioId);
            }

            return registrada;
        }

        /// <summary>
        /// Elimina la sesion activa de un usuario.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        public void EliminarSesion(int usuarioId)
        {
            SesionActiva sesionRemovida;
            if (_sesionesActivas.TryRemove(usuarioId, out sesionRemovida))
            {
                _logger.InfoFormat(
                    MensajesError.Bitacora.SesionEliminadaUsuario,
                    usuarioId);
            }
        }

        /// <summary>
        /// Elimina la sesion activa de un usuario por nombre de usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario.</param>
        public void EliminarSesionPorNombre(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return;
            }

            string nombreNormalizado = nombreUsuario.Trim();

            var sesionEncontrada = _sesionesActivas.FirstOrDefault(parClaveValor =>
                string.Equals(
                    parClaveValor.Value.NombreUsuario,
                    nombreNormalizado,
                    StringComparison.OrdinalIgnoreCase));

            if (sesionEncontrada.Value != null)
            {
                SesionActiva sesionRemovida;
                if (_sesionesActivas.TryRemove(sesionEncontrada.Key, out sesionRemovida))
                {
                    _logger.InfoFormat(
                        MensajesError.Bitacora.SesionEliminadaUsuario,
                        sesionEncontrada.Key);
                }
            }
        }

        /// <summary>
        /// Obtiene el numero total de sesiones activas.
        /// </summary>
        /// <returns>Numero de sesiones activas.</returns>
        public int ObtenerConteoSesiones()
        {
            return _sesionesActivas.Count;
        }

        /// <summary>
        /// Clase interna que representa una sesion activa.
        /// </summary>
        private sealed class SesionActiva
        {
            public string NombreUsuario { get; }
            public DateTime FechaInicio { get; }

            public SesionActiva(string nombreUsuario, DateTime fechaInicio)
            {
                NombreUsuario = nombreUsuario;
                FechaInicio = fechaInicio;
            }
        }
    }
}
