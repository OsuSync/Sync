using Sync.Client;
using Sync.MessageFilter;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Plugins
{
    public class MessageDispatcher
    {
        FilterManager filters;

        internal MessageDispatcher(FilterManager f)
        {
            filters = f;
            MessageManager.LimitLevel = 3;
            MessageManager.Option = MessageManager.PeekOption.Auto;
            MessageManager.Init(filters);
            MessageManager.SetSendMessageAction(new MessageManager.SendMessageAction((target,message) =>{
                    SyncHost.Instance.ClientWrapper.Client.SendMessage(new IRCMessage(target, message));
            }));

        }
        /// <summary>
        /// Send danmaku message to osu!IRC
        /// </summary>
        /// <param name="danmaku">Danmaku</param>
        public void onDanmaku(IBaseDanmakuEvent danmaku)
        {
            IMessageBase msg = new DanmakuMessage(danmaku);
            RaiseMessage<ISourceDanmaku>(msg);
        }

        /// <summary>
        /// Send osu!irc message to damaku
        /// </summary>
        /// <param name="user">Sender</param>
        /// <param name="message">Message</param>
        public void onIRC(StringElement user, StringElement message)
        {
            IMessageBase msg = new IRCMessage(user, message);
            RaiseMessage<ISourceClient>(msg);
        }



        public void RaiseMessage<Source>(IMessageBase msg)
        {
            RaiseMessage(typeof(Source), msg);
        }

        /// <summary>
        /// Raise a message and pass to filter list
        /// </summary>
        private void RaiseMessage(Type msgType, IMessageBase msg)
        {
            IMessageBase newMsg = msg;

            //From danmaku
            if (msgType == typeof(ISourceDanmaku))
            {
                filters.PassFilterDanmaku(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    //Send this danmaku message to osu!irc
                    MessageManager.PostIRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, newMsg);
                }
                return;
            }

            //From osu!orc
            else if (msgType == typeof(ISourceClient))
            {
                filters.PassFilterOSU(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    //Detect sender is osu!irc self
                    if (newMsg.User.RawText == SyncHost.Instance.ClientWrapper.Client.NickName)
                    {
                        //Send message to danmaku source
                        if (!SyncHost.Instance.SourceWrapper.Sendable)
                        {
                            IO.CurrentIO.WriteColor(DefaultI18n.LANG_SendNotReady, ConsoleColor.Red);
                        }
                        else
                        {
                            SyncHost.Instance.SourceWrapper.SendableSource.Send(newMsg);
                        }
                    }
                    //Send message to irc if Not sender
                    else
                    {
                        MessageManager.PostIRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, newMsg);
                    }

                }
                return;
            }

            //From danmaku gift(subscribe or other gift)
            else if (msgType == typeof(ISourceGift))
            {
                filters.PassFilterGift(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    SyncHost.Instance.ClientWrapper.Client?.SendMessage(new IRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, newMsg.Message));
                }
            }

            //The spectator count change
            else if (msgType == typeof(ISourceOnlineChange))
            {
                filters.PassFilterOnlineChange(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    SyncHost.Instance.ClientWrapper.Client?.SendMessage(new IRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, newMsg.Message));
                }
            }

        }
    }


    /// <summary>
    /// Message limiter
    /// </summary>
    public class MessageManager
    {
        class SendFilter : IFilter, ISourceDanmaku
        {
            public void onMsg(ref IMessageBase msg)
            {
                if (!msg.Message.RawText.StartsWith("?send"))
                    msg.Cancel = true;
                msg.Message = new StringElement(msg.Message.RawText.TrimStart("?send".ToArray()));
            }
        }

        public enum PeekOption
        {
            Force_All,
            Only_Send_Command,
            Auto
        }

        private static SendFilter sendFilter = new SendFilter();

        private static PeekOption option = PeekOption.Auto;
        public static PeekOption Option
        {
            set
            {
                if (option != value)
                {
                    if (option == PeekOption.Only_Send_Command)
                    {
                        filterManager.deleteFilter(sendFilter);
                        IO.CurrentIO.WriteColor(DefaultI18n.LANG_MsgMgr_Free, ConsoleColor.Green);
                    }
                    else if (value == PeekOption.Only_Send_Command)
                    {
                        filterManager.AddFilter(sendFilter);
                        IO.CurrentIO.WriteColor(DefaultI18n.LANG_MsgMgr_Limit, ConsoleColor.Green);
                    }

                }
                option = value;
            }
            get
            {
                return option;
            }
        }

        static System.Threading.Timer timer = null;
        static FilterManager filterManager = null;

        public static void Init(FilterManager manager)
        {
            timer = new System.Threading.Timer(runThread, null, 0, Convert.ToInt32(time_inv));
            filterManager = manager;
        }

        static float sendLimit_pre = 0;

        static float time_inv = 1000;
        static float time = 0;

        static float recoverTime = 60000;
        public static float RecoverTime
        {
            get { return recoverTime; }
        }

        public static int CurrentQueueCount
        {
            get { return MessageQueue.Count; }
        }

        static float currentRecoverTime = 0;

        static Object lockObj = new object();

        static volatile int sendCount = 0;
        public int CurrentCount
        {
            get
            {
                return sendCount;
            }
        }

        static int limitCount = 5;
        public static int LimitLevel
        {
            get { return limitCount; }
            set
            {
                limitCount = value;
                sendLimit_pre = (float)limitCount / (60000 / time_inv);
            }
        }

        private static volatile bool isLimit = false;
        public static bool IsLimit
        {
            get
            {
                return isLimit;
            }
        }

        static List<IMessageBase> MessageQueue = new List<IMessageBase>();

        public static void PostIRCMessage(string target, IMessageBase message)
        {
            lock (lockObj)
            {
                MessageQueue.Add(message);
                sendCount++;

                if (isLimit)
                    return;

                SendMessage(target, message.User + message.Message.ToString());
                MessageQueue.Remove(message);
            }
        }

        public delegate void SendMessageAction(string userName, string message);

        private static SendMessageAction action;

        public static void SetSendMessageAction(SendMessageAction action)
        {
            MessageManager.action = action;
        }

        public static void SendMessage(string userName, string message)
        {
            action(userName, message);
        }

        private static void runThread(object state)
        {
            time = time + time_inv;
            StringBuilder sb;

            if (option == PeekOption.Auto)
            {
                //Exceeded limit?
                if (!isLimit && (float)sendCount / (60000.0f / time_inv) >= sendLimit_pre)
                {
                    isLimit = true;
                    IO.CurrentIO.WriteColor("isLimit is true now", ConsoleColor.Yellow);
                    currentRecoverTime = 0;
                }

                //unlimit time
                currentRecoverTime += isLimit ? time_inv : 0;
                if (currentRecoverTime >= recoverTime)
                {
                    currentRecoverTime = 0;
                    isLimit = false;
                    IO.CurrentIO.WriteColor("isLimit is false now", ConsoleColor.Yellow);
                }
            }

            sendCount = 0;

            if (isLimit)
            {
                sb = new StringBuilder();
                IMessageBase message = null;
                lock (lockObj)
                {
                    while (MessageQueue.Count != 0)
                    {
                        message = MessageQueue[0];
                        MessageQueue.RemoveAt(0);
                        if (message == null)
                            break;
                        sb.AppendFormat("{0}:{1} || ", message.User, message.Message.RawText);
                    }
                }
                if (sb.Length != 0)
                    SendMessage("", sb.ToString());
            }
        }

    }
}
