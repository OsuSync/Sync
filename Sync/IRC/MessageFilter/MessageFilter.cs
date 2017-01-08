using Sync.IRC.MessageFilter.Filters.Osu;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.IRC.MessageFilter
{
    class MessageFilter
    {
        List<FilterBase> filters;
        Sync parent;
        public MessageFilter(Sync p)
        {
            parent = p;

            /*
             * Default initial filters
             * */
            addFilter(new PPQuery());
        }

        public string onDanmaku(CBaseDanmuku danmaku)
        {
            MessageBase msg = new DanmakuMessage(danmaku);
            PassFilterDanmaku(ref msg);
            if (!msg.cancel) return msg.user + msg.message;
            else return null;
        }

        public void PassFilterDanmaku(ref MessageBase msg)
        {
            foreach (IDanmaku filter in filters)
            {
                filter.onMsg(ref msg);
            }
        }

        public void PassFilterOSU(ref MessageBase msg)
        {
            foreach (IOsu filter in filters)
            {
                filter.onMsg(ref msg);
            }
        }

        public string onIRC(string user, string message)
        {
            MessageBase msg = new IRCMessage(user, message);
            PassFilterOSU(ref msg);
            if (!msg.cancel) return msg.user + msg.message;
            else return null;
        }



        public void addFilter(FilterBase filter) { filters.Add(filter); }

        public void RaiseMessage(Type msgType, MessageBase msg)
        {
            MessageBase newMsg = msg;
            if (msgType == typeof(IDanmaku))
            {
                PassFilterDanmaku(ref newMsg);
                if (newMsg.cancel) return;
                else
                {
                    parent.GetIRC().sendMessage(Meebey.SmartIrc4net.SendType.Action, msg.user + msg.message);
                }
            }
            else if(msgType == typeof(IOsu))
            {
                PassFilterOSU(ref newMsg);
                if (newMsg.cancel) return;
                else
                {
                    parent.GetIRC().sendRawMessage(Configuration.TargetIRC, newMsg.user + newMsg.message);
                }
            }

        }
    }
}
