using Sync.Command;
using Sync.MessageFilter;
using Sync.Plugins;
using Sync.Source;
using Sync.Tools;
using System;
using System.Diagnostics;
using static Sync.Tools.IO;

namespace Sync
{
    static class Program
    {

        internal static SyncHost host;

        static void Main(string[] args)
        {
            /*  程序工作流程：
             *    1.程序枚举所有插件，保存所有IPlugin到List中
             *    2.程序整理出所有的List<ISourceBase>
             *    3.初始化Sync类，Sync类检测配置文件，用正确的类初始化SyncInstance
             *    4.程序IO Manager开始工作，等待用户输入
             */

            while(true)
            {
                host = new SyncHost();
                host.Load();

                CurrentIO.WriteConfig();
                CurrentIO.WriteWelcome();

                string cmd = CurrentIO.ReadCommand();
                while (true)
                {
                    host.Commands.invokeCmdString(cmd);
                    cmd = CurrentIO.ReadCommand();
                }
            }

        }

    }
}
