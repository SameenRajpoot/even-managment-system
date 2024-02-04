using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMS
{
    public partial class AdminPanel : Form
    {
        private Form previousForm;
        public AdminPanel()
        {
            InitializeComponent();
        }
        public AdminPanel(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EventCreation eventCreation = new EventCreation(this);
            eventCreation.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ManageUsers manageUsers = new ManageUsers(this);
            manageUsers.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ManageSpeakers manageSpeakers = new ManageSpeakers(this);
            manageSpeakers.Show();
            this.Hide();
        }
    }
}
