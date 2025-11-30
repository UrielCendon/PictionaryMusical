using System;
using System.Data;
using System.Data.Entity.Core;
using System.ServiceModel;
using PictionaryMusicalServidor.Datos.DAL.Implementaciones;
using Datos.Modelo;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Servicios.Servicios
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
        private readonly ManejadorCallback<IAmigosManejadorCallback> _manejadorCallback;
        private readonly INotificadorAmigos _notificador;
        private readonly INotificadorListaAmigos _notificadorListaAmigos;
        private readonly IContextoFactory _contextoFactory;
        private readonly IAmistadServicio _amistadServicio;

        /// <summary>
        /// Constructor por defecto para compatibilidad con WCF.
        /// </summary>
        public AmigosManejador(IContextoFactory contextoFactory, IAmistadServicio amistadServicio,
            INotificadorListaAmigos notificadorLista)
        {
            _contextoFactory = contextoFactory ?? 
                throw new ArgumentNullException(nameof(contextoFactory));
            _amistadServicio = amistadServicio ?? 
                throw new ArgumentNullException(nameof(amistadServicio));
            _notificadorListaAmigos = notificadorLista ?? 
                throw new ArgumentNullException(nameof(notificadorLista));

            _manejadorCallback = new ManejadorCallback<IAmigosManejadorCallback>
                (StringComparer.OrdinalIgnoreCase);
            _notificador = new NotificadorAmigos(_manejadorCallback, _amistadServicio);
        }

        /// <summary>
        /// Suscribe un usuario para recibir notificaciones de solicitudes de amistad.
        /// Normaliza el nombre de usuario, registra el callback y notifica solicitudes pendientes.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario a suscribir.</param>
        /// <exception cref="FaultException">Se lanza si el nombre de usuario es invalido, 
        /// no existe, o hay errores de base de datos.</exception>
        public void Suscribir(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new FaultException
                    (MensajesError.Cliente.NombreUsuarioObligatorioSuscripcion);
            }

            Usuario usuario;
            string nombreNormalizado;

            try
            {
                using (var contexto = _contextoFactory.CrearContexto())
                {
                    var usuarioRepositorio = new UsuarioRepositorio(contexto);
                    usuario = usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuario);

                    if (usuario == null)
                    {
                        throw new FaultException(MensajesError.Cliente.UsuarioNoEncontrado);
                    }
                    nombreNormalizado = ValidadorNombreUsuario.ObtenerNombreNormalizado
                        (usuario.Nombre_Usuario, nombreUsuario);
                }

                if (string.IsNullOrWhiteSpace(nombreNormalizado))
                {
                    throw new FaultException(MensajesError.Cliente.UsuarioNoEncontrado);
                }

                RegistrarCallback(nombreUsuario, nombreNormalizado);
                _notificador.NotificarSolicitudesPendientesAlSuscribir(nombreNormalizado, 
                    usuario.idUsuario);
            }
            catch (EntityException ex)
            {
                _logger.Error("Error de base de datos al suscribir a notificaciones de amistad.",
                    ex);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarSolicitudes);
            }
            catch (DataException ex)
            {
                _logger.Error("Error de datos al suscribir a notificaciones de amistad.", ex);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarSolicitudes);
            }
        }

        /// <summary>
        /// Cancela la suscripcion de un usuario de notificaciones de amistad.
        /// Elimina el callback del usuario del manejador de callbacks.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario que cancela la suscripcion.</param>
        /// <exception cref="FaultException">Se lanza si el nombre de usuario es invalido.
        /// </exception>
        public void CancelarSuscripcion(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new FaultException
                    (MensajesError.Cliente.NombreUsuarioObligatorioCancelar);
            }

            _manejadorCallback.Desuscribir(nombreUsuario);
        }

        /// <summary>
        /// Envia una solicitud de amistad de un usuario a otro.
        /// Valida que ambos usuarios existan, crea la solicitud en la base de datos y notifica al receptor.
        /// </summary>
        /// <param name="nombreUsuarioEmisor">Nombre del usuario que envia la solicitud.</param>
        /// <param name="nombreUsuarioReceptor">Nombre del usuario que recibe la solicitud.</param>
        /// <exception cref="FaultException">Se lanza si los nombres son invalidos, los usuarios no existen, o hay errores de base de datos.</exception>
        public void EnviarSolicitudAmistad(string nombreUsuarioEmisor, 
            string nombreUsuarioReceptor)
        {
            try
            {
                ValidadorNombreUsuario.Validar(nombreUsuarioEmisor, 
                    nameof(nombreUsuarioEmisor));
                ValidadorNombreUsuario.Validar(nombreUsuarioReceptor, 
                    nameof(nombreUsuarioReceptor));

                Usuario usuarioEmisor;
                Usuario usuarioReceptor;

                using (var contexto = _contextoFactory.CrearContexto())
                {
                    var usuarioRepositorio = new UsuarioRepositorio(contexto);
                    usuarioEmisor = 
                        usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuarioEmisor);
                    usuarioReceptor = 
                        usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuarioReceptor);

                    ValidarUsuariosExistentes(usuarioEmisor, usuarioReceptor);

                    _amistadServicio.CrearSolicitud(usuarioEmisor.idUsuario, 
                        usuarioReceptor.idUsuario);
                }

                NotificarSolicitudNueva(usuarioEmisor, nombreUsuarioEmisor, usuarioReceptor, 
                    nombreUsuarioReceptor);
            }
            catch (FaultException)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn("Regla de negocio violada al enviar solicitud de amistad.", ex);
                throw new FaultException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.Warn("Datos invalidos al enviar solicitud de amistad.", ex);
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error inesperado al enviar solicitud de amistad.", ex);
                throw new FaultException(MensajesError.Cliente.ErrorAlmacenarSolicitud);
            }
        }

        /// <summary>
        /// Responde una solicitud de amistad aceptandola.
        /// Actualiza la solicitud en la base de datos, notifica a ambos usuarios y actualiza sus 
        /// listas de amigos.
        /// </summary>
        /// <param name="nombreUsuarioEmisor">Nombre del usuario que envio la solicitud original.
        /// </param>
        /// <param name="nombreUsuarioReceptor">Nombre del usuario que responde la solicitud.
        /// </param>
        /// <exception cref="FaultException">Se lanza si los nombres son invalidos, los usuarios 
        /// no existen, o hay errores de base de datos.</exception>
        public void ResponderSolicitudAmistad(string nombreUsuarioEmisor, 
            string nombreUsuarioReceptor)
        {
            try
            {
                ValidadorNombreUsuario.Validar(nombreUsuarioEmisor, nameof(nombreUsuarioEmisor));
                ValidadorNombreUsuario.
                    Validar(nombreUsuarioReceptor, nameof(nombreUsuarioReceptor));

                string nombreEmisorNormalizado;
                string nombreReceptorNormalizado;

                using (var contexto = _contextoFactory.CrearContexto())
                {
                    var usuarioRepositorio = new UsuarioRepositorio(contexto);
                    var usuarioEmisor = usuarioRepositorio.ObtenerPorNombreUsuario
                        (nombreUsuarioEmisor);
                    var usuarioReceptor = usuarioRepositorio.ObtenerPorNombreUsuario
                        (nombreUsuarioReceptor);

                    if (usuarioEmisor == null || usuarioReceptor == null)
                    {
                        throw new FaultException
                            (MensajesError.Cliente.UsuariosEspecificadosNoExisten);
                    }

                    _amistadServicio.AceptarSolicitud(usuarioEmisor.idUsuario, 
                        usuarioReceptor.idUsuario);

                    nombreEmisorNormalizado = ValidadorNombreUsuario.ObtenerNombreNormalizado
                        (usuarioEmisor.Nombre_Usuario, nombreUsuarioEmisor);
                    nombreReceptorNormalizado = ValidadorNombreUsuario.ObtenerNombreNormalizado
                        (usuarioReceptor.Nombre_Usuario, nombreUsuarioReceptor);
                }

                NotificarSolicitudAceptada(nombreEmisorNormalizado, nombreReceptorNormalizado);

                _notificadorListaAmigos.NotificarCambioAmistad(nombreEmisorNormalizado);
                _notificadorListaAmigos.NotificarCambioAmistad(nombreReceptorNormalizado);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn("Regla de negocio violada al aceptar solicitud de amistad.", ex);
                throw new FaultException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.Warn("Datos invalidos al aceptar solicitud de amistad.", ex);
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error al responder solicitud de amistad.", ex);
                throw new FaultException(MensajesError.Cliente.ErrorActualizarSolicitud);
            }
        }

        /// <summary>
        /// Elimina la relacion de amistad entre dos usuarios.
        /// Elimina la amistad de la base de datos, notifica a ambos usuarios y actualiza sus 
        /// listas de amigos.
        /// </summary>
        /// <param name="nombreUsuarioA">Nombre del primer usuario.</param>
        /// <param name="nombreUsuarioB">Nombre del segundo usuario.</param>
        /// <exception cref="FaultException">Se lanza si los nombres son invalidos, los usuarios 
        /// no existen, o hay errores de base de datos.</exception>
        public void EliminarAmigo(string nombreUsuarioA, string nombreUsuarioB)
        {
            try
            {
                ValidadorNombreUsuario.Validar(nombreUsuarioA, nameof(nombreUsuarioA));
                ValidadorNombreUsuario.Validar(nombreUsuarioB, nameof(nombreUsuarioB));

                Amigo relacionEliminada;
                string nombreUsuarioANormalizado;
                string nombreUsuarioBNormalizado;

                using (var contexto = _contextoFactory.CrearContexto())
                {
                    var usuarioRepositorio = new UsuarioRepositorio(contexto);
                    var usuarioA = usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuarioA);
                    var usuarioB = usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuarioB);

                    if (usuarioA == null || usuarioB == null)
                    {
                        throw new FaultException
                            (MensajesError.Cliente.UsuariosEspecificadosNoExisten);
                    }

                    relacionEliminada = _amistadServicio.EliminarAmistad
                        (usuarioA.idUsuario, usuarioB.idUsuario);
                    nombreUsuarioANormalizado = ValidadorNombreUsuario.ObtenerNombreNormalizado
                        (usuarioA.Nombre_Usuario, nombreUsuarioA);
                    nombreUsuarioBNormalizado = ValidadorNombreUsuario.ObtenerNombreNormalizado
                        (usuarioB.Nombre_Usuario, nombreUsuarioB);
                }

                NotificarEliminacion(relacionEliminada, nombreUsuarioANormalizado, 
                    nombreUsuarioBNormalizado);

                _notificadorListaAmigos.NotificarCambioAmistad(nombreUsuarioANormalizado);
                _notificadorListaAmigos.NotificarCambioAmistad(nombreUsuarioBNormalizado);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn("Regla de negocio violada al eliminar amistad.", ex);
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error inesperado al eliminar amistad.", ex);
                throw new FaultException(MensajesError.Cliente.ErrorEliminarAmistad);
            }
        }

        private void RegistrarCallback(string nombreUsuario, string nombreNormalizado)
        {
            var callback = ManejadorCallback<IAmigosManejadorCallback>.ObtenerCallbackActual();
            _manejadorCallback.Suscribir(nombreNormalizado, callback);

            if (!string.Equals(nombreUsuario, nombreNormalizado, StringComparison.Ordinal))
            {
                _manejadorCallback.Desuscribir(nombreUsuario);
            }
            _manejadorCallback.ConfigurarEventosCanal(nombreNormalizado);
        }

        private void ValidarUsuariosExistentes(Usuario emisor, Usuario receptor)
        {
            if (emisor == null)
            {
                throw new FaultException(MensajesError.Cliente.JugadorNoAsociado);
            }
            if (receptor == null)
            {
                throw new FaultException(MensajesError.Cliente.UsuarioNoEncontrado);
            }
        }

        private void NotificarSolicitudNueva(Usuario emisor, string nombreEmisorInput, 
            Usuario receptor, string nombreReceptorInput)
        {
            string nombreEmisor = ValidadorNombreUsuario.ObtenerNombreNormalizado
                (emisor.Nombre_Usuario, nombreEmisorInput);
            string nombreReceptor = ValidadorNombreUsuario.ObtenerNombreNormalizado
                (receptor.Nombre_Usuario, nombreReceptorInput);

            var solicitud = new SolicitudAmistadDTO
            {
                UsuarioEmisor = nombreEmisor,
                UsuarioReceptor = nombreReceptor,
                SolicitudAceptada = false
            };

            _notificador.NotificarSolicitudActualizada(nombreReceptor, solicitud);
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

        private void NotificarEliminacion(Amigo relacion, string usuarioA, string usuarioB)
        {
            bool usuarioAEsEmisor = relacion.UsuarioEmisor == relacion.Usuario.idUsuario;
            var solicitud = new SolicitudAmistadDTO
            {
                UsuarioEmisor = usuarioA,
                UsuarioReceptor = usuarioB,
                SolicitudAceptada = false
            };

            _notificador.NotificarAmistadEliminada(usuarioA, solicitud);
            _notificador.NotificarAmistadEliminada(usuarioB, solicitud);
        }
    }
}