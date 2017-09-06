using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Timers;

namespace AMI_Monitor
{
    class AMI_Monitor
    {   
        enum config {address = 0, durationTime = 1 };
        enum addr{ hostname = 0, port = 1 };
        private CultureInfo cultureEN;
        private List<AMIServer> list_AMIServer;
        private AvailableInfo info;
        
        public void run()
        {
            this.list_AMIServer = new List<AMIServer>();

            // Read Configuration
            readConf();
            foreach (var amiserver in list_AMIServer)
            {
                checkAMIServer(amiserver);
            }


        }

        private void readConf() {
            // Read Configuration
            this.list_AMIServer = new List<AMIServer>();
            foreach (string key in ConfigurationManager.AppSettings) {
                String value = ConfigurationManager.AppSettings[key];
                String[] values = value.Split(',');
                String[] host_port = values[(int)config.address].Split(':');
                Console.WriteLine("key: "+ key +"value : " + value+" host: "+host_port[(int)addr.hostname]+", port:"+host_port[(int)addr.port]+", time: "+values[1]);
                this.list_AMIServer.Add(new AMIServer(key, host_port[(int)addr.hostname], int.Parse(host_port[(int)addr.port])));
                
            }
        }

        private void callback_checkAMIServer(Object myObject , ElapsedEventArgs elapsedEventArgs) {

            checkAMIServer((AMIServer)myObject);
 
        }

        private void checkAMIServer(AMIServer amiServer)
        {
            bool flag = false;
            IPEndPoint remoteEP;
            this.WriteLog(amiServer, "Initiailize server Host: " + amiServer.Host + ", port : "+amiServer.Port);

            try
            {
                remoteEP = new IPEndPoint(IPAddress.Parse(amiServer.Host), amiServer.Port);
            }
            catch (Exception ex) {
                this.WriteLog(amiServer, ex.Message);
                return;
            }

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    int millisecondsTimeout = 0xbb8;
                    socket.SendTimeout = millisecondsTimeout;
                    socket.ReceiveTimeout = millisecondsTimeout;
                    if (!socket.BeginConnect(remoteEP, null, null).AsyncWaitHandle.WaitOne(millisecondsTimeout, false))
                    {
                        throw new SocketException(0x274c);
                    }
                    this.WriteLog(amiServer, "Connect: Success");
                }
                catch (Exception exception)
                {
                    flag = true;
                    this.WriteLog(amiServer, "Connect: Error " + exception.Message);
                }
                if (!flag)
                {
                    try
                    {
                        string s = "000100,0";
                        byte[] bytes = Encoding.UTF8.GetBytes(s);
                        socket.Send(bytes, bytes.Length, SocketFlags.None);
                        try
                        {
                            bytes = new byte[1024];
                            int count = socket.Receive(bytes);
                            string str2 = Encoding.UTF8.GetString(bytes, 0, count);
                            this.WriteLog(amiServer, "Response: " + str2);
                            info = JsonConvert.DeserializeObject<AvailableInfo>(str2);
                        }
                        catch (Exception exception2)
                        {
                            this.WriteLog(amiServer, "Response: Error " + exception2.Message);
                        }
                    }
                    catch (Exception exception3)
                    {
                        flag = true;
                        this.WriteLog(amiServer, "Request: Error " + exception3.Message);
                    }
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        this.WriteLog(amiServer, "Disconnect: Success");
                    }
                    catch (Exception exception4)
                    {
                        this.WriteLog(amiServer, "Disconnect: Error " + exception4.Message);
                    }
                }
            }
            if (info == null)
            {
                info = new AvailableInfo
                {
                    ReturnCode = "X0003",
                    ReturnMessage = "Can't connect to AMI Connector",
                    Available = false
                };
            }
            else {
                // Validate RetunCode and notify to line 
                if(false) {
                    sendLineNotification(amiServer);
                }
                    
            }
        }

        private void sendLineNotification(AMIServer amiServer) { }

        private void WriteLog(AMIServer amiServer, string log)
        {
            Console.WriteLine(log);
            try
            {
                cultureEN = new CultureInfo("en-US");
                string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss >> ", cultureEN);
                string str2 = log.Replace("\r\n", "\r\n" + str);
                if (!Directory.Exists("Log"))
                {
                    Directory.CreateDirectory("Log");
                }
                System.IO.File.AppendAllText(string.Concat(new object[] { "Log/AMI_Monitor.", amiServer.Name,'-',amiServer.Host,"-",amiServer.Port, ".", DateTime.Now.ToString("yyyyMMdd", cultureEN), ".log" }), str + str2 + "\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while writting log file : " + ex.Message);
            }
        }

        private static void TimeEventProcessor()
        {
            while (true)
            {
                Console.WriteLine("Hellow World");
                Thread.Sleep(1000);
            }
        }


    }
}
