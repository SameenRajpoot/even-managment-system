using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EMS
{
    public partial class RemoveSpeaker : Form
    {
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        private Form previousForm;
        private bool checkEvent = false;
        private string event_id;
        public RemoveSpeaker()
        {
            InitializeComponent();
        }
        public RemoveSpeaker(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            checkEvent = true;
            comboBox2.Items.Clear();
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
                comboBox2.Items.Clear();
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

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            if(checkEvent == true)
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                string sql = "SELECT Event_ID FROM EVENT WHERE EVENT_NAME = :EventName";
                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("EventName", comboBox1.Text));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            event_id = reader["Event_ID"].ToString();
                        }
                    }
                }
                string sql2 = "SELECT Speaker_fname, Speaker_lname FROM speaker WHERE Event_ID = :EventId";
                using (OracleCommand cmd = new OracleCommand(sql2, con))
                {
                    cmd.Parameters.Add(new OracleParameter("EventId", event_id));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        comboBox2.Items.Clear(); // Clear existing items

                        while (reader.Read())
                        {
                            string speakerName = reader["Speaker_fname"].ToString() + " " + reader["Speaker_lname"].ToString();
                            comboBox2.Items.Add(speakerName);
                        }

                        if (comboBox2.Items.Count > 0)
                            comboBox2.SelectedIndex = 0;
                        else
                            MessageBox.Show("No speakers found for this event.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Select Event!!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                // Splitting the concatenated speaker name into first and last name
                string[] nameParts = comboBox2.Text.Split(new[] { ' ' }, 2);
                string speakerFname = nameParts[0];
                string speakerLname = nameParts.Length > 1 ? nameParts[1] : "";

                // SQL to delete the record
                string sql = "DELETE FROM speaker WHERE Event_ID = :EventId AND Speaker_fname = :SpeakerFname AND Speaker_lname = :SpeakerLname";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    // Using parameterized query to prevent SQL injection
                    cmd.Parameters.Add(new OracleParameter("EventId", event_id));
                    cmd.Parameters.Add(new OracleParameter("SpeakerFname", speakerFname));
                    cmd.Parameters.Add(new OracleParameter("SpeakerLname", speakerLname));

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Speaker record deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Speaker record not found.");
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
    }
}
