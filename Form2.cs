using System;
using System.Drawing;
using System.Windows.Forms;

namespace L5
{
    public partial class Form2 : Form
    {
        public Form2(Image img)
        {
            InitializeComponent();
            TopMost = true;
            pictureBox1.Image = img;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
