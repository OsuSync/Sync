using Sync.Client;
using Sync.MessageFilter;
using Sync.Plugins;
using Sync.Source;
using Sync.Tools;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Sync.Tools.DefaultI18n;
namespace Sync
{
    /// <summary>
    /// 连接逻辑类，负责协调IRC和弹幕源的通讯、通讯时事件触发与管理
    /// </summary>
    public class SyncConnector
    {
        private CooCClient client = null;
        private SourceBase source = null;
        private Thread ClientThread = null;
        private Thread SourceThread = null;

        private int usercount;

        public Thread ThreadClient { get => ClientThread; }
        public Thread ThreadSource { get => SourceThread; }
        public CooCClient Client { get => client; }
        public SourceBase Source { get => source; }

        /// <summary>
        /// 使用Message Filter替代直接发送消息（改用IRC类内部方法）
        /// </summary>
        /// <param name="msg">要发送的消息</param>
        [Obsolete("Using 'Message Filter' to replace sendMessage directly.", true)]
        public void IRCSendMessage(string msg)
        {
            //if(IRCStatus)
            //IRC.sendMessage(Meebey.SmartIrc4net.SendType.Message, msg);
        }

        /// <summary>
        /// 使用Message Filter替代直接发送消息（改用IRC类内部方法）
        /// </summary>
        /// <param name="msg">要发送的消息</param>
        [Obsolete("Using 'Message Filter' to replace sendMessage directly.", true)]
        public void IRCSendAction(string msg)
        {
            //if(IRCStatus)
            //IRC.sendMessage(Meebey.SmartIrc4net.SendType.Action, msg);
        }

        /// <summary>
        /// 用连接源实例化一个Sync类
        /// </summary>
        /// <param name="Source">连接源</param>
        public SyncConnector(SourceBase Source)
        {

            client = new CooCClient(this);
            this.source = Source;

            Source.EventBus.BindEvent<BaseStatusEvent>(OnStatusChange);
            Source.EventBus.BindEvent<IBaseGiftEvent>(OnGift);
            Source.EventBus.BindEvent<IBaseDanmakuEvent>(OnDanmuku);
            Source.EventBus.BindEvent<BaseOnlineCountEvent>(OnOnlineChange);
        }

        #region 连接源的事件

        private void OnStatusChange(BaseStatusEvent evt)
        {
            BaseStatusEvent status = evt;
            switch (status.Status)
            {
                case SourceStatus.REMOTE_DISCONNECTED:
                    OnDisconnected();
                    break;
                case SourceStatus.CONNECTED_WORKING:
                    OnConnected();
                    break;
                default:
                    break;
            }
        }

        private void OnGift(IBaseGiftEvent gift)
        {
            Program.host.Messages.RaiseMessage<ISourceGift>(new GiftMessage(gift));
        }

        private void OnDanmuku(IBaseDanmakuEvent danmuku)
        {
            Program.host.Messages.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmuku));
        }

        private void OnOnlineChange(BaseOnlineCountEvent count)
        {
            int lCount = count.Count;
            IO.CurrentIO.Write(string.Format(LANG_UserCount, lCount));
            if (Math.Abs(usercount - lCount) > 4) 
            {
                IBaseDanmakuEvent d = new BaseDanmakuEvent()
                {
                    Danmuku = string.Format(LANG_UserCount_Change,(string)(usercount > lCount ? LANG_UserCount_Change_Decrease : LANG_UserCount_Change_Increase), lCount)
                };
                Program.host.Messages.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(d));

                usercount = lCount;
            }
        }

        private void OnDisconnected()
        {
            if (source.Status == SourceStatus.REMOTE_DISCONNECTED)
            {
                IO.CurrentIO.Write(LANG_Source_Disconnected);
                Task.Delay(3000);
                Connect();
            }
            else
            {
                IO.CurrentIO.Write(LANG_Source_Disconnected_Succ);
            }

        }

        private void OnConnected()
        {
            IO.CurrentIO.Write(LANG_Source_Connected_Succ);
        }
        #endregion

        #region 连接方法
        private void StartSourceT()
        {
            IO.CurrentIO.Write(LANG_Source_Connect);
            SourceThread = new Thread(StartSource)
            {
                IsBackground = true
            };
            SourceThread.Start();
        }

        private void StopSourceT()
        {
            IO.CurrentIO.Write(LANG_Source_Disconnecting);
            source.disconnect();
        }

        private void StartClientT()
        {
            IO.CurrentIO.Write(LANG_IRC_Connecting);
            ClientThread = new Thread(StartClient)
            {
                IsBackground = true
            };
            ClientThread.Start();
        }

        private void StopClientT()
        {
            IO.CurrentIO.Write(LANG_IRC_Disconnect);
            client.disconnect();
            while (client.isConnected) { Thread.Sleep(1); }
        }

        private void StartSource()
        {
            source.connect();
            while (source.Status == SourceStatus.CONNECTED_WORKING) { Thread.Sleep(1); }
            source.disconnect();
        }

        private void StartClient()
        {
            client.connect();
            while (client.isConnected) { Thread.Sleep(1); }
            client.disconnect();
        }

        #endregion

        /// <summary>
        /// 开始工作
        /// </summary>
        public void Connect()
        {
            IO.CurrentIO.Write(LANG_Start);
            StartClientT();
            StartSourceT();
            Source.EventBus.RaiseEventAsync(new StartSyncEvent());
        }

        /// <summary>
        /// 停止工作
        /// </summary>
        public void Disconnect()
        {
            IO.CurrentIO.Write(LANG_Stopping);
            if (ClientThread != null && ClientThread.IsAlive) StopClientT();
            if (SourceThread != null && SourceThread.IsAlive) StopSourceT();
        }

        /// <summary>
        /// 重新连接(未实现)
        /// </summary>
        [Obsolete]
        public void Reconnect()
        {
            IO.CurrentIO.Write(LANG_Restarting);
            Disconnect();
            Connect();
        }
    }
}