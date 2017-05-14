using Sync.Plugins;
using Sync;
using DefaultPlugin.Source.BiliBili;
using DefaultPlugin.Sources.Twitch;
using DefaultPlugin.Filters;
using DefaultPlugin.Commands;
using System;
using Sync.Tools;

namespace DefaultPlugin
{
    public class DefaultPlugin : Plugin
    {

        public static SyncManager MainInstance = null;
        public static MessageDispatcher MainMessager = null;
        public static FilterManager MainFilters = null;
        public static SourceManager MainSources = null;
        private BiliBili srcBili;
        private Twitch srcTwitch;
        private DefaultFormat fltFormat;
        private GiftReceivePeeker fltGift;
        private OnlineChangePeeker fltOnline;

        public static PluginConfigurationManager Config { get; set; }

        public DefaultPlugin() : base("Default Plug-ins", "Deliay")
        {
            Sync.Tools.I18n.Instance.ApplyLanguage(new Language());
            base.onInitPlugin += () => Sync.Tools.IO.CurrentIO.WriteColor("Default Plugin by Deliay", System.ConsoleColor.DarkCyan);

            srcBili = new BiliBili();
            srcTwitch = new Twitch();

            base.onInitCommand += manager => new BaseCommand(manager);
            base.onInitSource += manager => {
                manager.AddSource(srcBili);
                manager.AddSource(srcTwitch);
            };

            fltFormat = new DefaultFormat();
            fltGift = new GiftReceivePeeker();
            fltOnline = new OnlineChangePeeker();

            base.onInitFilter += manager => manager.AddFilters(fltFormat, fltGift, fltOnline);
            base.onStartSync += connector => fltGift.StartRecycler();

            base.onLoadComplete += DefaultPlugin_onLoadComplete;

            base.onStopSync += () => { Config.SaveAll(); };

        }

        private void DefaultPlugin_onLoadComplete(SyncHost host)
        {
            MainFilters = host.Filters;
            MainSources = host.Sources;
            MainInstance = host.SyncInstance;
            MainMessager = host.Messages;

            //config load
            Config = new PluginConfigurationManager(this);
            Config.AddItem(srcBili);
            Config.AddItem(srcTwitch);

            srcTwitch.LoadConfig();
        }
    }
}
