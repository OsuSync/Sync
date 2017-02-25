using System;
using Sync.Source.BiliBili.BiliBili_dm;
using Sync.Source;
using System.Threading.Tasks;

namespace DefaultPlugin.Source.BiliBili
{
    /// <summary>
    /// BiliBili Live的同步源类
    /// </summary>
    class BiliBili : ISourceBase, ISendable
    {
        public const string SOURCE_NAME = "Bilibili";
        public const string SOURCE_AUTHOR = "Sender: Deliay, Receive: copyliu";
        DanmakuLoader client = new DanmakuLoader();
        BiliBiliSender sender;
        private bool isConnected = false;

        public event Sync.Source.ConnectedEvt onConnected;
        public event DisconnectedEvt onDisconnected;
        public event DanmukuEvt onDanmuku;
        public event GiftEvt onGift;
        public event CurrentOnlineEvt onOnlineChange;

        public BiliBili()
        {
            sender = new BiliBiliSender(null, null);
        }

        public bool Connect(int roomId)
        {
            client.ReceivedDanmaku += Dl_ReceivedDanmaku;
            client.ReceivedRoomCount += Dl_ReceivedRoomCount;
            client.Disconnected += Dl_Disconnected;
            Task<bool> task = client.ConnectAsync(roomId);
            if(task.Status == TaskStatus.Running)
            {
                isConnected = true;
            }
            isConnected = true;
            onConnected();
            return true;
        }

        public bool Disconnect()
        {
            client.Disconnect();
            return true;
        }
        private void Dl_Disconnected(object sender, DisconnectEvtArgs args)
        {
            isConnected = false;
            onDisconnected();
        }

        private void Dl_ReceivedRoomCount(object sender, ReceivedRoomCountArgs e)
        {

            onOnlineChange(e.UserCount);

        }

        private void Dl_ReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
        {

            if (e.Danmaku.MsgType == MsgTypeEnum.Comment)
            {
                onDanmuku(new BiliBiliDanmuku(e.Danmaku));
            }
            else if (e.Danmaku.MsgType == MsgTypeEnum.GiftSend)
            {
                onGift(new BiliBiliGift(e.Danmaku));
            }
        }

        public bool Stauts()
        {
            return isConnected;
        }

        public void Send(string str)
        {
            if(sender.loginStauts)
            {
                sender.send(str);
            }
        }

        public void Login(string user, string password)
        {
            sender = new BiliBiliSender(user, password);
            sender.login();
        }

        public Type getSourceType()
        {
            return typeof(BiliBili);
        }

        public bool LoginStauts()
        {
            return sender.loginStauts;
        }

        public string getSourceName()
        {
            return SOURCE_NAME;
        }

        public string getSourceAuthor()
        {
            return SOURCE_AUTHOR;
        }

        public void Dispose()
        {
            client.Disconnect();
        }
    }
}