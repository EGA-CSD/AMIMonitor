using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadConfig
{
    class AMIServer
    {
        private String host = "127.0.0.1";
        private int port = 59000;
        private long time_stamp;
        public string Host { get => host; set => host = value; }
        public int Port { get => port; set => port = value; }
        public long Time_stamp { get => time_stamp; set => time_stamp = value; }
        public AMIServer() {}
        public AMIServer(String host, int port) { this.host = host; this.port = port; }

    }
}
