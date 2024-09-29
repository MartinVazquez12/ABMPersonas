using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient; 
//Agrego el cliente de sql

namespace ABMPersonas
{
    public partial class frmPersona : Form
    {
        bool esNuevo = false;

        //Variables de tipo ADO.NET
        //Conexion con la base de datos desde Herramientas, copio la source y la pego en la variable
        SqlConnection conexion = new SqlConnection("Data Source=172.16.10.196;Initial Catalog=TUPPI;User ID=alumno1w1;Password=alumno1w1");   
        SqlCommand comando;
        SqlDataReader lector;

 
        public frmPersona()
        {
            InitializeComponent();
        }

        private void frmPersona_Load(object sender, EventArgs e)
        {
            habilitar(false);

            CargarCombo(cboTipoDocumento, "tipo_documento", "id_tipo_documento", "n_tipo_documento"); //Uso el metodo
            CargarCombo(cboEstadoCivil, "estado_civil", "id_estado_civil", "n_estado_civil"); //Cargo ambos combos
            CargarLista();
        }

        //Creo un metodo para no tener que copiar-pegar cada vez que quiero cargar un combo, con parametros
        private void CargarCombo(ComboBox combo, string nombreTabla, string campoValor, string campoMostrar)
        {
            //Abro y cierro la conexion para la seguridad a la hora de cada operacion
            conexion.Open();

            comando = new SqlCommand(); //Instancio el comando, creo un nuevo comando
            comando.Connection = conexion; //Le doy conexion a mi comando
            comando.CommandType = CommandType.Text; //Le asigno el tipo
            comando.CommandText = "SELECT * FROM " + nombreTabla + " ORDER BY 2"; //Le digo que voy a traer de SQL

            DataTable tabla = new DataTable(); //Creo una tabla para guardar lo que traigo con el SELECT
            tabla.Load(comando.ExecuteReader()); //Cargo con lo que trae el comando en esa tabla

            conexion.Close(); //Me desconecto ya que el DataTable trabaja desconectado una vez cargados en alguna tabla

            combo.DataSource = tabla; //Le pido que en ese espacio quiero que me muestre la "tabla"
            combo.DisplayMember = campoMostrar; //Le pido que me muestre el campo que requiera
            combo.ValueMember = campoValor; //Le digo que valide el campo clave de cada nombre
        }

        //Hago un arreglo para cargar las personas
        const int tamanio = 10;
        Persona[] aPersonas = new Persona[tamanio];
        int ultimo = 0;
        //Creo un metodo para cargar la lista de personas

        private void CargarLista()
        {
            conexion.Open();
            //Arrancamos el comando
            comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.Text;
            comando.CommandText = "SELECT * FROM personas ORDER BY 2";
            //No hace falta instanciarlo
            lector = comando.ExecuteReader();// Executo mi lector con lo que antes pedi en un select
            ultimo = 0;

            while (lector.Read())//Recorremos el dataReader, mientras el lector se pueda leer(No sea NULL)
            {
                Persona p = new Persona();//Por cada fila voy a crear un objeto con esta sentencia
                //Guardo los datos del DataReader en algun lugar
                //Distintos metodos para tomar los campos/ podemos usar nro de col 0 a n, o nombre de la tabla
                if (!lector.IsDBNull(0))//Si no es nula la columna 0 hace esto
                    p.Apellido = lector[0].ToString(); //Carga el apellido
                if (!lector.IsDBNull(1))
                    p.Nombres = lector[1].ToString();
                if (!lector.IsDBNull(2))
                    p.TipoDocumento = Convert.ToInt32(lector[2]);
                if (!lector.IsDBNull(3))
                    p.Documento = lector.GetInt32(3);
                if (!lector.IsDBNull(4))
                    p.EstadoCivil = (int)lector[4];
                if (!lector.IsDBNull(5))
                    p.Sexo = int.Parse(lector[5].ToString());
                if (!lector.IsDBNull(6))
                    p.Fallecio = lector.GetBoolean(6);
                if (!lector.IsDBNull(7))//Lo hago en cada campo para que si no es null cargue, sino que lo deje null
                    p.FechaNacimiento = lector.GetDateTime(7);

                aPersonas[ultimo] = p;
                ultimo++;
            }
            conexion.Close();//Cierro la conexion una vez terminado el uso del data reader con los datos ya cargados
            //Ahora lo voy a cargar en la lista
            lstPersonas.Items.Clear();//Limpio la lista primero
            for(int i = 0; i < ultimo; i++)//Recorro el arreglo para cargar todas las personas
            {
                lstPersonas.Items.Add(aPersonas[i]);//Uso ADD para agregar al arreglo de personas
            }

            lstPersonas.SelectedIndex = 0; //Le hago seleccionar uno con arrancar el programa
        }

