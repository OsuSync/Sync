using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Source
{

    public sealed class SourceEvents : BaseEventDispatcher
    {
        public readonly static SourceEvents Instance = new SourceEvents();
        private SourceEvents()
        {
            EventDispatcher.Instance.RegistNewDispatcher(GetType());
        }
    }

    public struct StartSyncEvent : SourceEvent
    {
        public SyncConnector Connector { get => Program.host.SyncInstance.Connector; }
    }

    public struct StopSyncEvent : SourceEvent
    {

    }

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

    public interface IBaseDanmakuEvent : SourceEvent
    {
        string Danmuku { get; set; }
        string SenderName { get; set; }
        string SendTime { get; set; }
    }

    public interface IBaseGiftEvent : SourceEvent
    {
        string GiftName { get; set; }
        int GiftCount { get; set; }
        string SenderName { get; set; }
        string SendTime { get; set; }

    }

    public struct BaseStatusEvent : SourceEvent
    {
        public SourceStatus Status { get; private set; }

        public BaseStatusEvent(SourceStatus status)
        {
            Status = status;
        }
    }

    public struct BaseOnlineCountEvent : SourceEvent
    {
        public int Count { get; set; }
    }

    public interface SourceEvent : IBaseEvent
    {
    }
}
