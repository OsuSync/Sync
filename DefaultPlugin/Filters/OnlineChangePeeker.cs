using Sync.MessageFilter;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultPlugin.Filters
{
    class OnlineChangePeeker : IFilter, ISourceOnlineChange
    {
        private int usercount = 0;

        public void onMsg(ref IMessageBase msg)
        {
            OnlineChangeMessage castMsg = (OnlineChangeMessage)msg;

            IO.CurrentIO.Write("用户变更:" + castMsg.Count);
            if (Math.Abs(usercount - castMsg.Count) > 4)
            {
                IBaseDanmakuEvent d = new BaseDanmakuEvent();
                d.Danmuku = "直播间围观人数" + (usercount > castMsg.Count ? "减少" : "增加") + "到" + castMsg.Count + "人";
                DefaultPlugin.MainMessager.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(d));
                usercount = castMsg.Count;
            }

            msg.Cancel = true;
        }
    }
}
