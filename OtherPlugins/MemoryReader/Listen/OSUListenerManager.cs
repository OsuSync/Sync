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
            Closing,
            Listening,
            Playing,
            Editing
        }

        private MemoryFinder m_memory_finder;

        private OsuStatus m_last_osu_status = OsuStatus.NoFoundProcess;
        private IOSUStatus m_now_player_status = new OSUStatus();
        private bool m_stop = false;
        private Thread m_listen_thread;
        private List<IOSUListener> m_listeners = new List<IOSUListener>();

        private BeatmapSet m_last_beatmapset = new BeatmapSet();
        private Beatmap m_last_beatmap = new Beatmap();
        private ModsInfo m_last_mods = new ModsInfo();

        private double m_last_hp = 0;
        private double m_last_acc = 0;
        private int m_last_combo = 0;

        public OSUListenerManager(SyncHost host)
        {
            foreach (var t in host.EnumPluings())
            {
                if (t.getName() == "Now Playing")
                {
                    ((NowPlaying.NowPlaying)t).registerCallback(p =>
                    {
                        return new System.Threading.Tasks.Task<bool>(status =>
                        {
                            m_now_player_status = (NowPlaying.IOSUStatus)status;
                            return true;
                        }, p);
                    });
                    break;
                }
            }

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
            m_stop = true;
        }

        private void LoadMemoryFinder(Process osu)
        {
            m_memory_finder = new MemoryFinder(osu);
        }

        private void ListenLoop()
        {
            UInt32 count = 0;

            if (GetCurrentOsuStatus() != OsuStatus.NoFoundProcess)
                LoadMemoryFinder(Process.GetProcessesByName("osu!")[0]);

            while (!m_stop)
            {
                OsuStatus status = GetCurrentOsuStatus(); ;
                //last status
                if (m_last_osu_status == OsuStatus.NoFoundProcess && m_last_osu_status != status)
                {
                    LoadMemoryFinder(Process.GetProcessesByName("osu!")[0]);
                }
                m_last_osu_status = status;

                if (m_last_osu_status != OsuStatus.NoFoundProcess && m_last_osu_status != OsuStatus.Closing)
                {
                    foreach (var listner in m_listeners)
                    {
                        BeatmapSet beatmapset = GetCurrentBeatmapSet();
                        if (beatmapset.BeatmapSetID != m_last_beatmapset.BeatmapSetID)
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

                        if (m_last_osu_status == OsuStatus.Playing)
                        {
                            ModsInfo mods = GetCurrentMods();
                            if (mods.Mod != m_last_mods.Mod)
                            {
                                listner.OnCurrentModsChange(mods);
                            }
                            m_last_mods = mods;

                            double hp = GetCurrentHP();
                            if (hp != m_last_hp)
                            {
                                listner.OnHPChange(hp);
                            }

                            m_last_hp = hp;

                            double acc = GetCurrentAcc();
                            if (acc != m_last_acc)
                            {
                                listner.OnAccuracyChange(acc);
                            }
                            m_last_acc = acc;

                            int cb = GetCurrentCombo();
                            if (cb != m_last_combo)
                            {
                                listner.OnComboChange(cb);
                            }
                            m_last_combo = cb;
                        }
                        else
                        {
                            m_last_acc = 0;
                            m_last_hp = 0;
                        }
                    }
                }
                else
                {
                    if (m_last_osu_status == OsuStatus.NoFoundProcess)
                    {
                        if (count % (Setting.NoFoundOSUHintInterval * Setting.ListenInterval) == 0)
                        {
                            Sync.Tools.ConsoleWriter.WriteColor("没有发现 OSU! 进程，请打开OSU！", ConsoleColor.Red);
                            count = 0;
                        }
                        count++;
                    }
                }

                Thread.Sleep(Setting.ListenInterval);
            }
        }

        private double GetCurrentAcc()
        {
            double acc = 0.0;
            try
            {
                acc = m_memory_finder.GetMemoryDouble(new List<int>() { -0x320, 0x124, 0x384, 0x3c, 0x24, 0x25c, 0x48, 0x14 });
            }
            catch (ThreadStackNoFoundException e)
            {
                acc = -1.0;
            }
            return acc;
        }

        private double GetCurrentHP()
        {
            double hp = 0.0;
            try
            {
                hp = m_memory_finder.GetMemoryDouble(new List<int>() { -0x320, 0x124, 0x384, 0x3c, 0x24, 0x25c, 0x40, 0x1c });
            }
            catch (ThreadStackNoFoundException e)
            {
                hp = -1.0;
            }
            return hp;
        }

        private int GetCurrentCombo()
        {
            int cb = 0;
            try
            {
                cb = m_memory_finder.GetMemoryInt(new List<int>() { -0x320, 0x124, 0x384, 0x3c, 0x24, 0x25c, 0x34, 0x18 });
            }
            catch (ThreadStackNoFoundException e)
            {
                cb = -1;
            }
            return cb;
        }

        private Beatmap GetCurrentBeatmap()
        {
            Beatmap beatmapinfo = new Beatmap();
            try
            {
                beatmapinfo.BeatmapID = m_memory_finder.GetMemoryInt(new List<Int32>() { -0x320, 0x110, 0x68, 0x620, 0x6b4, 0x744, 0xc0 });
            }
            catch (ThreadStackNoFoundException e)
            {
                beatmapinfo.BeatmapID = -1;
            }
            return beatmapinfo;
        }

        private ModsInfo GetCurrentMods()
        {
            ModsInfo mods = new ModsInfo();
            try
            {
                int salt = m_memory_finder.GetMemoryInt(new List<Int32>() { -0x320, 0x124, 0x384, 0x3c, 0x24, 0x25c, 0x38, 0x1c, 0x8 });
                int mod = m_memory_finder.GetMemoryInt(new List<Int32>() { -0x320, 0x124, 0x384, 0x3c, 0x24, 0x25c, 0x38, 0x1c, 0xc });//混淆后的mods
                mods.Mod = (ModsInfo.Mods)(mod ^ salt);
            }
            catch (ThreadStackNoFoundException e)
            {
                //mods;
            }
            return mods;
        }

        private BeatmapSet GetCurrentBeatmapSet()
        {
            //TODO:通过BeatmapFinder从数据库查找更详细的歌曲信息
            BeatmapSet beatmapsetset = new BeatmapSet();
            try
            {
                beatmapsetset.BeatmapSetID = m_memory_finder.GetMemoryInt(new List<Int32>() { -0x320, 0x110, 0x68, 0x620, 0x6b4, 0x744, 0xc4 });
            }
            catch (ThreadStackNoFoundException e)
            {
                beatmapsetset.BeatmapSetID = -1;
            }
            return beatmapsetset;
        }

        private OsuStatus GetCurrentOsuStatus()
        {
            if (Process.GetProcessesByName("osu!").Count() == 0) return OsuStatus.NoFoundProcess;
            string osu_title = Process.GetProcessesByName("osu!")[0].MainWindowTitle;

            if (m_now_player_status.status == null) return OsuStatus.Closing;

            if (m_now_player_status.status == "Editing" || (osu_title != "osu!" && osu_title.Contains(".osu"))) return OsuStatus.Editing;

            if (m_now_player_status.status == "Playing" || (osu_title != "osu!" && osu_title != "")) return OsuStatus.Playing;

            return OsuStatus.Listening;
        }
    }
}
