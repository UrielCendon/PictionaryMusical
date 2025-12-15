using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace PictionaryMusicalCliente.VistaModelo.Amigos
{
    /// <summary>
    /// Controla la logica para invitar amigos conectados a una sala de juego.
    /// </summary>
    public class InvitarAmigosVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IInvitacionesServicio _invitacionesServicio;
        private readonly IPerfilServicio _perfilServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _avisoServicio;
        private readonly string _codigoSala;
        private readonly Action<int> _registrarAmigoInvitado;

        public InvitarAmigosVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            IEnumerable<DTOs.AmigoDTO> amigos,
            IInvitacionesServicio invitacionesServicio,
            IPerfilServicio perfilServicio,
            SonidoManejador sonidoManejador,
            IAvisoServicio avisoServicio,
            string codigoSala,
            Func<int, bool> amigoInvitado,
            Action<int> registrarAmigoInvitado)
            : base(ventana, localizador)
        {
            _invitacionesServicio = invitacionesServicio ??
                throw new ArgumentNullException(nameof(invitacionesServicio));
            _perfilServicio = perfilServicio ??
                throw new ArgumentNullException(nameof(perfilServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
            _avisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));

            if (string.IsNullOrWhiteSpace(codigoSala))
            {
                throw new ArgumentException(
                    "El codigo de la sala es obligatorio.",
                    nameof(codigoSala));
            }

            _codigoSala = codigoSala;
            _registrarAmigoInvitado = registrarAmigoInvitado;

            Amigos = new ObservableCollection<AmigoInvitacionItemVistaModelo>(
                CrearElementos(amigos, amigoInvitado));
        }

        /// <summary>
        /// Coleccion de amigos disponibles para invitar.
        /// </summary>
        public ObservableCollection<AmigoInvitacionItemVistaModelo> Amigos { get; }

        internal async Task InvitarAsync(AmigoInvitacionItemVistaModelo amigo)
        {
            ResultadoValidacionAmigo validacion = ValidarAmigo(amigo);
            
            if (!validacion.EsValido)
            {
                MostrarErrorValidacion(validacion);
                return;
            }

            amigo.EstaProcesando = true;

            await EjecutarOperacionAsync(async () =>
            {
                DTOs.UsuarioDTO perfil = await ObtenerPerfilAmigoAsync(
                    amigo.UsuarioId);
                
                if (!ValidarPerfil(perfil, amigo.UsuarioId))
                {
                    return;
                }

                await EnviarInvitacionPorCorreoAsync(perfil.Correo, amigo);
            },
            excepcion =>
            {
                _logger.Error("Error al enviar invitacion.", excepcion);
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(excepcion.Message ?? Lang.errorTextoEnviarCorreo);
                amigo.EstaProcesando = false;
            });

            amigo.EstaProcesando = false;
        }

        private static ResultadoValidacionAmigo ValidarAmigo(
            AmigoInvitacionItemVistaModelo amigo)
        {
            if (amigo == null)
            {
                return ResultadoValidacionAmigo.Invalido(null);
            }

            if (amigo.InvitacionEnviada)
            {
                return ResultadoValidacionAmigo.Invalido(
                    Lang.invitarAmigosTextoYaInvitado);
            }

            if (amigo.UsuarioId <= 0)
            {
                return ResultadoValidacionAmigo.Invalido(
                    Lang.errorTextoErrorProcesarSolicitud);
            }

            return ResultadoValidacionAmigo.Valido();
        }

        private void MostrarErrorValidacion(ResultadoValidacionAmigo validacion)
        {
            if (string.IsNullOrWhiteSpace(validacion.MensajeError))
            {
                return;
            }

            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(validacion.MensajeError);
        }

        private async Task<DTOs.UsuarioDTO> ObtenerPerfilAmigoAsync(int usuarioId)
        {
            _logger.InfoFormat("Obteniendo perfil para invitar amigo ID: {0}",
                usuarioId);
            
            return await _perfilServicio
                .ObtenerPerfilAsync(usuarioId)
                .ConfigureAwait(true);
        }

        private bool ValidarPerfil(DTOs.UsuarioDTO perfil, int usuarioId)
        {
            if (perfil == null || string.IsNullOrWhiteSpace(perfil.Correo))
            {
                _logger.WarnFormat(
                    "Perfil o correo no disponible para amigo ID: {0}",
                    usuarioId);
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(Lang.invitarAmigosTextoCorreoNoDisponible);
                return false;
            }

            return true;
        }

        private async Task EnviarInvitacionPorCorreoAsync(
            string correo,
            AmigoInvitacionItemVistaModelo amigo)
        {
            _logger.InfoFormat("Enviando invitacion por correo a: {0}", correo);
            
            DTOs.ResultadoOperacionDTO resultado = await _invitacionesServicio
                .EnviarInvitacionAsync(_codigoSala, correo)
                .ConfigureAwait(true);

            ProcesarResultadoInvitacion(resultado, amigo);
        }

        private void ProcesarResultadoInvitacion(
            DTOs.ResultadoOperacionDTO resultado,
            AmigoInvitacionItemVistaModelo amigo)
        {
            if (resultado != null && resultado.OperacionExitosa)
            {
                _sonidoManejador.ReproducirNotificacion();
                amigo.MarcarInvitacionEnviada();
                _registrarAmigoInvitado?.Invoke(amigo.UsuarioId);
                _avisoServicio.Mostrar(Lang.invitarCorreoTextoEnviado);
            }
            else
            {
                _logger.WarnFormat("Fallo al enviar invitacion: {0}",
                    resultado?.Mensaje);
                _sonidoManejador.ReproducirError();
                string mensaje = _localizador.Localizar(
                    resultado?.Mensaje,
                    Lang.errorTextoEnviarCorreo);
                _avisoServicio.Mostrar(mensaje);
            }
        }

        private IEnumerable<AmigoInvitacionItemVistaModelo> CrearElementos(
            IEnumerable<DTOs.AmigoDTO> amigos,
            Func<int, bool> amigoInvitado)
        {
            if (amigos == null)
            {
                return Array.Empty<AmigoInvitacionItemVistaModelo>();
            }

            var invitados = new HashSet<int>();
            var elementos = new List<AmigoInvitacionItemVistaModelo>();

            foreach (DTOs.AmigoDTO amigo in amigos.Where(
                a => a != null && a.UsuarioId > 0))
            {
                if (!invitados.Add(amigo.UsuarioId))
                {
                    continue;
                }

                bool yaInvitado = amigoInvitado?.Invoke(amigo.UsuarioId) 
                    ?? false;
                elementos.Add(new AmigoInvitacionItemVistaModelo(
                    amigo, 
                    this, 
                    _sonidoManejador, 
                    yaInvitado));
            }

            return elementos;
        }
    }

    /// <summary>
    /// Representa un item individual en la lista de amigos para invitar.
    /// </summary>
    public class AmigoInvitacionItemVistaModelo : INotifyPropertyChanged
    {
        private readonly InvitarAmigosVistaModelo _padre;
        private readonly SonidoManejador _sonidoManejador;
        private bool _invitacionEnviada;
        private bool _estaProcesando;

        public event PropertyChangedEventHandler PropertyChanged;

        public AmigoInvitacionItemVistaModelo(
            DTOs.AmigoDTO amigo,
            InvitarAmigosVistaModelo padre,
            SonidoManejador sonidoManejador,
            bool invitacionEnviada)
        {
            if (amigo == null)
            {
                throw new ArgumentNullException(nameof(amigo));
            }

            _padre = padre ?? throw new ArgumentNullException(nameof(padre));
            UsuarioId = amigo.UsuarioId;
            NombreUsuario = amigo.NombreUsuario ?? string.Empty;
            _invitacionEnviada = invitacionEnviada;
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));

            InvitarComando = new ComandoAsincrono(
                async () => await EjecutarInvitarAsync(),
                () => !EstaProcesando);
        }

        /// <summary>
        /// Identificador unico del usuario amigo.
        /// </summary>
        public int UsuarioId { get; }

        /// <summary>
        /// Nombre de usuario del amigo.
        /// </summary>
        public string NombreUsuario { get; }

        /// <summary>
        /// Indica si ya se envio una invitacion a este amigo.
        /// </summary>
        public bool InvitacionEnviada
        {
            get => _invitacionEnviada;
            private set
            {
                if (_invitacionEnviada != value)
                {
                    _invitacionEnviada = value;
                    NotificarCambioPropiedad(nameof(InvitacionEnviada));
                    NotificarCambioPropiedad(nameof(TextoBoton));
                    InvitarComando.NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Indica si hay una operacion en curso para este amigo.
        /// </summary>
        public bool EstaProcesando
        {
            get => _estaProcesando;
            set
            {
                if (_estaProcesando != value)
                {
                    _estaProcesando = value;
                    NotificarCambioPropiedad(nameof(EstaProcesando));
                    InvitarComando.NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Texto a mostrar en el boton de invitar.
        /// </summary>
        public string TextoBoton => InvitacionEnviada
            ? Lang.invitarAmigosTextoInvitado
            : Lang.globalTextoInvitar;

        /// <summary>
        /// Comando para enviar la invitacion al amigo.
        /// </summary>
        public IComandoAsincrono InvitarComando { get; }

        private async Task EjecutarInvitarAsync()
        {
            _sonidoManejador.ReproducirClick();
            await _padre.InvitarAsync(this).ConfigureAwait(true);
        }

        private void NotificarCambioPropiedad(string nombrePropiedad)
        {
            PropertyChanged?.Invoke(
                this, 
                new PropertyChangedEventArgs(nombrePropiedad));
        }

        internal void MarcarInvitacionEnviada()
        {
            InvitacionEnviada = true;
        }
    }

    /// <summary>
    /// Resultado de la validacion de un amigo para invitar.
    /// </summary>
    internal sealed class ResultadoValidacionAmigo
    {
        private ResultadoValidacionAmigo(bool esValido, string mensajeError)
        {
            EsValido = esValido;
            MensajeError = mensajeError;
        }

        /// <summary>
        /// Indica si la validacion fue exitosa.
        /// </summary>
        public bool EsValido { get; }

        /// <summary>
        /// Mensaje de error si la validacion fallo.
        /// </summary>
        public string MensajeError { get; }

        /// <summary>
        /// Crea un resultado de validacion exitosa.
        /// </summary>
        public static ResultadoValidacionAmigo Valido()
        {
            return new ResultadoValidacionAmigo(true, null);
        }

        /// <summary>
        /// Crea un resultado de validacion fallida.
        /// </summary>
        /// <param name="mensajeError">Mensaje de error a mostrar.</param>
        public static ResultadoValidacionAmigo Invalido(string mensajeError)
        {
            return new ResultadoValidacionAmigo(false, mensajeError);
        }
    }
}