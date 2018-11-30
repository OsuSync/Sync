using Sync.Tools;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using static Sync.Tools.IO;

namespace Sync.Tools
{
    public class StartupHelper
    {
        public const string SYNC_GUID = "{781d2da2-1b44-46d9-8b01-e1d59adc018b}";

        internal delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        internal static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        internal static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        private static bool HandlerRoutine(int CtrlType)
        {
            CurrentIO.Write("Exiting...");
            SyncHost.Instance?.SaveSync();
            CurrentIO.Write("Saved.");
            SetConsoleCtrlHandler(cancelHandler, false);
            return true;
        }
 
        private void SyncInstanceLocker(MemoryMappedFile syncMappedFile, bool forceExit)
        {
            using (var syncViewStream = syncMappedFile.CreateViewStream())
            {
                int pid = 0;
                using (var reader = new BinaryReader(syncViewStream))
                {
                    pid = reader.ReadInt32();

                    if (pid != 0)
                    {
                        var oldSync = Process.GetProcesses().FirstOrDefault((proc) => proc.Id == pid);
                        if (oldSync != null)
                        {
                            if (forceExit)
                            {
                                oldSync.Kill();
                            }
                            else
                            {
                                CurrentIO.WriteColor(DefaultI18n.LANG_Instance_Exist, ConsoleColor.Red);
                                int limit = 50, current = 0;
                                while (!oldSync.WaitForExit(100))
                                {
                                    current++;
                                    if (current > limit) break;
                                }

                                if (!oldSync.WaitForExit(100))
                                {
                                    oldSync.Kill();
                                }
                            }
                        }
                    }

                    using (var writer = new BinaryWriter(syncViewStream))
                    {
                        writer.Seek(0, SeekOrigin.Begin);
                        writer.Write(Process.GetCurrentProcess().Id);
                    }
                }
            }
        }

        static void PreInitSync()
        {
            //Initialize I18n
            I18n.Instance.ApplyLanguage(new DefaultI18n());
        }

        static void InitSync()
        {
            //Apply update
            if (Updater.ApplyUpdate(NeedUpdateSync))
                Environment.Exit(0);

            //Add Console close event handler
            SetConsoleCtrlHandler(cancelHandler, true);

            //Initialize Sync core
            SyncHost.Instance = new SyncHost();
            SyncHost.Instance.Load();

            //Sync ready message to all plugins
            SyncHost.Instance.Plugins.ReadySync();

            //Check update
            if (DefaultConfiguration.Instance.CheckUpdateOnStartup.ToBool())
                Updater.update.Latest();

            //Sync program update check
            if (Updater.IsUpdated)
                CurrentIO.WriteColor("Sync is already up to date!", ConsoleColor.Green);

        }

        public static bool ForceStart { get; internal set; }
        public static bool NeedUpdateSync { get; internal set; }

        internal StartupHelper(string[] args)
        {
            (new CommandParser<StartupHelper>(args, StartupArgument.Arguments, StartupArgument.Actions)).ExecuteActionOn(this);
        }

        internal void Start()
        {
            //Check sync.exe is run
            using (var syncMappedFile = MemoryMappedFile.CreateOrOpen(SYNC_GUID, 4))
            {
                PreInitSync();
                SyncInstanceLocker(syncMappedFile, ForceStart);

                InitSync();
                CurrentIO.WriteWelcome();

                while (true)
                {
                    var cmd = CurrentIO.ReadCommand();
                    SyncHost.Instance.Commands.invokeCmdString(cmd);
                }
            }
        }
    }
}
