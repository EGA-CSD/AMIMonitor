﻿using Newtonsoft.Json;
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
using System.Threading.Tasks;

namespace AMI_Monitor
{
    class AMI_Monitor
    {   
        private CultureInfo cultureEN;
        private List<AMIServer> list_AMIServer = new List<AMIServer>();
        private AvailableInfo info;
        private LinkedList<Task> allTask = new LinkedList<Task>();
        static bool shutdownStatus = false;
        static String lineToken = "5MIkfmCenOQ57YoCCq5F2pg0DycCfLjP5B3IdrUbxKs";
        public void run()
        {

            // Read Configuration
            readConf();

            foreach (var amiserver in list_AMIServer)
            {
                amiserver.State = AMIServer.state_enum.running;
                Task T = new Task(checkAMIServer, amiserver);
                allTask.AddLast(T);
                T.Start();
            }

            CommandLine();
        }

        private void readConf() {
            foreach (string key in ConfigurationManager.AppSettings)
            {
                String value = ConfigurationManager.AppSettings[key];
                if (key != "token")
                {
                    String[] values = value.Split(',');
                    String[] host_port = values[(int)AMIServer.config_en.address].Split(':');
                    String str = "key=" + key + ", value=" + value + " ==> host=" + host_port[(int)AMIServer.config_en.hostname] + ", port=" + host_port[(int)AMIServer.config_en.port] + ", duration time=" + values[(int)AMIServer.config_en.durationTime];
                    Console.WriteLine(str);
                    this.list_AMIServer.Add(new AMIServer(key, host_port[(int)AMIServer.config_en.hostname], int.Parse(host_port[(int)AMIServer.config_en.port]), int.Parse(values[(int)AMIServer.config_en.durationTime])));
                }
                else {
                    Console.WriteLine("Key={0} value={1}", key, value);
                    lineToken = value;
                }
            }
        }

        private void checkAMIServer(object obj)
        {
            AMIServer amiServer = (AMIServer)obj;
            bool flag = false;
            IPEndPoint remoteEP;
            this.WriteLog(amiServer, "Initiailize Service="+amiServer.Name+", Host:" + amiServer.Host + ", Port="+amiServer.Port+", DurationTime="+amiServer.DurationTime);
            while (amiServer.State == AMIServer.state_enum.running)
            {
                Thread.Sleep(1000);
                DateTime dateTime = DateTime.Now;
                double diff = dateTime.Subtract(amiServer.TimeStmap).TotalSeconds;
                if (diff >= amiServer.DurationTime ) {
                    amiServer.TimeStmap = DateTime.Now;
                    Console.WriteLine("Servcie {0} sending request", amiServer.Name);
                    try
                    {
                        remoteEP = new IPEndPoint(IPAddress.Parse(amiServer.Host), amiServer.Port);
                    }
                    catch (Exception ex)
                    {
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

                    Console.WriteLine(info.ReturnCode + ", " + info.ReturnMessage);
                    // Validate RetunCode and notify to line 
                    if (info.ReturnCode == "X0003" || info.ReturnCode == "X0001")
                    {
                        String warningMessage = "[AMIChecker]: " + amiServer.Name + " " + amiServer.Host + ":" + amiServer.Port + " : " + info.ReturnMessage;
                        Console.WriteLine(warningMessage);
                        sendLineNotification(amiServer, warningMessage);
                    }

                }
            }
            Console.WriteLine("Service {0} {1}:{2} stoped", amiServer.Name, amiServer.Host, amiServer.Port);
            Thread.CurrentThread.Abort();
        }

        private void sendLineNotification(AMIServer amiServer, String argMsg) {
            var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
            var postData = string.Format("message={0}", argMsg);
            var data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            String tokenStr = "Bearer " + lineToken;
            request.Headers.Add("Authorization", tokenStr); //KlPhgOKMqBYSuLsZBLAY7uUCXD1s0jEjwHfbUPbQE0I
            //request.Headers.Add("Authorization", "Bearer TRp6byyCsJG7S2poh5ON3zdH88SSm3LMffZ1fXy8o1H"); //KlPhgOKMqBYSuLsZBLAY7uUCXD1s0jEjwHfbUPbQE0I
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Console.WriteLine("\r\n{0} \r\n", responseString);
        }

        private void WriteLog(AMIServer amiServer, string log)
        {
            //Console.WriteLine(log);
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

        private void CommandLine()
        {
            while (!shutdownStatus)
            {
                String input = Console.ReadLine();
                String[] cmd = input.Split(' ');
                switch (cmd[0])
                {
                    case "status":
                        status();
                        break;
                    case "stop":
                        if (cmd.Length >= 2)
                        {
                            stop(cmd[1]);
                        }
                        else
                        {
                            Console.WriteLine("Invalid parameter");
                        }
                        break;
                    case "start":
                        if (cmd.Length >= 2)
                        {
                            start(cmd[1]);
                        }
                        else
                        {
                            Console.WriteLine("Invalid parameter");
                        }
                        break;
                    case "exit":
                        shutdown();
                        break;
                    default:
                        help();
                        break;
                }
            }
        }

        private void shutdown()
        {
            foreach (Task T in allTask) {
                AMIServer server = (AMIServer)T.AsyncState;
                Console.WriteLine("Service {0} is stoping", server.Name);
                server.State = AMIServer.state_enum.stop;
            }
            shutdownStatus = true;
        }

        private void status() {
            foreach (Task T in allTask) {
                AMIServer server = (AMIServer)T.AsyncState;
                Console.WriteLine("Service {0} is running", server.Name);
            }
        }

        private void start(String service) { }

        private void stop(String service) {
            foreach (Task T in allTask)
            {
                AMIServer server = (AMIServer)T.AsyncState;
                if (server.Name == service)
                {
                    server.State = AMIServer.state_enum.stop;
                    allTask.Remove(T);
                    return;
                }
            }
            Console.WriteLine("Service {0} has not run yet", service);
        }

        private void help() {
            Console.WriteLine("=========================================================");
            Console.WriteLine("======================:: Command ::======================");
            Console.WriteLine("| start <Service>| Use for starting new service.        |");
            Console.WriteLine("| stop <Service> | Use for stopping the running service.|");
            Console.WriteLine("| status         | Display all of the running services. |");
            Console.WriteLine("| exit           | Close all service and exit program.  |");
            Console.WriteLine("=========================================================");
        }
    }
}
