using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync.MessageFilter;
using System.Threading.Tasks;
using Sync.Plugins;
using Sync;
using Sync.Command;

namespace BeatmapSuggest
{
    public class BeatmapSuggestPlugin : IPlugin
    {
        private Danmaku.BeatmapSuggestFilter filter = new Danmaku.BeatmapSuggestFilter();

        public const string PLUGIN_NAME = "Beatmap Suggest Command";
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
            this.filter.SetFilterManager(filter);
            filter.addFilter(this.filter);
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
