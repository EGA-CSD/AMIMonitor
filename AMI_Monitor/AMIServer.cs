using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMI_Monitor
{
    class AMIServer
    {
        static int Second = 1000;
        public enum state_enum { stop=0, running=1, pause=2 };
        public enum config_en { address = 0, durationTime = 1, hostname = 0, port = 1, exception = 2, excpetional_start_time=0, excpetional_end_time=1}
        private state_enum state = state_enum.pause;
        private String host = "127.0.0.1";
        private String name = "Undefined";
        private TimeSpan exceptional_start_time;
        private TimeSpan exceptional_end_time;
        
        private int port = 59000;
        private int durationTime = 30; //second 
        private long time_stamp = 0;
        private DateTime timeStmap;

        public int Port { get => port; set => port = value; }
        public long Time_stamp { get => time_stamp; set => time_stamp = value; }
        public string Name { get => name; set => name = value; }
        public string Host { get => host; set => host = value; }
        internal state_enum State { get => state; set => state = value; }
        public int DurationTime { get => durationTime; set => durationTime = value; }
        public DateTime TimeStmap { get => timeStmap; set => timeStmap = value; }
        public TimeSpan Exceptional_start_time { get => exceptional_start_time; set => exceptional_start_time = value; }
        public TimeSpan Exceptional_end_time { get => exceptional_end_time; set => exceptional_end_time = value; }

        public AMIServer() { this.TimeStmap = DateTime.Now; }
        public AMIServer(String name, String host, int port) { this.Name = name; this.Host = host; this.Port = port; this.TimeStmap = DateTime.Now; }
        public AMIServer(String name, String host, int port, int durationTime) { this.Name = name;  this.Host = host; this.Port = port; this.DurationTime = durationTime; this.TimeStmap = DateTime.Now; }
        public AMIServer(String name, String host, int port, int durationTime, TimeSpan exceptional_start_time, TimeSpan exceptional_end_time) { this.Name = name; this.Host = host; this.Port = port; this.DurationTime = durationTime; this.TimeStmap = DateTime.Now; this.exceptional_start_time = exceptional_start_time; this.exceptional_end_time = exceptional_end_time; }

    }
}
