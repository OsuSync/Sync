using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using static Sync.Tools.DefaultI18n;
using Sync.Command;

namespace Sync.Plugins
{ 
    public class PluginEvents : BaseEventDispatcher
    {
        public abstract class PluginEvent : IBaseEvent { }

        public class InitPluginEvent : PluginEvent { }
        public class InitSourceEvent : PluginEvent
        {
            public SourceManager Sources { get; private set; }
            public InitSourceEvent(SourceManager source)
            {
                Sources = source;
            }
        }
        public class InitFilterEvent : PluginEvent
        {
            public FilterManager Filters { get; private set; }
            public InitFilterEvent(FilterManager filters)
            {
                Filters = filters;
            }
        }
        public class InitCommandEvent : PluginEvent
        {
            public CommandManager Commands { get; private set; }
            public InitCommandEvent(CommandManager commands)
            {
                Commands = commands;
            }
        }
        public class LoadCompleteEvent : PluginEvent
        {
            public SyncHost Host { get; private set; }
            public LoadCompleteEvent(SyncHost host)
            {
                Host = host;
            }
        }
        public class SyncManagerCompleteEvent : PluginEvent
        {
            public SyncManager Manager { get; private set; }
            public SyncManagerCompleteEvent()
            {
                Manager = Program.host.SyncInstance;
            }
        }

        public static readonly PluginEvents Instance = new PluginEvents();
        private PluginEvents()
        {
            EventDispatcher.Instance.RegistNewDispatcher(GetType());
        }
    }

    public class PluginManager
    {

        List<Plugin> pluginList;
        private List<Assembly> asmList;
        internal PluginManager()
        {

        }

        [Obsolete("Instead with EventBus(PluginEvents class)", true)]
        internal void Loader<T>(T instance)
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => instance);
            }
        }

        internal int LoadCommnads()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitCommandEvent(Program.host.Commands));
            return Program.host.Commands.Dispatch.count;
        }

        internal int LoadSources()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitSourceEvent(Program.host.Sources));
            return Program.host.Sources.SourceList.Count();
        }

        internal int LoadFilters()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitFilterEvent(Program.host.Filters));
            return Program.host.Filters.Count;
        }

        internal void ReadySync()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.SyncManagerCompleteEvent());
        }

        [Obsolete("Event tigger move to SourceBase", true)]
        internal void StartSync()
        {
            Loader(Program.host.SyncInstance.Connector);
        }

        [Obsolete("Event tigger move to SourceBase", true)]
        internal void StopSync()
        {
            Loader<SyncConnector>(null);
        }

        public IEnumerable<Plugin> GetPlugins()
        {
            return pluginList;
        }

        internal void ReadyProgram()
        {
            PluginEvents.Instance.RaiseEventAsync<PluginEvents.LoadCompleteEvent>(new PluginEvents.LoadCompleteEvent(Program.host));
        }

        internal int LoadPlugins()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            pluginList = new List<Plugin>();
            asmList = new List<Assembly>();
            asmList.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            if (!Directory.Exists(path)) return 0;
            Directory.SetCurrentDirectory(path);


            foreach (string file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    if (asmList.Where(a => a.Location == file).Count() != 0)
                        continue;
                    Assembly asm = Assembly.LoadFrom(file);
                    asmList.Add(asm);
                }
                catch(Exception e)
                {
                    IO.CurrentIO.WriteColor(String.Format(LANG_LoadPluginErr, file, e.Message), ConsoleColor.Red);
                    continue;
                }
            }

            foreach (Assembly asm in asmList)
            {
                try
                {
                    foreach (Type t in asm.GetExportedTypes())
                    {
                        Type it = asm.GetType(t.FullName);
                        if (it == null ||
                            !it.IsClass || !it.IsPublic ||
                            !typeof(Plugin).IsAssignableFrom(it) ||
                            typeof(Plugin) == it)
                            continue;

                        object pluginTest = asm.CreateInstance(it.FullName);
                        if (pluginTest == null || !(pluginTest is Plugin)) continue;
                        Plugin plugin = pluginTest as Plugin;
                        IO.CurrentIO.WriteColor(String.Format(LANG_LoadingPlugin, plugin.Name), ConsoleColor.White);
                        PluginEvents.Instance.RaiseEventAsync<PluginEvents.InitPluginEvent>(new PluginEvents.InitPluginEvent());
                        pluginList.Add(plugin);
                    }
                }
                catch (Exception e)
                {
                    IO.CurrentIO.WriteColor(String.Format(LANG_NotPluginErr, asm.FullName ,e.Message), ConsoleColor.Red);
                    continue;
                }
            }
            return pluginList.Count;
        }
    }
}
