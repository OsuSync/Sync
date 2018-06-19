using Sync.MessageFilter;
using Sync.Tools.ConfigurationAttribute;

namespace Sync.Tools
{
    /// <summary>
    /// Default plugin confiuration
    /// </summary>
    public class Configuration : IConfigurable
    {
        public const string DEFAULT_LANGUAGE = "LocalSettings";

        [ClientList(NoCheck = true)]
        public ConfigurationElement Client { get; set; } = "";

        [SourceList(NoCheck = true)]
        public ConfigurationElement Source { get; set; } = "";

        public ConfigurationElement Language { get; set; } = "zh-CN";

        [Path(IsDirectory = false)]
        public ConfigurationElement LoggerFile { get; set; } = "Log.txt";

        [Bool]
        public ConfigurationElement EnableViewersChangedNotify { get; set; } = "False";

        [Bool]
        public ConfigurationElement EnableGiftChangedNotify { get; set; } = "False";

        [List(ValueList = new[] { "Auto", "ForceAll", "OnlySendCommand", "DisableAll" }, IgnoreCase = true)]
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