using Sync.Command;
using Sync.Plugins;
using System;

namespace Sync.Tools.Builtin
{
    public class InternalPlugin : Plugin
    {
        //private PluginConfigurationManager config;

        private CommonCommand commonCommand = new CommonCommand();

        public InternalPlugin() : base("InternalPlugin", "OsuSync")
        {

        }

        public override void OnEnable()
        {
            EventBus.BindEvent<PluginEvents.InitCommandEvent>(p =>
            {
                Func<string, CommandDelegate, string, bool> addCmd = p.Commands.Dispatch.bind;
                addCmd("plugins", PluginCommand.Instance.Plugins, "Install & Update Plugins online, type 'plugins' to get help.");
                commonCommand.BindCommondCommand(p.Commands.Dispatch);
            });
        }

        internal bool CheckUpdate(string guid) => PluginCommand.Instance.InternalUpdate(guid,true);
    }
}