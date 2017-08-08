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

        SyncConnector parent;
        FilterManager filters;
        internal MessageDispatcher(SyncConnector p, FilterManager f)
        {
            parent = p;

            filters = f;

            MessageManager.LimitLevel = 3;
            MessageManager.Option = MessageManager.PeekOption.Auto;
            MessageManager.Init(filters);
            MessageManager.SetSendMessageAction(new MessageManager.SendMessageAction((target,message) =>{
                parent.Client.sendRawMessage(Configuration.TargetIRC, message);
            }));

        }
        /// <summary>
        /// 简易实现直接传递弹幕消息
        /// </summary>
        /// <param name="danmaku">弹幕</param>
        public void onDanmaku(IBaseDanmakuEvent danmaku)
        {
            IMessageBase msg = new DanmakuMessage(danmaku);
            RaiseMessage<ISourceDanmaku>(msg);
        }

        /// <summary>
        /// 简易实现的传递IRC消息
        /// </summary>
        /// <param name="user">发信人</param>
        /// <param name="message">信息</param>
        public void onIRC(StringElement user, StringElement message)
        {
            IMessageBase msg = new IRCMessage(user, message);
            RaiseMessage<ISourceOsu>(msg);
        }



        public void RaiseMessage<Source>(IMessageBase msg)
        {
            RaiseMessage(typeof(Source), msg);
        }

        /// <summary>
        /// 产生一个消息  
        /// 该消息会被按顺序编译
        /// </summary>
        /// <param name="msgType">消息类型，此处传IOsu(来自IRC)和IDanmaku(来自弹幕)</param>
        /// <param name="msg">具体消息实例</param>
        private void RaiseMessage(Type msgType, IMessageBase msg)
        {
            IMessageBase newMsg = msg;

            //消息来自弹幕
            if (msgType == typeof(ISourceDanmaku))
            {
                filters.PassFilterDanmaku(ref newMsg);
                //将消息过滤一遍插件之后，判断是否取消消息（消息是否由插件自行处理拦截）
                if (newMsg.Cancel) return;
                else
                {
                    //parent.GetIRC().sendRawMessage(Configuration.TargetIRC, newMsg.user + newMsg.message.RawText);
                    MessageManager.PostIRCMessage(Configuration.TargetIRC, newMsg);
                }
                return;
            }

            //消息来自osu!IRC
            else if (msgType == typeof(ISourceOsu))
            {
                filters.PassFilterOSU(ref newMsg);
                //同上
                if (newMsg.Cancel) return;
                else
                {
                    //发信用户为设置的目标IRC
                    if (newMsg.User.RawText == Configuration.TargetIRC)
                    {
                        if (parent.Source.SupportSend)
                        {
                            parent.Source.Send(newMsg.Message);
                        }
                    }
                    //其他用户则转发到目标IRC
                    else
                    {
                        MessageManager.PostIRCMessage(Configuration.TargetIRC, newMsg);
                    }

                }
                return;
            }

            //消息来自弹幕礼物
            else if (msgType == typeof(ISourceGift))
            {
                filters.PassFilterGift(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    parent.Client.sendRawMessage(Configuration.TargetIRC, Client.CooCClient.CONST_ACTION_FLAG + newMsg.Message);
                }
            }

            //观看人数变化
            else if (msgType == typeof(ISourceOnlineChange))
            {
                filters.PassFilterOnlineChange(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    parent.Client.sendRawMessage(Configuration.TargetIRC, Client.CooCClient.CONST_ACTION_FLAG + newMsg.Message);
                }
            }

        }
    }


    /// <summary>
    /// 消息管理器,管制消息的发送
    /// </summary>
    public class MessageManager
    {
        class SendFilter : IFilter, ISourceDanmaku
        {
            public void onMsg(ref IMessageBase msg)
            {
                if (!msg.Message.RawText.StartsWith("?send"))
                    msg.Cancel = true;
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
                option = value;
                if (option != value)
                {
                    if (option == PeekOption.Only_Send_Command)
                        filterManager.deleteFilter(sendFilter);
                    else if (value == PeekOption.Only_Send_Command)
                        filterManager.AddFilter(sendFilter);
                }
                isLimit = false;
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

                SendMessage(target, message.User + message.Message.RawText);
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
                //判断是否超出限定
                if (!isLimit && (float)sendCount / (60000.0f / time_inv) >= sendLimit_pre)
                {
                    isLimit = true;
                    IO.CurrentIO.WriteColor("isLimit is true now", ConsoleColor.Yellow);
                    currentRecoverTime = 0;
                }

                //解禁时间
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
