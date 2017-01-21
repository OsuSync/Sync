using MemoryReader.BeatmapInfo;
using MemoryReader.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryReader.Listen.InterFace
{ 
    public interface IOSUListener
    {
        void OnCurrentBeatmapSetChange(BeatmapSet beatmap);
        void OnCurrentBeatmapChange(Beatmap beatmap);
        void OnComboChange(int combo);
        void OnCurrentModsChange(ModsInfo mod);
        void OnHPChange(double hp);
        void OnAccuracyChange(double acc);
    }
}
