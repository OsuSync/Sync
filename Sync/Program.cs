using Sync.Tools;
using System;
using System.Runtime.InteropServices;
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

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(cancelHandler, true);
            if (Updater.ApplyUpdate(args)) return;
 
            I18n.Instance.ApplyLanguage(new DefaultI18n());

            while (true)
            {
                SyncHost.Instance = new SyncHost();
                SyncHost.Instance.Load();

                CurrentIO.WriteWelcome();

                SyncHost.Instance.Plugins.ReadySync();

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
