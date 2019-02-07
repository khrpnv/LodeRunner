using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private static int _time;
        private bool _onceShow = true;
        private static int[] code = new int[4];
        public Form1()
        {
            InitializeComponent();
            _time = 45;
            GenerateCode();
            timer1.Start();
            label3.Font = new Font("Arial", 12);
            label4.Font = new Font("Arial", 12);
            label5.Font = new Font("Arial", 12);
            label6.Font = new Font("Arial", 12);
        }
        private static void GenerateCode()
        {
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                int number = rnd.Next(1, 10);
                code[i] = number;
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_time == 0 && _onceShow)
            {
                _onceShow = false;
                MessageBox.Show("YOU LOOSE!");
                Close();
            }
            else if (CheckResults() && _onceShow)
            {
                _onceShow = false;
                MessageBox.Show("YOU WIN!");
                Close();
            }
            _time--;
            label2.Text = "" + _time;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void label5_Click(object sender, EventArgs e)
        {

        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Check(numericUpDown1, label3, 0);
            Check(numericUpDown2, label4, 1);
            Check(numericUpDown3, label5, 2);
            Check(numericUpDown4, label6, 3);
        }
        private void Check(NumericUpDown num, Label label, int i)
        {
            if ((int)num.Value == code[i])
            {
                label.Text = "RIGHT";
                num.BackColor = Color.Green;
                num.ForeColor = Color.White;
            }
            else if ((int)num.Value < code[i])
            {
                label.Text = "GREATER";
            }
            else if ((int)num.Value > code[i])
            {
                label.Text = "LESS";
            }
        }
        private bool CheckResults()
        {
            if (label3.Text == "RIGHT" && label4.Text == "RIGHT" && label5.Text == "RIGHT" && label6.Text == "RIGHT")
                return true;
            return false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
