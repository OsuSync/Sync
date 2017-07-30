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
        public PPQuery() : base("PP Query", "Deliay")
        {
            base.onInitPlugin += () => IO.CurrentIO.WriteColor("PP Query Plugin By Deliay >w<", ConsoleColor.DarkCyan);
            base.onInitFilter += filters => filters.AddFilter(this);

        }

        public void onMsg(ref IMessageBase msg)
        {
            if (msg.User.RawText == Configuration.TargetIRC)
            {
                if (msg.Message.RawText.StartsWith(Sync.Client.CooCClient.CONST_ACTION_FLAG) && msg.Message.RawText.Contains("osu.ppy.sh/b/"))
                {
                    msg.Cancel = true;
                    getHoster().SyncInstance.Connector.Client.sendRawMessage("tillerino", msg.Message.RawText);
                }
            }

            if (msg.User.Result.ToLower().Equals("tillerino"))
            {
                msg.Cancel = true;
                getHoster().SyncInstance.Connector.Client.sendRawMessage(Configuration.TargetIRC, msg.Message.RawText);
            }
        }
    }
}
