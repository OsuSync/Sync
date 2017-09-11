using Sync.MessageFilter;
using Sync.Source;
using Sync.Tools;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
namespace Sync.Plugins
{
    /// <summary>
    /// Manager and filter message
    /// </summary>
    public class FilterManager : BaseEventDispatcher
    {
        Dictionary<Type, List<IFilter>> filters;

        internal FilterManager()
        {
            filters = new Dictionary<Type, List<IFilter>>();

            AddSource<ISourceClient>();
            AddSource<ISourceDanmaku>();
            AddSource<ISourceOnlineChange>();
            AddSource<ISourceGift>();

            EventDispatcher.Instance.RegisterNewDispatcher(GetType());


            //Bind source event for Message Dispathcer
            SourceEvents.Instance.BindEvent<BaseDanmakuEvent>(
                evt => 
                SyncHost.Instance.Messages.RaiseMessage<ISourceDanmaku>(new IRCMessage(evt.SenderName, evt.Danmuku)));
            SourceEvents.Instance.BindEvent<BaseOnlineCountEvent>(evt => SyncHost.Instance.Messages.RaiseMessage<ISourceOnlineChange>(new OnlineChangeMessage(evt.Count)));
            SourceEvents.Instance.BindEvent<IBaseGiftEvent>(evt => SyncHost.Instance.Messages.RaiseMessage<ISourceClient>(new GiftMessage(evt)));
        }

        private void AddSource<T>()
        {
            filters.Add(typeof(T), new List<IFilter>());
        }

        public IEnumerable<KeyValuePair<Type, IFilter>> GetFiltersEnum()
        {
            foreach (var list in filters)
            {
                foreach(var item in list.Value)
                {
                    yield return new KeyValuePair<Type, IFilter>(list.Key, item);
                }
            }
        }


        public int Count { get { return filters.Sum(p => p.Value.Count); } }

        internal void PassFilterDanmaku(ref IMessageBase msg)
        {
            PassFilter<ISourceDanmaku>(ref msg);
        }

        internal void PassFilterOSU(ref IMessageBase msg)
        {
            PassFilter<ISourceClient>(ref msg);
        }

        internal void PassFilterGift(ref IMessageBase msg)
        {
            PassFilter<ISourceGift>(ref msg);
        }

        internal void PassFilterOnlineChange(ref IMessageBase msg)
        {
            PassFilter<ISourceOnlineChange>(ref msg);
        }

        private void PassFilter<T>(ref IMessageBase msg)
        {
            PassFilter(typeof(T), ref msg);
        }

        private void PassFilter(Type identify, ref IMessageBase msg)
        {
            foreach (var filter in filters[identify])
            {
                filter.onMsg(ref msg);
            }

            RaiseEventAsync(msg);
        }

        public void AddFilter(IFilter filter)
        {
            foreach (var i in filter.GetType().GetInterfaces())
            {
                if(filters.ContainsKey(i))
                {
                    filters[i].Add(filter);
                }
            }
        }

        public void deleteFilter(IFilter filter)
        {
            foreach (var i in filter.GetType().GetInterfaces())
            {
                if (filters.ContainsKey(i))
                {
                    filters[i].Remove(filter);
                }
            }
        }

        public void AddFilters(params IFilter[] filters)
        {
            foreach (IFilter filter in filters)
            {
                AddFilter(filter);
            }
        }
    }

}
