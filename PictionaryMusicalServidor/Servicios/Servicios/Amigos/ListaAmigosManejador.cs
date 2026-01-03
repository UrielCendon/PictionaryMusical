using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;
using Datos.Modelo;
using log4net;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Servicios.Servicios.Amigos
{
    /// <summary>
    /// Implementacion del servicio de gestion de listas de amigos.
    /// Maneja suscripciones para notificaciones de cambios en listas de amigos con notificaciones
    /// en tiempo real via callbacks.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ListaAmigosManejador : IListaAmigosManejador
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(ListaAmigosManejador));

        private readonly ManejadorCallback<IListaAmigosManejadorCallback> _manejadorCallback;
        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;
        private readonly IAmistadServicio _amistadServicio;
        private readonly INotificadorListaAmigos _notificador;

        /// <summary>
        /// Constructor por defecto para uso en WCF.
        /// Usa CallbacksCompartidos para asegurar que AmigosManejador pueda
        /// notificar a los clientes suscritos aqui.
        /// </summary>
        public ListaAmigosManejador() : this(
            new ContextoFactoria(),
            new RepositorioFactoria(),
            new AmistadServicio(),
            new NotificadorListaAmigos(
                CallbacksCompartidos.ListaAmigos,
                new AmistadServicio(),
                new RepositorioFactoria()))
        {
        }

        /// <summary>
        /// Constructor con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        /// <param name="contextoFactoria">Factoria para crear contextos de base de datos.</param>
        /// <param name="repositorioFactoria">Factoria para crear repositorios.</param>
        /// <param name="amistadServicio">Servicio de amistad.</param>
        /// <param name="notificador">Notificador de lista de amigos.</param>
        public ListaAmigosManejador(
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria,
            IAmistadServicio amistadServicio,
            INotificadorListaAmigos notificador)
        {
            _contextoFactoria = contextoFactoria ??
                throw new ArgumentNullException(nameof(contextoFactoria));
            _repositorioFactoria = repositorioFactoria ??
                throw new ArgumentNullException(nameof(repositorioFactoria));
            _amistadServicio = amistadServicio ??
                throw new ArgumentNullException(nameof(amistadServicio));
            _notificador = notificador ??
                throw new ArgumentNullException(nameof(notificador));

            _manejadorCallback = CallbacksCompartidos.ListaAmigos;
        }

        /// <summary>
        /// Suscribe un usuario para recibir notificaciones sobre cambios en su lista 
        /// de amigos. Obtiene la lista actual de amigos, registra el callback y notifica 
        /// inmediatamente.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario a suscribir.</param>
        public void Suscribir(string nombreUsuario)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(
                    nombreUsuario, 
                    nameof(nombreUsuario));

                var amigosActuales = ObtenerAmigosPorNombre(nombreUsuario);

                IListaAmigosManejadorCallback callback =
                    ManejadorCallback<IListaAmigosManejadorCallback>.ObtenerCallbackActual();

                _manejadorCallback.Suscribir(nombreUsuario, callback);
                _manejadorCallback.ConfigurarEventosCanal(nombreUsuario);

                _notificador.NotificarLista(nombreUsuario, amigosActuales);
            }
            catch (ArgumentOutOfRangeException excepcion)
            {
                _logger.Warn(
                    "El identificador del usuario esta fuera de rango " +
                    "al intentar suscribirse a la lista de amigos.",
                    excepcion);
                throw new FaultException(excepcion.Message);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(
                    "El nombre de usuario tiene un formato invalido " +
                    "para suscribirse a la lista de amigos.",
                    excepcion);
                throw new FaultException(excepcion.Message);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    "No se pudo actualizar la base de datos al suscribir " +
                    "al usuario a la lista de amigos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorSuscripcionAmigos);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    "La conexion a la base de datos fallo al suscribir " +
                    "al usuario a la lista de amigos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorSuscripcionAmigos);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    "Los datos del usuario son invalidos " +
                    "al intentar suscribirse a la lista de amigos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorSuscripcionAmigos);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Ocurrio un error desconocido al suscribir al usuario " +
                    "a la lista de amigos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorSuscripcionAmigos);
            }
        }

        /// <summary>
        /// Cancela la suscripcion de un usuario de notificaciones de lista de amigos.
        /// Elimina el callback del usuario del manejador de callbacks.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario que cancela la suscripcion.</param>
        public void CancelarSuscripcion(string nombreUsuario)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(
                    nombreUsuario, 
                    nameof(nombreUsuario));
                _manejadorCallback.Desuscribir(nombreUsuario);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(
                    "El nombre de usuario tiene un formato invalido " +
                    "para cancelar la suscripcion a la lista de amigos.",
                    excepcion);
                throw new FaultException(excepcion.Message);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(
                    "Ocurrio un error inesperado al cancelar la suscripcion " +
                    "del usuario a la lista de amigos.",
                    excepcion);
                throw new FaultException(excepcion.Message);
            }
        }

        /// <summary>
        /// Obtiene la lista de amigos de un usuario especifico.
        /// Recupera todos los amigos del usuario desde la base de datos.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario cuya lista se desea obtener.</param>
        /// <returns>Lista de amigos del usuario.</returns>
        public List<AmigoDTO> ObtenerAmigos(string nombreUsuario)
        {
            try
            {
                EntradaComunValidador.ValidarNombreUsuario(
                    nombreUsuario, 
                    nameof(nombreUsuario));
                return ObtenerAmigosPorNombre(nombreUsuario);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(
                    "El nombre de usuario tiene un formato invalido " +
                    "para obtener la lista de amigos.",
                    excepcion);
                throw new FaultException(excepcion.Message);
            }
            catch (DbUpdateException excepcion)
            {
                _logger.Error(
                    "No se pudo consultar la base de datos para obtener " +
                    "la lista de amigos del usuario.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarListaAmigos);
            }
            catch (EntityException excepcion)
            {
                _logger.Error(
                    "La conexion a la base de datos fallo al obtener " +
                    "la lista de amigos del usuario.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarListaAmigos);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    "Los datos del usuario son invalidos " +
                    "al intentar obtener su lista de amigos.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarListaAmigos);
            }
            catch (Exception excepcion)
            {
                _logger.Error(
                    "Ocurrio un error desconocido al obtener la lista " +
                    "de amigos del usuario.",
                    excepcion);
                throw new FaultException(MensajesError.Cliente.ErrorRecuperarListaAmigos);
            }
        }

        private List<AmigoDTO> ObtenerAmigosPorNombre(string nombreUsuario)
        {
            using (var contexto = _contextoFactoria.CrearContexto())
            {
                var usuarioRepositorio = 
                    _repositorioFactoria.CrearUsuarioRepositorio(contexto);
                Usuario usuario = usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuario);

                if (usuario == null)
                {
                    throw new FaultException(MensajesError.Cliente.UsuarioNoEncontrado);
                }

                return _amistadServicio.ObtenerAmigosDTO(usuario.idUsuario);
            }
        }
    }
}
