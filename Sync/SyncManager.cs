using Sync.Plugins;
using Sync.Source;
using Sync.Tools;
using System;
using System.Diagnostics;
using static Sync.Tools.ConsoleWriter;
using System.Linq;

namespace Sync
{
    /// <summary>
    /// 同步弹幕源 实例的管理类
    /// 负责从配置文件中实例化指定弹幕源
    /// 并判断其是否支持弹幕发送。
    /// </summary>
    public class SyncManager
    {
        public static bool loginable = false;
        private SyncConnector connector = null;
        public SyncConnector Connector { get { return connector; } }

        public SyncManager(SourceManager sources)
        {

            if (Configuration.LiveRoomID.Length == 0)
            {
                WriteColor("请配置 'config.ini' 后再开始进行同步操作。\n", ConsoleColor.DarkRed);
                Process.Start(ConfigurationIO.ConfigFile);
            }

            if (sources.SourceList.Count() == 0)
            {
                WriteColor("无法找到任何直播源！请安装一个直播源之后，再启动程序。", ConsoleColor.Red);
                return;
            }

            foreach (ISourceBase item in sources.SourceList)
            {
                if (item.getSourceName() == Configuration.Provider)
                {
                    connector = new SyncConnector(item);
                }
            }

            if (connector == null)
            {
                WriteColor("找不到默认匹配的直播源，直接使用第一个。", ConsoleColor.DarkRed);
                connector = new SyncConnector(sources.SourceList.First());
            }

            WriteColor("设置 " + connector.GetSource().getSourceName() + " 为直播弹幕源", ConsoleColor.Yellow);

            if (connector.GetSource() is ISendable)
            {
                loginable = true;
                WriteColor("提示:当前弹幕源支持游戏内发送到弹幕源的功能，请输入login [用户名] [密码] 来登录!(用户名、密码二者可选输入)\n", ConsoleColor.Yellow);
                if (Configuration.LoginCertification.Length > 0)
                {
                    Write("Certification长度:" + Configuration.LoginCertification.Length);
                    WriteColor("提示：当前已有登录Certification记录，如需覆盖，请输入login [用户名] [密码]进行覆盖！（用户名密码可选输入）\n", ConsoleColor.Red);
                }
            }

        }
    }
}
