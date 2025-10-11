using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Servicios;

namespace PictionaryMusicalCliente
{
    public partial class VerificarCodigo : Window
    {
        private readonly string _tokenVerificacion;
        private readonly string _correoDestino;
        private readonly string _textoOriginalReenviar;
        private readonly DispatcherTimer _temporizador;
        private DateTime _siguienteReenvioPermitido;

        public bool RegistroCompletado { get; private set; }

        public VerificarCodigo(string tokenVerificacion, string correoDestino)
        {
            if (string.IsNullOrWhiteSpace(tokenVerificacion))
            {
                throw new ArgumentException("El token de verificación es obligatorio.", nameof(tokenVerificacion));
            }

            InitializeComponent();

            _tokenVerificacion = tokenVerificacion;
            _correoDestino = correoDestino ?? string.Empty;
            _textoOriginalReenviar = botonReenviarCodigo.Content?.ToString() ?? "Reenviar código";
            _temporizador = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _temporizador.Tick += TemporizadorTick;

            RegistroCompletado = false;
            _siguienteReenvioPermitido = DateTime.UtcNow.AddMinutes(1);
            textoDescripcion.Text = string.IsNullOrWhiteSpace(_correoDestino)
                ? "Ingresa el código de verificación que enviamos a tu correo."
                : $"Ingresa el código de verificación enviado a {_correoDestino}.";

            ActualizarEstadoReenvio();
        }

        private async void BotonVerificarCodigo(object sender, RoutedEventArgs e)
        {
            string codigoIngresado = bloqueTextoCodigoVerificacion.Text?.Trim();

            if (string.IsNullOrWhiteSpace(codigoIngresado))
            {
                new Avisos("Ingrese el código de verificación enviado a su correo.").ShowDialog();
                bloqueTextoCodigoVerificacion.Focus();
                return;
            }

            botonVerificarCodigo.IsEnabled = false;

            try
            {
                ResultadoRegistroCuenta resultado = await ConfirmarCodigoAsync(codigoIngresado);

                if (resultado == null)
                {
                    new Avisos("No se pudo verificar el código. Intente nuevamente.").ShowDialog();
                    return;
                }

                if (resultado.RegistroExitoso)
                {
                    new Avisos(resultado.Mensaje ?? "Registro completado exitosamente.").ShowDialog();
                    RegistroCompletado = true;
                    Close();
                    return;
                }

                string mensajeError = string.IsNullOrWhiteSpace(resultado.Mensaje)
                    ? "El código ingresado no es correcto o ha expirado."
                    : resultado.Mensaje;

                new Avisos(mensajeError).ShowDialog();
            }
            catch (Exception)
            {
                new Avisos("Ocurrió un problema al validar el código. Intente más tarde.").ShowDialog();
            }
            finally
            {
                if (!RegistroCompletado)
                {
                    botonVerificarCodigo.IsEnabled = true;
                }
            }
        }

        private async void BotonReenviarCodigo(object sender, RoutedEventArgs e)
        {
            await SolicitarNuevoCodigoAsync();
        }

        private void BotonCancelarCodigo(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async Task<ResultadoRegistroCuenta> ConfirmarCodigoAsync(string codigo)
        {
            using (var proxy = new ServidorProxy())
            {
                var solicitud = new SolicitudConfirmarCodigo
                {
                    TokenVerificacion = _tokenVerificacion,
                    Codigo = codigo
                };

                return await proxy.ConfirmarCodigoVerificacionAsync(solicitud);
            }
        }

        private async Task SolicitarNuevoCodigoAsync()
        {
            if (!botonReenviarCodigo.IsEnabled)
            {
                return;
            }

            botonReenviarCodigo.IsEnabled = false;

            try
            {
                using (var proxy = new ServidorProxy())
                {
                    var solicitud = new SolicitudReenviarCodigo
                    {
                        TokenVerificacion = _tokenVerificacion
                    };

                    ResultadoSolicitudCodigo resultado = await proxy.ReenviarCodigoVerificacionAsync(solicitud);

                    if (resultado == null)
                    {
                        new Avisos("No se pudo solicitar un nuevo código. Intente nuevamente.").ShowDialog();
                        return;
                    }

                    if (resultado.CodigoEnviado)
                    {
                        new Avisos(resultado.Mensaje ?? "Se envió un nuevo código a su correo electrónico.").ShowDialog();
                        _siguienteReenvioPermitido = DateTime.UtcNow.AddMinutes(1);
                        ActualizarEstadoReenvio();
                        return;
                    }

                    string mensaje = string.IsNullOrWhiteSpace(resultado.Mensaje)
                        ? "No es posible reenviar el código todavía."
                        : resultado.Mensaje;

                    new Avisos(mensaje).ShowDialog();
                }
            }
            catch (Exception)
            {
                new Avisos("Ocurrió un problema al reenviar el código. Intente más tarde.").ShowDialog();
            }
            finally
            {
                ActualizarEstadoReenvio();
            }
        }

        private void TemporizadorTick(object sender, EventArgs e)
        {
            ActualizarEstadoReenvio();
        }

        private void ActualizarEstadoReenvio()
        {
            DateTime ahora = DateTime.UtcNow;
            if (ahora >= _siguienteReenvioPermitido)
            {
                botonReenviarCodigo.IsEnabled = true;
                botonReenviarCodigo.Content = _textoOriginalReenviar;
                if (_temporizador.IsEnabled)
                {
                    _temporizador.Stop();
                }
                return;
            }

            TimeSpan restante = _siguienteReenvioPermitido - ahora;
            int segundos = Math.Max(1, (int)Math.Ceiling(restante.TotalSeconds));
            botonReenviarCodigo.IsEnabled = false;
            botonReenviarCodigo.Content = $"{_textoOriginalReenviar} ({segundos}s)";

            if (!_temporizador.IsEnabled)
            {
                _temporizador.Start();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_temporizador.IsEnabled)
            {
                _temporizador.Stop();
            }

            base.OnClosed(e);
        }
    }
}
