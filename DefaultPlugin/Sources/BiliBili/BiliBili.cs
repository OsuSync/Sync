using System;
using Sync.Source.BiliBili.BiliBili_dm;
using Sync.Source;
using System.Threading.Tasks;
using Sync.Tools;

namespace DefaultPlugin.Source.BiliBili
{
    /// <summary>
    /// BiliBili Live的同步源类
    /// </summary>
    class BiliBili : SourceBase , IConfigurable
    {
        public const string SOURCE_NAME = "Bilibili";
        public const string SOURCE_AUTHOR = "Sender: Deliay, Receive: copyliu";
        DanmakuLoader client = new DanmakuLoader();
        BiliBiliSender sender;
        private bool isConnected = false;

        public static ConfigurationElement Cookies { get; set; } = "";
        public static ConfigurationElement RoomID { get; set; } = "";

        public BiliBili() : base(SOURCE_NAME, SOURCE_AUTHOR, true)
        {
            sender = new BiliBiliSender(null, null);
        }

        public override void Send(string Message)
        {

        }

        public override void Connect()
        {
            client.ReceivedDanmaku += Dl_ReceivedDanmaku;
            client.ReceivedRoomCount += Dl_ReceivedRoomCount;
            client.Disconnected += Dl_Disconnected;
            Task<bool> task = client.ConnectAsync(int.Parse(RoomID));
            if(task.Status == TaskStatus.Running)
            {
                isConnected = true;
            }
            isConnected = true;
            RaiseEvent(new SourceEventArgs<BaseStatusEvent>(new BaseStatusEvent(SourceStatus.CONNECTED_WORKING)));
        }

        public override void Disconnect()
        {
            client.Disconnect(); 
        }
        private void Dl_Disconnected(object sender, DisconnectEvtArgs args)
        {
            isConnected = false;
            RaiseEvent(new SourceEventArgs<BaseStatusEvent>(new BaseStatusEvent(SourceStatus.REMOTE_DISCONNECTED)));
        }

        private void Dl_ReceivedRoomCount(object sender, ReceivedRoomCountArgs e)
        {
            base.RaiseEvent<BaseOnlineCountEvent>(
                new SourceEventArgs<BaseOnlineCountEvent>(
                    new BaseOnlineCountEvent() {
                        Count = (int)e.UserCount
                    }));
        }

        private void Dl_ReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
        {

            if (e.Danmaku.MsgType == MsgTypeEnum.Comment)
            {
                base.RaiseEvent<BaseDanmakuEvent>(new SourceEventArgs<BaseDanmakuEvent>(new BiliBiliDanmuku(e.Danmaku)));
            }
            else if (e.Danmaku.MsgType == MsgTypeEnum.GiftSend)
            {
                base.RaiseEvent<BaseGiftEvent>(new SourceEventArgs<BaseGiftEvent>(new BiliBiliGift(e.Danmaku)));
            }
        }

        public bool Stauts()
        {
            return isConnected;
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

        public override string ToString()
        {
            return SOURCE_NAME;
        }

        public void onConfigurationLoad()
        {

        }

        public void onConfigurationSave()
        {

        }

    }
}