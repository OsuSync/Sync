using Sync.Command;
using Sync.Plugins;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Sync.Tools.Builtin
{
    [Export(typeof(Plugin))]
    public class InternalPlugin : Plugin
    {
        //private PluginConfigurationManager config;

        private CommonCommand commonCommand = new CommonCommand();
        private PluginCommand pluginCommand = new PluginCommand();

        public InternalPlugin() : base("InternalPlugin", "OsuSync")
        {
        }

        public override void OnEnable()
        {
            //config = new PluginConfigurationManager(this);

            this.EventBus.BindEvent<PluginEvents.InitCommandEvent>(p =>
            {
                Func<string, CommandDelegate, string, bool> addCmd = p.Commands.Dispatch.bind;
                addCmd("plugins", pluginCommand.Plugins, "Install & Update Plugins online, type 'plugins' to get help.");
                commonCommand.BindCommondCommand(p.Commands.Dispatch);
            });

            Updater.update = this.pluginCommand;
        }

        internal bool CheckUpdate(string guid) => pluginCommand.CheckUpdate(guid);
    }
}