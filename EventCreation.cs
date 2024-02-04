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
using System.Xml.Linq;

namespace EMS
{
    public partial class EventCreation : Form
    {
        private Form previousForm;
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        public EventCreation()
        {
            InitializeComponent();
        }
        public EventCreation(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            previousForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string sql = "INSERT INTO EVENT (EVENT_NAME, LOCATION, EVENT_DATE, DESCRIPTION) VALUES (:name, :location, :event_date, :description)";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("name", OracleDbType.Varchar2)).Value = textBox1.Text;
                    cmd.Parameters.Add(new OracleParameter("location", OracleDbType.Varchar2)).Value = textBox2.Text;
                    cmd.Parameters.Add(new OracleParameter("event_date", OracleDbType.Date)).Value = dateTimePicker1.Value;
                    cmd.Parameters.Add(new OracleParameter("description", OracleDbType.Varchar2)).Value = richTextBox1.Text;

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Event added successfully.");
                    this.Close();
                    previousForm.Show();
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
