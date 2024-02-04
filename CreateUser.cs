using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;  //for mail format verification   
using System.Net.Mail;   //for email codes
using System.Net;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace EMS
{
    public partial class CreateUser : Form
    {
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        private Form previousForm;
        string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
        public CreateUser()
        {
            InitializeComponent();
        }
        public CreateUser(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private bool CheckUserExists(string username, string email)
        {
            try
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM users WHERE username = :username OR email = :email";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = username;
                    cmd.Parameters.Add(new OracleParameter("email", OracleDbType.Varchar2)).Value = email;

                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return userCount > 0;
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            previousForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Generate a 4-digit verification code
            Random random = new Random();
            string verificationCode = random.Next(1000, 9999).ToString();

            if(CheckUserExists(textBox1.Text,textBox4.Text))
            {
                MessageBox.Show("User already exists!");
            }
            else 
            {
                try
                {
                    // Set up SMTP client
                    SmtpClient client = new SmtpClient("smtp-relay.brevo.com"); // Replace with your SMTP server
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("f219331@cfd.nu.edu.pk", "qJw6gSPM4fG0bs7X");
                    client.EnableSsl = true;
                    client.Port = 587; // Standard port for SMTP, adjust if needed

                    // Create and send the email
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("f219331@cfd.nu.edu.pk");
                    mailMessage.To.Add(textBox4.Text); // Replace with the recipient's email address
                    mailMessage.Body = $"Your verification code is: {verificationCode}";
                    mailMessage.Subject = "Email Verification";
                    client.Send(mailMessage);

                    // Show EmailVerification form and hide current form
                    EmailVerification emailverification = new EmailVerification(previousForm, this, verificationCode, textBox5.Text, textBox2.Text, textBox1.Text, textBox3.Text, textBox4.Text);
                    emailverification.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error in sending email: {ex.Message}");
                }
            }
        }


        private void textBox5_Leave(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox5.Text) == true)
            {
                textBox5.Focus();
                errorProvider1.SetError(this.textBox5, "Please fill this feild!");
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text) == true)
            {
                textBox2.Focus();
                errorProvider2.SetError(this.textBox2, "Please fill this feild!");
            }
            else
            {
                errorProvider2.Clear();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) == true)
            {
                textBox1.Focus();
                errorProvider3.SetError(this.textBox1, "Please fill this feild!");
            }
            else
            {
                errorProvider3.Clear();
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text) == true)
            {
                textBox3.Focus();
                errorProvider4.SetError(this.textBox3, "Please fill this feild!");
            }
            else
            {
                errorProvider4.Clear();
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox5.Text) == true)
            {
                textBox4.Focus();
                errorProvider5.SetError(this.textBox4, "Please fill this feild!");
            }
            else
            {
                errorProvider5.Clear();
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBox4.Text, pattern) == false)
            {
                textBox4.Focus();
                errorProvider5.SetError(this.textBox4, "Invalid Email!");
            }
            else
            {
                errorProvider5.Clear();
            }
        }
    }
}