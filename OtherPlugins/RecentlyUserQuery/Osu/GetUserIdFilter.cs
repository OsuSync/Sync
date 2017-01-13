using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.MessageFilter;
using Sync.Plugins;
using Sync.Source;

namespace RecentlyUserQuery.Osu
{
    class GetUserIdFilter : FilterBase, IOsu
    {
        FilterManager manager = null;

        public GetUserIdFilter(FilterManager manager)
        {
            this.manager = manager;
        }

        const string queryUserIdCommand= "?userid",queryUserNameCommand="?username";

        public override void onMsg(ref MessageBase msg)
        {
            string message = msg.message.RawText, param = string.Empty;
            CBaseDanmuku danmaku;

            if (message.StartsWith(queryUserIdCommand))
            {
                param = message.Substring(queryUserIdCommand.Length).Trim();

                danmaku = new CBaseDanmuku();
                danmaku.danmuku = String.Format("userid \"{0}\" is {1} ", param, (UserIdGenerator.GetId(param)));

                manager.RaiseMessage(typeof(IDanmaku), new DanmakuMessage(danmaku));
                msg.cancel = true;
                return;
            }

            if (message.StartsWith(queryUserNameCommand))
            {
                msg.cancel = true;
                param = message.Substring(queryUserNameCommand.Length).Trim();
                int id = 0;

                if (Int32.TryParse(param, out id))
                    return;

                danmaku = new CBaseDanmuku();
                danmaku.danmuku = String.Format("userName \"{0}\" is {1} ", UserIdGenerator.GetUserName(id), param);
                manager.RaiseMessage(typeof(IDanmaku), new DanmakuMessage(danmaku));
            }
        }
    }
}
