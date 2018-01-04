using static Sync.Tools.ConfigurationIO;

namespace Sync.Tools
{
    /// <summary>
    /// Default plugin confiuration
    /// </summary>
    public static class Configuration
    {

        public const string DEFAULT_LANGUAGE = "LocalSettings";

        public static string Client
        {
            get => ReadConfig(DefaultConfig.Client);
            set => WriteConfig(DefaultConfig.Client, value);
        }
        
        public static string Source
        {
            get => ReadConfig(DefaultConfig.Source);
            set => WriteConfig(DefaultConfig.Source, value);
        }

        public static string Language
        {
            get => ReadConfig(DefaultConfig.Language);
            set => WriteConfig(DefaultConfig.Language, value);
        }
    }
}
