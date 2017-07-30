using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Source
{
    public class BaseDanmakuEvent : SourceEvent
    {
        public static string EVENT_NAME = "DANMAKU";

        public BaseDanmakuEvent() : base(EVENT_NAME)
        {
        }

        public string Danmuku { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }

    }

    public class BaseGiftEvent : SourceEvent
    {
        public static string EVENT_NAME = "GIFT";
        public string GiftName { get; set; }
        public int GiftCount { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }

        public BaseGiftEvent() : base(EVENT_NAME)
        {
        }
    }

    public class BaseStatusEvent : SourceEvent
    {
        public static string EVENT_NAME = "STATUS";
        public SourceStatus Status { get; private set; }

        public BaseStatusEvent(SourceStatus status) : base(EVENT_NAME)
        {
            Status = status;
        }
    }

    public class BaseOnlineCountEvent : SourceEvent
    {
        public static string EVENT_NAME = "ONLINE";
        public int Count { get; set; }
        public BaseOnlineCountEvent() : base(EVENT_NAME)
        {
        }
    }

    public abstract class SourceEvent
    {
        public string EventName { get; private set; }
        public SourceEvent(string EventName)
        {
            this.EventName = EventName;
        }

        public T CastTo<T>() where T : SourceEvent
        {
            if (this is T)
                return (T)this;
            else
                return null;
        }
    }
}
