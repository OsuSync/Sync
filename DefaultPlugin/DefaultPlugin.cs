using Sync.Plugins;
using Sync;
using DefaultPlugin.Source.BiliBili;
using DefaultPlugin.Sources.Twitch;
using DefaultPlugin.Filters;
using DefaultPlugin.Commands;
using System;
using Sync.Tools;
using System.Threading.Tasks;

namespace DefaultPlugin
{
    public class MyMessageEvent : IBaseEvent
    {

    }

    public class MyEvent : BaseEventDispatcher
    {
        public readonly static MyEvent Instance = new MyEvent();
    }

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
            I18n.Instance.ApplyLanguage(new Language());
            EventBus.BindEvent<PluginEvents.InitPluginEvent>( (evt) => Task.Run(() => IO.CurrentIO.WriteColor("Default Plugin by Deliay", System.ConsoleColor.DarkCyan)));

            srcBili = new BiliBili();
            srcTwitch = new Twitch();

            base.EventBus.BindEvent<PluginEvents.InitCommandEvent>(evt => new BaseCommand(evt.Commands));
            base.EventBus.BindEvent<PluginEvents.InitSourceEvent>(evt =>{
                evt.Sources.AddSource(srcBili);
                evt.Sources.AddSource(srcTwitch);
            });

            fltFormat = new DefaultFormat();
            fltGift = new GiftReceivePeeker();
            fltOnline = new OnlineChangePeeker();

            base.EventBus.BindEvent<PluginEvents.InitFilterEvent>(evt => evt.Filters.AddFilters(fltFormat, fltGift, fltOnline));

            //TODO : 
            //base.onStartSync += connector => fltGift.StartRecycler();

            base.EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(DefaultPlugin_onLoadComplete);

            //TODO:
            //{ Config.SaveAll(); };

            EventDispatcher.Instance.RegistNewDispatcher<MyEvent>();

        }

        private void DefaultPlugin_onLoadComplete(PluginEvents.LoadCompleteEvent @event)
        {
            SyncHost host = @event.Host;
            MainFilters = host.Filters;
            MainSources = host.Sources;
            MainInstance = host.SyncInstance;
            MainMessager = host.Messages;

            //config load
            Config = new PluginConfigurationManager(this);
            Config.AddItem(srcBili);
            Config.AddItem(srcTwitch);

            MyEvent.Instance.RaiseEvent(new MyMessageEvent());
        }
    }
}
