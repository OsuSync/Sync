using Sync.Plugins;
using Sync.Source;
using Sync.Tools;
using System;
using System.Diagnostics;
using static Sync.Tools.IO;
using static Sync.Tools.DefaultI18n;
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
                CurrentIO.WriteColor(LANG_NotConfig, ConsoleColor.DarkRed);
            }

            if (sources.SourceList.Count() == 0)
            {
                CurrentIO.WriteColor(LANG_NoSource, ConsoleColor.Red);
                return;
            }

            foreach (SourceBase item in sources.SourceList)
            {
                if (item.Name == Configuration.Provider)
                {
                    connector = new SyncConnector(item);
                }
            }

            if (connector == null)
            {
                CurrentIO.WriteColor(LANG_MissSource, ConsoleColor.DarkRed);
                connector = new SyncConnector(sources.SourceList.First());
            }

            CurrentIO.WriteColor(String.Format(LANG_SetSource, connector.Source.Name), ConsoleColor.Yellow);

            if (connector.Source.SupportSend)
            {
                loginable = true;
                CurrentIO.WriteColor(LANG_SupportSend, ConsoleColor.Yellow);
            }

        }
    }
}
