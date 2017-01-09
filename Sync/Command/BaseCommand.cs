using Sync.IRC.MessageFilter;
using Sync.Source;
using System;
using System.Reflection;
using static Sync.Program;
using static Sync.Tools.ConsoleWriter;

namespace Sync.Command
{
    class BaseCommand
    {
        public BaseCommand(CommandManager manager)
        {
            manager.Dispatch.bind("login", login);
            manager.Dispatch.bind("exit", exit);
            manager.Dispatch.bind("clear", clear);
            manager.Dispatch.bind("status", status);
            manager.Dispatch.bind("stop", stop);
            manager.Dispatch.bind("start", start);
            manager.Dispatch.bind("help", help);
            manager.Dispatch.bind("danmaku", danmaku);
            manager.Dispatch.bind("chat", chat);
        }

        public bool login(Arguments arg)
        {
            if (loginable)
            {
                ISendable s = (ISendable)syncInstance.GetSource();
                if (arg.Count == 0) s.Login(null, null);
                if (arg.Count == 1) s.Login(arg[1], null);
                if (arg.Count == 2) s.Login(arg[1], arg[2]);
            }
            else
            {
                WriteColor("提示：当前弹幕源不支持发送弹幕，请更换弹幕源！\n", ConsoleColor.DarkYellow);
            }
            return true;
        }

        public bool exit(Arguments arg)
        {
            syncInstance.Disconnect();
            Write("退出操作已完成，如果窗口还未关闭，您可以强制关闭。");
            return true;
        }

        public bool chat(Arguments arg)
        {
            if (arg.Count == 0 || !syncInstance.IRCStatus)
            {
                Write("osu! irc 尚未连接，您还不能发送消息。");
            }
            syncInstance.GetMessageFilter().RaiseMessage(typeof(IOsu), new IRCMessage("Console", string.Join("", arg)));
            return true;
            
        }

        public bool danmaku(Arguments arg)
        {
            if (loginable)
            {
                ISendable sender = syncInstance.GetSource() as ISendable;
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
            if(syncInstance.IsConnect)
            {
                Write("同步实例已经在运行，无法再次启动。");
                Write("如果您想重启实例，您必须重启软件");
                return true;
            }
            syncInstance.Connect();
            return true;
        }

        public bool stop(Arguments arg)
        {
            syncInstance.Disconnect();
            Environment.Exit(0);
            return true;
        }

        public bool status(Arguments arg)
        {
            WriteStatus(syncInstance);
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
