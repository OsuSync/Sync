using Sync.Command;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using static Sync.Tools.IO;

namespace Sync
{
    /// <summary>
    /// 程序Host类，用于管理和初始化各个模块的实例与可见性
    /// </summary>
    public class SyncHost : IDisposable
    {
        private SyncManager sync;
        private CommandManager commands;
        private PluginManager plugins;
        private SourceManager sources;
        private FilterManager filters;
        private MessageDispatcher messages;
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
            CurrentIO.Write("Loading......");

            plugins = new PluginManager();

            sources = new SourceManager();
            CurrentIO.WriteColor("读取了 " + plugins.LoadSources() + " 个直播源。", ConsoleColor.Green);

            sync = new SyncManager(sources);

            if (sync.Connector == null)
            {
                CurrentIO.Write("");
                CurrentIO.WriteColor("程序无法继续工作，请向上查找错误原因！", ConsoleColor.Red);
                CurrentIO.ReadCommand();
                throw new NullReferenceException("无法初始化Sync Manager实例!");
                
            }

            plugins.ReadySync();

            commands = new CommandManager();
            CurrentIO.WriteColor("载入了 " + plugins.LoadCommnads() + " 个可用命令。", ConsoleColor.Green);

            filters = new FilterManager();
            CurrentIO.WriteColor("载入了 " + plugins.LoadFilters() + " 个消息过滤器。\n", ConsoleColor.Green);

            messages = new MessageDispatcher(sync.Connector, filters);

            plugins.ReadyProgram();

            CurrentIO.WriteColor("同步已准备就绪！", ConsoleColor.Cyan);
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

        public void Dispose()
        {
            commands?.Dispose();
            filters?.Dispose();
            messages?.Dispose();
            sync?.Dispose();
            sources?.Dispose();
            plugins?.Dispose();
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

        public SyncManager SyncInstance
        {
            get
            {
                return sync;
            }
        }

        public MessageDispatcher Messages
        {
            get { return messages; }
        }
    }
}
