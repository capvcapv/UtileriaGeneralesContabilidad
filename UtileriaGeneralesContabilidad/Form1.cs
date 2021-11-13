using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace UtileriaGeneralesContabilidad
{
    public partial class Form1 : Form
    {
        public List<registro> listaRegistros = new List<registro>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox4.Text = folderBrowserDialog1.SelectedPath;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            DirectoryInfo info = new DirectoryInfo(textBox4.Text);
            FileInfo[] files = info.GetFiles("*.mdf");

            for (int i = 0; i < files.Length; i++)
            {

                obtenerID((((FileInfo)files[i]).Name).Replace(".mdf",""));

            }

            dataGridView1.DataSource = listaRegistros;

        }

        //INSERT INTO ListaEmpresas(Id,RowVersion,Nombre,RutaDatos,RutaResp,AliasBDD,HashSchema,ModulosIntegrados,VersionBDD) VALUES (514,1211568637,'ctABBC','localhost','C:\Compac\Empresas\ctABBC\','ctABBC',0,'CT','1022')

        private void obtenerID(string bd)
        {
            SqlConnection conn = new SqlConnection("Server="+ textBox1.Text +";Database=" + bd + ";User Id=" + textBox2.Text + ";Password = " + textBox3.Text + ";");
            conn.Open();

            SqlCommand comando = new SqlCommand("select IdEmpresa from Parametros where Id=1", conn);
            SqlDataReader lector = comando.ExecuteReader();

            try
            {
                lector.Read();
                var reg = new registro();
                reg.basedatos = bd;
                reg.id = lector.GetValue(0).ToString();
                listaRegistros.Add(reg);
                conn.Close();
                conn.Dispose();
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
                conn.Close();
                conn.Dispose();
            }
            
        }

        private void altaGenerales(registro tmp)
        {
            try
            {
                SqlConnection conn = new SqlConnection("Server=" + textBox1.Text + ";Database=GeneralesSQL;User Id=" + textBox2.Text + ";Password = " + textBox3.Text + ";");
                conn.Open();

                SqlCommand comando = new SqlCommand(@"INSERT INTO ListaEmpresas(Id,RowVersion,Nombre,RutaDatos,RutaResp,AliasBDD,HashSchema,ModulosIntegrados,VersionBDD) VALUES (" + tmp.id + ",1211568637,'" + tmp.basedatos + @"','localhost','C:\Compac\Empresas\" + tmp.basedatos + @"\','" + tmp.basedatos + "',0,'CT','1022')", conn);
                comando.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        private void adjuntaBD(string nombre, string mdf, string ldf)
        {
            try
            {
                SqlConnection conn = new SqlConnection("Server=" + textBox1.Text + ";Database=master;User Id=" + textBox2.Text + ";Password = " + textBox3.Text + ";");
                conn.Open();

                SqlCommand comando = new SqlCommand("sp_attach_db", conn);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@dbname", nombre);
                comando.Parameters.AddWithValue("@filename1", mdf);
                comando.Parameters.AddWithValue("@filename2", ldf);

                comando.ExecuteNonQuery();

                comando.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch(Exception e)
            {

                richTextBox1.Text = richTextBox1.Text + "Error de carga: " + e.Message + System.Environment.NewLine;
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {

            DirectoryInfo info = new DirectoryInfo(textBox5.Text);
            FileInfo[] files = info.GetFiles("*.mdf");

            richTextBox1.Text = richTextBox1.Text + "Iniciando proceso de carga de " + files.Length.ToString() + " base de datos" + System.Environment.NewLine;

            for (int i = 0; i < files.Length; i++)
            {

                richTextBox1.Text = richTextBox1.Text + "Iniciando de carga de " + ((FileInfo)files[i]).FullName + System.Environment.NewLine;

                if ( ((FileInfo)files[i]).FullName.Contains("document") || ((FileInfo)files[i]).FullName.Contains("other"))
                {
                    if(File.Exists((((FileInfo)files[i]).FullName).Replace(".mdf", ".LDF")))
                    {
                        adjuntaBD((((FileInfo)files[i]).Name).Replace(".mdf", ""), ((FileInfo)files[i]).FullName, (((FileInfo)files[i]).FullName).Replace(".mdf", ".LDF"));
                    }
                    else
                    {
                        adjuntaBD((((FileInfo)files[i]).Name).Replace(".mdf", ""), ((FileInfo)files[i]).FullName, (((FileInfo)files[i]).FullName).Replace(".mdf", "_log.LDF"));
                    }
                    
                }
                else
                {
                    adjuntaBD((((FileInfo)files[i]).Name).Replace(".mdf", ""), ((FileInfo)files[i]).FullName, (((FileInfo)files[i]).FullName).Replace(".mdf", "_log.LDF"));
                }

                

            }

            MessageBox.Show("Adjunte lo que pude");


        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(var a in listaRegistros)
            {
                altaGenerales(a);
            }

            MessageBox.Show("Terminado");
        }
    }
}
