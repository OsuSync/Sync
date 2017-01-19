using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DefaultPlugin.Sources.Douyutv
{
    static class STTHelper
    {

        public static string ConvertTo(this string val)
        {
            return val.Replace("@", "@A").Replace("/", "@S");
        }

        public static string ConvertBy(this string val)
        {
            return val.Replace("@S", "/").Replace("@A", "@");
        }
    }

    class STT
    {
        List<KeyValuePair<string, string>> data;

        public STT()
        {
            data = new List<KeyValuePair<string, string>>();
        }

        public STT(string val) : this()
        {
            parseString(val);
        }

        public STT(byte[] data) : this()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(data);
            parseString(sb.ToString());
        }

        public string get(string key)
        {
            return data.FirstOrDefault(p => p.Key == key).Value;
        }

        private void parseString(string val)
        {
            foreach (var item in val.Split('/'))
            {
                if (item.Length > 0)
                {
                    string[] result = item.Split("@=".ToCharArray());
                    Add(result[0].ConvertBy(), result[1].ConvertBy());
                }
            }
        }

        public void Add(string key, string value)
        {
            data.Add(new KeyValuePair<string, string>(key, value));
        }

        public override string ToString()
        {
            string result = string.Empty;
            foreach (var item in data)
            {

                result += item.Key.ConvertTo() + "@=" + item.Value.ConvertTo() + "/";
            }
            return result;
        }

        public byte[] ToByte()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public static implicit operator string (STT src)
        {
            return src.ToString();
        }

        public static implicit operator STT (string val)
        {
            return new STT(val);
        }

        public static implicit operator byte[] (STT src)
        {
            return src.ToByte();
        }

        public static implicit operator STT (byte[] data)
        {
            return new STT(data);
        }
    }

    struct Packet
    { 
        public int size;            //4
        public int size2;           //4     8
        public short msgType;       //2     10
        public byte hash;           //1     11
        public byte reserve;        //1     12
        public byte[] data;         //a     12+a
    }

    class DouyuPacket
    {
        public enum PacketType
        {
            ServerMsg = 690,
            ClientMsg = 689
        }

        private Packet pack;

        protected DouyuPacket()
        {
            pack = new Packet();

        }

        protected DouyuPacket(byte[] data) : this()
        {
            using (MemoryStream sr = new MemoryStream(data))
            {
                IFormatter f = new BinaryFormatter();
                pack = (Packet)f.Deserialize(sr);
            }
            
        }

        protected DouyuPacket(PacketType type, STT data)
        {
            pack = new Packet();
            pack.msgType = (short)type;
            pack.data = data;
            
        }

        public byte[] RaiseByte()
        {
            pack.size = 12 + pack.data.Length;
            pack.size2 = pack.size;
            pack.reserve = 0;
            pack.hash = 0;

            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter f = new BinaryFormatter();
                f.Serialize(ms, pack);
                return ms.GetBuffer();
            }
        }

        public STT getSerialData()
        {
            return pack.data;
        }

        protected void setData(byte[] data)
        {
            pack.data = data;
        }

        protected void setType(PacketType type)
        {
            pack.msgType = (short)type;
        }

        public int GetDataSize()
        {
            return pack.data.Length;
        }

        public int GetAllSize()
        {
            return pack.size;
        }

        public PacketType GetPacketType()
        {
            PacketType type;
            Enum.TryParse<PacketType>(string.Concat(pack.msgType), out type);
            return type;
        }

        public static implicit operator byte[] (DouyuPacket obj)
        {
            return obj.RaiseByte();
        }

        public static implicit operator DouyuPacket (byte[] src)
        {
            return new DouyuPacket(src);
        }
    }

    class LoginReq : DouyuPacket
    {
        public LoginReq(int roomID)
        {
            STT data = new STT();
            base.setType(PacketType.ClientMsg);
            data.Add("type", "loginreq");
            data.Add("roomid", string.Concat(roomID));
        }
    }
}
