using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;

namespace DefaultPlugin
{
    public class Setting : IConfigurable
    {
        public ConfigurationElement HostChannelName { get; set; } = "";
        public ConfigurationElement DefaultClientID { get; set; } = "esmhw2lcvrgtqw545ourqjwlg7twee";
        public ConfigurationElement CurrentClientID { get; set; } = "";
        public ConfigurationElement IsUsingCurrentClientID { get; set; } = "1";
        public ConfigurationElement OAuth { get; set; } = "";

        public void onConfigurationLoad()
        {

        }

        public void onConfigurationSave()
        {

        }
    }

    public class DefaultSettingConfiuration : PluginConfiuration<DefaultPlugin, Setting>
    {
        public DefaultSettingConfiuration(DefaultPlugin plugin, Setting config) : base(plugin, config) { }

        ~DefaultSettingConfiuration() =>ForceSave();
    }
}
