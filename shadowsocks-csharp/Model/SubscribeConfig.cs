using System;

namespace Shadowsocks.Model
{
    /*
     * Data come from WinINET
     */

    [Serializable]
    public class SubscribeConfig
    {
        public string url;
        public string title;

        public SubscribeConfig()
        {
            url = "";
            title = "";
        }
    }
}