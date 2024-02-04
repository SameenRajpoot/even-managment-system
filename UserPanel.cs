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
    public partial class UserPanel : Form
    {
        Form previousForm;
        public UserPanel()
        {
            InitializeComponent();
        }
        public UserPanel(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;    
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            previousForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EventReport eventReport = new EventReport(this);
            eventReport.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ParticipantRegisteration participantRegisteration = new ParticipantRegisteration(this);
            participantRegisteration.Show();
            this.Hide();
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Black;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Black;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Black;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Lime;
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Black;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Lime;
        }
    }
}
