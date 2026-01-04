using log4net;
using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Salas.Auxiliares
{
    /// <summary>
    /// Gestiona la logica de jugadores en una sala de juego.
    /// </summary>
    public sealed class SalaJugadoresManejador
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int MaximoJugadoresSala = 4;
        private static readonly StringComparer ComparadorJugadores =
            StringComparer.OrdinalIgnoreCase;

        private readonly SalaJugadoresManejadorDependencias _dependencias;
        private readonly ContextoSalaJugador _contexto;

        private readonly ObservableCollection<JugadorElemento> _jugadores;
        private Func<bool> _obtenerJuegoIniciado;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="SalaJugadoresManejador"/>.
        /// </summary>
        public SalaJugadoresManejador(
            SalaJugadoresManejadorDependencias dependencias,
            ContextoSalaJugador contexto)
        {
            _dependencias = dependencias ?? 
                throw new ArgumentNullException(nameof(dependencias));
            _contexto = contexto ?? 
                throw new ArgumentNullException(nameof(contexto));

            _jugadores = new ObservableCollection<JugadorElemento>();
        }

        /// <summary>
        /// Delegado para mostrar dialogos de confirmacion.
        /// </summary>
        public Func<string, bool> MostrarConfirmacion { get; set; }

        /// <summary>
        /// Delegado para solicitar datos de un reporte de jugador.
        /// </summary>
        public Func<string, ResultadoReporteJugador> SolicitarDatosReporte 
        { 
            get; set; 
        }

        /// <summary>
        /// Evento que se dispara cuando cambia el progreso de ronda.
        /// </summary>
        public event Action<int> ProgresoRondaCambiado;

        /// <summary>
        /// Obtiene la coleccion de jugadores.
        /// </summary>
        public ObservableCollection<JugadorElemento> Jugadores => _jugadores;

        /// <summary>
        /// Establece el delegado para obtener si el juego esta iniciado.
        /// </summary>
        /// <param name="obtenerJuegoIniciado">
        /// Funcion que devuelve si el juego esta iniciado.
        /// </param>
        public void EstablecerObtenerJuegoIniciado(Func<bool> obtenerJuegoIniciado)
        {
            _obtenerJuegoIniciado = obtenerJuegoIniciado;
        }

        /// <summary>
        /// Actualiza la lista de jugadores preservando los puntajes existentes.
        /// </summary>
        /// <param name="jugadores">Lista de nombres de jugadores.</param>
        public void ActualizarJugadores(IEnumerable<string> jugadores)
        {
            if (jugadores == null)
            {
                _jugadores.Clear();
                return;
            }

            var puntajesExistentes = _jugadores.ToDictionary(
                j => j.Nombre,
                j => j.Puntos,
                ComparadorJugadores);

            _jugadores.Clear();

            var jugadoresUnicos = new HashSet<string>(ComparadorJugadores);

            foreach (string jugador in jugadores)
            {
                if (string.IsNullOrWhiteSpace(jugador))
                {
                    continue;
                }

                if (!jugadoresUnicos.Add(jugador))
                {
                    continue;
                }

                int puntosPreservados = 0;
                puntajesExistentes.TryGetValue(jugador, out puntosPreservados);

                AgregarJugadorConPuntos(jugador, puntosPreservados);

                if (jugadoresUnicos.Count >= MaximoJugadoresSala)
                {
                    break;
                }
            }

            NotificarCambioProgreso();
            ActualizarVisibilidadBotonesExpulsion();
            ActualizarVisibilidadBotonesReporte();
        }

        /// <summary>
        /// Agrega un jugador a la sala.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador.</param>
        public void AgregarJugador(string nombreJugador)
        {
            AgregarJugadorConPuntos(nombreJugador, 0);
        }

        /// <summary>
        /// Agrega un jugador a la sala con puntos iniciales.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador.</param>
        /// <param name="puntos">Puntos iniciales del jugador.</param>
        private void AgregarJugadorConPuntos(string nombreJugador, int puntos)
        {
            var jugadorElemento = new JugadorElemento
            {
                Nombre = nombreJugador,
                MostrarBotonExpulsar = PuedeExpulsarJugador(nombreJugador),
                ExpulsarComando = new ComandoAsincrono(async _ =>
                    await EjecutarExpulsarJugadorAsync(nombreJugador)),
                MostrarBotonReportar = PuedeReportarJugador(nombreJugador),
                ReportarComando = new ComandoAsincrono(async _ =>
                    await EjecutarReportarJugadorAsync(nombreJugador)),
                Puntos = puntos
            };

            _jugadores.Add(jugadorElemento);
            NotificarCambioProgreso();
        }

        /// <summary>
        /// Elimina un jugador de la sala.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador a eliminar.</param>
        /// <returns>True si se elimino, false en caso contrario.</returns>
        public bool EliminarJugador(string nombreJugador)
        {
            var jugador = _jugadores.FirstOrDefault(j => string.Equals(
                j.Nombre,
                nombreJugador,
                StringComparison.OrdinalIgnoreCase));

            if (jugador == null)
            {
                return false;
            }

            _jugadores.Remove(jugador);
            NotificarCambioProgreso();
            ActualizarVisibilidadBotonesExpulsion();
            return true;
        }

        /// <summary>
        /// Reinicia los puntajes de todos los jugadores.
        /// </summary>
        public void ReiniciarPuntajes()
        {
            foreach (var jugador in _jugadores)
            {
                jugador.Puntos = 0;
            }
        }

        /// <summary>
        /// Agrega puntos a un jugador.
        /// </summary>
        /// <param name="nombreJugador">Nombre del jugador.</param>
        /// <param name="puntos">Puntos a agregar.</param>
        public void AgregarPuntos(string nombreJugador, int puntos)
        {
            var jugador = _jugadores.FirstOrDefault(j => string.Equals(
                j.Nombre,
                nombreJugador,
                StringComparison.OrdinalIgnoreCase));

            if (jugador != null && puntos > 0)
            {
                jugador.Puntos += puntos;
            }
        }

        /// <summary>
        /// Actualiza la visibilidad de los botones de expulsion.
        /// </summary>
        public void ActualizarVisibilidadBotonesExpulsion()
        {
            foreach (var jugador in _jugadores)
            {
                jugador.MostrarBotonExpulsar = PuedeExpulsarJugador(jugador?.Nombre);
            }
        }

        /// <summary>
        /// Actualiza la visibilidad de los botones de reporte.
        /// </summary>
        public void ActualizarVisibilidadBotonesReporte()
        {
            foreach (var jugador in _jugadores)
            {
                jugador.MostrarBotonReportar = PuedeReportarJugador(jugador?.Nombre);
            }
        }

        /// <summary>
        /// Limpia la coleccion de jugadores.
        /// </summary>
        public void Limpiar()
        {
            _jugadores.Clear();
        }

        /// <summary>
        /// Obtiene el conteo de jugadores.
        /// </summary>
        public int Conteo => _jugadores?.Count ?? 0;

        private bool PuedeExpulsarJugador(string nombreJugador)
        {
            if (string.IsNullOrWhiteSpace(nombreJugador))
            {
                return false;
            }

            bool juegoIniciado = _obtenerJuegoIniciado?.Invoke() ?? false;
            bool esElMismo = string.Equals(
                nombreJugador,
                _contexto.NombreUsuarioSesion,
                StringComparison.OrdinalIgnoreCase);
            bool esCreador = string.Equals(
                nombreJugador,
                _contexto.CreadorSala,
                StringComparison.OrdinalIgnoreCase);

            return _contexto.EsHost && !juegoIniciado && !esElMismo && !esCreador;
        }

        private bool PuedeReportarJugador(string nombreJugador)
        {
            if (string.IsNullOrWhiteSpace(nombreJugador))
            {
                return false;
            }

            if (_contexto.EsInvitado)
            {
                return false;
            }

            bool esElMismo = string.Equals(
                nombreJugador,
                _contexto.NombreUsuarioSesion,
                StringComparison.OrdinalIgnoreCase);

            if (esElMismo)
            {
                return false;
            }

            if (ValidadorEntrada.EsNombreInvitado(nombreJugador))
            {
                return false;
            }

            return true;
        }

        private void NotificarCambioProgreso()
        {
            ProgresoRondaCambiado?.Invoke(_jugadores?.Count ?? 0);
        }

        private async Task EjecutarExpulsarJugadorAsync(string nombreJugador)
        {
            if (MostrarConfirmacion == null)
            {
                return;
            }

            string mensaje = string.Format(
                Lang.expulsarJugadorTextoConfirmacion,
                nombreJugador);
            bool confirmado = MostrarConfirmacion.Invoke(mensaje);

            if (!confirmado)
            {
                return;
            }

            try
            {
                await _dependencias.SalasServicio.ExpulsarJugadorAsync(
                    _contexto.CodigoSala,
                    _contexto.NombreUsuarioSesion,
                    nombreJugador).ConfigureAwait(true);

                _dependencias.SonidoManejador.ReproducirNotificacion();
                _dependencias.AvisoServicio.Mostrar(Lang.expulsarJugadorTextoExito);
            }
            catch (ServicioExcepcion excepcion)
            {
                ManejarErrorExpulsion(excepcion);
            }
            catch (ArgumentException excepcion)
            {
                ManejarErrorExpulsion(excepcion);
            }
        }

        private void ManejarErrorExpulsion(Exception excepcion)
        {
            _logger.Error("Error al expulsar jugador de la sala.", excepcion);
            _dependencias.SonidoManejador.ReproducirError();
            _dependencias.AvisoServicio.Mostrar(
                excepcion.Message ?? Lang.errorTextoExpulsarJugador);
        }

        private async Task EjecutarReportarJugadorAsync(string nombreJugador)
        {
            if (SolicitarDatosReporte == null || _contexto.EsInvitado)
            {
                return;
            }

            var resultado = SolicitarDatosReporte.Invoke(nombreJugador);
            if (resultado == null || !resultado.Confirmado)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(resultado.Motivo))
            {
                _dependencias.AvisoServicio.Mostrar(Lang.reportarJugadorTextoMotivoRequerido);
                return;
            }

            var reporte = CrearReporteDTO(nombreJugador, resultado.Motivo);

            try
            {
                await EnviarReporteAsync(reporte).ConfigureAwait(true);
            }
            catch (ServicioExcepcion excepcion)
            {
                ManejarErrorReporte(excepcion);
            }
            catch (ArgumentException excepcion)
            {
                ManejarErrorReporte(excepcion);
            }
        }

        private DTOs.ReporteJugadorDTO CrearReporteDTO(
            string nombreJugador, 
            string motivo)
        {
            return new DTOs.ReporteJugadorDTO
            {
                NombreUsuarioReportado = nombreJugador,
                NombreUsuarioReportante = _contexto.NombreUsuarioSesion,
                Motivo = motivo
            };
        }

        private async Task EnviarReporteAsync(
            DTOs.ReporteJugadorDTO reporte)
        {
            DTOs.ResultadoOperacionDTO respuesta = await _dependencias.ReportesServicio
                .ReportarJugadorAsync(reporte)
                .ConfigureAwait(true);

            ProcesarResultadoReporte(respuesta);
        }

        private void ProcesarResultadoReporte(DTOs.ResultadoOperacionDTO respuesta)
        {
            if (respuesta?.OperacionExitosa == true)
            {
                _dependencias.SonidoManejador.ReproducirNotificacion();
                _dependencias.AvisoServicio.Mostrar(Lang.reportarJugadorTextoExito);
            }
            else
            {
                _dependencias.SonidoManejador.ReproducirError();
                string mensajeLocalizado = _dependencias.LocalizadorServicio.Localizar(
                    respuesta?.Mensaje,
                    Lang.errorTextoReportarJugador);
                _dependencias.AvisoServicio.Mostrar(mensajeLocalizado);
            }
        }

        private void ManejarErrorReporte(Exception excepcion)
        {
            _logger.Error("Error al reportar jugador.", excepcion);
            _dependencias.SonidoManejador.ReproducirError();
            string mensajeLocalizado = _dependencias.LocalizadorServicio.Localizar(
                excepcion.Message,
                Lang.errorTextoReportarJugador);
            _dependencias.AvisoServicio.Mostrar(mensajeLocalizado);
        }
    }
}
