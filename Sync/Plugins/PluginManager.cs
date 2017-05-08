using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using static Sync.Tools.DefaultI18n;
namespace Sync.Plugins
{
    public class PluginManager
    {

        List<Plugin> pluginList;
        private List<Assembly> asmList;
        internal PluginManager()
        {

        }

        internal void Loader<T>(T instance)
        {
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => instance);
            }
        }

        internal int LoadCommnads()
        {
            Loader(Program.host.Commands);
            return Program.host.Commands.Dispatch.count;
        }

        internal int LoadSources()
        {
            Loader(Program.host.Sources);
            return Program.host.Sources.SourceList.Count();
        }

        internal int LoadFilters()
        {
            Loader(Program.host.Filters);
            return Program.host.Filters.Count;
        }

        internal void ReadySync()
        {
            Loader(Program.host.SyncInstance);
        }

        internal void StartSync()
        {
            Loader(Program.host.SyncInstance.Connector);
        }

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
            foreach (Plugin item in pluginList)
            {
                item.onEvent(() => Program.host);
            }
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
                        plugin.onEvent(() => plugin);
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
