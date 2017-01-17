using Sync.Command;

namespace Sync.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        string Author { get; }
        string IdentityName { get; }
        void onInitPlugin();
        void onSyncMangerComplete(SyncManager sync);
        void onInitSource(SourceManager manager);
        void onInitFilter(FilterManager filter);
        void onInitCommand(CommandManager manager);
    }
}
