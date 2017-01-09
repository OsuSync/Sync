using Sync.IRC.MessageFilter.Filters;
using Sync.IRC.MessageFilter.Filters.Osu;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;

namespace Sync.IRC.MessageFilter
{
    class MessageFilter
    {
        Dictionary<Type, List<FilterBase>> filters;
        Sync parent;
        public MessageFilter(Sync p)
        {
            parent = p;
            filters = new Dictionary<Type, List<FilterBase>>();
            filters.Add(typeof(IOsu), new List<FilterBase>());
            filters.Add(typeof(IDanmaku), new List<FilterBase>());

            addFilter(new PPQuery());
            addFilter(new DefaultFormat());
        }

        /// <summary>
        /// 简易实现直接传递弹幕消息
        /// </summary>
        /// <param name="danmaku">弹幕</param>
        public void onDanmaku(CBaseDanmuku danmaku)
        {
            MessageBase msg = new DanmakuMessage(danmaku);
            RaiseMessage(typeof(IDanmaku), msg);
        }

        /// <summary>
        /// 简易实现的传递IRC消息
        /// </summary>
        /// <param name="user">发信人</param>
        /// <param name="message">信息</param>
        public void onIRC(StringElement user, StringElement message)
        {
            MessageBase msg = new IRCMessage(user, message);
            RaiseMessage(typeof(IOsu), msg);
        }


        public void PassFilterDanmaku(ref MessageBase msg)
        {
            PassFilter(typeof(IDanmaku), ref msg);
        }

        public void PassFilterOSU(ref MessageBase msg)
        {
            PassFilter(typeof(IOsu), ref msg);
        }

        public void PassFilter(Type identify, ref MessageBase msg)
        {
            foreach (var filter in filters[identify])
            {
                filter.onMsg(ref msg);
            }
        }

        public void addFilter(FilterBase filter)
        {
            foreach (var i in filter.GetType().GetInterfaces())
            {
                if(filters.ContainsKey(i))
                {
                    filters[i].Add(filter);
                }
            }
        }

        /// <summary>
        /// 产生一个消息  
        /// 该消息会被按顺序编译
        /// </summary>
        /// <param name="msgType">消息类型，此处传IOsu(来自IRC)和IDanmaku(来自弹幕)</param>
        /// <param name="msg">具体消息实例</param>
        public void RaiseMessage(Type msgType, MessageBase msg)
        {
            MessageBase newMsg = msg;

            //消息来自弹幕
            if (msgType == typeof(IDanmaku))
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
            else if(msgType == typeof(IOsu))
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

        }
    }
}
