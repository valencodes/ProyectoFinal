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
    /// Lógica de interacción para winGeneros.xaml
    /// </summary>
    public partial class winGeneros : Window
    {
        DataTable tabla;
        DataRow fila;

        public winGeneros()
        {
            InitializeComponent();
        }

        private void cambiaventana(bool modo)
        {
            lbGeneros.IsEnabled = modo;
            uGridNuevoModificarEliminar.IsEnabled = modo;
            btGuardar.IsEnabled = !modo;
            btActualizar.IsEnabled = !modo;
            btCancelar.IsEnabled = !modo;
            texboxsololectura(modo);
        }

        private void texboxsololectura(bool modo)
        {
            foreach (Control controlgrid in gridTextbox.Children)
            {
                if (controlgrid is TextBox)
                {
                    ((TextBox)controlgrid).IsReadOnly = modo;
                }
            }
        }

        private void limpiartextbox()
        {
            foreach (Control controlgrid in gridTextbox.Children)
            {
                if (controlgrid is TextBox)
                {
                    ((TextBox)controlgrid).Clear();
                }
            }
        }

        private void cargardatoslisbox()
        {
            OleDbDataAdapter adaptador = new OleDbDataAdapter("SELECT * FROM generos", winPrincipal.miconexion);
            DataSet ds = new DataSet();
            adaptador.Fill(ds);
            tabla = new DataTable();
            tabla = ds.Tables[0];
            lbGeneros.Items.Clear();
            for (int i = 0; i < tabla.Rows.Count; i++)
            {
                fila = tabla.Rows[i];
                lbGeneros.Items.Add(fila["nombre"].ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            limpiartextbox();
            cargardatoslisbox();
            cambiaventana(true);
        }

        private void lbGeneros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbGeneros.SelectedIndex > -1)
            {
                fila = tabla.Rows[lbGeneros.SelectedIndex];
                tbCodigo.Text = fila["codigogenero"].ToString();
                tbNombreGenero.Text = fila["nombre"].ToString();
            }
        }

        private void btNuevo_Click(object sender, RoutedEventArgs e)
        {
            limpiartextbox();
            lbGeneros.SelectedItem = null;
            cambiaventana(false);
            btActualizar.IsEnabled = false;

            string numFilas = "SELECT TOP 1 codigogenero FROM generos ORDER BY codigogenero DESC ";
            OleDbCommand comando1 = new OleDbCommand(numFilas, winPrincipal.miconexion);
            object resultado = comando1.ExecuteScalar();
            int numfil = 0;
            if (resultado != null)
            {
                numfil = Convert.ToInt32(resultado);
            }

            tbCodigo.Text = Convert.ToString(numfil + 1);
            tbNombreGenero.Focus();
        }

        private void btModificar_Click(object sender, RoutedEventArgs e)
        {
            if (lbGeneros.Items.Count > 0)
            {
                if (lbGeneros.SelectedItem != null)
                {
                    cambiaventana(false);
                    btGuardar.IsEnabled = false;
                    tbNombreGenero.Focus();
                }
                else
                {
                    MessageBox.Show("Tienes que seleccionar un género de la lista para poder modificarlo.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Actualmente no hay ningún género en la base de datos.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool camposrequeridos()
        {
            foreach (Control controlgrid in gridTextbox.Children)
            {
                string[] requeridos = { "tbNombreGenero", "tbDescripcion" };
                if ((controlgrid is TextBox) && (((TextBox)controlgrid).Text == ""))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (controlgrid.Name == requeridos[i])
                        {
                            MessageBox.Show("Falta Dato " + controlgrid.Name.Substring(2));
                            controlgrid.Focus();
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lbGeneros.Items.Count > 0)
            {
                if (lbGeneros.SelectedItem != null)
                {
                    if (MessageBox.Show("¿Realmente desea eliminar el género " + tbNombreGenero.Text + " de la base de datos?", "Confirmar Eliminación de Registro", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        string borra = "DELETE FROM generos WHERE codigogenero = @idborra";
                        OleDbCommand comandoborra = new OleDbCommand(borra, winPrincipal.miconexion);
                        comandoborra.Parameters.AddWithValue("@idborra", tbCodigo.Text);
                        try
                        {
                            comandoborra.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        MessageBox.Show("Datos borrados correctamente.");
                        lbGeneros.Items.RemoveAt(lbGeneros.SelectedIndex);
                        lbGeneros.SelectedIndex = lbGeneros.Items.Count - 1;
                        limpiartextbox();
                        lbGeneros.SelectedItem = null;
                        tbCodigo.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Selecciona un género a borrar.");
                    lbGeneros.Focus();
                }
            }
        }

        private void btGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!camposrequeridos())
            {
                return;
            }
            String insertar = "INSERT INTO generos(codigogenero, nombre)" + "VALUES('" + tbCodigo.Text + "', '" + tbNombreGenero.Text + "')";
            OleDbCommand comando = new OleDbCommand(insertar, winPrincipal.miconexion);
            try
            {
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Datos guardados correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            cargardatoslisbox();
            lbGeneros.SelectedItem = lbGeneros.Items.Count - 1;
            lbGeneros.Focus();
            cambiaventana(true);
            limpiartextbox();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            lbGeneros.SelectedItem = null;
            limpiartextbox();
            tbCodigo.Clear();
            cambiaventana(true);
        }

        private void btActualizar_Click(object sender, RoutedEventArgs e)
        {
            if (!camposrequeridos())
            {
                return;
            }
            Int32 mid = Convert.ToInt32(tbCodigo.Text);
            string modificar = "UPDATE generos " + "SET nombre = '" + tbNombreGenero.Text +  "' WHERE codigogenero = @mid; ";
            OleDbCommand comando2 = new OleDbCommand(modificar, winPrincipal.miconexion);
            comando2.Parameters.AddWithValue("@mid", mid);
            try
            {
                comando2.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Datos modificados correctamente.");
            cargardatoslisbox();
            lbGeneros.Focus();
            cambiaventana(true);
            limpiartextbox();
        }

        private void btSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
