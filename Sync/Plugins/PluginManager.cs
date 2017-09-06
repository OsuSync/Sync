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
        private LinkedList<Type> loadedList;
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

            loadedList = new LinkedList<Type>();
            List<Type> lazylist = new List<Type>();
            //Load all plugins first
            foreach (Assembly asm in asmList)
            {
                foreach (Type item in asm.GetExportedTypes())
                {
                    Type it = asm.GetType(item.FullName);
                    if (it == null ||
                        !it.IsClass || !it.IsPublic ||
                        !typeof(Plugin).IsAssignableFrom(it) ||
                        typeof(Plugin) == it)
                        continue;
                    lazylist.Add(it);
                }
            }

            //looping add for resolve dependency
            do
            {

                lazylist = layerLoader(lazylist);

            } while (lazylist.Count != 0);

            return pluginList.Count;
        }

        private List<Type> layerLoader(IList<Type> asmList)
        {
            List<Type> nextLoad = new List<Type>();
            foreach (Type it in asmList)
            {
                try
                {

                    if (Check_Should_Late_Load(it))
                    {
#if (DEBUG)
                        IO.CurrentIO.WriteColor($"Lazy load [{it.Name}]", ConsoleColor.Green);
#endif
                        nextLoad.Add(it);
                        //Dependency load at this time
                        //Lazy load this plugin at next time
                        continue;
                    }


                    //no dependencies or dependencies all was loaded
                    if (!it.IsSubclassOf(typeof(Plugin))) continue;
                    else
                    {
                        LoadPluginFormType(it);
                        loadedList.AddLast(it);
                    }

                }
                catch (Exception e)
                {
                    IO.CurrentIO.WriteColor(String.Format(LANG_NotPluginErr, it.Name, e.Message), ConsoleColor.Red);
                    continue;
                }
            }

            return nextLoad;
        }

        private bool Check_Should_Late_Load(Type a)
        {

            SyncRequirePlugin requireAttr = a.GetCustomAttribute<SyncRequirePlugin>();
            if (requireAttr == null)
            {
                return false;
            }

            foreach (var item in requireAttr.RequirePluguins)
            {
                //Dependency was been loaded
                if (loadedList.Contains(item)) continue;
                else
                {

                    //Check cycle reference
                    if (Check_A_IS_Reference_TO_B(item, a)) return false;
                    else return true;
                }
            }

            return false;
        }

        private bool Check_A_IS_Reference_TO_B(Type a, Type b)
        {
            SyncRequirePlugin refRequireCheck = a.GetCustomAttribute<SyncRequirePlugin>();
            if (refRequireCheck == null) return false;
            return refRequireCheck.RequirePluguins.Contains(b);
        }

        private Plugin LoadPluginFormType(Type it)
        {
            object pluginTest = it.Assembly.CreateInstance(it.FullName);
            if (pluginTest == null)
            {
                throw new NullReferenceException();
            }

            Plugin plugin = (Plugin)pluginTest;
            IO.CurrentIO.WriteColor(String.Format(LANG_LoadingPlugin, plugin.Name), ConsoleColor.White);

            pluginList.Add(plugin);

            PluginEvents.Instance.RaiseEventAsync(new PluginEvents.InitPluginEvent());
            return plugin;
        }
    }

    public class SyncRequirePlugin : Attribute
    {
        public IReadOnlyList<Type> RequirePluguins;

        public SyncRequirePlugin(params Type[] types)
        {
            RequirePluguins = new List<Type>(types);
        }
    }
}
