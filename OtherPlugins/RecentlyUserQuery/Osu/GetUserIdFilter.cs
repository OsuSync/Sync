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
    class GetUserIdFilter : IFilter, ISourceOsu
    {
        MessageDispatcher messageSender = null;

        public GetUserIdFilter(MessageDispatcher messageSender)
        {
            this.messageSender = messageSender;
        }

        const string queryUserIdCommand= "?userid",queryUserNameCommand="?username";

        public void onMsg(ref MessageBase msg)
        {
            string message = msg.message.RawText, param = string.Empty;
            CBaseDanmuku danmaku;

            if (message.StartsWith(queryUserIdCommand))
            {
                param = message.Substring(queryUserIdCommand.Length).Trim();

                danmaku = new CBaseDanmuku();
                danmaku.Danmuku = String.Format("userid \"{0}\" is {1} ", param, (UserIdGenerator.GetId(param)));

                messageSender.RaiseMessage<ISourceDanmaku>( new DanmakuMessage(danmaku));
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
                danmaku.Danmuku = String.Format("userName \"{0}\" is {1} ", UserIdGenerator.GetUserName(id), param);
                messageSender.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
            }
        }

        public void Dispose()
        {
            //nothing to do
        }
    }
}
