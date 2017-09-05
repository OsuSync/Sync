using Sync.MessageFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Client
{
    public class ClientManager
    {
        private LinkedList<DefaultReciveClient> clients;
        public IReadOnlyList<DefaultReciveClient> Clients { get => clients.ToList(); }
        public int Count { get => clients.Count; }

        public static readonly ClientManager Instance = new ClientManager();
        private ClientManager()
        {
            clients = new LinkedList<DefaultReciveClient>();
        }

        public bool AddClient(DefaultReciveClient client)
        {
            if(clients.Contains(client))
            {
                return false;
            }
            else
            {
                clients.AddLast(client);
                return true;
            }
        }
    }
}
