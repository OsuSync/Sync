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
            try
            {
                (new StartupHelper(args)).Start();
            }
            catch (Exception e)
            {
                SentryHelper.Instance.RepoterError(e);
                Thread.Sleep(2000);
                Environment.Exit(1);
            }
        }
    }
}
