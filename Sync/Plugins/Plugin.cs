using Sync.Command;
using System;

namespace Sync.Plugins
{
    public delegate void SyncManagerCompleteEvt(SyncManager sync);
    public delegate void InitPluginEvt();
    public delegate void InitSourceEvt(SourceManager sources);
    public delegate void InitFilterEvt(FilterManager filters);
    public delegate void InitCommandEvt(CommandManager commands);
    public delegate void LoadCompleteEvt(SyncHost host);
    public delegate void StartSyncEvt(SyncConnector connector);
    public delegate void StopSyncEvt();

    public interface IPlugin
    {
        string getName();
        string getAuthor();
    }

    public abstract class Plugin : IPlugin
    {
        public readonly string Name;
        public readonly string Author;

        public Plugin(string Name, string Author)
        {
            this.Name = Name;
            this.Author = Author;
        }

        protected event InitPluginEvt onInitPlugin;
        protected event SyncManagerCompleteEvt onSyncMangerComplete;
        protected event InitSourceEvt onInitSource;
        protected event InitFilterEvt onInitFilter;
        protected event InitCommandEvt onInitCommand;
        protected event LoadCompleteEvt onLoadComplete;
        protected event StartSyncEvt onStartSync;
        protected event StopSyncEvt onStopSync;
        private bool isComplete = false;

        /// <summary>
        /// 简单实现的简易事件分发器，通过类型来判别应该cast什么事件
        /// </summary>
        /// <typeparam name="T">类别</typeparam>
        /// <param name="handler">产生器</param>
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
                throw new NullReferenceException("当前状态不能立即获得host类实例");
        }

        public string getName()
        {
            return Name;
        }

        public string getAuthor()
        {
            return Author;
        }
    }
}
