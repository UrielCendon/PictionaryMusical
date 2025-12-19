using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using log4net;
using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Modelo.Catalogos;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.Vista;
using PictionaryMusicalCliente.VistaModelo.Perfil;

namespace PictionaryMusicalCliente.ClienteServicios.Dialogos
{
    /// <summary>
    /// Gestiona el dialogo modal para que el usuario seleccione su avatar.
    /// </summary>
    public class SeleccionAvatarDialogoServicio : ISeleccionarAvatarServicio
    {
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(SeleccionAvatarDialogoServicio));

        private readonly IAvisoServicio _avisoServicio;
        private readonly ICatalogoAvatares _catalogoAvatares;
        private readonly SonidoManejador _sonidoManejador;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de seleccion de avatar.
        /// </summary>
        /// <param name="avisoServicio">Servicio para mostrar avisos al usuario.</param>
        /// <param name="catalogoAvatares">Catalogo que provee los avatares disponibles.</param>
        /// <param name="sonidoManejador">Manejador de efectos de sonido.</param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si alguno de los parametros es nulo.
        /// </exception>
        public SeleccionAvatarDialogoServicio(
            IAvisoServicio avisoServicio,
            ICatalogoAvatares catalogoAvatares,
            SonidoManejador sonidoManejador)
        {
            _avisoServicio = avisoServicio ??
                throw new ArgumentNullException(nameof(avisoServicio));
            _catalogoAvatares = catalogoAvatares ??
                throw new ArgumentNullException(nameof(catalogoAvatares));
            _sonidoManejador = sonidoManejador ??
                throw new ArgumentNullException(nameof(sonidoManejador));
        }

        /// <summary>
        /// Abre la ventana de seleccion y retorna el avatar elegido por el usuario.
        /// </summary>
        /// <param name="idAvatar">
        /// Identificador del avatar actualmente seleccionado para preseleccion.
        /// Use 0 o negativo si no hay preseleccion.
        /// </param>
        /// <returns>
        /// El avatar seleccionado por el usuario, o null si cancelo la seleccion
        /// o no se pudieron cargar los avatares.
        /// </returns>
        public Task<ObjetoAvatar> SeleccionarAvatarAsync(int idAvatar)
        {
            var avatares = ObtenerAvataresLocales();

            if (!HayAvataresDisponibles(avatares))
            {
                RegistrarErrorCargaAvatares();
                NotificarErrorCargaAvatares();
                return Task.FromResult<ObjetoAvatar>(null);
            }

            return EjecutarSeleccionEnDispatcher(avatares, idAvatar);
        }

        private IList<ObjetoAvatar> ObtenerAvataresLocales()
        {
            return (IList<ObjetoAvatar>)_catalogoAvatares.ObtenerAvatares();
        }

        private static bool HayAvataresDisponibles(IList<ObjetoAvatar> avatares)
        {
            return avatares != null && avatares.Count > 0;
        }

        private static void RegistrarErrorCargaAvatares()
        {
            _logger.Warn("No se cargaron avatares locales.");
        }

        private void NotificarErrorCargaAvatares()
        {
            _avisoServicio.Mostrar(Lang.errorTextoNoCargaronAvatares);
        }

        private Task<ObjetoAvatar> EjecutarSeleccionEnDispatcher(
            IList<ObjetoAvatar> avatares,
            int idAvatar)
        {
            var finalizacion = new TaskCompletionSource<ObjetoAvatar>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                EjecutarMostrarVentanaConManejo(avatares, idAvatar, finalizacion);
            });

            return finalizacion.Task;
        }

        private void EjecutarMostrarVentanaConManejo(
            IList<ObjetoAvatar> avatares,
            int idAvatar,
            TaskCompletionSource<ObjetoAvatar> finalizacion)
        {
            try
            {
                MostrarVentanaAvatar(avatares, idAvatar, finalizacion);
            }
            catch (XamlParseException excepcion)
            {
                RegistrarErrorXaml(excepcion);
                EstablecerExcepcionInterfaz(finalizacion, excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                RegistrarErrorOperacionInvalida(excepcion);
                EstablecerExcepcion(finalizacion, excepcion);
            }
        }

        private void MostrarVentanaAvatar(
            IList<ObjetoAvatar> avatares,
            int idAvatarPreseleccionado,
            TaskCompletionSource<ObjetoAvatar> finalizacion)
        {
            var ventana = CrearVentanaSeleccionAvatar();
            var vistaModelo = CrearVistaModelo(avatares);

            ConfigurarPreseleccion(vistaModelo, idAvatarPreseleccionado);
            ConfigurarEventosViewModel(vistaModelo, ventana, finalizacion);
            ConfigurarEventosVentana(ventana, finalizacion);
            MostrarVentana(ventana, vistaModelo);
        }

        private static SeleccionAvatar CrearVentanaSeleccionAvatar()
        {
            return new SeleccionAvatar();
        }

        private SeleccionAvatarVistaModelo CrearVistaModelo(IList<ObjetoAvatar> avatares)
        {
            return new SeleccionAvatarVistaModelo(
                App.VentanaServicio,
                App.Localizador,
                avatares,
                _avisoServicio,
                _sonidoManejador);
        }

        private void ConfigurarPreseleccion(
            SeleccionAvatarVistaModelo vistaModelo,
            int idAvatar)
        {
            ResultadoOperacion<ObjetoAvatar> resultadoAvatar = 
                _catalogoAvatares.ObtenerPorId(idAvatar);
            if (idAvatar > 0 && resultadoAvatar.Exitoso)
            {
                vistaModelo.AvatarSeleccionado = resultadoAvatar.Valor;
            }
        }

        private static void ConfigurarEventosViewModel(
            SeleccionAvatarVistaModelo vistaModelo,
            Window ventana,
            TaskCompletionSource<ObjetoAvatar> finalizacion)
        {
            vistaModelo.SeleccionConfirmada = avatar =>
            {
                finalizacion.TrySetResult(avatar);
                ventana.Close();
            };
        }

        private static void ConfigurarEventosVentana(
            Window ventana,
            TaskCompletionSource<ObjetoAvatar> finalizacion)
        {
            ventana.Closed += (_, __) =>
            {
                if (!finalizacion.Task.IsCompleted)
                {
                    finalizacion.TrySetResult(null);
                }
            };
        }

        private static void MostrarVentana(
            SeleccionAvatar ventana,
            SeleccionAvatarVistaModelo vistaModelo)
        {
            ventana.DataContext = vistaModelo;
            ventana.ShowDialog();
        }

        private static void RegistrarErrorXaml(XamlParseException excepcion)
        {
            _logger.Error(
                "Error XAML al cargar la interfaz de seleccion de avatar.",
                excepcion);
        }

        private static void EstablecerExcepcionInterfaz(
            TaskCompletionSource<ObjetoAvatar> finalizacion,
            XamlParseException excepcion)
        {
            var excepcionEnvuelta = new InvalidOperationException(
                "Error al cargar la interfaz de seleccion de avatar.",
                excepcion);

            finalizacion.TrySetException(excepcionEnvuelta);
        }

        private static void RegistrarErrorOperacionInvalida(InvalidOperationException excepcion)
        {
            _logger.Error("Operacion invalida al mostrar dialogo de avatar.", excepcion);
        }

        private static void EstablecerExcepcion(
            TaskCompletionSource<ObjetoAvatar> finalizacion,
            InvalidOperationException excepcion)
        {
            finalizacion.TrySetException(excepcion);
        }
    }
}