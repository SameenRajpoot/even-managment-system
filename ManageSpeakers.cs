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
    public partial class ManageSpeakers : Form
    {
        Form previousForm;
        public ManageSpeakers()
        {
            InitializeComponent();
        }
        public ManageSpeakers(Form prevForm)
        {
            InitializeComponent();
            previousForm = prevForm;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddSpeaker addSpeaker = new AddSpeaker(this);
            addSpeaker.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RemoveSpeaker removeSpeaker = new RemoveSpeaker(this);
            removeSpeaker.Show();
            this.Hide();
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Black;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Lime;
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Black;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Lime;
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Black;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Lime;
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.ForeColor = Color.Black;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.ForeColor = Color.Lime;
        }
    }
}
