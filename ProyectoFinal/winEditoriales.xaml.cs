using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace ProyectoFinal
{
    /// <summary>
    /// Lógica de interacción para winEditoriales.xaml
    /// </summary>
    public partial class winEditoriales : Window
    {
        DataTable tabla;
        DataRow fila;
        public winEditoriales()
        {
            InitializeComponent();
            CargarDatosListBox();
            cambiaventana(true);
        }

        private void cambiaventana(bool modo) //parámetro que recibe true o false
        { //Con esto cambiamos a la ventana a modo edición o no 
            lbEditoriales.IsEnabled = modo;
            uGridNuevoModificarEliminar.IsEnabled = modo;// Si cambiamos modo al 
                                                         // UniformGrid cambiamos también botones contenidos
            btGuardar.IsEnabled = !modo;
            btActualizar.IsEnabled = !modo;
            btCancelar.IsEnabled = !modo;
            texboxsololectura(modo);//otra función para los textbox y combobox
        }

        private void texboxsololectura(bool modo)
        { // cambiamos modo de los textbox, combobox, etc 
            foreach (Control controlgrid in gridTextbox.Children) // Recorremos la 
            { // matriz children del grid que tiene los textbox
                if (controlgrid is TextBox) // cambiamos la propiedad a los textbox 
                { // según se indique en el parámetro recibido 
                    ((TextBox)controlgrid).IsReadOnly = modo;
                }
            }
        }

        private void CargarDatosListBox()
        {
            OleDbDataAdapter adaptador = new OleDbDataAdapter("SELECT * FROM Editoriales", winPrincipal.miconexion);
            DataSet ds = new DataSet();
            adaptador.Fill(ds);
            tabla = ds.Tables[0];
            lbEditoriales.ItemsSource = tabla.AsEnumerable().Select(r => r.Field<string>("Nombre")).ToList();
        }

        private void lbEditoriales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbEditoriales.SelectedIndex >= 0)
            {
                fila = tabla.Rows[lbEditoriales.SelectedIndex];
                tbCodigo.Text = fila["CodigoEditorial"].ToString();
                tbNombreEditorial.Text = fila["Nombre"].ToString();
                tbDireccion.Text = fila["Direccion"].ToString();
                tbPoblacion.Text = fila["Poblacion"].ToString();
                tbTelefono.Text = fila["Telefono"].ToString();
                tbCIF.Text = fila["CIF"].ToString();
            }
        }
        private bool camposrequeridos() //Los campos obligatorios tienen un *
        { //comprobamos si los datos obligatorios han sido introducidos

            foreach (Control controlgrid in gridTextbox.Children)
            {
                string[] requeridos = { "tbNombreEditorial", "tbDireccion", "tbPoblacion",
 "tbTelefono", "tbTelefono", "tbCIF" };
                if ((controlgrid is TextBox) && (((TextBox)controlgrid).Text == ""))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (controlgrid.Name == requeridos[i]) //compruebo si es uno de 
                        { //los 5 texbox requeridos
                            MessageBox.Show("Falta Dato " +
                            controlgrid.Name.Substring(2)); //uso nombre para mensaje
                            controlgrid.Focus();
                            return false; // devuelve false falta algo
                        }
                    }
                }
            }
            return true;
        }

        private void limpiartextbox()
        {
            // Limpia el texto de cada TextBox.
            tbCodigo.Clear();
            tbNombreEditorial.Clear();
            tbDireccion.Clear();
            tbPoblacion.Clear();
            tbTelefono.Clear();
            tbCIF.Clear();

        }

        private void btNuevo_Click(object sender, RoutedEventArgs e)
        {
            limpiartextbox();
            lbEditoriales.SelectedItem = null; // Quitamos selección del listBox
            cambiaventana(false); // Cambiamos de estado a controles 
            btActualizar.IsEnabled = false;
            string numFilas = "SELECT TOP 1 codigoeditorial FROM editoriales ORDER BY codigoeditorial DESC "; // Para ver el último código
            OleDbCommand comando1 = new OleDbCommand(numFilas, winPrincipal.miconexion);
            int numfil = (int)comando1.ExecuteScalar();// Ver explicación anterior
            tbCodigo.Text = Convert.ToString(numfil + 1); // Nuevo código último + 1
            tbNombreEditorial.Focus();//Enviamos foco para empezar tecleo de nuevos datos

        }

        private void btModificar_Click(object sender, RoutedEventArgs e)
        {
            if (lbEditoriales.Items.Count > 0) // Verifica si hay editoriales en la lista
            {
                if (lbEditoriales.SelectedItem != null) // Verifica si hay una editorial seleccionada
                {
                    cambiaventana(false); // Cambia la ventana a modo de edición
                    btGuardar.IsEnabled = true; // Habilita el botón Guardar para permitir guardar los cambios
                    btActualizar.IsEnabled = true; // Habilita el botón Actualizar
                    btCancelar.IsEnabled = true; // Habilita el botón Cancelar para cancelar los cambios

                    // Enfoca el primer TextBox editable para comenzar la edición
                    tbNombreEditorial.Focus();
                }
                else
                {
                    MessageBox.Show("Tienes que seleccionar una editorial de la lista para poder modificarla.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Actualmente no hay ninguna editorial en la base de datos.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lbEditoriales.SelectedItem != null)
            {
                DataRowView selectedItem = (DataRowView)lbEditoriales.SelectedItem;
                string codigoEditorial = selectedItem["CodigoEditorial"].ToString();

                if (MessageBox.Show($"¿Realmente desea eliminar la editorial {selectedItem["Nombre"].ToString()} de la base de datos?",
                    "Confirmar Eliminación de Registro", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string borra = "DELETE FROM editoriales WHERE codigoeditorial = ?";
                    OleDbCommand comandoborra = new OleDbCommand(borra, winPrincipal.miconexion);
                    comandoborra.Parameters.AddWithValue("?", codigoEditorial); // Usa el valor directamente

                    try
                    {
                        comandoborra.ExecuteNonQuery();
                        MessageBox.Show("Datos borrados correctamente.");

                        // Actualizar la interfaz gráfica para reflejar la eliminación
                        CargarDatosListBox(); // Asumiendo que este método refresca la lista de editoriales desde la base de datos
                        limpiartextbox(); // Limpia los detalles mostrados de la editorial eliminada
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar la editorial: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Tienes que seleccionar una editorial de la lista para poder eliminarla.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        
    
    }

        private void btGuardar_Click(object sender, RoutedEventArgs e)
        {

            if (!camposrequeridos())//Comprobamos si están todos los requeridos
            { // Ver detalle en función
                return;
            }
            // Sentencia SQL de inserción del nuevo libro tecleado en los textbox 
            String insertar = "INSERT INTO editoriales(codigoeditorial, nombre, direccion, poblacion, telefono, cif)" + "VALUES('" + tbCodigo.Text + "', '" + tbNombreEditorial.Text +
 "', '" + tbDireccion.Text + "', '" + tbPoblacion.Text + "', '" +
 tbTelefono.Text + "', '" + tbCIF.Text + "')";
            OleDbCommand comando = new OleDbCommand(insertar, winPrincipal.miconexion);
            // Representa una instrucción SQL con conexión definida en winPrincipal
            try
            {
                comando.ExecuteNonQuery(); //Método para ejecutar sentencia INSERT
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Datos guardados correctamente.", "Información",
            MessageBoxButton.OK, MessageBoxImage.Information);
            CargarDatosListBox();//mostros nuevo libro 
            lbEditoriales.SelectedItem = lbEditoriales.Items.Count - 1; // lo seleccionamos
            lbEditoriales.Focus();
            cambiaventana(true); //cambiamos controles para que no estén en edición
            limpiartextbox();
        }


        private void btActualizar_Click(object sender, RoutedEventArgs e)
        {
            // Primero, verifica si hay una selección válida en el ListBox.
            if (lbEditoriales.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona una editorial para actualizar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Luego, verifica que los campos requeridos estén llenos.
            if (!camposrequeridos())
            {
                // CamposRequeridos() debe mostrar su propio mensaje de error.
                return;
            }

            // Preparar la consulta SQL para actualizar la editorial existente.
            string consultaSql = "UPDATE Editoriales SET Nombre = ?, Direccion = ?, Poblacion = ?, Telefono = ?, CIF = ? WHERE CodigoEditorial = ?";
            OleDbCommand comando = new OleDbCommand(consultaSql, winPrincipal.miconexion);

            // Añade los parámetros necesarios desde los controles de entrada.
            comando.Parameters.AddWithValue("Nombre", tbNombreEditorial.Text);
            comando.Parameters.AddWithValue("Direccion", tbDireccion.Text);
            comando.Parameters.AddWithValue("Poblacion", tbPoblacion.Text);
            comando.Parameters.AddWithValue("Telefono", tbTelefono.Text);
            comando.Parameters.AddWithValue("CIF", tbCIF.Text);
            comando.Parameters.AddWithValue("CodigoEditorial", tbCodigo.Text);

            try
            {
                comando.ExecuteNonQuery();
                MessageBox.Show("La editorial ha sido actualizada con éxito.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarDatosListBox(); // Recarga la lista para reflejar los cambios.
                limpiartextbox(); // Limpia los campos después de la actualización.
                cambiaventana(true); // Cambia el estado de la ventana de vuelta a modo "no edición".
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la editorial: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            lbEditoriales.SelectedItem = null; //Quitamos selección
            limpiartextbox();
            tbCodigo.Clear();
            cambiaventana(true); // Dejamos controles sin edición
        }

        private void btSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            limpiartextbox();// Al iniciar limpiamos los textbox
             CargarDatosListBox(); //cargamos los nombres de los libros en el listbox
            cambiaventana(true);
        }
    }
}

