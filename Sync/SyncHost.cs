using Sync.Command;
using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using static Sync.Tools.IO;
using static Sync.Tools.DefaultI18n;
using Sync.Client;
using Sync.Source;

namespace Sync
{
    /// <summary>
    /// 程序Host类，用于管理和初始化各个模块的实例与可见性
    /// </summary>
    public class SyncHost
    {
        public static SyncHost Instance { get; internal set; }

        private SourceWorkWrapper sourceWrapper;
        private ClientWorkWrapper clientWrapper;
        private CommandManager commands;
        private PluginManager plugins;
        private SourceManager sources;
        private FilterManager filters;
        private MessageDispatcher messages;
        private ClientManager clients;
        /// <summary>
        /// 对程序集外不可见，不允许主程序外的程序初始化Host的实例
        /// 
        /// 因为在调用时会完成事件的通知，如果在构造函数中初始化程序，此时本类并未实例化完成。
        /// 故分开实现。
        /// </summary>
        internal SyncHost()
        {

        }

        /// <summary>
        /// 调用即开始读取各种插件，只允许主程序的管理模块调用
        /// </summary>
        internal void Load()
        {
            CurrentIO.Write(LANG_Loading);

            //初始化插件管理器，并初始化插件，让插件做准备工作
            plugins = new PluginManager();
            CurrentIO.WriteColor(String.Format(LANG_Plugins, plugins.LoadPlugins()), ConsoleColor.Green);

            //初始化弹幕源
            sources = new SourceManager();
            CurrentIO.WriteColor(String.Format(LANG_Sources, plugins.LoadSources()), ConsoleColor.Green);

            //挑选需要作为工作弹幕源的弹幕源
            sourceWrapper = new SourceWorkWrapper(sources);
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitSourceWarpperEvent(sourceWrapper));

            if (sourceWrapper.Source == null)
            {
                CurrentIO.Write("");
                CurrentIO.WriteColor(LANG_Error, ConsoleColor.Red);
                CurrentIO.ReadCommand();
                throw new NullReferenceException(LANG_Error);
            }

            //获得单例
            clients = ClientManager.Instance;
            CurrentIO.WriteColor(String.Format(LANG_Client, plugins.LoadClients()), ConsoleColor.Green);

            clientWrapper = new ClientWorkWrapper(clients);
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitClientWarpperEvent(clientWrapper));

            //if (Configuration.CoocAccount.Length == 0)
            //{
            //    CurrentIO.WriteColor(LANG_RqueireLogin, ConsoleColor.Red);
            //    CurrentIO.WriteColor(LANG_AccountName, ConsoleColor.Green);
            //    Configuration.CoocAccount = CurrentIO.ReadCommand();
            //    CurrentIO.WriteColor(LANG_AccountPw, ConsoleColor.Green);
            //    Configuration.CoocPassword = CurrentIO.ReadCommand();

            //}

            plugins.ReadySync();

            commands = new CommandManager();
            CurrentIO.WriteColor(String.Format(LANG_Commands, plugins.LoadCommnads()), ConsoleColor.Green);

            filters = new FilterManager();
            CurrentIO.WriteColor(String.Format(LANG_Filters, plugins.LoadFilters()), ConsoleColor.Green);

            messages = new MessageDispatcher(filters);

            plugins.ReadyProgram();

            CurrentIO.WriteColor(LANG_Ready, ConsoleColor.Cyan);
        }

        /// <summary>
        /// 程序集内可见的完全PluginManager类操作器
        /// </summary>
        internal PluginManager Plugins { get { return plugins; } }

        /// <summary>
        /// 只读的Plugins列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Plugin> EnumPluings()
        {
            return plugins.GetPlugins();
        }

        public CommandManager Commands
        {
            get
            {
                return commands;
            }
        }

        public SourceManager Sources
        {
            get
            {
                return sources;
            }
        }

        public FilterManager Filters
        {
            get
            {
                return filters;
            }
        }

        public MessageDispatcher Messages
        {
            get { return messages; }
        }

        public ClientManager Clients { get => clients; }

        public SourceWorkWrapper SourceWrapper { get => sourceWrapper; }

        public ClientWorkWrapper ClientWrapper { get => clientWrapper; }
    }
}
