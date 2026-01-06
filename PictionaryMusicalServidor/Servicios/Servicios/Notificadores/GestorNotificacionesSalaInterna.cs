using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Implementacion del gestor de notificaciones que maneja los callbacks WCF 
    /// y el control de errores de comunicacion.
    /// </summary>
    public class GestorNotificacionesSalaInterna : IGestorNotificacionesSalaInterna
    {
        private static readonly ILog _logger = 
            LogManager.GetLogger(typeof(GestorNotificacionesSalaInterna));
        private readonly Dictionary<string, ISalasManejadorCallback> _callbacks;
        private readonly object _sincronizacion = new object();

        /// <summary>
        /// Inicializa una nueva instancia del gestor de notificaciones.
        /// </summary>
        public GestorNotificacionesSalaInterna()
        {
            _callbacks = new Dictionary<string, ISalasManejadorCallback>
                (StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Registra un cliente para recibir notificaciones de la sala.
        /// </summary>
        public void Registrar(string nombreUsuario, ISalasManejadorCallback callback)
        {
            lock (_sincronizacion)
            {
                _callbacks[nombreUsuario] = callback;
            }
        }

        /// <summary>
        /// Elimina el registro de notificaciones de un cliente.
        /// </summary>
        public void Remover(string nombreUsuario)
        {
            lock (_sincronizacion)
            {
                _callbacks.Remove(nombreUsuario);
            }
        }

        /// <summary>
        /// Obtiene el callback asociado a un usuario especifico.
        /// </summary>
        public ISalasManejadorCallback ObtenerCallback(string nombreUsuario)
        {
            lock (_sincronizacion)
            {
                ISalasManejadorCallback callback;
                if (_callbacks.TryGetValue(nombreUsuario, out callback))
                {
                    return callback;
                }
                return new SalasCallbackNulo();
            }
        }

        /// <summary>
        /// Limpia todos los registros de notificaciones.
        /// </summary>
        public void Limpiar()
        {
            lock (_sincronizacion)
            {
                _callbacks.Clear();
            }
        }

        /// <summary>
        /// Notifica a los integrantes que un nuevo jugador ha ingresado y envia la sala 
        /// actualizada.
        /// </summary>
        public void NotificarIngreso(string codigoSala, string nombreUsuario, 
            SalaDTO salaActualizada)
        {
            var destinatarios = ObtenerDestinatariosExcluyendo(nombreUsuario);
            foreach (var callback in destinatarios)
            {
                NotificarJugadorSeUnioSeguro(callback, codigoSala, nombreUsuario);
            }
            NotificarActualizacionGlobal(salaActualizada);
        }

        /// <summary>
        /// Notifica a los integrantes que un jugador ha salido y envia la sala actualizada.
        /// </summary>
        public void NotificarSalida(string codigoSala, string nombreUsuario, 
            SalaDTO salaActualizada)
        {
            var destinatarios = ObtenerTodosLosDestinatarios();
            foreach (var callback in destinatarios)
            {
                NotificarJugadorSalioSeguro(callback, codigoSala, nombreUsuario);
                NotificarSalaActualizadaSeguro(callback, salaActualizada);
            }
        }

        /// <summary>
        /// Notifica una accion sobre un jugador (expulsion o baneo) al afectado y actualiza 
        /// a los demas.
        /// </summary>
        /// <param name="parametros">Objeto con los datos necesarios para la notificacion.</param>
        public void NotificarAccionJugador(AccionJugadorSalaParametros parametros)
        {
            string tipoAccion = parametros.TipoAccion == TipoAccionJugador.Baneo 
                ? "baneo" 
                : "expulsion";

            _logger.InfoFormat(
                "Notificando {0} en sala '{1}' a todos los clientes.",
                tipoAccion,
                parametros.CodigoSala);

            var todosLosDestinatarios = ObtenerTodosLosDestinatarios();

            if (parametros.TipoAccion == TipoAccionJugador.Baneo)
            {
                foreach (var callback in todosLosDestinatarios)
                {
                    NotificarJugadorBaneadoSeguro(
                        callback, 
                        parametros.CodigoSala, 
                        parametros.NombreJugadorAfectado);
                }

                if (parametros.CallbackAfectado != null)
                {
                    NotificarJugadorBaneadoSeguro(
                        parametros.CallbackAfectado, 
                        parametros.CodigoSala, 
                        parametros.NombreJugadorAfectado);
                }
            }
            else
            {
                foreach (var callback in todosLosDestinatarios)
                {
                    NotificarJugadorExpulsadoSeguro(
                        callback, 
                        parametros.CodigoSala, 
                        parametros.NombreJugadorAfectado);
                }

                if (parametros.CallbackAfectado != null)
                {
                    NotificarJugadorExpulsadoSeguro(
                        parametros.CallbackAfectado, 
                        parametros.CodigoSala, 
                        parametros.NombreJugadorAfectado);
                }
            }

            var destinatarios = ObtenerDestinatariosExcluyendo(parametros.NombreJugadorAfectado);
            foreach (var callback in destinatarios)
            {
                NotificarSalaActualizadaSeguro(callback, parametros.SalaActualizada);
            }

            _logger.InfoFormat(
                "{0} notificada a todos los clientes en sala '{1}'.",
                tipoAccion,
                parametros.CodigoSala);
        }

        /// <summary>
        /// Notifica a todos los integrantes que la sala ha sido cancelada.
        /// </summary>
        public void NotificarCancelacion(string codigoSala)
        {
            var destinatarios = ObtenerTodosLosDestinatarios();
            foreach (var callback in destinatarios)
            {
                NotificarSalaCanceladaSeguro(callback, codigoSala);
            }
        }

        private List<ISalasManejadorCallback> ObtenerDestinatariosExcluyendo(
            string usuarioExcluido)
        {
            lock (_sincronizacion)
            {
                var resultado = new List<ISalasManejadorCallback>();
                foreach (var parCallbackRegistrado in _callbacks)
                {
                    bool esUsuarioExcluido = string.Equals(
                        parCallbackRegistrado.Key,
                        usuarioExcluido,
                        StringComparison.OrdinalIgnoreCase);

                    if (!esUsuarioExcluido)
                    {
                        resultado.Add(parCallbackRegistrado.Value);
                    }
                }
                return resultado;
            }
        }

        private List<ISalasManejadorCallback> ObtenerTodosLosDestinatarios()
        {
            lock (_sincronizacion)
            {
                return new List<ISalasManejadorCallback>(_callbacks.Values);
            }
        }

        private void NotificarActualizacionGlobal(SalaDTO sala)
        {
            var destinatarios = ObtenerTodosLosDestinatarios();
            foreach (var callback in destinatarios)
            {
                NotificarSalaActualizadaSeguro(callback, sala);
            }
        }

        private static void NotificarJugadorSeUnioSeguro(
            ISalasManejadorCallback callback, 
            string codigoSala, 
            string nombreUsuario)
        {
            try
            {
                callback.NotificarJugadorSeUnio(codigoSala, nombreUsuario);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorComunicacionNotificarClienteSala,
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorTimeoutNotificarClienteSala,
                    excepcion);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorCanalCerradoNotificarClienteSala,
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorInesperadoNotificarClienteSala,
                    excepcion);
            }
        }

        private static void NotificarJugadorSalioSeguro(
            ISalasManejadorCallback callback, 
            string codigoSala, 
            string nombreUsuario)
        {
            try
            {
                callback.NotificarJugadorSalio(codigoSala, nombreUsuario);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorComunicacionNotificarClienteSala,
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorTimeoutNotificarClienteSala,
                    excepcion);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorCanalCerradoNotificarClienteSala,
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorInesperadoNotificarClienteSala,
                    excepcion);
            }
        }

        private static void NotificarSalaActualizadaSeguro(
            ISalasManejadorCallback callback, 
            SalaDTO sala)
        {
            try
            {
                callback.NotificarSalaActualizada(sala);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorComunicacionNotificarClienteSala,
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorTimeoutNotificarClienteSala,
                    excepcion);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorCanalCerradoNotificarClienteSala,
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorInesperadoNotificarClienteSala,
                    excepcion);
            }
        }

        private static void NotificarJugadorExpulsadoSeguro(
            ISalasManejadorCallback callback, 
            string codigoSala, 
            string nombreExpulsado)
        {
            try
            {
                callback.NotificarJugadorExpulsado(codigoSala, nombreExpulsado);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorComunicacionNotificarClienteSala,
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorTimeoutNotificarClienteSala,
                    excepcion);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorCanalCerradoNotificarClienteSala,
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorInesperadoNotificarClienteSala,
                    excepcion);
            }
        }

        private static void NotificarSalaCanceladaSeguro(
            ISalasManejadorCallback callback, 
            string codigoSala)
        {
            try
            {
                callback.NotificarSalaCancelada(codigoSala);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorComunicacionNotificarClienteSala,
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorTimeoutNotificarClienteSala,
                    excepcion);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorCanalCerradoNotificarClienteSala,
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorInesperadoNotificarClienteSala,
                    excepcion);
            }
        }

        private static void NotificarJugadorBaneadoSeguro(
            ISalasManejadorCallback callback, 
            string codigoSala, 
            string nombreBaneado)
        {
            try
            {
                callback.NotificarJugadorBaneado(codigoSala, nombreBaneado);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorComunicacionNotificarClienteSala, 
                    excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorTimeoutNotificarClienteSala, 
                    excepcion);
            }
            catch (ObjectDisposedException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.ErrorCanalCerradoNotificarClienteSala, 
                    excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorInesperadoNotificarClienteSala, 
                    excepcion);
            }
        }

        /// <summary>
        /// Implementacion del patron Null Object para evitar referencias nulas en callbacks.
        /// </summary>
        private sealed class SalasCallbackNulo : ISalasManejadorCallback
        {
            public void NotificarJugadorSeUnio(string codigoSala, string nombreJugador) { }
            public void NotificarJugadorSalio(string codigoSala, string nombreJugador) { }
            public void NotificarListaSalasActualizada(SalaDTO[] salas) { }
            public void NotificarSalaActualizada(SalaDTO sala) { }
            public void NotificarJugadorExpulsado(string codigoSala, string nombreJugador) { }
            public void NotificarSalaCancelada(string codigoSala) { }
            public void NotificarJugadorBaneado(string codigoSala, string nombreJugador) { }
        }
    }
}
