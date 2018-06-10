using Sync.Tools.Builtin;

namespace Sync.Tools
{
    /// <summary>
    /// Default plugin confiuration
    /// </summary>
    public class Configuration : IConfigurable
    {
        public const string DEFAULT_LANGUAGE = "LocalSettings";

        public ConfigurationElement Client { get; set; } = "";

        public ConfigurationElement Source { get; set; } = "";

        public ConfigurationElement Language { get; set; } = "zh-CN";

        public ConfigurationElement LoggerFile { get; set; } = "Log.txt";

        public ConfigurationElement EnableViewersChangedNotify { get; set; } = "False";

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