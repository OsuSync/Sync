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
    public class ConsoleReciveSendOnlyClient : DefaultClient
    {
        public ConsoleReciveSendOnlyClient() : base("Deliay", "ConsoleReciveSendOnlyClient")
        {
            this.CurrentStatus = SourceStatus.IDLE;
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
