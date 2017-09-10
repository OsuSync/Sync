using Sync.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.MessageFilter;
using System.Net.Sockets;
using System.IO;
using Sync.Tools;
using System.Threading;
using Sync.Source;
using System.Text.RegularExpressions;

namespace DefaultPlugin.Clients
{
    public class DirectOSUIRCBot : DefaultClient, IConfigurable
    {
        public const string CONST_ACTION_FLAG = "\x0001ACTION ";

        TcpClient tcpClient;
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;

        Thread recive, send;

        bool workStatus = false;

        Queue<string> messageQueue;

        public static ConfigurationElement IRCBotName { get; set; } = "";
        public static ConfigurationElement IRCBotPasswd { get; set; } = "";
        public static ConfigurationElement IRCNick { get; set; } = "";

        static readonly Regex msgRegex = new Regex(":(.+?)!.+?:(.*?)$");

        public DirectOSUIRCBot() : base("Deliay", "DirectOsuIRCBot")
        {
            messageQueue = new Queue<string>();
        }

        public override void Restart()
        {
            StopWork();
            StartWork();
        }

        public override void SendMessage(IMessageBase message)
        {
            SendRawMessage($"PRIVMSG {message.User} :{message.Message}");
        }

        private void SendRawMessage(string msg)
        {
            messageQueue.Enqueue(msg);
        }

        private void ReciveRawMessage(string msg)
        {

            if (!msg.Contains(@"PRIVMSG "))
            {
                //处理非对话消息
                if (msg.StartsWith("PING "))
                    SendRawMessage(msg.Replace(@"PING", @"PONG"));
            }
            else
            {
                Match match = msgRegex.Match(msg);
                string nick = match.Groups[1].Value;
                string rawmsg = match.Groups[2].Value;
                DefaultPlugin.MainMessager.RaiseMessage<ISourceClient>(new IRCMessage(nick, rawmsg));
            }
        }

        private void ReciveLoop()
        {
            while(workStatus)
            {
                if (ns.DataAvailable)
                {
                    string message = sr.ReadLine();

                    ReciveRawMessage(message);
                }
                Thread.Sleep(1);
            }
        }

        private void SendLoop()
        {
            while (workStatus)
            {
                Thread.Sleep(1);
                if(messageQueue.Count > 0)
                {
                    if (!tcpClient.Connected)
                    {
                        ConnectAndLogin();
                    }

                    string message = string.Empty;
                    lock (messageQueue)
                    {
                        message = messageQueue.Dequeue();
                    }
                    if (message == string.Empty) continue;
                    if (!tcpClient.Connected)
                    {
                        workStatus = false;
                        CurrentStatus = SourceStatus.REMOTE_DISCONNECTED;
                        continue;
                    }
                    sw.WriteLine(message);
                    sw.Flush();
                    message = string.Empty;
                }
            }
        }


        public override void StartWork()
        {
            EventBus.RaiseEvent(new ClientStartWorkEvent());
            ConnectAndLogin();
            recive = new Thread(ReciveLoop);
            send = new Thread(SendLoop);
            recive.Start();
            send.Start();
        }

        private void ConnectAndLogin()
        {
            if (workStatus) return;
            tcpClient = new TcpClient();
            workStatus = true;

            tcpClient.Connect("irc.ppy.sh", 6667);
            if (!tcpClient.Connected)
            {
                throw new Exception("Network error!");
            }
            CurrentStatus = SourceStatus.CONNECTED_WAITING;
            ns = tcpClient.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);

            IRCLogin();

        }

        private void IRCLogin()
        {
            sw.WriteLine($"PASS {IRCBotPasswd}");
            sw.WriteLine($"USER {IRCBotName} 1 * : {IRCBotName}");
            sw.WriteLine($"NICK {IRCBotName}");
            SendMessage(new IRCMessage(IRCNick.ToString(), "[DirectOSUIRCBot]Welcome!"));
            sw.Flush();

            CurrentStatus = SourceStatus.CONNECTED_WORKING;

        }

        public override void StopWork()
        {
            if(tcpClient.Connected)
            {
                sw.Write("QUIT");
                sw.Flush();
            }
            workStatus = false;
            EventBus.RaiseEvent(new ClientStopWorkEvent());
        }

        public override void SwitchOtherClient()
        {
            StopWork();
        }

        public override void SwitchThisClient()
        {
            CurrentStatus = SourceStatus.IDLE;
        }

        public void onConfigurationLoad()
        {
            this.NickName = IRCNick;
        }

        public void onConfigurationSave()
        {
            
        }
    }
}
