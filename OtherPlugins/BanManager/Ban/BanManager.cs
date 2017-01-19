using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Plugins;

namespace BanManagerPlugin.Ban
{
    class BanManager
    {
        BanInfo info = null;
        BanClientFilter clientFliter = null;
        BanServerFilter serverFliter = null;

        FilterManager filterManager = null;
        MessageDispatcher messageSender = null;

        public BanManager(FilterManager manager,MessageDispatcher dispatcher)
        {
            info = new BanInfo();
            filterManager = manager;
            clientFliter = new BanClientFilter(this);
            serverFliter = new BanServerFilter(this);
            messageSender = dispatcher;
        }

        /// <summary>
        /// 钦定过滤控制器，里面包含着黑名单白名单相关内容
        /// </summary>
        /// <param name="control">新的控制器</param>
        public void SetFliterControl(BanInfo control)
        {
            this.info = control;
        }

        /// <summary>
        /// 钦定客户端(直播间)用的消息过滤器
        /// </summary>
        /// <param name="fliter">新的消息过滤器</param>
        public void SetClientFliter(BanClientFilter fliter)
        {
            clientFliter = fliter;
            fliter.SetBanManager(this);
        }

        /// <summary>
        /// 钦定服务器(osu!irc)用的消息过滤器
        /// </summary>
        /// <param name="fliter">新的消息过滤器</param>
        public void SetServerFliter(BanServerFilter fliter)
        {
            serverFliter = fliter;
            fliter.SetBanManager(this);
        }

        /// <summary>
        /// 获取当前过滤控制器
        /// </summary>
        /// <returns></returns>
        public BanInfo GetFliterInfo()
        {
            return info;
        }

        /// <summary>
        /// 获取当前客户端(直播间)用的消息过滤器
        /// </summary>
        /// <returns></returns>
        public BanClientFilter GetClientFliter()
        {
            return clientFliter;
        }

        /// <summary>
        /// 获取当前服务器(osu!irc)用的消息过滤器
        /// </summary>
        /// <returns></returns>
        public BanServerFilter GetServerFliter()
        {
            return serverFliter;
        }

        public FilterManager GetFilterManager()
        {
            return filterManager;
        }

        public MessageDispatcher GetMessageDispatcher()
        {
            return messageSender;
        }

        public void SetMessageDispatcher(MessageDispatcher messageDispatcher)
        {
            messageSender = messageDispatcher;
        }
    }
}
