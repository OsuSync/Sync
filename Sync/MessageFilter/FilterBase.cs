using System;
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

    public class OnlineChangeMessage : MessageBase
    {
        public StringElement user { get; set; }
        public StringElement message { get; set; }
        public StringElement name { get; set; }
        public uint count { get; set; }
        public bool cancel { get; set; }

        OnlineChangeMessage(uint count)
        {
            user = "";
            message = "当前在线人数:" + count;
            this.count = count;
        }
    }

    public class GiftMessage : MessageBase
    {
        public StringElement user { get; set; }
        public StringElement message { get; set; }
        public StringElement name { get; set; }
        public CBaseGift source { get; }
        public uint count { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自弹幕的消息将不会同步到IRC。
        /// </summary>
        public bool cancel { get; set; }

        public GiftMessage(CBaseGift src)
        {
            this.user = src.senderName;
            this.name = src.giftName;
            this.count = src.giftCount;
            this.source = src;
            this.message = "我送给你" + count + "份" + name;
        }
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

    public interface ISourceOnlineChange
    {
        //just flag
    }

    public interface ISourceGift
    {
        //just flag
    }

    public interface ISourceDanmaku
    {
        //just flag
    }

    public interface ISourceOsu
    {
        //just flag
    }

    public interface IFilter
    {
        void onMsg(ref MessageBase msg);

    }
}
