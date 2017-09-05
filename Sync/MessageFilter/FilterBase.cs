using System;
using Sync.Source;
using Sync.Tools;
using static Sync.Tools.DefaultI18n;
using Sync.Plugins;

namespace Sync.MessageFilter
{
    public interface IMessageBase : IBaseEvent
    {
        StringElement User { get; set; }
        StringElement Message { get; set; }
        bool Cancel { get; set; }
    }

    public struct OnlineChangeMessage : IMessageBase
    {
        public StringElement User { get; set; }
        public StringElement Message { get; set; }
        public StringElement Name { get; set; }
        public int Count { get; set; }
        public bool Cancel { get; set; }

        public OnlineChangeMessage(int count)
        {
            User = "";
            Message = string.Format(LANG_Current_Online, count);
            Name = "";
            Cancel = false;
            this.Count = count;
        }
    }

    public struct GiftMessage : IMessageBase
    {
        public StringElement User { get; set; }
        public StringElement Message { get; set; }
        public StringElement Name { get; set; }
        public IBaseGiftEvent Source { get; }
        public int Count { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自弹幕的消息将不会同步到IRC。
        /// </summary>
        public bool Cancel { get; set; }

        public GiftMessage(IBaseGiftEvent src)
        {
            this.User = src.SenderName;
            this.Name = src.GiftName;
            this.Count = src.GiftCount;
            this.Source = src;
            this.Message = string.Format(LANG_Gift_Sent, Count, Name);
            Cancel = false;
        }
    }

    public struct DanmakuMessage : IMessageBase
    {
        public StringElement User { get; set; }
        public StringElement Message { get; set; }
        /// <summary>
        /// cancel标志指示了这条来自弹幕的消息将不会同步到IRC。
        /// </summary>
        public bool Cancel { get; set; }

        public DanmakuMessage(IBaseDanmakuEvent src)
        {
            this.User = src.SenderName;
            this.Message = src.Danmuku;
            this.Cancel = false;
        }
    }

    public struct IRCMessage : IMessageBase
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
            this.Cancel = false;
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

    public interface ISourceClient 
    {
        //just flag
    }

    public interface IFilter
    {
        void onMsg(ref IMessageBase msg);

    }
}
