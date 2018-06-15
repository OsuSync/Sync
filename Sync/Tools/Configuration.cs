using Sync.Tools.Builtin;
using Sync.Tools.ConfigGUI;

namespace Sync.Tools
{
    /// <summary>
    /// Default plugin confiuration
    /// </summary>
    public class Configuration : IConfigurable
    {
        public const string DEFAULT_LANGUAGE = "LocalSettings";

        [ConfigurationHolder(NoCheck = true)]
        [ClientList]
        public ConfigurationElement Client { get; set; } = "";

        [ConfigurationHolder(NoCheck = true)]
        [SourceList]
        public ConfigurationElement Source { get; set; } = "";

        public ConfigurationElement Language { get; set; } = "zh-CN";

        [Path(IsDirectory = false)]
        public ConfigurationElement LoggerFile { get; set; } = "Log.txt";

        [Bool]
        public ConfigurationElement EnableViewersChangedNotify { get; set; } = "False";

        [Bool]
        public ConfigurationElement EnableGiftChangedNotify { get; set; } = "False";

        public void onConfigurationLoad()
        {
        }

        public void onConfigurationReload()
        {
        }

        public void onConfigurationSave()
        {
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