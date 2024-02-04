using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace EMS
{
    public partial class EmailVerification : Form
    {
        private Form previousForm;
        private Form userLogin;
        private string fname, lname, usrname, pass, email;
        private string verificationCode;
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        public EmailVerification()
        {
            InitializeComponent();
        }
        public EmailVerification(Form login,Form prevForm, string code, string Fname, string Lname, string Usrname, string Pass, string Email)
        {
            InitializeComponent();
            previousForm = prevForm;
            userLogin = login;
            verificationCode = code;
            fname = Fname;
            lname = Lname;
            usrname = Usrname;
            pass = Pass;
            email = Email;
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) == true)
            {
                textBox1.Focus();
                errorProvider1.SetError(this.textBox1, "Please fill this feild!");
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            previousForm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == verificationCode)
            {
                string fullName = fname + " " + lname;
                string query = "INSERT INTO users (username, password, email, Full_Name, Join_Date) VALUES (:usrname, :pass, :email, :full_Name, :Join_Date)";

                try
                {
                    con.Open();

                    using (OracleCommand command = new OracleCommand(query, con))
                    {
                        command.Parameters.Add("username", OracleDbType.Varchar2).Value = usrname;
                        command.Parameters.Add("password", OracleDbType.Varchar2).Value = pass;
                        command.Parameters.Add("email", OracleDbType.Varchar2).Value = email;
                        command.Parameters.Add("Full_Name", OracleDbType.Varchar2).Value = fullName;
                        command.Parameters.Add("Join_Date", OracleDbType.Date).Value = DateTime.Now; 

                        command.ExecuteNonQuery();
                        MessageBox.Show("User registered successfully.");
                        userLogin.Show();
                        this.Close();
                        previousForm.Close();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Verification code does not match.");
            }
        }
    }
}
