using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync;
using Sync.Command;
using Sync.Plugins;
using Sync.Tools;
using static RecentlyUserQuery.DefaultLanguage;

namespace RecentlyUserQuery
{
    public class RecentlyMessageQueryPlugin : Plugin
    {
        MessageRecorder recorder = new MessageRecorder();

        public const string PLUGIN_NAME = "Recently Message Query Plugin";
        public const string PLUGIN_AUTHOR = "Dark Projector";
        
        public RecentlyMessageQueryPlugin() : base(PLUGIN_NAME, PLUGIN_AUTHOR)
        {
            ///todo
            I18n.Instance.ApplyLanguage(new DefaultLanguage());

            base.onInitCommand += manager => manager.Dispatch.bind("recently", onProcessCommand, "recently --<command> [arg...] 操作消息记录器相关功能,--help获取相关指令");
            base.onInitFilter += manager => manager.AddFilters(new Danmaku.MessageRecorderFilter(recorder));
            base.onLoadComplete += host => host.Filters.AddFilters(new Osu.MessageRecorderControlFilter(host.Messages, recorder),new Osu.GetUserIdFilter(host.Messages));
            base.onInitPlugin += () => Sync.Tools.IO.CurrentIO.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);
        }

        private bool onProcessCommand(Arguments args)
        {
            string[] newArgs = new string[args.Count+1];
            int i = 0;
            newArgs[0] = "";
            foreach(string argv in args)
                newArgs[1 + i++]=argv;

            if (args.Count != 0 )
                Sync.Tools.IO.CurrentIO.Write(recorder.ProcessCommonCommand(newArgs).Replace(" || ","\n"));
            else
                Sync.Tools.IO.CurrentIO.WriteColor(LANG_HELP,ConsoleColor.Yellow); 
            return true;
        }

        private void SendResponseMessage(string message)
        {
            Sync.Tools.IO.CurrentIO.Write(message);
        }

        public void onConfigurationLoad()
        {
            throw new NotImplementedException();
        }

        public void onConfigurationSave()
        {
            throw new NotImplementedException();
        }
		
    }

    public class UserIdGenerator
    {
        private UserIdGenerator() { }

        private static Dictionary<string, int> idrecorder = new Dictionary<string, int>();
        private static Dictionary<int, string> userNamerecorder = new Dictionary<int, string>();
        private static int current_id;

        public static int GetId(string userName)
        {
            if (idrecorder.ContainsKey(userName))
                return idrecorder[userName];
            int id = current_id++;
            idrecorder[userName] = id;
            userNamerecorder[id] = userName;
            return id;
        }

        //没有id对应的用户就返回String.Empty
        public static string GetUserName(int id)
        {
            return userNamerecorder.ContainsKey(id) ? userNamerecorder[id] : string.Empty;
        }

        public static void Clear()
        {
            idrecorder.Clear();
            userNamerecorder.Clear();
            current_id = 0;
        }
    }
}
