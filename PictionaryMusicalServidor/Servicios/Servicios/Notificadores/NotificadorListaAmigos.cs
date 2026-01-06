using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Datos.DAL.Interfaces;
using PictionaryMusicalServidor.Servicios.Contratos;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Notificadores
{
    /// <summary>
    /// Servicio especializado en notificaciones de cambios en lista de amigos.
    /// Gestiona las notificaciones cuando la lista de amigos de un usuario cambia.
    /// </summary>
    internal class NotificadorListaAmigos : INotificadorListaAmigos
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(NotificadorListaAmigos));
        private readonly IManejadorCallback<IListaAmigosManejadorCallback> _manejadorCallback;
        private readonly IAmistadServicio _amistadServicio;
        private readonly IContextoFactoria _contextoFactoria;
        private readonly IRepositorioFactoria _repositorioFactoria;

        /// <summary>
        /// Constructor con inyeccion de dependencias.
        /// </summary>
        /// <param name="manejadorCallback">Manejador de callbacks.</param>
        /// <param name="amistadServicio">Servicio de amistad.</param>
        /// <param name="repositorioFactoria">Factoria de repositorios.</param>
        public NotificadorListaAmigos(
            IManejadorCallback<IListaAmigosManejadorCallback> manejadorCallback, 
            IAmistadServicio amistadServicio,
            IRepositorioFactoria repositorioFactoria)
            : this(manejadorCallback, amistadServicio, new ContextoFactoria(), repositorioFactoria)
        {
        }

        /// <summary>
        /// Constructor completo con inyeccion de dependencias para pruebas unitarias.
        /// </summary>
        public NotificadorListaAmigos(
            IManejadorCallback<IListaAmigosManejadorCallback> manejadorCallback, 
            IAmistadServicio amistadServicio,
            IContextoFactoria contextoFactoria,
            IRepositorioFactoria repositorioFactoria)
        {
            _manejadorCallback = manejadorCallback;
            _amistadServicio = amistadServicio;
            _contextoFactoria = contextoFactoria 
                ?? throw new ArgumentNullException(nameof(contextoFactoria));
            _repositorioFactoria = repositorioFactoria 
                ?? throw new ArgumentNullException(nameof(repositorioFactoria));
        }

        /// <summary>
        /// Notifica a un usuario sobre cambios en su lista de amigos.
        /// </summary>
        /// <param name="nombreUsuario">Usuario a notificar.</param>
        public void NotificarCambioAmistad(string nombreUsuario)
        {
            if (!EntradaComunValidador.EsMensajeValido(nombreUsuario))
            {
                return;
            }

            try
            {
                List<AmigoDTO> amigos = ObtenerAmigosPorNombre(nombreUsuario);
                NotificarLista(nombreUsuario, amigos);
            }
            catch (FaultException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorObtenerListaAmigosNotificacion, 
                    excepcion);
            }
            catch (ArgumentOutOfRangeException excepcion)
            {
                _logger.Warn(
                    MensajesError.Bitacora.IdentificadorInvalidoListaAmigos, excepcion);
            }
            catch (ArgumentException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.DatosInvalidosActualizarListaAmigos, 
                    excepcion);
            }
            catch (DataException excepcion)
            {
                _logger.Error(
                    MensajesError.Bitacora.ErrorDatosObtenerAmigos, 
                    excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.OperacionInvalidaListaAmigos, excepcion);
            }
            catch (Exception excepcion)
            {
                _logger.Warn(MensajesError.Bitacora.ErrorInesperadoListaAmigos, excepcion);
            }
        }

        /// <summary>
        /// Notifica la lista actualizada de amigos a un usuario.
        /// </summary>
        /// <param name="nombreUsuario">Usuario a notificar.</param>
        /// <param name="amigos">Lista actualizada de amigos.</param>
        public void NotificarLista(string nombreUsuario, List<AmigoDTO> amigos)
        {
            IListaAmigosManejadorCallback callback = 
                _manejadorCallback.ObtenerCallback(nombreUsuario);
            if (callback != null)
            {
                try
                {
                    callback.NotificarListaAmigosActualizada(amigos);
                }
                catch (Exception excepcion)
                {
                    _logger.Warn(
                        MensajesError.Bitacora.ErrorNotificarListaAmigosActualizada, 
                        excepcion);
                }
            }
        }

        private List<AmigoDTO> ObtenerAmigosPorNombre(string nombreUsuario)
        {
            using (var contexto = _contextoFactoria.CrearContexto())
            {
                var usuarioRepositorio = _repositorioFactoria.CrearUsuarioRepositorio(contexto);
                var usuario = usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuario);
                if (usuario == null)
                {
                    throw new FaultException(MensajesError.Cliente.UsuarioNoEncontrado);
                }

                return _amistadServicio.ObtenerAmigosDTO(usuario.idUsuario);
            }
        }
    }
}
