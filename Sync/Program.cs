using SharpRaven;
using Sync.Tools;
using System;
using System.Threading;

namespace Sync
{
    public static class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            (new StartupHelper(args)).Start();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SentryHelper.Instance.RepoterError(e.ExceptionObject as Exception);
            Thread.Sleep(2000);
            Environment.Exit(1);
        }
    }
}
