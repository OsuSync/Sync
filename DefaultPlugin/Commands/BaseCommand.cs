using Sync.Source;
using System;
using System.Reflection;
using System.Linq;
using static DefaultPlugin.DefaultPlugin;
using static Sync.Tools.IO;
using static DefaultPlugin.Language;
using Sync.MessageFilter;
using Sync.Command;
using Sync.Tools;
using Sync.Plugins;
using System.Diagnostics;
using System.Globalization;
using DefaultPlugin.Sources.BiliBili;

namespace DefaultPlugin.Commands
{
    class BaseCommand
    {
        public BaseCommand(CommandManager manager)
        {
            manager.Dispatch.bind("exit", exit, LANG_COMMANDS_EXIT);
            manager.Dispatch.bind("stop", stop, LANG_COMMANDS_STOP);
            manager.Dispatch.bind("start", start, LANG_COMMANDS_START);
            manager.Dispatch.bind("restart", restart, LANG_COMMANDS_RESTART);

            manager.Dispatch.bind("clear", clear, LANG_COMMANDS_CLEAR);

            manager.Dispatch.bind("status", status, LANG_COMMANDS_STATUS);
            manager.Dispatch.bind("help", help, LANG_COMMANDS_HELP);
            manager.Dispatch.bind("listlang", languages, LANG_COMMANDS_LISTLANG);

            manager.Dispatch.bind("lang", language, LANG_COMMANDS_LANG);
            manager.Dispatch.bind("msgmgr", msgmgr, LANG_COMMANDS_MSGMGR);
            manager.Dispatch.bind("sources", listsource, LANG_COMMANDS_SOURCES);
            manager.Dispatch.bind("filters", filters, LANG_COMMANDS_FILTERS);

            manager.Dispatch.bind("sourcemsg", sourcemsg, LANG_COMMANDS_SOURCEMSG);
            manager.Dispatch.bind("clientmsg", clientmsg, LANG_COMMANDS_CLIENTMSG);

            manager.Dispatch.bind("setbili", setBilibili, LANG_COMMANDS_BILIBILI);
            manager.Dispatch.bind("sourcelogin", sourcelogin, LANG_COMMANDS_FILTERS);

        }

        private bool sourcelogin(Arguments arg)
        {
            if(MainSource.Sendable)
            {
                if (arg.Count == 0) MainSource.SendableSource.Login("", "");
                else if (arg.Count == 1) MainSource.SendableSource.Login(arg[0], "");
                else if (arg.Count == 2) MainSource.SendableSource.Login(arg[0], arg[1]);
            }

            return true;
        }

        private bool setBilibili(Arguments arg)
        {
            if (arg.Count == 0) return false;
            BiliBili.RoomID = arg[0];
            return true;
        }

        private bool restart(Arguments arg)
        {
            Process.Start(Assembly.GetEntryAssembly().Location);
            Environment.Exit(0);
            return true;
        }

        private bool filters(Arguments arg)
        {
            foreach (var item in MainFilters.GetFiltersEnum())
            {
                CurrentIO.WriteColor("", ConsoleColor.Gray, false);
                CurrentIO.WriteColor(LANG_COMMANDS_FILTERS_ITEM, ConsoleColor.Cyan, false, false);
                CurrentIO.WriteColor(item.Key.Name.PadRight(22), ConsoleColor.White, false, false);
                CurrentIO.WriteColor(LANG_COMMANDS_FILTERS_OBJ, ConsoleColor.DarkCyan, false, false);
                CurrentIO.WriteColor(item.Value.GetType().Name, ConsoleColor.White, true, false);
            }
            return true;
        }

        private bool target(Arguments arg)
        {
            if (arg.Count == 0)
            {
                CurrentIO.WriteColor(string.Format(LANG_COMMANDS_TARGET_CURRENT, BiliBili.RoomID), ConsoleColor.Green);
            }
            else
            {
                BiliBili.RoomID = arg[0];
                CurrentIO.WriteColor(string.Format(LANG_COMMANDS_TARGET_SET, BiliBili.RoomID), ConsoleColor.Green);
            }

            return true;
        }

        public bool listsource(Arguments arg)
        {
            foreach(SourceBase src in MainSources.SourceList)
            {
                CurrentIO.WriteColor("", ConsoleColor.Gray, false);
                CurrentIO.WriteColor(LANG_COMMANDS_SOURCES_NAME, ConsoleColor.Cyan, false, false);
                CurrentIO.WriteColor(src.Name.PadRight(18), ConsoleColor.White, false, false);
                CurrentIO.WriteColor(LANG_COMMANDS_SOURCES_AUTHOR, ConsoleColor.DarkCyan, false, false);
                CurrentIO.WriteColor(src.Author, ConsoleColor.White, true, false);
            }
            return true;
        }

        public bool exit(Arguments arg)
        {
            MainClient?.Client?.StopWork();
            MainSource?.Source?.Disconnect();
            CurrentIO.Write(LANG_COMMANDS_EXIT_DONE);
            Environment.Exit(0);
            return true;
        }

        public bool clientmsg(Arguments arg)
        {
            if (arg.Count == 0 || (MainSendable != null && MainSendable.SendStatus == false))
            {
                CurrentIO.Write(LANG_COMMANDS_CHAT_IRC_NOTCONNECT);
                return true;
            }
            
            MainMessager.RaiseMessage<ISourceClient>(new IRCMessage("Console", string.Join(" ", arg)));
            return true;
            
        }

