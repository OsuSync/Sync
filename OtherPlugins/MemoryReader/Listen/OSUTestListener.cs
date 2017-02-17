using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryReader.BeatmapInfo;
using MemoryReader.Listen.InterFace;
using MemoryReader.Mods;

namespace MemoryReader.Listen
{
    class OSUTestListener : IOSUListener
    {
        public void OnAccuracyChange(double acc)
        {
            Sync.Tools.IO.CurrentIO.Write(String.Format("当前Acc:{0}", acc));
        }

        public void OnComboChange(int combo)
        {
            Sync.Tools.IO.CurrentIO.Write(String.Format("当前Combo:{0}", combo));
        }

        public void OnCurrentBeatmapChange(Beatmap beatmap)
        {
            Sync.Tools.IO.CurrentIO.Write(String.Format("当前Beatmap ID:{0}", beatmap.BeatmapID));
        }

        public void OnCurrentBeatmapSetChange(BeatmapSet beatmap)
        {
            Sync.Tools.IO.CurrentIO.Write(String.Format("当前BeatmapSet ID:{0}", beatmap.BeatmapSetID));
        }

        public void OnCurrentModsChange(ModsInfo mod)
        {
            Sync.Tools.IO.CurrentIO.Write(String.Format("当前Mods:{0}", mod.ShortName));
        }

        public void OnHPChange(double hp)
        {
            Sync.Tools.IO.CurrentIO.Write(String.Format("当前HP:{0}",hp));
        }
    }
}
