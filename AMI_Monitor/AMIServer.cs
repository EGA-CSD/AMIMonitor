using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMI_Monitor
{
    class AMIServer
    {
        public enum state_enum { stop=0, running=1, pause=2 };
        private String host = "127.0.0.1";
        private int port = 59000;
        private String name = "Undefined";
        private long time_stamp = 0;
        private state_enum state = state_enum.pause;
        public int Port { get => port; set => port = value; }
        public long Time_stamp { get => time_stamp; set => time_stamp = value; }
        public string Name { get => name; set => name = value; }
        public string Host { get => host; set => host = value; }
        internal state_enum State { get => state; set => state = value; }

        public AMIServer() {}
        public AMIServer(String name, String host, int port) { this.Name = name; this.Host = host; this.port = port; }

    }
}
