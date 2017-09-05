using Sync.MessageFilter;
using Sync.Plugins;
using Sync.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Client
{
    public abstract class DefaultReciveClient
    {

        public string Author { get; }
        public string ClientName { get; }

        public BaseEventDispatcher EventBus { get => ClientEvents.Instance; }

        public string NickName { get; }

        private Queue<IRCMessage> pending_messages;

        /// <summary>
        /// Invoke while user switch to other client instance
        /// </summary>
        public abstract void SwitchOtherClient();
        /// <summary>
        /// Invoke while user switch to this instance
        /// </summary>
        public abstract void SwitchThisClient();

        public DefaultReciveClient(string Author, string Name)
        {
            pending_messages = new Queue<IRCMessage>();
            this.Author = Author;
            this.ClientName = Name;
        }

        /// <summary>
        /// Check for next new message
        /// </summary>
        /// <returns></returns>
        public bool HaveNextMessage()
        {
            return pending_messages.Count > 0;
        }

        /// <summary>
        /// Dequeue next message
        /// </summary>
        /// <returns></returns>
        public IRCMessage NextMessage()
        {
            return pending_messages.Dequeue();
        }

        /// <summary>
        /// Enqueue next message
        /// </summary>
        /// <param name="nick"></param>
        /// <param name="message"></param>
        protected void EnqueueMessage(IRCMessage msg)
        {
            pending_messages.Enqueue(msg);
            EventBus.RaiseEvent(new ClientOnMessageEvent(msg));
        }

        public abstract void StartWork();
        public abstract void StopWork();
        public abstract void Restart();

        public SourceStatus CurrentStatus { get; protected set; }

        /// <summary>
        /// 警告，此方法不应随意调用，请用MessageDispather里的RaiseMessage来发送消息
        /// </summary>
        /// <param name="message"></param>
        public abstract void SendMessage(IMessageBase message);

    }
}
