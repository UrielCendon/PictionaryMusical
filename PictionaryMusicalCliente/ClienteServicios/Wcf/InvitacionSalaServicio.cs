using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.ClienteServicios.Wcf.Implementacion
{
    /// <summary>
    /// Administra la conexion Duplex para mantener actualizada la lista de amigos.
    /// </summary>
    public sealed class ListaAmigosServicio : IListaAmigosServicio,
        PictionaryServidorServicioListaAmigos.IListaAmigosManejadorCallback
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ListaAmigosServicio));
        private const string Endpoint = "NetTcpBinding_IListaAmigosManejador";

        private readonly SemaphoreSlim _semaforo = new(1, 1);
        private readonly object _amigosBloqueo = new();
        private readonly List<DTOs.AmigoDTO> _amigos = new();
        private readonly IManejadorErrorServicio _manejadorError;

        private PictionaryServidorServicioListaAmigos.ListaAmigosManejadorClient _cliente;
        private string _usuarioSuscrito;

        /// <summary>
        /// Inicializa el servicio de lista de amigos.
        /// </summary>
        public ListaAmigosServicio(IManejadorErrorServicio manejadorError)
        {
            _manejadorError = manejadorError ??
                throw new ArgumentNullException(nameof(manejadorError));
        }

        /// <inheritdoc />
        public event EventHandler<IReadOnlyList<DTOs.AmigoDTO>> ListaActualizada;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task SuscribirAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("Usuario obligatorio.", nameof(nombreUsuario));

            await _semaforo.WaitAsync().ConfigureAwait(false);
            try
            {
                if (EsSuscripcionActiva(nombreUsuario)) return;

                await CancelarSuscripcionInternaAsync();

                var cliente = CrearCliente();
                await cliente.SuscribirAsync(nombreUsuario).ConfigureAwait(false);

                _cliente = cliente;
                _usuarioSuscrito = nombreUsuario;
                _logger.InfoFormat("Usuario '{0}' suscrito a amigos.", nombreUsuario);
            }
            catch (Exception ex)
            {
                ManejarErrorSuscripcion(ex);
            }
            finally
            {
                _semaforo.Release();
            }
        }

        /// <inheritdoc />
        public async Task CancelarSuscripcionAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario)) return;

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
                _semaforo.Release();
            }
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<DTOs.AmigoDTO>> ObtenerAmigosAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("Usuario obligatorio.", nameof(nombreUsuario));

            await _semaforo.WaitAsync().ConfigureAwait(false);

            var cliente = _cliente ?? CrearCliente();
            bool esTemporal = (_cliente == null);

            try
            {
                var amigos = await cliente.ObtenerAmigosAsync(nombreUsuario).ConfigureAwait(false);
                var lista = ConvertirLista(amigos);

                if (!esTemporal) ActualizarListaInterna(lista);
                else CerrarClienteSeguro(cliente);

                return lista;
            }
            catch (Exception ex)
            {
                if (esTemporal) cliente.Abort();
                throw ConvertirExcepcion(ex);
            }
            finally
            {
                _semaforo.Release();
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

        private async Task CancelarSuscripcionInternaAsync()
        {
            var cliente = _cliente;
            var usuario = _usuarioSuscrito;

            _cliente = null;
            _usuarioSuscrito = null;

            if (cliente == null) return;

            try
            {
                if (!string.IsNullOrWhiteSpace(usuario))
                {
                    await cliente.CancelarSuscripcionAsync(usuario).ConfigureAwait(false);
                }
                CerrarClienteSeguro(cliente);
            }
            catch (Exception ex)
            {
                _logger.Warn("Error al cancelar suscripcion.", ex);
                cliente.Abort();
            }
        }

        private void ManejarErrorSuscripcion(Exception ex)
        {
            _cliente?.Abort();
            _cliente = null;
            _usuarioSuscrito = null;
            throw ConvertirExcepcion(ex);
        }

        private Exception ConvertirExcepcion(Exception ex)
        {
            if (ex is FaultException fe)
            {
                string msg = _manejadorError.ObtenerMensaje(
                    fe,
                    Lang.errorTextoErrorProcesarSolicitud);
                return new ServicioExcepcion(TipoErrorServicio.FallaServicio, msg, fe);
            }

            if (ex is CommunicationException || ex is EndpointNotFoundException)
            {
                return new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion,
                    Lang.errorTextoServidorNoDisponible,
                    ex);
            }

            if (ex is TimeoutException)
            {
                return new ServicioExcepcion(
                    TipoErrorServicio.TiempoAgotado,
                    Lang.errorTextoServidorTiempoAgotado,
                    ex);
            }

            return new ServicioExcepcion(
                TipoErrorServicio.Desconocido,
                Lang.errorTextoErrorProcesarSolicitud,
                ex);
        }

        private PictionaryServidorServicioListaAmigos.ListaAmigosManejadorClient CrearCliente()
        {
            return new PictionaryServidorServicioListaAmigos.ListaAmigosManejadorClient(
                new InstanceContext(this),
                Endpoint);
        }

        private static void CerrarClienteSeguro(ICommunicationObject cliente)
        {
            if (cliente == null) return;
            try
            {
                if (cliente.State == CommunicationState.Opened) cliente.Close();
                else cliente.Abort();
            }
            catch (Exception)
            {
                cliente.Abort();
            }
        }

        private static IReadOnlyList<DTOs.AmigoDTO> ConvertirLista(
            IEnumerable<DTOs.AmigoDTO> amigos)
        {
            if (amigos == null) return Array.Empty<DTOs.AmigoDTO>();

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
    }
}