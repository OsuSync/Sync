using Sync.MessageFilter;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Client
{
    public class ClientEvents : BaseEventDispatcher
    {
        public readonly static ClientEvents Instance = new ClientEvents();
        private ClientEvents()
        {
            EventDispatcher.Instance.RegistNewDispatcher(GetType());
        }
    }

    public interface ClientEvent : IBaseEvent { }

    public struct ClientStartWorkEvent : ClientEvent
    {
        public ClientWorkWrapper Client { get => SyncHost.Instance.ClientWrapper; }
    }

    public struct ClientStopWorkEvent : ClientEvent
    {

    }

    public struct ClientOnMessageEvent : ClientEvent
    {
        public IRCMessage Message { get; }

        public ClientOnMessageEvent(IRCMessage message)
        {
            Message = message;
        }
    }
}
