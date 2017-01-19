using Sync.Source;
using System;
using System.Reflection;
using System.Linq;
using static Sync.SyncManager;
using static DefaultPlugin.DefaultPlugin;
using static Sync.Tools.ConsoleWriter;
using Sync.MessageFilter;
using Sync.Command;
using Sync.Tools;
using Sync.Plugins;

namespace DefaultPlugin.Commands
{
    class BaseCommand
    {
        public BaseCommand(CommandManager manager)
        {
            manager.Dispatch.bind("login", login, "login <user> [pass] 登录到目标弹幕网站，启动弹幕发送功能");
            manager.Dispatch.bind("exit", exit, "退出软件");
            manager.Dispatch.bind("clear", clear, "清空屏幕");
            manager.Dispatch.bind("status", status, "获得当前连接状态属性");
            manager.Dispatch.bind("stop", stop, "停止当前连接");
            manager.Dispatch.bind("start", start, "开始同步");
            manager.Dispatch.bind("help", help, "打印帮助信息");
            manager.Dispatch.bind("danmaku", danmaku, "danmaku <message> 发送弹幕测试");
            manager.Dispatch.bind("chat", chat, "chat <message> 发送IRC信息测试");
            manager.Dispatch.bind("chatuser", chatuser, "chat <userName> <message> 以某个用户名字发送IRC信息测试");
            manager.Dispatch.bind("sources", listsource, "获得当前所有弹幕源列表");
            manager.Dispatch.bind("target", target, "target <roomID> 设置目标直播地址");
            manager.Dispatch.bind("irc", setirc, "irc <ircID> 设置目标IRC(空格请替换为下划线)");
            manager.Dispatch.bind("botirc", setbotirc, "botirc <ircID> <irc_password> 设置BotIRC(空格请替换为下划线)");
            manager.Dispatch.bind("msgmgr", msgmgr, "查看或者设置消息控制器相关内容,添加--help参数获取帮助");
            manager.Dispatch.bind("filters", filters, "列表所有当前可用消息过滤器");
        }

        private bool filters(Arguments arg)
        {
            foreach (var item in MainFilters.GetFiltersEnum())
            {
                WriteColor("", ConsoleColor.Gray, false);
                WriteColor("过滤项 ", ConsoleColor.Cyan, false, false);
                WriteColor(item.Key.Name.PadRight(22), ConsoleColor.White, false, false);
                WriteColor("过滤器 ", ConsoleColor.DarkCyan, false, false);
                WriteColor(item.Value.GetType().Name, ConsoleColor.White, true, false);
            }
            return true;
        }

        private bool setbotirc(Arguments arg)
        {
            if (arg.Count == 0)
            {
                WriteColor("当前BotIRC: " + Configuration.BotIRC, ConsoleColor.Green);
            }
            else
            {
                Configuration.BotIRC = arg[0];
                Configuration.BotIRCPassword = arg[1];
                WriteColor("当前BotIRC设置为 " + Configuration.BotIRC, ConsoleColor.Green);
            }
            return true;
        }

        private bool setirc(Arguments arg)
        {
            if(arg.Count == 0)
            {
                WriteColor("当前目标IRC: " + Configuration.TargetIRC, ConsoleColor.Green);
            }
            else
            {
                Configuration.TargetIRC = arg[0];
                WriteColor("当前目标IRC设置为 " + Configuration.TargetIRC, ConsoleColor.Green);
            }

            return true;
        }

        private bool target(Arguments arg)
        {
            if(arg.Count == 0)
            {
                WriteColor("当前直播ID: " + Configuration.LiveRoomID, ConsoleColor.Green);
            }
            else
            {
                Configuration.LiveRoomID = arg[0];
                WriteColor("当前直播ID设置为 " + Configuration.LiveRoomID, ConsoleColor.Green);
            }
  
            return true;
        }

        public bool listsource(Arguments arg)
        {
            foreach(ISourceBase src in MainSources.SourceList)
            {
                WriteColor("", ConsoleColor.Gray, false);
                WriteColor("弹幕源 ", ConsoleColor.Cyan, false, false);
                WriteColor(src.getSourceName().PadRight(18), ConsoleColor.White, false, false);
                WriteColor("作者 ", ConsoleColor.DarkCyan, false, false);
                WriteColor(src.getSourceAuthor(), ConsoleColor.White, true, false);
            }
            return true;
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
            Environment.Exit(0);
            return true;
        }

        public bool chat(Arguments arg)
        {
            if (arg.Count == 0 || !MainInstance.Connector.IRCStatus)
            {
                Write("osu! irc 尚未连接，您还不能发送消息。");
            }
            MainMessager.RaiseMessage<ISourceOsu>(new IRCMessage("Console", string.Join(" ", arg)));
            return true;
            
        }

        public bool chatuser(Arguments arg)
        {
            if (arg.Count <1 || !MainInstance.Connector.IRCStatus)
            {
                Write("osu! irc 尚未连接，您还不能发送消息。");
            }
            string message = "";
            for (int i = 1; i < arg.Count; i++)
                message += arg[i] + " ";
            MainMessager.RaiseMessage<ISourceOsu>(new IRCMessage(arg[0].Trim(), message));
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

        public bool msgmgr(Arguments arg)
        {
            int value = 0;

            if (arg.Count == 0)
                WriteColor("喵喵喵?,你的参数呢", ConsoleColor.Red);
            else
            {
                switch (arg[0].Trim())
                {
                    case "--help":
                        WriteColor("\n--status :查看当前消息管理器的信息\n--limit <数值> :是设置限制发送信息的等级，越低就越容易触发管控\n--option <名称> :是设置管控的方式，其中auto是自动管控，force_all强行全都发送,force_limit是仅发送使用?send命令的消息", ConsoleColor.Yellow);
                        break;
                    case "--status":
                        WriteColor(String.Format("MessageManager mode:{4},status:{0},queueCount/limitCount/recoverTime:{1}/{2}/{3}", MessageManager.IsLimit ? "limiting" : "free", MessageManager.CurrentQueueCount, MessageManager.LimitLevel, MessageManager.RecoverTime, MessageManager.Option.ToString()), ConsoleColor.Yellow);
                        break;
                    case "--limit":
                        if (arg.Count == 2 && Int32.TryParse(arg[1].Trim(), out value))
                        {
                            MessageManager.LimitLevel = value;
                            WriteColor(string.Format("设置限制发送速度等级为{0}",MessageManager.LimitLevel), ConsoleColor.Yellow);
                        }
                        else
                            WriteColor("错误的参数或者并没有输入数值", ConsoleColor.Red);
                        break;

                    case "--option":
                        if (arg.Count == 2)
                        {
                            switch (arg[1].Trim().ToLower())
                            {
                                case "auto":
                                    MessageManager.Option = MessageManager.PeekOption.Auto;
                                    break;
                                case "force_all":
                                    MessageManager.Option = MessageManager.PeekOption.Force_All;
                                    break;
                                case "force_limit":
                                    MessageManager.Option = MessageManager.PeekOption.Only_Send_Command;
                                    break;
                            }
                            WriteColor(string.Format("设置消息管理器的管制方式为{0}",MessageManager.Option.ToString()), ConsoleColor.Yellow);
                        }
                        else
                            WriteColor("错误的参数或者并没有输入数值", ConsoleColor.Red);
                        break;
                }    
            }
            return true;
        }
    }
}
