using System;
using System.Collections.Concurrent;
using log4net;

namespace PictionaryMusicalServidor.Servicios.Servicios.Autenticacion
{
    /// <summary>
    /// Registro centralizado de sesiones activas en memoria.
    /// Permite verificar si un usuario ya tiene una sesion abierta
    /// para prevenir inicios de sesion duplicados.
    /// </summary>
    public static class SesionesActivasRegistro
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(SesionesActivasRegistro));

        private static readonly ConcurrentDictionary<string, DateTime> _sesionesActivas =
            new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Verifica si el usuario ya tiene una sesion activa.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a verificar.</param>
        /// <returns>True si el usuario ya tiene sesion activa, false en caso contrario.</returns>
        public static bool TieneSesionActiva(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return false;
            }

            return _sesionesActivas.ContainsKey(nombreUsuario);
        }

        /// <summary>
        /// Registra una nueva sesion activa para el usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a registrar.</param>
        /// <returns>True si se registro correctamente, false si ya existia una sesion.</returns>
        public static bool RegistrarSesion(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return false;
            }

            bool registrado = _sesionesActivas.TryAdd(nombreUsuario, DateTime.UtcNow);
            
            if (registrado)
            {
                _logger.Info("Nueva sesion registrada.");
            }

            return registrado;
        }

        /// <summary>
        /// Elimina el registro de sesion activa del usuario.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a desregistrar.</param>
        public static void EliminarSesion(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return;
            }

            DateTime valorDescartado;
            bool eliminado = _sesionesActivas.TryRemove(nombreUsuario, out valorDescartado);
            
            if (eliminado)
            {
                _logger.Info("Sesion eliminada del registro.");
            }
        }

        /// <summary>
        /// Obtiene el numero de sesiones activas actualmente.
        /// </summary>
        /// <returns>Cantidad de sesiones activas.</returns>
        public static int ObtenerCantidadSesiones()
        {
            return _sesionesActivas.Count;
        }
    }
}
