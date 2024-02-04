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
    public partial class applyForAdmin : Form
    {
        private Form previousForm;
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        public applyForAdmin()
        {
            InitializeComponent();
        }
        public applyForAdmin(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                // First, check if the user exists in the admin table
                string sqlCheckAdmin = "SELECT email FROM admin WHERE email = (SELECT email FROM users WHERE username = :username AND password = :password)";
                using (OracleCommand cmdCheckAdmin = new OracleCommand(sqlCheckAdmin, con))
                {
                    cmdCheckAdmin.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = textBox1.Text;
                    cmdCheckAdmin.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = textBox2.Text;

                    using (OracleDataReader adminReader = cmdCheckAdmin.ExecuteReader())
                    {
                        if (adminReader.Read())
                        {
                            MessageBox.Show("User is already an admin.");
                            return; 
                        }
                    }
                }

                string sqlCheckUser = "SELECT email, full_name FROM users WHERE username = :username AND password = :password";
                using (OracleCommand cmdCheckUser = new OracleCommand(sqlCheckUser, con))
                {
                    cmdCheckUser.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = textBox1.Text;
                    cmdCheckUser.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = textBox2.Text;

                    using (OracleDataReader reader = cmdCheckUser.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string email = reader["email"].ToString();
                            string fullName = reader["full_name"].ToString(); // Retrieve the full name

                            string sqlCheckApproval = "SELECT COUNT(*) FROM approval WHERE username = :username";
                            using (OracleCommand cmdCheckApproval = new OracleCommand(sqlCheckApproval, con))
                            {
                                cmdCheckApproval.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = textBox1.Text;
                                int userExists = Convert.ToInt32(cmdCheckApproval.ExecuteScalar());

                                if (userExists == 0)
                                {
                                    // User not in approval table, insert new record
                                    string sqlInsertApproval = "INSERT INTO approval (username, password, email, full_name) VALUES (:username, :password, :email, :full_name)";
                                    using (OracleCommand cmdInsertApproval = new OracleCommand(sqlInsertApproval, con))
                                    {
                                        cmdInsertApproval.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2)).Value = textBox1.Text;
                                        cmdInsertApproval.Parameters.Add(new OracleParameter("password", OracleDbType.Varchar2)).Value = textBox2.Text;
                                        cmdInsertApproval.Parameters.Add(new OracleParameter("email", OracleDbType.Varchar2)).Value = email;
                                        cmdInsertApproval.Parameters.Add(new OracleParameter("full_name", OracleDbType.Varchar2)).Value = fullName; // Add the full name

                                        cmdInsertApproval.ExecuteNonQuery();
                                        MessageBox.Show("Your request has been submitted. You will be notified via email upon acceptance.");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Your request has already been submitted. You will be notified upon acceptance");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid Username or Passwrord");
                        }
                    }
                }
                con.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                con.Close();
            }

        }
    }
}