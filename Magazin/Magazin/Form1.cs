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

namespace Magazin
{
    public partial class Form1 : Form
    {
        public class Comanda
        {
            public string product;
            public int quantity;
            public int price;

            public Comanda(string prod, int q, int p)
            {
                product = prod;
                quantity = q;
                price = p;
            }
        }
        Comanda[] produse = new Comanda[100];
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataReader reader;
        bool selectat = false, cantitate = false;
        int index, sum, i;
        string pret;

        public Form1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.SetData("DataDirectory", System.Environment.CurrentDirectory.Replace("bin\\Debug", ""));
            conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Magazin.mdf;Integrated Security=True");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("SELECT * FROM Stoc;", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader[1].ToString());
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            cantitate = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectat = true;
            try
            {
                conn.Open();
                cmd = new SqlCommand(String.Format("SELECT * FROM Stoc WHERE Denumire = '{0}';", comboBox1.SelectedItem.ToString()), conn);
                reader = cmd.ExecuteReader();
                reader.Read();
                pret = reader[3].ToString();
                textBox2.Text = String.Format("{0} lei pe bucata", pret);
                reader.Close();
                conn.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(selectat && cantitate)
            {
                try
                {
                    Comanda item = new Comanda(comboBox1.SelectedItem.ToString(), Convert.ToInt32(textBox1.Text), Convert.ToInt32(pret));
                    produse[index++] = item;
                }
                catch(Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Informatii insuficiente");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        { 
            if(index==0)
            {
                MessageBox.Show("Nu ai produse in cos");
            }
            for(i=0; i<index; i++)
            {
                sum = sum + produse[i].price * produse[i].quantity;
            }
            MessageBox.Show(sum.ToString());
            sum = 0;
            try
            {
                for (i = 0; i < index; i++)
                {
                    conn.Open();
                    cmd = new SqlCommand(String.Format("SELECT * FROM Stoc WHERE Denumire = '{0}';", produse[i].product), conn);
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    int val = Convert.ToInt32(reader[2]);
                    reader.Close();
                    cmd = new SqlCommand(String.Format("UPDATE Stoc SET Cantitate={0} WHERE Denumire='{1}';", (val-produse[i].quantity).ToString(), produse[i].product), conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                Array.Clear(produse, 0, index);
                index = 0;
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }
}
