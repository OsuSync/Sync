using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Sync.Plugins
{
    public class PluginManager
    {

        List<Plugin> pluginList;
        private List<Assembly> asmList;
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
            asmList = new List<Assembly>();

            if (!Directory.Exists(path)) return 0;
            Directory.SetCurrentDirectory(path);


            foreach (string file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(file);
                    asmList.Add(asm);
                }
                catch(Exception e)
                {
                    ConsoleWriter.WriteColor("文件:" + file + " 无法加载(加载错误:" + e.Message + ")", ConsoleColor.Red);
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
                            !typeof(Plugin).IsAssignableFrom(it))
                            continue;

                        object pluginTest = asm.CreateInstance(it.FullName);
                        if (pluginTest == null || !(pluginTest is Plugin)) continue;
                        Plugin plugin = pluginTest as Plugin;
                        plugin.onEvent(() => plugin);
                        pluginList.Add(plugin);
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriter.WriteColor(asm.FullName + " 不是有效插件(加载错误:" + e.Message + ")", ConsoleColor.Red);
                    continue;
                }
            }
            return pluginList.Count;
        }

        public static IPlugin GetPlugin(string pluginIdentityName)
        {
            return pluginMap[pluginIdentityName];
        }
    }
}
