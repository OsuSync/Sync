using Sync.Source;
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

        private int groupId = -9999;
        private string server = "openbarrage.douyutv.com";
        private short port = 8601;
        private TcpClient socket;
        private NetworkStream stream;
        private Douyutv parent;
        private int roomId = 0;

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
            stream = socket.GetStream();

            Thread heartLoop = new Thread(HeartbeatLoop);
            //Login first
            if(LoginRequest())
            {
                //Login success

            }
            else
            {
                //Login Fail
                return false;
            }



        }

        private bool LoginRequest()
        {
            LoginReq login = new LoginReq(roomId);
            stream.Write(login.RaiseByte(), 0, login.GetAllSize());

            return true;
        }

        private void HeartbeatLoop()
        {

        }

        public bool Connect(int roomID)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public string getSourceAuthor()
        {
            throw new NotImplementedException();
        }

        public string getSourceName()
        {
            throw new NotImplementedException();
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
