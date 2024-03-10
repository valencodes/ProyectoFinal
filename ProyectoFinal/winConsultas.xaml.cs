using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
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

namespace ProyectoFinal
{
    /// <summary>
    /// Lógica de interacción para winConsultas.xaml
    /// </summary>
    public partial class winConsultas : Window
    {
        String nombretabla;
        public winConsultas()
        {
            InitializeComponent();
        }

        private void ckLibrerias_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Control micontrolgrid in gridCK.Children) // Recorremoms matriz con 
                                                               // controles del grid secundario
            {
                if (micontrolgrid is CheckBox && micontrolgrid != sender) //Si CheckBox 
                                                                          // y no genera evento 
                    ((CheckBox)micontrolgrid).IsChecked = false; //Desmarcamos 
            }
            // nombretabla variable string global
            nombretabla = ((CheckBox)sender).Name.Substring(2); //Extraemos el nombre de 
                                                                // la tabla del nombre del CheckBox
                                                                // SQL para obtener datos tabla seleccionada y mostrar en DataGrid superior
            OleDbDataAdapter adaptador = new OleDbDataAdapter("SELECT * FROM " + nombretabla, winPrincipal.miconexion);
            DataSet ds = new DataSet();
            adaptador.Fill(ds);
            dgTabla1.ItemsSource = ds.Tables[0].DefaultView;
            btMostrar.IsEnabled = true;
            Title = "Mostrar Libros por: " + nombretabla;

        }

        private void ckLibrerias_Unchecked(object sender, RoutedEventArgs e)
        {
            dgTabla1.ItemsSource = null; //Vaciamos los DataGrid
            dgLibros.ItemsSource = null;
            btMostrar.IsEnabled = false; //Deshabilitamos botón
            Title = "Mostrar Libros ";

        }

        private void btMostrar_Click(object sender, RoutedEventArgs e)
        {
            if (dgTabla1.SelectedIndex > -1)
            {
                DataView datos = (DataView)dgTabla1.ItemsSource; //El ItemsSource no es 
                                                                 // accesible necesitamos objeto DataView
                string ced = "";
                string sentencia = "";
                switch (nombretabla) // nombre proporcionado desde el CheckBox
                {
                    case "Librerias":
                        ced = datos.Table.Rows[dgTabla1.SelectedIndex]["Codigolibreria"].ToString();
                        sentencia = "SELECT CodigoLibro, Titulo, Observacion, ISBN, FechaImpresion FROM libros WHERE Codigolibreria=@micod";
                        break;
                    case "Generos":
                        ced = datos.Table.Rows[dgTabla1.SelectedIndex]["CodigoGenero"].ToString();
                        sentencia = "SELECT CodigoLibro, Titulo, Observacion, ISBN, FechaImpresion FROM libros WHERE CodigoGenero=@micod";
                        break;
                    case "Autores":
                        ced = datos.Table.Rows[dgTabla1.SelectedIndex]["CodigoAutor"].ToString();
                        sentencia = "SELECT CodigoLibro, Titulo, Observacion, ISBN, FechaImpresion FROM libros WHERE CodigoAutor=@micod";
                        break;
                    case "Editoriales":
                        ced = datos.Table.Rows[dgTabla1.SelectedIndex]["CodigoEditorial"].ToString();
                        sentencia = "SELECT CodigoLibro, Titulo, Observacion, ISBN, FechaImpresion FROM libros WHERE CodigoEditorial=@micod";
                        break;

            } // Fin del switch
                OleDbDataAdapter adaptador = new OleDbDataAdapter(sentencia, winPrincipal.miconexion);
                adaptador.SelectCommand.Parameters.AddWithValue("@micod", ced);
                DataSet ds = new DataSet();
                adaptador.Fill(ds);
                dgLibros.ItemsSource = ds.Tables[0].DefaultView;
                string ced2 =
                datos.Table.Rows[dgTabla1.SelectedIndex]["nombre"].ToString();
                laTotal.Content = "Total Libros de " + ced2 + ": " +
                dgLibros.Items.Count;
            } // Fin del if
            else
            {
                MessageBox.Show("Selecciona un/a " + nombretabla);
            }
        }

        private void btSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
