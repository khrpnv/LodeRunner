using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static Random rnd = new Random();
        public int number = rnd.Next(1, 100);
        public int maxNumber;
        public int lives = 0;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {
            Random rnd = new Random();
            number = rnd.Next(1,maxNumber);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lives == 0)
            {
                label5.Text = "YOU LOOSE!";
                return;
            }
            int inputNum = Convert.ToInt32(textBox1.Text);
            if (inputNum < number)
            {
                label5.Text = "The number is GREATER";
                lives--;
                label2.Text = Convert.ToString(lives);
            }
            else if (inputNum > number)
            {
                label5.Text = "The number is LESS";
                lives--;
                label2.Text = Convert.ToString(lives);
            }
            else
            {
                MessageBox.Show("YOU WIN!");
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            if (textBox1.Text != "" && Convert.ToInt32(textBox1.Text) >= 20)
                maxNumber = Convert.ToInt32(textBox1.Text);
            else maxNumber = 100;
            lives = (int)Math.Log(maxNumber, 2) + 1;
            button1.Visible = true;
            button1.Enabled = true;
            button2.Visible = true;
            button2.Enabled = true;
            button3.Visible = true;
            button3.Enabled = true;
            button4.Visible = false;
            button4.Enabled = false;
            label1.Visible = true;
            label3.Visible = false;
            label5.Text = "";
            label2.Text = Convert.ToString(lives);
            label4.Visible = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
