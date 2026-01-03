using log4net;
using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.Utilidades.Resultados;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Salas
{
    /// <summary>
    /// Controla el flujo para que un usuario invitado se una a una partida 
    /// mediante codigo.
    /// </summary>
    public class IngresoPartidaInvitadoVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int MaximoJugadoresSala = 4;

        private readonly ISalasServicio _salasServicio;
        private readonly ILocalizacionServicio _localizacionServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _avisoServicio;
        private readonly INombreInvitadoGenerador _nombreInvitadoGenerador;

        private bool _estaProcesando;
        private string _codigoSala;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="IngresoPartidaInvitadoVistaModelo"/>.
        /// </summary>
        /// <param name="ventana">Servicio de ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="localizacionServicio">Servicio de localizacion cultural.</param>
        /// <param name="salasServicio">Servicio de salas.</param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        /// <param name="nombreInvitadoGenerador">
        /// Generador de nombres para invitados.
        /// </param>
        public IngresoPartidaInvitadoVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            IngresoPartidaInvitadoDependencias dependencias)
            : base(ventana, localizador)
        {
            if (dependencias == null)
            {
                throw new ArgumentNullException(nameof(dependencias));
            }

            _localizacionServicio = dependencias.LocalizacionServicio;
            _salasServicio = dependencias.SalasServicio;
            _avisoServicio = dependencias.AvisoServicio;
            _sonidoManejador = dependencias.SonidoManejador;
            _nombreInvitadoGenerador = dependencias.NombreInvitadoGenerador;

            UnirseSalaComando = new ComandoAsincrono(
                EjecutarComandoUnirseSalaAsync,
                ValidarNoEstaProcesando);

            CancelarComando = new ComandoDelegado(
                EjecutarComandoCancelar,
                ValidarNoEstaProcesandoSinParametro);
        }

        private async Task EjecutarComandoUnirseSalaAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await UnirseSalaComoInvitadoAsync().ConfigureAwait(true);
        }

        private bool ValidarNoEstaProcesando(object parametro)
        {
            return !EstaProcesando;
        }

        private void EjecutarComandoCancelar()
        {
            _sonidoManejador.ReproducirClick();
            _ventana.CerrarVentana(this);
        }

        private bool ValidarNoEstaProcesandoSinParametro()
        {
            return !EstaProcesando;
        }

        /// <summary>
        /// Obtiene o establece el codigo de la sala a unirse.
        /// </summary>
        public string CodigoSala
        {
            get => _codigoSala;
            set => EstablecerPropiedad(ref _codigoSala, value);
        }

        /// <summary>
        /// Obtiene un valor que indica si hay una operacion en proceso.
        /// </summary>
        public bool EstaProcesando
        {
            get => _estaProcesando;
            private set
            {
                if (EstablecerPropiedad(ref _estaProcesando, value))
                {
                    ((IComandoNotificable)UnirseSalaComando).NotificarPuedeEjecutar();
                    ((IComandoNotificable)CancelarComando).NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si el invitado se unio exitosamente.
        /// </summary>
        public bool SeUnioSala { get; private set; }

        /// <summary>
        /// Obtiene el comando para unirse a la sala.
        /// </summary>
        public IComandoAsincrono UnirseSalaComando { get; }

        /// <summary>
        /// Obtiene el comando para cancelar la operacion.
        /// </summary>
        public ICommand CancelarComando { get; }

        /// <summary>
        /// Obtiene la sala a la que se unio el invitado.
        /// </summary>
        public DTOs.SalaDTO SalaUnida { get; private set; }

        /// <summary>
        /// Obtiene el nombre generado para el invitado.
        /// </summary>
        public string NombreInvitadoGenerado { get; private set; }

        private async Task UnirseSalaComoInvitadoAsync()
        {
            if (EstaProcesando)
            {
                return;
            }

            var resultadoValidacion = ValidadorEntrada.ValidarCodigoSala(CodigoSala);
            if (!resultadoValidacion.OperacionExitosa)
            {
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(resultadoValidacion.Mensaje);
                return;
            }

            await IntentarUnirseSalaAsync(CodigoSala.Trim()).ConfigureAwait(true);
        }

        private async Task IntentarUnirseSalaAsync(string codigo)
        {
            EstaProcesando = true;

            await EjecutarOperacionAsync(async () =>
            {
                await BuscarNombreUnicoYUnirseAsync(codigo).ConfigureAwait(true);
            },
            excepcion =>
            {
                _logger.Error("Error al intentar unirse a la sala.", excepcion);
                EstaProcesando = false;
            });

            EstaProcesando = false;
        }

        private async Task BuscarNombreUnicoYUnirseAsync(string codigo)
        {
            var nombresReservados = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            var culturaActual = _localizacionServicio?.CulturaActual;
            const int maxIntentos = 20;

            for (int intento = 0; intento < maxIntentos; intento++)
            {
                ResultadoGeneracion resultadoGeneracion = GenerarNombreInvitado(
                    culturaActual, 
                    nombresReservados);

                if (!resultadoGeneracion.Exitoso)
                {
                    RegistrarFalloGeneracion(resultadoGeneracion.Motivo);
                    break;
                }

                string nombreInvitado = resultadoGeneracion.NombreGenerado;

                ResultadoUnionInvitado resultado = await IntentarUnirseAsync(
                    codigo,
                    nombreInvitado).ConfigureAwait(true);

                if (await ProcesarResultadoUnionAsync(
                    resultado, 
                    nombreInvitado, 
                    nombresReservados))
                {
                    return;
                }
            }

            MostrarErrorIntentosAgotados();
        }

        private static void RegistrarFalloGeneracion(MotivoFalloGeneracion motivo)
        {
            _logger.WarnFormat("Generador de nombres fallo con motivo: {0}", motivo);
        }

        private ResultadoGeneracion GenerarNombreInvitado(
            System.Globalization.CultureInfo cultura,
            HashSet<string> nombresReservados)
        {
            return _nombreInvitadoGenerador.Generar(cultura, nombresReservados);
        }

        private Task<bool> ProcesarResultadoUnionAsync(
            ResultadoUnionInvitado resultado,
            string nombreInvitado,
            HashSet<string> nombresReservados)
        {
            switch (resultado.Estado)
            {
                case EstadoUnionInvitado.Exito:
                    MarcarUnionExitosa(resultado.Sala, nombreInvitado);
                    return Task.FromResult(true);

                case EstadoUnionInvitado.NombreDuplicado:
                    AgregarNombresReservados(nombreInvitado, 
                        resultado.JugadoresActuales, 
                        nombresReservados);
                    return Task.FromResult(true);

                case EstadoUnionInvitado.SalaLlena:
                    MostrarErrorSalaLlena();
                    return Task.FromResult(true);

                case EstadoUnionInvitado.SalaNoEncontrada:
                    MostrarErrorSalaNoEncontrada();
                    return Task.FromResult(true);

                case EstadoUnionInvitado.Error:
                    MostrarError(resultado.Mensaje);
                    return Task.FromResult(true);

                default:
                    return Task.FromResult(true);
            }
        }

        private void MarcarUnionExitosa(DTOs.SalaDTO sala, string nombreInvitado)
        {
            _sonidoManejador.ReproducirNotificacion();
            EstablecerResultadoUnion(sala, nombreInvitado);
            _ventana.CerrarVentana(this);
        }

        private void EstablecerResultadoUnion(DTOs.SalaDTO sala, string nombreInvitado)
        {
            SeUnioSala = true;
            SalaUnida = sala;
            NombreInvitadoGenerado = nombreInvitado;
        }

        private static void AgregarNombresReservados(
            string nombreInvitado,
            IReadOnlyCollection<string> jugadoresActuales,
            HashSet<string> nombresReservados)
        {
            nombresReservados.Add(nombreInvitado);
            AgregarJugadoresAReservados(jugadoresActuales, nombresReservados);
        }

        private static void AgregarJugadoresAReservados(
            IReadOnlyCollection<string> jugadoresActuales,
            HashSet<string> nombresReservados)
        {
            if (jugadoresActuales == null)
            {
                return;
            }

            foreach (string jugador in jugadoresActuales)
            {
                nombresReservados.Add(jugador);
            }
        }

        private void MostrarErrorSalaLlena()
        {
            RegistrarErrorSalaLlena();
            NotificarError(Lang.errorTextoSalaLlena);
        }

        private static void RegistrarErrorSalaLlena()
        {
            _logger.Warn("Intento de unirse a sala llena.");
        }

        private void MostrarErrorSalaNoEncontrada()
        {
            RegistrarErrorSalaNoEncontrada();
            NotificarError(Lang.errorTextoNoEncuentraPartida);
        }

        private void RegistrarErrorSalaNoEncontrada()
        {
            _logger.WarnFormat("Sala no encontrada: {0}", CodigoSala);
        }

        private void MostrarError(string mensaje)
        {
            RegistrarError(mensaje);
            string mensajeLocalizado = _localizador.Localizar(
                mensaje,
                Lang.errorTextoNoEncuentraPartida);
            NotificarError(mensajeLocalizado);
        }

        private static void RegistrarError(string mensaje)
        {
            _logger.ErrorFormat("Error al unirse: {0}", mensaje);
        }

        private void NotificarError(string mensaje)
        {
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(mensaje);
        }

        private void MostrarErrorIntentosAgotados()
        {
            RegistrarErrorIntentosAgotados();
            NotificarError(Lang.errorTextoNombresInvitadoAgotados);
        }

        private static void RegistrarErrorIntentosAgotados()
        {
            _logger.Error("Se agotaron los intentos de generar nombre unico.");
        }

        private async Task<ResultadoUnionInvitado> IntentarUnirseAsync(
            string codigo,
            string nombreInvitado)
        {
            try
            {
                DTOs.SalaDTO sala = await _salasServicio.UnirseSalaAsync(
                    codigo,
                    nombreInvitado).ConfigureAwait(true);

                if (sala == null)
                {
                    return ResultadoUnionInvitado.SalaNoEncontrada();
                }

                if (SalaLlena(sala))
                {
                    await IntentarAbandonarSalaAsync(
                        sala.Codigo ?? codigo,
                        nombreInvitado).ConfigureAwait(true);
                    return ResultadoUnionInvitado.CrearErrorSalaLlena();
                }

                if (NombreDuplicado(sala, nombreInvitado))
                {
                    await IntentarAbandonarSalaAsync(
                        sala.Codigo ?? codigo,
                        nombreInvitado).ConfigureAwait(true);
                    return ResultadoUnionInvitado.CrearErrorNombreDuplicado(sala.Jugadores);
                }

                return ResultadoUnionInvitado.Exito(sala);
            }
            catch (ServicioExcepcion excepcion)
            {
                _logger.Error("Excepcion de servicio al intentar unirse como invitado.", excepcion);
                string mensaje = ObtenerMensajeErrorUnionInvitado(excepcion);
                _sonidoManejador.ReproducirError();
                return ResultadoUnionInvitado.Error(mensaje);
            }
        }

        private string ObtenerMensajeErrorUnionInvitado(ServicioExcepcion excepcion)
        {
            if (excepcion.Tipo == TipoErrorServicio.TiempoAgotado ||
                excepcion.Tipo == TipoErrorServicio.Comunicacion)
            {
                return Lang.errorTextoServidorSinDisponibilidad;
            }

            return _localizador.Localizar(
                excepcion.Message,
                Lang.errorTextoNoEncuentraPartida);
        }

        private static bool SalaLlena(DTOs.SalaDTO sala)
        {
            if (sala?.Jugadores == null)
            {
                return false;
            }

            int jugadoresValidos = sala.Jugadores.Count(
                jugador => !string.IsNullOrWhiteSpace(jugador));
            return jugadoresValidos > MaximoJugadoresSala;
        }

        private static bool NombreDuplicado(DTOs.SalaDTO sala, string nombreInvitado)
        {
            if (sala?.Jugadores == null)
            {
                return false;
            }

            int coincidencias = sala.Jugadores
                .Count(jugador => string.Equals(
                    jugador?.Trim(),
                    nombreInvitado,
                    StringComparison.OrdinalIgnoreCase));

            return coincidencias > 1;
        }

        private async Task IntentarAbandonarSalaAsync(
            string codigoSala, 
            string nombreInvitado)
        {
            try
            {
                await _salasServicio.AbandonarSalaAsync(
                    codigoSala,
                    nombreInvitado).ConfigureAwait(true);
            }
            catch (ServicioExcepcion excepcion)
            {
                RegistrarErrorCleanup(excepcion);
            }
            catch (FaultException excepcion)
            {
                RegistrarErrorCleanup(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                RegistrarErrorCleanup(excepcion);
            }
            catch (TimeoutException excepcion)
            {
                RegistrarErrorCleanup(excepcion);
            }
        }

        private static void RegistrarErrorCleanup(Exception excepcion)
        {
            _logger.Warn(
                "Error en cleanup al abandonar sala (ignorado intencionalmente).",
                excepcion);
        }

        private enum EstadoUnionInvitado
        {
            Exito,
            SalaNoEncontrada,
            SalaLlena,
            NombreDuplicado,
            Error
        }

        private sealed class ResultadoUnionInvitado
        {
            private ResultadoUnionInvitado(EstadoUnionInvitado estado)
            {
                Estado = estado;
            }

            public EstadoUnionInvitado Estado { get; }

            public DTOs.SalaDTO Sala { get; private set; }

            public IReadOnlyCollection<string> JugadoresActuales { get; private set; }

            public string Mensaje { get; private set; }

            public static ResultadoUnionInvitado Exito(DTOs.SalaDTO sala)
            {
                return new ResultadoUnionInvitado(EstadoUnionInvitado.Exito)
                {
                    Sala = sala
                };
            }

            public static ResultadoUnionInvitado SalaNoEncontrada()
            {
                return new ResultadoUnionInvitado(EstadoUnionInvitado.SalaNoEncontrada);
            }

            public static ResultadoUnionInvitado CrearErrorSalaLlena()
            {
                return new ResultadoUnionInvitado(EstadoUnionInvitado.SalaLlena);
            }

            public static ResultadoUnionInvitado CrearErrorNombreDuplicado(
                IEnumerable<string> jugadores)
            {
                return new ResultadoUnionInvitado(EstadoUnionInvitado.NombreDuplicado)
                {
                    JugadoresActuales = jugadores?.Where(j => !string.IsNullOrWhiteSpace(j))
                        .Select(jugadores => jugadores.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray()
                };
            }

            public static ResultadoUnionInvitado Error(string mensaje)
            {
                return new ResultadoUnionInvitado(EstadoUnionInvitado.Error)
                {
                    Mensaje = mensaje
                };
            }
        }
    }
}