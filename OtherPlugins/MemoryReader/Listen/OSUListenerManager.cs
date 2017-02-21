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
using osu_database_reader;

namespace MemoryReader.Listen
{
    class OSUListenerManager
    {
        [Flags]
        public enum OsuStatus
        {
            NoFoundProcess,
            Unkonw,
            Listening,
            Playing,
            Editing
        }

        private MemoryFinder m_memory_finder;
        private MemoryScanner m_memory_scanner;
        private OsuDb m_osu_db;

        private IntPtr m_beatmap_id_address;

        private OsuStatus m_last_osu_status = OsuStatus.Unkonw;
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

        private List<BeatmapEntry> current_map_tmp = new List<BeatmapEntry>();

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

        private void LoadMemorySearch(Process osu)
        {
            m_memory_finder = new MemoryFinder(osu);
            m_memory_scanner = new MemoryScanner(osu)
            {
                BeginAddress = 0x3004A8C,
                EndAddress = 0x8004A8C,
                InterVal = 0x0010000,
                BufferSize = 4,
                AddressFilter = (address,target) =>
                {
                    byte[] buf = new byte[4];
                    IntPtr t;
                    Win32API.ReadProcessMemory(osu.Handle, (IntPtr)(target+0xc4), buf, 4, out t);
                    Int32 id = BitConverter.ToInt32(buf, 0);
                    foreach (var map in current_map_tmp)
                    {
                        if (map.BeatmapSetId == id)
                            return true;
                    }
                    return false;
                }
            };

            if (m_osu_db == null)
            {
                m_osu_db = OsuDb.Read(Setting.OsuPath+ @"\osu!.db");
            }
        }

        private void ListenLoop()
        {
            UInt32 count = 0;

            if (GetCurrentOsuStatus() != OsuStatus.NoFoundProcess)
                LoadMemorySearch(Process.GetProcessesByName("osu!")[0]);

            while (!m_stop)
            {
                OsuStatus status = GetCurrentOsuStatus();

                //last status
                if (m_last_osu_status == OsuStatus.NoFoundProcess && m_last_osu_status != status)
                {
                    LoadMemorySearch(Process.GetProcessesByName("osu!")[0]);
                }

                if(m_now_player_status.title!=null&& m_now_player_status.title!=""&& m_last_osu_status!=status)
                {

                    if (m_last_osu_status != OsuStatus.Playing&& m_last_osu_status!=OsuStatus.Listening)
                    {
                        foreach (var map in m_osu_db.Beatmaps)
                        {
                            if (map.Title == m_now_player_status.title && map.Artist == m_now_player_status.artist)
                            {
                                current_map_tmp.Add(map);
                            }
                        }

                        m_beatmap_id_address = (IntPtr)(m_memory_scanner.Scan()[0]);
                    }
                }

                m_last_osu_status = status;

                if (m_last_osu_status != OsuStatus.NoFoundProcess && m_last_osu_status != OsuStatus.Unkonw)
                {
                    BeatmapSet beatmapset = GetCurrentBeatmapSet();
                    Beatmap beatmap = GetCurrentBeatmap();
                    ModsInfo mods = GetCurrentMods();
                    double hp = GetCurrentHP();
                    double acc = GetCurrentAcc();
                    int cb = GetCurrentCombo();

                    foreach (var listner in m_listeners)
                    {

                        if (beatmapset.BeatmapSetID != m_last_beatmapset.BeatmapSetID)
                        {
                            listner.OnCurrentBeatmapSetChange(beatmapset);
                        }

                        
                        if (beatmap.BeatmapID != m_last_beatmap.BeatmapID)
                        {
                            listner.OnCurrentBeatmapChange(beatmap);
                        }

                        if (m_last_osu_status == OsuStatus.Playing)
                        {
                            if (mods.Mod != m_last_mods.Mod)
                            {
                                listner.OnCurrentModsChange(mods);
                            }

                            if (hp != m_last_hp)
                            {
                                listner.OnHPChange(hp);
                            }


                            if (acc != m_last_acc)
                            {
                                listner.OnAccuracyChange(acc);
                            }

                            if (cb != m_last_combo)
                            {
                                listner.OnComboChange(cb);
                            }
                        }
                        else
                        {
                            m_last_acc = 0;
                            m_last_hp = 0;
                        }
                    }

                    m_last_beatmapset = beatmapset;
                    m_last_beatmap = beatmap;
                    m_last_mods = mods;
                    m_last_hp = hp;
                    m_last_acc = acc;
                    m_last_combo = cb;

                }
                else
                {
                    if (m_last_osu_status == OsuStatus.NoFoundProcess)
                    {
                        if (count % (Setting.NoFoundOSUHintInterval * Setting.ListenInterval) == 0)
                        {
                            Sync.Tools.IO.CurrentIO.WriteColor("没有发现 OSU! 进程，请打开OSU！", ConsoleColor.Red);
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
            catch (ThreadStackNoFoundException)
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
            catch (ThreadStackNoFoundException)
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
            catch (ThreadStackNoFoundException)
            {
                cb = -1;
            }
            return cb;
        }

        private Beatmap GetCurrentBeatmap()
        {
            Beatmap beatmapinfo = new Beatmap();
            beatmapinfo.BeatmapID = m_memory_finder.GetMemoryInt(new List<Int32>() { (Int32)m_beatmap_id_address, 0xc0 },false);
            return beatmapinfo;
        }

        private BeatmapSet GetCurrentBeatmapSet()
        {
            BeatmapSet beatmapsetset = new BeatmapSet();
            beatmapsetset.BeatmapSetID = m_memory_finder.GetMemoryInt(new List<Int32>() { (Int32)m_beatmap_id_address, 0xc4 }, false);
            return beatmapsetset;
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
            catch (ThreadStackNoFoundException)
            {
                //mods;
            }
            return mods;
        }

        private OsuStatus GetCurrentOsuStatus()
        {
            if (Process.GetProcessesByName("osu!").Count() == 0) return OsuStatus.NoFoundProcess;
            string osu_title = Process.GetProcessesByName("osu!")[0].MainWindowTitle;

            if (m_now_player_status.status == null) return OsuStatus.Unkonw;

            if (m_now_player_status.status == "Editing" || (osu_title != "osu!" && osu_title.Contains(".osu"))) return OsuStatus.Editing;

            if (m_now_player_status.status == "Playing" || (osu_title != "osu!" && osu_title != "")) return OsuStatus.Playing;

            return OsuStatus.Listening;
        }
    }
}
