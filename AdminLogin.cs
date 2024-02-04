using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace EMS
{
    public partial class AdminLogin : Form
    {
        private Form previousForm;
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public AdminLogin()
        {
            InitializeComponent();
        }
        public AdminLogin(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            previousForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM admin WHERE admin_username = :username AND admin_password = :password";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = textBox1.Text;
                    cmd.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = textBox2.Text;

                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (userCount > 0)
                    {
                        MessageBox.Show("Login successful.");
                        textBox1.Clear();
                        textBox2.Clear();
                        AdminPanel adminPanel = new AdminPanel(this);
                        adminPanel.Show();
                        this.Hide();
                        con.Close();
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            applyForAdmin applyForAdmin = new applyForAdmin(this);
            applyForAdmin.Show();
            this.Hide();
        }
    }
}