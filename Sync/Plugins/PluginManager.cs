using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Sync.Plugins
{
    class PluginManager
    {
        List<Plugin> pluginList;
        internal PluginManager()
        {
            ConsoleWriter.WriteColor("载入了 " + LoadPlugins() + " 个插件。", ConsoleColor.Green);
        }

        internal int LoadCommnads()
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => Program.host.Commands);
            }

            return Program.host.Commands.Dispatch.count;
        }

        internal int LoadSources()
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => Program.host.Sources);
            }
            return Program.host.Sources.SourceList.Count();
        }

        internal int LoadFilters()
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => Program.host.Filters);
            }
            return Program.host.Filters.Count;
        }

        internal void ReadySync()
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => Program.host.SyncInstance);
            }
        }

        internal void StartSync()
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => Program.host.SyncInstance.Connector);
            }
        }

        internal void StopSync()
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent<SyncConnector>(() => null);
            }
        }

        public IEnumerable<Plugin> GetPlugins()
        {
            return pluginList;
        }

        internal void ReadyProgram()
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => Program.host);
            }
        }

        internal int LoadPlugins()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            string interfaceName = typeof(Plugin).FullName;
            pluginList = new List<Plugin>();

            if (!Directory.Exists(path)) return 0;

            foreach(string file in Directory.GetFiles(path, "*.dll"))
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(file);
                    foreach (Type t in asm.GetExportedTypes())
                    {
                        Type it = asm.GetType(t.FullName);
                        if (it == null ||
                            !it.IsClass || !it.IsPublic ||
                            !typeof(Plugin).IsAssignableFrom(it))
                            continue;

                        object pluginTest = Activator.CreateInstance(it);
                        if (pluginTest == null || !(pluginTest is Plugin)) continue;
                        Plugin plugin = pluginTest as Plugin;
                        plugin.onEvent(() => plugin);
                        pluginList.Add(plugin);
                    }
                }
                catch (Exception e)
                {

                    ConsoleWriter.WriteColor(file + " 不是有效插件(加载错误:" + e.Message + ")", ConsoleColor.Red);
                    continue;
                }
            }
            return pluginList.Count;
        }
    }
}
