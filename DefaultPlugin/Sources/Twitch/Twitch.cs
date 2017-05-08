using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync.Source;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DefaultPlugin.Sources.Twitch
{
    public class Twitch:ISourceBase/*,ISendable*///未实现
    {
        public const string SOURCE_NAME = "Twitch";
        public const string SOURCE_AUTHOR = "DarkProjector";

        static Regex parseRawMessageRegex = new Regex(@":(?<UserName>.+)!.+(PRIVMSG\s*#.+:)(?<Message>.+)");

        TwitchIRCIO currentIRCIO;

        int prev_onlineCount = 0;

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
                currentIRCIO.OnNamesCountUpdate -= onUpdateChannelPeopleCount;

                currentIRCIO = null;
            }
            try
            {
                currentIRCIO = new TwitchIRCIO(roomName);
                currentIRCIO.Connect();

                currentIRCIO.OnRecieveRawMessage += onRecieveRawMessage;
                currentIRCIO.OnNamesCountUpdate += onUpdateChannelPeopleCount;

                onConnected?.Invoke();
                updateChannelWatcherCount();
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
            currentIRCIO.DisConnect();
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
        public void updateChannelWatcherCount()
        {
            currentIRCIO?.SendRawMessage(@"NAMES");
        }

        public void onUpdateChannelPeopleCount(int newPeopleCount)
        {
            if (Math.Abs(newPeopleCount - prev_onlineCount) > 6)
            {
                onOnlineChange?.Invoke(Convert.ToUInt32(newPeopleCount));
                prev_onlineCount = newPeopleCount;
            }
        }

        public override string ToString()
        {
            return SOURCE_NAME;
        }
    }
}
