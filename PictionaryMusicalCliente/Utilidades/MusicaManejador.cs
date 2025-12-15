using System;
using System.IO;
using System.Windows.Media;
using log4net;

namespace PictionaryMusicalCliente.Utilidades
{
    /// <summary>
    /// Controla la reproduccion de musica de fondo en la aplicacion.
    /// </summary>
    public class MusicaManejador : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private const double VolumenPredeterminado = 0.5;

        private readonly MediaPlayer _reproductor;
        private bool _desechado;
        private bool _estaReproduciendo;

        /// <summary>
        /// Inicializa una nueva instancia del manejador de musica.
        /// </summary>
        public MusicaManejador()
        {
            _reproductor = new MediaPlayer();
            SuscribirEventos();
            InicializarVolumen();
        }

        private void SuscribirEventos()
        {
            _reproductor.MediaEnded += EnMedioTerminado;
            _reproductor.MediaFailed += EnMedioFallido;
        }

        private void InicializarVolumen()
        {
            double volumenGuardado = ObtenerVolumenGuardado();
            _reproductor.Volume = NormalizarVolumen(volumenGuardado);
        }

        private static double ObtenerVolumenGuardado()
        {
            double valor = Properties.Settings.Default.volumenMusica;

            if (double.IsNaN(valor) || double.IsInfinity(valor))
            {
                return VolumenPredeterminado;
            }

            return valor;
        }

        private static double NormalizarVolumen(double volumen)
        {
            return Math.Max(0, Math.Min(1, volumen));
        }

        /// <summary>
        /// Obtiene o establece el volumen de la musica (0.0 a 1.0).
        /// </summary>
        public double Volumen
        {
            get => _reproductor.Volume;
            set
            {
                if (_desechado)
                {
                    return;
                }

                double valorSeguro = NormalizarVolumen(value);
                _reproductor.Volume = valorSeguro;
                GuardarPreferenciaVolumen(valorSeguro);
            }
        }

        private static void GuardarPreferenciaVolumen(double volumen)
        {
            Properties.Settings.Default.volumenMusica = volumen;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Inicia la reproduccion en bucle del archivo especificado.
        /// </summary>
        /// <param name="nombreArchivo">Nombre del archivo de audio a reproducir.</param>
        public void ReproducirEnBucle(string nombreArchivo)
        {
            if (!PuedeReproducir(nombreArchivo))
            {
                return;
            }

            EjecutarReproduccionEnBucle(nombreArchivo);
        }

        private bool PuedeReproducir(string nombreArchivo)
        {
            return !_desechado && !string.IsNullOrWhiteSpace(nombreArchivo);
        }

        private void EjecutarReproduccionEnBucle(string nombreArchivo)
        {
            try
            {
                IniciarReproduccion(nombreArchivo);
            }
            catch (UriFormatException excepcion)
            {
                RegistrarErrorUri(nombreArchivo, excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                RegistrarErrorEstadoInvalido(nombreArchivo, excepcion);
            }
        }

        private void IniciarReproduccion(string nombreArchivo)
        {
            string ruta = ConstruirRutaArchivo(nombreArchivo);
            _reproductor.Open(new Uri(ruta, UriKind.Absolute));
            _reproductor.Play();
            _estaReproduciendo = true;
        }

        private static string ConstruirRutaArchivo(string nombreArchivo)
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Recursos",
                nombreArchivo);
        }

        private static void RegistrarErrorUri(
            string nombreArchivo,
            UriFormatException excepcion)
        {
            _logger.ErrorFormat(
                "URI de musica invalida ({0}): {1}",
                nombreArchivo,
                excepcion);
        }

        private static void RegistrarErrorEstadoInvalido(
            string nombreArchivo,
            InvalidOperationException excepcion)
        {
            _logger.WarnFormat(
                "No se pudo iniciar la musica ({0}) por estado invalido: {1}",
                nombreArchivo,
                excepcion);
        }

        /// <summary>
        /// Detiene la reproduccion actual.
        /// </summary>
        public void Detener()
        {
            if (!PuedeDetener())
            {
                return;
            }

            EjecutarDetencion();
        }

        private bool PuedeDetener()
        {
            return !_desechado && _estaReproduciendo;
        }

        private void EjecutarDetencion()
        {
            try
            {
                _reproductor.Stop();
                _estaReproduciendo = false;
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn("Error al detener el reproductor de musica.", excepcion);
            }
        }
        
        /// <summary>
        /// Alterna entre silenciar y restaurar el volumen.
        /// </summary>
        /// <returns>True si quedo silenciado, False si tiene volumen.</returns>
        public bool AlternarSilencio()
        {
            bool estaSilenciado = _reproductor.IsMuted;
            _reproductor.IsMuted = !estaSilenciado;
            return !estaSilenciado;
        }

        private void EnMedioTerminado(object remitente, EventArgs argumentosEvento)
        {
            if (!_desechado && _estaReproduciendo)
            {
                try
                {
                    _reproductor.Position = TimeSpan.Zero;
                    _reproductor.Play();
                }
                catch (InvalidOperationException excepcion)
                {
                    _logger.Warn("Error al reiniciar el bucle de musica.", excepcion);
                }
            }
        }

        private void EnMedioFallido(object remitente, ExceptionEventArgs argumentosEvento)
        {
            RegistrarFalloCritico(argumentosEvento.ErrorException);
            _estaReproduciendo = false;
        }

        private static void RegistrarFalloCritico(Exception excepcion)
        {
            _logger.ErrorFormat("Fallo critico en MediaPlayer de musica: {0}", excepcion);
        }

        /// <summary>
        /// Libera los recursos del reproductor.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos del reproductor.
        /// </summary>
        /// <param name="disposing">
        /// True si se invoca desde Dispose(), false si es desde el finalizador.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_desechado)
            {
                return;
            }

            if (disposing)
            {
                DesuscribirEventos();
                LiberarReproductor();
            }

            _desechado = true;
        }

        private void DesuscribirEventos()
        {
            _reproductor.MediaEnded -= EnMedioTerminado;
            _reproductor.MediaFailed -= EnMedioFallido;
        }

        private void LiberarReproductor()
        {
            try
            {
                _reproductor.Stop();
                _reproductor.Close();
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Warn(
                    "El reproductor de musica no estaba en un estado valido para cerrarse.",
                    excepcion);
            }
        }
    }
}