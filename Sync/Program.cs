using Sync.Command;
using Sync.IRC.MessageFilter;
using Sync.Source;
using Sync.Source.BiliBili;
using Sync.Tools;
using System;
using System.Diagnostics;
using static Sync.Tools.ConsoleWriter;

namespace Sync
{
    static class Program
    {

        public static Sync syncInstance = null;
        public static bool loginable = false;

        static void Main(string[] args)
        {
            WriteWelcome();
            WriteConfig();
            Write("Config loaded.");

            if (Configuration.LiveRoomID == 0)
            {
                WriteColor("请配置 'config.ini' 后再开始进行同步操作。\n", ConsoleColor.DarkRed);
                Process.Start(ConfigurationIO.ConfigFile);
            }

            if (Configuration.Provider == Configuration.PROVIDER_BILIBILI)
            {
                syncInstance = new Sync(new BiliBili());
            }
            else
            {
                syncInstance = new Sync(new BiliBili());
            }

            if (syncInstance.GetSource() is ISendable)
            {
                loginable = true;
                WriteColor("提示:当前弹幕源支持游戏内发送到弹幕源的功能，请输入login [用户名] [密码] 来登录!(用户名、密码二者可选输入)\n\n", ConsoleColor.Yellow);
                if(Configuration.LoginCookie.Length > 0)
                {
                    Write("Cookie长度:" + Configuration.LoginCookie.Length);
                    WriteColor("提示：当前已有登录Cookie记录，如需覆盖，请输入login [用户名] [密码]进行覆盖！（用户名密码可选输入）", ConsoleColor.Red);
                }
            }

            CommandManager cm = new CommandManager();

            while (true)
            {
                cm.invokeCmdString(ReadCommand());
            }
        }

    }
}
