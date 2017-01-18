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
        List<IPlugin> pluginList;

        static Dictionary<string, IPlugin> pluginMap=new Dictionary<string, IPlugin>();

        public PluginManager()
        {
            ConsoleWriter.WriteColor("载入了 " + LoadPlugins() + " 个插件。", ConsoleColor.Green);
        }

        public int LoadCommnads()
        {
            foreach (IPlugin item in pluginList)
            {
                item.onInitCommand(Program.commands);
            }

            return Program.commands.Dispatch.count;
        }

        public int LoadSources()
        {
            foreach (IPlugin item in pluginList)
            {
                item.onInitSource(Program.sources);
            }
            return Program.sources.SourceList.Count();
        }

        public int LoadFilters()
        {
            foreach (IPlugin item in pluginList)
            {
                item.onInitFilter(Program.filters);
            }
            return Program.filters.Count;
        }

        public void ReadySync()
        {
            foreach (IPlugin item in pluginList)
            {
                item.onSyncMangerComplete(Program.sync);
            }
        }

        public int LoadPlugins()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            string interfaceName = typeof(IPlugin).FullName;
            pluginList = new List<IPlugin>();

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
                            it.GetInterface(interfaceName) == null)
                            continue;

                        object pluginTest = Activator.CreateInstance(it);
                        if (pluginTest == null || !(pluginTest is IPlugin)) continue;
                        IPlugin plugin = pluginTest as IPlugin;
                        plugin.onInitPlugin();
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

        public static IPlugin GetPlugin(string pluginIdentityName)
        {
            return pluginMap[pluginIdentityName];
        }
    }
}
