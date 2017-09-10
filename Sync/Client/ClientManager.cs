using Sync.MessageFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Client
{
    /// <summary>
    /// The manager for Clients
    /// </summary>
    public class ClientManager
    {
        private LinkedList<DefaultClient> clients;
        public IReadOnlyList<DefaultClient> Clients { get => clients.ToList(); }
        public int Count { get => clients.Count; }

        public static readonly ClientManager Instance = new ClientManager();
        private ClientManager()
        {
            clients = new LinkedList<DefaultClient>();
        }

        public void AddAllClient(params DefaultClient[] clients)
        {
            foreach (var client in clients)
            {
                if (!this.clients.Contains(client))
                {
                    this.clients.AddLast(client);
                }
            }
        }

        public bool AddClient(DefaultClient client)
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
