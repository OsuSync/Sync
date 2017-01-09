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
                if (msg.message.RawText.Substring(0, 8) == "ACTION " && msg.message.RawText.IndexOf("osu.ppy.sh/b/") > 0)
                {
                    msg.cancel = true;
                    Program.syncInstance.GetIRC().sendRawMessage("tillerino", msg.message.RawText);
                }
            }

            if(msg.user.Result.ToLower() == "tillerino")
            {
                msg.cancel = true;
                Program.syncInstance.GetIRC().sendRawMessage(Configuration.TargetIRC, msg.message.RawText);
            }
        }
    }
}
