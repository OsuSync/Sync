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
    public class NowPlaying : Plugin, IFilter, ISourceDanmaku
    {
        private FilterManager MainFilter = null;
        private OSUStatus osuStat = new OSUStatus();
        public const string Author = "Deliay";
        public const string Name = "Now Playing";

        public NowPlaying() : base(Author, Name)
        {
            base.onInitFilter += filter => filter.AddFilter(this);
            base.onInitPlugin += NowPlaying_onInitPlugin;
            base.onLoadComplete += host => MainFilter = host.Filters;
        }

        private void NowPlaying_onInitPlugin()
        {
            MSNHandler.Load();
            Sync.Tools.ConsoleWriter.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);
            MSNHandler.registerCallback(p =>
            {
                return new System.Threading.Tasks.Task<bool>(OnOSUStatusChange, p);
            });

            MSNHandler.StartHandler();
        }

        private bool OnOSUStatusChange(object stat)
        {
            osuStat = (OSUStatus)stat;
#if (DEBUG)
            Sync.Tools.ConsoleWriter.WriteColor(osuStat.status + " " + osuStat.artist + " - " + osuStat.title, ConsoleColor.DarkCyan);
#endif
            return true;
        }

        public void onMsg(ref MessageBase msg)
        {
            if (msg.message.RawText.Equals("?np"))
            {
                msg.cancel = true;
                string strMsg = string.Empty;
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
