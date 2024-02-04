using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace EMS
{
    public partial class UserLogin : Form
    {
        private Form previousForm;
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        public UserLogin()
        {
            InitializeComponent();
        }
        public UserLogin(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }
        private void UserLogin_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            previousForm.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateUser createUser = new CreateUser(this);
            createUser.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {

           
            try
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM users WHERE username = :username AND password = :password";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = textBox1.Text;
                    cmd.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = textBox2.Text;

                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Clone();

                    if (userCount > 0)
                    {
                        MessageBox.Show("Login successful.");
                        textBox1.Clear();
                        textBox2.Clear();
                        UserPanel userPanel = new UserPanel(this);
                        userPanel.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Login failed. Username or password is incorrect.");
                        con.Close();
                    }

                   
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
             if(string.IsNullOrEmpty(textBox1.Text))
             {
                 textBox1.Focus();
                 errorProvider1.SetError(this.textBox1, "Please fill this feild!");
             }
             else
             {
                 errorProvider1.Clear();
             } 

           
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
             if (string.IsNullOrEmpty(textBox2.Text))
             {
                 textBox2.Focus();
                 errorProvider2.SetError(this.textBox2, "Please fill this feild!");
             }
             else
             {
                 errorProvider2.Clear();
             } 

            


        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                textBox1.ForeColor = Color.White;
                
            }

            catch
            {

            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {

            button1.ForeColor = Color.Black;

        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {

            button1.ForeColor = Color.Lime;

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.ForeColor = Color.White;
           

        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {

            button3.ForeColor = Color.Black;

        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {

            button3.ForeColor = Color.Red;

        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.SelectAll();
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPassword forgotPassword = new ForgotPassword(this);
            this.Hide();
            forgotPassword.Show();
        }
    }
}
