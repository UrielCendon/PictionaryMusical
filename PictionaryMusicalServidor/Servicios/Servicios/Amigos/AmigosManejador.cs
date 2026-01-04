using Datos.Modelo;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Implementacion del servicio de gestion de amistades entre usuarios.
    /// Maneja suscripciones para notificaciones, envio y respuesta de solicitudes de amistad,
    /// y eliminacion de relaciones de amistad con notificaciones en tiempo real via callbacks.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode =
        ConcurrencyMode.Multiple)]
    public class AmigosManejador : IAmigosManejador
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AmigosManejador));

        private readonly IManejadorCallback<IAmigosManejadorCallback> _manejadorCallback;
        private readonly INotificadorAmigos _notificador;
        private readonly INotificadorListaAmigos _notificadorListaAmigos;
        private readonly IOperacionAmistadServicio _operacionAmistadServicio;
        private readonly IProveedorCallback<IAmigosManejadorCallback> _proveedorCallback;

        /// <summary>
        /// Constructor por defecto para uso en WCF.
        /// Usa CallbacksCompartidos para asegurar que las notificaciones lleguen
        /// a todos los clientes suscritos en ListaAmigosManejador.
        /// </summary>
        public AmigosManejador() : this(
            new AmistadServicio(),
            new OperacionAmistadServicio(),
            new NotificadorListaAmigos(
                CallbacksCompartidos.ListaAmigos,
                new AmistadServicio(),
                new RepositorioFactoria()),
            new NotificadorAmigos(
                new ManejadorCallback<IAmigosManejadorCallback>(StringComparer.OrdinalIgnoreCase),
                new AmistadServicio()),
            new ManejadorCallback<IAmigosManejadorCallback>(StringComparer.OrdinalIgnoreCase),
            new ProveedorCallback<IAmigosManejadorCallback>())
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="amistadServicio">Servicio de amistad.</param>
        /// <param name="operacionAmistadServicio">Servicio de operaciones de amistad.</param>
        /// <param name="notificadorLista">Notificador de lista de amigos.</param>
        /// <param name="notificadorAmigos">Notificador de eventos de amistad.</param>
        /// <param name="manejadorCallback">Manejador de callbacks de amigos.</param>
        /// <param name="proveedorCallback">Proveedor de callback actual.</param>
        public AmigosManejador(
            IAmistadServicio amistadServicio,
            IOperacionAmistadServicio operacionAmistadServicio,
            INotificadorListaAmigos notificadorLista,
            INotificadorAmigos notificadorAmigos,
            IManejadorCallback<IAmigosManejadorCallback> manejadorCallback,
            IProveedorCallback<IAmigosManejadorCallback> proveedorCallback)
        {
            if (amistadServicio == null)
            {
                throw new ArgumentNullException(nameof(amistadServicio));
            }

            _operacionAmistadServicio = operacionAmistadServicio ??
                throw new ArgumentNullException(nameof(operacionAmistadServicio));

            _notificadorListaAmigos = notificadorLista ??
                throw new ArgumentNullException(nameof(notificadorLista));

            _notificador = notificadorAmigos ??
                throw new ArgumentNullException(nameof(notificadorAmigos));

            _manejadorCallback = manejadorCallback ??
                throw new ArgumentNullException(nameof(manejadorCallback));

            _proveedorCallback = proveedorCallback ??
                throw new ArgumentNullException(nameof(proveedorCallback));
        }

        /// <summary>
        /// Suscribe un usuario para recibir notificaciones de solicitudes de amistad.
        /// Normaliza el nombre de usuario, registra el callback y notifica solicitudes 
        /// pendientes.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario a suscribir.</param>
        public void Suscribir(string nombreUsuario)
        {
            EntradaComunValidador.ValidarNombreUsuarioSuscripcion(nombreUsuario);

            try
            {
                var datosUsuario = _operacionAmistadServicio.ObtenerDatosUsuarioSuscripcion(
                    nombreUsuario);

                RegistrarCallback(nombreUsuario, datosUsuario.NombreNormalizado);

                _notificador.NotificarSolicitudesPendientesAlSuscribir(
                    datosUsuario.NombreNormalizado,
                    datosUsuario.IdUsuario);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    "Fallo la conexion a la base de datos al suscribir " +
                    "al usuario con identificador relacionado.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarSolicitudes);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    "Los datos del usuario son inconsistentes " +
                    "al intentar suscribirlo a notificaciones de amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarSolicitudes);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Ocurrio un error desconocido al suscribir al usuario " +
                    "a notificaciones de amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarSolicitudes);
            }
        }

        /// <summary>
        /// Cancela la suscripcion de un usuario de notificaciones de amistad.
        /// Elimina el callback del usuario del manejador de callbacks.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario que cancela la suscripcion.</param>
        public void CancelarSuscripcion(string nombreUsuario)
        {
            if (!EntradaComunValidador.EsMensajeValido(nombreUsuario))
            {
                throw new FaultException(
                    MensajesError.Cliente.NombreUsuarioObligatorioCancelar);
            }

            _manejadorCallback.Desuscribir(nombreUsuario);
        }

        /// <summary>
        /// Envia una solicitud de amistad de un usuario a otro.
        /// Valida que ambos usuarios existan, crea la solicitud y notifica al receptor.
        /// </summary>
        /// <param name="nombreUsuarioEmisor">Nombre del usuario que envia la solicitud.</param>
        /// <param name="nombreUsuarioReceptor">Nombre del usuario que recibe la solicitud.</param>
        public void EnviarSolicitudAmistad(
            string nombreUsuarioEmisor,
            string nombreUsuarioReceptor)
        {
            try
            {
                EntradaComunValidador.ValidarUsuariosInteraccion(
                    nombreUsuarioEmisor,
                    nombreUsuarioReceptor);

                var resultado = _operacionAmistadServicio.EjecutarCreacionSolicitud(
                    nombreUsuarioEmisor,
                    nombreUsuarioReceptor);

                var datosNotificacion = new DatosNotificacionSolicitud
                {
                    NombreEmisorOriginal = nombreUsuarioEmisor,
                    NombreReceptorOriginal = nombreUsuarioReceptor
                };

                NotificarSolicitudNueva(resultado, datosNotificacion);
            }
            catch (Datos.Excepciones.BaseDatosExcepcion excepcion)
            {
                _logger.Warn(
                    "El usuario emisor o receptor no existe en la base de datos " +
                    "al intentar enviar solicitud de amistad.",
                    excepcion);
                throw new FaultException(excepcion.Message);
            }
            catch (KeyNotFoundException excepcion)
            {
                _logger.Warn(
                    "No se encontro al usuario receptor al enviar solicitud de amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.UsuariosEspecificadosNoExisten);
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(
                    "La validacion fallo al enviar solicitud de amistad.",
                    excepcion);
                throw;
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(
                    "No se puede crear la solicitud de amistad: " +
                    "ya existe una relacion o se intento enviar solicitud a si mismo.",
                    excepcion);
                throw new FaultException(
                    excepcion.Message ?? MensajesError.Cliente.ErrorAlmacenarSolicitud);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(
                    "Los datos proporcionados para la solicitud de amistad son invalidos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    "No se pudo guardar la solicitud de amistad " +
                    "debido a un conflicto en la base de datos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorAlmacenarSolicitud);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    "La conexion a la base de datos fallo al crear " +
                    "la solicitud de amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorAlmacenarSolicitud);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    "Los datos de la solicitud de amistad " +
                    "son invalidos o estan corruptos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorAlmacenarSolicitud);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Ocurrio un error inesperado al procesar la solicitud de amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorAlmacenarSolicitud);
            }
        }

        /// <summary>
        /// Responde una solicitud de amistad aceptandola.
        /// Actualiza la solicitud, notifica a ambos usuarios y actualiza sus listas de amigos.
        /// </summary>
        /// <param name="nombreUsuarioEmisor">Usuario que envio la solicitud original.</param>
        /// <param name="nombreUsuarioReceptor">Usuario que responde la solicitud.</param>
        public void ResponderSolicitudAmistad(
            string nombreUsuarioEmisor,
            string nombreUsuarioReceptor)
        {
            try
            {
                EntradaComunValidador.ValidarUsuariosInteraccion(
                    nombreUsuarioEmisor,
                    nombreUsuarioReceptor);

                var resultado = _operacionAmistadServicio.EjecutarAceptacionSolicitud(
                    nombreUsuarioEmisor,
                    nombreUsuarioReceptor);

                EjecutarNotificacionesRespuesta(
                    resultado.NombreNormalizadoEmisor,
                    resultado.NombreNormalizadoReceptor);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(
                    "No se puede aceptar la solicitud de amistad: " +
                    "la solicitud no existe, ya fue aceptada o el receptor no corresponde.",
                    excepcion);
                throw new FaultException(
                    excepcion.Message ?? MensajesError.Cliente.ErrorActualizarSolicitud);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(
                    "Los datos proporcionados para aceptar la solicitud son invalidos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.DatosInvalidos);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    "No se pudo actualizar la solicitud de amistad " +
                    "debido a un conflicto en la base de datos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorActualizarSolicitud);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    "La conexion a la base de datos fallo al aceptar " +
                    "la solicitud de amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorActualizarSolicitud);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    "Los datos de la solicitud de amistad " +
                    "son invalidos o estan corruptos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorActualizarSolicitud);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Ocurrio un error inesperado al aceptar la solicitud de amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorActualizarSolicitud);
            }
        }

        /// <summary>
        /// Elimina la relacion de amistad entre dos usuarios.
        /// Elimina la amistad, notifica a ambos usuarios y actualiza sus listas de amigos.
        /// </summary>
        /// <param name="nombreUsuarioA">Nombre del primer usuario.</param>
        /// <param name="nombreUsuarioB">Nombre del segundo usuario.</param>
        public void EliminarAmigo(string nombreUsuarioA, string nombreUsuarioB)
        {
            try
            {
                EntradaComunValidador.ValidarUsuariosInteraccion(
                    nombreUsuarioA, 
                    nombreUsuarioB);

                var resultado = _operacionAmistadServicio.EjecutarEliminacion(
                    nombreUsuarioA,
                    nombreUsuarioB);

                EjecutarNotificacionesEliminacion(resultado);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(
                    "No se puede eliminar la amistad: " +
                    "la relacion no existe o se intento eliminar con el mismo usuario.",
                    excepcion);
                throw new FaultException(
                    excepcion.Message ?? MensajesError.Cliente.RelacionAmistadNoExiste);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    "No se pudo eliminar la amistad " +
                    "debido a un conflicto en la base de datos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorEliminarAmistad);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    "La conexion a la base de datos fallo al eliminar la amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorEliminarAmistad);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    "Los datos de la amistad son invalidos o estan corruptos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorEliminarAmistad);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Ocurrio un error inesperado al eliminar la amistad.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorEliminarAmistad);
            }
        }

        private void RegistrarCallback(string nombreUsuario, string nombreNormalizado)
        {
            var callback = _proveedorCallback.ObtenerCallbackActual();
            _manejadorCallback.Suscribir(nombreNormalizado, callback);

            if (!string.Equals(
                nombreUsuario,
                nombreNormalizado,
                StringComparison.Ordinal))
            {
                _manejadorCallback.Desuscribir(nombreUsuario);
            }

            _manejadorCallback.ConfigurarEventosCanal(nombreNormalizado);
        }

        private void NotificarSolicitudNueva(
            ResultadoCreacionSolicitud resultado,
            DatosNotificacionSolicitud datosNotificacion)
        {
            string nombreEmisor = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                resultado.Emisor.Nombre_Usuario,
                datosNotificacion.NombreEmisorOriginal);

            string nombreReceptor = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                resultado.Receptor.Nombre_Usuario,
                datosNotificacion.NombreReceptorOriginal);

            var solicitud = new SolicitudAmistadDTO
            {
                UsuarioEmisor = nombreEmisor,
                UsuarioReceptor = nombreReceptor,
                SolicitudAceptada = false
            };

            _notificador.NotificarSolicitudActualizada(nombreReceptor, solicitud);
        }

        private void EjecutarNotificacionesRespuesta(string emisor, string receptor)
        {
            NotificarSolicitudAceptada(emisor, receptor);
            _notificadorListaAmigos.NotificarCambioAmistad(emisor);
            _notificadorListaAmigos.NotificarCambioAmistad(receptor);
        }

        private void NotificarSolicitudAceptada(string emisor, string receptor)
        {
            var solicitud = new SolicitudAmistadDTO
            {
                UsuarioEmisor = emisor,
                UsuarioReceptor = receptor,
                SolicitudAceptada = true
            };

            _notificador.NotificarSolicitudActualizada(emisor, solicitud);
            _notificador.NotificarSolicitudActualizada(receptor, solicitud);
        }

        private void EjecutarNotificacionesEliminacion(ResultadoEliminacionAmistad resultado)
        {
            NotificarEliminacion(
                resultado.Relacion,
                resultado.NombrePrimerUsuarioNormalizado,
                resultado.NombreSegundoUsuarioNormalizado);

            _notificadorListaAmigos.NotificarCambioAmistad(
                resultado.NombrePrimerUsuarioNormalizado);
            _notificadorListaAmigos.NotificarCambioAmistad(
                resultado.NombreSegundoUsuarioNormalizado);
        }

        private void NotificarEliminacion(
            Amigo relacion, 
            string primerUsuario, 
            string segundoUsuario)
        {
            if (relacion == null)
            {
                _logger.Warn(
                    "Se solicito notificar una eliminacion de amistad sin relacion.");
                return;
            }

            var solicitud = new SolicitudAmistadDTO
            {
                UsuarioEmisor = primerUsuario,
                UsuarioReceptor = segundoUsuario,
                SolicitudAceptada = false
            };

            _notificador.NotificarAmistadEliminada(primerUsuario, solicitud);
            _notificador.NotificarAmistadEliminada(segundoUsuario, solicitud);
        }
    }
}
