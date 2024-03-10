using MultiVentana;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoFinal
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class winPrincipal : Window
    {
        public static OleDbConnection miconexion;
        public winPrincipal()
        {
            InitializeComponent();
        }

        private void btSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btAcercade_Click(object sender, RoutedEventArgs e)
        {
            Window acercade = new vtAcercade();
            acercade.ShowDialog();
        }

        private void btAutores_Click(object sender, RoutedEventArgs e)
        {
            Window autores = new winAutores();
            autores.ShowDialog();
        }

        private void btLibros_Click(object sender, RoutedEventArgs e)
        {
            Window libros = new winLibros();
            libros.ShowDialog();
        }

        private void btEditoriales_Click(object sender, RoutedEventArgs e)
        {
            Window editoriales = new winEditoriales();
            editoriales.ShowDialog();
        }

        private void btLibrerias_Click(object sender, RoutedEventArgs e)
        {
            Window librerias = new winLibrerias();
            librerias.ShowDialog();
        }

        private void btGeneros_Click(object sender, RoutedEventArgs e)
        {
            Window generos = new winGeneros();
            generos.ShowDialog();
        }

        private void btConsultar_Click(object sender, RoutedEventArgs e)
        {
            Window consultas = new winConsultas();
            consultas.ShowDialog();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Acceso con ruta relativa 
            string conecta = (@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=.\datos\libros.accdb");
            miconexion = new OleDbConnection(conecta);
            miconexion.Open();

        }
    }
}
