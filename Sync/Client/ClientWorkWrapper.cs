using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Client
{
    public class ClientWorkWrapper
    {
        public DefaultClient Client { get; internal set; }
        
        private ClientManager clients;

        public ClientWorkWrapper(ClientManager manager)
        {
            clients = manager;
            Client = clients.Clients.FirstOrDefault(p => p.ClientName == Configuration.Client);
            if(Client == null)
            {
                if (clients.Count == 0) return;
                Client = clients.Clients.First();
            }
        }
    }
}
