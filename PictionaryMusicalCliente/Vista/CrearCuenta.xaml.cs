using System.Text.RegularExpressions;
using System.Windows;
using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Servicios;
using PictionaryMusicalCliente.Properties.Langs;

namespace PictionaryMusicalCliente
{
    public partial class CrearCuenta : Window
    {
        private int? _avatarId; // guardamos el Id

        public CrearCuenta()
        {
            InitializeComponent();
        }

        private void EtiquetaSeleccionarAvatar(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var ventanaSeleccion = new SeleccionarAvatar();
            if (ventanaSeleccion.ShowDialog() == true && ventanaSeleccion.AvatarSeleccionadoId.HasValue)
            {
                _avatarId = ventanaSeleccion.AvatarSeleccionadoId.Value;
                // opcional: mostrar un “Seleccionado: #Id” en una etiqueta
            }
        }

        private async void Boton_CrearCuenta(object sender, RoutedEventArgs e)
        {
            // Lee tus controles (ajusta los nombres a tu XAML)
            string usuario = bloqueTextoUsuario.Text?.Trim();
            string correo = bloqueTextoCorreo.Text?.Trim();
            string nombre = bloqueTextoNombre.Text?.Trim();
            string apellido = bloqueTextoApellido.Text?.Trim();
            string pass1 = bloqueContrasenaContrasena.Password;

            // Validaciones rápidas en cliente
            if (string.IsNullOrWhiteSpace(usuario)) { new Avisos("Ingresa un usuario.").ShowDialog(); return; }
            if (string.IsNullOrWhiteSpace(correo) || !Regex.IsMatch(correo, @"^\S+@\S+\.\S+$"))
            { new Avisos("Correo no válido.").ShowDialog(); return; }
            if (string.IsNullOrWhiteSpace(pass1) || pass1.Length < 6)
            { new Avisos("La contraseña debe tener al menos 6 caracteres.").ShowDialog(); return; }
            if (!_avatarId.HasValue)
            { new Avisos(Lang.globalTextoSeleccionarAvatar).ShowDialog(); return; }

            var req = new SolicitudRegistrarUsuario
            {
                Usuario = usuario,
                Correo = correo,
                Nombre = nombre,
                Apellido = apellido,
                ContrasenaPlano = pass1,
                AvatarId = _avatarId.Value
            };

            bool ok;
            using (var proxy = new ServidorProxy())
            {
                ok = await proxy.RegistrarCuentaAsync(req);
            }


            if (ok) new Avisos("Registro exitoso.").ShowDialog();
            else new Avisos("No se pudo registrar. Intenta más tarde.").ShowDialog();
        }

        private void Boton_Cancelar(object sender, RoutedEventArgs e) => Close();
    }
}
