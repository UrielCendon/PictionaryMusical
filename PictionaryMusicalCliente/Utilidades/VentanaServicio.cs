using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PictionaryMusicalCliente.Utilidades.Abstracciones;
using PictionaryMusicalCliente.Vista;
using PictionaryMusicalCliente.VistaModelo.Ajustes;
using PictionaryMusicalCliente.VistaModelo.Amigos;
using PictionaryMusicalCliente.VistaModelo.InicioSesion;
using PictionaryMusicalCliente.VistaModelo.Perfil;
using PictionaryMusicalCliente.VistaModelo.Salas;
using PictionaryMusicalCliente.VistaModelo.Sesion;
using PictionaryMusicalCliente.VistaModelo.VentanaPrincipal;

namespace PictionaryMusicalCliente.Utilidades
{
    /// <summary>
    /// Implementacion concreta del servicio de ventanas para WPF.
    /// Utiliza la ventana personalizada 'Avisos' para notificaciones.
    /// </summary>
    public class VentanaServicio : IVentanaServicio
    {
        private static readonly ILog _logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Dictionary<Type, Func<Window>> FabricasVentanas = 
            new Dictionary<Type, Func<Window>>
            {
                [typeof(InicioSesionVistaModelo)] = () => new InicioSesion(),
                [typeof(VentanaPrincipalVistaModelo)] = () => new VentanaPrincipal(),
                [typeof(SalaVistaModelo)] = () => new Sala(),
                [typeof(CreacionCuentaVistaModelo)] = () => new CreacionCuenta(),
                [typeof(AjustesPartidaVistaModelo)] = () => new AjustesPartida(),
                [typeof(AjustesVistaModelo)] = () => new Ajustes(),
                [typeof(ConfirmacionSalirPartidaVistaModelo)] = () => 
                    new ConfirmacionSalirPartida(),
                [typeof(BusquedaAmigoVistaModelo)] = () => new BusquedaAmigo(),
                [typeof(EliminacionAmigoVistaModelo)] = () => new EliminacionAmigo(),
                [typeof(InvitarAmigosVistaModelo)] = () => new InvitarAmigos(),
                [typeof(SolicitudesVistaModelo)] = () => new Solicitudes(),
                [typeof(CambioContrasenaVistaModelo)] = () => new CambioContrasena(),
                [typeof(PerfilVistaModelo)] = () => new Perfil(),
                [typeof(SeleccionAvatarVistaModelo)] = () => new SeleccionAvatar(),
                [typeof(VerificacionCodigoVistaModelo)] = () => new VerificacionCodigo(),
                [typeof(ExpulsionJugadorVistaModelo)] = () => new ExpulsionJugador(),
                [typeof(IngresoPartidaInvitadoVistaModelo)] = () => new IngresoPartidaInvitado(),
                [typeof(ReportarJugadorVistaModelo)] = () => new ReportarJugador(),
                [typeof(TerminacionSesionVistaModelo)] = () => new TerminacionSesion(),
                [typeof(ClasificacionVistaModelo)] = () => new Clasificacion()
            };

        /// <summary>
        /// Muestra una ventana no modal asociada a un ViewModel especifico.
        /// </summary>
        /// <param name="vistaModelo">
        /// El ViewModel que define el contenido y logica de la ventana.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="vistaModelo"/> es nulo.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Se lanza si no existe una vista registrada para el tipo de ViewModel.
        /// </exception>
        public void MostrarVentana(object vistaModelo)
        {
            var ventana = CrearVentana(vistaModelo);
            ventana.Show();
        }

        /// <summary>
        /// Muestra una ventana en modo dialogo (bloqueante) asociada a un ViewModel.
        /// </summary>
        /// <param name="vistaModelo">El ViewModel que define el contenido de la ventana.</param>
        /// <returns>El resultado del dialogo (true, false o null) al cerrarse.</returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="vistaModelo"/> es nulo.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Se lanza si no existe una vista registrada para el tipo de ViewModel.
        /// </exception>
        public bool? MostrarVentanaDialogo(object vistaModelo)
        {
            var ventana = CrearVentana(vistaModelo);
            return ventana.ShowDialog();
        }

        /// <summary>
        /// Cierra la ventana activa que este vinculada al ViewModel proporcionado.
        /// </summary>
        /// <param name="vistaModelo">El ViewModel cuya ventana asociada se debe cerrar.</param>
        public void CerrarVentana(object vistaModelo)
        {
            var ventana = Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(v => v.DataContext == vistaModelo);

            ventana?.Close();
        }

        /// <summary>
        /// Cierra todas las ventanas abiertas excepto la ventana de inicio de sesion.
        /// Util para limpiar ventanas huerfanas cuando se detecta una desconexion.
        /// </summary>
        public void CerrarTodasLasVentanas()
        {
            var ventanasAbiertas = Application.Current?.Windows.OfType<Window>()
                .Where(v => v.IsVisible && !(v is Vista.InicioSesion))
                .ToList();

            if (ventanasAbiertas == null)
            {
                return;
            }

            foreach (var ventana in ventanasAbiertas)
            {
                try
                {
                    ventana.Close();
                }
                catch (InvalidOperationException excepcion)
                {
                    _logger.Info(
                        "No se pudo cerrar una ventana, ya estaba en proceso de cierre.",
                        excepcion);
                }
            }
        }

        /// <summary>
        /// Muestra un mensaje emergente informativo al usuario utilizando la ventana de Avisos.
        /// </summary>
        /// <param name="titulo">El titulo del mensaje (se usa para contexto si es necesario).
        /// </param>
        /// <param name="mensaje">El contenido textual del mensaje a mostrar.</param>
        public void MostrarMensaje(string titulo, string mensaje)
        {
            var aviso = new Avisos(mensaje);
            ConfigurarPropietario(aviso);
            aviso.ShowDialog();
        }

        /// <summary>
        /// Muestra un mensaje de error critico o de validacion al usuario.
        /// </summary>
        /// <param name="mensaje">El texto descriptivo del error.</param>
        public void MostrarError(string mensaje)
        {
            var aviso = new Avisos(mensaje);
            ConfigurarPropietario(aviso);
            aviso.ShowDialog();
        }

        private static void ConfigurarPropietario(Window ventanaHija)
        {
            if (Application.Current != null &&
                Application.Current.MainWindow != null &&
                Application.Current.MainWindow.IsVisible)
            {
                ventanaHija.Owner = Application.Current.MainWindow;
            }
        }

        private static Window CrearVentana(object vistaModelo)
        {
            if (vistaModelo == null)
            {
                throw new ArgumentNullException(nameof(vistaModelo));
            }

            Window ventana = ResolverVentanaPorVistaModelo(vistaModelo);
            ventana.DataContext = vistaModelo;
            return ventana;
        }

        private static Window ResolverVentanaPorVistaModelo(object vistaModelo)
        {
            Type tipoVistaModelo = vistaModelo.GetType();

            if (FabricasVentanas.TryGetValue(tipoVistaModelo, out Func<Window> fabricaVentana))
            {
                return fabricaVentana();
            }

            throw new InvalidOperationException(
                $"No existe vista registrada para {tipoVistaModelo.Name}");
        }
    }
}