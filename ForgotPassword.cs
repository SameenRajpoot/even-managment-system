using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;  //checking email format
using Oracle.ManagedDataAccess.Client;  //for sql connection and query
using System.Configuration;  //to get data from app.config file
using System.Net.Mail; //for mail
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;

namespace EMS
{
    public partial class ForgotPassword : Form
    {
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        private Form previousForm;
        string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
        public ForgotPassword()
        {
            InitializeComponent();
        }
        public ForgotPassword(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private bool CheckEmailExists(string email)
        {
            bool exists = false;

            try
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM users WHERE email = :Email";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("Email", email));
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        exists = true;
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
            return exists;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(CheckEmailExists(textBox2.Text) == true)
            {
                //send mail code
                Random random = new Random();
                string verificationCode = random.Next(1000, 9999).ToString();
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
                    mailMessage.To.Add(textBox2.Text); // Replace with the recipient's email address
                    mailMessage.Body = $"Your verification code for password reset is: {verificationCode}";
                    mailMessage.Subject = "Password Reset Confirmation";
                    client.Send(mailMessage);

                    UpdatePassword updatePassword = new UpdatePassword(this,verificationCode,textBox2.Text);
                    this.Hide();
                    updatePassword.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error in sending email: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("This User doesnt exist!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            previousForm.Show();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(Regex.IsMatch(textBox2.Text,pattern) == false)
            {
                textBox2.Focus();
                errorProvider1.SetError(textBox2, "Invalid Email");
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void ForgotPassword_Load(object sender, EventArgs e)
        {

        }
    }
}
