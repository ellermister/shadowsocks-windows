using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using System.Net;

namespace Shadowsocks.View
{
    public partial class SubscribeForm : Form
    {
        private ShadowsocksController controller;

        public SubscribeForm(ShadowsocksController controller)
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;
            Byte[] pageData = MyWebClient.DownloadData(textUrl.Text);
            string pageHtml = Encoding.Default.GetString(pageData);
            Byte[] originString = Convert.FromBase64String(pageHtml);
            MessageBox.Show(System.Text.Encoding.Default.GetString(originString));
        }
    }
}
