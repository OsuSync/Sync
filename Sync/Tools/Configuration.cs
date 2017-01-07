using static Sync.Tools.ConfigurationReader;

namespace Sync.Tools
{
    /// <summary>
    /// 该类提供直接对配置文件的访问（不可写）
    /// </summary>
    static class Configuration
    {

        /// <summary>
        /// BiliBili Live 同步源的标识常数
        /// </summary>
        public const string PROVIDER_BILIBILI = "BiliBili";

        public static int LiveRoomID
        {
            get
            {
                return int.Parse(ReadConfig(DefaultConfig.LiveRoomID));
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

    }
}
