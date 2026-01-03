using System;
using System.Collections.Generic;
using System.Data;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Servicio especializado en notificaciones de eventos de amistad.
    /// Gestiona las notificaciones de solicitudes y eliminaciones de amistad.
    /// </summary>
    internal class NotificadorAmigos : INotificadorAmigos
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(NotificadorAmigos));
        private readonly ManejadorCallback<IAmigosManejadorCallback> _manejadorCallback;
        private readonly IAmistadServicio _amistadServicio;

        public NotificadorAmigos(ManejadorCallback<IAmigosManejadorCallback> manejadorCallback, 
            IAmistadServicio amistadServicio)
        {
            _manejadorCallback = manejadorCallback;
            _amistadServicio = amistadServicio;
        }

        /// <summary>
        /// Notifica una solicitud de amistad actualizada a un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Usuario a notificar.</param>
        /// <param name="solicitud">Detalles de la solicitud.</param>
        public void NotificarSolicitudActualizada(string nombreUsuario, 
            SolicitudAmistadDTO solicitud)
        {
            IAmigosManejadorCallback callback = _manejadorCallback.ObtenerCallback(nombreUsuario);
            if (callback != null)
            {
                try
                {
                    callback.NotificarSolicitudActualizada(solicitud);
                }
                catch (Exception excepcion)
                {
                    _logger.Warn(MensajesError.Bitacora.ErrorNotificarSolicitudActualizada, excepcion);
                }
            }
        }

        /// <summary>
        /// Notifica la eliminacion de una amistad a un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Usuario a notificar.</param>
        /// <param name="solicitud">Detalles de la relacion eliminada.</param>
        public void NotificarAmistadEliminada(string nombreUsuario, SolicitudAmistadDTO solicitud)
        {
            IAmigosManejadorCallback callback = _manejadorCallback.ObtenerCallback(nombreUsuario);
            if (callback != null)
            {
                try
                {
                    callback.NotificarAmistadEliminada(solicitud);
                }
                catch (Exception excepcion)
                {
                    _logger.Warn(MensajesError.Bitacora.ErrorNotificarAmistadEliminada, excepcion);
                }
            }
        }

        /// <summary>
        /// Notifica todas las solicitudes pendientes al suscribirse un usuario.
        /// </summary>
        /// <param name="nombreNormalizado">Nombre normalizado del usuario.</param>
        /// <param name="usuarioId">ID del usuario.</param>
        /// <exception cref="DataException">Si hay error al recuperar las solicitudes.</exception>
        public void NotificarSolicitudesPendientesAlSuscribir(string nombreNormalizado, 
            int usuarioId)
        {
            try
            {
                List<SolicitudAmistadDTO> solicitudesDTO = 
                    _amistadServicio.ObtenerSolicitudesPendientesDTO(usuarioId);

                if (solicitudesDTO == null || solicitudesDTO.Count == 0)
                {
                    return;
                }

                foreach (var solicitudDto in solicitudesDTO)
                {
                    NotificarSolicitudActualizada(nombreNormalizado, solicitudDto);
                }
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    "Error de datos al recuperar las solicitudes pendientes de amistad.", 
                    excepcion);
                throw;
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Error inesperado al recuperar las solicitudes pendientes de amistad.", 
                    excepcion);
                throw new DataException(
                    "Error al recuperar solicitudes pendientes de amistad.", 
                    excepcion);
            }
        }
    }
}
