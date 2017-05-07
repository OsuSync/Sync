using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace DefaultPlugin.Sources.Twitch
{
    /// <summary>
    /// TwitchIRC通信类，用于接收/发送Comment
    /// ChannelName指频道，类似于bilibili live的room id
    /// oauth:pjaicvg4jon0o0doiwjlo5z9s05a7l
    /// </summary>

    public class TwitchIRCIO
    {
        #region 属性字段
        TcpClient clientSocket;



        string oauth = @"oauth:pjaicvg4jon0o0doiwjlo5z9s05a7l";
        string name = @"osuSync";
        string channelName = null;

        bool isLooping = false;

        Thread outputThread/*发送线程*/, inputThread/*接收线程*/;
        List<string> rawOutputMsgList=new List<string>(), rawInputMsgList=new List<string>();

        public delegate void OnRecieveMessageFunc(string rawMessage);

        /// <summary>
        /// 触发接收消息事件
        /// </summary>
        public event OnRecieveMessageFunc OnRecieveRawMessage;
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

            rawInputMsgList.Clear();
            rawOutputMsgList.Clear();

            outputThread = new Thread(outputThreadFunc);
            inputThread = new Thread(inputThreadFunc);

            isLooping = true;

            outputThread.Start();
            inputThread.Start();
        }

        /// <summary>
        /// 负责发送消息
        /// </summary>
        private void inputThreadFunc()
        {
            while (isLooping)
            {
                
            }

            //释放资源?
        }

        /// <summary>
        /// 负责接收并处理消息
        /// </summary>
        private void outputThreadFunc()
        {
            while (isLooping)
            {

            }

            //释放资源?
        }

        #endregion

        #region 公共rbq方法

        public void Connect()
        {
            
        }

        public void DisConnect()
        {
            isLooping = false;
        }

        public void SendMessage(string message,string channel=null) => SendRawMessage($"PRIVMSG #{(channel!=null?channel:this.channelName)} : {message}");

        public void SendRawMessage(string rawMessage)
        {

        }

        #endregion

        ~TwitchIRCIO()=>DisConnect();
    }
}
