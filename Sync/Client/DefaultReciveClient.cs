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
    public abstract class DefaultClient
    {

        //Author and Client Name
        public string Author { get; }
        public string ClientName { get; }

        public BaseEventDispatcher<IClientEvent> EventBus { get => ClientEvents.Instance; }

        public string NickName { get; protected set; }

        /// <summary>
        /// Invoke while user switch to other client instance
        /// </summary>
        public abstract void SwitchOtherClient();
        /// <summary>
        /// Invoke while user switch to this instance
        /// </summary>
        public abstract void SwitchThisClient();

        public DefaultClient(string Author, string Name)
        {
            this.Author = Author;
            this.ClientName = Name;
        }


        /// <summary>
        /// Raise event and pass messages
        /// </summary>
        /// <param name="msg">Bypass message</param>
        protected void EnqueueMessage(IRCMessage msg)
        {
            EventBus.RaiseEvent(new ClientOnMessageEvent(msg));
        }

        public abstract void StartWork();
        public abstract void StopWork();
        public abstract void Restart();

        public SourceStatus CurrentStatus { get; protected set; }

        /// <summary>
        /// SendMessage to Client
        /// WARNING: SEND MESSAGE DIRECTLY WHIT THIS WILL NOT PASS BY FILTERS!
        /// </summary>
        /// <param name="message"></param>
        public abstract void SendMessage(IMessageBase message);

    }
}
