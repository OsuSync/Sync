using Sync.Source;
using Sync.Tools;


namespace Sync.MessageFilter
{
    public interface MessageBase
    {
        StringElement user { get; set; }
        StringElement message { get; set; }
        bool cancel { get; set; }
    }

    public class DanmakuMessage : MessageBase
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

    public class IRCMessage : MessageBase
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

    public interface IDanmaku
    {
        //just flag
    }

    public interface IOsu
    {
        //just flag
    }

    public abstract class FilterBase
    {
        public abstract void onMsg(ref MessageBase msg);

    }
}
