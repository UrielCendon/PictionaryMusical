using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalCliente.Utilidades;

namespace PictionaryMusicalCliente.VistaModelo.VentanaPrincipal
{
    /// <summary>
    /// Gestiona la logica de presentacion para la tabla de clasificacion de jugadores.
    /// </summary>
    public class ClasificacionVistaModelo : BaseVistaModelo
    {
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _avisoServicio;

        private readonly IClasificacionServicio _clasificacionServicio;
        private IReadOnlyList<DTOs.ClasificacionUsuarioDTO> _clasificacionOriginal;
        private ObservableCollection<DTOs.ClasificacionUsuarioDTO> _clasificacion;
        private bool _estaCargando;
        private bool _cargaFallida;

        /// <summary>
        /// Inicializa el ViewModel con el servicio de clasificacion.
        /// </summary>
        /// <param name="ventana">Servicio para gestionar ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="clasificacionServicio">Servicio para obtener los datos del ranking.
        /// </param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <param name="sonidoManejador">Servicio de sonido.</param>
        public ClasificacionVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            IClasificacionServicio clasificacionServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
            : base(ventana, localizador)
        {
            _clasificacionServicio = clasificacionServicio ??
                throw new ArgumentNullException(nameof(clasificacionServicio));
            _avisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));

            _clasificacionOriginal = Array.Empty<DTOs.ClasificacionUsuarioDTO>();
            _clasificacion = new ObservableCollection<DTOs.ClasificacionUsuarioDTO>();

            OrdenarPorRondasComando = new ComandoDelegado(EjecutarComandoOrdenarPorRondas, ValidarPuedeOrdenar);
            OrdenarPorPuntosComando = new ComandoDelegado(EjecutarComandoOrdenarPorPuntos, ValidarPuedeOrdenar);
            CerrarComando = new ComandoDelegado(EjecutarComandoCerrar);

