using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LodeRunnerForm
{
    public partial class LodeRunner : Form
    {
        public LodeRunner()
        {
            InitializeComponent();
            Game.PlayMusic();
        }
        public void button1_Click(object sender, EventArgs e)
        {
            Game.PlayMusic();
            button1.Visible = false;
            pictureBox1.Visible = false;
            button1.Enabled = false;
            button2.Visible = false;
            button2.Enabled = false;
            button3.Visible = false;
            button3.Enabled = false;
            button5.Visible = false;
            button5.Enabled = false;
            button6.Visible = false;
            button6.Enabled = false;
            label4.Visible = true;
            List<string> curLevels = new List<string>();
            curLevels = MainUI.DirectoryContain();
            for (int i = 0, counter = 1; i < curLevels.Count; i++, counter += 10)
            {
                Random rnd = new Random();
                Button levelBtn = new Button();
                levelBtn.Size = new Size(250, 50);
                levelBtn.BackColor = Color.Orange;
                levelBtn.Location = new Point(this.Width / 3 + 100, counter * 10);
                levelBtn.Text = curLevels[i];
                levelBtn.Font = new Font("Arial", 12, FontStyle.Regular);
                levelBtn.Click += new EventHandler(btnClick);
                Controls.Add(levelBtn);
            }
            Button levelBtn2 = new Button();
            levelBtn2.Size = new Size(250, 50);
            levelBtn2.BackColor = Color.Yellow;
            levelBtn2.Location = new Point(10, 10);
            levelBtn2.Text = "Back";
            levelBtn2.Font = new Font("Arial", 12, FontStyle.Regular);
            levelBtn2.Click += new EventHandler(backToMenu);
            Controls.Add(levelBtn2);
            AutoScroll = true;
        }
        private void btnClick(object sender, EventArgs e)
        {
            HideBtns();
            label4.Visible = false;
            Game.PlayMusic();
            string Name = (sender as Button).Text;
            Game game = new Game();
            game.GameProcess(this, Name);
            label1.Visible = true;
            label3.Visible = true;
        }
        private void backToMenu(object sender, EventArgs e)
        {
            HideBtns();
            Game.PlayMusic();
            button1.Visible = true;
            button1.Enabled = true;
            button2.Visible = true;
            button2.Enabled = true;
            button3.Visible = true;
            button3.Enabled = true;
            button5.Visible = true;
            button5.Enabled = true;
            button6.Visible = true;
            button6.Enabled = true;
            button4.Visible = false;
            button4.Enabled = false;
            pictureBox1.Visible = true;
            label4.Visible = false;
        }
        private void HideBtns()
        {
            AutoScroll = false;
            foreach (Control c in Controls)
            {
                Button b = c as Button;
                if (b != null)
                {
                    b.Visible = false;
                    b.Enabled = false;
                }
            }
            button4.Visible = true;
            button4.Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            Environment.Exit(0);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Game.PlayMusic();
            Form2 form = new Form2();
            form.Show();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Game.PlayMusic();
            GC.Collect();
            Application.Restart();
            Environment.Exit(0);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button5_Click(object sender, EventArgs e)
        {
            Game.PlayMusic();
            WindowsFormsApp3.Form1 feedback = new WindowsFormsApp3.Form1();
            feedback.Show();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void button6_Click_1(object sender, EventArgs e)
        {
            Game.PlayMusic();
            Form3 form = new Form3();
            form.Show();
        }
    }
}
