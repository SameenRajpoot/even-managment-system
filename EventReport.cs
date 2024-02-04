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
    public partial class EventReport : Form
    {
        private Form previousForm;
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public EventReport()
        {
            InitializeComponent();
        }
        public EventReport(Form prevForm)
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Please select an event.");
                    return;
                }

                string selectedEventName = comboBox1.SelectedItem.ToString();
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                string sql = @"
                SELECT
                    e.event_name,
                    COUNT(p.participant_id) AS total_participants,
                    s.speaker_fname || ' ' || s.speaker_lname AS speaker_name,
                    e.location,
                    e.event_date
                FROM
                    event e
                JOIN
                    participants p ON e.event_id = p.event_id
                JOIN
                    speaker s ON e.event_id = s.event_id
                WHERE
                    e.event_name = :EventName
                GROUP BY
                    e.event_id, e.event_name, s.speaker_fname, s.speaker_lname, e.location, e.event_date";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("EventName", selectedEventName));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        dataGridView1.Rows.Clear();

                        while (reader.Read())
                        {
                            int rowIndex = dataGridView1.Rows.Add();
                            dataGridView1.Rows[rowIndex].Cells["event_name"].Value = reader["event_name"].ToString();
                            dataGridView1.Rows[rowIndex].Cells["participant_count"].Value = reader["total_participants"].ToString();
                            dataGridView1.Rows[rowIndex].Cells["speaker_name"].Value = reader["speaker_name"].ToString();
                            dataGridView1.Rows[rowIndex].Cells["event_location"].Value = reader["location"].ToString();
                            dataGridView1.Rows[rowIndex].Cells["date"].Value = reader["event_date"].ToString();
                        }
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