        private void habilitar(bool x) //Metodo para asignar que botones puedo tocar
        {
            txtApellido.Enabled = x;
            txtNombres.Enabled = x;
            cboTipoDocumento.Enabled = x;
            txtDocumento.Enabled = x;
            cboEstadoCivil.Enabled = x;
            dtpFechaNacimiento.Enabled = x;
            rbtFemenino.Enabled = x;
            rbtMasculino.Enabled = x;
            chkFallecio.Enabled = x;
            btnGrabar.Enabled = x;
            btnCancelar.Enabled = x;
            btnNuevo.Enabled = !x;
            btnEditar.Enabled = !x;
            btnBorrar.Enabled = !x;
            btnSalir.Enabled = !x;
            lstPersonas.Enabled = !x;
        }

        private void limpiar() //Metodo para vaciar los campos, estableces los valores en 0
        {
            txtApellido.Text = "";
            txtNombres.Text = "";
            cboTipoDocumento.SelectedIndex = -1;
            txtDocumento.Text = "";
            cboEstadoCivil.SelectedIndex = -1;
            dtpFechaNacimiento.Value = DateTime.Today;
            rbtFemenino.Checked = false;
            rbtMasculino.Checked = false;
            chkFallecio.Checked = false;
        }
      
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;

            habilitar(true);
            limpiar();
            txtApellido.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            habilitar(true);
            txtDocumento.Enabled = false;
            txtApellido.Focus();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esta seguro de eliminar a" + aPersonas[lstPersonas.SelectedIndex].ToString() +"?", "BORRAR", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                //Delete de la persona seleccionada en la lista
                string strDelete = "DELETE FROM personas" 
                  + " WHERE documento = " + aPersonas[lstPersonas.SelectedIndex].ToString();

                conexion.Open();
                comando = new SqlCommand();
                comando.Connection = conexion;
                comando.CommandType = CommandType.Text;
                comando.CommandText = strDelete;
                comando.ExecuteNonQuery();
                conexion.Close();

