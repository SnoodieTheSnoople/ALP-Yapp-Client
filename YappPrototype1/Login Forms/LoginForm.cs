using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YappPrototype1.Chat_Forms;
using YappPrototype1.Login_Forms;

namespace YappPrototype1
{
    public partial class LoginForm : Form
    {
        public bool Authenticated { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }

        DbCommands db = new DbCommands();
        Hash hash = new Hash();

        public LoginForm()
        {
            InitializeComponent();
        }

        private bool EmailFieldEmpty()
        {
            if (Email_TxtBox1.Text.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool UnameFieldEmpty()
        {
            if (Username_TxtBox2.Text.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool PwordFieldEmpty()
        {
            if (Pword_TxtBox3.Text.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Login_Btn2_Click(object sender, EventArgs e)
        {
            if (!EmailFieldEmpty() && !UnameFieldEmpty() && !PwordFieldEmpty())
            {
                if (db.Select(Email_TxtBox1.Text, Username_TxtBox2.Text, Pword_TxtBox3.Text))
                {
                    Authenticated = true;
                    Username = Username_TxtBox2.Text;
                    Email = Email_TxtBox1.Text;
                    Close();
                }
                else
                {
                    MessageBox.Show("Incorrect details to account or account does not exist.", "Yapp! Error");
                }
            }
            else if (EmailFieldEmpty())
            {
                MessageBox.Show("Please enter your email.", "Yapp! Error");
            }
            else if (UnameFieldEmpty())
            {
                MessageBox.Show("Please enter your username.", "Yapp! Error");
            }
            else if (PwordFieldEmpty())
            {
                MessageBox.Show("Please enter your password.", "Yapp! Error");
            }
            hash.RefreshSalt();
        }

        private void CreateAcc_link2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var createAcc = new CreateAccForm();
            createAcc.Show();
        }

        private void Quit_Btn1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Forgotpword_link1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var confirmAcc = new ConfirmAccount();
            confirmAcc.Show();
        }

    }
}
