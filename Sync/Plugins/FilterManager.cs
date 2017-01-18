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
        SyncConnector parent;
        public FilterManager(SyncConnector p)
        {
            parent = p;
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

        /// <summary>
        /// 简易实现直接传递弹幕消息
        /// </summary>
        /// <param name="danmaku">弹幕</param>
        public void onDanmaku(CBaseDanmuku danmaku)
        {
            MessageBase msg = new DanmakuMessage(danmaku);
            RaiseMessage<ISourceDanmaku>(msg);
        }

        /// <summary>
        /// 简易实现的传递IRC消息
        /// </summary>
        /// <param name="user">发信人</param>
        /// <param name="message">信息</param>
        public void onIRC(StringElement user, StringElement message)
        {
            MessageBase msg = new IRCMessage(user, message);
            RaiseMessage<ISourceOsu>(msg);
        }


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


        public void RaiseMessage<Source>(MessageBase msg)
        {
            RaiseMessage(typeof(Source), msg);
        }

        /// <summary>
        /// 产生一个消息  
        /// 该消息会被按顺序编译
        /// </summary>
        /// <param name="msgType">消息类型，此处传IOsu(来自IRC)和IDanmaku(来自弹幕)</param>
        /// <param name="msg">具体消息实例</param>
        private void RaiseMessage(Type msgType, MessageBase msg)
        {
            MessageBase newMsg = msg;

            //消息来自弹幕
            if (msgType == typeof(ISourceDanmaku))
            {
                PassFilterDanmaku(ref newMsg);
                //将消息过滤一遍插件之后，判断是否取消消息（消息是否由插件自行处理拦截）
                if (newMsg.cancel) return;
                else
                {
                    parent.GetIRC().sendRawMessage(Configuration.TargetIRC, newMsg.user + newMsg.message.RawText);
                }
                return;
            }

            //消息来自osu!IRC
            else if(msgType == typeof(ISourceOsu))
            {
                PassFilterOSU(ref newMsg);
                //同上
                if (newMsg.cancel) return;
                else
                {
                    //发信用户为设置的目标IRC
                    if(newMsg.user.RawText == Configuration.TargetIRC)
                    {
                        if(parent.GetSource() is ISendable)
                        {
                            ISendable sender = parent.GetSource() as ISendable;
                            if(sender.LoginStauts())
                            {
                                sender.Send(newMsg.message);
                            }
                        }
                    }
                    //其他用户则转发到目标IRC
                    else
                    {
                        parent.GetIRC().sendRawMessage(Configuration.TargetIRC, newMsg.user + newMsg.message.RawText);
                    }
                    
                }
                return;
            }

            //消息来自弹幕礼物
            else if(msgType == typeof(ISourceGift))
            {
                PassFilterGift(ref newMsg);
                if (newMsg.cancel) return;
                else
                {
                    parent.GetIRC().sendRawMessage(Configuration.TargetIRC, IRC.IRCClient.CONST_ACTION_FLAG + newMsg.message);
                }
            }

            //观看人数变化
            else if(msgType == typeof(ISourceOnlineChange))
            {
                PassFilterOnlineChange(ref newMsg);
                if (newMsg.cancel) return;
                else
                {
                    parent.GetIRC().sendRawMessage(Configuration.TargetIRC, IRC.IRCClient.CONST_ACTION_FLAG + newMsg.message);
                }
            }

        }
    }
}
