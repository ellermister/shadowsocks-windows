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
using System.Text.RegularExpressions;

namespace Shadowsocks.View
{
    public partial class SubscribeForm : Form
    {
        private ShadowsocksController controller;
        public static readonly Regex
        UrlFinder = new Regex(@"ss://(?<base64>[A-Za-z0-9+-/=_]+)(?:#(?<tag>\S+))?", RegexOptions.IgnoreCase),
        DetailsParser = new Regex(@"^((?<method>.+?):(?<password>.*)@(?<hostname>.+?):(?<port>\d+?))$", RegexOptions.IgnoreCase),
        UrlFinderR = new Regex(@"ssr://(?<base64>[\S]+)[\r\s]?", RegexOptions.IgnoreCase),
        GroupFinder = new Regex(@"group=(?<base64>[A-Za-z0-9+-/=_]+)&?", RegexOptions.IgnoreCase);

        List<SubscribeConfig> SubscribeList;

        private void SubscribeForm_Load(object sender, EventArgs e)
        {
            SubscribeList = controller.ListSubscribe();
            foreach(var item in SubscribeList)
            {
                SubscribeListBox.Items.Add(item.title);
            }
        }

        public SubscribeForm(ShadowsocksController controller)
        {
            this.controller = controller;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var url = textUrl.Text;
            string title = "";
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;
            Byte[] pageData = MyWebClient.DownloadData(url);
            string pageHtml = Encoding.Default.GetString(pageData);
            Byte[] originString = Convert.FromBase64String(pageHtml);
            string ssUrl = System.Text.Encoding.UTF8.GetString(originString);
            MessageBox.Show(ssUrl);
            var match = UrlFinderR.Match(ssUrl);
            if (match.Success)
            {
                var ssrLink = controller.Base64UrlDecode(match.Groups["base64"].Value);
                var GroupMatch = GroupFinder.Match(ssrLink);
                if (GroupMatch.Success)
                {
                    title = controller.Base64UrlDecode(GroupMatch.Groups["base64"].Value);
                }
            }
            controller.AddSubscribe(title, url);
        }
    }
}
