using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Servicio especializado en notificaciones de cambios en salas.
    /// Gestiona las suscripciones y notificaciones de listas de salas.
    /// </summary>
    internal class NotificadorSalas : INotificadorSalas
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(NotificadorSalas));
        private readonly ConcurrentDictionary<Guid, ISalasManejadorCallback> _suscripciones =
            new ConcurrentDictionary<Guid, ISalasManejadorCallback>();
        private readonly IObtenerSalas _proveedorSalas;

        public NotificadorSalas(IObtenerSalas proveedorSalas)
        {
            _proveedorSalas = proveedorSalas;
        }

        /// <summary>
        /// Suscribe un callback a las notificaciones de la lista de salas.
        /// </summary>
        /// <param name="callback">Callback a suscribir.</param>
        /// <returns>ID de la suscripcion.</returns>
        public Guid Suscribir(ISalasManejadorCallback callback)
        {
            var sesionId = Guid.NewGuid();
            _suscripciones[sesionId] = callback;

            return sesionId;
        }

        /// <summary>
        /// Elimina una suscripcion especifica.
        /// </summary>
        /// <param name="sesionId">ID de la suscripcion a eliminar.</param>
        public void Desuscribir(Guid sesionId)
        {
            ISalasManejadorCallback valorDescartado;
            _suscripciones.TryRemove(sesionId, out valorDescartado);
        }

        /// <summary>
        /// Elimina todas las suscripciones asociadas a un callback especifico.
        /// </summary>
        /// <param name="callback">Callback a desuscribir.</param>
        public void DesuscribirPorCallback(ISalasManejadorCallback callback)
        {
            var clavesSuscripciones = _suscripciones
                .Where(s => ReferenceEquals(s.Value, callback))
                .Select(s => s.Key)
                .ToList();

            foreach (var claveSuscripcion in clavesSuscripciones)
            {
                ISalasManejadorCallback valorDescartado;
                _suscripciones.TryRemove(claveSuscripcion, out valorDescartado);
            }
        }

        /// <summary>
        /// Notifica la lista actualizada de salas a un callback especifico.
        /// </summary>
        /// <param name="callback">Callback a notificar.</param>
        public void NotificarListaSalas(ISalasManejadorCallback callback)
        {
            try
            {
                var salasInternas = _proveedorSalas.ObtenerSalasInternas();
                var salas = ConvertirSalasADto(salasInternas);
                callback.NotificarListaSalasActualizada(salas);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    "Error de comunicacion al notificar la lista de salas a los suscriptores.", 
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn("Timeout al notificar la lista de salas a los suscriptores.", excepcion);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Error(
                    "Canal cerrado al notificar la lista de salas a los suscriptores.", excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Error inesperado al notificar la lista de salas a los suscriptores.", excepcion);
            }
        }

        /// <summary>
        /// Notifica la lista actualizada de salas a todos los suscriptores.
        /// </summary>
        public void NotificarListaSalasATodos()
        {
            var salasInternas = _proveedorSalas.ObtenerSalasInternas();
            var salas = ConvertirSalasADto(salasInternas);

            foreach (var suscripcion in _suscripciones)
            {
                try
                {
                    suscripcion.Value.NotificarListaSalasActualizada(salas);
                }
                catch (CommunicationException excepcion)
                {
                    _logger.Warn(
                        "Error de comunicacion al notificar masivamente. Eliminando suscripcion defectuosa.",
                        excepcion);
                    ISalasManejadorCallback valorDescartado;
                    _suscripciones.TryRemove(suscripcion.Key, out valorDescartado);
                }
                catch (TimeoutException excepcion)
                {
                    _logger.Warn(
                        "Timeout al notificar masivamente lista de salas. Eliminando suscripcion defectuosa.",
                        excepcion);
                    ISalasManejadorCallback valorDescartado;
                    _suscripciones.TryRemove(suscripcion.Key, out valorDescartado);
                }
                catch (ObjectDisposedException excepcion)
                {
                    _logger.Error(
                        "Canal cerrado al notificar masivamente lista de salas.", excepcion);
                }
                catch (Exception excepcion)
                {
                    _logger.Error(
                        "Error inesperado al notificar masivamente lista de salas.", excepcion);
                }
            }
        }

        private static SalaDTO[] ConvertirSalasADto(IEnumerable<SalaInternaManejador> salasInternas)
        {
            var listaSalas = new List<SalaDTO>();
            foreach (var sala in salasInternas)
            {
                listaSalas.Add(sala.ConvertirADto());
            }
            return listaSalas.ToArray();
        }
    }

    /// <summary>
    /// Interfaz para obtener la coleccion de salas.
    /// </summary>
    internal interface IObtenerSalas
    {
        IEnumerable<SalaInternaManejador> ObtenerSalasInternas();
    }
}