                CargarLista(); //Vuelvo a actualizar la lista
            }



        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            
            limpiar();
            habilitar(false);
            esNuevo = false;
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            //Validar todos los datos
            if (ValidarDatos())
            {
                Persona p = new Persona(); //Crear objeto
                //Cargo el objeto
                p.Apellido = txtApellido.Text;
                p.Nombres = txtNombres.Text;
                p.TipoDocumento = Convert.ToInt32(cboTipoDocumento.SelectedValue);
                p.Documento = Convert.ToInt32(txtDocumento.Text);
                p.EstadoCivil = Convert.ToInt32(cboEstadoCivil.SelectedValue);
                if (rbtMasculino.Checked)
                    p.Sexo = 1;
                else
                    p.Sexo = 2;
                p.Fallecio = chkFallecio.Checked;
                p.FechaNacimiento = dtpFechaNacimiento.Value;
           


            if (esNuevo) 
                {
                    //HACEMOS INSERT SI ES NUEVO
                    if (!ExistePK(p.Documento))
                    {//COMPLETAR
                        string strInsert = "INSERT INTO personas(apellido, nombres, documento) VALUES('" + p.Apellido + "' , '" + p.Nombres + "' ,"+ p.Documento + ")";
                                                                                                                                                                                                                   

                        conexion.Open();
                        comando = new SqlCommand();
                        comando.Connection = conexion;
                        comando.CommandType = CommandType.Text;
                        comando.CommandText = strInsert;
                        comando.ExecuteNonQuery();
                        conexion.Close();
                    }

                }
            else
                {
                    //ACA HACEMOS UPDATE  
                    string strUpdate = "UPDATE personas SET apellido = '" + p.Apellido
                                      + "', nombres='" + p.Nombres
                                      + "' WHERE documento = " + p.Documento;

                    conexion.Open();
                    comando = new SqlCommand();
                    comando.Connection = conexion;
                    comando.CommandType = CommandType.Text;
                    comando.CommandText = strUpdate;
                    comando.ExecuteNonQuery();
                    conexion.Close();

                }
            }
                habilitar(false);
                esNuevo = false;
                CargarLista();
        }

        private bool ExistePK(int pk) //Validamos que la pk no se repita con un metodo
        {
            conexion.Open();
            comando = new SqlCommand(); 
            comando.Connection = conexion; 
            comando.CommandType = CommandType.Text;
            comando.CommandText = "SELECT * FROM personas WHERE documento =" + pk;
            DataTable tabla = new DataTable(); 
            tabla.Load(comando.ExecuteReader());
            conexion.Close();

            if (tabla.Rows.Count > 0)//Si el documento es mayor a 0 existe sino NO
                return true;
            else 
                return false;

        }

        private bool ValidarDatos()//Pregunto acerca de cada campo para validar, de lo contrario obligo a corregirlo e indico el lugar del error
        {
            if (txtApellido.Text == string.Empty)//Si no puso apellido le avisas que no lo puso y lo mandas ahi con un focus
            {
                MessageBox.Show("Debe ingresar un apellido");
                txtApellido.Focus();
                return false;
            }
            if (txtNombres.Text == string.Empty)//Si no puso nombre le avisas que no lo puso y lo mandas ahi con un focus
            {
                MessageBox.Show("Debe ingresar un nombre");
                txtNombres.Focus();
                return false;
            }
            if (cboTipoDocumento.SelectedIndex == -1)//Si no selecciono ningun elemento del cbo le aviso
            {
                MessageBox.Show("Debe elegir el tipo de Documento");
                cboTipoDocumento.Focus();
                return false;
            }
            if (txtDocumento.Text == string.Empty)//Si no puso documento le avisas que no lo puso y lo mandas ahi con un focus
            {
                MessageBox.Show("Debe ingresar un documento");
                txtDocumento.Focus();
                return false;
            }
            if (cboEstadoCivil.SelectedIndex == -1)//Si no selecciono ningun elemento del cbo le aviso
            {
                MessageBox.Show("Debe elegir el tipo de Documento");
                cboEstadoCivil.Focus();
                return false;
            }
            if(dtpFechaNacimiento.Value > DateTime.Now)
            {
                MessageBox.Show("Ingrese correctamente la fecha");
                dtpFechaNacimiento.Focus();
                return false;
            }
            if (!rbtMasculino.Checked && !rbtFemenino.Checked)
            {
                MessageBox.Show("Marque una opcion, Seleccione su sexo");
                rbtMasculino.Focus();
                return false;
            }

            return true;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seguro de abandonar la aplicación ?",
                "SALIR", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                this.Close();
        }

        private void lstPersonas_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCampos(lstPersonas.SelectedIndex); //Segun que tenga seleccionado es de lo que me va mostrar lso campos
        }

        private void CargarCampos(int posicion)
        {
            txtApellido.Text = aPersonas[posicion].Apellido;//Dame el apellido que este en esa posicion
            txtNombres.Text = aPersonas[posicion].Nombres;
            txtDocumento.Text = aPersonas[posicion].Documento.ToString();
            cboTipoDocumento.SelectedValue = aPersonas[posicion].TipoDocumento; //Le digo que se pare en el valor seleccionado en el combo box
            cboEstadoCivil.SelectedValue = aPersonas[posicion].EstadoCivil;
            dtpFechaNacimiento.Value = aPersonas[posicion].FechaNacimiento;//Le digo que tome el valor que esta en el arreglo
            chkFallecio.Checked = aPersonas[posicion].Fallecio;//Si esta checked lo valido
            if (aPersonas[posicion].Sexo == 1)//Si la persona tiene sexo 1, pongo un RBT Checked, sino pongo checked el otro
                rbtMasculino.Checked = true;
            else rbtFemenino.Checked = true;

        }
    }
}
