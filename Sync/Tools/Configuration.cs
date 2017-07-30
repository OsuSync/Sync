using static Sync.Tools.ConfigurationIO;

namespace Sync.Tools
{
    /// <summary>
    /// 该类提供直接对配置文件的读写
    /// </summary>
    public static class Configuration
    {

        /// <summary>
        /// BiliBili Live 同步源的标识常数
        /// </summary>
        public const string PROVIDER_BILIBILI = "BiliBili";
        public const string DEFAULT_LANGUAGE = "LocalSettings";
        public static string LiveRoomID
        {
            get
            {
                return ReadConfig(DefaultConfig.LiveRoomID);
            }
            set
            {
                WriteConfig(DefaultConfig.LiveRoomID, value);
            }
        }

        public static string TargetIRC
        {
            get
            {
                return ReadConfig(DefaultConfig.TargetIRC);
            }
            set
            {
                WriteConfig(DefaultConfig.TargetIRC, value);
            }
        }

        public static string CoocAccount
        {
            get
            {
                return ReadConfig(DefaultConfig.CooCID);
            }
            set
            {
                WriteConfig(DefaultConfig.CooCID, value);
            }
        }

        public static string CoocPassword
        {
            get
            {
                return ReadConfig(DefaultConfig.CooCPassword);
            }
            set
            {
                WriteConfig(DefaultConfig.CooCPassword, value);
            }
        }

        public static string Provider
        {
            get
            {
                return ReadConfig(DefaultConfig.Provider);
            }
            set
            {
                WriteConfig(DefaultConfig.Provider, value);
            }
        }
        
        public static string Language
        {

            get
            {
                return ReadConfig(DefaultConfig.Language);
            }
            set
            {
                WriteConfig(DefaultConfig.Language, value);
            }
        }
    }
}
