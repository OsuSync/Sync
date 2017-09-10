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
using System.Threading;

namespace NowPlaying
{
    public class NowPlaying : Plugin, IFilter, ISourceDanmaku
    {
        private MessageDispatcher MainMessager = null;
        private MSNHandler handler = null;
        private OSUStatus osuStat = new OSUStatus();

        PluginConfigurationManager config;

        public static ConfigurationElement OsuFolderPath { get; set; } = "";
        private bool supportAdvanceInfo { get => CurrentBeatmapList != null; }
        Stopwatch sw = new Stopwatch();

        List<BeatmapEntry> CurrentBeatmapList;
        FileSystemWatcher CurrentOsuFilesWatcher;
        BeatmapEntry CurrentPlayingBeatmap;

        public delegate void OnCurrentPlayingBeatmapChangedFunc(BeatmapEntry new_beatmap);
        public event OnCurrentPlayingBeatmapChangedFunc OnCurrentPlayingBeatmapChangedEvent;

        public NowPlaying() : base("Now Playing", "Deliay")
        {
        }

        private void InitAdvance()
        {
            if (!Directory.Exists(OsuFolderPath))
                return;

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                if (string.IsNullOrWhiteSpace(OsuFolderPath))
                {
                    IO.CurrentIO.WriteColor($"[NowPlaying]未设置osu文件夹路径，将自行搜寻当前运行中的osu程序来自动设置", ConsoleColor.Yellow);

                    var processes = Process.GetProcessesByName(@"osu!");
                    if (processes.Length != 0)
                    {
                        OsuFolderPath = processes[0].MainModule.FileName.Replace(@"osu!.exe", string.Empty);
                        IO.CurrentIO.WriteColor($"[NowPlaying]Found osu!.exe ,Get osu folder:{OsuFolderPath}", ConsoleColor.Green);
                    }
                    else
                    {
                        IO.CurrentIO.WriteColor($"[NowPlaying]未设置osu文件夹路径，也没运行中的osu程序，无法使用此插件其他高级功能，请设置好路径并重新启动osuSync才能继续使用", ConsoleColor.Red);
                    }
                }

                var currentDatabase = OsuDb.Read(OsuFolderPath + "osu!.db");
                Console.WriteLine($"========={sw.ElapsedMilliseconds}=============");
                sw.Stop();
                CurrentBeatmapList = currentDatabase.Beatmaps;
                CurrentOsuFilesWatcher = new FileSystemWatcher(OsuFolderPath + @"Songs", "*.osu");
                CurrentOsuFilesWatcher.EnableRaisingEvents = true;
                CurrentOsuFilesWatcher.IncludeSubdirectories = true;
                CurrentOsuFilesWatcher.Changed += CurrentOsuFilesWatcher_Changed;

            }
            catch (Exception e)
            {
                IO.CurrentIO.WriteColor($"[NowPlaying]trying support advance features failed!,{e.Message}", ConsoleColor.Yellow);
                CurrentBeatmapList = null;
            }
        }

        private void CurrentOsuFilesWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            string content = "";

            Thread.Sleep(10);//如果没这货就会和屙屎程序发生IO冲突

            using (StreamReader reader = new StreamReader(File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                content = reader.ReadToEnd();
            }

            BeatmapEntry beatmap = OsuFileParser.ParseText(content);

            if (beatmap == null)
            {
                return;
            }

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

        private void NowPlaying_onInitPlugin(PluginEvents.InitPluginEvent e)
        {
            base.EventBus.BindEvent<InitFilterEvent>((filter) => filter.Filters.AddFilter(this));
            base.EventBus.BindEvent<LoadCompleteEvent>(evt => MainMessager = evt.Host.Messages);
            handler = new MSNHandler();

            Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);
            handler.Load();
            base.EventBus.BindEvent<StatusChangeEvent>(OnOSUStatusChange);

            InitAdvance();
            if (supportAdvanceInfo)
                base.EventBus.BindEvent<StatusChangeEvent>(OnOsuStatusAdvanceChange);

            handler.StartHandler();
        }

