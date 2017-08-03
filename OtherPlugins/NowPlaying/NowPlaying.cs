using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync;
using Sync.Command;
using Sync.MessageFilter;
using System.Threading.Tasks;
using osu_database_reader;
using Sync.Tools;
using System.Diagnostics;
using System.IO;
using Sync.Source;

namespace NowPlaying
{
    public class NowPlaying : Plugin, IFilter, ISourceDanmaku, IMSNHandler
    {
        private MessageDispatcher MainMessager = null;
        private MSNHandler handler = null;
        private OSUStatus osuStat = new OSUStatus();
        private BeatmapEntry currentPlayingBeatmap=null;

        private OsuDb currentDatabase;
        private ConfigurationElement OsuFolderPath = @"H:\osu!\";
        private bool supportAdvanceInfo { get => currentDatabase != null; }
        private Stopwatch sw = new Stopwatch();
        private static PluginConfiuration plugin_config;

        public NowPlaying() : base("Now Playing", "Deliay")
        {
            base.onInitFilter += filter => filter.AddFilter(this);
            base.onInitPlugin += NowPlaying_onInitPlugin;
            base.onLoadComplete += host => MainMessager = host.Messages;
            handler = new MSNHandler();
        }

        private void NowPlaying_onInitPlugin()
        {
            Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);
            handler.Load();

            handler.registerCallback(p =>
            {
                return new Task<bool>(OnOSUStatusChange, p);
            });

            #region InitAdvanceQuery

            InitAdvanceQuery();

            #endregion
            
            handler.StartHandler();
        }

        private void InitAdvanceQuery()
        {
            if (!Directory.Exists(OsuFolderPath))
                return;

            try
            {
                currentDatabase = OsuDb.Read(OsuFolderPath + "osu!.db");
            }
            catch { currentDatabase = null; }
        }


        private bool OnOSUStatusChange(object stat)
        {
            osuStat = (OSUStatus)stat;

            if (!supportAdvanceInfo)
                return true;

            #region TryGetPlayingBeatmap
            
            #if DEBUG
            sw.Reset();
            sw.Start();
            #endif

            var query_result = currentDatabase.Beatmaps.AsParallel().Where((beatmap) => {
                if (((osuStat.title == beatmap.TitleUnicode) || (osuStat.title == beatmap.Title)) && osuStat.diff == beatmap.Difficulty)
                    return true;
                return false;
            });

            #if DEBUG
            IO.CurrentIO.WriteColor($"query_result count:{query_result.Count()}\ttime={sw.ElapsedMilliseconds}ms\t", ConsoleColor.Cyan);
            sw.Stop();
            #endif

            if (query_result.Count() != 0)
            {
                BeatmapEntry beatmap = query_result.First();
                IO.CurrentIO.WriteColor($"[{beatmap.SongSource}]({beatmap.ArtistUnicode}) - {beatmap.TitleUnicode}[{beatmap.Difficulty}](AR/HP/OD/CS:{beatmap.DiffAR}/{beatmap.DiffHP}/{beatmap.DiffOD}/{beatmap.DiffCS})", ConsoleColor.Cyan);

                currentPlayingBeatmap = beatmap;
                //todo : send message
            }
            else
            {
                currentPlayingBeatmap = null;
            }

            #endregion

            return true;
        }
        
        public void onMsg(ref IMessageBase msg)
        {
            if (msg.Message.RawText.Equals("?np"))
            {
                msg.Cancel = true;
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
    }
}
