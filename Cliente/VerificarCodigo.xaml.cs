using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PictionaryMusicalCliente
{
    /// <summary>
    /// Lógica de interacción para VerificarCodigo.xaml
    /// </summary>
    public partial class VerificarCodigo : Window
    {
        public VerificarCodigo()
        {
            InitializeComponent();
        }

        private void BotonVerificarCodigo(object sender, RoutedEventArgs e)
        {
            CambioContrasena ventana = new CambioContrasena();
            ventana.ShowDialog();
        }

        private void BotonReenviarCodigo(object sender, RoutedEventArgs e)
        {

        }

        private void BotonCancelarCodigo(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
