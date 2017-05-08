using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Meebey.SmartIrc4net;
using Sync.Tools;
using Sync.Source;
using Sync.MessageFilter;
using static Sync.Tools.DefaultI18n;

namespace Sync.IRC
{
    public class IRCClient
    {
        SyncConnector parent;
        IrcClient client = null;
        public const string CONST_ACTION_FLAG = "\x0001ACTION ";
        string username = Configuration.BotIRC;
        string password = Configuration.BotIRCPassword;
        string master = Configuration.TargetIRC;

        public IRCClient(SyncConnector p)
        {
            parent = p;
            client = new IrcClient();
        }

        public bool isConnected = false;
        public void connect()
        {
            client.EnableUTF8Recode = true;

            try {
                client.Connect(new string[] { "cho.ppy.sh", "irc.ppy.sh" }, 6667);
            }
            catch
            {
                IO.CurrentIO.WriteColor(LANG_IRC_Connect_Timeout, ConsoleColor.Red);
                return;
            }
            username = Configuration.BotIRC;
            password = Configuration.BotIRCPassword;
            master = Configuration.TargetIRC;
            client.Login(username, username, 1, username, password);

            client.OnConnected += Client_OnConnected;
            client.OnConnecting += Client_OnConnecting;
            client.OnRawMessage += Client_OnRawMessage;
            client.OnWriteLine += Client_OnWriteLine;
            client.OnDisconnected += Client_OnDisconnected;

            isConnected = client.IsConnected;

            client.Listen();
        }

        private void Client_OnConnected(object sender, EventArgs e)
        {
            IO.CurrentIO.Write(LANG_IRC_Ready);
        }

        private void Client_OnConnecting(object sender, EventArgs e)
        {
            IO.CurrentIO.Write(LANG_IRC_Connecting);
        }

        private void Client_OnRawMessage(object sender, IrcEventArgs e)
        {
            if (e.Data.Type == ReceiveType.ChannelAction || e.Data.Type == ReceiveType.QueryAction || e.Data.Type == ReceiveType.QueryMessage || e.Data.Type == ReceiveType.ChannelMessage)
            {
                Program.host.Messages.RaiseMessage<ISourceOsu>(new IRCMessage(e.Data.Nick, e.Data.Message));
            }
        }

        private void Client_OnWriteLine(object sender, WriteLineEventArgs e)
        {
            //IO.CurrentIO.Write("[IRC] " + e.Line);
            return;
        }

        public void disconnect()
        {
            isConnected = false;
            if (client != null && client.IsConnected) client.Disconnect();
        }

        private void Client_OnDisconnected(object sender, EventArgs e)
        {
            isConnected = false;
        }

        public void sendRawMessage(string user, string msg)
        {
            client.WriteLine("PRIVMSG " + user + " :" + msg);
        }
    }
}
