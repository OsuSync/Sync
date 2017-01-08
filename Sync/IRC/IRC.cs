using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Meebey.SmartIrc4net;
using Sync.Tools;
using Sync.Source;

namespace Sync.IRC
{
    class IRCClient
    {
        Sync parent;
        IrcClient client = new IrcClient();
        string username = Configuration.BotIRC;
        string password = Configuration.BotIRCPassword;
        string master = Configuration.TargetIRC;

        public IRCClient(Sync p)
        {
            parent = p;
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
                ConsoleWriter.WriteColor("osu! IRC连接错误，无法连接到老板小霸王服务器 !!", ConsoleColor.Red);
                ConsoleWriter.WriteColor("请稍后重试或者开一个VPN。", ConsoleColor.Red);
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
            ConsoleWriter.Write("[IRC] osu! IRC 已经准备就绪!");
        }

        private void Client_OnConnecting(object sender, EventArgs e)
        {
            ConsoleWriter.Write("[IRC] osu! IRC正在连接中..");
        }

        private void Client_OnRawMessage(object sender, IrcEventArgs e)
        {
            if (e.Data.Type == ReceiveType.ChannelAction || e.Data.Type == ReceiveType.QueryAction || e.Data.Type == ReceiveType.QueryMessage || e.Data.Type == ReceiveType.ChannelMessage)
            {
                if (e.Data.From.Substring(0, master.Length) == master)
                {
                    if (e.Data.Message.Length > 7 && e.Data.Message.Substring(0, 8) == "ACTION " && e.Data.Message.IndexOf("osu.ppy.sh/b/") > 0)
                    {
                        client.WriteLine("PRIVMSG tillerino :" + e.Data.Message);
                    }
                    else
                    {
                        if(Program.loginable)
                        {
                            ISendable dsender = (ISendable)parent.GetInstanceSource();
                            if(dsender.LoginStauts())
                            {
                                dsender.Send(e.Data.Message);
                            }
                        }
                    }

                }
                else if (e.Data.From.Substring(0, 9).ToLower() == "tillerino")
                {
                    sendMessage(SendType.Message, "[PP查询] " + e.Data.Message);
                }
                else
                {
                    ConsoleWriter.Write("[IRC] <" + e.Data.From + "> " + e.Data.Message);
                    sendMessage(SendType.Message, "[IRC] <" + e.Data.From + "> " + e.Data.Message);
                }
            }
        }

        private void Client_OnWriteLine(object sender, WriteLineEventArgs e)
        {
            ConsoleWriter.Write("[IRC] " + e.Line);
        }

        public void disconnect()
        {
            client.WriteLine("QUIT :quit");
            if (client.IsConnected) client.Disconnect();
        }

        private void Client_OnDisconnected(object sender, EventArgs e)
        {
            isConnected = false;
        }

        public void sendMessage(SendType type, string msg)
        {
            switch (type)
            {
                case SendType.Action:
                    client.WriteLine("PRIVMSG " + master + " :ACTION " + msg);
                    break;
                case SendType.Message:
                    client.WriteLine("PRIVMSG " + master + " :" + msg);
                    break;
                default:
                    break;
            }
        }
    }
}
