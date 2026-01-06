using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.VentanaPrincipal.Auxiliares
{
    /// <summary>
    /// Gestiona la lista de amigos del usuario, incluyendo suscripciones WCF,
    /// actualizaciones y operaciones de eliminacion.
    /// </summary>
    public sealed class GestorListaAmigos : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IListaAmigosServicio _listaAmigosServicio;
        private readonly IAmigosServicio _amigosServicio;
        private readonly string _nombreUsuario;
        private readonly Action<Action> _ejecutarEnDispatcher;

        private bool _suscripcionActiva;
        private bool _canalDisponible;
        private bool _desconexionProcesada;
        private bool _actualizacionEnProgreso;
        private bool _disposed;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GestorListaAmigos"/>.
        /// </summary>
        /// <param name="parametros">Parametros de configuracion del gestor.</param>
        public GestorListaAmigos(GestorListaAmigosParametros parametros)
        {
            if (parametros == null)
            {
                throw new ArgumentNullException(nameof(parametros));
            }

            _listaAmigosServicio = parametros.ListaAmigosServicio ??
                throw new ArgumentNullException(nameof(parametros.ListaAmigosServicio));
            _amigosServicio = parametros.AmigosServicio ??
                throw new ArgumentNullException(nameof(parametros.AmigosServicio));
            _nombreUsuario = parametros.NombreUsuario;
            _ejecutarEnDispatcher = parametros.EjecutarEnDispatcher ??
                throw new ArgumentNullException(nameof(parametros.EjecutarEnDispatcher));

            Amigos = new ObservableCollection<DTOs.AmigoDTO>();
            SuscribirEventos();
        }

        /// <summary>
        /// Obtiene la coleccion observable de amigos.
        /// </summary>
        public ObservableCollection<DTOs.AmigoDTO> Amigos { get; }

        /// <summary>
        /// Obtiene el amigo actualmente seleccionado.
        /// </summary>
        public DTOs.AmigoDTO AmigoSeleccionado { get; private set; }

        /// <summary>
        /// Indica si el canal de comunicacion de amigos esta disponible.
        /// </summary>
        public bool CanalDisponible => _canalDisponible;

        /// <summary>
        /// Indica si hubo error al cargar solicitudes de amistad.
        /// </summary>
        public bool HuboErrorCargaSolicitudes => _amigosServicio.HuboErrorCargaSolicitudes;

        /// <summary>
        /// Obtiene las solicitudes de amistad pendientes.
        /// </summary>
        public IReadOnlyCollection<DTOs.SolicitudAmistadDTO> SolicitudesPendientes
            => _amigosServicio?.SolicitudesPendientes;

        /// <summary>
        /// Obtiene el servicio de lista de amigos subyacente.
        /// </summary>
        public IListaAmigosServicio ListaAmigosServicio => _listaAmigosServicio;

        /// <summary>
        /// Evento disparado cuando la lista de amigos se actualiza.
        /// </summary>
        public event EventHandler ListaAmigosActualizada;

        /// <summary>
        /// Evento disparado cuando se detecta desconexion del canal.
        /// </summary>
        public event EventHandler<EventArgs> CanalDesconectado;

        /// <summary>
        /// Evento disparado cuando se actualizan las solicitudes de amistad.
        /// </summary>
        public event EventHandler SolicitudesActualizadas;

        /// <summary>
        /// Inicializa las suscripciones a servicios y carga la lista de amigos.
        /// </summary>
        /// <returns>Tarea que representa la operacion asincrona.</returns>
        public async Task InicializarAsync()
        {
            if (!ValidarCondicionesInicializacion())
            {
                return;
            }

            await SuscribirAServiciosAsync().ConfigureAwait(false);
            MarcarSuscripcionActiva();
            CargarListaAmigosInicial();
        }

        /// <summary>
        /// Finaliza las suscripciones y libera recursos.
        /// </summary>
        /// <returns>Tarea que representa la operacion asincrona.</returns>
        public async Task FinalizarAsync()
        {
            DesuscribirEventos();

            if (string.IsNullOrWhiteSpace(_nombreUsuario))
            {
                return;
            }

            await CancelarSuscripcionesAsync().ConfigureAwait(false);
            MarcarSuscripcionInactiva();
        }

        /// <summary>
        /// Actualiza el amigo seleccionado.
        /// </summary>
        /// <param name="amigo">Amigo a seleccionar.</param>
        public void SeleccionarAmigo(DTOs.AmigoDTO amigo)
        {
            AmigoSeleccionado = amigo;
        }

        /// <summary>
        /// Elimina un amigo de la lista.
        /// </summary>
        /// <param name="nombreAmigo">Nombre del amigo a eliminar.</param>
        /// <returns>Tarea que representa la operacion asincrona.</returns>
        public async Task EliminarAmigoAsync(string nombreAmigo)
        {
            if (string.IsNullOrWhiteSpace(nombreAmigo))
            {
                return;
            }

            await _amigosServicio.EliminarAmigoAsync(
                _nombreUsuario,
                nombreAmigo).ConfigureAwait(true);

            await ActualizarListaAmigosDesdeServidorAsync().ConfigureAwait(true);
        }

        /// <summary>
        /// Fuerza la actualizacion de la lista de amigos desde el servidor.
        /// </summary>
        /// <returns>Tarea que representa la operacion asincrona.</returns>
        public async Task ActualizarListaAmigosDesdeServidorAsync()
        {
            if (!ValidarSesionParaActualizarAmigos() || _actualizacionEnProgreso)
            {
                return;
            }

            _actualizacionEnProgreso = true;
            try
            {
                var amigos = await ObtenerAmigosDelServidorAsync().ConfigureAwait(false);
                _ejecutarEnDispatcher(() => ActualizarAmigos(amigos));
            }
            finally
            {
                _actualizacionEnProgreso = false;
            }
        }

        /// <summary>
        /// Aborta las conexiones de los canales de amigos.
        /// </summary>
        public void AbortarConexiones()
        {
            try
            {
                _listaAmigosServicio.AbortarConexion();
            }
            catch (Exception excepcion)
            {
                _logger.Warn("Error al abortar canal de lista de amigos.", excepcion);
            }

            try
            {
                _amigosServicio.AbortarConexion();
            }
            catch (Exception excepcion)
            {
                _logger.Warn("Error al abortar canal de amigos.", excepcion);
            }
        }

        /// <summary>
        /// Marca que la desconexion ha sido procesada.
        /// </summary>
        public void MarcarDesconexionProcesada()
        {
            _desconexionProcesada = true;
        }

        /// <summary>
        /// Indica si la desconexion ya fue procesada.
        /// </summary>
        public bool DesconexionProcesada => _desconexionProcesada;

        /// <summary>
        /// Libera los recursos utilizados por el gestor.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            DesuscribirEventos();
            _disposed = true;
        }

        private void SuscribirEventos()
        {
            _listaAmigosServicio.ListaActualizada += OnListaActualizada;
            _amigosServicio.SolicitudesActualizadas += OnSolicitudesActualizadas;
            _listaAmigosServicio.CanalDesconectado += OnCanalDesconectado;
            _amigosServicio.CanalDesconectado += OnCanalDesconectado;
        }

        private void DesuscribirEventos()
        {
            _listaAmigosServicio.ListaActualizada -= OnListaActualizada;
            _amigosServicio.SolicitudesActualizadas -= OnSolicitudesActualizadas;
            _listaAmigosServicio.CanalDesconectado -= OnCanalDesconectado;
            _amigosServicio.CanalDesconectado -= OnCanalDesconectado;
        }

        private bool ValidarCondicionesInicializacion()
        {
            return !_suscripcionActiva && !string.IsNullOrWhiteSpace(_nombreUsuario);
        }

        private async Task SuscribirAServiciosAsync()
        {
            _desconexionProcesada = false;
            _actualizacionEnProgreso = false;

            await _listaAmigosServicio.SuscribirAsync(_nombreUsuario)
                .ConfigureAwait(false);
            await _amigosServicio.SuscribirAsync(_nombreUsuario)
                .ConfigureAwait(false);

            _canalDisponible = true;
        }

        private void MarcarSuscripcionActiva()
        {
            _suscripcionActiva = true;
        }

        private void CargarListaAmigosInicial()
        {
            _ejecutarEnDispatcher(() =>
                ActualizarAmigos(_listaAmigosServicio.ListaActual));
        }

        private async Task CancelarSuscripcionesAsync()
        {
            await _listaAmigosServicio.CancelarSuscripcionAsync(_nombreUsuario)
                .ConfigureAwait(false);
            await _amigosServicio.CancelarSuscripcionAsync(_nombreUsuario)
                .ConfigureAwait(false);
        }

        private void MarcarSuscripcionInactiva()
        {
            _suscripcionActiva = false;
        }

        private void OnListaActualizada(
            object remitente,
            IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            _ejecutarEnDispatcher(() => ActualizarAmigos(amigos));
        }

        private void OnSolicitudesActualizadas(
            object remitente,
            IReadOnlyCollection<DTOs.SolicitudAmistadDTO> solicitudes)
        {
            if (!_canalDisponible || _desconexionProcesada || _actualizacionEnProgreso)
            {
                return;
            }

            SolicitudesActualizadas?.Invoke(this, EventArgs.Empty);
            _ = ActualizarListaAmigosDesdeServidorAsync();
        }

        private void OnCanalDesconectado(object remitente, EventArgs argumentosEvento)
        {
            if (_desconexionProcesada)
            {
                return;
            }

            _desconexionProcesada = true;
            _logger.Error(
                "Modulo: GestorListaAmigos - Se detecto desconexion del canal " +
                "de amigos. El servidor de WCF no esta disponible o cerro la " +
                "conexion por inactividad.");
            _canalDisponible = false;

            DesuscribirEventosCanales();
            CanalDesconectado?.Invoke(this, EventArgs.Empty);
        }

        private void DesuscribirEventosCanales()
        {
            _listaAmigosServicio.CanalDesconectado -= OnCanalDesconectado;
            _amigosServicio.CanalDesconectado -= OnCanalDesconectado;
        }

        private void ActualizarAmigos(IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            InicializarListaSiEsNecesario();
            LimpiarLista();
            AgregarAmigosValidos(amigos);
            ValidarAmigoSeleccionado(amigos);
            ListaAmigosActualizada?.Invoke(this, EventArgs.Empty);
        }

        private void InicializarListaSiEsNecesario()
        {
        }

        private void LimpiarLista()
        {
            Amigos.Clear();
        }

        private void AgregarAmigosValidos(IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            if (amigos == null)
            {
                return;
            }

            foreach (var amigo in amigos.Where(EsAmigoValido))
            {
                Amigos.Add(amigo);
            }
        }

        private static bool EsAmigoValido(DTOs.AmigoDTO amigo)
        {
            return !string.IsNullOrWhiteSpace(amigo?.NombreUsuario);
        }

        private void ValidarAmigoSeleccionado(IReadOnlyList<DTOs.AmigoDTO> amigos)
        {
            if (AmigoSeleccionado == null)
            {
                return;
            }

            bool amigoExiste = amigos != null && amigos.Any(a =>
                string.Equals(
                    a.NombreUsuario,
                    AmigoSeleccionado.NombreUsuario,
                    StringComparison.OrdinalIgnoreCase));

            if (!amigoExiste)
            {
                AmigoSeleccionado = null;
            }
        }

        private bool ValidarSesionParaActualizarAmigos()
        {
            return !string.IsNullOrWhiteSpace(_nombreUsuario);
        }

        private async Task<IReadOnlyList<DTOs.AmigoDTO>> ObtenerAmigosDelServidorAsync()
        {
            return await _listaAmigosServicio.ObtenerAmigosAsync(_nombreUsuario)
                .ConfigureAwait(false);
        }
    }
}
