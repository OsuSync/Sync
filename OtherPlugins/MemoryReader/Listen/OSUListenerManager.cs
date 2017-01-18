using MemoryReader.BeatmapInfo;
using MemoryReader.Listen.InterFace;
using MemoryReader.Mods;
using MemoryReader.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sync.Plugins;
using System.Reflection;
using System.IO;
using Sync;
using NowPlaying;

namespace MemoryReader.Listen
{
    class OSUListenerManager
    {
        [Flags]
        public enum OsuStatus
        {
            NoFoundProcess,
            Listening,
            Playing,
            Editing
        }

        private MemoryFinder m_memory_finder;

        public OsuStatus CurrentOsuStatus { get; set; }
        private IOSUStatus m_now_player_status=new OSUStatus();
        private bool m_stop=false;
        private Thread m_listen_thread;
        private List<IOSUListener> m_listeners=new List<IOSUListener>();

        private BeatmapSet m_last_beatmapset=new BeatmapSet();
        private Beatmap m_last_beatmap=new Beatmap();
        private ModsInfo m_last_mods = new ModsInfo();
        private NowPlaying.NowPlaying m_now_playing;

        private double m_last_hp=0;
        private double m_last_acc=0;

        public OSUListenerManager(SyncHost host)
        {
            foreach (var t in host.EnumPluings())
            {
                if (t.getName() == "Now Playing")
                {
                    m_now_playing = (NowPlaying.NowPlaying)t;
                    break;
                }
            }
            m_now_playing.registerCallback(p =>
            {
                return new System.Threading.Tasks.Task<bool>(status=>
                {
                    m_now_player_status = (NowPlaying.IOSUStatus)status;
                    return true;
                }, p);
            });

            m_listen_thread = new Thread(ListenLoop);
        }

        public void AddListener(IOSUListener listener)
        {
            m_listeners.Add(listener);
        }

        public void RemoveListener(IOSUListener listener)
        {
            m_listeners.Remove(listener);
        }

        public void Start()
        {
            m_stop = false;
            m_listen_thread.Start();
        }

        public void Stop()
        {
            m_stop=true;
        }

        private void LoadMemoryFinder(Process osu)
        {
            m_memory_finder = new MemoryFinder(osu);
        }

        private void ListenLoop()
        {
            UInt32 count = 0;

            if(GetCurrentOsuStatus()!=OsuStatus.NoFoundProcess)
                LoadMemoryFinder(Process.GetProcessesByName("osu!")[0]);

            while(!m_stop)
            {
                OsuStatus status = GetCurrentOsuStatus(); ;
                //last status
                if (CurrentOsuStatus == OsuStatus.NoFoundProcess && CurrentOsuStatus != status)
                {
                    LoadMemoryFinder(Process.GetProcessesByName("osu!")[0]);
                }
                CurrentOsuStatus = status;

                if (CurrentOsuStatus!=OsuStatus.NoFoundProcess)
                {
                    foreach(var listner in m_listeners)
                    {
                        BeatmapSet beatmapset = GetCurrentBeatmapSet();
                        if(beatmapset.BeatmapSetID!=m_last_beatmapset.BeatmapSetID)
                        {
                            listner.OnCurrentBeatmapSetChange(beatmapset);
                        }
                        m_last_beatmapset = beatmapset;

                        Beatmap beatmap = GetCurrentBeatmap();
                        if (beatmap.BeatmapID != m_last_beatmap.BeatmapID)
                        {
                            listner.OnCurrentBeatmapChange(beatmap);
                        }
                        m_last_beatmap = beatmap;

                        ModsInfo mods = GetCurrentMods();
                        if(mods.Mod!=m_last_mods.Mod)
                        {
                            listner.OnCurrentModsChange(mods);
                        }
                        m_last_mods = mods;

                        if(CurrentOsuStatus==OsuStatus.Playing)
                        {
                            double hp = GetCurrentHP();
                            if (hp != m_last_hp)
                            {
                                listner.OnHPChange(hp);
                            }

                            m_last_hp = hp;

                            double acc = GetCurrentAcc();
                            if(acc!=m_last_acc)
                            {
                                listner.OnAccuracyChange(acc);
                            }
                            m_last_acc = acc;
                        }
                        else
                        {
                            m_last_acc = 100.0;
                            m_last_hp = 200;
                        }
                    }
                }
                else
                {
                    if (count % 1200 == 0)
                    {
                        Sync.Tools.ConsoleWriter.WriteColor("没有发现 OSU! 进程，请打开OSU！", ConsoleColor.Red);
                        count = 0;
                    }
                    count++;
                }

                Thread.Sleep(100);
            }
        }

        private double GetCurrentAcc()
        {
            double acc = 0.0;
            try
            {
                acc = m_memory_finder.GetMemoryDouble(new List<int>() { -0x320, 0x124, 0x384, 0x3c, 0x24, 0x25c, 0x48,0x14});
            }
            catch (ThreadStackNoFoundException e)
            {
                acc=-1.0;
            }
            return acc;
        }

        private double GetCurrentHP()
        {
            double hp = 0.0;
            try
            {
                hp = m_memory_finder.GetMemoryDouble(new List<int>() { -0x320, 0x124,0x384,0x3c,0x24,0x25c, 0x40, 0x1c});
            }
            catch (ThreadStackNoFoundException e)
            {
                hp = -1.0;
            }
            return hp;
        }

        private Beatmap GetCurrentBeatmap()
        {
            Beatmap beatmapinfo = new Beatmap();
            try
            {
                beatmapinfo.BeatmapID = m_memory_finder.GetMemoryInt(new List<Int32>() { -0x320, 0x0, 0x1ec, 0x230, 0x270, 0x1b8, 0xc0 });
            }
            catch (ThreadStackNoFoundException e)
            {
                beatmapinfo.BeatmapID = -1;
            }
            return beatmapinfo;
        }

        private BeatmapSet GetCurrentBeatmapSet()
        {
            //TODO:通过BeatmapFinder从数据库查找更详细的歌曲信息
            BeatmapSet beatmapsetset = new BeatmapSet();
            try
            {
                beatmapsetset.BeatmapSetID = m_memory_finder.GetMemoryInt(new List<Int32>() { -0x320, 0x0, 0x1ec, 0x230, 0x270, 0x1b8 ,0xc4});
            }
            catch (ThreadStackNoFoundException e)
            {
                beatmapsetset.BeatmapSetID = -1;
            }
            return beatmapsetset;
        }

        private ModsInfo GetCurrentMods()
        {
            //被OSU混淆，懒得找了
            ModsInfo mods = new ModsInfo();
            mods.Mod = ModsInfo.Mods.None;
            return mods;
        }

        private OsuStatus GetCurrentOsuStatus()
        {
            if (Process.GetProcessesByName("osu!").Count() == 0) return OsuStatus.NoFoundProcess;

            if (m_now_player_status.status == "Playing"||
                (Process.GetProcessesByName("osu!")[0].MainWindowTitle!="osu!"&& 
                 Process.GetProcessesByName("osu!")[0].MainWindowTitle!="")) return OsuStatus.Playing;

            if (m_now_player_status.status == "Editing"|| 
                (Process.GetProcessesByName("osu!")[0].MainWindowTitle != "osu!" && 
                 Process.GetProcessesByName("osu!")[0].MainWindowTitle.Contains(".osu")))return OsuStatus.Editing;
            return OsuStatus.Listening;
        }
    }
}
