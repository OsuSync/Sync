using Sync.IRC.MessageFilter;
using Sync.Source;
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
        public static bool loginable = false;

        static void Main(string[] args)
        {
            

            WriteWelcome();
            WriteConfig();
            Write("Config loaded.");

            if (Configuration.LiveRoomID == 0)
            {
                WriteColor("\n请配置 'config.ini' 后再开始进行同步操作。\n", ConsoleColor.DarkRed);
                Process.Start(ConfigurationIO.ConfigFile);
            }

            if (Configuration.Provider == Configuration.PROVIDER_BILIBILI)
            {
                syncInstance = new Sync(new BiliBili());
            }
            else
            {
                syncInstance = new Sync(new BiliBili());
            }

            if (syncInstance.GetSource() is ISendable)
            {
                loginable = true;
                WriteColor("提示:当前弹幕源支持游戏内发送到弹幕源的功能，请输入login [用户名] [密码] 来登录!(用户名、密码二者可选输入)\n\n", ConsoleColor.Yellow);
                if(Configuration.LoginCookie.Length > 0)
                {
                    Write("Cookie长度:" + Configuration.LoginCookie.Length);
                    WriteColor("提示：当前已有登录Cookie记录，如需覆盖，请输入login [用户名] [密码]进行覆盖！（用户名密码可选输入）", ConsoleColor.Red);
                }
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
                else if(cmd.Length > 5 &&  cmd.Substring(0,4) == "chat")
                {
                    Write("<控制台>" + cmd.Substring(5));
                    //syncInstance.IRCSendMessage(cmd.Substring(5));
                    syncInstance.GetMessageFilter().RaiseMessage(typeof(IOsu), new IRCMessage("", cmd.Substring(5)));
                }
                else if (cmd.Length > 8 && cmd.Substring(0, 7) == "danmaku")
                {

                    if (loginable)
                    {
                        ISendable sender = (ISendable)syncInstance.GetSource();
                        if (sender.LoginStauts())
                        {
                            Write("<弹幕>" + cmd.Substring(8));
                            sender.Send(cmd.Substring(8));
                        }
                        else
                        {
                            Write("你必须登录才能发送弹幕!");
                        }
                    }
                    else
                    {
                        Write("当前同步源不支持弹幕发送！");
                    }
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
                else if (cmd.Length > 4 && cmd.Substring(0, 5) == "login")
                {
                    if(loginable)
                    {
                        ISendable s = (ISendable)syncInstance.GetSource();
                        var split = cmd.Split(' ');
                        if (split.Length == 1) s.Login(null, null);
                        if (split.Length == 2) s.Login(split[1], null);
                        if (split.Length == 3) s.Login(split[1], split[2]);
                    }
                    else
                    {
                        WriteColor("\n提示：当前弹幕源不支持发送弹幕，请更换弹幕源！\n\n", ConsoleColor.DarkYellow);
                    }
                }
            }
        }

    }
}
