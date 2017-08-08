using Sync.Command;
using System;
using System.Reflection;

namespace Sync.Plugins
{
    [Obsolete("Instead with EventBus(PluginEvents class)", true)]
    public delegate void SyncManagerCompleteEvt(SyncManager sync);
    [Obsolete("Instead with EventBus(PluginEvents class)", true)]
    public delegate void InitPluginEvt();
    [Obsolete("Instead with EventBus(PluginEvents class)", true)]
    public delegate void InitSourceEvt(SourceManager sources);
    [Obsolete("Instead with EventBus(PluginEvents class)", true)]
    public delegate void InitFilterEvt(FilterManager filters);
    [Obsolete("Instead with EventBus(PluginEvents class)", true)]
    public delegate void InitCommandEvt(CommandManager commands);
    [Obsolete("Instead with EventBus(PluginEvents class)", true)]
    public delegate void LoadCompleteEvt(SyncHost host);
    [Obsolete("Instead with EventBus(PluginEvents class)", true)]
    public delegate void StartSyncEvt(SyncConnector connector);
    [Obsolete("Instead with EventBus(PluginEvents class)", true)]
    public delegate void StopSyncEvt();

    public interface IPlugin
    {
        string getName();
        string getAuthor();
    }

    public abstract class Plugin
    {
        public readonly string Name;
        public readonly string Author;
        public BaseEventDispatcher EventBus { get => PluginEvents.Instance; }

        public Plugin(string Name, string Author)
        {
            this.Name = Name;
            this.Author = Author;
        }
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        protected event InitPluginEvt onInitPlugin;
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        protected event SyncManagerCompleteEvt onSyncMangerComplete;
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        protected event InitSourceEvt onInitSource;
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        protected event InitFilterEvt onInitFilter;
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        protected event InitCommandEvt onInitCommand;
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        protected event LoadCompleteEvt onLoadComplete;
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        protected event StartSyncEvt onStartSync;
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        protected event StopSyncEvt onStopSync;
        private bool isComplete = false;

        /// <summary>
        /// 简单实现的简易事件分发器，通过类型来判别应该cast什么事件
        /// </summary>
        /// <typeparam name="T">类别</typeparam>
        /// <param name="handler">产生器</param>
        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        internal void onEvent<T>(Func<T> handler)
        {
            Type eventType = typeof(T);
            T result = handler();

            if (eventType == typeof(Plugin))
            {
                onInitPlugin?.Invoke();
            }
            else if (eventType == typeof(SyncManager))
            {
                onSyncMangerComplete?.Invoke(result as SyncManager);
            }
            else if (eventType == typeof(SourceManager))
            {
                onInitSource?.Invoke(result as SourceManager);
            }
            else if (eventType == typeof(FilterManager))
            {
                onInitFilter?.Invoke(result as FilterManager);
            }
            else if (eventType == typeof(CommandManager))
            {
                onInitCommand?.Invoke(result as CommandManager);
            }
            else if(eventType == typeof(SyncHost))
            {
                isComplete = true;
                onLoadComplete?.Invoke(result as SyncHost);
            }
            else if(eventType == typeof(SyncConnector))
            {
                if (result != null) onStartSync?.Invoke(result as SyncConnector);
                else onStopSync?.Invoke();
            }
        }

        protected SyncHost getHoster()
        {
            if (isComplete)
                return Program.host;
            else
                throw new NullReferenceException("Can't get Instance of the Hoster.");
        }

        public string getName()
        {
            return Name;
        }

        public string getAuthor()
        {
            return Author;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
