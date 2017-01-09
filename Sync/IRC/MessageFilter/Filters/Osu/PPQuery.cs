using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.IRC.MessageFilter.Filters.Osu
{
    /// <summary>
    /// PP查询，/np给BotIRC，BotIRC转发给tillerino查询结果并返回。
    /// </summary>
    class PPQuery : FilterBase, IOsu
    {
        public override void onMsg(ref MessageBase msg)
        {
            
            if (msg.user.RawText == Configuration.TargetIRC)
            {
                if (msg.message.RawText.StartsWith(IRC.IRCClient.STATIC_ACTION_FLAG) && msg.message.RawText.Contains("osu.ppy.sh/b/"))
                {
                    msg.cancel = true;
                    Program.syncInstance.GetIRC().sendRawMessage("tillerino", msg.message.RawText);
                }
            }

            if(msg.user.Result.ToLower().Equals("tillerino"))
            {
                msg.cancel = true;
                Program.syncInstance.GetIRC().sendRawMessage(Configuration.TargetIRC, msg.message.RawText);
            }
        }
    }
}
