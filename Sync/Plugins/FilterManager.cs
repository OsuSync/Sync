using Sync.MessageFilter;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Sync.Plugins
{
    /// <summary>
    /// 提供对消息的发送、管理、过滤功能
    /// </summary>
    public class FilterManager
    {
        Dictionary<Type, List<IFilter>> filters;
        public FilterManager()
        {
            filters = new Dictionary<Type, List<IFilter>>();
            AddSource<ISourceOsu>();
            AddSource<ISourceDanmaku>();
            AddSource<ISourceOnlineChange>();
            AddSource<ISourceGift>();
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

        internal void PassFilterDanmaku(ref MessageBase msg)
        {
            PassFilter<ISourceDanmaku>(ref msg);
        }

        internal void PassFilterOSU(ref MessageBase msg)
        {
            PassFilter<ISourceOsu>(ref msg);
        }

        internal void PassFilterGift(ref MessageBase msg)
        {
            PassFilter<ISourceGift>(ref msg);
        }

        internal void PassFilterOnlineChange(ref MessageBase msg)
        {
            PassFilter<ISourceGift>(ref msg);
        }

        private void PassFilter<T>(ref MessageBase msg)
        {
            PassFilter(typeof(T), ref msg);
        }

        private void PassFilter(Type identify, ref MessageBase msg)
        {
            foreach (var filter in filters[identify])
            {
                filter.onMsg(ref msg);
            }
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

        public void AddFilters(params IFilter[] filters)
        {
            foreach (IFilter filter in filters)
            {
                AddFilter(filter);
            }
        }

    }
}
