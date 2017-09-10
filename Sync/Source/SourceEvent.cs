using Sync.Client;
using Sync.MessageFilter;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Source
{

    /// <summary>
    /// Singleton for source events
    /// </summary>
    public sealed class SourceEvents : BaseEventDispatcher
    {
        public readonly static SourceEvents Instance = new SourceEvents();

        private SourceEvents()
        {
            EventDispatcher.Instance.RegisterNewDispatcher(GetType());
        }
    }

    /// <summary>
    /// This message will fire when source start work
    /// </summary>
    public struct StartSourceEvent : ISourceEvent
    {
        public SourceWorkWrapper Source { get => SyncHost.Instance.SourceWrapper; }
    }

    /// <summary>
    /// This message will fire when source stop work
    /// </summary>
    public struct StopSyncEvent : ISourceEvent
    {

    }

    /// <summary>
    /// The message will fire when source recive a danmaku
    /// </summary>
    public struct BaseDanmakuEvent : IBaseDanmakuEvent
    {
        public string Danmuku { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }

        public BaseDanmakuEvent(string danmaku, string sender, string time)
        {
            Danmuku = danmaku;
            SenderName = sender;
            SendTime = time;
        }
    }

    /// <summary>
    /// Base danmaku interface
    /// </summary>
    public interface IBaseDanmakuEvent : ISourceEvent
    {
        string Danmuku { get; set; }
        string SenderName { get; set; }
        string SendTime { get; set; }
    }

    /// <summary>
    /// Base gift interface
    /// </summary>
    public interface IBaseGiftEvent : ISourceEvent
    {
        string GiftName { get; set; }
        int GiftCount { get; set; }
        string SenderName { get; set; }
        string SendTime { get; set; }

    }

    /// <summary>
    /// This event will fire when source status change
    /// </summary>
    public struct BaseStatusEvent : ISourceEvent
    {
        public SourceStatus Status { get; private set; }

        public BaseStatusEvent(SourceStatus status)
        {
            Status = status;
        }
    }

    /// <summary>
    /// This event will fire when source check online change
    /// </summary>
    public struct BaseOnlineCountEvent : ISourceEvent
    {
        public int Count { get; set; }

        public BaseOnlineCountEvent(int Count)
        {
            this.Count = Count;
        }
    }

    /// <summary>
    /// Source event flag
    /// </summary>
    public interface ISourceEvent : IBaseEvent
    {
    }
}
