using Sync.Command;
using Sync.MessageFilter;
using Sync.Plugins;
using Sync.Source;
using Sync.Tools;
using System;
using System.Diagnostics;
using static Sync.Tools.ConsoleWriter;

namespace Sync
{
    static class Program
    {
        public static SyncManager sync;
        public static CommandManager commands;
        public static PluginManager plugins;
        public static SourceManager sources;
        public static FilterManager filters;

        static void Main(string[] args)
        {
            /*  程序工作流程：
             *    1.程序枚举所有插件，保存所有IPlugin到List中
             *    2.程序整理出所有的List<ISourceBase>
             *    3.初始化Sync类，Sync类检测配置文件，用正确的类初始化SyncInstance
             *    4.程序CLI Mangager开始工作，等待用户输入
             */
            WriteWelcome();
            WriteConfig();

            plugins = new PluginManager();


            sync = new SyncManager();

            if(sync.Connector == null)
            {
                Write("");
                WriteColor("程序无法继续工作，请向上查找错误原因！", ConsoleColor.Red);
                ReadCommand();
                return;
            }

            plugins.ReadySync();

            commands = new CommandManager();
            WriteColor("载入了 " + plugins.LoadCommnads() + " 个可用命令。", ConsoleColor.Green);

            filters = new FilterManager(sync.Connector);
            WriteColor("载入了 " + plugins.LoadFilters() + " 个消息过滤器。\n", ConsoleColor.Green);

            WriteColor("同步已准备就绪！", ConsoleColor.Cyan);

            while (true)
            {
                commands.invokeCmdString(ReadCommand());
            }
        }

    }
}
