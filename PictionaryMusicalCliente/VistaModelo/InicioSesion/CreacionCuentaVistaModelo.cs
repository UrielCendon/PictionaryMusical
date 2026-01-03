using log4net;
using PictionaryMusicalCliente.ClienteServicios;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using PictionaryMusicalCliente.VistaModelo.InicioSesion.Auxiliares;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.InicioSesion
{
    /// <summary>
    /// Gestiona la logica para el registro de nuevas cuentas de usuario.
    /// </summary>
    /// <remarks>
    /// Coordina la validacion de campos, envio de codigo de verificacion
    /// y registro final de la cuenta.
    /// </remarks>
    public class CreacionCuentaVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICodigoVerificacionServicio _codigoVerificacionServicio;
        private readonly ICuentaServicio _cuentaServicio;
        private readonly ISeleccionarAvatarServicio _seleccionarAvatarServicio;
        private readonly IVerificacionCodigoDialogoServicio 
            _verificarCodigoDialogoServicio;
        private readonly ILocalizacionServicio _localizacionServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly ICatalogoAvatares _catalogoAvatares;
        private readonly IAvisoServicio _avisoServicio;
        private readonly ValidadorCuenta _validadorCuenta;

        private string _usuario;
        private string _nombre;
        private string _apellido;
        private string _correo;
        private string _contrasena;
        private ImageSource _avatarSeleccionadoImagen;
        private int _avatarSeleccionadoId;
        private bool _mostrarErrorUsuario;
        private bool _mostrarErrorCorreo;
        private bool _estaProcesando;

        /// <summary>
        /// Inicializa una nueva instancia de la clase.
        /// </summary>
        /// <param name="dependenciasBase">
        /// Dependencias comunes de UI del ViewModel.
        /// </param>
        /// <param name="dependencias">
        /// Dependencias especificas de creacion de cuenta.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Si algun parametro requerido es nulo.
        /// </exception>
        public CreacionCuentaVistaModelo(
            VistaModeloBaseDependencias dependenciasBase,
            CreacionCuentaDependencias dependencias)
            : base(dependenciasBase?.Ventana, dependenciasBase?.Localizador)
        {
            ValidarDependencias(dependenciasBase, dependencias);

            _sonidoManejador = dependenciasBase.SonidoManejador;
            _avisoServicio = dependenciasBase.AvisoServicio;

            _codigoVerificacionServicio = dependencias.CodigoVerificacionServicio;
            _cuentaServicio = dependencias.CuentaServicio;
            _seleccionarAvatarServicio = dependencias.SeleccionarAvatarServicio;
            _verificarCodigoDialogoServicio = 
                dependencias.VerificacionCodigoDialogoServicio;
            _catalogoAvatares = dependencias.CatalogoAvatares;
            _localizacionServicio = dependencias.LocalizacionServicio;
            _validadorCuenta = new ValidadorCuenta();

            CrearCuentaComando = new ComandoAsincrono(
                EjecutarComandoCrearCuentaAsync, 
                ValidarPuedeCrearCuenta);

            CancelarComando = new ComandoDelegado(EjecutarComandoCancelarCreacion);

            SeleccionarAvatarComando = new ComandoAsincrono(EjecutarComandoSeleccionarAvatarAsync);

            EstablecerAvatarPredeterminado();
        }

        private async Task EjecutarComandoCrearCuentaAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await CrearCuentaAsync();
        }

        private bool ValidarPuedeCrearCuenta(object parametro)
        {
            return !EstaProcesando;
        }

        private void EjecutarComandoCancelarCreacion(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            _ventana.CerrarVentana(this);
        }

        private async Task EjecutarComandoSeleccionarAvatarAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await SeleccionarAvatarAsync();
        }

        private static void ValidarDependencias(
            VistaModeloBaseDependencias dependenciasBase,
            CreacionCuentaDependencias dependencias)
        {
            if (dependenciasBase == null)
            {
                throw new ArgumentNullException(nameof(dependenciasBase));
            }

            if (dependencias == null)
            {
                throw new ArgumentNullException(nameof(dependencias));
            }
        }

        /// <summary>
        /// Nombre de usuario para la nueva cuenta.
        /// </summary>
        public string Usuario
        {
            get => _usuario;
            set => EstablecerPropiedad(ref _usuario, value);
        }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre
        {
            get => _nombre;
            set => EstablecerPropiedad(ref _nombre, value);
        }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public string Apellido
        {
            get => _apellido;
            set => EstablecerPropiedad(ref _apellido, value);
        }

        /// <summary>
        /// Correo electronico del usuario.
        /// </summary>
        public string Correo
        {
            get => _correo;
            set => EstablecerPropiedad(ref _correo, value);
        }

        /// <summary>
        /// Contrasena para la nueva cuenta.
        /// </summary>
        public string Contrasena
        {
            get => _contrasena;
            set => EstablecerPropiedad(ref _contrasena, value);
        }

        /// <summary>
        /// Imagen del avatar seleccionado.
        /// </summary>
        public ImageSource AvatarSeleccionadoImagen
        {
            get => _avatarSeleccionadoImagen;
            private set => EstablecerPropiedad(ref _avatarSeleccionadoImagen, value);
        }

        /// <summary>
        /// Identificador del avatar seleccionado.
        /// </summary>
        public int AvatarSeleccionadoId
        {
            get => _avatarSeleccionadoId;
            private set => EstablecerPropiedad(ref _avatarSeleccionadoId, value);
        }

        /// <summary>
        /// Indica si debe mostrarse error en el campo usuario.
        /// </summary>
        public bool MostrarErrorUsuario
        {
            get => _mostrarErrorUsuario;
            private set => EstablecerPropiedad(ref _mostrarErrorUsuario, value);
        }

        /// <summary>
        /// Indica si debe mostrarse error en el campo correo.
        /// </summary>
        public bool MostrarErrorCorreo
        {
            get => _mostrarErrorCorreo;
            private set => EstablecerPropiedad(ref _mostrarErrorCorreo, value);
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
                    ((IComandoNotificable)CrearCuentaComando)
                        .NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Comando para iniciar el proceso de creacion de cuenta.
        /// </summary>
        public IComandoAsincrono CrearCuentaComando { get; }

        /// <summary>
        /// Comando para cancelar y cerrar la ventana.
        /// </summary>
        public ICommand CancelarComando { get; }

        /// <summary>
        /// Comando para abrir el selector de avatar.
        /// </summary>
        public IComandoAsincrono SeleccionarAvatarComando { get; }

        /// <summary>
        /// Accion para notificar campos invalidos a la vista.
        /// </summary>
        public Action<IList<string>> MostrarCamposInvalidos { get; set; }

        /// <summary>
        /// Accion para mostrar mensajes al usuario.
        /// </summary>
        public Action<string> MostrarMensaje { get; set; }

        /// <summary>
        /// Indica si el registro se completo exitosamente.
        /// </summary>
        public bool RegistroExitoso { get; private set; }

        private async Task CrearCuentaAsync()
        {
            LimpiarErroresVisuales();
            var resultadoValidacion = ValidarEntradas();
            
            if (!resultadoValidacion.EsValido)
            {
                MostrarErroresValidacion(
                    resultadoValidacion.CamposInvalidos,
                    resultadoValidacion.PrimerMensajeError);
                _sonidoManejador.ReproducirError();
                _logger.Warn(
                    "Intento de creacion de cuenta fallido por validacion.");
                return;
            }

            EstaProcesando = true;

            await EjecutarOperacionAsync(
                async () => await EjecutarFlujoDeRegistroAsync(
                    resultadoValidacion.Solicitud),
                ManejarErrorCreacion);

            EstaProcesando = false;
        }

        private void ManejarErrorCreacion(Exception excepcion)
        {
            _logger.WarnFormat(
                "Error de servicio durante la creacion de cuenta: {0}",
                excepcion.Message);
            _sonidoManejador.ReproducirError();
            string mensaje = ObtenerMensajeErrorCreacion(excepcion);
            MostrarMensaje?.Invoke(mensaje);
            EstaProcesando = false;
        }

        private string ObtenerMensajeErrorCreacion(Exception excepcion)
        {
            if (excepcion is ServicioExcepcion servicioExcepcion)
            {
                if (servicioExcepcion.Tipo == TipoErrorServicio.TiempoAgotado ||
                    servicioExcepcion.Tipo == TipoErrorServicio.Comunicacion)
                {
                    return Lang.errorTextoServidorSinDisponibilidad;
                }
            }

            return _localizador.Localizar(
                excepcion.Message,
                Lang.errorTextoRegistrarCuentaMasTarde);
        }

        private ResultadoValidacionCuenta ValidarEntradas()
        {
            var (solicitud, camposInvalidos, primerMensajeError) = 
                LimpiarYValidarEntradas();

            if (camposInvalidos.Count == 0)
            {
                return ResultadoValidacionCuenta.Exitoso(solicitud);
            }

            return ResultadoValidacionCuenta.Fallido(
                camposInvalidos,
                primerMensajeError);
        }

        private void LimpiarErroresVisuales()
        {
            MostrarErrorUsuario = false;
            MostrarErrorCorreo = false;
            MostrarCamposInvalidos?.Invoke(Array.Empty<string>());
        }

        private void MostrarErroresValidacion(
            List<string> camposInvalidos,
            string primerMensajeError)
        {
            MostrarCamposInvalidos?.Invoke(camposInvalidos);
            
            string mensajeMostrar = camposInvalidos.Count > 1
                ? Lang.errorTextoCamposInvalidosGenerico
                : primerMensajeError;
            
            MostrarMensaje?.Invoke(mensajeMostrar ?? 
                Lang.errorTextoCamposInvalidosGenerico);
        }

        private async Task EjecutarFlujoDeRegistroAsync(DTOs.NuevaCuentaDTO solicitud)
        {
            if (!await SolicitarYValidarCodigoRegistroAsync(solicitud))
            {
                return;
            }

            if (!await VerificarCodigoConUsuarioAsync())
            {
                return;
            }

            await CompletarRegistroCuentaAsync(solicitud);
        }

        private async Task<bool> SolicitarYValidarCodigoRegistroAsync(
            DTOs.NuevaCuentaDTO solicitud)
        {
            var (codigoEnviado, _, errorDuplicado) =
                await SolicitarCodigoRegistroAsync(solicitud);

            if (!codigoEnviado)
            {
                _sonidoManejador.ReproducirError();
                
                if (errorDuplicado)
                {
                    _logger.Warn(
                        "Intento de registro con usuario o correo ya existente.");
                    MostrarErroresCamposDuplicados();
                }
                
                return false;
            }

            return true;
        }

        private async Task<bool> VerificarCodigoConUsuarioAsync()
        {
            var (verificacionExitosa, _) = 
                await MostrarDialogoVerificacionAsync();

            if (!verificacionExitosa)
            {
                _logger.Warn(
                    "Verificacion de codigo fallida o cancelada por el usuario.");
                _sonidoManejador.ReproducirError();
                return false;
            }

            return true;
        }

        private async Task CompletarRegistroCuentaAsync(
            DTOs.NuevaCuentaDTO solicitud)
        {
            var (registroExitoso, _) = await RegistrarCuentaAsync(solicitud);

            if (registroExitoso)
            {
                FinalizarRegistroExitoso(solicitud.Usuario);
            }
            else
            {
                ManejarErrorRegistro();
            }
        }

        private void FinalizarRegistroExitoso(string usuario)
        {
            _logger.InfoFormat("Cuenta creada exitosamente para usuario: {0}",
                usuario);
            _sonidoManejador.ReproducirNotificacion();
            MostrarMensaje?.Invoke(Lang.crearCuentaTextoExitosoMensaje);
            RegistroExitoso = true;
            _ventana.CerrarVentana(this);
        }

        private void ManejarErrorRegistro()
        {
            _logger.Warn("Fallo en el paso final de registro de cuenta.");
            _sonidoManejador.ReproducirError();
            MostrarErroresCamposDuplicados();
        }

        private void MostrarErroresCamposDuplicados()
        {
            var camposDuplicados = new List<string>();
            
            if (MostrarErrorUsuario)
            {
                camposDuplicados.Add(nameof(Usuario));
            }
            
            if (MostrarErrorCorreo)
            {
                camposDuplicados.Add(nameof(Correo));
            }
            
            MostrarCamposInvalidos?.Invoke(camposDuplicados);
        }

        private (DTOs.NuevaCuentaDTO Solicitud, List<string> CamposInvalidos,
            string PrimerMensajeError) LimpiarYValidarEntradas()
        {
            LimpiarCamposTexto();
            
            ResultadoValidacionCampos resultado = _validadorCuenta.ValidarCamposCreacion(
                Usuario, Nombre, Apellido, Correo, Contrasena, AvatarSeleccionadoId);

            if (!resultado.EsValido)
            {
                return (null, resultado.CamposInvalidos, resultado.PrimerMensajeError);
            }

            DTOs.NuevaCuentaDTO solicitud = CrearSolicitudNuevaCuenta();
            return (solicitud, resultado.CamposInvalidos, resultado.PrimerMensajeError);
        }

        private void LimpiarCamposTexto()
        {
            Usuario = Usuario?.Trim();
            Nombre = Nombre?.Trim();
            Apellido = Apellido?.Trim();
            Correo = Correo?.Trim();
            Contrasena = Contrasena?.Trim();
        }

        private DTOs.NuevaCuentaDTO CrearSolicitudNuevaCuenta()
        {
            return new DTOs.NuevaCuentaDTO
            {
                Usuario = Usuario,
                Nombre = Nombre,
                Apellido = Apellido,
                Correo = Correo,
                Contrasena = Contrasena,
                AvatarId = AvatarSeleccionadoId,
                Idioma = _localizacionServicio?.CulturaActual?.Name
                    ?? CultureInfo.CurrentUICulture?.Name
            };
        }

        private DTOs.ResultadoSolicitudCodigoDTO _resultadoSolicitudCodigo;

        private async Task<(bool CodigoEnviado, 
            DTOs.ResultadoSolicitudCodigoDTO Resultado, bool ErrorDuplicado)>
            SolicitarCodigoRegistroAsync(DTOs.NuevaCuentaDTO solicitud)
        {
            DTOs.ResultadoSolicitudCodigoDTO resultado = 
                await _codigoVerificacionServicio
                    .SolicitarCodigoRegistroAsync(solicitud)
                    .ConfigureAwait(true);

            if (!ValidarResultadoSolicitudCodigo(resultado))
            {
                return (false, null, false);
            }

            _resultadoSolicitudCodigo = resultado;

            ActualizarErroresDuplicados(resultado);

            if (TieneCamposDuplicados(resultado))
            {
                return (false, resultado, true);
            }

            if (!resultado.CodigoEnviado)
            {
                MostrarErrorEnvioCodigo(resultado);
                return (false, resultado, false);
            }

            return (true, resultado, false);
        }

        private bool ValidarResultadoSolicitudCodigo(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            if (resultado == null)
            {
                _logger.Error(
                    "El servicio de codigo de verificacion retorno null.");
                MostrarMensaje?.Invoke(Lang.errorTextoRegistrarCuentaMasTarde);
                return false;
            }

            return true;
        }

        private void ActualizarErroresDuplicados(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            MostrarErrorUsuario = resultado.UsuarioRegistrado;
            MostrarErrorCorreo = resultado.CorreoRegistrado;
        }

        private static bool TieneCamposDuplicados(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            return resultado.UsuarioRegistrado || resultado.CorreoRegistrado;
        }

        private void MostrarErrorEnvioCodigo(
            DTOs.ResultadoSolicitudCodigoDTO resultado)
        {
            _logger.WarnFormat("No se pudo enviar el codigo. Mensaje: {0}",
                resultado.Mensaje);
            string mensajeLocalizado = _localizador.Localizar(
                resultado.Mensaje,
                Lang.errorTextoRegistrarCuentaMasTarde);
            MostrarMensaje?.Invoke(mensajeLocalizado);
        }

        private async Task<(bool VerificacionExitosa, 
            DTOs.ResultadoRegistroCuentaDTO Resultado)>
            MostrarDialogoVerificacionAsync()
        {
            var parametros = new VerificacionDialogoParametros(
                Lang.cambiarContrasenaTextoCodigoVerificacion,
                _resultadoSolicitudCodigo.TokenCodigo,
                _codigoVerificacionServicio);

            DTOs.ResultadoRegistroCuentaDTO resultadoVerificacion =
                await _verificarCodigoDialogoServicio.MostrarDialogoAsync(
                    parametros,
                    _avisoServicio,
                    _sonidoManejador)
                .ConfigureAwait(true);

            if (!ValidarResultadoVerificacion(resultadoVerificacion))
            {
                MostrarMensajeVerificacionFallida(resultadoVerificacion);
                return (false, resultadoVerificacion);
            }

            return (true, resultadoVerificacion);
        }

        private static bool ValidarResultadoVerificacion(
            DTOs.ResultadoRegistroCuentaDTO resultadoVerificacion)
        {
            return resultadoVerificacion != null && 
                   resultadoVerificacion.RegistroExitoso;
        }

        private void MostrarMensajeVerificacionFallida(
            DTOs.ResultadoRegistroCuentaDTO resultadoVerificacion)
        {
            if (!string.IsNullOrWhiteSpace(resultadoVerificacion?.Mensaje))
            {
                MostrarMensaje?.Invoke(resultadoVerificacion.Mensaje);
            }
        }

        private async Task<(bool RegistroExitoso, 
            DTOs.ResultadoRegistroCuentaDTO Resultado)>
            RegistrarCuentaAsync(DTOs.NuevaCuentaDTO solicitud)
        {
            DTOs.ResultadoRegistroCuentaDTO resultadoRegistro = 
                await _cuentaServicio
                    .RegistrarCuentaAsync(solicitud)
                    .ConfigureAwait(true);

            if (!ValidarResultadoRegistro(resultadoRegistro))
            {
                return (false, null);
            }

            if (!resultadoRegistro.RegistroExitoso)
            {
                return ProcesarRegistroFallido(resultadoRegistro);
            }

            return (true, resultadoRegistro);
        }

        private bool ValidarResultadoRegistro(
            DTOs.ResultadoRegistroCuentaDTO resultadoRegistro)
        {
            if (resultadoRegistro == null)
            {
                _logger.Error("El servicio de registro de cuenta retorno null.");
                MostrarMensaje?.Invoke(Lang.errorTextoRegistrarCuentaMasTarde);
                return false;
            }

            return true;
        }

        private (bool RegistroExitoso, DTOs.ResultadoRegistroCuentaDTO Resultado)
            ProcesarRegistroFallido(DTOs.ResultadoRegistroCuentaDTO resultadoRegistro)
        {
            MostrarErrorUsuario = resultadoRegistro.UsuarioRegistrado;
            MostrarErrorCorreo = resultadoRegistro.CorreoRegistrado;

            if (resultadoRegistro.UsuarioRegistrado || 
                resultadoRegistro.CorreoRegistrado)
            {
                return (false, resultadoRegistro);
            }

            _logger.WarnFormat("Error al registrar cuenta: {0}",
                resultadoRegistro.Mensaje);
            string mensajeLocalizado = _localizador.Localizar(
                resultadoRegistro.Mensaje,
                Lang.errorTextoRegistrarCuentaMasTarde);
            MostrarMensaje?.Invoke(mensajeLocalizado);
            return (false, resultadoRegistro);
        }

        private async Task SeleccionarAvatarAsync()
        {
            ObjetoAvatar avatar = await _seleccionarAvatarServicio
                .SeleccionarAvatarAsync(AvatarSeleccionadoId)
                .ConfigureAwait(true);

            if (avatar == null)
            {
                return;
            }

            ActualizarAvatarSeleccionado(avatar);
        }

        private void ActualizarAvatarSeleccionado(ObjetoAvatar avatar)
        {
            AvatarSeleccionadoId = avatar.Id;
            AvatarSeleccionadoImagen = avatar.Imagen;
        }

        private void EstablecerAvatarPredeterminado()
        {
            var avatares = _catalogoAvatares.ObtenerAvatares();
            
            if (avatares != null && avatares.Count > 0)
            {
                var avatar = avatares[0];
                AvatarSeleccionadoId = avatar.Id;
                AvatarSeleccionadoImagen = avatar.Imagen;
            }
        }
    }

    /// <summary>
    /// Encapsula el resultado de la validacion de datos de una nueva cuenta.
    /// </summary>
    internal sealed class ResultadoValidacionCuenta
    {
        private ResultadoValidacionCuenta(
            bool esValido,
            DTOs.NuevaCuentaDTO solicitud,
            List<string> camposInvalidos,
            string primerMensajeError)
        {
            EsValido = esValido;
            Solicitud = solicitud;
            CamposInvalidos = camposInvalidos ?? new List<string>();
            PrimerMensajeError = primerMensajeError;
        }

        /// <summary>
        /// Indica si la validacion fue exitosa.
        /// </summary>
        public bool EsValido { get; }

        /// <summary>
        /// Solicitud de nueva cuenta si la validacion fue exitosa.
        /// </summary>
        public DTOs.NuevaCuentaDTO Solicitud { get; }

        /// <summary>
        /// Lista de nombres de campos que fallaron la validacion.
        /// </summary>
        public List<string> CamposInvalidos { get; }

        /// <summary>
        /// Primer mensaje de error encontrado durante la validacion.
        /// </summary>
        public string PrimerMensajeError { get; }

        /// <summary>
        /// Crea un resultado de validacion exitosa.
        /// </summary>
        /// <param name="solicitud">Solicitud validada.</param>
        public static ResultadoValidacionCuenta Exitoso(
            DTOs.NuevaCuentaDTO solicitud)
        {
            return new ResultadoValidacionCuenta(true, solicitud, null, null);
        }

        /// <summary>
        /// Crea un resultado de validacion fallida.
        /// </summary>
        /// <param name="camposInvalidos">Campos que fallaron.</param>
        /// <param name="primerMensajeError">Mensaje del primer error.</param>
        public static ResultadoValidacionCuenta Fallido(
            List<string> camposInvalidos,
            string primerMensajeError)
        {
            return new ResultadoValidacionCuenta(
                false,
                null,
                camposInvalidos,
                primerMensajeError);
        }
    }
}