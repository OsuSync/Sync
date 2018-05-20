using Sync.Command;
using Sync.Plugins.BuildInPlugin.Commands;
using Sync.Tools;
using System;

namespace Sync.Plugins.BuildInPlugin
{
    public class InternalPlugin : Plugin
    {
        private PluginConfigurationManager config;

        private CommonCommand common_command = new CommonCommand();
        private PluginCommand plugin_command = new PluginCommand();

        public InternalPlugin() : base("InternalPlugin", "OsuSync")
        {
        }

        public override void OnEnable()
        {
            config = new PluginConfigurationManager(this);

            this.EventBus.BindEvent<PluginEvents.InitCommandEvent>(p =>
            {
                Func<string, CommandDelegate, string, bool> addCmd = p.Commands.Dispatch.bind;
                addCmd("plugins", plugin_command.Plugins, "Install & Update Plugins online, type 'plugins' to get help.");
                common_command.BindCommondCommand(p.Commands.Dispatch);
            });

            Updater.update = this;
        }

        public bool CheckUpdate(string guid) => plugin_command.CheckUpdate(guid);
    }
}