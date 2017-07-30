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
        private uint usercount = 0;

        public void onMsg(ref MessageBase msg)
        {
            OnlineChangeMessage castMsg = msg as OnlineChangeMessage;

            IO.CurrentIO.Write("用户变更:" + castMsg.count);
            if (Math.Abs(usercount - castMsg.count) > 4)
            {
                CBaseDanmuku d = new CBaseDanmuku();
                d.Danmuku = "直播间围观人数" + (usercount > castMsg.count ? "减少" : "增加") + "到" + castMsg.count + "人";
                DefaultPlugin.MainMessager.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(d));
                usercount = castMsg.count;
            }

            msg.cancel = true;
        }
    }
}
