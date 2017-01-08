using Sync.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.IRC.MessageFilter
{
    interface MessageBase
    {
        string user { get; set; }
        string message { get; set; }
        bool cancel { get; set; }
    }

    class DanmakuMessage : MessageBase
    {
        public string user { get; set; }
        public string message { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自弹幕的消息将不会同步到IRC。
        /// </summary>
        public bool cancel { get; set; }

        public DanmakuMessage(CBaseDanmuku src)
        {
            this.user = src.senderName;
            this.message = src.danmuku;
        }
    }

    class IRCMessage : MessageBase
    {
        public string user { get; set; }
        public string message { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自IRC的消息，不会同步到弹幕
        /// </summary>
        public bool cancel { get; set; }

        public IRCMessage(string user, string rawMessage)
        {
            this.user = user;
            this.message = message;
        }
    }

    interface IDanmaku
    {
        void onMsg(ref MessageBase msg);
    }

    interface IOsu
    {
        void onMsg(ref MessageBase msg);
    }

    public class FilterBase
    {
        public FilterBase()
        {
            // Just flag
        }
    }
}
