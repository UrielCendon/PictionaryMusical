using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf
{
    /// <summary>
    /// Administra la conexion Duplex para mantener actualizada la lista de amigos conectados.
    /// </summary>
    [CallbackBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    public sealed class ListaAmigosServicio : IListaAmigosServicio,
        PictionaryServidorServicioListaAmigos.IListaAmigosManejadorCallback
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ListaAmigosServicio));
        private readonly IWcfClienteFabrica _fabricaClientes;
        private readonly SemaphoreSlim _semaforo = new(1, 1);
        private readonly object _amigosBloqueo = new();
        private readonly List<DTOs.AmigoDTO> _amigos = new();
        private readonly IManejadorErrorServicio _manejadorError;
        private volatile bool _desechado = false;

        private PictionaryServidorServicioListaAmigos.ListaAmigosManejadorClient _cliente;
        private string _usuarioSuscrito;

        /// <summary>
        /// Inicializa el servicio de lista de amigos.
        /// </summary>
        public ListaAmigosServicio(IManejadorErrorServicio manejadorError,
            IWcfClienteFabrica fabricaClientes)
        {
            _manejadorError = manejadorError ??
                throw new ArgumentNullException(nameof(manejadorError));
            _fabricaClientes = fabricaClientes ??
                throw new ArgumentNullException(nameof(fabricaClientes));
        }

        /// <summary>
        /// Evento que se dispara cuando la lista de amigos cambia en el servidor.
        /// </summary>
        public event EventHandler<IReadOnlyList<DTOs.AmigoDTO>> ListaActualizada;

        /// <summary>
        /// Se dispara cuando el canal de comunicacion con el servidor falla o se desconecta.
        /// </summary>
        public event EventHandler CanalDesconectado;

        /// <summary>
        /// Obtiene la coleccion local actual de amigos.
        /// </summary>
        public IReadOnlyList<DTOs.AmigoDTO> ListaActual
        {
            get
            {
                lock (_amigosBloqueo)
                {
                    return _amigos.Count == 0
                        ? Array.Empty<DTOs.AmigoDTO>()
                        : _amigos.ToArray();
                }
            }
        }

        /// <summary>
        /// Establece la conexion y suscripcion para recibir actualizaciones de amigos.
        /// </summary>
        public async Task SuscribirAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException("Usuario obligatorio.", nameof(nombreUsuario));
            }

            if (_desechado)
            {
                return;
            }

            await _semaforo.WaitAsync().ConfigureAwait(false);
            PictionaryServidorServicioListaAmigos.ListaAmigosManejadorClient clienteLocal = null;
            try
            {
                if (EsSuscripcionActiva(nombreUsuario)) return;

                await CancelarSuscripcionInternaAsync();

                clienteLocal = CrearCliente();
                await clienteLocal.SuscribirAsync(nombreUsuario).ConfigureAwait(false);

                _cliente = clienteLocal;
                _usuarioSuscrito = nombreUsuario;
                clienteLocal = null;
            }
            catch (FaultException excepcion)
            {
                LimpiarClienteLocal(clienteLocal);
                ManejarErrorSuscripcion(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                LimpiarClienteLocal(clienteLocal);
                ManejarErrorSuscripcion(excepcion);
            }
            catch (TimeoutException excepcion)
            {
                LimpiarClienteLocal(clienteLocal);
                ManejarErrorSuscripcion(excepcion);
            }
            finally
            {
                LiberarSemaforoSeguro();
            }
        }

        private void LimpiarClienteLocal(
            PictionaryServidorServicioListaAmigos.ListaAmigosManejadorClient cliente)
        {
            if (cliente == null)
            {
                return;
            }
            DesuscribirEventosCanal(cliente);
            cliente.Abort();
        }

        /// <summary>
        /// Cierra la suscripcion y desconecta del servidor.
        /// </summary>
        public async Task CancelarSuscripcionAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return;
            }

            if (_desechado)
            {
                return;
            }

            await _semaforo.WaitAsync().ConfigureAwait(false);
            try
            {
                if (EsSuscripcionActiva(nombreUsuario))
                {
                    await CancelarSuscripcionInternaAsync();
                }
            }
            finally
            {
                LiberarSemaforoSeguro();
            }
        }

        /// <summary>
        /// Aborta la conexion inmediatamente sin esperar una desuscripcion limpia.
        /// </summary>
        public void AbortarConexion()
        {
            var cliente = _cliente;
            _cliente = null;
            _usuarioSuscrito = null;

            if (cliente != null)
            {
                DesuscribirEventosCanal(cliente);
                try
                {
                    cliente.Abort();
                }
                catch (Exception excepcion)
                {
                    _logger.Warn("Error al abortar conexion de lista de amigos.", excepcion);
                }
            }

            lock (_amigosBloqueo)
            {
                _amigos.Clear();
            }
        }

        /// <summary>
        /// Consulta la lista de amigos directamente al servidor.
        /// </summary>
        public async Task<IReadOnlyList<DTOs.AmigoDTO>> ObtenerAmigosAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException("Usuario obligatorio.", nameof(nombreUsuario));
            }

            VerificarConexionRed();

            if (_desechado)
            {
                return Array.Empty<DTOs.AmigoDTO>();
            }

            await _semaforo.WaitAsync().ConfigureAwait(false);

            var clienteExistente = _cliente;
            bool usarClienteExistente = clienteExistente != null && 
                clienteExistente.State == CommunicationState.Opened;
            
            var cliente = usarClienteExistente ? clienteExistente : CrearCliente();
            bool esTemporal = !usarClienteExistente;

            try
            {
                var amigos = await cliente.ObtenerAmigosAsync(nombreUsuario).ConfigureAwait(false);
                var lista = ConvertirLista(amigos);

                if (!esTemporal)
                {
                    ActualizarListaInterna(lista);
                }
                else
                {
                    CerrarClienteSeguro(cliente);
                }

                return lista;
            }
            catch (FaultException excepcion)
            {
                if (esTemporal)
                {
                    cliente.Abort();
                }
                throw ConvertirExcepcion(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                if (esTemporal)
                {
                    cliente.Abort();
                }
                throw ConvertirExcepcion(excepcion);
            }
            catch (TimeoutException excepcion)
            {
                if (esTemporal)
                {
                    cliente.Abort();
                }
                throw ConvertirExcepcion(excepcion);
            }
            finally
            {
                LiberarSemaforoSeguro();
            }
        }

        /// <summary>
        /// Callback del servidor: Actualiza la lista local de amigos.
        /// </summary>
        public void NotificarListaAmigosActualizada(DTOs.AmigoDTO[] amigos)
        {
            var lista = ConvertirLista(amigos);
            ActualizarListaInterna(lista);
            ListaActualizada?.Invoke(this, lista);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_desechado)
            {
                return;
            }

            _desechado = true;
            CerrarClienteSeguro(_cliente);
            _cliente = null;
            _usuarioSuscrito = null;
            _semaforo?.Dispose();
        }

        private bool EsSuscripcionActiva(string usuario)
        {
            return _cliente != null &&
                   string.Equals(_usuarioSuscrito, usuario, StringComparison.OrdinalIgnoreCase);
        }

        private void LiberarSemaforoSeguro()
        {
            if (_desechado)
            {
                return;
            }

            try
            {
                _semaforo.Release();
            }
            catch (ObjectDisposedException)
            {
                _logger.Info("Semaforo ya dispuesto al liberar.");
            }
        }

        private async Task CancelarSuscripcionInternaAsync()
        {
            var cliente = _cliente;
            var usuario = _usuarioSuscrito;

            _cliente = null;
            _usuarioSuscrito = null;

            if (cliente == null)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(usuario))
                {
                    await cliente.CancelarSuscripcionAsync(usuario)
                        .ConfigureAwait(false);
                }
                CerrarClienteSeguro(cliente);
            }
            catch (FaultException excepcion)
            {
                _logger.Warn("Error al cancelar suscripcion.", excepcion);
                cliente.Abort();
            }
            catch (CommunicationException excepcion)
            {
                _logger.Warn("Error al cancelar suscripcion.", excepcion);
                cliente.Abort();
            }
            catch (TimeoutException excepcion)
            {
                _logger.Warn("Error al cancelar suscripcion.", excepcion);
                cliente.Abort();
            }
        }

        private void ManejarErrorSuscripcion(Exception ex)
        {
            _logger.ErrorFormat(
                "Modulo: ListaAmigosServicio - Error al suscribir al servidor. " +
                "Tipo de error: {0}. Posible perdida de conexion de red o servidor no disponible.",
                ex.GetType().Name);
            _cliente?.Abort();
            _cliente = null;
            _usuarioSuscrito = null;
            throw ConvertirExcepcion(ex);
        }

        private Exception ConvertirExcepcion(Exception excepcion)
        {
            if (excepcion is FaultException faultExcepcion)
            {
                _logger.WarnFormat(
                    "Modulo: ListaAmigosServicio - Falla controlada del servidor.");
                string mensaje = _manejadorError.ObtenerMensaje(
                    faultExcepcion,
                    Lang.errorTextoErrorProcesarSolicitud);
                return new ServicioExcepcion(TipoErrorServicio.FallaServicio, mensaje, 
                    faultExcepcion);
            }

            if (excepcion is CommunicationException || excepcion is EndpointNotFoundException)
            {
                _logger.ErrorFormat(
                    "Modulo: ListaAmigosServicio - Error de comunicacion. " +
                    "El servidor puede no estar disponible o hay problemas de conectividad.");
                return new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible,
                    excepcion);
            }

            if (excepcion is TimeoutException)
            {
                _logger.ErrorFormat(
                    "Modulo: ListaAmigosServicio - Tiempo de espera agotado. " +
                    "El servidor no respondio a tiempo.");
                return new ServicioExcepcion(
                    TipoErrorServicio.TiempoAgotado,
                    Lang.errorTextoServidorNoDisponible,
                    excepcion);
            }

            _logger.ErrorFormat(
                "Modulo: ListaAmigosServicio - Error desconocido. Tipo: {0}.",
                excepcion.GetType().Name);
            return new ServicioExcepcion(
                TipoErrorServicio.Desconocido,
                Lang.errorTextoErrorProcesarSolicitud,
                excepcion);
        }

        private PictionaryServidorServicioListaAmigos.ListaAmigosManejadorClient CrearCliente()
        {
            var contexto = new InstanceContext(this);
            var cliente = (PictionaryServidorServicioListaAmigos.ListaAmigosManejadorClient)
                   _fabricaClientes.CrearClienteListaAmigos(contexto);
            
            SuscribirEventosCanal(cliente);
            return cliente;
        }

        private void SuscribirEventosCanal(ICommunicationObject canal)
        {
            if (canal != null)
            {
                canal.Faulted += Canal_Fallido;
                canal.Closed += Canal_Cerrado;
            }
        }

        private void DesuscribirEventosCanal(ICommunicationObject canal)
        {
            if (canal != null)
            {
                canal.Faulted -= Canal_Fallido;
                canal.Closed -= Canal_Cerrado;
            }
        }

        private void Canal_Fallido(object remitente, EventArgs argumentosEvento)
        {
            var canal = remitente as ICommunicationObject;
            string razonDetallada = ObtenerRazonDesconexion(canal);
            
            _logger.ErrorFormat(
                "Modulo: ListaAmigosServicio - El canal de lista de amigos fallo. " +
                "Razon: {0}",
                razonDetallada);
            
            LimpiarEstadoTrasDesconexion();
            CanalDesconectado?.Invoke(this, EventArgs.Empty);
        }

        private static void Canal_Cerrado(object remitente, EventArgs argumentosEvento)
        {
            _logger.Info(
                "Modulo: ListaAmigosServicio - Canal de lista de amigos cerrado normalmente.");
        }

        private static string ObtenerRazonDesconexion(ICommunicationObject canal)
        {
            if (canal == null)
            {
                return "Canal no disponible";
            }

            var clienteCanal = canal as IClientChannel;
            if (clienteCanal?.RemoteAddress != null)
            {
                bool posibleServidorCaido = canal.State == CommunicationState.Faulted;
                string direccionServidor = clienteCanal.RemoteAddress.Uri.ToString();

                if (posibleServidorCaido)
                {
                    return string.Format(
                        "Servidor posiblemente caido o inaccesible. " +
                        "Direccion: {0}. " +
                        "Causas probables: servidor cerrado, timeout por inactividad, " +
                        "o perdida de conexion de red.",
                        direccionServidor);
                }
            }

            return string.Format("Estado del canal: {0}", canal.State);
        }

        private void LimpiarEstadoTrasDesconexion()
        {
            var cliente = _cliente;
            _cliente = null;
            _usuarioSuscrito = null;

            if (cliente != null)
            {
                DesuscribirEventosCanal(cliente);
                try
                {
                    cliente.Abort();
                }
                catch (Exception excepcion)
                {
                    _logger.Warn("Error al abortar cliente tras desconexion.", excepcion);
                }
            }

            lock (_amigosBloqueo)
            {
                _amigos.Clear();
            }
        }

        /// <summary>
        /// Cierra el cliente WCF de forma segura, abortando si el cierre normal falla.
        /// El catch general es necesario porque el cierre de WCF puede lanzar multiples 
        /// tipos de excepciones y siempre debe intentarse Abort como fallback.
        /// </summary>
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
            catch (FaultException)
            {
                cliente.Abort();
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

        private static IReadOnlyList<DTOs.AmigoDTO> ConvertirLista(
            IEnumerable<DTOs.AmigoDTO> amigos)
        {
            if (amigos == null)
            {
                return Array.Empty<DTOs.AmigoDTO>();
            }

            var lista = amigos
                .Where(a => !string.IsNullOrWhiteSpace(a?.NombreUsuario))
                .Select(a => new DTOs.AmigoDTO
                {
                    UsuarioId = a.UsuarioId,
                    NombreUsuario = a.NombreUsuario
                })
                .ToList();

            return lista.AsReadOnly();
        }

        private void ActualizarListaInterna(IReadOnlyList<DTOs.AmigoDTO> lista)
        {
            lock (_amigosBloqueo)
            {
                _amigos.Clear();
                _amigos.AddRange(lista);
            }
        }

        private static void VerificarConexionRed()
        {
            if (!ConectividadRedMonitor.HayConexion)
            {
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible);
            }
        }
    }
}