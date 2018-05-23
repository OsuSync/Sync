using Sync.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using static Sync.Tools.IO;

namespace Sync
{
    public static class Program
    {
        internal delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        internal static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        internal static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        private static bool HandlerRoutine(int CtrlType)
        {
            CurrentIO.Write("Exiting...");
            SyncHost.Instance.SaveSync();
            CurrentIO.Write("Saved.");
            Program.SetConsoleCtrlHandler(Program.cancelHandler, false);
            return true;
        }

        private static void WaitLastSyncExit(MemoryMappedFile mmf)
        {
            using (var mmfs = mmf.CreateViewStream())
            {
                byte[] buf = new byte[4];
                mmfs.Read(buf, 0, 4);

                //get other sync.exe pid
                int pid = BitConverter.ToInt32(buf, 0);

                if (pid != 0)
                {
                    CurrentIO.WriteColor(DefaultI18n.LANG_Instance_Exist, ConsoleColor.Red);
                    bool wait = true;
                    while (wait)
                    {
                        try
                        {
                            Process.GetProcessById(pid);
                            Thread.Sleep(100);
                        }
                        catch (ArgumentException)
                        {
                            wait = false;
                        }
                    }
                }
                byte[] pidBytes = BitConverter.GetBytes(Process.GetCurrentProcess().Id);
                mmfs.Seek(0, SeekOrigin.Begin);
                //write current sync.exe pid
                mmfs.Write(pidBytes, 0, 4);
            }
        }

        static void Main(string[] args)
        {
            if (Updater.ApplyUpdate(args)) return;
            I18n.Instance.ApplyLanguage(new DefaultI18n());
            SetConsoleCtrlHandler(cancelHandler, true);

            using (var mmf = MemoryMappedFile.CreateOrOpen("{781d2da2-1b44-46d9-8b01-e1d59adc018b}",4))
            {
                //Check sync.exe is run
                WaitLastSyncExit(mmf);

                while (true)
                {
                    SyncHost.Instance = new SyncHost();
                    SyncHost.Instance.Load();

                    CurrentIO.WriteWelcome();

                    SyncHost.Instance.Plugins.ReadySync();

                    if (Updater.IsUpdated) IO.CurrentIO.WriteColor("Sync is already up to date!", ConsoleColor.Green);

                    string cmd = CurrentIO.ReadCommand();
                    while (true)
                    {
                        SyncHost.Instance.Commands.invokeCmdString(cmd);
                        cmd = CurrentIO.ReadCommand();
                    }
                }
            }
        }

    }
}
