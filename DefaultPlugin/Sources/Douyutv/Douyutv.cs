using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DefaultPlugin.Language;

namespace DefaultPlugin.Sources.Douyutv
{

    class Douyutv : SourceBase, IConfigurable
    {
        public const string SOURCE_NAME = "Douyutv";
        public const string SOURCE_AUTHOR = "Deliay";

        private const string server = "openbarrage.douyutv.com";
        private const short port = 8601;
        private TcpClient socket;
        private NetworkStream stream;
        private int roomId = 0;
        private long unix;
        
        public Douyutv() : base(SOURCE_NAME, SOURCE_AUTHOR, false)
        {
        }

        public static ConfigurationElement RoomID { get; set; } = "";

        public async void ConnectAsync(int roomId)
        {
            this.roomId = roomId;
            if (socket == null)
                socket = new TcpClient();
            else
            {
                socket.Close();
                socket = new TcpClient();
            }

            RaiseEvent(new BaseStatusEvent(SourceStatus.CONNECTED_WAITING));

            await socket.ConnectAsync(server, port);

            if (socket.Connected) stream = socket.GetStream();
            else return;

            //Login first
            LoginRequest();
            //Proceed to join group
            JoinGroup();
            Status = SourceStatus.CONNECTED_WORKING;
            //And first heartbeat loop
            HeartLoop();
            //Final, start new receive thread
            //Thread receive = new Thread(DataReceive);
            //receive.Start();

            RaiseEvent(new BaseStatusEvent(SourceStatus.CONNECTED_WORKING));
            DataReceive();
            return;

        }

        private void DataReceive()
        {

            while (Status == SourceStatus.CONNECTED_WORKING)
            {
                ServerPacket packet;
                packet = new ServerPacket(stream.ReadPacket());
                IO.CurrentIO.Write(Enum.GetName(packet.GetType(), packet.MsgType));
                switch (packet.MsgType)
                {
                    case ServerPacket.ServerMsg.keeplive:             // heart

                        if (!packet.get("tick").Equals(unix.ToString()))
                        {
                            RaiseEvent(new BaseOnlineCountEvent() { Count = 0 });
                            IO.CurrentIO.WriteColor(string.Format(LANG_DOUYU_FAIL, unix.ToString() ,packet.get("tick")), ConsoleColor.Red);
                        }
                    

                    break;
                    case ServerPacket.ServerMsg.loginres:             // login response

                        IO.CurrentIO.WriteColor(LANG_DOUYU_AUTH_SUCC, ConsoleColor.Green);
                        RaiseEvent(new BaseStatusEvent(SourceStatus.CONNECTED_WORKING));

                    break;
                    case ServerPacket.ServerMsg.chatmsg:              // danmaku
#if DEBUG
                        IO.CurrentIO.Write(string.Format(LANG_DOUYU_DANMAKU, packet.get("nn") ,packet.get("txt")));
#endif
                        RaiseEvent(new DouyuDanmaku(packet.get("nn"), packet.get("txt")));

                    break;
                    case ServerPacket.ServerMsg.dgb:                  // gift
                        RaiseEvent(new DouyuGift(packet.get("nn"), packet.get("gs"), packet.get("gfcnt")));

                    break;
                    case ServerPacket.ServerMsg.dc_buy_deserve:       // gift
                        RaiseEvent(new DouyuGift((new STT(packet.get("sui"))).get("nick"), LANG_DOUYU_GIFT, packet.get("cnt")));

                    break;
                    case ServerPacket.ServerMsg.spbc:                 // gift

                    break;
                    default:
                    break;
                }
            }
        }

        private async void HeartLoop()
        {
            while (this.Status == SourceStatus.CONNECTED_WORKING)
            {
                unix = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                Heratbeat heartbeat = new Heratbeat(unix);
                stream.SendPack(heartbeat);

                await Task.Delay(45000);
            }
        }

        private void JoinGroup()
        {
            GroupReq group = new GroupReq(roomId);
            stream.SendPack(group);
        }

        private bool LoginRequest()
        {
            LoginReq login = new LoginReq(roomId);
            stream.SendPack(login);
            return true;
            //DouyuPacket result = stream.ReadPacket();
            //STT data = result.Data;

            //if (result.Type == DouyuPacket.PacketType.ServerMsg &&
            //    data.get("type") == "loginres")
            //{
            //    return true;
            //}
            //else

            //{
            //    return false;
            //}

        }

        public override void Connect()
        {
            try
            {
                ConnectAsync(int.Parse(RoomID));
                RaiseEvent(new BaseStatusEvent(SourceStatus.CONNECTING));
            }
            catch
            {
                Disconnect();
            }
        }

        public override void Disconnect()
        {
            LogoutReq logout = new LogoutReq();
            stream.SendPack(logout);
            socket.Close();
            RaiseEvent(new BaseStatusEvent(SourceStatus.USER_DISCONNECTED));
        }

        public string getSourceAuthor()
        {
            return SOURCE_AUTHOR;
        }

        public string getSourceName()
        {
            return SOURCE_NAME;
        }

        public Type getSourceType()
        {
            return typeof(Douyutv);
        }

        public void onConfigurationLoad()
        {
        }

        public void onConfigurationSave()
        {
        }

        public override void Send(string Message)
        {
        }
    }
}
