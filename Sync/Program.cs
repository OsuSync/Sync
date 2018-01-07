using Sync.Tools;
using static Sync.Tools.IO;

namespace Sync
{
    public static class Program
    {

        static void Main(string[] args)
        {

            if(Updater.ApplyUpdate(args)) return;
 
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
