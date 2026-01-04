using System;
using System.IO;
using System.Windows.Media;
using log4net;

namespace PictionaryMusicalCliente.Utilidades
{
    /// <summary>
    /// Maneja la reproduccion de canciones especificas del juego.
    /// </summary>
    public class CancionManejador : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const double VolumenPredeterminado = 0.5;
        private const double ToleranciaComparacionVolumen = 0.0001;

        private readonly MediaPlayer _reproductor;
        private bool _desechado;

        /// <summary>
        /// Inicializa una nueva instancia del manejador de canciones.
        /// </summary>
        public CancionManejador()
        {
            _reproductor = new MediaPlayer();
            SuscribirEventoFinalizacion();
            _reproductor.Volume = ObtenerVolumenGuardado();
        }

        private void SuscribirEventoFinalizacion()
        {
            _reproductor.MediaEnded += ManejarFinalizacionReproduccion;
        }

        private void ManejarFinalizacionReproduccion(object remitente, EventArgs argumentosEvento)
        {
            EstaReproduciendo = false;
        }

        /// <summary>
        /// Obtiene o establece el volumen de la reproduccion (0.0 a 1.0).
        /// </summary>
        public double Volumen
        {
            get => _reproductor.Volume;
            set
            {
                double ajustado = Math.Max(0, Math.Min(1, value));
                if (Math.Abs(_reproductor.Volume - ajustado) < ToleranciaComparacionVolumen)
                {
                    return;
                }

                _reproductor.Volume = ajustado;
                GuardarPreferencia(ajustado);
            }
        }

        /// <summary>
        /// Indica si actualmente se esta reproduciendo una cancion.
        /// </summary>
        public bool EstaReproduciendo { get; private set; }

        /// <summary>
        /// Reproduce una cancion ubicada en la carpeta 'Recursos'.
        /// </summary>
        /// <param name="nombreArchivo">Nombre del archivo con extension.</param>
        public void Reproducir(string nombreArchivo)
        {
            if (!EsNombreArchivoValido(nombreArchivo))
            {
                return;
            }

            EjecutarReproduccion(nombreArchivo);
        }

        private static bool EsNombreArchivoValido(string nombreArchivo)
        {
            if (string.IsNullOrWhiteSpace(nombreArchivo))
            {
                _logger.Warn("Intento de reproducir archivo con nombre vacio.");
                return false;
            }

            return true;
        }

        private void EjecutarReproduccion(string nombreArchivo)
        {
            try
            {
                ReproducirSiExiste(nombreArchivo);
            }
            catch (UriFormatException excepcion)
            {
                RegistrarErrorUri(nombreArchivo, excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                RegistrarErrorOperacion(nombreArchivo, excepcion);
            }
        }

        private void ReproducirSiExiste(string nombreArchivo)
        {
            string ruta = ConstruirRutaArchivo(nombreArchivo);

            if (File.Exists(ruta))
            {
                IniciarReproduccion(ruta);
            }
            else
            {
                RegistrarArchivoNoEncontrado(ruta);
            }
        }

        private static void RegistrarArchivoNoEncontrado(string ruta)
        {
            _logger.ErrorFormat("Archivo de audio no encontrado: {0}", ruta);
        }

        private static void RegistrarErrorUri(string nombre, UriFormatException excepcion)
        {
            _logger.ErrorFormat("URI invalido para cancion: {0}. Detalle: {1}", nombre, excepcion);
        }

        private static void RegistrarErrorOperacion(
            string nombre,
            InvalidOperationException excepcion)
        {
            _logger.ErrorFormat(
                "Error de operacion en reproductor para: {0}. Detalle: {1}",
                nombre,
                excepcion);
        }

        /// <summary>
        /// Detiene la reproduccion actual.
        /// </summary>
        public void Detener()
        {
            try
            {
                _reproductor.Stop();
                EstaReproduciendo = false;
            }
            catch (InvalidOperationException excepcion)
            {
                _logger.Error("Error al intentar detener la reproduccion.", excepcion);
            }
        }

        /// <inheritdoc />
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
                LiberarReproductor();
            }

            _desechado = true;
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
                    "El reproductor no estaba en estado valido para cerrarse.",
                    excepcion);
            }
        }

        private static string ConstruirRutaArchivo(string nombreArchivo)
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Recursos",
                "Canciones",
                nombreArchivo);
        }

        private void IniciarReproduccion(string ruta)
        {
            _reproductor.Open(new Uri(ruta, UriKind.Absolute));
            _reproductor.Play();
            EstaReproduciendo = true;
        }

        private static double ObtenerVolumenGuardado()
        {
            double valor = Properties.Settings.Default.volumenCancion;
            if (double.IsNaN(valor) || double.IsInfinity(valor))
            {
                return VolumenPredeterminado;
            }
            return Math.Max(0, Math.Min(1, valor));
        }

        private static void GuardarPreferencia(double volumen)
        {
            Properties.Settings.Default.volumenCancion = volumen;
            Properties.Settings.Default.Save();
        }
    }
}