using Sync.Tools;
using System;
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

        static Mutex mutex = new Mutex(true, "{781d2da2-1b44-46d9-8b01-e1d59adc018b}");
        private static bool ChechkInstanceExist()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
                return false;
            else
                return true;
        }

        static void Main(string[] args)
        {
            if (Updater.ApplyUpdate(args)) return;
            I18n.Instance.ApplyLanguage(new DefaultI18n());

            if (ChechkInstanceExist())
            {
                CurrentIO.WriteColor(DefaultI18n.LANG_Instance_Exist, ConsoleColor.Red);
                Console.ReadKey();
                return;
            }

            SetConsoleCtrlHandler(cancelHandler, true);

            while (true)
            {
                SyncHost.Instance = new SyncHost();
                SyncHost.Instance.Load();

                CurrentIO.WriteWelcome();

                SyncHost.Instance.Plugins.ReadySync();

                if(Updater.IsUpdated) IO.CurrentIO.WriteColor("Sync already update!", ConsoleColor.Green);

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
