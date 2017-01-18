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
            Sync.Tools.ConsoleWriter.Write(String.Format("当前Acc:{0}", acc));
        }

        public void OnCurrentBeatmapChange(Beatmap beatmap)
        {
            Sync.Tools.ConsoleWriter.Write(String.Format("当前Beatmap ID:{0}", beatmap.BeatmapID));
        }

        public void OnCurrentBeatmapSetChange(BeatmapSet beatmap)
        {
            Sync.Tools.ConsoleWriter.Write(String.Format("当前BeatmapSet ID:{0}", beatmap.BeatmapSetID));
        }

        public void OnCurrentModsChange(ModsInfo mod)
        {
            //throw new NotImplementedException();
        }

        public void OnHPChange(double hp)
        {
            Sync.Tools.ConsoleWriter.Write(String.Format("当前HP:{0}",hp));
        }
    }
}
