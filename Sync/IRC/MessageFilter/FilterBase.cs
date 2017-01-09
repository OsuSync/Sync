using Sync.Source;
using Sync.Tools;


namespace Sync.IRC.MessageFilter
{
    interface MessageBase
    {
        StringElement user { get; set; }
        StringElement message { get; set; }
        bool cancel { get; set; }
    }

    class DanmakuMessage : MessageBase
    {
        public StringElement user { get; set; }
        public StringElement message { get; set; }
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
        public StringElement user { get; set; }
        public StringElement message { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自IRC的消息，不会同步到弹幕
        /// </summary>
        public bool cancel { get; set; }

        public IRCMessage(StringElement user, StringElement rawMessage)
        {
            this.user = user;
            this.message = rawMessage;
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
