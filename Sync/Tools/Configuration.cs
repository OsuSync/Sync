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

        public static string LoggerFile
        {
            get => ReadConfig(DefaultConfig.LoggerFile);
            set => WriteConfig(DefaultConfig.LoggerFile, value);
        }

        public static bool EnableViewersChangedNotify
        {
            get{
                var str = ReadConfig(DefaultConfig.EnableViewersChangedNotify);
                return string.IsNullOrWhiteSpace(str)?true:bool.Parse(str);
            }
            set=> WriteConfig(DefaultConfig.LoggerFile, value.ToString());
        }

        public static bool EnableGiftChangedNotify
        {
            get
            {
                var str = ReadConfig(DefaultConfig.EnableGiftChangedNotify);
                return string.IsNullOrWhiteSpace(str) ? true : bool.Parse(str);
            }
            set => WriteConfig(DefaultConfig.EnableGiftChangedNotify, value.ToString());
        }
    }
}
