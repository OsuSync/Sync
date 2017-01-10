using static Sync.Tools.ConfigurationIO;

namespace Sync.Tools
{
    /// <summary>
    /// 该类提供直接对配置文件的访问（不可写）
    /// </summary>
    public static class Configuration
    {

        /// <summary>
        /// BiliBili Live 同步源的标识常数
        /// </summary>
        public const string PROVIDER_BILIBILI = "BiliBili";

        public static string LiveRoomID
        {
            get
            {
                return ReadConfig(DefaultConfig.LiveRoomID);
            }
        }

        public static string TargetIRC
        {
            get
            {
                return ReadConfig(DefaultConfig.TargetIRC);
            }
        }

        public static string BotIRC
        {
            get
            {
                return ReadConfig(DefaultConfig.BotIRC);
            }
        }

        public static string BotIRCPassword
        {
            get
            {
                return ReadConfig(DefaultConfig.BotIRCPassword);
            }
        }

        public static string Provider
        {
            get
            {
                return ReadConfig(DefaultConfig.Provider);
            }
        }

        public static string LoginCookie
        {
            get
            {
                return ReadConfig(DefaultConfig.Cookie);
            }

            set
            {
                WriteConfig(DefaultConfig.Cookie, value);
            }
        }

    }
}
