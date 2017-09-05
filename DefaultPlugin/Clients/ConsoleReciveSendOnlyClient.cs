using Sync.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.MessageFilter;
using Sync.Tools;
using Sync.Source;
using static Sync.SyncHost;

namespace DefaultPlugin.Clients
{
#if DEBUG
    /// <summary>
    /// Debug only
    /// </summary>
    class ConsoleReciveSendOnlyClient : DefaultReciveClient
    {
        public ConsoleReciveSendOnlyClient() : base("Deliay", "ConsoleReciveSendOnlyClient")
        {
            //此Client需要监听Source的事件
            SourceEvents.Instance.BindEvent<IBaseDanmakuEvent>(evt => Instance.Messages.RaiseMessage<ISourceClient>(new IRCMessage(evt.SenderName, evt.Danmuku)));
            SourceEvents.Instance.BindEvent<BaseOnlineCountEvent>(evt => Instance.Messages.RaiseMessage<ISourceClient>(new IRCMessage("Online", evt.Count.ToString())));
        }

        public override void Restart()
        {
            
        }

        public override void SendMessage(IMessageBase message)
        {
            IO.CurrentIO.WriteColor($"[Damaku Message] {message.User} : {message.Message}", ConsoleColor.Gray);
        }

        public override void StartWork()
        {
            EventBus.RaiseEvent(new ClientStartWorkEvent());
        }

        public override void StopWork()
        {
            EventBus.RaiseEvent(new ClientStopWorkEvent());
        }

        public override void SwitchOtherClient()
        {
            
        }

        public override void SwitchThisClient()
        {
            
        }
    }
#endif
}
