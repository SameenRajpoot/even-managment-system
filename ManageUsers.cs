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
    public partial class ManageUsers : Form
    {
        private Form previousForm;
        public ManageUsers()
        {
            InitializeComponent();
        }
        public ManageUsers(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserPermission userPermission = new UserPermission(this);
            userPermission.Show();
            this.Hide();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ViewUsers viewUsers = new ViewUsers(this);
            viewUsers.Show();
            this.Hide();
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Black;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Red;
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.ForeColor = Color.Black;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.ForeColor = Color.Lime;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Black;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Lime;
        }
    }
}
