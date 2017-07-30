using System;
using Sync.Source;
using Sync.Tools;
using static Sync.Tools.DefaultI18n;

namespace Sync.MessageFilter
{
    public interface IMessageBase
    {
        StringElement User { get; set; }
        StringElement Message { get; set; }
        bool Cancel { get; set; }
    }

    public class OnlineChangeMessage : IMessageBase
    {
        public StringElement User { get; set; }
        public StringElement Message { get; set; }
        public StringElement Name { get; set; }
        public uint Count { get; set; }
        public bool Cancel { get; set; }

        OnlineChangeMessage(uint count)
        {
            User = "";
            Message = string.Format(LANG_Current_Online, Count);
            this.Count = count;
        }
    }

    public class GiftMessage : IMessageBase
    {
        public StringElement User { get; set; }        public StringElement Message { get; set; }
        public StringElement Name { get; set; }
        public BaseGiftEvent Source { get; }
        public int Count { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自弹幕的消息将不会同步到IRC。
        /// </summary>
        public bool Cancel { get; set; }

        public GiftMessage(BaseGiftEvent src)
        {
            this.User = src.SenderName;
            this.Name = src.GiftName;
            this.Count = src.GiftCount;
            this.Source = src;
            this.Message = string.Format(LANG_Gift_Sent, Count, Name);
        }
    }

    public class DanmakuMessage : IMessageBase
    {
        public StringElement User { get; set; }
        public StringElement Message { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自弹幕的消息将不会同步到IRC。
        /// </summary>
        public bool Cancel { get; set; }

        public DanmakuMessage(BaseDanmakuEvent src)
        {
            this.User = src.SenderName;
            this.Message = src.Danmuku;
        }
    }

    public class IRCMessage : IMessageBase
    {
        public StringElement User { get; set; }
        public StringElement Message { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自IRC的消息，不会同步到弹幕
        /// </summary>
        public bool Cancel { get; set; }

        public IRCMessage(StringElement user, StringElement rawMessage)
        {
            this.User = user;
            this.Message = rawMessage;
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
        void onMsg(ref IMessageBase msg);

    }
}
