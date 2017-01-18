using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync;
using Sync.Command;
using BanManagerPlugin.Ban;

namespace BanManagerPlugin
{
    public class BanManagerPlugin : IPlugin
    {
        public const string PLUGIN_NAME = "Ban Manager";
        public const string PLUGIN_AUTHOR = "Dark Projector";

        public string Author
        {
            get
            {
                return PLUGIN_NAME;
            }
        }

        public string Name
        {
            get
            {
                return PLUGIN_AUTHOR;
            }
        }

        public void onInitCommand(CommandManager manager)
        {

        }

        public void onInitFilter(FilterManager filter)
        {

            BanManager b = new BanManager(filter);
            filter.addFilter(b.GetServerFliter());
            filter.addFilter(b.GetClientFliter());
        }

        public void onInitPlugin()
        {
            Sync.Tools.ConsoleWriter.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);
        }

        public void onInitSource(SourceManager manager)
        {

        }

        public void onSyncMangerComplete(SyncManager sync)
        {

        }
    }
}