            ConfigurarEventoDesconexion();
        }

        private void ConfigurarEventoDesconexion()
        {
            DesconexionDetectada += ManejarDesconexionServidor;
            ConectividadRedMonitor.Instancia.ConexionPerdida += OnConexionInternetPerdida;
        }

        private void DesuscribirEventosDesconexion()
        {
            DesconexionDetectada -= ManejarDesconexionServidor;
            ConectividadRedMonitor.Instancia.ConexionPerdida -= OnConexionInternetPerdida;
        }

        private void ManejarDesconexionServidor(string mensaje)
        {
            EjecutarEnDispatcher(() =>
            {
                DesuscribirEventosDesconexion();
                RequiereReinicioSesion = true;
                _ventana.CerrarVentana(this);
                SolicitarReinicioSesion?.Invoke();
            });
        }

        private void OnConexionInternetPerdida(object remitente, EventArgs argumentos)
        {
            EjecutarEnDispatcher(() =>
            {
                DesuscribirEventosDesconexion();
                RequiereReinicioSesion = true;
                _ventana.CerrarVentana(this);
                SolicitarReinicioSesion?.Invoke();
            });
        }

        private void EjecutarComandoOrdenarPorRondas(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            OrdenarPorRondas();
        }

        private void EjecutarComandoOrdenarPorPuntos(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            OrdenarPorPuntos();
        }

        private bool ValidarPuedeOrdenar(object parametro)
        {
            return PuedeOrdenar();
        }

        private void EjecutarComandoCerrar(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            _ventana.CerrarVentana(this);
        }

        /// <summary>
        /// Coleccion observable de la clasificacion actual para mostrar en la vista.
        /// </summary>
        public ObservableCollection<DTOs.ClasificacionUsuarioDTO> Clasificacion
        {
            get => _clasificacion;
            private set
            {
                if (EstablecerPropiedad(ref _clasificacion, value))
                {
                    NotificarCambio(nameof(HayResultados));
                    NotificarEstadoComandosOrdenamiento();
                }
            }
        }

        /// <summary>
        /// Indica si se estan recuperando datos del servidor.
        /// </summary>
        public bool EstaCargando
        {
            get => _estaCargando;
            private set
            {
                if (EstablecerPropiedad(ref _estaCargando, value))
                {
                    NotificarEstadoComandosOrdenamiento();
                }
            }
        }

        /// <summary>
        /// Indica si existen resultados para mostrar en la tabla.
        /// </summary>
        public bool HayResultados => Clasificacion?.Count > 0;

        /// <summary>
        /// Comando para ordenar la lista por partidas ganadas.
        /// </summary>
        public IComandoNotificable OrdenarPorRondasComando { get; }

        /// <summary>
        /// Comando para ordenar la lista por puntuacion total acumulada.
        /// </summary>
        public IComandoNotificable OrdenarPorPuntosComando { get; }

        /// <summary>
        /// Comando para cerrar la ventana.
        /// </summary>
        public IComandoNotificable CerrarComando { get; }

        /// <summary>
        /// Obtiene o establece la accion para solicitar reinicio de sesion.
        /// </summary>
        public Action SolicitarReinicioSesion { get; set; }

        /// <summary>
        /// Obtiene un valor que indica si se requiere reiniciar sesion.
        /// </summary>
        public bool RequiereReinicioSesion { get; private set; }

        /// <summary>
        /// Obtiene un valor que indica si la carga de datos fallo.
        /// </summary>
        public bool CargaFallida
        {
            get => _cargaFallida;
            private set => EstablecerPropiedad(ref _cargaFallida, value);
        }

        /// <summary>
        /// Recupera la informacion de clasificacion desde el servicio.
        /// <returns>True si la carga fue exitosa; false en caso contrario.</returns>
        public async Task<bool> CargarClasificacionAsync()
        {
            EstaCargando = true;
            bool cargaExitosa = false;

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                IReadOnlyList<DTOs.ClasificacionUsuarioDTO> clasificacion =
                    await ObtenerClasificacionAsync().ConfigureAwait(true);

                _clasificacionOriginal = clasificacion 
                    ?? Array.Empty<DTOs.ClasificacionUsuarioDTO>();
                ActualizarClasificacion(_clasificacionOriginal);
                cargaExitosa = true;
            });

            EstaCargando = false;
            CargaFallida = !cargaExitosa;
            return cargaExitosa;
        }

        private async Task<IReadOnlyList<DTOs.ClasificacionUsuarioDTO>> 
            ObtenerClasificacionAsync()
        {
            return await _clasificacionServicio
                .ObtenerTopJugadoresAsync()
                .ConfigureAwait(true);
        }

        private void ActualizarClasificacion(
            IEnumerable<DTOs.ClasificacionUsuarioDTO> clasificacion)
        {
            var elementosValidos = clasificacion?.Where(c => c != null)
                ?? Enumerable.Empty<DTOs.ClasificacionUsuarioDTO>();

            Clasificacion = new ObservableCollection<DTOs.ClasificacionUsuarioDTO>(
                elementosValidos);
        }

        private void OrdenarPorRondas()
        {
            if (!PuedeOrdenar())
            {
                return;
            }

            IEnumerable<DTOs.ClasificacionUsuarioDTO> ordenados = 
                _clasificacionOriginal
                    .Where(c => c != null)
                    .OrderByDescending(c => c.RondasGanadas)
                    .ThenByDescending(c => c.Puntos)
                    .ThenBy(c => c.Usuario);

            ActualizarClasificacion(ordenados);
        }

        private void OrdenarPorPuntos()
        {
            if (!PuedeOrdenar())
            {
                return;
            }

            IEnumerable<DTOs.ClasificacionUsuarioDTO> ordenados = 
                _clasificacionOriginal
                    .Where(c => c != null)
                    .OrderByDescending(c => c.Puntos)
                    .ThenByDescending(c => c.RondasGanadas)
                    .ThenBy(c => c.Usuario);

            ActualizarClasificacion(ordenados);
        }

        private bool PuedeOrdenar()
        {
            return !EstaCargando && _clasificacionOriginal?.Count > 0;
        }

        private void NotificarEstadoComandosOrdenamiento()
        {
            OrdenarPorRondasComando?.NotificarPuedeEjecutar();
            OrdenarPorPuntosComando?.NotificarPuedeEjecutar();
        }
    }
}