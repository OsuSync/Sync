using Sync.MessageFilter;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Client
{
    /// <summary>
    /// Singleton Client event dispathcher
    /// </summary>
    public class ClientEvents : BaseEventDispatcher
    {
        public readonly static ClientEvents Instance = new ClientEvents();
        private ClientEvents()
        {
            EventDispatcher.Instance.RegisterNewDispatcher(GetType());
        }
    }

    /// <summary>
    /// Base client event interface flag
    /// </summary>
    public interface IClientEvent : IBaseEvent { }

    /// <summary>
    /// Fire when client start work (fire time decide by client)
    /// </summary>
    public struct ClientStartWorkEvent : IClientEvent
    {
        public ClientWorkWrapper Client { get => SyncHost.Instance.ClientWrapper; }
    }

    /// <summary>
    /// Fire when client stop work
    /// </summary>
    public struct ClientStopWorkEvent : IClientEvent
    {

    }

    /// <summary>
    /// Fire when Client recive a IRCMessage
    /// </summary>
    public struct ClientOnMessageEvent : IClientEvent
    {
        public IRCMessage Message { get; }

        public ClientOnMessageEvent(IRCMessage message)
        {
            Message = message;
        }
    }
}
