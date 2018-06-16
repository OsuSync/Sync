using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sync.MessageFilter
{
    /// <summary>
    /// Message limiter
    /// </summary>
    public class MessageManager
    {
        private static float sendLimit_pre = 0;

        private static int time_inv = 1000;
        private static float time = 0;

        private static float recoverTime = 60000;

        private static SendFilter sendFilter = new SendFilter();

        public static PeekOption Option { set; get; } = PeekOption.Auto;

        private static Thread thread = null;

        public static float RecoverTime
        {
            get { return recoverTime; }
        }

        public static int CurrentQueueCount
        {
            get { return MessageQueue.Count; }
        }

        private static float currentRecoverTime = 0;

        private static Object lockObj = new object();

        private static volatile int sendCount = 0;

        public int CurrentCount
        {
            get
            {
                return sendCount;
            }
        }

        private static int limitCount = 5;

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

        private static SendMessageAction action;
        
        private static List<MessageBaseWrapper> MessageQueue = new List<MessageBaseWrapper>();

        private struct MessageBaseWrapper
        {
            public DateTime time;
            public string target;
            public IMessageBase message;
        }

        private class SendFilter : IFilter, ISourceDanmaku
        {
            public void onMsg(ref IMessageBase msg)
            {
                switch (Option)
                {
                    case PeekOption.ForceAll://还要过滤可能存在的?前缀
                        break;

                    case PeekOption.DisableAll:
                        msg.Cancel = true;
                        return;

                    case PeekOption.OnlySendCommand:
                        if (!msg.Message.RawText.StartsWith("?send"))
                        {
                            msg.Cancel = true;
                            return;
                        }
                        break;

                    case PeekOption.Auto:
                        if (IsLimit && (!msg.Message.RawText.StartsWith("?send")))
                        {
                            msg.Cancel = true;
                            return;
                        }
                        break;

                    default:
                        break;
                }

                if (msg.Message.RawText.StartsWith("?send "))
                    msg.Message = new StringElement(msg.Message.RawText.Remove(0, 5));
            }
        }

        public enum PeekOption
        {
            ForceAll,
            DisableAll,
            OnlySendCommand,
            Auto
        }

        public static void Init(FilterManager manager)
        {
            thread = new Thread(runThread);
            SyncHost.Instance.Filters.AddFilter(sendFilter);
            thread.Start();
        }
        
        public static void PostIRCMessage(string target, IMessageBase message)
        {
            lock (lockObj)
            {
                MessageQueue.Add(new MessageBaseWrapper() {
                    time=DateTime.Now,
                    target=target,
                    message =message
                });

                sendCount++;
            }
        }

        public static void SetOption(string opt_name)
        {
            switch (opt_name.ToLower().Trim())
            {
                case "onlysendcommand":
                    Option = PeekOption.OnlySendCommand;
                    break;
                case "disableall":
                    Option = PeekOption.DisableAll;
                    break;
                case "forceall":
                    Option = PeekOption.ForceAll;
                    break;
                case "auto":
                default:
                    Option = PeekOption.Auto;
                    break;
            }
        }

        public delegate void SendMessageAction(string userName, string message);

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
            StringBuilder sb=new StringBuilder();

            while (true)
            {
                Thread.Sleep(time_inv);

                sb.Clear();
                
                time = time + time_inv;

                if (Option == PeekOption.Auto)
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
                    lock (lockObj)
                    {
                        while (MessageQueue.Count != 0)
                        {
                            var message = MessageQueue[0];
                            MessageQueue.RemoveAt(0);
                            if (message.message == null)
                                break;
                            sb.AppendFormat("{0}:{1} || ", message.message.User, message.message.Message.RawText);
                        }
                    }

                    if (sb.Length != 0)
                        SendMessage("", sb.ToString());
                }
                else
                {
                    if (MessageQueue.Count != 0)
                    {
                        var message = MessageQueue[0];
                        MessageQueue.RemoveAt(0);
                        SendMessage(message.target, message.message.User + message.message.Message.ToString());
                    }
                }
            }
        }
    }
}