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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace EMS
{
    public partial class UserPermission : Form
    {
        private Form previousForm;
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public UserPermission()
        {
            InitializeComponent();
        }
        public UserPermission(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void LoadDataIntoDataGridView()
        {
            try
            {
                con.Open();
                string sql = "SELECT full_name, username, email, password FROM approval";

                using (OracleDataAdapter adapter = new OracleDataAdapter(sql, con))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.Rows.Clear();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int rowIndex = dataGridView1.Rows.Add();
                        dataGridView1.Rows[rowIndex].Cells["select"].Value = false;
                        dataGridView1.Rows[rowIndex].Cells["name"].Value = row["full_name"];
                        dataGridView1.Rows[rowIndex].Cells["username"].Value = row["username"];
                        dataGridView1.Rows[rowIndex].Cells["email"].Value = row["email"];
                        dataGridView1.Rows[rowIndex].Cells["password"].Value = row["password"];
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
            con.Open();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // Skip the new row at the end which is not committed

                bool isSelected = Convert.ToBoolean(row.Cells["select"].Value);
                if (isSelected)
                {
                    string name = row.Cells["name"].Value.ToString();
                    string username = row.Cells["username"].Value.ToString();
                    string email = row.Cells["email"].Value.ToString();
                    string password = row.Cells["password"].Value.ToString();

                    // Transaction ensures that both operations (insert and delete) are completed successfully
                    using (OracleTransaction trans = con.BeginTransaction())
                    {
                        try
                        {
                            // Insert into admin table
                            string sqlInsertAdmin = "INSERT INTO admin (admin_name,admin_username, admin_password, email) VALUES (:name, :username, :password, :email)";
                            using (OracleCommand cmdInsertAdmin = new OracleCommand(sqlInsertAdmin, con))
                            {
                                cmdInsertAdmin.Parameters.Add(new OracleParameter("name", OracleDbType.Varchar2)).Value = name;
                                cmdInsertAdmin.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = username;
                                cmdInsertAdmin.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = password;
                                cmdInsertAdmin.Parameters.Add(new OracleParameter("email", OracleDbType.Varchar2)).Value = email;
                                cmdInsertAdmin.Transaction = trans;
                                cmdInsertAdmin.ExecuteNonQuery();
                            }

                            // Delete from approval table
                            string sqlDeleteApproval = "DELETE FROM approval WHERE username = :username";
                            using (OracleCommand cmdDeleteApproval = new OracleCommand(sqlDeleteApproval, con))
                            {
                                cmdDeleteApproval.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = username;
                                cmdDeleteApproval.Transaction = trans;
                                cmdDeleteApproval.ExecuteNonQuery();
                            }
                            trans.Commit();
                            MessageBox.Show("Access Granted");
                            con.Close();
                            //mailing the user
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
                                mailMessage.To.Add(email); // Replace with the recipient's email address
                                mailMessage.Body = $"Your request for admin approval has been approved";
                                mailMessage.Subject = "Approval Granted";
                                client.Send(mailMessage);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error in sending email: {ex.Message}");
                            }
                        }
                        catch (OracleException)
                        {
                            trans.Rollback();
                            throw; // Rethrow the exception to be caught by the outer catch block
                        }
                    }
                }
            }
            // Refresh the DataGridView to show updated data
            LoadDataIntoDataGridView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            this.Hide();
        }

        private void UserPermission_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Black;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Lime;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Black;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Lime;
        }
    }
}
