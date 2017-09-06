using Sync.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.MessageFilter;

namespace DefaultPlugin.Clients
{
    class DirectOSUIRCBot : DefaultClient
    {
        public DirectOSUIRCBot() : base("Deliay", "DirectOsuIRCBot")
        {
        }

        public override void Restart()
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(IMessageBase message)
        {
            throw new NotImplementedException();
        }

        public override void StartWork()
        {
            throw new NotImplementedException();
        }

        public override void StopWork()
        {
            throw new NotImplementedException();
        }

        public override void SwitchOtherClient()
        {
            throw new NotImplementedException();
        }

        public override void SwitchThisClient()
        {
            throw new NotImplementedException();
        }
    }
}
