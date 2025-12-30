using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.VistaModelo.Auxiliares;
using PictionaryMusicalCliente.VistaModelo.Dependencias;
using PictionaryMusicalCliente.VistaModelo.Perfil.Auxiliares;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Perfil
{
    /// <summary>
    /// Gestiona la logica para visualizar y editar el perfil del usuario.
    /// </summary>
    public class PerfilVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IPerfilServicio _perfilServicio;
        private readonly ISeleccionarAvatarServicio _seleccionarAvatarServicio;
        private readonly ICambioContrasenaServicio _cambioContrasenaServicio;
        private readonly IRecuperacionCuentaServicio 
            _recuperacionCuentaDialogoServicio;
        private readonly IAvisoServicio _avisoServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IUsuarioAutenticado _usuarioSesion;
        private readonly ICatalogoAvatares _catalogoAvatares;
        private readonly RedesSocialesManejador _redesSocialesManejador;

        private int _usuarioId;
        private string _usuario;
        private string _correo;
        private string _nombre;
        private string _apellido;
        private string _avatarSeleccionadoNombre;
        private int _avatarSeleccionadoId;
        private ImageSource _avatarSeleccionadoImagen;
        private bool _estaProcesando;
        private bool _estaCambiandoContrasena;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="PerfilVistaModelo"/>.
        /// </summary>
        /// <param name="ventana">Servicio de ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="perfilServicio">Servicio de perfil.</param>
        /// <param name="seleccionarAvatarServicio">
        /// Servicio de seleccion de avatar.
        /// </param>
        /// <param name="cambioContrasenaServicio">
        /// Servicio de cambio de contrasena.
        /// </param>
        /// <param name="recuperacionCuentaDialogoServicio">
        /// Servicio de dialogo de recuperacion.
        /// </param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        /// <param name="usuarioSesion">Usuario autenticado actual.</param>
        /// <param name="catalogoAvatares">Catalogo de avatares.</param>
        /// <param name="catalogoPerfil">Catalogo de imagenes de perfil.</param>
        public PerfilVistaModelo(
            VistaModeloBaseDependencias dependenciasBase,
            PerfilDependencias dependencias)
            : base(dependenciasBase?.Ventana, dependenciasBase?.Localizador)
        {
            ValidarDependencias(dependenciasBase, dependencias);

            _avisoServicio = dependenciasBase.AvisoServicio;
            _sonidoManejador = dependenciasBase.SonidoManejador;

            _perfilServicio = dependencias.PerfilServicio;
            _seleccionarAvatarServicio = dependencias.SeleccionarAvatarServicio;
            _cambioContrasenaServicio = dependencias.CambioContrasenaServicio;
            _recuperacionCuentaDialogoServicio = dependencias.RecuperacionCuentaServicio;
            _usuarioSesion = dependencias.UsuarioSesion;
            _catalogoAvatares = dependencias.CatalogoAvatares;

            var catalogoPerfil = dependencias.CatalogoPerfil;
            _redesSocialesManejador = new RedesSocialesManejador(catalogoPerfil);

            GuardarCambiosComando = new ComandoAsincrono(
                EjecutarComandoGuardarCambiosAsync, 
                ValidarPuedeProcesar);

            SeleccionarAvatarComando = new ComandoAsincrono(
                EjecutarComandoSeleccionarAvatarAsync, 
                ValidarPuedeProcesar);

            CambiarContrasenaComando = new ComandoAsincrono(
                EjecutarComandoCambiarContrasenaAsync, 
                ValidarPuedeCambiarContrasena);

            CerrarComando = new ComandoDelegado(EjecutarComandoCerrar);

            ConfigurarEventoDesconexion();
        }

        private async Task EjecutarComandoGuardarCambiosAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await GuardarCambiosAsync();
        }

        private async Task EjecutarComandoSeleccionarAvatarAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await SeleccionarAvatarAsync();
        }

        private async Task EjecutarComandoCambiarContrasenaAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await CambiarContrasenaAsync();
        }

        private bool ValidarPuedeProcesar(object parametro)
        {
            return !EstaProcesando;
        }

        private bool ValidarPuedeCambiarContrasena(object parametro)
        {
            return !EstaProcesando && !EstaCambiandoContrasena;
        }

        private void EjecutarComandoCerrar(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            _ventana.CerrarVentana(this);
        }

        private void ConfigurarEventoDesconexion()
        {
            DesconexionDetectada += ManejarDesconexionServidor;
        }

        private void ManejarDesconexionServidor(string mensaje)
        {
            EjecutarEnDispatcher(() =>
            {
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(mensaje);
                _usuarioSesion?.Limpiar();
                RequiereReinicioSesion = true;
                SolicitarReinicioSesion?.Invoke();
                _ventana.CerrarVentana(this);
            });
        }

        private static void ValidarDependencias(
            VistaModeloBaseDependencias dependenciasBase,
            PerfilDependencias dependencias)
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
        /// Obtiene el nombre de usuario.
        /// </summary>
        public string Usuario 
        { 
            get => _usuario;
            private set => EstablecerPropiedad(ref _usuario, value);
        }

        /// <summary>
        /// Obtiene el correo electronico del usuario.
        /// </summary>
        public string Correo
        {
            get => _correo;
            private set => EstablecerPropiedad(ref _correo, value);
        }

        /// <summary>
        /// Obtiene o establece el nombre del usuario.
        /// </summary>
        public string Nombre
        {
            get => _nombre;
            set => EstablecerPropiedad(ref _nombre, value);
        }

        /// <summary>
        /// Obtiene o establece el apellido del usuario.
        /// </summary>
        public string Apellido
        {
            get => _apellido;
            set => EstablecerPropiedad(ref _apellido, value);
        }

        /// <summary>
        /// Obtiene el nombre del avatar seleccionado.
        /// </summary>
        public string AvatarSeleccionadoNombre
        {
            get => _avatarSeleccionadoNombre;
            private set => EstablecerPropiedad(ref _avatarSeleccionadoNombre, value);
        }

        /// <summary>
        /// Obtiene el identificador del avatar seleccionado.
        /// </summary>
        public int AvatarSeleccionadoId
        {
            get => _avatarSeleccionadoId;
            private set => EstablecerPropiedad(ref _avatarSeleccionadoId, value);
        }

        /// <summary>
        /// Obtiene la imagen del avatar seleccionado.
        /// </summary>
        public ImageSource AvatarSeleccionadoImagen
        {
            get => _avatarSeleccionadoImagen;
            private set => EstablecerPropiedad(ref _avatarSeleccionadoImagen, value);
        }

        /// <summary>
        /// Obtiene la coleccion de redes sociales editables.
        /// </summary>
        public ObservableCollection<RedSocialItemVistaModelo> RedesSociales 
            => _redesSocialesManejador.RedesSociales;

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
                    ((IComandoNotificable)GuardarCambiosComando)
                        .NotificarPuedeEjecutar();
                    ((IComandoNotificable)SeleccionarAvatarComando)
                        .NotificarPuedeEjecutar();
                    ((IComandoNotificable)CambiarContrasenaComando)
                        .NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si se esta cambiando la contrasena.
        /// </summary>
        public bool EstaCambiandoContrasena
        {
            get => _estaCambiandoContrasena;
            private set
            {
                if (EstablecerPropiedad(ref _estaCambiandoContrasena, value))
                {
                    ((IComandoNotificable)CambiarContrasenaComando)
                        .NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Obtiene el comando para guardar cambios del perfil.
        /// </summary>
        public IComandoAsincrono GuardarCambiosComando { get; }

        /// <summary>
        /// Obtiene el comando para seleccionar un avatar.
        /// </summary>
        public IComandoAsincrono SeleccionarAvatarComando { get; }

        /// <summary>
        /// Obtiene el comando para cambiar la contrasena.
        /// </summary>
        public IComandoAsincrono CambiarContrasenaComando { get; }

        /// <summary>
        /// Obtiene el comando para cerrar la ventana.
        /// </summary>
        public ICommand CerrarComando { get; }

        /// <summary>
        /// Obtiene o establece la accion para mostrar campos invalidos en la UI.
        /// </summary>
        public Action<IList<string>> MostrarCamposInvalidos { get; set; }

        /// <summary>
        /// Obtiene o establece la accion para solicitar reinicio de sesion.
        /// </summary>
        public Action SolicitarReinicioSesion { get; set; }

        /// <summary>
        /// Obtiene un valor que indica si se requiere reiniciar sesion.
        /// </summary>
        public bool RequiereReinicioSesion { get; private set; }

        /// <summary>
        /// Obtiene un valor que indica si la carga del perfil fallo.
        /// </summary>
        public bool CargaFallida { get; private set; }

        /// <summary>
        /// Carga los datos del perfil del usuario desde el servidor.
        /// </summary>
        /// <returns>True si la carga fue exitosa; false en caso contrario.</returns>
        public async Task<bool> CargarPerfilAsync()
        {
            if (!ValidarSesionActiva())
            {
                CargaFallida = true;
                return false;
            }

            EstaProcesando = true;
            bool cargaExitosa = false;

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                DTOs.UsuarioDTO perfil = await ObtenerPerfilDesdeServidorAsync();
                
                if (!ValidarPerfilObtenido(perfil))
                {
                    return;
                }

                AplicarPerfil(perfil);
                cargaExitosa = true;
            });

            EstaProcesando = false;
            CargaFallida = !cargaExitosa;
            return cargaExitosa;
        }

        private bool ValidarSesionActiva()
        {
            if (TieneSesionValida())
            {
                return true;
            }

            NotificarSesionInvalida();
            return false;
        }

        private bool TieneSesionValida()
        {
            return _usuarioSesion != null && _usuarioSesion.IdUsuario > 0;
        }

        private void NotificarSesionInvalida()
        {
            _logger.Warn("Intento de cargar perfil sin sesion valida.");
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(Lang.errorTextoPerfilActualizarInformacion);
            _ventana.CerrarVentana(this);
        }

        private async Task<DTOs.UsuarioDTO> ObtenerPerfilDesdeServidorAsync()
        {
            return await _perfilServicio
                .ObtenerPerfilAsync(_usuarioSesion.IdUsuario)
                .ConfigureAwait(true);
        }

        private bool ValidarPerfilObtenido(DTOs.UsuarioDTO perfil)
        {
            if (perfil != null)
            {
                return true;
            }

            NotificarPerfilNoObtenido();
            return false;
        }

        private void NotificarPerfilNoObtenido()
        {
            _logger.ErrorFormat(
                "Perfil obtenido es nulo para ID: {0}",
                _usuarioSesion.IdUsuario);
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(Lang.errorTextoServidorObtenerPerfil);
        }

        private async Task SeleccionarAvatarAsync()
        {
            ObjetoAvatar avatar = await _seleccionarAvatarServicio
                .SeleccionarAvatarAsync(AvatarSeleccionadoId).ConfigureAwait(true);

            if (avatar == null)
            {
                return;
            }

            EstablecerAvatar(avatar);
        }

        private async Task GuardarCambiosAsync()
        {
            LimpiarEstadosValidacion();

            ResultadoValidacionFormulario resultadoValidacion = ValidarDatosFormulario();
            if (!resultadoValidacion.EsValido)
            {
                MostrarErroresValidacion(resultadoValidacion.CamposInvalidos);
                return;
            }

            DTOs.ActualizacionPerfilDTO solicitud = CrearSolicitudActualizacion();
            EstaProcesando = true;

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                DTOs.ResultadoOperacionDTO resultado = 
                    await EnviarActualizacionAsync(solicitud);
                
                ProcesarResultadoActualizacion(resultado);
            });

            EstaProcesando = false;
        }

        private void LimpiarEstadosValidacion()
        {
            MostrarCamposInvalidos?.Invoke(Array.Empty<string>());
            _redesSocialesManejador.LimpiarErrores();
        }

        private ResultadoValidacionFormulario ValidarDatosFormulario()
        {
            var (sonCamposValidos, _, invalidos) = 
                ValidarCamposPrincipales();
            var (sonRedesValidas, _) = _redesSocialesManejador.ValidarRedesSociales();

            List<string> camposInvalidos = invalidos ?? new List<string>();
            
            if (!sonRedesValidas)
            {
                camposInvalidos.Add("RedesSociales");
            }

            bool esValido = sonCamposValidos && sonRedesValidas;
            return new ResultadoValidacionFormulario(esValido, camposInvalidos);
        }

        /// <summary>
        /// Representa el resultado de la validacion de un formulario.
        /// </summary>
        private sealed class ResultadoValidacionFormulario
        {
            public bool EsValido { get; }
            public List<string> CamposInvalidos { get; }

            public ResultadoValidacionFormulario(bool esValido, List<string> camposInvalidos)
            {
                EsValido = esValido;
                CamposInvalidos = camposInvalidos ?? new List<string>();
            }
        }

        private void MostrarErroresValidacion(List<string> camposInvalidos)
        {
            _sonidoManejador.ReproducirError();
            MostrarCamposInvalidos?.Invoke(camposInvalidos);

            string mensaje = ObtenerMensajeError(camposInvalidos);
            _avisoServicio.Mostrar(mensaje);
        }

        private string ObtenerMensajeError(List<string> camposInvalidos)
        {
            if (camposInvalidos == null || camposInvalidos.Count == 0)
            {
                return Lang.errorTextoCamposInvalidosGenerico;
            }

            if (camposInvalidos.Count == 1)
            {
                return ObtenerMensajeErrorCampo(camposInvalidos[0]);
            }

            return Lang.errorTextoCamposInvalidosGenerico;
        }

        private string ObtenerMensajeErrorCampo(string campo)
        {
            if (campo == nameof(Nombre))
            {
                return ObtenerMensajeErrorNombre();
            }
            
            if (campo == nameof(Apellido))
            {
                return ObtenerMensajeErrorApellido();
            }

            return Lang.errorTextoCamposInvalidosGenerico;
        }

        private string ObtenerMensajeErrorNombre()
        {
            var resultado = ValidadorEntrada.ValidarNombre(Nombre?.Trim());
            return resultado?.Mensaje ?? Lang.errorTextoCamposInvalidosGenerico;
        }

        private string ObtenerMensajeErrorApellido()
        {
            var resultado = ValidadorEntrada.ValidarApellido(Apellido?.Trim());
            return resultado?.Mensaje ?? Lang.errorTextoCamposInvalidosGenerico;
        }

        private DTOs.ActualizacionPerfilDTO CrearSolicitudActualizacion()
        {
            return new DTOs.ActualizacionPerfilDTO
            {
                UsuarioId = _usuarioId,
                Nombre = Nombre.Trim(),
                Apellido = Apellido.Trim(),
                AvatarId = AvatarSeleccionadoId,
                Instagram = _redesSocialesManejador.Instagram,
                Facebook = _redesSocialesManejador.Facebook,
                X = _redesSocialesManejador.X,
                Discord = _redesSocialesManejador.Discord
            };
        }

        private async Task<DTOs.ResultadoOperacionDTO> 
            EnviarActualizacionAsync(DTOs.ActualizacionPerfilDTO solicitud)
        {
            _logger.InfoFormat("Guardando cambios de perfil para usuario ID: {0}",
                _usuarioId);
            
            return await _perfilServicio
                .ActualizarPerfilAsync(solicitud)
                .ConfigureAwait(true);
        }

        private void ProcesarResultadoActualizacion(
            DTOs.ResultadoOperacionDTO resultado)
        {
            if (resultado == null)
            {
                NotificarResultadoActualizacionNulo();
                return;
            }

            string mensajeResultado = ObtenerMensajeActualizacion(resultado);
            _avisoServicio.Mostrar(mensajeResultado);

            if (resultado.OperacionExitosa)
            {
                CompletarActualizacionExitosa();
            }
            else
            {
                RegistrarErrorActualizacion(resultado.Mensaje);
            }
        }

        private void NotificarResultadoActualizacionNulo()
        {
            _logger.Error(
                "El servicio de actualizacion de perfil devolvio null.");
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(Lang.errorTextoServidorActualizarPerfil);
        }

        private string ObtenerMensajeActualizacion(
            DTOs.ResultadoOperacionDTO resultado)
        {
            string mensajePorDefecto = resultado.OperacionExitosa
                ? Lang.avisoTextoPerfilActualizado
                : Lang.errorTextoActualizarPerfil;

            return _localizador.Localizar(resultado.Mensaje, mensajePorDefecto);
        }

        private void CompletarActualizacionExitosa()
        {
            _sonidoManejador.ReproducirNotificacion();
            ActualizarSesion();
        }

        private static void RegistrarErrorActualizacion(string mensaje)
        {
            _logger.WarnFormat("Error al guardar perfil: {0}", mensaje);
        }

        private (bool EsValido, string MensajeError, List<string> CamposInvalidos)
            ValidarCamposPrincipales()
        {
            var camposInvalidos = new List<string>();
            string primerError = null;

            ValidarCampo(
                ValidadorEntrada.ValidarNombre(Nombre?.Trim()),
                nameof(Nombre),
                camposInvalidos,
                ref primerError);

            ValidarCampo(
                ValidadorEntrada.ValidarApellido(Apellido?.Trim()),
                nameof(Apellido),
                camposInvalidos,
                ref primerError);

            if (AvatarSeleccionadoId <= 0)
            {
                camposInvalidos.Add("Avatar");
                primerError ??= Lang.errorTextoSeleccionAvatarValido;
            }

            return (camposInvalidos.Count == 0, primerError, camposInvalidos);
        }

        private static void ValidarCampo(
            DTOs.ResultadoOperacionDTO resultado,
            string nombreCampo,
            List<string> invalidos,
            ref string primerError)
        {
            if (resultado?.OperacionExitosa != true)
            {
                invalidos.Add(nombreCampo);
                primerError ??= resultado?.Mensaje;
            }
        }

        private async Task CambiarContrasenaAsync()
        {
            if (!ValidarCorreoParaCambioContrasena())
            {
                return;
            }

            EstaProcesando = true;
            EstaCambiandoContrasena = true;

            await EjecutarOperacionConDesconexionAsync(async () =>
            {
                _logger.InfoFormat(
                    "Iniciando solicitud de cambio de contrasena para: {0}",
                    Correo);
                
                DTOs.ResultadoOperacionDTO resultado = 
                    await SolicitarCambioContrasenaAsync();

                ProcesarResultadoCambioContrasena(resultado);
            });

            EstaCambiandoContrasena = false;
            EstaProcesando = false;
        }

        private bool ValidarCorreoParaCambioContrasena()
        {
            if (!string.IsNullOrWhiteSpace(Correo))
            {
                return true;
            }

            NotificarCorreoInvalidoParaCambio();
            return false;
        }

        private void NotificarCorreoInvalidoParaCambio()
        {
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(Lang.errorTextoIniciarCambioContrasena);
        }

        private async Task<DTOs.ResultadoOperacionDTO> 
            SolicitarCambioContrasenaAsync()
        {
            return await _recuperacionCuentaDialogoServicio
                .RecuperarCuentaAsync(
                    Correo,
                    _cambioContrasenaServicio)
                .ConfigureAwait(true);
        }

        private void ProcesarResultadoCambioContrasena(
            DTOs.ResultadoOperacionDTO resultado)
        {
            if (EsCambioContrasenaFallido(resultado))
            {
                NotificarErrorCambioContrasena(resultado.Mensaje);
                return;
            }

            if (resultado?.OperacionExitosa == true)
            {
                CompletarCambioContrasenaExitoso();
            }
        }

        private static bool EsCambioContrasenaFallido(
            DTOs.ResultadoOperacionDTO resultado)
        {
            return resultado?.OperacionExitosa == false &&
                   !string.IsNullOrWhiteSpace(resultado.Mensaje);
        }

        private void NotificarErrorCambioContrasena(string mensaje)
        {
            _logger.WarnFormat("Error en cambio de contrasena: {0}", mensaje);
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(mensaje);
        }

        private void CompletarCambioContrasenaExitoso()
        {
            _sonidoManejador.ReproducirNotificacion();
            FinalizarSesionPorCambioContrasena();
        }

        private void FinalizarSesionPorCambioContrasena()
        {
            _avisoServicio.Mostrar(Lang.avisoTextoReinicioSesion);
            _usuarioSesion.Limpiar();
            RequiereReinicioSesion = true;
            SolicitarReinicioSesion?.Invoke();
            _ventana.CerrarVentana(this);
        }

        private void AplicarPerfil(DTOs.UsuarioDTO perfil)
        {
            _usuarioId = perfil.UsuarioId;
            Usuario = perfil.NombreUsuario;
            Correo = perfil.Correo;
            Nombre = perfil.Nombre;
            Apellido = perfil.Apellido;

            EstablecerAvatarPorId(perfil.AvatarId);

            var redesSociales = new RedesSocialesDTO(
                perfil.Instagram,
                perfil.Facebook,
                perfil.X,
                perfil.Discord);
            _redesSocialesManejador.CargarDesdeDTO(redesSociales);

            ActualizarSesion(perfil);
        }

        private void EstablecerAvatarPorId(int avatarId)
        {
            var avatares = _catalogoAvatares.ObtenerAvatares();

            ResultadoOperacion<ObjetoAvatar> resultadoAvatar = 
                _catalogoAvatares.ObtenerPorId(avatarId);
            if (resultadoAvatar.Exitoso)
            {
                EstablecerAvatar(resultadoAvatar.Valor);
                return;
            }

            if (avatares != null && avatares.Count > 0)
            {
                EstablecerAvatar(avatares[0]);
            }
        }
        private void EstablecerAvatar(ObjetoAvatar avatar)
        {
            if (avatar == null)
            {
                return;
            }

            AvatarSeleccionadoNombre = avatar.Nombre;
            AvatarSeleccionadoId = avatar.Id;
            AvatarSeleccionadoImagen = avatar.Imagen;
        }

        private void ActualizarSesion()
        {
            if (_usuarioSesion == null || _usuarioSesion.IdUsuario <= 0)
            {
                return;
            }

            var dto = new DTOs.UsuarioDTO
            {
                UsuarioId = _usuarioId,
                JugadorId = _usuarioSesion.JugadorId,
                NombreUsuario = Usuario,
                Nombre = Nombre?.Trim(),
                Apellido = Apellido?.Trim(),
                Correo = Correo,
                AvatarId = AvatarSeleccionadoId,
                Instagram = _redesSocialesManejador.Instagram,
                Facebook = _redesSocialesManejador.Facebook,
                X = _redesSocialesManejador.X,
                Discord = _redesSocialesManejador.Discord
            };
            _usuarioSesion.CargarDesdeDTO(dto);
        }
        private void ActualizarSesion(DTOs.UsuarioDTO perfil)
        {
            if (perfil == null)
            {
                return;
            }

            _usuarioSesion.CargarDesdeDTO(perfil);
        }
    }
}