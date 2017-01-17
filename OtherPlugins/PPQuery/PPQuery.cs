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
    public class PPQuery : FilterBase, IPlugin, IOsu
    {
        public const string PLUGIN_NAME = "PP Query";
        public const string PLUGIN_AUTHOR = "Deliay";
        public static SyncManager MainInstance = null;
        public string Author
        {
            get
            {
                return PLUGIN_AUTHOR;
            }
        }

        public string Name
        {
            get
            {
                return PLUGIN_NAME;
            }
        }

        public string IdentityName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public void onInitCommand(CommandManager manager)
        {
        }

        public void onInitFilter(FilterManager filter)
        {
            filter.addFilter(new PPQuery());
        }

        public void onInitPlugin()
        {
            ConsoleWriter.WriteColor("PP Query Plugin By Deliay >w<", ConsoleColor.DarkCyan);
        }

        public void onInitSource(SourceManager manager)
        {
        }

        public void onSyncMangerComplete(SyncManager sync)
        {
            MainInstance = sync;
        }

        public override void onMsg(ref MessageBase msg)
        {
            if (msg.user.RawText == Configuration.TargetIRC)
            {
                if (msg.message.RawText.StartsWith(Sync.IRC.IRCClient.STATIC_ACTION_FLAG) && msg.message.RawText.Contains("osu.ppy.sh/b/"))
                {
                    msg.cancel = true;
                    MainInstance.Connector.GetIRC().sendRawMessage("tillerino", msg.message.RawText);
                }
            }

            if (msg.user.Result.ToLower().Equals("tillerino"))
            {
                msg.cancel = true;
                MainInstance.Connector.GetIRC().sendRawMessage(Configuration.TargetIRC, msg.message.RawText);
            }
        }
    }
}
