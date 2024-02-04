using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;  //for sql connection and query
using System.Configuration;  //to get data from app.config file

namespace EMS
{
    public partial class UpdatePassword : Form
    {
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        private Form previousForm;
        private string checkemail;
        private string verificationcode;
        private bool checkVerification = false;
        public UpdatePassword()
        {
            InitializeComponent();
        }
        public UpdatePassword(Form prevForm, string code, string email)
        {
            InitializeComponent();
            previousForm = prevForm;
            verificationcode = code;
            checkemail = email;
        }

        private bool CheckUserExists(OracleConnection con, string tableName, string email)
        {
            string sql = $"SELECT COUNT(*) FROM {tableName} WHERE email = :Email";

            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter("Email", email));

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void UpdatePasswordInDB(OracleConnection con, string tableName, string input_email, string newPassword)
        {
            string sql = $"UPDATE {tableName} SET password = :NewPassword WHERE email = :Email";

            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter("NewPassword", newPassword));
                cmd.Parameters.Add(new OracleParameter("Email", input_email));

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdatePasswordInadmin(OracleConnection con, string tableName, string input_email, string newPassword)
        {
            string sql = $"UPDATE {tableName} SET admin_password = :NewPassword WHERE email = :Email";

            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter("NewPassword", newPassword));
                cmd.Parameters.Add(new OracleParameter("Email", input_email));

                cmd.ExecuteNonQuery();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == verificationcode)
            {
                checkVerification = true;
            }
            else
            {
                errorProvider1.SetError(textBox1, "Incorrect Code");
                textBox1.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkVerification) 
            {
                errorProvider1.SetError(textBox1, "Please Verify");
                textBox1.Focus();
            }
            else
            {
                errorProvider1.Clear();
                con.Open();
                try
                {
                    // Update password in users table
                    UpdatePasswordInDB(con, "users", checkemail, textBox2.Text);

                    // Check and update in approval table
                    if (CheckUserExists(con, "approval", checkemail))
                    {
                        UpdatePasswordInDB(con, "approval", checkemail, textBox2.Text);
                    }

                    // Check and update in admin table
                    if (CheckUserExists(con, "admin", checkemail))
                    {
                        UpdatePasswordInadmin(con, "admin", checkemail, textBox2.Text);
                    }

                    MessageBox.Show("Password update process complete.");
                    this.Close();
                    previousForm.Show();
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
}
