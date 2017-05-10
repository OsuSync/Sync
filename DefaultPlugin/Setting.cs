using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;

namespace DefaultPlugin
{
    public class TwitchSetting : IConfigurable
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

    public class BilibiliSetting : IConfigurable
    {
        public static ConfigurationElement Cookies { get; set; } = "";

        public void onConfigurationLoad()
        {

        }

        public void onConfigurationSave()
        {

        }
    }

    public class DefaultPluginConfiuration
    {
        List<PluginConfiuration<DefaultPlugin, IConfigurable>> storage;
        public DefaultPluginConfiuration(DefaultPlugin instance)
        {

        }

        public void AddConfig()
        {

        }
    }
}
