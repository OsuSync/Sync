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
            base.EventBus.BindEvent<PluginEvents.InitPluginEvent>((e) => Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan));
            base.EventBus.BindEvent<PluginEvents.InitFilterEvent>(manager => manager.Filters.AddFilter(this.filter));
            base.EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(host => this.filter.SetFilterManager(host.Host.Messages));
        }
    }
}
