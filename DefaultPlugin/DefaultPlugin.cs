using Sync.Plugins;
using Sync.Command;
using Sync;
using DefaultPlugin.Source.BiliBili;
using DefaultPlugin.Filters;
using DefaultPlugin.Commands;

namespace DefaultPlugin
{
    public class DefaultPlugin : Plugin
    {
        public static SyncManager MainInstance = null;
        public static FilterManager MainFilters = null;
        public static SourceManager MainSources = null;
        public const string Author = "Deliay";
        public const string Name = "Default Plug-ins";
        private GiftReceivePeeker giftPeeker;
        public DefaultPlugin() : base(Author, Name)
        {
            base.onInitPlugin += () => Sync.Tools.ConsoleWriter.WriteColor("Default Plugin by Deliay", System.ConsoleColor.DarkCyan);

            giftPeeker = new GiftReceivePeeker();

            base.onInitCommand += manager => new BaseCommand(manager);
            base.onInitSource += manager => manager.AddSource(new BiliBili());
            base.onInitFilter += manager => manager.AddFilters(new DefaultFormat(), 
                                                               new GiftReceivePeeker(),
                                                               new OnlineChangePeeker());
            base.onStartSync += connector => giftPeeker.StartRecycler();

            base.onLoadComplete += DefaultPlugin_onLoadComplete;
        }

        private void DefaultPlugin_onLoadComplete(SyncHost host)
        {
            MainFilters = host.Filters;
            MainSources = host.Sources;
            MainInstance = host.SyncInstance;
        }
    }
}
