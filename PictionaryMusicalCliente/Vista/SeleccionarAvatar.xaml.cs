using PictionaryMusicalCliente.Modelo;
using PictionaryMusicalCliente.Properties.Langs;
using PictionaryMusicalCliente.Servicios;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PictionaryMusicalCliente
{
    public partial class SeleccionarAvatar : Window
    {
        public int? AvatarSeleccionadoId { get; private set; }

        public SeleccionarAvatar()
        {
            InitializeComponent();
            Loaded += SeleccionarAvatar_Loaded;
        }

        private async void SeleccionarAvatar_Loaded(object sender, RoutedEventArgs e)
        {
            List<PictionaryMusicalCliente.Modelo.ObjetoAvatar> avatares;
            using (var proxy = new ServidorProxy())
            {
                avatares = await proxy.ObtenerAvataresAsync();
            }


            // Asumiendo que tienes un ListBox llamado "listaAvatares"
            listaAvatares.Items.Clear();
            foreach (var a in avatares)
            {
                var img = new Image
                {
                    Width = 72,
                    Height = 72,
                    Source = string.IsNullOrEmpty(a.ImagenUriAbsoluta) ? null : new BitmapImage(new System.Uri(a.ImagenUriAbsoluta))
                };
                var item = new ListBoxItem { Content = img, Tag = a.Id, ToolTip = a.Nombre };
                listaAvatares.Items.Add(item);
            }
        }

        private void BotonAceptarSeleccionAvatar(object sender, RoutedEventArgs e)
        {
            if (listaAvatares.SelectedItem is ListBoxItem li && li.Tag is int id)
            {
                AvatarSeleccionadoId = id;
                DialogResult = true;
                Close();
            }
            else
            {
                new Avisos(Lang.globalTextoSeleccionarAvatar).ShowDialog();
            }
        }
    }
}

