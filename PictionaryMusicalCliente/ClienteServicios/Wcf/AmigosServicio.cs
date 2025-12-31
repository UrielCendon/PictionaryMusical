using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Wcf.Administrador;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Gestiona la comunicacion Duplex para solicitudes de amistad en tiempo real.
    /// </summary>
    [CallbackBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    public class AmigosServicio : IAmigosServicio,
        PictionaryServidorServicioAmigos.IAmigosManejadorCallback
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AmigosServicio));
        private readonly SemaphoreSlim _semaforo = new(1, 1);
        private readonly IWcfClienteFabrica _fabricaClientes; 
        private readonly ISolicitudesAmistadAdministrador _administradorSolicitudes;
        private readonly IManejadorErrorServicio _manejadorError;

        private PictionaryServidorServicioAmigos.AmigosManejadorClient _cliente;
        private string _usuarioSuscrito;
        private bool _recursosLiberados;
        private bool _huboErrorCargaSolicitudes;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de amigos.
        /// </summary>
        /// <param name="administradorSolicitudes">Administrador de solicitudes de amistad.</param>
        /// <param name="manejadorError">Manejador para procesar errores de servicio.</param>
        /// <param name="fabricaClientes">Fabrica para crear clientes WCF.</param>
        /// <exception cref="ArgumentNullException">
        /// Si alguna dependencia es nula.
        /// </exception>
        public AmigosServicio(
            ISolicitudesAmistadAdministrador administradorSolicitudes,
            IManejadorErrorServicio manejadorError, 
            IWcfClienteFabrica fabricaClientes)
        {
            _administradorSolicitudes = administradorSolicitudes ??
                throw new ArgumentNullException(nameof(administradorSolicitudes));
            _manejadorError = manejadorError ??
                throw new ArgumentNullException(nameof(manejadorError));
            _fabricaClientes = fabricaClientes ??
                throw new ArgumentNullException(nameof(fabricaClientes));
        }

        /// <summary>
        /// Evento disparado al recibir cambios desde el servidor.
        /// </summary>
        public event EventHandler<IReadOnlyCollection<DTOs.SolicitudAmistadDTO>>
            SolicitudesActualizadas;

        /// <summary>
        /// Se dispara cuando el canal de comunicacion con el servidor falla o se desconecta.
        /// </summary>
        public event EventHandler CanalDesconectado;
        
        /// <summary>
        /// Obtiene una copia segura de las solicitudes actuales.
        /// </summary>
        public IReadOnlyCollection<DTOs.SolicitudAmistadDTO> SolicitudesPendientes
        {
            get
            {
                return _administradorSolicitudes.ObtenerSolicitudes();
            }
        }

        /// <summary>
        /// Indica si hubo un error al cargar las solicitudes desde el servidor.
        /// </summary>
        public bool HuboErrorCargaSolicitudes => _huboErrorCargaSolicitudes;

        /// <summary>
        /// Conecta al usuario al servicio de notificaciones de amistad.
        /// </summary>
        public async Task SuscribirAsync(string nombreUsuario)
        {
            ValidarNombreUsuario(nombreUsuario);

            await EjecutarEnSeccionCriticaAsync(async () =>
            {
                if (EsSuscripcionActual(nombreUsuario))
                {
                    return;
                }
                await SuscribirNuevoClienteAsync(nombreUsuario);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Desconecta al usuario y libera el canal de comunicacion.
        /// </summary>
        public async Task CancelarSuscripcionAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return;
            }

            await EjecutarEnSeccionCriticaAsync(async () =>
            {
                if (CoincideUsuarioSuscrito(nombreUsuario))
                {
                    await CancelarSuscripcionInternaAsync();
                }
            }).ConfigureAwait(false);

        }

        /// <summary>
        /// Envia una nueva peticion de amistad al servidor.
        /// </summary>
        public Task EnviarSolicitudAsync(string nombreUsuarioEmisor, string nombreUsuarioReceptor) 
            => EjecutarOperacionAsync(c => 
            c.EnviarSolicitudAmistadAsync(nombreUsuarioEmisor, nombreUsuarioReceptor));

        /// <summary>
        /// Responde a una peticion existente (aceptar/rechazar).
        /// </summary>
        public Task ResponderSolicitudAsync
            (string nombreUsuarioEmisor, string nombreUsuarioReceptor) =>
            EjecutarOperacionAsync(c => 
            c.ResponderSolicitudAmistadAsync(nombreUsuarioEmisor, nombreUsuarioReceptor));

        /// <summary>
        /// Elimina a un amigo de la lista de contactos.
        /// </summary>
        public Task EliminarAmigoAsync(string nombreUsuarioA, string nombreUsuarioB) =>
            EjecutarOperacionAsync(c => c.EliminarAmigoAsync(nombreUsuarioA, nombreUsuarioB));

        /// <summary>
        /// Callback del servidor: Notifica que una solicitud cambio de estado.
        /// </summary>
        public void NotificarSolicitudActualizada(DTOs.SolicitudAmistadDTO solicitud)
        {
            ProcesarNotificacion(solicitud, EjecutarActualizacionSolicitud);
        }

        /// <summary>
        /// Callback del servidor: Notifica que una amistad ha sido eliminada.
        /// </summary>
        public void NotificarAmistadEliminada(DTOs.SolicitudAmistadDTO solicitud)
        {
            ProcesarNotificacion(solicitud, EjecutarEliminacionAmistad);
        }

        private bool EjecutarActualizacionSolicitud(
            DTOs.SolicitudAmistadDTO solicitud, 
            string usuario)
        {
            return _administradorSolicitudes.ActualizarSolicitud(solicitud, usuario);
        }

        private bool EjecutarEliminacionAmistad(
            DTOs.SolicitudAmistadDTO solicitud, 
            string usuario)
        {
            return _administradorSolicitudes.EliminaAmistadParaUsuario(solicitud, usuario);
        }

        /// <summary>
        /// Libera los recursos del cliente WCF y semaforos.
        /// </summary>
        /// <param name="liberando">
        /// True si se llama desde Dispose, false desde el finalizador.
        /// </param>
        protected virtual void Dispose(bool liberando)
        {
            if (_recursosLiberados)
            {
                return;
            }

            if (liberando)
            {
                CerrarClienteSeguro(_cliente);
                LimpiarEstadoLocal();
                _semaforo?.Dispose();
            }

            _recursosLiberados = true;
        }

        /// <summary>
        /// Implementacion de IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(liberando: true);
            GC.SuppressFinalize(this);
        }

        private void ProcesarNotificacion(
            DTOs.SolicitudAmistadDTO solicitud,
            Func<DTOs.SolicitudAmistadDTO, string, bool> accionActualizacion)
        {
            if (!EsSolicitudValida(solicitud)) return;

            string usuarioActual = _usuarioSuscrito;
            if (string.IsNullOrWhiteSpace(usuarioActual)) return;

            bool modificada = accionActualizacion(solicitud, usuarioActual);
            if (modificada)
            {
                NotificarSolicitudesActualizadas();
            }
        }

        private async Task EjecutarOperacionAsync(
            Func<PictionaryServidorServicioAmigos.AmigosManejadorClient, Task> operacion)
        {
            await _semaforo.WaitAsync().ConfigureAwait(false);
            try
            {
                var clienteExistente = _cliente;
                bool usarClienteExistente = clienteExistente != null && 
                    clienteExistente.State == CommunicationState.Opened;
                
                var cliente = usarClienteExistente ? clienteExistente : CrearCliente();
                bool esTemporal = !usarClienteExistente;

                await EjecutarLogicaClienteAsync(cliente, operacion, esTemporal);
            }
            finally
            {
                _semaforo.Release();
            }
        }

        private async Task EjecutarLogicaClienteAsync(
            PictionaryServidorServicioAmigos.AmigosManejadorClient cliente,
            Func<PictionaryServidorServicioAmigos.AmigosManejadorClient, Task> operacion,
            bool esTemporal)
        {
            try
            {
                await operacion(cliente).ConfigureAwait(false);
                if (esTemporal)
                {
                    CerrarClienteSeguro(cliente);
                }
            }
            catch (FaultException excepcion)
            {
                await ManejarErrorOperacionAsync(excepcion, cliente, esTemporal);
            }
            catch (CommunicationException excepcion)
            {
                await ManejarErrorOperacionAsync(excepcion, cliente, esTemporal);
            }
            catch (TimeoutException excepcion)
            {
                await ManejarErrorOperacionAsync(excepcion, cliente, esTemporal);
            }
        }

        private async Task ManejarErrorOperacionAsync(
            Exception excepcion,
            ICommunicationObject cliente,
            bool esTemporal)
        {
            if (esTemporal)
            {
                cliente.Abort();
            }
            else if (EsErrorComunicacion(excepcion))
            {
                _logger.Warn("Error comunicacion permanente. Intentando reconexion.");
                await IntentarReconexionAsync();
            }

            LanzarExcepcionServicio(excepcion, Lang.errorTextoErrorProcesarSolicitud);
        }

        private async Task IntentarReconexionAsync()
        {
            string usuario = _usuarioSuscrito;
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return;
            }

            await CancelarSuscripcionInternaAsync();

            try
            {
                await SuscribirNuevoClienteAsync(usuario);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Error("Fallo critico al reconectar suscripcion.", excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Error("Timeout al reconectar suscripcion.", excepcion);
            }
            catch (ServicioExcepcion excepcion)
            {
                _logger.Error("Error de servicio al reconectar suscripcion.", excepcion);
            }
        }

        private async Task SuscribirNuevoClienteAsync(string nombreUsuario)
        {
            await CancelarSuscripcionInternaAsync();
            LimpiarEstadoLocal();

            var cliente = CrearCliente();
            _cliente = cliente;
            _usuarioSuscrito = nombreUsuario;

            try
            {
                await cliente.SuscribirAsync(nombreUsuario).ConfigureAwait(false);
                _huboErrorCargaSolicitudes = false;
                NotificarSolicitudesActualizadas();
            }
            catch (FaultException excepcion)
            {
                _huboErrorCargaSolicitudes = true;
                AbortarYLimpiar(cliente);
                LanzarExcepcionServicio(excepcion, Lang.errorTextoErrorProcesarSolicitud);
            }
            catch (CommunicationException excepcion)
            {
                _huboErrorCargaSolicitudes = true;
                AbortarYLimpiar(cliente);
                LanzarExcepcionServicio(excepcion, Lang.errorTextoErrorProcesarSolicitud);
            }
            catch (TimeoutException excepcion)
            {
                _huboErrorCargaSolicitudes = true;
                AbortarYLimpiar(cliente);
                LanzarExcepcionServicio(excepcion, Lang.errorTextoErrorProcesarSolicitud);
            }
        }

        private void AbortarYLimpiar(ICommunicationObject cliente)
        {
            cliente.Abort();
            LimpiarEstadoLocal();
        }

        private async Task CancelarSuscripcionInternaAsync()
        {
            var cliente = _cliente;
            var usuario = _usuarioSuscrito;

            LimpiarEstadoLocal();

            if (cliente != null)
            {
                await IntentarCancelarEnServidorAsync(cliente, usuario);
                CerrarClienteSeguro(cliente);
            }
        }

        private static async Task IntentarCancelarEnServidorAsync(
            PictionaryServidorServicioAmigos.AmigosManejadorClient cliente,
            string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario) ||
                cliente.State != CommunicationState.Opened)
            {
                return;
            }

            try
            {
                await cliente.CancelarSuscripcionAsync(usuario).ConfigureAwait(false);
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn("No se pudo cancelar suscripcion en servidor.", excepcion);
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn("Timeout al cancelar suscripcion en servidor.", excepcion);
            }
        }

        private void LanzarExcepcionServicio(Exception excepcion, string mensajeDefault)
        {
            if (excepcion is FaultException fault)
            {
                string mensaje = _manejadorError.ObtenerMensaje(fault, mensajeDefault);
                throw new ServicioExcepcion(TipoErrorServicio.FallaServicio, mensaje, excepcion);
            }

            if (excepcion is TimeoutException)
            {
                throw new ServicioExcepcion(
                    TipoErrorServicio.TiempoAgotado,
                    Lang.errorTextoServidorTiempoAgotado,
                    excepcion);
            }

            if (EsErrorComunicacion(excepcion))
            {
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible,
                    excepcion);
            }

            throw new ServicioExcepcion(TipoErrorServicio.Desconocido, mensajeDefault, excepcion);
        }

        private static bool EsErrorComunicacion(Exception excepcion)
        {
            return excepcion is CommunicationException || excepcion is EndpointNotFoundException;
        }

        private void LimpiarEstadoLocal()
        {
            _cliente = null;
            _usuarioSuscrito = null;
            _administradorSolicitudes.LimpiarSolicitudes();
            NotificarSolicitudesActualizadas();
        }

        private void CerrarClienteSeguro(ICommunicationObject cliente)
        {
            if (cliente == null)
            {
                return;
            }

            DesuscribirEventosCanal(cliente);

            try
            {
                if (cliente.State == CommunicationState.Opened)
                {
                    cliente.Close();
                }
                else
                {
                    cliente.Abort();
                }
            }
            catch (CommunicationException)
            {
                cliente.Abort();
            }
            catch (TimeoutException)
            {
                cliente.Abort();
            }
        }

        private PictionaryServidorServicioAmigos.AmigosManejadorClient CrearCliente()
        {
            var contexto = new InstanceContext(this);
            var cliente = (PictionaryServidorServicioAmigos.AmigosManejadorClient)
                   _fabricaClientes.CrearClienteAmigos(contexto);
            
            SuscribirEventosCanal(cliente);
            return cliente;
        }

        private void SuscribirEventosCanal(ICommunicationObject canal)
        {
            if (canal != null)
            {
                canal.Faulted += Canal_Faulted;
            }
        }

        private void DesuscribirEventosCanal(ICommunicationObject canal)
        {
            if (canal != null)
            {
                canal.Faulted -= Canal_Faulted;
            }
        }

        private void Canal_Faulted(object remitente, EventArgs argumentosEvento)
        {
            _logger.Error("El canal de amigos entro en estado Faulted.");
            CanalDesconectado?.Invoke(this, EventArgs.Empty);
        }

        private async Task EjecutarEnSeccionCriticaAsync(Func<Task> accion)
        {
            await _semaforo.WaitAsync().ConfigureAwait(false);
            try
            {
                await accion().ConfigureAwait(false);
            }
            finally
            {
                _semaforo.Release();
            }
        }

        private void NotificarSolicitudesActualizadas()
        {
            SolicitudesActualizadas?.Invoke(this, _administradorSolicitudes.ObtenerSolicitudes());
        }

        private bool EsSuscripcionActual(string usuario)
        {
            return string.Equals(_usuarioSuscrito, usuario, StringComparison.OrdinalIgnoreCase)
                   && _cliente != null;
        }

        private bool CoincideUsuarioSuscrito(string usuario)
        {
            return string.Equals(_usuarioSuscrito, usuario, StringComparison.OrdinalIgnoreCase);
        }

        private static void ValidarNombreUsuario(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Usuario obligatorio.", nameof(nombre));
        }

        private static bool EsSolicitudValida(DTOs.SolicitudAmistadDTO s)
        {
            return s != null &&
                   !string.IsNullOrWhiteSpace(s.UsuarioEmisor) &&
                   !string.IsNullOrWhiteSpace(s.UsuarioReceptor);
        }
    }
}