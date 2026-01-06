using log4net;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.Entidades;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.LogicaNegocio.Interfaces;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace PictionaryMusicalServidor.Servicios.LogicaNegocio
{
    /// <summary>
    /// Contiene la configuración inicial para una partida.
    /// </summary>
    public class ConfiguracionPartida
    {
        public int TiempoRonda { get; set; }
        public string Dificultad { get; set; }
        public int TotalRondas { get; set; }
    }

    /// <summary>
    /// Agrupa las dependencias requeridas por el controlador de partida.
    /// </summary>
    public class DependenciasPartida
    {
        public ICatalogoCanciones Catalogo { get; set; }
        public IGestorJugadoresPartida GestorJugadores { get; set; }
        public IGestorTiemposPartida GestorTiempos { get; set; }
        public IValidadorAdivinanza ValidadorAdivinanza { get; set; }
        public IProveedorTiempo ProveedorTiempo { get; set; }
    }
    /// <summary>
    /// Controla el flujo de una partida, incluyendo rondas, turnos y eventos del juego.
    /// </summary>
    public class ControladorPartida
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(ControladorPartida));

        private const string RolDibujante = "Dibujante";
        private const int LimitePalabrasMensaje = 150;
        private const int TiempoOverlayClienteSegundos = 5;
        private const int MilisegundosPorSegundo = 1000;

        private readonly object _sincronizacion = new object();

        private readonly ICatalogoCanciones _catalogoCanciones;
        private readonly IGestorJugadoresPartida _gestorJugadores;
        private readonly IGestorTiemposPartida _gestorTiempos;
        private readonly IValidadorAdivinanza _validadorAdivinanza;
        private readonly IProveedorTiempo _proveedorTiempo;
        private readonly HashSet<int> _cancionesUsadas;

        private readonly int _duracionRondaSegundos;
        private readonly string _dificultad;
        private readonly int _totalRondas;
        private string _idiomaCanciones = "Espanol";

        private EstadoPartida _estadoActual;
        private int _rondaActual;
        private int _cancionActualId;
        private bool _rondaFinalizadaPorAciertos;
        private bool _enTransicionRonda;

        /// <summary>
        /// Evento disparado cuando la partida inicia.
        /// </summary>
        public event Action PartidaIniciada;

        /// <summary>
        /// Evento disparado al comenzar una nueva ronda con los datos correspondientes.
        /// </summary>
        public event Action<RondaDTO> InicioRonda;

        /// <summary>
        /// Evento disparado cuando un jugador adivina correctamente.
        /// Parametros: nombre del jugador, puntos obtenidos.
        /// </summary>
        public event Action<string, int> JugadorAdivino;

        /// <summary>
        /// Evento disparado cuando se recibe un mensaje de chat.
        /// Parametros: nombre del jugador, mensaje.
        /// </summary>
        public event Action<string, string> MensajeChatRecibido;

        /// <summary>
        /// Evento disparado cuando se recibe un trazo del dibujante.
        /// </summary>
        public event Action<TrazoDTO> TrazoRecibido;

        /// <summary>
        /// Evento disparado al finalizar una ronda.
        /// Parametro: indica si termino por tiempo agotado.
        /// </summary>
        public event Action<bool> FinRonda;

        /// <summary>
        /// Evento disparado cuando la partida finaliza.
        /// </summary>
        public event Action<ResultadoPartidaDTO> FinPartida;

        /// <summary>
        /// Evento disparado para limpiar el lienzo de dibujo.
        /// </summary>
        public event Action LimpiarLienzo;

        /// <summary>
        /// Evento disparado cuando un jugador se desconecta.
        /// Parametro: nombre del usuario desconectado.
        /// </summary>
        public event Action<string> JugadorDesconectado;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de partida.
        /// </summary>
        /// <param name="configuracion">Configuración de la partida.</param>
        /// <param name="dependencias">Dependencias requeridas por el controlador.</param>
        /// <exception cref="ArgumentNullException">Si configuracion o dependencias son nulas.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Si tiempoRonda o totalRondas son menores
        /// o iguales a cero.</exception>
        /// <exception cref="ArgumentException">Si dificultad es nula o vacia.</exception>
        public ControladorPartida(
            ConfiguracionPartida configuracion,
            DependenciasPartida dependencias)
        {
            if (configuracion == null)
            {
                throw new ArgumentNullException(nameof(configuracion));
            }

            if (dependencias == null)
            {
                throw new ArgumentNullException(nameof(dependencias));
            }

            if (configuracion.TiempoRonda <= 0 || configuracion.TotalRondas <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(configuracion),
                    "El tiempo de ronda y el número total de rondas deben ser mayores que cero.");
            }

            if (string.IsNullOrWhiteSpace(configuracion.Dificultad))
            {
                throw new ArgumentException(nameof(configuracion.Dificultad));
            }

            _catalogoCanciones = dependencias.Catalogo 
                ?? throw new ArgumentNullException(
                    nameof(dependencias), 
                    "Catalogo no puede ser nulo.");
            _gestorJugadores = dependencias.GestorJugadores 
                ?? throw new ArgumentNullException(
                    nameof(dependencias), 
                    "GestorJugadores no puede ser nulo.");
            _gestorTiempos = dependencias.GestorTiempos 
                ?? throw new ArgumentNullException(
                    nameof(dependencias), 
                    "GestorTiempos no puede ser nulo.");
            _validadorAdivinanza = dependencias.ValidadorAdivinanza 
                ?? throw new ArgumentNullException(
                    nameof(dependencias), 
                    "ValidadorAdivinanza no puede ser nulo.");
            _proveedorTiempo = dependencias.ProveedorTiempo 
                ?? throw new ArgumentNullException(
                    nameof(dependencias), 
                    "ProveedorTiempo no puede ser nulo.");

            _duracionRondaSegundos = configuracion.TiempoRonda;
            _dificultad = configuracion.Dificultad.Trim();
            _totalRondas = configuracion.TotalRondas;

            _cancionesUsadas = new HashSet<int>();
            _estadoActual = EstadoPartida.EnSalaEspera;

            SuscribirEventosTiempo();
        }

        /// <summary>
        /// Indica si la partida ha finalizado.
        /// </summary>
        public bool EstaFinalizada
        {
            get
            {
                lock (_sincronizacion)
                {
                    return _estadoActual == EstadoPartida.Finalizada;
                }
            }
        }

        /// <summary>
        /// Configura el idioma de las canciones para la partida.
        /// </summary>
        /// <param name="idioma">Codigo del idioma (Espanol, Ingles, Mixto).</param>
        public void ConfigurarIdiomaCanciones(string idioma)
        {
            lock (_sincronizacion)
            {
                _idiomaCanciones = idioma;
            }
        }

        /// <summary>
        /// Agrega un jugador a la partida.
        /// </summary>
        /// <param name="id">Identificador de conexion del jugador.</param>
        /// <param name="nombre">Nombre de usuario del jugador.</param>
        /// <param name="esHost">Indica si el jugador es el anfitrion.</param>
        /// <exception cref="InvalidOperationException">Si la partida ya inicio.</exception>
        public void AgregarJugador(string id, string nombre, bool esHost)
        {
            lock (_sincronizacion)
            {
                if (_estadoActual != EstadoPartida.EnSalaEspera)
                {
                    throw new InvalidOperationException(MensajesError.Cliente.PartidaYaIniciada);
                }

                _gestorJugadores.Agregar(id, nombre, esHost);
            }
        }

        /// <summary>
        /// Remueve un jugador de la partida y maneja las consecuencias.
        /// </summary>
        /// <param name="id">Identificador de conexion del jugador a remover.</param>
        public void RemoverJugador(string id)
        {
            string nombreUsuarioRemovido;
            var resultado = ProcesarRemocionJugador(id, out nombreUsuarioRemovido);
            EjecutarAccionPostRemocion(resultado);

            if (!string.IsNullOrWhiteSpace(nombreUsuarioRemovido))
            {
                JugadorDesconectado?.Invoke(nombreUsuarioRemovido);
            }
        }

        /// <summary>
        /// Inicia la partida si se cumplen las condiciones.
        /// </summary>
        /// <param name="idSolicitante">Identificador del jugador que solicita iniciar.</param>
        /// <exception cref="InvalidOperationException">Si la partida ya inicio o faltan 
        /// jugadores.</exception>
        /// <exception cref="SecurityException">Si el solicitante no es el anfitrion.</exception>
        public void IniciarPartida(string idSolicitante)
        {
            lock (_sincronizacion)
            {
                ValidarInicioPartida(idSolicitante);
                _estadoActual = EstadoPartida.Jugando;
            }

            PartidaIniciada?.Invoke();
            PrepararSiguienteRonda();
        }

        /// <summary>
        /// Procesa un mensaje enviado por un jugador.
        /// </summary>
        /// <param name="id">Identificador de conexion del jugador.</param>
        /// <param name="mensaje">Contenido del mensaje.</param>
        public void ProcesarMensaje(string id, string mensaje)
        {
            if (EsMensajeInvalido(id, mensaje))
            {
                return;
            }

            JugadorPartida jugador;
            lock (_sincronizacion)
            {
                jugador = _gestorJugadores.Obtener(id);
                if (jugador == null)
                {
                    return;
                }
            }

            if (_estadoActual == EstadoPartida.EnSalaEspera)
            {
                MensajeChatRecibido?.Invoke(jugador.NombreUsuario, mensaje);
                return;
            }

            ProcesarIntentoAdivinanza(jugador, mensaje);
        }

        /// <summary>
        /// Procesa un trazo enviado por el dibujante.
        /// </summary>
        /// <param name="id">Identificador de conexion del jugador.</param>
        /// <param name="trazo">Datos del trazo a dibujar.</param>
        public void ProcesarTrazo(string id, TrazoDTO trazo)
        {
            lock (_sincronizacion)
            {
                var jugador = _gestorJugadores.Obtener(id);
                
                if (_estadoActual == EstadoPartida.Jugando && 
                    !_enTransicionRonda && 
                    jugador != null && 
                    jugador.EsDibujante)
                {
                    TrazoRecibido?.Invoke(trazo);
                }
            }
        }

        /// <summary>
        /// Obtiene una copia de la lista de jugadores actuales.
        /// </summary>
        /// <returns>Coleccion de jugadores en la partida.</returns>
        public IEnumerable<JugadorPartida> ObtenerJugadores()
        {
            lock (_sincronizacion)
            {
                return _gestorJugadores.ObtenerCopiaLista();
            }
        }

        /// <summary>
        /// Obtiene el nombre de usuario de un jugador por su identificador de conexion.
        /// </summary>
        /// <param name="idConexion">Identificador de conexion del jugador.</param>
        /// <returns>Nombre de usuario o null si no existe.</returns>
        public string ObtenerNombreUsuarioPorId(string idConexion)
        {
            lock (_sincronizacion)
            {
                var jugador = _gestorJugadores.Obtener(idConexion);
                return jugador?.NombreUsuario;
            }
        }

        private ResultadoRemocionJugador ProcesarRemocionJugador(
            string id,
            out string nombreUsuarioRemovido)
        {
            nombreUsuarioRemovido = null;
            lock (_sincronizacion)
            {
                bool eraAnfitrion = _gestorJugadores.EsHost(id);
                bool eraDibujante;
                string nombreUsuario;

                if (!_gestorJugadores.Remover(id, out eraDibujante, out nombreUsuario))
                {
                    return ResultadoRemocionJugador.SinAccion();
                }

                nombreUsuarioRemovido = nombreUsuario;
                _logger.Info("Jugador removido de la partida.");
                
                return DeterminarAccionPostRemocion(new DatosRemocionJugador
                {
                    EraAnfitrion = eraAnfitrion,
                    EraDibujante = eraDibujante
                });
            }
        }

        private ResultadoRemocionJugador DeterminarAccionPostRemocion(
            DatosRemocionJugador datos)
        {
            if (_estadoActual != EstadoPartida.Jugando)
            {
                return ResultadoRemocionJugador.SinAccion();
            }

            if (datos.EraAnfitrion)
            {
                return ResultadoRemocionJugador.Cancelar(
                    MensajesError.Cliente.PartidaCanceladaHostSalio);
            }

            if (!_gestorJugadores.HaySuficientesJugadores)
            {
                return ResultadoRemocionJugador.Cancelar(
                    MensajesError.Cliente.PartidaCanceladaFaltaJugadores);
            }

            if (datos.EraDibujante)
            {
                return _enTransicionRonda
                    ? ResultadoRemocionJugador.AvanzarRondaDirecto()
                    : ResultadoRemocionJugador.AvanzarRonda();
            }

            if (_gestorJugadores.TodosAdivinaron())
            {
                return ResultadoRemocionJugador.AvanzarRonda();
            }

            return ResultadoRemocionJugador.SinAccion();
        }

        private void EjecutarAccionPostRemocion(ResultadoRemocionJugador resultado)
        {
            if (resultado.DebeCancelar)
            {
                CancelarPartida(resultado.MensajeCancelacion);
            }
            else if (resultado.DebeAvanzarDirecto)
            {
                PrepararSiguienteRonda();
            }
            else if (resultado.DebeAvanzar)
            {
                FinalizarRondaActual(new ParametrosFinalizacionRonda
                {
                    ForzarPorAciertos = false,
                    TiempoAgotado = false
                });
            }
        }

        private void ValidarInicioPartida(string id)
        {
            if (_estadoActual != EstadoPartida.EnSalaEspera)
            {
                throw new InvalidOperationException(MensajesError.Cliente.PartidaYaIniciada);
            }

            if (!_gestorJugadores.HaySuficientesJugadores)
            {
                throw new InvalidOperationException(MensajesError.Cliente.FaltanJugadores);
            }

            if (!_gestorJugadores.EsHost(id))
            {
                throw new SecurityException(MensajesError.Cliente.SoloHost);
            }
        }

        private void ProcesarIntentoAdivinanza(JugadorPartida jugador, string mensaje)
        {
            bool acierto = false;
            int puntos = 0;
            bool finRonda = false;

            lock (_sincronizacion)
            {
                if (_validadorAdivinanza.JugadorPuedeAdivinar(jugador, _estadoActual))
                {
                    acierto = _validadorAdivinanza.VerificarAcierto(_cancionActualId, mensaje, out puntos);
                    if (acierto)
                    {
                        _validadorAdivinanza.RegistrarAcierto(jugador, puntos);
                        finRonda = _gestorJugadores.TodosAdivinaron();
                    }
                }
            }

            if (acierto)
            {
                JugadorAdivino?.Invoke(jugador.NombreUsuario, puntos);
                if (finRonda)
                {
                    FinalizarRondaAnticipada();
                }
            }
            else
            {
                MensajeChatRecibido?.Invoke(jugador.NombreUsuario, mensaje);
            }
        }

        private void PrepararSiguienteRonda()
        {
            RondaDTO rondaDto = null;
            bool finJuego = false;

            lock (_sincronizacion)
            {
                if (_estadoActual != EstadoPartida.Jugando)
                {
                    return;
                }

                _enTransicionRonda = true;

                if (!_gestorJugadores.QuedanDibujantesPendientes())
                {
                    if (_rondaActual >= _totalRondas)
                    {
                        finJuego = true;
                    }
                    else
                    {
                        _rondaActual++;
                        _gestorJugadores.PrepararColaDibujantes();
                    }
                }

                if (!finJuego)
                {
                    _gestorJugadores.SeleccionarSiguienteDibujante();
                    var cancion = _catalogoCanciones.ObtenerCancionAleatoria(
                        _idiomaCanciones, 
                        _cancionesUsadas);
                    _cancionActualId = cancion.Id;
                    _cancionesUsadas.Add(cancion.Id);
                    rondaDto = CrearRondaDto(cancion);
                    IniciarTemporizadorRondaConRetardo();
                }
                else
                {
                    _estadoActual = EstadoPartida.Finalizada;
                }
            }

            if (finJuego)
            {
                NotificarFinPartida();
            }
            else
            {
                LimpiarLienzo?.Invoke();
                InicioRonda?.Invoke(rondaDto);
            }
        }

        private void IniciarTemporizadorRondaConRetardo()
        {
            int milisegundosRetraso = TiempoOverlayClienteSegundos * MilisegundosPorSegundo;
            _proveedorTiempo.Retrasar(milisegundosRetraso)
                .ContinueWith(ManejarRetardoTemporizador);
        }

        private void ManejarRetardoTemporizador(Task tarea)
        {
            bool debeLimpiar = false;
            lock (_sincronizacion)
            {
                if (_estadoActual == EstadoPartida.Jugando)
                {
                    _enTransicionRonda = false;
                    _gestorTiempos.IniciarRonda();
                    debeLimpiar = true;
                }
            }

            if (debeLimpiar)
            {
                LimpiarLienzo?.Invoke();
            }
        }

        private void FinalizarRondaActual(ParametrosFinalizacionRonda parametros)
        {
            lock (_sincronizacion)
            {
                if (_estadoActual != EstadoPartida.Jugando)
                {
                    return;
                }

                if (_rondaFinalizadaPorAciertos && !parametros.ForzarPorAciertos)
                {
                    return;
                }

                _gestorTiempos.DetenerTodo();
                _enTransicionRonda = true;
            }

            FinRonda?.Invoke(parametros.TiempoAgotado);
            EvaluarContinuidadPartida();
        }

        private void FinalizarRondaAnticipada()
        {
            bool yaFinalizada;
            lock (_sincronizacion)
            {
                yaFinalizada = _rondaFinalizadaPorAciertos;
                _rondaFinalizadaPorAciertos = true;
            }

            if (!yaFinalizada)
            {
                FinalizarRondaActual(new ParametrosFinalizacionRonda
                {
                    ForzarPorAciertos = true,
                    TiempoAgotado = false
                });
            }
        }

        private void EvaluarContinuidadPartida()
        {
            bool esFin = false;
            lock (_sincronizacion)
            {
                bool sinDibujantes = !_gestorJugadores.QuedanDibujantesPendientes();
                bool rondasCompletadas = _rondaActual >= _totalRondas && sinDibujantes;

                if (_estadoActual == EstadoPartida.Finalizada || rondasCompletadas)
                {
                    _estadoActual = EstadoPartida.Finalizada;
                    esFin = true;
                }
                else
                {
                    _rondaFinalizadaPorAciertos = false;
                    _gestorTiempos.IniciarTransicion();
                }
            }

            if (esFin)
            {
                NotificarFinPartida();
            }
        }

        private void CancelarPartida(string mensajeCancelacion)
        {
            bool debeNotificar = false;
            lock (_sincronizacion)
            {
                if (_estadoActual == EstadoPartida.Finalizada)
                {
                    return;
                }

                _logger.WarnFormat("Partida cancelada. Motivo: '{0}'.", mensajeCancelacion);
                _estadoActual = EstadoPartida.Finalizada;
                _gestorTiempos.DetenerTodo();
                debeNotificar = true;
            }

            if (debeNotificar)
            {
                string mensajeFinal = mensajeCancelacion 
                    ?? MensajesError.Cliente.PartidaCanceladaFaltaJugadores;
                FinPartida?.Invoke(new ResultadoPartidaDTO
                {
                    Clasificacion = _gestorJugadores.GenerarClasificacion(),
                    Mensaje = mensajeFinal
                });
            }
        }

        private void NotificarFinPartida()
        {
            FinPartida?.Invoke(new ResultadoPartidaDTO { Clasificacion = 
                _gestorJugadores.GenerarClasificacion() });
        }

        private void SuscribirEventosTiempo()
        {
            _gestorTiempos.TiempoRondaAgotado += ManejarTiempoRondaAgotado;
            _gestorTiempos.TiempoTransicionAgotado += PrepararSiguienteRonda;
        }

        private void ManejarTiempoRondaAgotado()
        {
            FinalizarRondaActual(new ParametrosFinalizacionRonda
            {
                ForzarPorAciertos = false,
                TiempoAgotado = true
            });
        }

        private static bool EsMensajeInvalido(string id, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(mensaje))
            {
                return true;
            }

            return mensaje.Split(' ').Length > LimitePalabrasMensaje;
        }

        private RondaDTO CrearRondaDto(Cancion cancion)
        {
            string genero = null;
            string artista = null;

            if (_dificultad.Equals("facil", StringComparison.OrdinalIgnoreCase))
            {
                artista = cancion.Artista;
            }

            if (!_dificultad.Equals("dificil", StringComparison.OrdinalIgnoreCase))
            {
                genero = cancion.Genero;
            }

            return new RondaDTO 
            { 
                IdCancion = cancion.Id, 
                Rol = RolDibujante, 
                PistaArtista = artista, 
                PistaGenero = genero, 
                TiempoSegundos = _duracionRondaSegundos 
            };
        }

        private sealed class ParametrosFinalizacionRonda
        {
            public bool ForzarPorAciertos { get; set; }
            public bool TiempoAgotado { get; set; }
        }

        private sealed class ResultadoRemocionJugador
        {
            public bool DebeCancelar { get; private set; }
            public bool DebeAvanzar { get; private set; }
            public bool DebeAvanzarDirecto { get; private set; }
            public string MensajeCancelacion { get; private set; }
            public static ResultadoRemocionJugador SinAccion() => new ResultadoRemocionJugador();
            public static ResultadoRemocionJugador Cancelar(string mensaje) => 
                new ResultadoRemocionJugador { DebeCancelar = true, MensajeCancelacion = mensaje };
            public static ResultadoRemocionJugador AvanzarRonda() => 
                new ResultadoRemocionJugador { DebeAvanzar = true };
            public static ResultadoRemocionJugador AvanzarRondaDirecto() => 
                new ResultadoRemocionJugador { DebeAvanzarDirecto = true };
        }

        private sealed class DatosRemocionJugador
        {
            public bool EraAnfitrion { get; set; }
            public bool EraDibujante { get; set; }
        }
    }
}