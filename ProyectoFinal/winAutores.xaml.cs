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
    /// Lógica de interacción para winAutores.xaml
    /// </summary>
    public partial class winAutores : Window
    {
        DataTable tabla; // Guardamos resultado de la ejecución de una sentencia SQL
        DataRow fila; // Guardamos fila de una tabla

        public winAutores()
        {
            InitializeComponent();
        }

        private void cambiaventana(bool modo) //parámetro que recibe true o false
        { //Con esto cambiamos a la ventana a modo edición o no 
            lbAutores.IsEnabled = modo;
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

        private void limpiartextbox()// vaciamos combobox y textbox correspondientes,
        { // al principio pero también en nuevo, etc
            foreach (Control controlgrid in gridTextbox.Children)
            { // Seleccionamos objetos a limpiar desde la matriz children del grid
                if (controlgrid is TextBox)
                {
                    ((TextBox)controlgrid).Clear();
                }
            }
        }

        private void cargardatoslisbox()
        {
            // Llenamos listbox con el nombre desde tabla autores 
            OleDbDataAdapter adaptador = new OleDbDataAdapter("SELECT * FROM autores", winPrincipal.miconexion);

            DataSet ds = new DataSet();
            adaptador.Fill(ds);
            tabla = new DataTable();
            tabla = ds.Tables[0];
            lbAutores.Items.Clear();
            for (int i = 0; i < tabla.Rows.Count; i++)
            {
                fila = tabla.Rows[i];
                lbAutores.Items.Add(fila["nombre"].ToString());
            }
        }

        public string poncodigo(string latabla, string elcodigo, string elitem)
        { // Función compartida recibe nombre tabla y código Devuelve valor código
            string sentenciasql = "SELECT " + elcodigo + " FROM " + latabla + " WHERE nombre = @micod";
            OleDbCommand comando = new OleDbCommand(sentenciasql, winPrincipal.miconexion);
            string mid = elitem;// extraído de elemento seleccionado de ComboBox 
            // correspondiente 
            comando.Parameters.AddWithValue("@micod", mid);
            OleDbDataReader lector = comando.ExecuteReader(); // Ejecuta consulta 
            if (lector.Read()) // SELECT y en un objeto DataReader.
            {
                return lector[elcodigo].ToString();// Devuelve código
            }
            else
            {
                return ""; // devuelve nada si la consulta está vacía
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            limpiartextbox();// Al iniciar limpiamos los textbox
            cargardatoslisbox(); // cargamos los nombres de los autores en el listbox
            cambiaventana(true); // Habilitamos botones, listbox, etc que corresponda
        }

        private void lbAutores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbAutores.SelectedIndex > -1) // Si hay selección 
            {
                fila = tabla.Rows[lbAutores.SelectedIndex];// fila objeto global 
                                                           // clase DataRow. tabla objeto global clase DataTable
                tbCodigo.Text = fila["codigoautor"].ToString();// Cargamos los datos
                tbNombreAutor.Text = fila["nombre"].ToString();// en los textbox
                tbCiudad.Text = fila["ciudad"].ToString();// y el datepicker
                tbNacionalidad.Text = fila["nacionalidad"].ToString();
                tbComentario.Text = fila["comentario"].ToString();
            }
        }

        private void btNuevo_Click(object sender, RoutedEventArgs e)
        {
            limpiartextbox();
            lbAutores.SelectedItem = null; // Quitamos selección del listBox
            cambiaventana(false); // Cambiamos de estado a controles 
            btActualizar.IsEnabled = false;

            string numFilas = "SELECT TOP 1 codigoautor FROM libros ORDER BY codigoautor DESC "; // Para ver el último código
            OleDbCommand comando1 = new OleDbCommand(numFilas, winPrincipal.miconexion);
            object resultado = comando1.ExecuteScalar(); // Obtiene el resultado como object
            int numfil = 0; // Variable para almacenar el código del autor
            if (resultado != null) // Verifica si el resultado es diferente de null
            {
                numfil = Convert.ToInt32(resultado); // Convierte el resultado a int de forma segura
            }

            tbCodigo.Text = Convert.ToString(numfil + 1); // Nuevo código último + 1
                                           // Nuevo código último + 1
            tbNombreAutor.Focus();//Enviamos foco para empezar tecleo de nuevos datos
        }

        private void btModificar_Click(object sender, RoutedEventArgs e)
        {
            if (lbAutores.Items.Count > 0) // Si no hay ítems en listbox no hay autores
            {
                if (lbAutores.SelectedItem != null) // No hay ítem seleccionado 
                {
                    cambiaventana(false);// cambia a modo edición con botones indicados.
                    btGuardar.IsEnabled = false;
                    tbNombreAutor.Focus();
                }
                else
                {
                    MessageBox.Show("Tienes que seleccionar un autor de la lista para poder modificarlo.", "Información", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Actualmente no hay ningún autor en la base de datos.", "Información", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        private bool camposrequeridos() //Los campos obligatorios tienen un *
        { //comprobamos si los datos obligatorios han sido introducidos

            foreach (Control controlgrid in gridTextbox.Children)
            {
                string[] requeridos = { "tbNombreAutor", "tbCiudad", "tbNacionalidad",
 "tbComentario" };
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
            private void btEliminarAutor_Click(object sender, RoutedEventArgs e)
        {
            if (lbAutores.Items.Count > 0) // mismas comprobaciones que en modificar
            {
                if (lbAutores.SelectedItem != null)
                {
                    if (MessageBox.Show("¿Realmente desea eliminar el autor " +
                        tbNombreAutor.Text + " de la base de datos?",
                        "Confirmar Eliminación de Registro", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        string borra = "DELETE FROM autores WHERE codigoautor = @idborra";
                        // Sentencia SQl de borrado con el autor seleccionado
                        OleDbCommand comandoborra = new OleDbCommand(borra, // instrucción
                            winPrincipal.miconexion); // SQL conexión definida en winPrincipal
                        comandoborra.Parameters.AddWithValue("@idborra", tbCodigo.Text);
                        // Parámetro con código a borrar
                        try
                        {
                            comandoborra.ExecuteNonQuery(); //Método usado para ejecutar 
                        } // sentencia de borrado
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        MessageBox.Show("Datos borrados correctamente.");
                        lbAutores.Items.RemoveAt(lbAutores.SelectedIndex); // Eliminamos 
                        lbAutores.SelectedIndex = lbAutores.Items.Count - 1; //el autor del 
                        limpiartextbox(); // listbox y borramos contenido de los textbox 
                        lbAutores.SelectedItem = null;
                        tbCodigo.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Selecciona un autor a borrar.");
                    lbAutores.Focus();
                }
            }
        }

        private void btGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!camposrequeridos())//Comprobamos si están todos los requeridos
            { // Ver detalle en función
                return;
            }
            // Sentencia SQL de inserción del nuevo libro tecleado en los textbox 
            String insertar = "INSERT INTO autores(codigoautor, nombre, ciudad, nacionalidad, comentario)" + "VALUES('" + tbCodigo.Text + "', '" + tbNombreAutor.Text +
 "', '" + tbCiudad.Text + "', '" + tbNacionalidad.Text + "', '" +
 tbComentario.Text + "')";
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
            cargardatoslisbox();//mostros nuevo libro 
            lbAutores.SelectedItem = lbAutores.Items.Count - 1; // lo seleccionamos
            lbAutores.Focus();
            cambiaventana(true); //cambiamos controles para que no estén en edición
            limpiartextbox();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            lbAutores.SelectedItem = null; //Quitamos selección
            limpiartextbox();
            tbCodigo.Clear();
            cambiaventana(true); // Dejamos controles sin edición
        }

        private void btActualizar_Click(object sender, RoutedEventArgs e)
        {
            if (!camposrequeridos()) //comprobamos campos obligatorios si tienen datos
            {
                return;
            }
            Int32 mid = Convert.ToInt32(tbCodigo.Text);
            //Sentencia SQL de actualización. Modifica datos con los nuevos tecleados.
            string modificar = "UPDATE autores " + "SET nombre = '" + tbNombreAutor.Text +
                "', ciudad = '" + tbCiudad.Text + "', nacionalidad = '" +
                tbNacionalidad.Text + "', comentario = '" + tbComentario.Text + "' WHERE codigoautor = @mid; ";
            OleDbCommand comando2 = new OleDbCommand(modificar, // instrucción SQL
                winPrincipal.miconexion); // conexión definida en winprincipal 
            comando2.Parameters.AddWithValue("@mid", mid); //Parámetro sentencia SQL
            try
            {
                comando2.ExecuteNonQuery(); //Método para ejecutar sentencia UPDATE
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Datos modificados correctamente.");
            cargardatoslisbox(); //Mostramos nuevo autor en listbox
            lbAutores.Focus(); // Esperamos un nuevo click 
            cambiaventana(true); //dejamos controles sin edición
            limpiartextbox();
        }

        private void btSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            {
                if (lbAutores.Items.Count > 0) // mismas comprobaciones que en modificar
                {
                    if (lbAutores.SelectedItem != null)
                    {
                        if (MessageBox.Show("¿Realmente desea eliminar el autor " +
                            tbNombreAutor.Text + " de la base de datos?",
                            "Confirmar Eliminación de Registro", MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            string borra = "DELETE FROM autores WHERE codigoautor = @idborra";
                            // Sentencia SQl de borrado con el autor seleccionado
                            OleDbCommand comandoborra = new OleDbCommand(borra, // instrucción
                                winPrincipal.miconexion); // SQL conexión definida en winPrincipal
                            comandoborra.Parameters.AddWithValue("@idborra", tbCodigo.Text);
                            // Parámetro con código a borrar
                            try
                            {
                                comandoborra.ExecuteNonQuery(); //Método usado para ejecutar 
                            } // sentencia de borrado
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            MessageBox.Show("Datos borrados correctamente.");
                            lbAutores.Items.RemoveAt(lbAutores.SelectedIndex); // Eliminamos 
                            lbAutores.SelectedIndex = lbAutores.Items.Count - 1; //el autor del 
                            limpiartextbox(); // listbox y borramos contenido de los textbox 
                            lbAutores.SelectedItem = null;
                            tbCodigo.Clear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Selecciona un autor a borrar.");
                        lbAutores.Focus();
                    }
                }
            }
            }
    }
}
