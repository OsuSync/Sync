using Sync.IRC;
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
        private IRCClient IRC = null;
        private ISourceBase Src = null;
        private Thread IRCThread = null;
        private Thread SrcThread = null;

        public bool IRCStatus = false;
        public bool SourceStatus = false;

        public bool IsConnect = false;
        private uint usercount;

        public Thread GetThreadIRC() { return IRCThread; }
        public Thread GetThreadSource() { return SrcThread; }
        public IRCClient GetIRC() { return IRC; }
        public ISourceBase GetSource() { return Src; }

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
        public SyncConnector(ISourceBase Source)
        {

            IRC = new IRCClient(this);
            Src = Source;

            Src.onConnected += Src_onConnected;
            Src.onDisconnected += Src_onDisconnected;
            Src.onDanmuku += Src_onDanmuku;
            Src.onOnlineChange += Src_onOnlineChange;
            Src.onGift += Src_onGift;
        }

        #region 连接源的事件
        private void Src_onGift(CBaseGift gift)
        {
            Program.host.Messages.RaiseMessage<ISourceGift>(new GiftMessage(gift));
        }

        private void Src_onOnlineChange(uint lCount)
        {
            IO.CurrentIO.Write(string.Format(LANG_UserCount, lCount));
            if (Math.Abs(usercount - lCount) > 4) 
            {
                CBaseDanmuku d = new CBaseDanmuku()
                {
                    danmuku = string.Format(LANG_UserCount_Change, (usercount > lCount ? LANG_UserCount_Change_Decrease : LANG_UserCount_Change_Increase), lCount)
                };
                Program.host.Messages.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(d));

                usercount = lCount;
            }
        }

        private void Src_onDisconnected()
        {
            if (IsConnect)
            {
                IsConnect = false;
                IO.CurrentIO.Write(LANG_Source_Disconnected);
                System.Threading.Tasks.Task.Delay(3000);
                Connect();
            }
            else
            {
                IsConnect = false;
                IO.CurrentIO.Write(LANG_Source_Disconnected_Succ);
            }
            
            
        }

        private void Src_onDanmuku(CBaseDanmuku danmuku)
        {
            Program.host.Messages.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmuku));
        }

        private void Src_onConnected()
        {
            SourceStatus = true;
            IO.CurrentIO.Write(LANG_Source_Connected_Succ);
        }
        #endregion

        #region 连接方法
        private void StartSourceT()
        {
            IO.CurrentIO.Write(LANG_Source_Connect);
            SourceStatus = true;
            SrcThread = new Thread(StartSource);
            SrcThread.IsBackground = true;
            SrcThread.Start();
        }

        private void StopSourceT()
        {
            IO.CurrentIO.Write(LANG_Source_Disconnecting);
            SourceStatus = false;
            Src.Disconnect();
            while (Src.Stauts()) { Thread.Sleep(1); }
        }

        private void StartIRCT()
        {
            IO.CurrentIO.Write(LANG_IRC_Connecting);
            IRCStatus = true;
            IRCThread = new Thread(StartIRC);
            IRCThread.IsBackground = true;
            IRCThread.Start();
        }

        private void StopIRCT()
        {
            IO.CurrentIO.Write(LANG_IRC_Disconnect);
            IRCStatus = false;
            IRC.disconnect();
            while (IRC.isConnected) { Thread.Sleep(1); }
        }

        private void StartSource()
        {
            bool result = Src.Connect(/*int.Parse*/(Configuration.LiveRoomID));
            while (SourceStatus && IsConnect && Src.Stauts())
            {
                Thread.Sleep(1);
            }
            Src.Disconnect();
            while (Src.Stauts())
            {
                Thread.Sleep(1);
            }
        }

        private void StartIRC()
        {
            IRC.connect();
            while (IRCStatus && IsConnect && IRC.isConnected) { Thread.Sleep(1); }
            IRC.disconnect();
            IRCStatus = false;
        }

        #endregion

        /// <summary>
        /// 开始工作
        /// </summary>
        public void Connect()
        {
            IO.CurrentIO.Write(LANG_Start);
            IsConnect = true;
            StartIRCT();
            StartSourceT();
            Program.host.Plugins.StartSync();
        }

        /// <summary>
        /// 停止工作
        /// </summary>
        public void Disconnect()
        {
            IO.CurrentIO.Write(LANG_Stopping);
            IsConnect = false;
            if (IRCThread != null && IRCThread.IsAlive) StopIRCT();
            if (SrcThread != null && SrcThread.IsAlive) StopSourceT();
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