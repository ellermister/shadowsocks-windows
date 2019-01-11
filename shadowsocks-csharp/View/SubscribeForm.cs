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
using Shadowsocks.Properties;

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
        string defaultUrl = "https://raw.githubusercontent.com/breakwa11/breakwa11.github.io/master/free/freenodeplain.txt";

        List<SubscribeConfig> SubscribeList;

        private void SubscribeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SelectedIndex = SubscribeListBox.SelectedIndex;
            if(SelectedIndex == -1)
            {
                return;
            }

            SubscribeConfig subscribe = SubscribeList[SelectedIndex];
            if (subscribe == null)
            {
                return;
            }
            textUrl.Text = subscribe.url;

            textGroup.Text = subscribe.title;
        }

        private void textUrl_TextChanged(object sender, EventArgs e)
        {
            var url = textUrl.Text;
            string title = "";
            Byte[] pageData,originString = null;

            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                pageData = MyWebClient.DownloadData(url);
                string pageHtml = Encoding.Default.GetString(pageData);
                if (pageHtml == "")
                {
                    return;
                }
                originString = Convert.FromBase64String(pageHtml);
            }
            catch
            {
                return;
            }
  
            string ssUrl = System.Text.Encoding.UTF8.GetString(originString);
            var match = UrlFinderR.Match(ssUrl);
            if (match.Success)
            {
                var ssrLink = controller.Base64UrlDecode(match.Groups["base64"].Value);
                var GroupMatch = GroupFinder.Match(ssrLink);
                if (GroupMatch.Success)
                {
                    title = controller.Base64UrlDecode(GroupMatch.Groups["base64"].Value);
                    textGroup.Text = title;

                    var index = SubscribeListBox.SelectedIndex;
                    if(index != -1)
                    {
                        SubscribeListBox.Items.RemoveAt(index);
                        SubscribeListBox.Items.Insert(index, title + " - " + url);

                        SubscribeList[index].title = title;
                        SubscribeList[index].url = url;

                        SubscribeListBox.SelectedIndex = index;
                    }

                }
            }
        }

        private void textGroup_TextChanged(object sender, EventArgs e)
        {
            string title = textGroup.Text;
            string url = textUrl.Text;
            var index = SubscribeListBox.SelectedIndex;
            if (index != -1)
            {
                SubscribeListBox.Items.RemoveAt(index);
                SubscribeListBox.Items.Insert(index, title + " - " + url);

                SubscribeList[index].title = title;
                SubscribeList[index].url = url;

                SubscribeListBox.SelectedIndex = index;
            }
        }

        private void SubscribeForm_Load(object sender, EventArgs e)
        {
            SubscribeList = controller.ListSubscribe();
            foreach(var item in SubscribeList)
            {
                string title = item.title + " - " + item.url;
                SubscribeListBox.Items.Add(title);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string title = "";
            SubscribeConfig subscribe = new SubscribeConfig();

            textUrl.Text = defaultUrl;
            textGroup.Text = "";

            title = textGroup.Text + " - " + textUrl.Text;

            subscribe.title = textGroup.Text;
            subscribe.url = textUrl.Text;
            SubscribeList.Add(subscribe);

            SubscribeListBox.Items.Add(title);
            SubscribeListBox.SelectedIndex = SubscribeListBox.Items.Count - 1;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            SubscribeListBox.Items.RemoveAt(SubscribeListBox.SelectedIndex);
            SubscribeListBox.SelectedIndex = SubscribeListBox.Items.Count - 1;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            controller.ClearSubscribe();
            foreach (var subscribe in SubscribeList)
            {
                controller.AddSubscribe(subscribe.title, subscribe.url);
            }
            this.Close();
            return;
        }

        public SubscribeForm(ShadowsocksController controller)
        {
            this.controller = controller;
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();

            UpdateTexts();
            this.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());

            
        }

        private void UpdateTexts()
        {
            AddButton.Text = I18N.GetString("&Add");
            DeleteButton.Text = I18N.GetString("&Delete");
            OKButton.Text = I18N.GetString("OK");
            CancelButton.Text = I18N.GetString("Cancel");

            SubscribeURLLabel.Text = I18N.GetString("SubscribeURL");
            SubscribeGroupLabel.Text = I18N.GetString("SubscribeGroup");

            this.Text = I18N.GetString("Import Subscribe");
        }

    }
}
