using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Globalization;

namespace ReadConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            AMI_Monitor ami_monitor = new AMI_Monitor();
            ami_monitor.run();
        }
    }
}
