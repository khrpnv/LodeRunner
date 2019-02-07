using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }
        private static void SendMail(string From, string Name, string Message, string Password)
        {
            MailAddress from = new MailAddress(From, Name);
            MailAddress to = new MailAddress("illia.khrypunov@nure.ua");
            MailMessage m = new MailMessage(from, to);
            m.Subject = "LodeRunnerGame";
            m.Body = Message;
            int pos = From.IndexOf("@") + 1;
            string smtpName = "smtp." + From.Substring(pos, From.Length - From.IndexOf("@") - 1);
            SmtpClient smtp = new SmtpClient(smtpName, 587);
            smtp.Credentials = new NetworkCredential(From, Password);
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
        private void label4_Click(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckData() && InternetAvailability.IsInternetAvailable())
            {
                SendMail(textBox1.Text, textBox3.Text, richTextBox1.Text, textBox2.Text);
                MessageBox.Show("The letter is sent. Thanks for your opinion!");
            }
            else MessageBox.Show("Oops! Something went wrong. Check your internet connection.");
            ClearDataFields();
        }
        private bool CheckData()
        {
            bool flag = true;
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text) || String.IsNullOrEmpty(richTextBox1.Text))
            {
                MessageBox.Show("Some fields are empty. Check data!");
                ClearDataFields();
                flag = false;
            }
            if(textBox1.Text.IndexOf("@") == -1)
            {
                MessageBox.Show("Invalid e-mail!");
                ClearDataFields();
                flag = false;
            }
            return flag;
        }
        private void ClearDataFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            richTextBox1.Text = "";
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
    }
    public static class InternetAvailability
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsInternetAvailable()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
    }
}
