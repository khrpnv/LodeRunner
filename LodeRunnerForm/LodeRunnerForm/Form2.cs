using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LodeRunnerForm
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            button2.Enabled = false;
            textBox1.Visible = false;
            textBox1.Enabled = false;
            textBox2.Visible = false;
            textBox2.Enabled = false;
            textBox3.Visible = false;
            textBox3.Enabled = false;
            pictureBox1.Visible = false;
            label2.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = true;
            LevelsCreator redactor = new LevelsCreator();
            if (textBox2.Text.Length <= 1)
                textBox2.Text = "NewLevel";
            redactor.RedactorProcess(this, Convert.ToInt16(textBox1.Text), Convert.ToInt16(textBox3.Text), textBox2.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int width = int.Parse(textBox1.Text);
            textBox1.MaxLength = 2;
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int height = int.Parse(textBox3.Text);
            textBox3.MaxLength = 2;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
