using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Comandos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using log4net;
using DTOs = PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalCliente.Utilidades.Abstracciones;

namespace PictionaryMusicalCliente.VistaModelo.Perfil
{
    /// <summary>
    /// Administra la logica para el cambio de contrasena mediante un token de recuperacion.
    /// </summary>
    /// <remarks>
    /// Requiere un token de verificacion previamente validado para poder
    /// actualizar la contrasena del usuario.
    /// </remarks>
    public class CambioContrasenaVistaModelo : BaseVistaModelo
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _tokenCodigo;
        private readonly ICambioContrasenaServicio _cambioContrasenaServicio;
        private readonly SonidoManejador _sonidoManejador;
        private readonly IAvisoServicio _avisoServicio;

        private string _nuevaContrasena;
        private string _confirmacionContrasena;
        private bool _estaProcesando;

        /// <summary>
        /// Inicializa una nueva instancia de la clase.
        /// </summary>
        /// <param name="ventana">Servicio para gestionar ventanas.</param>
        /// <param name="localizador">Servicio de localizacion.</param>
        /// <param name="tokenCodigo">
        /// Codigo de verificacion validado previamente.
        /// </param>
        /// <param name="cambioContrasenaServicio">
        /// Servicio para ejecutar la actualizacion.
        /// </param>
        /// <param name="avisoServicio">Servicio de avisos.</param>
        /// <param name="sonidoManejador">Manejador de sonidos.</param>
        /// <exception cref="ArgumentNullException">
        /// Si algun parametro requerido es nulo.
        /// </exception>
        public CambioContrasenaVistaModelo(
            IVentanaServicio ventana,
            ILocalizadorServicio localizador,
            string tokenCodigo,
            ICambioContrasenaServicio cambioContrasenaServicio,
            IAvisoServicio avisoServicio,
            SonidoManejador sonidoManejador)
            : base(ventana, localizador)
        {
            _tokenCodigo = tokenCodigo ?? 
                throw new ArgumentNullException(nameof(tokenCodigo));
            _cambioContrasenaServicio = cambioContrasenaServicio ??
                throw new ArgumentNullException(nameof(cambioContrasenaServicio));
            _avisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));

            ConfirmarComando = new ComandoAsincrono(
                EjecutarComandoConfirmarAsync, 
                ValidarPuedeConfirmar);

            CancelarComando = new ComandoDelegado(EjecutarComandoCancelar);
        }

        private async Task EjecutarComandoConfirmarAsync(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            await ConfirmarAsync();
        }

        private bool ValidarPuedeConfirmar(object parametro)
        {
            return !EstaProcesando;
        }

        private void EjecutarComandoCancelar(object parametro)
        {
            _sonidoManejador.ReproducirClick();
            Cancelar();
        }

        /// <summary>
        /// La nueva contrasena ingresada por el usuario.
        /// </summary>
        public string NuevaContrasena
        {
            get => _nuevaContrasena;
            set => EstablecerPropiedad(ref _nuevaContrasena, value);
        }

        /// <summary>
        /// La confirmacion de la contrasena para asegurar que el usuario no cometio errores.
        /// </summary>
        public string ConfirmacionContrasena
        {
            get => _confirmacionContrasena;
            set => EstablecerPropiedad(ref _confirmacionContrasena, value);
        }

        /// <summary>
        /// Indica si se esta realizando una operacion asincrona para bloquear la interfaz.
        /// </summary>
        public bool EstaProcesando
        {
            get => _estaProcesando;
            private set
            {
                if (EstablecerPropiedad(ref _estaProcesando, value))
                {
                    ((IComandoNotificable)ConfirmarComando).NotificarPuedeEjecutar();
                }
            }
        }

        /// <summary>
        /// Comando para ejecutar la validacion y solicitud de cambio de contrasena.
        /// </summary>
        public IComandoAsincrono ConfirmarComando { get; }

        /// <summary>
        /// Comando para cancelar la operacion y cerrar la vista.
        /// </summary>
        public ICommand CancelarComando { get; }

        /// <summary>
        /// Accion para notificar a la vista que campos especificos son invalidos.
        /// </summary>
        public Action<IList<string>> MostrarCamposInvalidos { get; set; }

        /// <summary>
        /// Accion que notifica el resultado final del proceso de cambio de contrasena.
        /// </summary>
        public Action<DTOs.ResultadoOperacionDTO> CambioContrasenaFinalizada { get; set; }

        private async Task ConfirmarAsync()
        {
            LimpiarErroresVisuales();

            var camposInvalidos = ValidarEntradas();

            if (TieneErroresValidacion(camposInvalidos))
            {
                MostrarErroresValidacion(camposInvalidos);
                return;
            }

            EstaProcesando = true;

            await EjecutarOperacionAsync(
                EjecutarCambioContrasenaAsync,
                ManejarErrorCambioContrasena);

            EstaProcesando = false;
        }

        private void LimpiarErroresVisuales()
        {
            MostrarCamposInvalidos?.Invoke(Array.Empty<string>());
        }

        private static bool TieneErroresValidacion(List<string> camposInvalidos)
        {
            return camposInvalidos != null && camposInvalidos.Count > 0;
        }

        private void MostrarErroresValidacion(List<string> camposInvalidos)
        {
            _logger.Warn("Validacion de contrasena fallida en cliente.");
            _sonidoManejador.ReproducirError();
            MostrarCamposInvalidos?.Invoke(camposInvalidos);
        }

        private async Task EjecutarCambioContrasenaAsync()
        {
            DTOs.ResultadoOperacionDTO resultado = await _cambioContrasenaServicio
                .ActualizarContrasenaAsync(_tokenCodigo, NuevaContrasena)
                .ConfigureAwait(true);

            if (!ValidarResultadoServicio(resultado))
            {
                return;
            }

            ProcesarResultadoCambio(resultado);
        }

        private bool ValidarResultadoServicio(DTOs.ResultadoOperacionDTO resultado)
        {
            if (resultado == null)
            {
                _logger.Error("Servicio de cambio de contrasena devolvio null.");
                _sonidoManejador.ReproducirError();
                _avisoServicio.Mostrar(Lang.errorTextoActualizarContrasena);
                return false;
            }

            return true;
        }

        private void ProcesarResultadoCambio(DTOs.ResultadoOperacionDTO resultado)
        {
            string mensaje = ObtenerMensajeResultado(resultado);

            if (resultado.OperacionExitosa)
            {
                CompletarCambioExitoso(resultado, mensaje);
            }
            else
            {
                ManejarCambioFallido(resultado, mensaje);
            }
        }

        private static string ObtenerMensajeResultado(DTOs.ResultadoOperacionDTO resultado)
        {
            if (!string.IsNullOrWhiteSpace(resultado.Mensaje))
            {
                return resultado.Mensaje;
            }

            return resultado.OperacionExitosa
                ? Lang.avisoTextoContrasenaActualizada
                : Lang.errorTextoActualizarContrasena;
        }

        private void CompletarCambioExitoso(
            DTOs.ResultadoOperacionDTO resultado,
            string mensaje)
        {
            _logger.Info("Contrasena actualizada exitosamente.");
            _sonidoManejador.ReproducirNotificacion();
            _avisoServicio.Mostrar(mensaje);
            CambioContrasenaFinalizada?.Invoke(resultado);
            _ventana.CerrarVentana(this);
        }

        private void ManejarCambioFallido(
            DTOs.ResultadoOperacionDTO resultado,
            string mensaje)
        {
            _logger.WarnFormat(
                "Fallo al actualizar contrasena en servidor: {0}",
                resultado.Mensaje);
            _sonidoManejador.ReproducirError();
            _avisoServicio.Mostrar(mensaje);
        }

        private void ManejarErrorCambioContrasena(Exception excepcion)
        {
            _logger.WarnFormat(
                "Excepcion de servicio al actualizar contrasena: {0}",
                excepcion.Message);
            _sonidoManejador.ReproducirError();
            string mensaje = _localizador.Localizar(
                excepcion.Message,
                Lang.errorTextoActualizarContrasena);
            _avisoServicio.Mostrar(mensaje);
        }

        private List<string> ValidarEntradas()
        {
            var camposInvalidos = ValidarCamposRequeridos();

            if (camposInvalidos.Count > 0)
            {
                _avisoServicio.Mostrar(Lang.errorTextoConfirmacionContrasenaRequerida);
                return camposInvalidos;
            }

            var errorFormato = ValidarFormatoContrasena();

            if (errorFormato.Count > 0)
            {
                return errorFormato;
            }

            var errorCoincidencia = ValidarCoincidenciaContrasenas();

            if (errorCoincidencia.Count > 0)
            {
                return errorCoincidencia;
            }

            return new List<string>();
        }

        private List<string> ValidarCamposRequeridos()
        {
            var camposInvalidos = new List<string>();

            if (string.IsNullOrWhiteSpace(NuevaContrasena))
            {
                camposInvalidos.Add(nameof(NuevaContrasena));
            }

            if (string.IsNullOrWhiteSpace(ConfirmacionContrasena))
            {
                camposInvalidos.Add(nameof(ConfirmacionContrasena));
            }

            return camposInvalidos;
        }

        private List<string> ValidarFormatoContrasena()
        {
            DTOs.ResultadoOperacionDTO validacion = ValidadorEntrada.ValidarContrasena(
                NuevaContrasena);

            if (validacion?.OperacionExitosa != true)
            {
                string mensaje = validacion?.Mensaje ?? Lang.errorTextoContrasenaFormato;
                _avisoServicio.Mostrar(mensaje);
                return new List<string> { nameof(NuevaContrasena) };
            }

            return new List<string>();
        }

        private List<string> ValidarCoincidenciaContrasenas()
        {
            bool coinciden = string.Equals(
                NuevaContrasena,
                ConfirmacionContrasena,
                StringComparison.Ordinal);

            if (!coinciden)
            {
                _avisoServicio.Mostrar(Lang.errorTextoContrasenasNoCoinciden);
                return new List<string>
                {
                    nameof(NuevaContrasena),
                    nameof(ConfirmacionContrasena)
                };
            }

            return new List<string>();
        }

        private void Cancelar()
        {
            _ventana.CerrarVentana(this);
        }
    }
}