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
    public partial class ViewUsers : Form
    {
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        private Form previousForm;
        public ViewUsers()
        {
            InitializeComponent();
        }
        public ViewUsers(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            previousForm.Show();
        }

        private void ViewUsers_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                string query = "SELECT full_name, email, join_date FROM users";

                OracleCommand cmd = new OracleCommand(query, con);
                OracleDataAdapter adapter = new OracleDataAdapter(cmd);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataTable.Columns["full_name"].ColumnName = "Name";
                dataTable.Columns["email"].ColumnName = "Email";
                dataTable.Columns["join_date"].ColumnName = "Join Date";

                dataGridView1.DataSource = dataTable;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
