using Sync.Command;
using Sync.MessageFilter;
using Sync.Plugins;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static Sync.Tools.DefaultI18n;

namespace Sync.Tools.Builtin
{
    public sealed class CommonCommand
    {
        public void BindCommondCommand(CommandDispatch dispatch)
        {
            dispatch.bind("exit", exit, LANG_COMMANDS_EXIT);
            dispatch.bind("restart", restart, LANG_COMMANDS_RESTART);
            dispatch.bind("stop", stop, LANG_COMMANDS_STOP);
            dispatch.bind("start", start, LANG_COMMANDS_START);
            dispatch.bind("status", status, LANG_COMMANDS_STATUS);
            dispatch.bind("sourcemsg", sourcemsg, LANG_COMMANDS_SOURCEMSG);
            dispatch.bind("clientmsg", clientmsg, LANG_COMMANDS_CLIENTMSG);

            dispatch.bind("clientusermsg", chatuser, LANG_COMMANDS_CLIENTUSERMSG);
            dispatch.bind("disable", disable, LANG_COMMANDS_DISABLE);
            dispatch.bind("client", switchclient, LANG_COMMANDS_SWITCH_CLIENT);
            dispatch.bind("sourcelogin", sourcelogin, LANG_COMMANDS_SOURCELOGIN);

            dispatch.bind("clear", clear, LANG_COMMANDS_CLEAR);

            dispatch.bind("help", help, LANG_COMMANDS_HELP);
            dispatch.bind("listlang", languages, LANG_COMMANDS_LISTLANG);

            dispatch.bind("lang", language, LANG_COMMANDS_LANG);
            dispatch.bind("msgmgr", msgmgr, LANG_COMMANDS_MSGMGR);
            dispatch.bind("sources", listsource, LANG_COMMANDS_SOURCES);
            dispatch.bind("filters", filters, LANG_COMMANDS_FILTERS);
        }

        private bool listsource(Arguments arg)
        {
            foreach (SourceBase src in SyncHost.Instance.Sources.SourceList)
            {
                IO.CurrentIO.WriteColor("", ConsoleColor.Gray, false);
                IO.CurrentIO.WriteColor(LANG_COMMANDS_SOURCES_NAME, ConsoleColor.Cyan, false, false);
                IO.CurrentIO.WriteColor(src.Name.PadRight(18), ConsoleColor.White, false, false);
                IO.CurrentIO.WriteColor(LANG_COMMANDS_SOURCES_AUTHOR, ConsoleColor.DarkCyan, false, false);
                IO.CurrentIO.WriteColor(src.Author, ConsoleColor.White, true, false);
            }
            return true;
        }

