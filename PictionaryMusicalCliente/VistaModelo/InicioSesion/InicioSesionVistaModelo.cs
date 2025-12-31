using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.VistaModelo.Auxiliares;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using PictionaryMusicalCliente.VistaModelo.Salas;
using PictionaryMusicalCliente.VistaModelo.InicioSesion.Auxiliares;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using PictionaryMusicalCliente.ClienteServicios.Dialogos;
using PictionaryMusicalCliente.ClienteServicios;

namespace PictionaryMusicalCliente.VistaModelo.InicioSesion
{
    /// <summary>
    /// Gestiona la logica de autenticacion y navegacion desde la pantalla de inicio.
    /// </summary>
    /// <remarks>
    /// Permite iniciar sesion con credenciales, como invitado,
    /// crear cuenta y recuperar contrasena.
    /// </remarks>
    public class InicioSesionVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IInicioSesionServicio _inicioSesionServicio;
        private readonly ICambioContrasenaServicio _cambioContrasenaServicio;
        private readonly IRecuperacionCuentaServicio _recuperacionCuentaDialogoServicio;
        private readonly ILocalizacionServicio _localizacionServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _avisoServicio;
        private readonly INombreInvitadoGenerador _generadorNombres;
        private readonly IUsuarioAutenticado _usuarioSesion;
        private readonly Func<ISalasServicio> _salasServicioFactory;
        private readonly InicioSesionNavegacion _navegacion;
        private readonly ValidadorCuenta _validadorCuenta;

        /// <summary>
        /// Nombre del campo contrasena para validacion.
        /// </summary>
        public const string CampoContrasena = "Contrasena";

        private string _identificador;
        private string _contrasena;
        private bool _estaProcesando;
        private ObservableCollection<IdiomaOpcion> _idiomasDisponibles;
        private IdiomaOpcion _idiomaSeleccionado;
        private bool _sesionIniciada;

        /// <summary>
        /// Inicializa una nueva instancia de la clase.
        /// </summary>
        /// <param name="dependenciasBase">
        /// Dependencias comunes de UI del ViewModel.
        /// </param>
        /// <param name="dependencias">
        /// Dependencias especificas de inicio de sesion.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Si algun parametro requerido es nulo.
        /// </exception>
        public InicioSesionVistaModelo(
            VistaModeloBaseDependencias dependenciasBase,
            InicioSesionDependencias dependencias)
            : base(
                dependenciasBase?.Ventana, 
                dependenciasBase?.Localizador)
        {
            ValidarDependenciasBase(dependenciasBase);
            ValidarDependenciasInicioSesion(dependencias);

            _sonidoManejador = dependenciasBase.SonidoManejador;
            _avisoServicio = dependenciasBase.AvisoServicio;

            _inicioSesionServicio = dependencias.InicioSesionServicio;
            _cambioContrasenaServicio = dependencias.CambioContrasenaServicio;
            _recuperacionCuentaDialogoServicio = dependencias.RecuperacionCuentaServicio;
            _localizacionServicio = dependencias.LocalizacionServicio;
            _generadorNombres = dependencias.GeneradorNombres;
            _usuarioSesion = dependencias.UsuarioSesion;
            _salasServicioFactory = dependencias.SalasServicioFactory;
            _validadorCuenta = new ValidadorCuenta();
            _navegacion = new InicioSesionNavegacion(
                _ventana,
                _localizador,
                _localizacionServicio,
                _sonidoManejador,
                _avisoServicio,
                _usuarioSesion);

            IniciarSesionComando = new ComandoAsincrono(
                EjecutarComandoIniciarSesionAsync, 
                ValidarPuedeProcesarSesion);

            RecuperarCuentaComando = new ComandoAsincrono(
                EjecutarComandoRecuperarCuentaAsync, 
                ValidarPuedeProcesarSesion);

            AbrirCrearCuentaComando = new ComandoDelegado(EjecutarComandoAbrirCrearCuenta);

            IniciarSesionInvitadoComando = new ComandoAsincrono(
                EjecutarComandoIniciarSesionInvitadoAsync, 
                ValidarPuedeProcesarSesion);

            CargarIdiomas();
        }

        private async Task EjecutarComandoIniciarSesionAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await IniciarSesionAsync();
        }

        private async Task EjecutarComandoRecuperarCuentaAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await RecuperarCuentaAsync();
        }

        private void EjecutarComandoAbrirCrearCuenta(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            AbrirVentanaCrearCuenta();
        }

        private async Task EjecutarComandoIniciarSesionInvitadoAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await IniciarSesionInvitadoAsync().ConfigureAwait(true);
        }

        private bool ValidarPuedeProcesarSesion(object parametro)
        {
            return !EstaProcesando;
        }

        private static void ValidarDependenciasBase(
            VistaModeloBaseDependencias dependenciasBase)
        {
            if (dependenciasBase == null)
            {
                throw new ArgumentNullException(nameof(dependenciasBase));
            }
        }

        private static void ValidarDependenciasInicioSesion(
            InicioSesionDependencias dependencias)
        {
            if (dependencias == null)
            {
                throw new ArgumentNullException(nameof(dependencias));
            }
        }

        /// <summary>
        /// Identificador del usuario (nombre de usuario o correo).
        /// </summary>
        public string Identificador
        {
            get => _identificador;
            set => EstablecerPropiedad(ref _identificador, value);
        }

        /// <summary>
        /// Coleccion de idiomas disponibles para seleccionar.
        /// </summary>
        public ObservableCollection<IdiomaOpcion> IdiomasDisponibles
        {
            get => _idiomasDisponibles;
            private set => EstablecerPropiedad(ref _idiomasDisponibles, value);
        }

        /// <summary>
        /// Idioma actualmente seleccionado.
        /// </summary>
        public IdiomaOpcion IdiomaSeleccionado
        {
            get => _idiomaSeleccionado;
            set
            {
                if (EstablecerPropiedad(ref _idiomaSeleccionado, value) && value != null)
                {
                    _localizacionServicio.EstablecerIdioma(value.Codigo);
                }
            }
        }

        /// <summary>
        /// Indica si hay una operacion en curso.
        /// </summary>
        public bool EstaProcesando
        {
            get => _estaProcesando;
            private set
            {
                if (EstablecerPropiedad(ref _estaProcesando, value))
                {
                    IniciarSesionComando.NotificarPuedeEjecutar();
                    RecuperarCuentaComando.NotificarPuedeEjecutar();
                    IniciarSesionInvitadoComando.NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Indica si la sesion se inicio exitosamente.
        /// </summary>
        public bool SesionIniciada
        {
            get => _sesionIniciada;
            private set => EstablecerPropiedad(ref _sesionIniciada, value);
        }

        /// <summary>
        /// Comando para iniciar sesion con credenciales.
        /// </summary>
        public IComandoAsincrono IniciarSesionComando { get; }

        /// <summary>
        /// Comando para iniciar recuperacion de cuenta.
        /// </summary>
        public IComandoAsincrono RecuperarCuentaComando { get; }

        /// <summary>
        /// Comando para abrir ventana de creacion de cuenta.
        /// </summary>
        public ICommand AbrirCrearCuentaComando { get; }

        /// <summary>
        /// Comando para iniciar sesion como invitado.
        /// </summary>
        public IComandoAsincrono IniciarSesionInvitadoComando { get; }

        /// <summary>
        /// Accion para notificar campos invalidos a la vista.
        /// </summary>
        public Action<IList<string>> MostrarCamposInvalidos { get; set; }

        /// <summary>
        /// Establece la contrasena ingresada por el usuario.
        /// </summary>
        /// <param name="contrasena">Contrasena a establecer.</param>
        public void EstablecerContrasena(string contrasena)
        {
            _contrasena = contrasena;
        }

        private void AbrirVentanaCrearCuenta()
        {
            var codigoServ = new VerificacionCodigoServicio(
                App.WcfEjecutor, App.WcfFabrica, App.ManejadorError);
            var cuentaServ = new CuentaServicio(
                App.WcfEjecutor, App.WcfFabrica, App.ManejadorError);
            var selectAvatar = new SeleccionAvatarDialogoServicio(
                _avisoServicio, App.CatalogoAvatares, _sonidoManejador);
            var verifCodigo = new VerificacionCodigoDialogoServicio();

            var dependenciasBase = new VistaModeloBaseDependencias(
                _ventana,
                _localizador,
                _sonidoManejador,
                _avisoServicio);

            var dependenciasCreacion = new CreacionCuentaDependencias(
                codigoServ,
                cuentaServ,
                selectAvatar,
                verifCodigo,
                App.CatalogoAvatares,
                _localizacionServicio);

            var crearCuentaVistaModelo = new CreacionCuentaVistaModelo(
                dependenciasBase,
                dependenciasCreacion);

            _ventana.MostrarVentanaDialogo(crearCuentaVistaModelo);
        }

        private void NavegarAVentanaPrincipal()
        {
            _navegacion.NavegarAVentanaPrincipal(this);
        }

        private void NavegarAVentanaSala(NavegacionSalaParametros parametros)
        {
            _navegacion.NavegarAVentanaSala(parametros);
        }

        private async Task IniciarSesionInvitadoAsync()
        {
            await EjecutarOperacionAsync(() =>
            {
                ISalasServicio salasServicio = CrearServicioSalas();

                if (!ValidarServicioSalas(salasServicio))
                {
                    return Task.CompletedTask;
                }

                var vistaModelo = CrearVistaModeloIngresoInvitado(salasServicio);

                if (!MostrarDialogoIngresoInvitado(vistaModelo))
                {
                    salasServicio.Dispose();
                    return Task.CompletedTask;
                }

                if (vistaModelo.SeUnioSala)
                {
                    UnirseASalaComoInvitado(vistaModelo, salasServicio);
                }
                else
                {
                    salasServicio.Dispose();
                }

                return Task.CompletedTask;
            }, ManejarErrorInicioInvitado);
        }

        private ISalasServicio CrearServicioSalas()
        {
            return _salasServicioFactory?.Invoke();
        }

        private bool ValidarServicioSalas(ISalasServicio servicio)
        {
            if (servicio == null)
            {
                _logger.Error("La fabrica de servicios devolvio un servicio de salas nulo.");
                MostrarErrorInvitadoGenerico();
                return false;
            }

            return true;
        }

        private IngresoPartidaInvitadoVistaModelo CrearVistaModeloIngresoInvitado(
            ISalasServicio salasServicio)
        {
            return new IngresoPartidaInvitadoVistaModelo(
                _ventana,
                _localizador,
                _localizacionServicio,
                salasServicio,
                _avisoServicio,
                _sonidoManejador,
                _generadorNombres);
        }

        private void UnirseASalaComoInvitado(
            IngresoPartidaInvitadoVistaModelo vistaModelo,
            ISalasServicio salasServicio)
        {
            _logger.InfoFormat("Invitado {0} se unio a sala {1}",
                vistaModelo.NombreInvitadoGenerado, vistaModelo.SalaUnida?.Codigo);
            var parametros = new NavegacionSalaParametros(
                vistaModelo.SalaUnida,
                salasServicio,
                vistaModelo.NombreInvitadoGenerado,
                true,
                this);
            NavegarAVentanaSala(parametros);
        }

        private bool MostrarDialogoIngresoInvitado(
            IngresoPartidaInvitadoVistaModelo vistaModelo)
        {
            _ventana.MostrarVentanaDialogo(vistaModelo);
            return true;
        }

        private void ManejarErrorInicioInvitado(Exception ex)
        {
            _logger.Error("Error critico al iniciar flujo de invitado.", ex);
            MostrarErrorInvitadoGenerico();
        }

        private void MostrarErrorInvitadoGenerico()
        {
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(Lang.errorTextoNoEncuentraPartida);
        }

        private async Task IniciarSesionAsync()
        {
            LimpiarErroresVisuales();
            var camposInvalidos = ValidarCamposInicioSesion();

            if (camposInvalidos.Any())
            {
                ManejarValidacionFallida(camposInvalidos);
                return;
            }

            EstaProcesando = true;

            await EjecutarOperacionAsync(
                EjecutarInicioSesionAsync,
                ManejarErrorInicioSesion);

            EstaProcesando = false;
        }

        private void ManejarValidacionFallida(List<string> camposInvalidos)
        {
            MarcarCamposInvalidosEnUI(camposInvalidos);
            _avisoServicio.Mostrar(Lang.errorTextoCamposInvalidosGenerico);
            _sonidoManejador.ReproducirError();
            _logger.Warn("Intento de inicio de sesion con campos vacios.");
        }

        private async Task EjecutarInicioSesionAsync()
        {
            var solicitud = CrearCredenciales();
            
            DTOs.ResultadoInicioSesionDTO resultado = 
                await EnviarCredencialesAlServidorAsync(solicitud);

            ProcesarResultadoInicioSesion(resultado);
        }

        private void ManejarErrorInicioSesion(Exception excepcion)
        {
            _logger.Error("Error durante inicio de sesion.", excepcion);
            _sonidoManejador.ReproducirError();
            
            string localizado = ObtenerMensajeErrorInicioSesion(excepcion);
            _avisoServicio.Mostrar(localizado);
        }

        private string ObtenerMensajeErrorInicioSesion(Exception excepcion)
        {
            if (excepcion is ServicioExcepcion servicioExcepcion)
            {
                if (servicioExcepcion.Tipo == TipoErrorServicio.TiempoAgotado ||
                    servicioExcepcion.Tipo == TipoErrorServicio.Comunicacion)
                {
                    return servicioExcepcion.Message;
                }
            }

            return _localizador.Localizar(
                excepcion.Message,
                Lang.errorTextoInicioSesionServicio);
        }

        private List<string> ValidarCamposInicioSesion()
        {
            string identificador = Identificador?.Trim();
            return _validadorCuenta.ValidarCamposInicioSesion(identificador, _contrasena);
        }

        private void LimpiarErroresVisuales()
        {
            MostrarCamposInvalidos?.Invoke(Array.Empty<string>());
        }

        private void MarcarCamposInvalidosEnUI(List<string> campos)
        {
            MostrarCamposInvalidos?.Invoke(campos);
        }

        private DTOs.CredencialesInicioSesionDTO CrearCredenciales()
        {
            return new DTOs.CredencialesInicioSesionDTO
            {
                Identificador = Identificador?.Trim(),
                Contrasena = _contrasena
            };
        }

        private async Task<DTOs.ResultadoInicioSesionDTO> EnviarCredencialesAlServidorAsync(
            DTOs.CredencialesInicioSesionDTO solicitud)
        {
            return await _inicioSesionServicio.IniciarSesionAsync(solicitud)
                .ConfigureAwait(true);
        }

        private void ProcesarResultadoInicioSesion(
            DTOs.ResultadoInicioSesionDTO resultado)
        {
            if (!ValidarResultadoInicioSesion(resultado))
            {
                return;
            }

            if (!resultado.InicioSesionExitoso)
            {
                ManejarInicioSesionFallido(resultado);
                return;
            }

            CompletarInicioSesionExitoso(resultado);
        }

        private bool ValidarResultadoInicioSesion(
            DTOs.ResultadoInicioSesionDTO resultado)
        {
            if (resultado == null)
            {
                _logger.Error("El servicio de inicio de sesion retorno null.");
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(Lang.errorTextoServidorInicioSesion);
                return false;
            }

            return true;
        }

        private void ManejarInicioSesionFallido(
            DTOs.ResultadoInicioSesionDTO resultado)
        {
            _logger.WarnFormat(
                "Inicio de sesion fallido. Mensaje servidor: {0}",
                resultado.Mensaje);
            _sonidoManejador.ReproducirError();
            MostrarErrorInicioSesion(resultado);
        }

        private void CompletarInicioSesionExitoso(
            DTOs.ResultadoInicioSesionDTO resultado)
        {
            CargarUsuarioAutenticado(resultado);
            _sonidoManejador.ReproducirNotificacion();
            SesionIniciada = true;
            NavegarAVentanaPrincipal();
        }

        private void CargarUsuarioAutenticado(
            DTOs.ResultadoInicioSesionDTO resultado)
        {
            if (resultado.Usuario != null)
            {
                _logger.InfoFormat(
                    "Sesion establecida exitosamente para ID: {0}",
                    resultado.Usuario.UsuarioId);
                _usuarioSesion.CargarDesdeDTO(resultado.Usuario);
            }
        }

        private void MostrarErrorInicioSesion(DTOs.ResultadoInicioSesionDTO resultado)
        {
            string mensaje = resultado?.Mensaje;

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = (resultado?.ContrasenaIncorrecta == true || 
                    resultado?.CuentaEncontrada == false)
                    ? Lang.errorTextoCredencialesIncorrectas
                    : Lang.errorTextoInicioSesionServicio;
            }

            _avisoServicio.Mostrar(mensaje);
        }

        private async Task RecuperarCuentaAsync()
        {
            if (!ValidarIdentificadorRecuperacion())
            {
                return;
            }

            EstaProcesando = true;

            await EjecutarOperacionAsync(
                EjecutarRecuperacionAsync,
                ManejarErrorRecuperacion);

            EstaProcesando = false;
        }

        private async Task EjecutarRecuperacionAsync()
        {
            string identificador = Identificador?.Trim();
            _logger.InfoFormat(
                "Solicitando recuperacion de cuenta para: {0}",
                identificador);

            DTOs.ResultadoOperacionDTO resultado = 
                await SolicitarRecuperacionCuentaAsync(identificador);

            ProcesarResultadoRecuperacion(resultado);
        }

        private void ManejarErrorRecuperacion(Exception excepcion)
        {
            _logger.Error("Error durante recuperacion de cuenta.", excepcion);
            _sonidoManejador.ReproducirError();
            
            string mensajeLocalizado = ObtenerMensajeErrorRecuperacion(excepcion);
            _avisoServicio.Mostrar(mensajeLocalizado);
        }

        private string ObtenerMensajeErrorRecuperacion(Exception excepcion)
        {
            if (excepcion is ServicioExcepcion servicioExcepcion)
            {
                if (servicioExcepcion.Tipo == TipoErrorServicio.TiempoAgotado ||
                    servicioExcepcion.Tipo == TipoErrorServicio.Comunicacion)
                {
                    return servicioExcepcion.Message;
                }
            }

            return _localizador.Localizar(
                excepcion.Message, 
                Lang.errorTextoCuentaNoRegistrada);
        }

        private bool ValidarIdentificadorRecuperacion()
        {
            string identificador = Identificador?.Trim();

            if (string.IsNullOrWhiteSpace(identificador))
            {
                MostrarErrorIdentificadorRequerido();
                return false;
            }

            return true;
        }

        private void MostrarErrorIdentificadorRequerido()
        {
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(Lang.errorTextoIdentificadorRecuperacionRequerido);
        }

        private async Task<DTOs.ResultadoOperacionDTO> SolicitarRecuperacionCuentaAsync(
            string identificador)
        {
            return await _recuperacionCuentaDialogoServicio.RecuperarCuentaAsync(
                identificador, _cambioContrasenaServicio).ConfigureAwait(true);
        }

        private void ProcesarResultadoRecuperacion(
            DTOs.ResultadoOperacionDTO resultado)
        {
            if (resultado?.OperacionExitosa == false &&
                !string.IsNullOrWhiteSpace(resultado.Mensaje))
            {
                _logger.WarnFormat(
                    "Recuperacion fallida o cancelada: {0}",
                    resultado.Mensaje);
                _sonidoManejador.ReproducirError();

                string mensajeLocalizado = LocalizarMensajeRecuperacion(
                    resultado.Mensaje);
                _avisoServicio.Mostrar(mensajeLocalizado);
            }
        }

        private string LocalizarMensajeRecuperacion(string mensaje)
        {
            return _localizador.Localizar(mensaje, Lang.errorTextoCuentaNoRegistrada);
        }

        private void CargarIdiomas()
        {
            WeakEventManager<ILocalizacionServicio, EventArgs>.AddHandler(
                _localizacionServicio,
                nameof(ILocalizacionServicio.IdiomaActualizado),
                LocalizacionServicioEnIdiomaActualizado);

            ActualizarIdiomasDisponibles(_localizacionServicio.CulturaActual?.Name
                ?? CultureInfo.CurrentUICulture?.Name);
        }

        private void LocalizacionServicioEnIdiomaActualizado(
            object remitente,
            EventArgs argumentosEvento)
        {
            ActualizarIdiomasDisponibles(_localizacionServicio.CulturaActual?.Name);
        }

        private void ActualizarIdiomasDisponibles(string culturaActual)
        {
            var opciones = CrearOpcionesIdioma();
            EstablecerOpcionesIdioma(opciones);
            SeleccionarIdiomaActual(culturaActual);
        }

        private static IdiomaOpcion[] CrearOpcionesIdioma()
        {
            return new[]
            {
                new IdiomaOpcion("es-MX", Lang.idiomaTextoEspanol),
                new IdiomaOpcion("en-US", Lang.idiomaTextoIngles)
            };
        }

        private void EstablecerOpcionesIdioma(IdiomaOpcion[] opciones)
        {
            if (IdiomasDisponibles == null)
            {
                IdiomasDisponibles = new ObservableCollection<IdiomaOpcion>(opciones);
            }
            else
            {
                IdiomasDisponibles.Clear();

                foreach (var opcion in opciones)
                {
                    IdiomasDisponibles.Add(opcion);
                }
            }
        }

        private void SeleccionarIdiomaActual(string culturaActual)
        {
            if (string.IsNullOrWhiteSpace(culturaActual))
            {
                IdiomaSeleccionado = IdiomasDisponibles.FirstOrDefault();
                return;
            }

            IdiomaSeleccionado = IdiomasDisponibles
                .FirstOrDefault(i => string.Equals(
                    i.Codigo,
                    culturaActual,
                    StringComparison.OrdinalIgnoreCase))
                ?? IdiomasDisponibles.FirstOrDefault();
        }
    }
}