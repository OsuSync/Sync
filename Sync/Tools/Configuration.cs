using Sync.MessageFilter;
using Sync.Tools.Builtin;
using Sync.Tools.ConfigGUI;
using System;

namespace Sync.Tools
{
    /// <summary>
    /// Default plugin confiuration
    /// </summary>
    public class Configuration : IConfigurable
    {
        public const string DEFAULT_LANGUAGE = "LocalSettings";

        [ClientList]
        public ConfigurationElement Client { get; set; } = "";

        [SourceList]
        public ConfigurationElement Source { get; set; } = "";

        public ConfigurationElement Language { get; set; } = "zh-CN";

        [Path(IsDirectory = false)]
        public ConfigurationElement LoggerFile { get; set; } = "Log.txt";

        [Bool]
        public ConfigurationElement EnableViewersChangedNotify { get; set; } = "False";

        [Bool]
        public ConfigurationElement EnableGiftChangedNotify { get; set; } = "False";

        [List(ValueList =new []{"auto","force_all", "only_send_command", "disable_all"},IgnoreCase =true)]
        public ConfigurationElement MessageManagerDefaultOption { get; set; } = "Auto";
        
        public void onConfigurationLoad()
        {
            MessageManager.SetOption(MessageManagerDefaultOption);
        }

        public void onConfigurationReload()
        {
            MessageManager.SetOption(MessageManagerDefaultOption);
        }

        public void onConfigurationSave()
        {
            MessageManagerDefaultOption = MessageManager.Option.ToString();
        }

        private static Configuration instance;
        private static PluginConfigurationManager config;

        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    config = new PluginConfigurationManager("Sync");
                    instance = new Configuration();
                    config.AddItem(instance);
                }
                return instance;
            }
        }
    }
}