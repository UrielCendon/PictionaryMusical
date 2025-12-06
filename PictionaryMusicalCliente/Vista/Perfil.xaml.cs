using PictionaryMusicalCliente.ClienteServicios.Abstracciones;
using PictionaryMusicalCliente.ClienteServicios.Dialogos;
using PictionaryMusicalCliente.ClienteServicios.Wcf;
using PictionaryMusicalCliente.Utilidades;
using PictionaryMusicalCliente.VistaModelo.Perfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PictionaryMusicalCliente
{
    /// <summary>
    /// Ventana de gestion del perfil de usuario.
    /// </summary>
    public partial class Perfil : Window
    {
        /// <summary>
        /// Inicializa la ventana y carga los datos del perfil.
        /// </summary>
        public Perfil()
        {
            InitializeComponent();

            IPerfilServicio perfilServicio = new PerfilServicio();
            ISeleccionarAvatarServicio seleccionarAvatarServicio =
                new SeleccionAvatarDialogoServicio();
            ICambioContrasenaServicio cambioContrasenaServicio =
                new CambioContrasenaServicio();
            IVerificacionCodigoDialogoServicio verificarCodigoDialogoServicio =
                new VerificacionCodigoDialogoServicio();
            IRecuperacionCuentaServicio recuperacionCuentaDialogoServicio =
                new RecuperacionCuentaDialogoServicio(verificarCodigoDialogoServicio);

            var vistaModelo = new PerfilVistaModelo(
                perfilServicio,
                seleccionarAvatarServicio,
                cambioContrasenaServicio,
                recuperacionCuentaDialogoServicio)
            {
                CerrarAccion = Close
            };

            vistaModelo.MostrarCamposInvalidos = MarcarCamposInvalidos;
            vistaModelo.SolicitarReinicioSesion = NavegarAlInicioSesion;

            DataContext = vistaModelo;
        }

        private async void Perfil_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (DataContext is PerfilVistaModelo vistaModelo)
            {
                await vistaModelo.CargarPerfilAsync().ConfigureAwait(true);
            }
        }

        private void PopupRedSocial_Opened(object sender, EventArgs e)
        {
            if (sender is Popup popup &&
                popup.Child is Border border &&
                border.Child is TextBox textBox)
            {
                textBox.Focus();
                textBox.CaretIndex = textBox.Text?.Length ?? 0;
            }
        }

        private void RedSocialTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox &&
                textBox.Tag is ToggleButton toggle &&
                (e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Escape))
            {
                toggle.IsChecked = false;
                e.Handled = true;
            }
        }

        private void MarcarCamposInvalidos(IList<string> camposInvalidos)
        {
            ControlVisual.RestablecerEstadoCampo(campoTextoNombre);
            ControlVisual.RestablecerEstadoCampo(campoTextoApellido);

            if (camposInvalidos == null)
            {
                return;
            }

            if (camposInvalidos.Contains(nameof(PerfilVistaModelo.Nombre)))
            {
                ControlVisual.MarcarCampoInvalido(campoTextoNombre);
            }

            if (camposInvalidos.Contains(nameof(PerfilVistaModelo.Apellido)))
            {
                ControlVisual.MarcarCampoInvalido(campoTextoApellido);
            }
        }

        private void NavegarAlInicioSesion()
        {
            var inicioSesion = new InicioSesion();

            var ventanasACerrar = Application.Current.Windows
                .Cast<Window>()
                .Where(v => v != inicioSesion)
                .ToList();

            inicioSesion.Show();
            Application.Current.MainWindow = inicioSesion;

            foreach (var ventana in ventanasACerrar)
            {
                ventana.Close();
            }
        }
    }
}