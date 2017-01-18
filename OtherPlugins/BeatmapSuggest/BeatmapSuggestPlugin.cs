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
    public class BeatmapSuggestPlugin : Plugin
    {
        private Danmaku.BeatmapSuggestFilter filter = new Danmaku.BeatmapSuggestFilter();
        public BeatmapSuggestPlugin() : base("Beatmap Suggest Command", "Dark Projector")
        {
            base.onInitPlugin += () => Sync.Tools.ConsoleWriter.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);
            base.onInitFilter += manager => manager.AddFilter(this.filter);
            base.onLoadComplete += host => this.filter.SetFilterManager(host.Messages);
        }
    }
}
