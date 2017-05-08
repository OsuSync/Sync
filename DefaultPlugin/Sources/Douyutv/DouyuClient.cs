using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DefaultPlugin.Sources.Douyutv
{
    /*
     * Extension method for STT convert
     */
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

    /*
     * Douyu datapack format
     */
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
            parseString(Encoding.UTF8.GetString(data));
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
                    string[] result = item.Split("@=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
            return Encoding.UTF8.GetBytes(ToString() + "\0");
        }

        public static implicit operator string(STT src)
        {
            return src.ToString();
        }

        public static implicit operator STT(string val)
        {
            return new STT(val);
        }

        public static implicit operator byte[] (STT src)
        {
            return src.ToByte();
        }

        public static implicit operator STT(byte[] data)
        {
            return new STT(data);
        }
    }

    /*
     * Base packet definition
     */
    struct Packet
    {
        public int size;            //4             4
        //public int size2;         //4     4       8
        public short msgType;       //2     6       10
        public byte hash;           //1     7       11
        public byte reserve;        //1     8       12
        public byte[] data;         //a     8+a     12+a

        public static implicit operator byte[](Packet pack)
        {
            return ToBytes(pack);
        }

        public static implicit operator Packet(byte[] src)
        {
            return ToPacket(src);
        }

        public static byte[] ToBytes(Packet packet)
        {
            int size = packet.size;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(packet, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                Buffer.BlockCopy(packet.data, 0, bytes, 8, packet.data.Length);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        
        public static Packet ToPacket(byte[] bytes)
        {
            Packet pack = new Packet();
            pack.size = bytes.Length + 4;
            if(BitConverter.ToInt32(bytes, 0) != pack.size)
            {
                throw new InvalidDataException("Data Failed");
            }
            pack.msgType = (short)BitConverter.ToInt16(bytes, 4);
            byte[] buffer = new byte[pack.size - 12];
            Buffer.BlockCopy(bytes, 8, buffer, 0, buffer.Length);
            pack.data = buffer;
            return pack;
        }
    }

    /*
     * Extension method for NetworkStream reader
     */
    static class DouyuPacketStreamHelper
    {
        public static void ReadLength(this NetworkStream stream, int length, out byte[] buffers)
        {
            buffers = new byte[length];
            int current = 0;
            int offset = 0;
            while (current < length)
            {
                int remain = stream.Read(buffers, offset, length - current);
                current += remain;          
                offset += remain;
            }
        }

        public static DouyuPacket ReadPacket(this NetworkStream stream)
        {
            var buffer = new byte[4];
            stream.ReadLength(4, out buffer);
            int dataSize = BitConverter.ToInt32(buffer, 0) - 4;

            buffer = new byte[dataSize];

            stream.ReadLength(dataSize, out buffer); 

            return buffer;
        }

        public static void Send(this NetworkStream stream, byte[] buffer, int offset, int size)
         {
            stream.Write(buffer, offset, size);
            stream.Flush();
        }

        public static void SendPack(this NetworkStream stream, DouyuPacket packet)
        {
            Send(stream, packet, 0, packet.Size);
        }
    }

    /*
     * Base datapack parser implement
     */
    class DouyuPacket
    {
        public enum PacketType
        {
            ServerMsg = 690,
            ClientMsg = 689
        }

        private Packet pack;

        protected DouyuPacket(DouyuPacket src)
        {
            this.pack = src.pack;
        }

        protected DouyuPacket()
        {
            pack = new Packet();

        }

        protected DouyuPacket(byte[] data) : this()
        {
            pack = data;

        }

        protected DouyuPacket(PacketType type) : this()
        {
            setType(type);
        }

        protected DouyuPacket(PacketType type, STT data)
        {
            pack = new Packet();
            pack.msgType = (short)type;
            pack.data = data;

        }

        public byte[] RaiseByte()
        {
            pack.size = 8 + pack.data.Length;
            pack.reserve = 0;
            pack.hash = 0;

            byte[] srcBuf = pack;
            byte[] buffer = new byte[srcBuf.Length + 4];

            Buffer.BlockCopy(srcBuf, 0, buffer, 0, 4);
            Buffer.BlockCopy(srcBuf, 0, buffer, 4, srcBuf.Length);

            return buffer;
        }

        public STT Data
        {
            get { return pack.data; }
        }

        protected void setData(byte[] data)
        {
            pack.data = data;
        }

        protected void setType(PacketType type)
        {
            pack.msgType = (short)type;
        }

        public int DataSize
        {
            get { return pack.data.Length; }
        }

        public int Size
        {
            get { return pack.size; }
        }

        public PacketType Type
        {
            get
            {
                PacketType type;
                Enum.TryParse<PacketType>(pack.msgType.ToString(), out type);
                return type;
            }
        }

        public static implicit operator byte[] (DouyuPacket obj)
        {
            return obj.RaiseByte();
        }

        public static implicit operator DouyuPacket(byte[] src)
        {
            return new DouyuPacket(src);
        }
    }

    /*
     * Clientside request packet specialized
     */
    class LoginReq : DouyuPacket
    {
        public LoginReq(int roomId) : base(PacketType.ClientMsg)
        {
            STT data = new STT();
            data.Add("type", "loginreq");
            base.setData(data);
        }
    }


    class GroupReq : DouyuPacket
    {
        public GroupReq(int roomId) : base(PacketType.ClientMsg)
        {
            STT data = new STT();
            data.Add("type", "joingroup");
            data.Add("rid", roomId.ToString());
            data.Add("gid", "-9999");
            base.setData(data);
        }
    }

    class Heratbeat : DouyuPacket
    {
        public Heratbeat(long currentUnixTick) : base(PacketType.ClientMsg)
        {
            STT data = new STT();
            data.Add("type", "keeplive");
            data.Add("tick", currentUnixTick.ToString());
            base.setData(data);
        }
    }

    class LogoutReq : DouyuPacket
    {
        public LogoutReq() : base(PacketType.ClientMsg)
        {
            STT data = new STT();
            data.Add("type", "logout");
            base.setData(data);
        }
    }

    /*
     *  Server return packet specialized
     */
    class ServerPacket : DouyuPacket
    {
        public enum ServerMsg
        {
            keeplive,   //heart
            loginres,   //login ret
            chatmsg,    //danmaku
            dgb,        //gift
            dc_buy_deserve, //deserve (also gift)
            spbc,       //rocket or flight
        }

        private STT data;

        public ServerPacket(byte[] buffer) : base(buffer)
        {
            //initialize with DouyuPacket(byte[]) constructor
            data = base.Data;
        }
        
        public ServerPacket(DouyuPacket srcPacket) : base(srcPacket)
        {
            data = base.Data;
        }

        public ServerMsg MsgType
        {
            get
            {
                return (ServerMsg)Enum.Parse(typeof(ServerMsg), data.get("type"));
            }
        }

        public string get(string key)
        {
            return data.get(key);
        }


    }
}
