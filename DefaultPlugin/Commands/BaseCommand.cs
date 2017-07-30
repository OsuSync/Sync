using Sync.Source;
using System;
using System.Reflection;
using System.Linq;
using static Sync.SyncManager;
using static DefaultPlugin.DefaultPlugin;
using static Sync.Tools.IO;
using static DefaultPlugin.Language;
using Sync.MessageFilter;
using Sync.Command;
using Sync.Tools;
using Sync.Plugins;
using System.Diagnostics;
using System.Globalization;

namespace DefaultPlugin.Commands
{
    class BaseCommand
    {
        public BaseCommand(CommandManager manager)
        {
            manager.Dispatch.bind("exit", exit, LANG_COMMANDS_EXIT);
            manager.Dispatch.bind("clear", clear, LANG_COMMANDS_CLEAR);
            manager.Dispatch.bind("status", status, LANG_COMMANDS_STATUS);
            manager.Dispatch.bind("stop", stop, LANG_COMMANDS_STOP);
            manager.Dispatch.bind("start", start, LANG_COMMANDS_START);
            manager.Dispatch.bind("help", help, LANG_COMMANDS_HELP);
            manager.Dispatch.bind("danmaku", danmaku, LANG_COMMANDS_DANMAKU);
            manager.Dispatch.bind("chat", chat, LANG_COMMANDS_CHAT);
            manager.Dispatch.bind("chatuser", chatuser, LANG_COMMANDS_CHATUSER);
            manager.Dispatch.bind("sources", listsource, LANG_COMMANDS_SOURCES);
            manager.Dispatch.bind("target", target, LANG_COMMANDS_TARGET);
            manager.Dispatch.bind("irc", setirc, LANG_COMMANDS_IRC);
            manager.Dispatch.bind("botirc", setbotirc, LANG_COMMANDS_BOTIRC);
            manager.Dispatch.bind("msgmgr", msgmgr, LANG_COMMANDS_MSGMGR);
            manager.Dispatch.bind("filters", filters, LANG_COMMANDS_FILTERS);
            manager.Dispatch.bind("restart", restart, LANG_COMMANDS_RESTART);
            manager.Dispatch.bind("lang", language, LANG_COMMANDS_LANG);
            manager.Dispatch.bind("listlang", languages, LANG_COMMANDS_LISTLANG);
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

        private bool setbotirc(Arguments arg)
        {
            if (arg.Count == 0)
            {
                CurrentIO.WriteColor(string.Format(LANG_COMMANDS_BOTIRC_CURRENT, Configuration.CoocAccount), ConsoleColor.Green);
            }
            else
            {
                Configuration.CoocAccount = arg[0];
                Configuration.CoocPassword = arg[1];
                CurrentIO.WriteColor(string.Format(LANG_COMMANDS_BOTIRC_SET, Configuration.CoocAccount), ConsoleColor.Green);
            }
            return true;
        }

        private bool setirc(Arguments arg)
        {
            if(arg.Count == 0)
            {
                CurrentIO.WriteColor(string.Format(LANG_COMMANDS_IRC_CURRENT, Configuration.TargetIRC), ConsoleColor.Green);
            }
            else
            {
                Configuration.TargetIRC = arg[0];
                CurrentIO.WriteColor(string.Format(LANG_COMMANDS_IRC_SET, Configuration.TargetIRC), ConsoleColor.Green);
            }

            return true;
        }

        private bool target(Arguments arg)
        {
            if(arg.Count == 0)
            {
                CurrentIO.WriteColor(string.Format(LANG_COMMANDS_TARGET_CURRENT, Configuration.LiveRoomID), ConsoleColor.Green);
            }
            else
            {
                Configuration.LiveRoomID = arg[0];
                CurrentIO.WriteColor(string.Format(LANG_COMMANDS_TARGET_SET, Configuration.LiveRoomID), ConsoleColor.Green);
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
            MainInstance.Connector.Disconnect();
            CurrentIO.Write(LANG_COMMANDS_EXIT_DONE);
            Environment.Exit(0);
            return true;
        }

        public bool chat(Arguments arg)
        {
            if (arg.Count == 0 || !MainInstance.Connector.Client.isConnected)
            {
                CurrentIO.Write(LANG_COMMANDS_CHAT_IRC_NOTCONNECT);
            }
            MainMessager.RaiseMessage<ISourceOsu>(new IRCMessage("Console", string.Join(" ", arg)));
            return true;
            
        }

        public bool chatuser(Arguments arg)
        {
            if (arg.Count <1 || !MainInstance.Connector.Client.isConnected)
            {
                CurrentIO.Write(LANG_COMMANDS_CHAT_IRC_NOTCONNECT);
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
                if (MainInstance.Connector.Source.SupportSend)
                {
                    MainInstance.Connector.Source.Send(string.Join("", arg));
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
            if (MainInstance.Connector.Source.Status == SourceStatus.CONNECTED_WORKING)
            {
                CurrentIO.Write(LANG_COMMANDS_START_ALREADY_RUN);
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
            CurrentIO.WriteStatus(MainInstance.Connector);
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