        private void OnOsuStatusAdvanceChange(StatusChangeEvent stat)
        {
            if (!supportAdvanceInfo)
                return;

            var currentOsuStat = stat.CurrentStatus;

            sw.Reset();
            sw.Start();

            var query_result = (!string.IsNullOrWhiteSpace(currentOsuStat.title ?? currentOsuStat.artist)) ? CurrentBeatmapList.AsParallel().Where(
                (beatmap) => (((currentOsuStat.title.Trim() == beatmap.TitleUnicode.Trim()) || (currentOsuStat.title.Trim() == beatmap.Title.Trim())) && currentOsuStat.diff == beatmap.Difficulty && ((currentOsuStat.artist.Trim() == beatmap.ArtistUnicode.Trim()) || (currentOsuStat.artist.Trim() == beatmap.Artist.Trim())))
                ) : null;

            var temp_beatmap = CurrentPlayingBeatmap;
            CurrentPlayingBeatmap = null;

            if (query_result != null && query_result.Count() != 0)
            {
                IO.CurrentIO.WriteColor($"query_result count:{query_result.Count()}\ttime={sw.ElapsedMilliseconds}ms\t", ConsoleColor.Cyan);
                BeatmapEntry beatmap = query_result.First();
                var title = string.IsNullOrWhiteSpace(beatmap.TitleUnicode) ? beatmap.Title : beatmap.TitleUnicode;
                var artist = string.IsNullOrWhiteSpace(beatmap.ArtistUnicode) ? beatmap.Artist : beatmap.ArtistUnicode;
                IO.CurrentIO.WriteColor($"[{beatmap.SongSource}]({artist}) - {title}[{beatmap.Difficulty}](AR/HP/OD/CS:{beatmap.DiffAR}/{beatmap.DiffHP}/{beatmap.DiffOD}/{beatmap.DiffCS})", ConsoleColor.Cyan);
                CurrentPlayingBeatmap = beatmap;
            }

            sw.Stop();

            if (temp_beatmap != CurrentPlayingBeatmap)
            {
                OnCurrentPlayingBeatmapChangedEvent?.Invoke(CurrentPlayingBeatmap);
            }

            return;
        }

        private void OnOSUStatusChange(StatusChangeEvent stat)
        {
            osuStat = stat.CurrentStatus;
#if (DEBUG)
            Sync.Tools.IO.CurrentIO.WriteColor(osuStat.status + " " + osuStat.artist + " - " + osuStat.title, ConsoleColor.DarkCyan);
#endif
            return;
        }

        public void onMsg(ref IMessageBase msg)
        {
            if (!msg.Message.RawText.StartsWith("?np"))
                return;

            msg.Cancel = true;
            string param = msg.Message.RawText.Replace("?np", string.Empty).Trim();
            switch (param)
            {
                case "":
                    SendCurrentStatus();
                    break;

                case "-setid":
                case "-sid":
                    SendCurrentBeatmapSetID();
                    break;

                case "-hp":
                    SendCurrentBeatmapHP();
                    break;

                case "-od":
                    SendCurrentBeatmapOD();
                    break;

                case "-cs":
                    SendCurrentBeatmapCS();
                    break;

                case "-ar":
                    SendCurrentBeatmapAR();
                    break;

                case "-id":
                    SendCurrentBeatmapID();
                    break;

                default:
                    MainMessager.onIRC(Sync.Tools.Configuration.Client, $"无效的命令\"{param}\"");
                    break;
            }
        }

        private void SendCurrentStatus()
        {
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
                MainMessager.onIRC(string.Empty, "我在" + strMsg + osuStat.title.Substring(0, 14) + "...");
            }
            else
            {
                MainMessager.onIRC(string.Empty, "我在" + strMsg + osuStat.title);
            }
        }

        public void SendCurrentBeatmapSetID() => MainMessager.onIRC(string.Empty, CurrentPlayingBeatmap != null ? $"当前铺面SetID:{CurrentPlayingBeatmap.BeatmapSetId}" : $"咕咕咕,当前并没打任何图");

        public void SendCurrentBeatmapID() => MainMessager.onIRC(string.Empty, CurrentPlayingBeatmap != null ? $"当前铺面ID:{CurrentPlayingBeatmap.BeatmapId}" : $"咕咕咕,当前并没打任何图");

        public void SendCurrentBeatmapAR() => MainMessager.onIRC(string.Empty, CurrentPlayingBeatmap != null ? $"当前铺面AR:{CurrentPlayingBeatmap.DiffAR}" : $"咕咕咕,当前并没打任何图");

        public void SendCurrentBeatmapHP() => MainMessager.onIRC(string.Empty, CurrentPlayingBeatmap != null ? $"当前铺面HP:{CurrentPlayingBeatmap.DiffHP}" : $"咕咕咕,当前并没打任何图");

        public void SendCurrentBeatmapCS() => MainMessager.onIRC(string.Empty, CurrentPlayingBeatmap != null ? $"当前铺面CS:{CurrentPlayingBeatmap.DiffCS}" : $"咕咕咕,当前并没打任何图");

        public void SendCurrentBeatmapOD() => MainMessager.onIRC(string.Empty, CurrentPlayingBeatmap != null ? $"当前铺面OD:{CurrentPlayingBeatmap.DiffOD}" : $"咕咕咕,当前并没打任何图");

        [Obsolete("Replace with EventBus", true)]
        public void registerCallback(Func<IOSUStatus, Task<bool>> callback)
        {
            ((IMSNHandler)handler).registerCallback(callback);
        }
    }
}
