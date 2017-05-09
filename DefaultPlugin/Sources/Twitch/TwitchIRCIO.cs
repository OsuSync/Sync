using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace DefaultPlugin.Sources.Twitch
{
    /// <summary>
    /// TwitchIRC通信类，用于接收/发送Comment
    /// ChannelName指频道，类似于bilibili live的room id
    /// oauth:pjaicvg4jon0o0doiwjlo5z9s05a7l
    /// Client-ID:esmhw2lcvrgtqw545ourqjwlg7twee
    /// </summary>

    public class TwitchIRCIO
    {
        #region 属性字段
        TcpClient clientSocket;
        NetworkStream rawSocketStream;
        StreamReader inputStreamReciever;
        StreamWriter outputStreamSender;

        string oauth = @"oauth:pjaicvg4jon0o0doiwjlo5z9s05a7l";
        string name = @"osuSync";
        string client_id = "esmhw2lcvrgtqw545ourqjwlg7twee";
        string ircAddress = @"irc.twitch.tv";
        int ircPort = 6667;
        string channelName = null;

        int sleepInterval = 30;

        int prev_peopleCount = -1;
        int tmpCurrent_peopleCount = 0;

        public bool IsConnected
        {
            get
            {
                return clientSocket != null && clientSocket.Connected;
            }
        }

        public string OAuth { get { return oauth; } set { oauth = value; } }

        public string ClientID { get { return client_id; } set { client_id = value; } }

        public string ChannelName { get { return channelName; } set { channelName = value; } }

        bool isLooping = false;

        Thread outputThread/*发送线程*/, inputThread/*接收线程*/;
        List<string> rawOutputMsgList = new List<string>();

        public delegate void OnRecieveMessageFunc(string rawMessage);

        public delegate void OnNamesCountFunc(int newPeopleCount);

        /// <summary>
        /// 触发接收消息事件
        /// </summary>
        public event OnRecieveMessageFunc OnRecieveRawMessage;
        public event OnNamesCountFunc OnNamesCountUpdate;
        #endregion

        public TwitchIRCIO(string channelName)
        {
            this.channelName = channelName;
        }

        #region 实现

        private void initStart()
        {
            if (isLooping)
            {
                return;
            }

            clientSocket = new TcpClient();

            isLooping = true;

            clientSocket.Connect(ircAddress, ircPort);
            if (!clientSocket.Connected)
            {
                //failed
                throw new Exception("Cant connect to server.");
            }

            rawSocketStream = clientSocket.GetStream();
            outputStreamSender = new StreamWriter(rawSocketStream);
            inputStreamReciever = new StreamReader(rawSocketStream);

            outputStreamSender.WriteLine($"PASS {oauth}\nNICK {name.ToLower()}");
            outputStreamSender.Flush();

            outputStreamSender.WriteLine(@"CAP REQ :twitch.tv/membership");
            outputStreamSender.Flush();
        }

        /// <summary>
        /// 负责接收并处理消息
        /// </summary>
        private void inputThreadFunc()
        {
            while (isLooping)
            {
                Thread.Sleep(sleepInterval);

                //没消息？吃惊，睡觉
                if (!rawSocketStream.DataAvailable)
                    continue;

                string message = inputStreamReciever.ReadLine();

                if (!message.Contains(@"PRIVMSG #"))
                {
                    //处理非对话消息
                    if (message.StartsWith("PING "))
                        SendRawMessage(message.Replace(@"PING", @"PONG"));
                    else
                    {
                        var result = message.Split(' ');
                        if (result.Length > 1)
                            processCommandMessage(result[1], message);
                    }
                }
                else
                    OnRecieveRawMessage?.Invoke(message);
            }

            //释放资源?
        }

        private void processCommandMessage(string messageID, string rawMessage)
        {
            switch (messageID)
            {
                case "001":
                    {
                        SendRawMessage($"JOIN #{channelName}");
                    }
                    break;

                case "353":
                    {
                        int position = rawMessage.LastIndexOf(':');
                        int count = rawMessage.Substring(position).Split(' ').Length;
                        tmpCurrent_peopleCount += count;
                    }
                    break;

                case "366":
                    {
                        if (tmpCurrent_peopleCount != prev_peopleCount)
                        {
                            OnNamesCountUpdate?.Invoke(tmpCurrent_peopleCount);
                            prev_peopleCount = tmpCurrent_peopleCount;
                        }
                        tmpCurrent_peopleCount = 0;
                    }
                    break;
            }
        }

        /// <summary>
        /// 负责发送消息
        /// </summary>
        private void outputThreadFunc()
        {
            Stopwatch timer = new Stopwatch();
            timer.Reset();
            timer.Start();
            int sendCount = 0;

            while (isLooping)
            {
                Thread.Sleep(sleepInterval);

                //twitch irc限制每30秒最多能发20条信息否则就是被圣神制裁45分钟
                if (timer.ElapsedMilliseconds > 30000)
                {
                    sendCount = 0;
                    timer.Reset();
                    timer.Start();
                }

                if (sendCount >= 20)
                    continue;//超过限制，先退出

                if (rawOutputMsgList.Count == 0)
                    continue;

                lock (rawOutputMsgList)
                {
                    while (rawOutputMsgList.Count != 0)
                    {
                        var message = rawOutputMsgList[0];
                        rawOutputMsgList.RemoveAt(0);

                        if (!clientSocket.Connected)
                        {
                            Reconnect();
                        }

                        outputStreamSender.WriteLine(message);
                        outputStreamSender.Flush();

                        sendCount++;
                        if (sendCount >= 20)
                            break;
                    }
                }
            }

            //释放资源?
            timer.Stop();
        }

        #endregion

        #region 公共rbq方法

        public void Reconnect()
        {
            if (isLooping)
                initStart();
            else
                Connect();
        }

        public void Connect()
        {
            if (isLooping)
                return;

            outputThread = new Thread(outputThreadFunc);
            inputThread = new Thread(inputThreadFunc);

            initStart();

            outputThread.Start();
            inputThread.Start();
        }

        public void DisConnect()
        {
            isLooping = false;

            rawOutputMsgList.Clear();
        }

        public void SendMessage(string message, string channel = null) => SendRawMessage($"PRIVMSG #{(channel != null ? channel : this.channelName)} : {message}");

        public void SendRawMessage(string rawMessage)
        {
            lock (rawOutputMsgList)
            {
                rawOutputMsgList.Add(rawMessage);
            }
        }

        #endregion

        ~TwitchIRCIO() => DisConnect();
    }
}