using Sync.Plugins;
using Sync;
using DefaultPlugin.Source.BiliBili;
using DefaultPlugin.Filters;
using DefaultPlugin.Commands;
using System;

namespace DefaultPlugin
{
    public class DefaultPlugin : Plugin
    {

        public static SyncManager MainInstance = null;
        public static MessageDispatcher MainMessager = null;
        public static FilterManager MainFilters = null;
        public static SourceManager MainSources = null;
        private BiliBili srcBili;
        private DefaultFormat fltFormat;
        private GiftReceivePeeker fltGift;
        private OnlineChangePeeker fltOnline;

        public DefaultPlugin() : base("Default Plug-ins", "Deliay")
        {
            base.onInitPlugin += () => Sync.Tools.IO.CurrentIO.WriteColor("Default Plugin by Deliay", System.ConsoleColor.DarkCyan);

            srcBili = new BiliBili();
            

            base.onInitCommand += manager => new BaseCommand(manager);
            base.onInitSource += manager => {
                manager.AddSource(srcBili);
            };

            fltFormat = new DefaultFormat();
            fltGift = new GiftReceivePeeker();
            fltOnline = new OnlineChangePeeker();

            base.onInitFilter += manager => manager.AddFilters(fltFormat, fltGift, fltOnline);
            base.onStartSync += connector => fltGift.StartRecycler();

            base.onLoadComplete += DefaultPlugin_onLoadComplete;

        }

        private void DefaultPlugin_onLoadComplete(SyncHost host)
        {
            MainFilters = host.Filters;
            MainSources = host.Sources;
            MainInstance = host.SyncInstance;
            MainMessager = host.Messages;
        }

        public override void Dispose()
        {
            fltFormat.Dispose();
            fltGift.Dispose();
            fltOnline.Dispose();
        }
    }
}
