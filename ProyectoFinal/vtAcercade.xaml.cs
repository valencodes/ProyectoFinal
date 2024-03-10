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

namespace MultiVentana
{
    /// <summary>
    /// Lógica de interacción para vtAcercade.xaml
    /// </summary>
    public partial class vtAcercade : Window
    {
        public vtAcercade()
        {
            InitializeComponent();
        }

        private void btAceptar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
