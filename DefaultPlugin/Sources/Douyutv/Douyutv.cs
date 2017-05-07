using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DefaultPlugin.Sources.Douyutv
{

    class Douyutv : ISourceBase
    {
        public const string SOURCE_NAME = "Douyutv";
        public const string SOURCE_AUTHOR = "Deliay";
        public event ConnectedEvt onConnected;
        public event DanmukuEvt onDanmuku;
        public event DisconnectedEvt onDisconnected;
        public event GiftEvt onGift;
        public event CurrentOnlineEvt onOnlineChange;

        private const string server = "openbarrage.douyutv.com";
        private const short port = 8601;
        private TcpClient socket;
        private NetworkStream stream;
        private int roomId = 0;
        private long unix;

        private bool isConencted = false;

        public async Task<bool> ConnectAsync(int roomId)
        {
            this.roomId = roomId;
            if (socket == null)
                socket = new TcpClient();
            else
            {
                socket.Close();
                socket = new TcpClient();
            }

            
            await socket.ConnectAsync(server, port);

            if (socket.Connected) stream = socket.GetStream();
            else return false;

            //Login first
            LoginRequest();
            //Proceed to join group
            JoinGroup();
            //And first heartbeat loop
            HeartLoop();
            //Final, start new receive thread
            //Thread receive = new Thread(DataReceive);
            //receive.Start();
            isConencted = true;
            DataReceive();
            return true;

        }

        private void DataReceive()
        {
            while (isConencted)
            {
                ServerPacket packet;
                packet = new ServerPacket(stream.ReadPacket());
                IO.CurrentIO.Write(Enum.GetName(packet.GetType(), packet.MsgType));
                switch (packet.MsgType)
                {
                    case ServerPacket.ServerMsg.keeplive:             // heart

                        if (!packet.get("tick").Equals(unix.ToString()))
                        {
                            onOnlineChange(0);
                            IO.CurrentIO.WriteColor("连接状态检测失败! " + unix.ToString() + " except:" + packet.get("tick"), ConsoleColor.Red);
                        }
                    

                    break;
                    case ServerPacket.ServerMsg.loginres:             // login response

                        IO.CurrentIO.WriteColor("斗鱼服务器连接认证成功！", ConsoleColor.Green);
                        onConnected?.Invoke();

                    break;
                    case ServerPacket.ServerMsg.chatmsg:              // danmaku
#if DEBUG
                        IO.CurrentIO.Write("收到弹幕: 来自" + packet.get("nn") + ":" + packet.get("txt"));
#endif
                        this.onDanmuku?.Invoke(new DouyuDanmaku(packet.get("nn"), packet.get("txt")));

                    break;
                    case ServerPacket.ServerMsg.dgb:                  // gift

                        this.onGift?.Invoke(new DouyuGift(packet.get("nn"), packet.get("gs"), packet.get("gfcnt")));

                    break;
                    case ServerPacket.ServerMsg.dc_buy_deserve:       // gift

                        this.onGift?.Invoke(new DouyuGift((new STT(packet.get("sui"))).get("nick"), "酬勤",  packet.get("cnt")));

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
            while (this.isConencted)
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

        public bool Connect(string roomName)
        {
            try
            {
                return ConnectAsync(int.Parse(roomName)).Result;
            }
            catch
            {
                return Disconnect();
            }
        }

        public bool Disconnect()
        {
            LogoutReq logout = new LogoutReq();
            stream.SendPack(logout);
            socket.Close();
            isConencted = false;
            this.onDisconnected?.Invoke();
            return true;
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

        public bool Stauts()
        {
            return isConencted;
        }
    }
}
