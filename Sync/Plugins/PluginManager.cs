using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using static Sync.Tools.DefaultI18n;
using Sync.Command;
using Sync.Client;
using Sync.Source;

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

        public class InitClientEvent : PluginEvent
        {
            public ClientManager Clients { get; private set; }
            public InitClientEvent(ClientManager clients)
            {
                Clients = clients;
            }
        }

        public class InitSourceWarpperEvent : PluginEvent
        {
            public SourceWorkWrapper SourceWrapper { get; private set; }
            public InitSourceWarpperEvent(SourceWorkWrapper wrapper)
            {
                SourceWrapper = wrapper;
            }
        }

        public class InitClientWarpperEvent : PluginEvent
        {
            public ClientWorkWrapper ClientWrapper { get; private set; }
            public InitClientWarpperEvent(ClientWorkWrapper wrapper)
            {
                ClientWrapper = wrapper;
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
            //public SyncManager Manager { get; private set; }
            //public SyncManagerCompleteEvent()
            //{
            //    Manager = Program.host.SyncInstance;
            //}
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

        internal int LoadCommnads()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitCommandEvent(SyncHost.Instance.Commands));
            return SyncHost.Instance.Commands.Dispatch.count;
        }

        internal int LoadSources()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitSourceEvent(SyncHost.Instance.Sources));
            return SyncHost.Instance.Sources.SourceList.Count();
        }

        internal int LoadFilters()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitFilterEvent(SyncHost.Instance.Filters));
            return SyncHost.Instance.Filters.Count;
        }

        internal int LoadClients()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitClientEvent(SyncHost.Instance.Clients));
            return SyncHost.Instance.Clients.Count;
        }

        internal void ReadySync()
        {
            PluginEvents.Instance.RaiseEvent(new PluginEvents.SyncManagerCompleteEvent());
        }


        public IEnumerable<Plugin> GetPlugins()
        {
            return pluginList;
        }

        internal void ReadyProgram()
        {
            PluginEvents.Instance.RaiseEventAsync(new PluginEvents.LoadCompleteEvent(SyncHost.Instance));
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
                        PluginEvents.Instance.RaiseEventAsync(new PluginEvents.InitPluginEvent());
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
