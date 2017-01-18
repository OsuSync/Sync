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

    public abstract class Plugin
    {
        internal readonly string Name;
        internal readonly string Author;

        public Plugin(string Name, string Author)
        {
            this.Name = Name;
            this.Author = Author;
        }

        public event InitPluginEvt onInitPlugin;
        public event SyncManagerCompleteEvt onSyncMangerComplete;
        public event InitSourceEvt onInitSource;
        public event InitFilterEvt onInitFilter;
        public event InitCommandEvt onInitCommand;
        public event LoadCompleteEvt onLoadComplete;
        public event StartSyncEvt onStartSync;
        public event StopSyncEvt onStopSync;
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
    }
}
