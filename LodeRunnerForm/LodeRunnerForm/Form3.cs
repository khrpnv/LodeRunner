using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LodeRunnerForm
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("Data\\SetupMiniGames\\Pexeso\\Pexeso\\Debug\\setup.exe");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("Data\\SetupMiniGames\\GuessTheNumber\\GuessTheNumber\\Debug\\setup.exe");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("Data\\SetupMiniGames\\SaveTheWorld\\SaveTheWorld\\Debug\\setup.exe");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
