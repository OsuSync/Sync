using Sync.Command;
using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using static Sync.Tools.IO;
using static Sync.Tools.DefaultI18n;
using Sync.Client;
using Sync.Source;
using System.Reflection;
using System.Diagnostics;

namespace Sync
{
    /// <summary>
    /// A manager for global modules
    /// </summary>
    public class SyncHost
    {
        public static SyncHost Instance { get; internal set; }

        private SourceWorkWrapper sourceWrapper;
        private ClientWorkWrapper clientWrapper;
        private CommandManager commands;
        private PluginManager plugins;
        private SourceManager sources;
        private FilterManager filters;
        private MessageDispatcher messages;
        private ClientManager clients;
        /// <summary>
        /// Internal use only
        /// </summary>
        internal SyncHost()
        {

        }

        /// <summary>
        /// Invoke to load plugins
        /// </summary>
        internal void Load()
        {
            CurrentIO.Write(LANG_Loading);

            //Initial plugins manager
            plugins = new PluginManager();
            CurrentIO.WriteColor(String.Format(LANG_Plugins, plugins.LoadPlugins()), ConsoleColor.Green);

            //Initial danmaku source
            sources = new SourceManager();
            CurrentIO.WriteColor(String.Format(LANG_Sources, plugins.LoadSources()), ConsoleColor.Green);

            //select a danmaku source by config
            try
            {
                sourceWrapper = new SourceWorkWrapper(sources);
                PluginEvents.Instance.RaiseEvent(new PluginEvents.InitSourceWarpperEvent(sourceWrapper));
            }
            catch
            {
                CurrentIO.Write("");
                CurrentIO.WriteColor(LANG_Error, ConsoleColor.Red);
                CurrentIO.WriteColor("Press enter to continue", ConsoleColor.Red);
                CurrentIO.ReadCommand();
            }

            //Get clients singleton
            clients = ClientManager.Instance;
            CurrentIO.WriteColor(String.Format(LANG_Client, plugins.LoadClients()), ConsoleColor.Green);

            clientWrapper = new ClientWorkWrapper(clients);
            PluginEvents.Instance.RaiseEvent(new PluginEvents.InitClientWarpperEvent(clientWrapper));

            commands = new CommandManager();
            CurrentIO.WriteColor(String.Format(LANG_Commands, plugins.LoadCommnads()), ConsoleColor.Green);

            filters = new FilterManager();
            CurrentIO.WriteColor(String.Format(LANG_Filters, plugins.LoadFilters()), ConsoleColor.Green);

            messages = new MessageDispatcher(filters);

            plugins.ReadyProgram();

            CurrentIO.WriteColor(LANG_Ready, ConsoleColor.Cyan);
        }

        /// <summary>
        /// The internal PluginManager instance property
        /// </summary>
        internal PluginManager Plugins { get { return plugins; } }

        /// <summary>
        /// Read only plugins list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Plugin> EnumPluings()
        {
            return plugins.GetPlugins();
        }

        public CommandManager Commands
        {
            get
            {
                return commands;
            }
        }

        public SourceManager Sources
        {
            get
            {
                return sources;
            }
        }

        public FilterManager Filters
        {
            get
            {
                return filters;
            }
        }

        public MessageDispatcher Messages
        {
            get { return messages; }
        }

        public ClientManager Clients { get => clients; }

        public SourceWorkWrapper SourceWrapper { get => sourceWrapper; }

        public ClientWorkWrapper ClientWrapper { get => clientWrapper; }

        public void ExitSync()
        {
            try
            {
                SaveSync();
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        public void SaveSync()
        {
            if (ClientWrapper.Client != null) ClientWrapper.Client?.StopWork();
            if (SourceWrapper.Source != null) SourceWrapper.Source?.Disconnect();

            foreach (var item in PluginConfigurationManager.ConfigurationSet)
            {
                item.SaveAll();
            }

            plugins.GetPluginList().ForEach(p => p.OnExit());

        }

        public void RestartSync()
        {
            try
            {
                SaveSync();
            }
            finally
            {
                ForceRestartSync();
            }
        }

        public void ForceRestartSync()
        {
            Process.Start(Assembly.GetEntryAssembly().Location);
            Environment.Exit(0);
        }
    }
}
