using Sync.Source.BiliBili;
using Sync.Tools;
using System;
using System.Diagnostics;
using static Sync.Tools.ConsoleWriter;

namespace Sync
{
    static class Program
    {

        public static Sync syncInstance = null;

        static void Main(string[] args)
        {
            WriteWelcome();
            WriteConfig();
            Write("Config loaded.");

            if (Configuration.LiveRoomID == 0)
            {
                WriteColor("\n请配置 'config.ini' 后输入 'reload' 来完成对软件的设置.\n\n", ConsoleColor.DarkRed);
                Process.Start(ConfigurationReader.ConfigFile);
            }

            if (Configuration.Provider == Configuration.PROVIDER_BILIBILI)
            {
                syncInstance = new Sync(new BiliBili());
            }
            else
            {
                syncInstance = new Sync(new BiliBili());
            }

            while (true)
            {
                string cmd = Console.ReadLine();
                if(cmd == "exit")
                {
                    syncInstance.Disconnect();
                    Write("退出操作已完成，如果窗口还未关闭，您可以强制关闭。");
                    return;
                }
                else if(cmd.Length >5 &&  cmd.Substring(0,4) == "chat")
                {
                    Write("<控制台>" + cmd.Substring(5));
                    syncInstance.IRCSendMessage(cmd.Substring(5));
                }
                else if(cmd == "help")
                {
                    Write("");
                    WriteHelp();

                }
                else if(cmd == "reload")
                {
                    WriteConfig();
                }
                else if (cmd == "start")
                {
                    syncInstance.Connect();
                }
                else if (cmd == "stop")
                {
                    syncInstance.Disconnect();
                    Environment.Exit(0);
                }
                else if (cmd == "status")
                {
                    WriteStatus(syncInstance);
                }
                else if (cmd =="clear" || cmd == "cls")
                {
                    Clear();
                    WriteWelcome();
                }
            }
        }

    }
}
