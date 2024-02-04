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
using System.Net.Mail;   //for email codes
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EMS
{
    public partial class AddSpeaker : Form
    {
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        private Form previousForm;
        public AddSpeaker()
        {
            InitializeComponent();
        }
        public AddSpeaker(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            this.Close();
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                string sql = "SELECT EVENT_NAME FROM EVENT";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    comboBox1.Items.Clear();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["event_name"].ToString());
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

        private bool DoesSpeakerExistForEvent(string fname, string lname, string email, string eventId)
        {
            string queryCheckSpeaker = "SELECT COUNT(*) FROM speaker WHERE speaker_fname = :FName AND speaker_lname = :LName AND email = :Email AND event_id = :EventId";
            using (OracleCommand cmdCheckSpeaker = new OracleCommand(queryCheckSpeaker, con))
            {
                cmdCheckSpeaker.Parameters.Add(new OracleParameter("FName", fname));
                cmdCheckSpeaker.Parameters.Add(new OracleParameter("LName", lname));
                cmdCheckSpeaker.Parameters.Add(new OracleParameter("Email", email));
                cmdCheckSpeaker.Parameters.Add(new OracleParameter("EventId", eventId));

                int count = Convert.ToInt32(cmdCheckSpeaker.ExecuteScalar());
                return count > 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string eventName = comboBox1.SelectedItem.ToString();
                string queryGetEventId = "SELECT event_id FROM event WHERE event_name = :EventName";
                OracleCommand cmdGetEventId = new OracleCommand(queryGetEventId, con);
                cmdGetEventId.Parameters.Add(new OracleParameter("EventName", eventName));
                string eventId = cmdGetEventId.ExecuteScalar()?.ToString();

                if (eventId != null)
                {
                    string fname = textBox1.Text;
                    string lname = textBox2.Text;
                    string email = textBox3.Text;

                    if(DoesSpeakerExistForEvent(fname, lname, email, eventId)) 
                    {
                        MessageBox.Show("Speaker is already registered for this event.");
                        return;
                    }

                    string queryInsertSpeaker = "INSERT INTO speaker (speaker_fname, speaker_lname, email, event_id) VALUES (:FName, :LName, :Email, :EventId)";
                    using (OracleCommand cmdInsertSpeaker = new OracleCommand(queryInsertSpeaker, con))
                    {
                        cmdInsertSpeaker.Parameters.Add(new OracleParameter("FName", fname));
                        cmdInsertSpeaker.Parameters.Add(new OracleParameter("LName", lname));
                        cmdInsertSpeaker.Parameters.Add(new OracleParameter("Email", email));
                        cmdInsertSpeaker.Parameters.Add(new OracleParameter("EventId", eventId));
                        cmdInsertSpeaker.ExecuteNonQuery();
                    }
                    MessageBox.Show("Speaker data inserted successfully.");

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
                    mailMessage.Body = $"Your participation as a speaker has been confirmed for: {eventName}";
                    mailMessage.Subject = "Email Verification";
                    client.Send(mailMessage);
                    this.Close();
                    previousForm.Show();
                }
                else
                {
                    MessageBox.Show("Selected event not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
