namespace AMI_Monitor
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization.Formatters.Binary;

    public class AvailableInfo
    {
        private bool _Available;

        public AvailableInfo Clone()
        {
            MemoryStream serializationStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(serializationStream, this);
            serializationStream.Position = 0L;
            object obj2 = formatter.Deserialize(serializationStream);
            serializationStream.Close();
            return (obj2 as AvailableInfo);
        }

        public bool Available
        {
            get
            {
                if (this.ReturnCode != "90011")
                {
                    return this._Available;
                }
                return true;
            }
            set
            {
                this._Available = (this.ReturnCode == "90011") || value;
            }
        }

        public string AvailableStatus
        {
            get
            {
                if (!this.Available)
                {
                    return "Unavailable";
                }
                return "Available";
            }
        }

        public string AvailableStatusLong
        {
            get
            {
                if (!this.Available)
                {
                    return "Unavailable - ไม่พร้อมใช้งาน";
                }
                return "Available - พร้อมใช้งาน";
            }
        }

        public string AvailableStatusTh
        {
            get
            {
                if (!this.Available)
                {
                    return "ไม่พร้อมใช้งาน";
                }
                return "พร้อมใช้งาน";
            }
        }

        public string LastCheck { get; set; }

        public string LastOnline { get; set; }

        public string RequestNo { get; set; }

        public string ReturnCode { get; set; }

        public string ReturnMessage { get; set; }

        public string StatusReturn =>
            $"{this.ReturnCode} - {this.ReturnMessage}";

        public string User { get; set; }
    }
}

