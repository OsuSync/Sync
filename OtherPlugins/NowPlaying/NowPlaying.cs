using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync;
using Sync.Command;
using Sync.MessageFilter;
using System.Threading.Tasks;

namespace NowPlaying
{
    public class NowPlaying : Plugin, IFilter, ISourceDanmaku, IMSNHandler
    {
        private MessageDispatcher MainMessager = null;
        private MSNHandler handler = null;
        private OSUStatus osuStat = new OSUStatus();

        public NowPlaying() : base("Now Playing", "Deliay")
        {
            base.onInitFilter += filter => filter.AddFilter(this);
            base.onInitPlugin += NowPlaying_onInitPlugin;
            base.onLoadComplete += host => MainMessager = host.Messages;
            handler = new MSNHandler();
        }

        private void NowPlaying_onInitPlugin()
        {
            handler.Load();
            Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);
            handler.registerCallback(p =>
            {
                return new System.Threading.Tasks.Task<bool>(OnOSUStatusChange, p);
            });

            handler.StartHandler();
        }


        private bool OnOSUStatusChange(object stat)
        {
            osuStat = (OSUStatus)stat;
#if (DEBUG)
            Sync.Tools.IO.CurrentIO.WriteColor(osuStat.status + " " + osuStat.artist + " - " + osuStat.title, ConsoleColor.DarkCyan);
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
                    MainMessager.onIRC(Sync.Tools.Configuration.TargetIRC, "我在" + strMsg + osuStat.title.Substring(0, 14) + "...");
                }
                else
                {
                    MainMessager.onIRC(Sync.Tools.Configuration.TargetIRC, "我在" + strMsg + osuStat.title);
                }
            }

        }

        public void registerCallback(Func<IOSUStatus, Task<bool>> callback)
        {
            ((IMSNHandler)handler).registerCallback(callback);
        }

        public override void Dispose()
        {
            handler.Dispose();
        }
    }
}
