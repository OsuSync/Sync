using Sync.IRC;
using Sync.Source;
using Sync.Tools;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Sync
{
    /// <summary>
    /// 连接逻辑类
    /// </summary>
    class Sync
    {
        private IRCClient IRC;
        private ISourceBase Src;

        private Thread IRCThread;
        private Thread SrcThread;

        private GiftRecycler giftRec;

        public bool IRCStatus = false;
        public bool SourceStatus = false;

        public bool IsConnect = false;
        private uint usercount;

        public Thread GetThreadIRC() { return IRCThread; }
        public Thread GetThreadSource() { return SrcThread; }
        public IRCClient GetInstanceIRC() { return IRC; }
        public ISourceBase GetInstanceSource() { return Src; }

        public void IRCSendMessage(string msg)
        {
            if(IRCStatus)
            IRC.sendMessage(Meebey.SmartIrc4net.SendType.Message, msg);
        }

        public void IRCSendAction(string msg)
        {
            if(IRCStatus)
            IRC.sendMessage(Meebey.SmartIrc4net.SendType.Action, msg);
        }

        /// <summary>
        /// 用连接源实例化一个Sync类
        /// </summary>
        /// <param name="Source">连接源</param>
        public Sync(ISourceBase Source)
        {
            IRC = new IRCClient();
            Src = Source;

            Src.onConnected += Src_onConnected;
            Src.onDisconnected += Src_onDisconnected;
            Src.onDanmuku += Src_onDanmuku;
            Src.onOnlineChange += Src_onOnlineChange;
            Src.onGift += Src_onGift;

            giftRec = new GiftRecycler();

            SrcThread = new Thread(StartSource);
            IRCThread = new Thread(StartIRC);
        }

        #region 连接源的事件
        private void Src_onGift(CBaseGift gift)
        {
            giftRec.AddGift(gift);
        }

        private void Src_onOnlineChange(uint lCount)
        {
            ConsoleWriter.Write("用户变更:" + lCount);
            if (Math.Abs(usercount - lCount) > 4) 
            {
                Program.syncInstance.IRCSendAction("直播间围观人数" + (usercount > lCount ? "减少" : "增加" ) +"到" 
                                                    + lCount + "人");
                usercount = lCount;
            }
        }

        private void Src_onDisconnected()
        {
            SourceStatus = false;
            ConsoleWriter.Write("源服务器断开连接成功！");
            
        }

        private void Src_onDanmuku(CBaseDanmuku danmuku)
        {
            string syncText = "[弹幕]<" + danmuku.senderName + "> " + danmuku.danmuku;
            Console.WriteLine(syncText);
            IRCSendMessage(syncText);
        }

        private void Src_onConnected()
        {
            SourceStatus = true;
            ConsoleWriter.Write("源服务器连接成功！");
            giftRec.StartRecycler();
        }
        #endregion

        #region 连接方法
        private void StartSourceT()
        {
            ConsoleWriter.Write("正在连接弹幕源服务器....");
            SourceStatus = true;
            SrcThread.Start();
        }

        private void StopSourceT()
        {
            ConsoleWriter.Write("正在断开弹幕源服务器的连接....");
            SourceStatus = false;
            Src.Disconnect();
        }

        private void StartIRCT()
        {
            ConsoleWriter.Write("正在连接IRC服务器....");
            IRCStatus = true;
            IRCThread.Start();
        }

        private void StopIRCT()
        {
            ConsoleWriter.Write("正在断开IRC服务器的连接....");
            IRCStatus = false;
            IRC.disconnect();
        }

        private void StartSource()
        {
            Src.Connect(Configuration.LiveRoomID);
            while (Src.Stauts() || SourceStatus || IsConnect) { }
            Src.Disconnect();
        }

        private void StartIRC()
        {
            IRC.connect();
            while (IRC.isConnected || IRCStatus || IsConnect) { Console.Title = "IRC:" + IRC.isConnected; }
            IRC.disconnect();
        }

        #endregion

        /// <summary>
        /// 开始工作
        /// </summary>
        public void Connect()
        {
            ConsoleWriter.Write("开始工作");
            IsConnect = true;
            StartIRCT();
            StartSourceT();
        }

        /// <summary>
        /// 停止工作
        /// </summary>
        public void Disconnect()
        {
            ConsoleWriter.Write("正在停止工作……");
            StopIRCT();
            StopSourceT();
        }

        /// <summary>
        /// 重新连接(未实现)
        /// </summary>
        [Obsolete]
        public void Reconnect()
        {
            ConsoleWriter.Write("重新开始工作中……");
            Disconnect();
            Connect();
        }
    }
}