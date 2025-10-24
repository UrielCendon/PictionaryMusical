using System.Windows;


namespace PictionaryMusicalCliente
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Avisos : Window
    {
        public Avisos(string mensaje)
        {
            InitializeComponent();
            textoMensaje.Text = mensaje; 

        }

        private void botonAceptar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
