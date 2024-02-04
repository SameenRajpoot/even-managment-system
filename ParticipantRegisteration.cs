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
using System.Net.Mail;   //for email codes
using System.Net;

namespace EMS
{
    public partial class ParticipantRegisteration : Form
    {
        private Form previousForm;
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        public ParticipantRegisteration()
        {
            InitializeComponent();
        }
        public ParticipantRegisteration(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string sql = "SELECT EVENT_NAME FROM EVENT";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    comboBox2.Items.Clear();

                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader["event_name"].ToString());
                    }
                }
                con.Close();
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

        private void button3_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            this.Close();
        }

        private bool IsEmailPresent(string email)
        {
            bool emailExists = false;

            try
            {
                con.Open();

                string sql = "SELECT COUNT(*) FROM participant WHERE email = :email";
                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("email", OracleDbType.Varchar2)).Value = email;

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    if (count > 0)
                    {
                        emailExists = true;
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);
            }
            return emailExists;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            con.Open();

            if (IsEmailPresent(textBox3.Text) == false) //not registered 
            {
                try
                {
                    string fname = textBox1.Text;
                    string lname = textBox2.Text;
                    string email = textBox3.Text;
                    string eventName = comboBox2.SelectedItem.ToString();

                    // Construct the SQL query to get the event ID
                    string queryGetId = "SELECT event_id FROM event WHERE event_name = :EventName";
                    OracleCommand cmdGetId = new OracleCommand(queryGetId, con);
                    cmdGetId.Parameters.Add(new OracleParameter("EventName", eventName));

                    string eventId = cmdGetId.ExecuteScalar()?.ToString(); // Execute query and get event ID

                    if (eventId != null)
                    {
                        string fullName = fname + " " + lname;

                        // Construct the SQL query to insert data into participant table
                        string queryInsertParticipant = "INSERT INTO participants (participant_name, email, event_id) VALUES (:FullName, :Email, :EventId)";
                        OracleCommand cmdInsertParticipant = new OracleCommand(queryInsertParticipant, con);

                        cmdInsertParticipant.Parameters.Add(new OracleParameter("FullName", fullName));
                        cmdInsertParticipant.Parameters.Add(new OracleParameter("Email", email));
                        cmdInsertParticipant.Parameters.Add(new OracleParameter("EventId", eventId));

                        cmdInsertParticipant.ExecuteNonQuery();

                        MessageBox.Show("Participant added successfully.");

                        // Set up SMTP client
                        SmtpClient client = new SmtpClient("smtp-relay.brevo.com"); // Replace with your SMTP server
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential("f219331@cfd.nu.edu.pk", "qJw6gSPM4fG0bs7X");
                        client.EnableSsl = true;
                        client.Port = 587; // Standard port for SMTP, adjust if needed

                        // Create and send the email
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress("f219331@cfd.nu.edu.pk");
                        mailMessage.To.Add(textBox3.Text); // Replace with the recipient's email address
                        mailMessage.Body = $"Your registeration has been confirmed in: {comboBox2.Text}";
                        mailMessage.Subject = "Participant Conformation";
                        client.Send(mailMessage);
                    }
                    else
                    {
                        MessageBox.Show("Event not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            else  //registered already
            {
                MessageBox.Show("This email has already been registered");
            }
        }
    }
}

