using System.Windows;

namespace PictionaryMusicalCliente
{
    /// <summary>
    /// Lógica de interacción para InvitarAmigos.xaml
    /// </summary>
    public partial class InvitarAmigos : Window
    {
        public InvitarAmigos()
        {
            InitializeComponent();
        }

        private void BotonRegresarInvitarAmigos(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
