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

namespace DefaultPlugin.Sources.Twitch
{
    public partial class TwitchAuthenticationDialog : Form
    {
        Twitch bindTwitchSource;

        public TwitchAuthenticationDialog(Twitch bindTwitchSource)
        {
            this.bindTwitchSource = bindTwitchSource;
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TwitchAuthenticationDialog_Load(object sender, EventArgs e)
        {
            textBox1.Text = bindTwitchSource.OAuth != null&&bindTwitchSource.OAuth.Length>6 ? bindTwitchSource.OAuth.Substring("oauth:".Length) : "";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://twitchapps.com/tmi/");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Cannel
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OK
            string oauth = textBox1.Text;

            if (oauth.Length == 0)
            {
                MessageBox.Show("OAuth不能为空", "TwitchAuthenticationDialog");
                return;
            }

            oauth = (oauth.StartsWith("oauth:") ? "" : "oauth") + oauth;

            bindTwitchSource.OAuth = oauth;

            string clientId = textBox2.Text;

            if (clientId.Length != 0)
            {
                bindTwitchSource.ClientID = clientId;
            }

            //Connect();

            Close();
        }

        private async void Connect() => bindTwitchSource.Connect();

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = checkBox1.Checked;
            textBox2.Visible = checkBox1.Checked;
            label4.Visible = checkBox1.Checked;
            linkLabel2.Visible = checkBox1.Checked;
        }
    }
}