        private bool switchclient(Arguments arg)
        {
            if (arg.Count == 0)
            {
                foreach (var item in SyncHost.Instance.Clients.Clients)
                {
                    IO.CurrentIO.WriteColor("", ConsoleColor.Gray, false);
                    IO.CurrentIO.WriteColor(LANG_COMMANDS_CLIENT_NAME, ConsoleColor.Cyan, false, false);
                    IO.CurrentIO.WriteColor(item.ClientName.PadRight(18), ConsoleColor.White, false, false);
                    IO.CurrentIO.WriteColor(LANG_COMMANDS_CLIENT_AUTHOR, ConsoleColor.DarkCyan, false, false);
                    IO.CurrentIO.WriteColor(item.Author, ConsoleColor.White, true, false);
                }

                IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_CURRENT, SyncHost.Instance.ClientWrapper.Client?.ClientName ?? "还没指定发送源"), ConsoleColor.Green);
            }
            else
            {
                if (SyncHost.Instance.Clients.Clients.FirstOrDefault(p => p.ClientName == arg[0]) == null) return false;
                DefaultConfiguration.Instance.Client = arg[0];
                SyncHost.Instance.ClientWrapper.ResetClient();
            }
            return true;
        }

        private bool disable(Arguments arg)
        {
            if (arg.Count == 0)
                IO.CurrentIO.WriteColor(LANG_NO_PLUGIN_SELECT, ConsoleColor.Red);
            else
                foreach (var item in Sync.SyncHost.Instance.EnumPluings())
                {
                    if (item.Name == arg[0])
                    {
                        item.OnDisable();
                        IO.CurrentIO.WriteColor(LANG_PLUGIN_DISABLED + arg[0], ConsoleColor.Red);
                    }
                }
            return true;
        }

        private bool sourcelogin(Arguments arg)
        {
            if (SyncHost.Instance.SourceWrapper.Sendable)
            {
                switch (arg.Count)
                {
                    case 0:
                        SyncHost.Instance.SourceWrapper.SendableSource.Login("", "");
                        break;

                    case 1:
                        SyncHost.Instance.SourceWrapper.SendableSource.Login(arg[0], "");
                        break;

                    case 2:
                        SyncHost.Instance.SourceWrapper.SendableSource.Login(arg[0], arg[1]);
                        break;

                    default:
                        break;
                }
            }
            else
                IO.CurrentIO.WriteColor(string.Format(LANG_SOURCE_NOT_SUPPORT_SEND, SyncHost.Instance.SourceWrapper.Source?.GetType().Name), ConsoleColor.Red);

            return true;
        }

        private bool clientmsg(Arguments arg)
        {
            if (arg.Count == 0 || (SyncHost.Instance.SourceWrapper.SendableSource != null && SyncHost.Instance.SourceWrapper.SendableSource.SendStatus == false))
            {
                IO.CurrentIO.Write(LANG_COMMANDS_CHAT_IRC_NOTCONNECT);
                return true;
            }

            SyncHost.Instance.Messages.RaiseMessage<ISourceClient>(new IRCMessage("Console", string.Join(" ", arg)));
            return true;
        }

        private bool chatuser(Arguments arg)
        {
            if (arg.Count < 1 || (SyncHost.Instance.SourceWrapper.SendableSource != null && SyncHost.Instance.SourceWrapper.SendableSource.SendStatus == false))
            {
                IO.CurrentIO.Write(LANG_COMMANDS_CHAT_IRC_NOTCONNECT);
            }
            string message = "";
            for (int i = 1; i < arg.Count; i++)
                message += arg[i] + " ";
            SyncHost.Instance.Messages.RaiseMessage<ISourceClient>(new IRCMessage(arg[0].Trim(), message));
            return true;
        }

        private bool sourcemsg(Arguments arg)
        {
            if (SyncHost.Instance.SourceWrapper.Sendable)
            {
                if (SyncHost.Instance.SourceWrapper.SendableSource.SendStatus)
                {
                    SyncHost.Instance.SourceWrapper.SendableSource.Send(new IRCMessage(string.Empty, string.Join("", arg)));
                    return true;
                }
                else
                {
                    IO.CurrentIO.Write(LANG_COMMANDS_DANMAKU_REQUIRE_LOGIN);
                }
            }
            else
            {
                IO.CurrentIO.Write(LANG_COMMANDS_DANMAKU_NOT_SUPPORT);
            }
            return true;
        }

        private bool start(Arguments arg)
        {
            if (SyncHost.Instance.SourceWrapper.Source == null)
            {
                IO.CurrentIO.WriteColor(LANG_COMMANDS_START_NO_SOURCE, ConsoleColor.Red);
                return true;
            }

            if (SyncHost.Instance.ClientWrapper.Client == null)
            {
                IO.CurrentIO.WriteColor(LANG_COMMANDS_START_NO_CLIENT, ConsoleColor.Red);
                return true;
            }

            if (SyncHost.Instance.SourceWrapper.Source.Status == SourceStatus.CONNECTED_WORKING)
            {
                IO.CurrentIO.WriteColor(LANG_COMMANDS_START_ALREADY_RUN, ConsoleColor.Red);
                return true;
            }

            SyncHost.Instance.ClientWrapper?.Client?.StartWork();
            SyncHost.Instance.SourceWrapper?.Source?.Connect();
            return true;
        }

        private bool stop(Arguments arg)
        {
            SyncHost.Instance.ClientWrapper.Client?.StopWork();
            SyncHost.Instance.SourceWrapper.Source?.Disconnect();

            return true;
        }

        private bool status(Arguments arg)
        {
            IO.CurrentIO.WriteStatus();
            return true;
        }

        private bool language(Arguments arg)
        {
            if (arg.Count == 0)
            {
                CultureInfo info = CultureInfo.GetCultureInfo(I18n.Instance.CurrentLanguage);
                IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_CURRENT_LANG, info.Name, info.NativeName), ConsoleColor.Yellow);
                return true;
            }
            else if (arg.Count == 1)
            {
                try
                {
                    CultureInfo info = CultureInfo.GetCultureInfo(arg[0]);
                    DefaultConfiguration.Instance.Language = arg[0];
                    IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_LANG_SWITCHED, arg[0], info.NativeName), ConsoleColor.Green);
                    return true;
                }
                catch (CultureNotFoundException)
                {
                    IO.CurrentIO.WriteColor(LANG_COMMANDS_LANG_NOT_FOUND, ConsoleColor.Red);
                    return false;
                }
            }
            return false;
        }

        private bool clear(Arguments arg)
        {
            IO.CurrentIO.Clear();
            IO.CurrentIO.WriteWelcome();
            return true;
        }

        private bool help(Arguments arg)
        {
            IO.CurrentIO.WriteHelp();
            return true;
        }

        private static Dictionary<string, Action<Arguments>> MessageManagerCommandMap = new Dictionary<string, Action<Arguments>>
        {
            {"--help",msgmgr_help},
            {"--status",msgmgr_status},
            {"--limit",msgmgr_limit},
            {"--option",msgmgr_option}
        };

        private static void msgmgr_help(Arguments arg)
        {
            IO.CurrentIO.WriteColor(LANG_COMMANDS_MSGMGR_HELP, ConsoleColor.Yellow);
        }

        private static void msgmgr_status(Arguments arg)
        {
            IO.CurrentIO.WriteColor(String.Format(LANG_COMMANDS_MSGMGR_STATUS, (string)(MessageManager.IsLimit ? LANG_COMMANDS_MSGMGR_LIMIT : LANG_COMMANDS_MSGMGR_FREE), MessageManager.CurrentQueueCount, MessageManager.LimitLevel, MessageManager.RecoverTime, MessageManager.Option.ToString()), ConsoleColor.Yellow);
        }

        private static void msgmgr_limit(Arguments arg)
        {
            if (arg.Count == 2 && Int32.TryParse(arg[1].Trim(), out int value))
            {
                MessageManager.LimitLevel = value;
                IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_MSGMGR_LIMIT_SPEED_SET, MessageManager.LimitLevel), ConsoleColor.Yellow);
            }
            else
                IO.CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
        }

        private static void msgmgr_option(Arguments arg)
        {
            if (arg.Count == 2)
            {
                MessageManager.SetOption(arg[1].Trim());

                IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_MSGMGR_LIMIT_STYPE_SET, MessageManager.Option.ToString()), ConsoleColor.Yellow);
            }
            else
                IO.CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
        }

        private bool msgmgr(Arguments arg)
        {
            if (arg.Count == 0)
                IO.CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
            else
                MessageManagerCommandMap[arg[0]](arg);

            return true;
        }

        private bool languages(Arguments arg)
        {
            if (arg.Count > 0 && arg[0] == "--all")
            {
                foreach (var item in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                {
                    IO.CurrentIO.WriteColor(string.Format("CultureName: {0:S}\t{1:S}", item.Name, item.NativeName), ConsoleColor.Yellow);
                }
            }
            else
            {
                foreach (var item in System.IO.Directory.EnumerateDirectories(I18n.Instance.LangFolder))
                {
                    string name = item.Substring(item.LastIndexOf('\\') + 1);
                    IO.CurrentIO.WriteColor(string.Format("CultureName: {0:S}\t{1:S}", name, CultureInfo.GetCultureInfo(name).NativeName), ConsoleColor.Yellow);
                }
            }
            return true;
        }

        private bool exit(Arguments arg)
        {
            stop(arg);
            SyncHost.Instance.ExitSync();
            IO.CurrentIO.Write(LANG_COMMANDS_EXIT_DONE);
            return true;
        }

        private bool restart(Arguments arg)
        {
            Process.Start(Assembly.GetEntryAssembly().Location, "-f");
            Environment.Exit(0);
            return true;
        }

        private bool filters(Arguments arg)
        {
            foreach (var item in SyncHost.Instance.Filters.GetFiltersEnum())
            {
                IO.CurrentIO.WriteColor("", ConsoleColor.Gray, false);
                IO.CurrentIO.WriteColor(LANG_COMMANDS_FILTERS_ITEM, ConsoleColor.Cyan, false, false);
                IO.CurrentIO.WriteColor(item.Key.Name.PadRight(22), ConsoleColor.White, false, false);
                IO.CurrentIO.WriteColor(LANG_COMMANDS_FILTERS_OBJ, ConsoleColor.DarkCyan, false, false);
                IO.CurrentIO.WriteColor(item.Value.GetType().Name, ConsoleColor.White, true, false);
            }
            return true;
        }
    }
}