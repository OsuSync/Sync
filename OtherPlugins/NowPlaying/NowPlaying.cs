using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync;
using Sync.Command;
using Sync.MessageFilter;

namespace NowPlaying
{
    public class NowPlaying : FilterBase, IPlugin, IDanmaku
    {
        public const string PLUGIN_NAME = "Now Playing";
        public const string PLUGIN_AUTHOR = "Deliay";
        private FilterManager MainFilter = null;
        private OSUStatus osuStat = new OSUStatus();
        private MSNHandler msn;

        public OSUStatus OsuStatus
        {
            private set { }
            get { return osuStat; }
        }

        public string Author
        {
            get
            {
                return PLUGIN_AUTHOR;
            }
        }

        public string Name
        {
            get
            {
                return PLUGIN_NAME;
            }
        }



        public void onInitCommand(CommandManager manager)
        {

        }

        public void onInitFilter(FilterManager filter)
        {
            MainFilter = filter;
            filter.addFilter(this);
        }

        public void onInitPlugin()
        {
            msn = new MSNHandler();
            
            Sync.Tools.ConsoleWriter.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);
            msn.registerCallback(p => {
                return new System.Threading.Tasks.Task<bool>(OnOSUStatusChange, p);
            });

            msn.StartHandler();
            
        }

        public void onInitSource(SourceManager manager)
        {

        }

        public void onSyncMangerComplete(SyncManager sync)
        {

        }

        private bool OnOSUStatusChange(object stat)
        {
            osuStat = (OSUStatus)stat;
#if (DEBUG)
            Sync.Tools.ConsoleWriter.WriteColor(osuStat.status + " " + osuStat.artist + " - " + osuStat.title, ConsoleColor.DarkCyan);
#endif
            return true;
        }

        public override void onMsg(ref MessageBase msg)
        {
            if (msg.message.RawText.Equals("?np"))
            {
                msg.cancel = true;
                string strMsg = string.Empty;
                int max = 20;
                if (osuStat.status == "Playing")
                {
                    strMsg = "玩";
                }
                else if (osuStat.status == "Editing")
                {
                    strMsg = "做";
                }
                else //include  Listening
                {
                    strMsg = "听";
                }
                if (osuStat.title.Length > 17)
                {
                    MainFilter.onIRC(Sync.Tools.Configuration.TargetIRC, "我在" + strMsg + osuStat.title.Substring(0, 14) + "...");
                }
                else
                {
                    MainFilter.onIRC(Sync.Tools.Configuration.TargetIRC, "我在" + strMsg + osuStat.title);
                }
            }
            
        }
    }
}
