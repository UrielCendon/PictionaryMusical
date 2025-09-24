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
    /// Lógica de interacción para CrearCuenta.xaml
    /// </summary>
    public partial class CrearCuenta : Window
    {
        public CrearCuenta()
        {
            InitializeComponent();
        }

        private void Boton_CrearCuenta(object sender, RoutedEventArgs e)
        {

        }

        private void Boton_Cancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
