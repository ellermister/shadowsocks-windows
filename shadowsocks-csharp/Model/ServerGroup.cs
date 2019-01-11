using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks.Model
{
    class ServerGroup
    {
        public List<Server> serverList;
        public string name;

        public ServerGroup()
        {
            name = "";
            serverList = new List<Server>();
        }
    }
}
