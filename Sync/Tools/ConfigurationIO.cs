using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sync.Tools
{
    /// <summary>
    /// 该类提供直接读取配置文件的方法
    /// </summary>
    static class ConfigurationIO
    {
        /// <summary>
        /// 配置文件枚举项
        /// </summary>
        public enum DefaultConfig
        {
            LiveRoomID,
            TargetIRC,
            BotIRC,
            BotIRCPassword,
            Provider,
            Certification
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public readonly static string ConfigFile = AppDomain.CurrentDomain.BaseDirectory + "config.ini";
        /// <summary>
        /// 实现读取配置文件
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="column">索引</param>
        /// <returns>配置信息</returns>
        private static string IniReadValue(string key, string column = "config")
        {
            StringBuilder temp = new StringBuilder(1536);
            GetPrivateProfileString(column, key, "", temp, 1536, ConfigFile);
            return temp.ToString();
        }
        /// <summary>
        /// 按需读取配置文件
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="column">索引</param>
        /// <returns>配置信息</returns>
        public static string Read(string key, string column = "config")
        {
            return IniReadValue(key, column);
        }
        /// <summary>
        /// 按枚举读取配置文件
        /// </summary>
        /// <param name="key">指定配置节</param>
        /// <returns>配置信息</returns>
        public static string ReadConfig(DefaultConfig key)
        {
            return IniReadValue(Enum.GetName(typeof(DefaultConfig), key));
        }
        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="column">索引</param>
        /// <returns></returns>
        public static bool Write(string key, string value, string column = "config")
        {
            return WritePrivateProfileString(column, key, value, ConfigFile);
        }
        /// <summary>
        /// 按枚举写入配置文件
        /// </summary>
        /// <param name="key">枚举</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteConfig(DefaultConfig key, string value)
        {
            return Write(Enum.GetName(typeof(DefaultConfig), key), value);
        }
    }
}
