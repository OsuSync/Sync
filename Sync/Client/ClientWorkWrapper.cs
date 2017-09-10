using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Client
{
    /// <summary>
    /// Client instance switch and dispatch
    /// </summary>
    public class ClientWorkWrapper
    {
        public DefaultClient Client { get; internal set; }
        
        private ClientManager clients;

        public ClientWorkWrapper(ClientManager manager)
        {
            clients = manager;
            ResetClient();
        }

        /// <summary>
        /// Call when reload client form confiuration file
        /// </summary>
        public void ResetClient()
        {
            DefaultClient client = clients.Clients.FirstOrDefault(p => p.ClientName == Configuration.Client);

            if (Client != null && Client != client)
            {
                Client.SwitchOtherClient();
            }
            
            if (client == null)
            {
                if (clients.Count == 0) return;
                client = clients.Clients.First();
            }


            Client = client;

            Client.SwitchThisClient();
        }
    }
}
