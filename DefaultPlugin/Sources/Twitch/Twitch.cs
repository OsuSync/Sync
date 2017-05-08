using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync.Source;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace DefaultPlugin.Sources.Twitch
{
    public class Twitch:ISourceBase,ISendable
    {
        public const string SOURCE_NAME = "Twitch";
        public const string SOURCE_AUTHOR = "DarkProjector";

        static Regex parseRawMessageRegex = new Regex(@":(?<UserName>.+)!.+(PRIVMSG\s*#.+:)(?<Message>.+)");

        TwitchIRCIO currentIRCIO;

        int prev_ViewersCount = 0;

        int onlineViewersCountInv = 6;

        public event ConnectedEvt onConnected;
        public event DisconnectedEvt onDisconnected;
        public event DanmukuEvt onDanmuku;
        public event GiftEvt onGift;
        public event CurrentOnlineEvt onOnlineChange;

        #region 接口实现

        public bool Connect(string roomName)
        {
            if (currentIRCIO != null)
            {
                currentIRCIO.DisConnect();

                currentIRCIO.OnRecieveRawMessage -= onRecieveRawMessage;

                currentIRCIO = null;
            }
            try
            {
                currentIRCIO = new TwitchIRCIO(roomName);
                currentIRCIO.Connect();

                currentIRCIO.OnRecieveRawMessage += onRecieveRawMessage;

                onConnected?.Invoke();
                UpdateChannelViewersCount();
            }
            catch (Exception e)
            {
                Sync.Tools.IO.CurrentIO.WriteColor("twitch connect error!" + e.Message, ConsoleColor.Red);
                return false;
            }
            return true;
        }

        public bool Disconnect()
        {
            currentIRCIO?.DisConnect();
            currentIRCIO = null;
            onDisconnected?.Invoke();
            return true;
        }

        public string getSourceAuthor()
        {
            return SOURCE_AUTHOR;
        }

        public string getSourceName()
        {
            return SOURCE_NAME;
        }

        public Type getSourceType()
        {
            return typeof(Twitch);
        }

        public bool Stauts()
        {
            return currentIRCIO != null && currentIRCIO.IsConnected;
        }

        public void Send(string str)
        {
            currentIRCIO?.SendMessage(str);
        }

        public void Login(string user, string password)
        {
            //只需要oauth
        }

        public bool LoginStauts()
        {
            return true;//只需要oauth
        }

        #endregion  

        public void onRecieveRawMessage(string rawMessage)
        {
            var result=parseRawMessageRegex.Match(rawMessage);

            if (!result.Success)
                return;

            string userName = result.Groups["UserName"].Value;
            string message = result.Groups["Message"].Value;

            CBaseDanmuku danmuku = new CBaseDanmuku();
            danmuku.senderName = userName;
            danmuku.danmuku = message;

            onDanmuku?.Invoke(danmuku);
        }

        /// <summary>
        /// 更新观众人数并汇报
        /// </summary>
        public async void UpdateChannelViewersCount()
        {
            //currentIRCIO?.SendRawMessage(@"NAMES");
            int nowViewersCount = await new Task<int>(() =>
            {
                string uri = $"https://api.twitch.tv/kraken/streams/{currentIRCIO.ChannelName}&client_id={currentIRCIO.ClientID}";

                HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
                request.Method = "GET";

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    StreamReader stream;
                    using (stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        string data = stream.ReadToEnd();
                        string viewers = GetJSONValue(ref data, "viewers");
                        return int.Parse(viewers);
                    }
                }
                catch (Exception e)
                {
                    return prev_ViewersCount;//就当做啥事都没发生(
                }
            });

            if (Math.Abs(nowViewersCount - prev_ViewersCount) > onlineViewersCountInv)
            {
                onOnlineChange?.Invoke(Convert.ToUInt32(nowViewersCount));
                prev_ViewersCount = nowViewersCount;
            }
        }

        private string GetJSONValue(ref string text, string key)
        {
            var result = Regex.Match(text, $"{key}\":\"(.+?)\"");

            if (!result.Success)
                return null;

            return result.Groups[1].Value;
        }

        public override string ToString()
        {
            return SOURCE_NAME;
        }
    }
}
