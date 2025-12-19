using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Perfil
{
    /// <summary>
    /// Gestiona la logica de validacion de codigos de verificacion con 
    /// temporizadores.
    /// </summary>
    public class VerificacionCodigoVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAvisoServicio _avisoServicio;
        private readonly SonidoManejador _sonidoManejador;

        private const int SegundosEsperaReenvio = 30;
        private static readonly TimeSpan TiempoExpiracionCodigo = 
            TimeSpan.FromMinutes(5);

        private readonly ICodigoVerificacionServicio _codigoVerificacionServicio;
        private string _tokenCodigo;
        private readonly DispatcherTimer _temporizadorReenvio;
        private readonly DispatcherTimer _temporizadorExpiracion;

        private string _codigoVerificacion;
        private bool _estaVerificando;
        private bool _puedeReenviar;
        private string _textoBotonReenviar;
        private int _segundosRestantes;

        /// <summary>
        /// Inicializa una nueva instancia de 
        /// <see cref="VerificacionCodigoVistaModelo"/>.
        /// </summary>
        /// <param name="ventana">Servicio de ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="descripcion">Descripcion del proposito de verificacion.</param>
        /// <param name="tokenCodigo">Token asociado al codigo de verificacion.</param>
        /// <param name="codigoVerificacionServicio">
        /// Servicio de verificacion de codigos.
        /// </param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        public VerificacionCodigoVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            string descripcion,
            string tokenCodigo,
            ICodigoVerificacionServicio codigoVerificacionServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
            : base(ventana, localizador)
        {
            Descripcion = descripcion ?? 
                throw new ArgumentNullException(nameof(descripcion));
            _tokenCodigo = tokenCodigo ?? 
                throw new ArgumentNullException(nameof(tokenCodigo));
            _codigoVerificacionServicio = codigoVerificacionServicio ??
                throw new ArgumentNullException(
                    nameof(codigoVerificacionServicio));
            _avisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));

            VerificarCodigoComando = new ComandoAsincrono(EjecutarComandoVerificarCodigoAsync);

            ReenviarCodigoComando = new ComandoAsincrono(
                EjecutarComandoReenviarCodigoAsync, 
                ValidarPuedeReenviar);

            CancelarComando = new ComandoDelegado(EjecutarComandoCancelar);

            _temporizadorReenvio = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _temporizadorReenvio.Tick += TemporizadorReenvioTick;

            _temporizadorExpiracion = new DispatcherTimer
            {
                Interval = TiempoExpiracionCodigo
            };
            _temporizadorExpiracion.Tick += TemporizadorExpiracionTick;

            IniciarTemporizadorReenvio();
            IniciarTemporizadorExpiracion();
        }

        private async Task EjecutarComandoVerificarCodigoAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await VerificarCodigoAsync();
        }

        private async Task EjecutarComandoReenviarCodigoAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await ReenviarCodigoAsync();
        }

        private bool ValidarPuedeReenviar(object parametro)
        {
            return PuedeReenviar;
        }

        private void EjecutarComandoCancelar(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            Cancelar();
        }

        /// <summary>
        /// Obtiene la descripcion del proposito de la verificacion.
        /// </summary>
        public string Descripcion { get; }

        /// <summary>
        /// Obtiene o establece el codigo de verificacion ingresado.
        /// </summary>
        public string CodigoVerificacion
        {
            get => _codigoVerificacion;
            set => EstablecerPropiedad(ref _codigoVerificacion, value);
        }

        /// <summary>
        /// Obtiene un valor que indica si se esta verificando el codigo.
        /// </summary>
        public bool EstaVerificando
        {
            get => _estaVerificando;
            private set
            {
                if (EstablecerPropiedad(ref _estaVerificando, value))
                {
                    ((IComandoNotificable)ReenviarCodigoComando)
                        .NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si se puede reenviar el codigo.
        /// </summary>
        public bool PuedeReenviar
        {
            get => _puedeReenviar;
            private set
            {
                if (EstablecerPropiedad(ref _puedeReenviar, value))
                {
                    ((IComandoNotificable)ReenviarCodigoComando)
                        .NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Obtiene el texto actual del boton de reenvio.
        /// </summary>
        public string TextoBotonReenviar
        {
            get => _textoBotonReenviar;
            private set => EstablecerPropiedad(ref _textoBotonReenviar, value);
        }

        /// <summary>
        /// Obtiene el comando para verificar el codigo ingresado.
        /// </summary>
        public IComandoAsincrono VerificarCodigoComando { get; }

        /// <summary>
        /// Obtiene el comando para reenviar el codigo de verificacion.
        /// </summary>
        public IComandoAsincrono ReenviarCodigoComando { get; }

        /// <summary>
        /// Obtiene el comando para cancelar la verificacion.
        /// </summary>
        public ICommand CancelarComando { get; }

        /// <summary>
        /// Obtiene o establece la accion para marcar el codigo como invalido.
        /// </summary>
        public Action<bool> MarcarCodigoInvalido { get; set; }

        /// <summary>
        /// Obtiene o establece la accion a ejecutar cuando la verificacion 
        /// se completa exitosamente.
        /// </summary>
        public Action<DTOs.ResultadoRegistroCuentaDTO> VerificacionCompletada 
        { 
            get; set; 
        }

        /// <summary>
        /// Obtiene o establece la accion a ejecutar cuando se cancela.
        /// </summary>
        public Action Cancelado { get; set; }

        /// <summary>
        /// Obtiene el resultado de la verificacion.
        /// </summary>
        public DTOs.ResultadoRegistroCuentaDTO ResultadoVerificacion 
        { 
            get; private set; 
        }

        /// <summary>
        /// Obtiene un valor que indica si la verificacion fue exitosa.
        /// </summary>
        public bool VerificacionExitosa { get; private set; }

        private async Task VerificarCodigoAsync()
        {
            MarcarCodigoInvalido?.Invoke(false);

            if (!ValidarCodigoIngresado())
            {
                return;
            }

            EstaVerificando = true;

            await EjecutarOperacionAsync(async () =>
            {
                DTOs.ResultadoRegistroCuentaDTO resultado = 
                    await ConfirmarCodigoEnServidorAsync();

                await ProcesarResultadoVerificacionAsync(resultado);
            },
            excepcion =>
            {
                _logger.WarnFormat(
                    "Error de servicio durante la verificacion del codigo: {0}",
                    excepcion.Message);
                _sonidoManejador.ReproducirError();
                MarcarCodigoInvalido?.Invoke(true);
                string mensaje = !string.IsNullOrWhiteSpace(excepcion.Message)
                    ? excepcion.Message
                    : Lang.errorTextoVerificarCodigo;
                _avisoServicio.Mostrar(mensaje);
                EstaVerificando = false;
            });

            EstaVerificando = false;
        }

        private bool ValidarCodigoIngresado()
        {
            if (!string.IsNullOrWhiteSpace(CodigoVerificacion))
            {
                return true;
            }

            NotificarCodigoRequerido();
            return false;
        }

        private void NotificarCodigoRequerido()
        {
            _sonidoManejador.ReproducirError();
            MarcarCodigoInvalido?.Invoke(true);
            _avisoServicio.Mostrar(Lang.errorTextoCodigoVerificacionRequerido);
        }

        private async Task<DTOs.ResultadoRegistroCuentaDTO> 
            ConfirmarCodigoEnServidorAsync()
        {
            _logger.Info("Enviando codigo de verificacion al servidor.");
            return await _codigoVerificacionServicio
                .ConfirmarCodigoRegistroAsync(
                    _tokenCodigo,
                    CodigoVerificacion).ConfigureAwait(true);
        }

        private async Task ProcesarResultadoVerificacionAsync(
            DTOs.ResultadoRegistroCuentaDTO resultado)
        {
            if (resultado == null)
            {
                ManejarResultadoNulo();
                return;
            }

            if (!resultado.RegistroExitoso)
            {
                await ManejarVerificacionFallidaAsync(resultado);
                return;
            }

            ManejarVerificacionExitosa(resultado);
        }

        private void ManejarResultadoNulo()
        {
            RegistrarErrorResultadoNulo();
            NotificarErrorVerificacion(Lang.errorTextoVerificarCodigo);
        }

        private static void RegistrarErrorResultadoNulo()
        {
            _logger.Error("El servicio de verificacion retorno null.");
        }

        private void NotificarErrorVerificacion(string mensaje)
        {
            _sonidoManejador.ReproducirError();
            MarcarCodigoInvalido?.Invoke(true);
            _avisoServicio.Mostrar(mensaje);
        }

        private async Task ManejarVerificacionFallidaAsync(
            DTOs.ResultadoRegistroCuentaDTO resultado)
        {
            RegistrarVerificacionFallida(resultado.Mensaje);
            _sonidoManejador.ReproducirError();
            
            string mensajeLocalizado = LocalizarMensajeError(resultado.Mensaje);
            resultado.Mensaje = mensajeLocalizado;
            MarcarCodigoInvalido?.Invoke(true);

            if (EsCodigoExpirado(mensajeLocalizado, resultado.Mensaje))
            {
                await ManejarCodigoExpiradoAsync(resultado);
                return;
            }

            _avisoServicio.Mostrar(mensajeLocalizado);
        }

        private static void RegistrarVerificacionFallida(string mensaje)
        {
            _logger.WarnFormat("Verificacion fallida: {0}", mensaje);
        }

        private string LocalizarMensajeError(string mensajeOriginal)
        {
            return !string.IsNullOrWhiteSpace(mensajeOriginal)
                ? mensajeOriginal
                : Lang.errorTextoCodigoIncorrecto;
        }

        private static bool EsCodigoExpirado(
            string mensajeLocalizado,
            string mensajeOriginal)
        {
            bool coincideLocalizado = string.Equals(
                mensajeLocalizado,
                Lang.avisoTextoCodigoExpirado,
                StringComparison.Ordinal);

            bool coincideOriginal = string.Equals(
                mensajeOriginal,
                Lang.avisoTextoCodigoExpirado,
                StringComparison.Ordinal);

            return coincideLocalizado || coincideOriginal;
        }

        private Task ManejarCodigoExpiradoAsync(
            DTOs.ResultadoRegistroCuentaDTO resultado)
        {
            DetenerTemporizadores();
            FinalizarConResultado(resultado, false);
            return Task.CompletedTask;
        }

        private void ManejarVerificacionExitosa(
            DTOs.ResultadoRegistroCuentaDTO resultado)
        {
            RegistrarVerificacionExitosa();
            NotificarExitoVerificacion();
            DetenerTemporizadores();
            FinalizarConResultado(resultado, true);
        }

        private static void RegistrarVerificacionExitosa()
        {
            _logger.Info("Codigo verificado correctamente.");
        }

        private void NotificarExitoVerificacion()
        {
            _sonidoManejador.ReproducirNotificacion();
            MarcarCodigoInvalido?.Invoke(false);
        }

        private void FinalizarConResultado(
            DTOs.ResultadoRegistroCuentaDTO resultado,
            bool exitoso)
        {
            ResultadoVerificacion = resultado;
            VerificacionExitosa = exitoso;

            if (exitoso)
            {
                VerificacionCompletada?.Invoke(resultado);
            }

            _ventana.CerrarVentana(this);
        }

        private async Task ReenviarCodigoAsync()
        {
            if (!PuedeReenviar)
            {
                return;
            }

            await EjecutarOperacionAsync(async () =>
            {
                _logger.Info("Solicitando reenvio de codigo de verificacion.");
                DTOs.ResultadoSolicitudCodigoDTO resultado = 
                    await _codigoVerificacionServicio
                        .ReenviarCodigoRegistroAsync(_tokenCodigo)
                        .ConfigureAwait(true);

                ProcesarResultadoReenvio(resultado);
            },
            excepcion =>
            {
                _logger.WarnFormat(
                    "Excepcion de servicio al reenviar codigo: {0}",
                    excepcion.Message);
                _sonidoManejador.ReproducirError();
                string mensaje = !string.IsNullOrWhiteSpace(excepcion.Message)
                    ? excepcion.Message
                    : Lang.errorTextoSolicitarNuevoCodigo;
                _avisoServicio.Mostrar(mensaje);
            });
        }

        private void ProcesarResultadoReenvio(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            if (resultado?.CodigoEnviado == true)
            {
                ManejarReenvioExitoso(resultado);
            }
            else
            {
                ManejarReenvioFallido(resultado);
            }
        }

        private void ManejarReenvioExitoso(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            RegistrarReenvioExitoso();
            _sonidoManejador.ReproducirNotificacion();
            ActualizarTokenSiEsNecesario(resultado.TokenCodigo);
            ReiniciarTemporizadores();
        }

        private static void RegistrarReenvioExitoso()
        {
            _logger.Info("Codigo reenviado exitosamente.");
        }

        private void ActualizarTokenSiEsNecesario(string nuevoToken)
        {
            if (!string.IsNullOrWhiteSpace(nuevoToken))
            {
                _tokenCodigo = nuevoToken;
            }
        }

        private void ReiniciarTemporizadores()
        {
            IniciarTemporizadorReenvio();
            IniciarTemporizadorExpiracion();
        }

        private void ManejarReenvioFallido(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            RegistrarReenvioFallido(resultado?.Mensaje);
            NotificarErrorReenvio(resultado?.Mensaje);
        }

        private static void RegistrarReenvioFallido(string mensaje)
        {
            _logger.WarnFormat("Fallo al reenviar codigo: {0}", mensaje);
        }

        private void NotificarErrorReenvio(string mensaje)
        {
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(mensaje ?? Lang.errorTextoSolicitarNuevoCodigo);
        }

        private void Cancelar()
        {
            DetenerTemporizadores();
            Cancelado?.Invoke(); 
            _ventana.CerrarVentana(this);
        }

        private void IniciarTemporizadorReenvio()
        {
            PuedeReenviar = false;
            _segundosRestantes = SegundosEsperaReenvio;
            ActualizarTextoReenvio();
            _temporizadorReenvio.Start();
        }

        private void IniciarTemporizadorExpiracion()
        {
            _temporizadorExpiracion.Stop();
            _temporizadorExpiracion.Start();
        }

        private void TemporizadorReenvioTick(
            object remitente,
            EventArgs argumentosEvento)
        {
            if (_segundosRestantes <= 0)
            {
                FinalizarTemporizadorReenvio();
                return;
            }

            _segundosRestantes--;
            ActualizarTextoReenvio();
        }

        private void FinalizarTemporizadorReenvio()
        {
            _temporizadorReenvio.Stop();
            PuedeReenviar = true;
            TextoBotonReenviar = Lang.cambiarContrasenaTextoReenviarCodigo;
        }

        private void TemporizadorExpiracionTick(
            object remitente,
            EventArgs argumentosEvento)
        {
            RegistrarExpiracionCodigo();
            _temporizadorExpiracion.Stop();
            _avisoServicio.Mostrar(Lang.avisoTextoCodigoExpirado);
            DetenerTemporizadores();
            _ventana.CerrarVentana(this);
        }

        private static void RegistrarExpiracionCodigo()
        {
            _logger.Info("El tiempo de validez del codigo ha expirado.");
        }

        private void ActualizarTextoReenvio()
        {
            TextoBotonReenviar = string.Format(
                "{0} ({1})",
                Lang.cambiarContrasenaTextoReenviarCodigo,
                _segundosRestantes);
        }

        private void DetenerTemporizadores()
        {
            _temporizadorReenvio.Stop();
            _temporizadorExpiracion.Stop();
        }
    }
}