        public bool chatuser(Arguments arg)
        {
            if (arg.Count < 1 || (MainSendable != null && MainSendable.SendStatus == false))
            {
                CurrentIO.Write(LANG_COMMANDS_CHAT_IRC_NOTCONNECT);
            }
            string message = "";
            for (int i = 1; i < arg.Count; i++)
                message += arg[i] + " ";
            MainMessager.RaiseMessage<ISourceClient>(new IRCMessage(arg[0].Trim(), message));
            return true;

        }

        public bool sourcemsg(Arguments arg)
        {
            if (MainSource.Sendable)
            {
                if (MainSource.Sendable)
                {
                    MainSource.SendableSource.Send(new IRCMessage(string.Empty, string.Join("", arg)));
                    return true;
                }
                else
                {
                    CurrentIO.Write(LANG_COMMANDS_DANMAKU_REQUIRE_LOGIN);
                }
            }
            else
            {
                CurrentIO.Write(LANG_COMMANDS_DANMAKU_NOT_SUPPORT);
            }
            return true;
        }

        public bool help(Arguments arg)
        {
            CurrentIO.WriteHelp();
            return true;
        }

        public bool start(Arguments arg)
        {
            if (MainSource.Source.Status == SourceStatus.CONNECTED_WORKING)
            {
                CurrentIO.Write(LANG_COMMANDS_START_ALREADY_RUN);
                return true;
            }
            MainClient?.Client?.StartWork();
            MainSource?.Source?.Connect();
            return true;
        }

        public bool stop(Arguments arg)
        {
            MainClient?.Client?.StopWork();
            MainSource?.Source?.Disconnect();
            Environment.Exit(0);
            return true;
        }

        public bool status(Arguments arg)
        {
            CurrentIO.WriteStatus();
            return true;
        }

        public bool clear(Arguments arg)
        {
            CurrentIO.Clear();
            CurrentIO.WriteWelcome();
            return true;
        }

        public bool msgmgr(Arguments arg)
        {
            int value = 0;

            if (arg.Count == 0)
                CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
            else
            {
                switch (arg[0].Trim())
                {
                    case "--help":
                        CurrentIO.WriteColor(LANG_COMMANDS_MSGMGR_HELP, ConsoleColor.Yellow);
                        break;
                    case "--status":
                        CurrentIO.WriteColor(String.Format(LANG_COMMANDS_MSGMGR_STATUS, MessageManager.IsLimit ? LANG_COMMANDS_MSGMGR_LIMIT : LANG_COMMANDS_MSGMGR_FREE, MessageManager.CurrentQueueCount, MessageManager.LimitLevel, MessageManager.RecoverTime, MessageManager.Option.ToString()), ConsoleColor.Yellow);
                        break;
                    case "--limit":
                        if (arg.Count == 2 && Int32.TryParse(arg[1].Trim(), out value))
                        {
                            MessageManager.LimitLevel = value;
                            CurrentIO.WriteColor(string.Format(LANG_COMMANDS_MSGMGR_LIMIT_SPEED_SET, MessageManager.LimitLevel), ConsoleColor.Yellow);
                        }
                        else
                            CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
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
                            CurrentIO.WriteColor(string.Format(LANG_COMMANDS_MSGMGR_LIMIT_STYPE_SET, MessageManager.Option.ToString()), ConsoleColor.Yellow);
                        }
                        else
                            CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
                        break;
                }    
            }
            return true;
        }

        public bool languages(Arguments arg)
        {
            if(arg.Count > 0 && arg[0] == "--all")
            {
                foreach (var item in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                {
                    CurrentIO.WriteColor(string.Format("CultureName: {0:S}\t{1:S}", item.Name, item.NativeName), ConsoleColor.Yellow);
                }
            }
            else
            {
                foreach(var item in System.IO.Directory.EnumerateDirectories(I18n.Instance.LangFolder))
                {
                    string name = item.Substring(item.LastIndexOf('\\') + 1);
                    CurrentIO.WriteColor(string.Format("CultureName: {0:S}\t{1:S}", name, CultureInfo.GetCultureInfo(name).NativeName), ConsoleColor.Yellow);
                }
            }
            return true;
        }

        public bool language(Arguments arg)
        {
            
            if(arg.Count == 0)
            {
                CultureInfo info = CultureInfo.GetCultureInfo(I18n.Instance.CurrentLanguage);
                CurrentIO.WriteColor(string.Format("Current culture: {0:S}\t{1:S}", info.Name, info.NativeName), ConsoleColor.Yellow);
                return true;
            }
            else if(arg.Count == 1)
            {
                try
                {
                    CultureInfo info = CultureInfo.GetCultureInfo(arg[0]);
                    Configuration.Language = arg[0];
                    CurrentIO.WriteColor(string.Format("Success switch to {1:S}({0:S})", arg[0], info.NativeName), ConsoleColor.Green);
                    return true;
                }
                catch(CultureNotFoundException)
                {
                    CurrentIO.WriteColor("Culture not found! Please specify other culture name.", ConsoleColor.Red);
                    return false;
                }
            }
            return false;
        }
    }
}
