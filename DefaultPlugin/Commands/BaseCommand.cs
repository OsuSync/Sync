using Sync.Source;
using System;
using System.Reflection;
using static Sync.SyncManager;
using static DefaultPlugin.DefaultPlugin;
using static Sync.Tools.ConsoleWriter;
using Sync.MessageFilter;
using Sync.Command;

namespace DefaultPlugin.Commands
{
    class BaseCommand
    {
        public BaseCommand(CommandManager manager)
        {
            manager.Dispatch.bind("login", login, "login <user> [pass] 登录到BiliBili弹幕网站，获得发送弹幕的功能");
            manager.Dispatch.bind("exit", exit, "退出软件");
            manager.Dispatch.bind("clear", clear, "清空屏幕");
            manager.Dispatch.bind("status", status, "获得当前连接状态属性");
            manager.Dispatch.bind("stop", stop, "停止当前连接");
            manager.Dispatch.bind("start", start, "开始同步");
            manager.Dispatch.bind("help", help, "打印帮助信息");
            manager.Dispatch.bind("danmaku", danmaku, "danmaku <message> 发送弹幕测试");
            manager.Dispatch.bind("chat", chat, "chat <message> 发送IRC信息测试");
        }

        public bool login(Arguments arg)
        {
            if (loginable)
            {
                ISendable s = MainInstance.Connector.GetSource() as ISendable;
                if (arg.Count == 0) s.Login(null, null);
                if (arg.Count == 1) s.Login(arg[0], null);
                if (arg.Count == 2) s.Login(arg[0], arg[1]);
            }
            else
            {
                WriteColor("提示：当前弹幕源不支持发送弹幕，请更换弹幕源！\n", ConsoleColor.DarkYellow);
            }
            return true;
        }

        public bool exit(Arguments arg)
        {
            MainInstance.Connector.Disconnect();
            Write("退出操作已完成，如果窗口还未关闭，您可以强制关闭。");
            return true;
        }

        public bool chat(Arguments arg)
        {
            if (arg.Count == 0 || !MainInstance.Connector.IRCStatus)
            {
                Write("osu! irc 尚未连接，您还不能发送消息。");
            }
            MainFilters.RaiseMessage(typeof(IOsu), new IRCMessage("Console", string.Join("", arg)));
            return true;
            
        }

        public bool danmaku(Arguments arg)
        {
            if (loginable)
            {
                ISendable sender = MainInstance.Connector.GetSource() as ISendable;
                if (sender.LoginStauts())
                {
                    sender.Send(string.Join("", arg));
                    return true;
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
            return true;
        }

        public bool help(Arguments arg)
        {
            WriteHelp();
            return true;
        }

        public bool start(Arguments arg)
        {
            if(MainInstance.Connector.IsConnect)
            {
                Write("同步实例已经在运行，无法再次启动。");
                Write("如果您想重启实例，您必须重启软件");
                return true;
            }
            MainInstance.Connector.Connect();
            return true;
        }

        public bool stop(Arguments arg)
        {
            MainInstance.Connector.Disconnect();
            Environment.Exit(0);
            return true;
        }

        public bool status(Arguments arg)
        {
            WriteStatus(MainInstance.Connector);
            return true;
        }

        public bool clear(Arguments arg)
        {
            Clear();
            WriteWelcome();
            return true;
        }
    }
}
