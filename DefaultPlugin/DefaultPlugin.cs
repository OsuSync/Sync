using Sync.Plugins;
using Sync.Command;
using Sync;
using DefaultPlugin.Source.BiliBili;
using DefaultPlugin.Filters;
using DefaultPlugin.Commands;

namespace DefaultPlugin
{
    public class DefaultPlugin : IPlugin
    {
        public static SyncManager MainInstance = null;
        public static FilterManager MainFilters = null;
        public const string PLUGIN_NAME = "Default Plug-ins";
        public const string PLUGIN_AUTHOR = "Deliay";
        public string Author { get { return PLUGIN_NAME; } }

        public string Name { get { return PLUGIN_AUTHOR; } }

        public void onInitCommand(CommandManager manager)
        {
            new BaseCommand(manager);
        }

        public void onInitFilter(FilterManager filter)
        {
            MainFilters = filter;
            filter.addFilter(new DefaultFormat());

        }

        public void onInitPlugin()
        {
            Sync.Tools.ConsoleWriter.WriteColor("Default Plugin by Deliay", System.ConsoleColor.DarkCyan);
        }

        public void onInitSource(SourceManager manager)
        {
            manager.AddSource(new BiliBili());
        }

        public void onSyncMangerComplete(SyncManager sync)
        {
            MainInstance = sync;
        }
    }
}
