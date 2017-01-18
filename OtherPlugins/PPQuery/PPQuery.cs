using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync;
using Sync.Command;
using Sync.MessageFilter;
using Sync.Tools;

namespace PPQuery
{
    public class PPQuery : Plugin, IFilter, ISourceOsu
    {
        public const string Name = "PP Query";
        public const string Author = "Deliay";

        public PPQuery() : base(Name, Author)
        {
            base.onInitPlugin += () => ConsoleWriter.WriteColor("PP Query Plugin By Deliay >w<", ConsoleColor.DarkCyan);
            base.onInitFilter += filters => filters.AddFilter(this);

        }

        public void onMsg(ref MessageBase msg)
        {
            if (msg.user.RawText == Configuration.TargetIRC)
            {
                if (msg.message.RawText.StartsWith(Sync.IRC.IRCClient.CONST_ACTION_FLAG) && msg.message.RawText.Contains("osu.ppy.sh/b/"))
                {
                    msg.cancel = true;
                    getHoster().SyncInstance.Connector.GetIRC().sendRawMessage("tillerino", msg.message.RawText);
                }
            }

            if (msg.user.Result.ToLower().Equals("tillerino"))
            {
                msg.cancel = true;
                getHoster().SyncInstance.Connector.GetIRC().sendRawMessage(Configuration.TargetIRC, msg.message.RawText);
            }
        }
    }
}
