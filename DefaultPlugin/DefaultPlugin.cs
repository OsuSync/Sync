using Sync.Plugins;
using Sync;
using DefaultPlugin.Source.BiliBili;
using DefaultPlugin.Filters;
using DefaultPlugin.Commands;

namespace DefaultPlugin
{
    public class DefaultPlugin : Plugin
    {

        public static SyncManager MainInstance = null;
        public static MessageDispatcher MainMessager = null;
        public static FilterManager MainFilters = null;
        public static SourceManager MainSources = null;
        private GiftReceivePeeker giftPeeker;
        public DefaultPlugin() : base("Default Plug-ins", "Deliay")
        {
            base.onInitPlugin += () => Sync.Tools.IO.CurrentIO.WriteColor("Default Plugin by Deliay", System.ConsoleColor.DarkCyan);

            giftPeeker = new GiftReceivePeeker();

            base.onInitCommand += manager => new BaseCommand(manager);
            base.onInitSource += manager => {
                manager.AddSource(new BiliBili());
            };
            
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
            MainMessager = host.Messages;
        }
    }
}
