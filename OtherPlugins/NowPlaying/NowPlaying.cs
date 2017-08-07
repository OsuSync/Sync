using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync;
using Sync.Command;
using Sync.MessageFilter;
using System.Threading.Tasks;
using Sync.Tools;
using System.IO;
using osu_database_reader;
using System.Diagnostics;

namespace NowPlaying
{
    public class NowPlaying : Plugin, IFilter, ISourceDanmaku, IMSNHandler
    {
        private MessageDispatcher MainMessager = null;
        private MSNHandler handler = null;
        private OSUStatus osuStat = new OSUStatus();
        
        public ConfigurationElement OsuFolderPath = "H:\\osu!\\";
        private bool supportAdvanceInfo { get => CurrentBeatmapList != null; }
        Stopwatch sw = new Stopwatch();

        List<BeatmapEntry> CurrentBeatmapList;
        FileSystemWatcher CurrentOsuFilesWatcher;

        public NowPlaying() : base("Now Playing", "Deliay")
        {
            base.onInitFilter += filter => filter.AddFilter(this);
            base.onInitPlugin += NowPlaying_onInitPlugin;
            base.onLoadComplete += host => MainMessager = host.Messages;
            handler = new MSNHandler();
        }

        private void InitAdvance()
        {
            if (!Directory.Exists(OsuFolderPath))
                return;

            try
            {
                var currentDatabase = OsuDb.Read(OsuFolderPath + "osu!.db");
                CurrentBeatmapList = currentDatabase.Beatmaps;
                CurrentOsuFilesWatcher = new FileSystemWatcher(OsuFolderPath + @"Songs", "*.osu");
                CurrentOsuFilesWatcher.EnableRaisingEvents = true;
                CurrentOsuFilesWatcher.IncludeSubdirectories = true;
                CurrentOsuFilesWatcher.Changed += CurrentOsuFilesWatcher_Changed;
            }
            catch
            {
                CurrentBeatmapList = null;
            }
        }

        private void CurrentOsuFilesWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            BeatmapEntry beatmap = OsuFileParser.ParseText(File.ReadAllText(e.FullPath));

            var select_beatmaps = CurrentBeatmapList.AsParallel().Where((enum_beatmap) => 
            {
                if (((enum_beatmap.Title.Trim() == beatmap.Title.Trim())) && enum_beatmap.Difficulty == beatmap.Difficulty && ((enum_beatmap.Artist.Trim() == beatmap.Artist.Trim())))
                    return true;
                return false;
            });

            if (select_beatmaps.Count() != 0)
            {
                CurrentBeatmapList.Remove(select_beatmaps.First());
            }

            CurrentBeatmapList.Add(beatmap);

            #if DEBUG

            IO.CurrentIO.WriteColor($"file {e.Name} was modified/created.beatmap :{beatmap.ArtistUnicode??beatmap.Artist} - {beatmap.TitleUnicode??beatmap.Title}", ConsoleColor.Green);
            
            #endif
        }

        private void NowPlaying_onInitPlugin()
        {
            Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);
            handler.Load();
            handler.registerCallback(p =>
            {
                return new Task<bool>(OnOSUStatusChange, p);
            });


            InitAdvance();
            if (supportAdvanceInfo)
                handler.registerCallback(p =>new Task<bool>(OnOsuStatusAdvanceChange,p));

            handler.StartHandler();
        }

        private bool OnOsuStatusAdvanceChange(object stat)
        {
            if (!supportAdvanceInfo)
                return false;

            var currentOsuStat = (OSUStatus)stat;

            sw.Reset();
            sw.Start();
            var query_result=CurrentBeatmapList.AsParallel().Where((beatmap) => {
                if (((currentOsuStat.title.Trim() == beatmap.TitleUnicode.Trim()) || (currentOsuStat.title.Trim() == beatmap.Title.Trim())) && currentOsuStat.diff == beatmap.Difficulty && ((currentOsuStat.artist.Trim() == beatmap.ArtistUnicode.Trim()) || (currentOsuStat.artist.Trim() == beatmap.Artist.Trim())))
                    return true;
                return false;
            });

            if (query_result.Count() != 0)
            {
                IO.CurrentIO.WriteColor($"query_result count:{query_result.Count()}\ttime={sw.ElapsedMilliseconds}ms\t", ConsoleColor.Cyan);  
                BeatmapEntry beatmap = query_result.First();
                var title = string.IsNullOrWhiteSpace(beatmap.TitleUnicode) ? beatmap.Title : beatmap.TitleUnicode;
                var artist = string.IsNullOrWhiteSpace(beatmap.ArtistUnicode) ? beatmap.Artist : beatmap.ArtistUnicode;
                IO.CurrentIO.WriteColor($"[{beatmap.SongSource}]({artist}) - {title}[{beatmap.Difficulty}](AR/HP/OD/CS:{beatmap.DiffAR}/{beatmap.DiffHP}/{beatmap.DiffOD}/{beatmap.DiffCS})", ConsoleColor.Cyan);
            }

            sw.Stop();

            return true;
        }

        private bool OnOSUStatusChange(object stat)
        {
            osuStat = (OSUStatus)stat;
#if (DEBUG)
            Sync.Tools.IO.CurrentIO.WriteColor(osuStat.status + " " + osuStat.artist + " - " + osuStat.title, ConsoleColor.DarkCyan);
#endif



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
