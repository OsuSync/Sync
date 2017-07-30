using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sync.Tools;
using Sync.Source;
using Sync.MessageFilter;

namespace Sync.Client
{
    public class CooCClient
    {
        SyncConnector parent;

        public const string CONST_ACTION_FLAG = "\x0001ACTION ";
        string username = Configuration.CoocAccount;
        string password = Configuration.CoocPassword;
        string master = Configuration.TargetIRC;

        public CooCClient(SyncConnector p)
        {
            parent = p;
        }

        public bool isConnected = false;
        public void connect()
        {

        }

        private void OnRawMessage(string Nick, string Message)
        {
            Program.host.Messages.RaiseMessage<ISourceOsu>(new IRCMessage(Nick, Message));

        }

        public void disconnect()
        {
            isConnected = false;

        }

        private void Client_OnDisconnected(object sender, EventArgs e)
        {
            isConnected = false;
        }

        public void sendRawMessage(string user, string msg)
        {
            //client.WriteLine("PRIVMSG " + user + " :" + msg);
        }
    }
}
