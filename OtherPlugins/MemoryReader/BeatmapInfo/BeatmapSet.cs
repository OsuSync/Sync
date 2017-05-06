using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MemoryReader.DefaultLanguage;

namespace MemoryReader.BeatmapInfo
{
    public class BeatmapSet
    {
        public int BeatmapSetID { get; set; }
        public string DownloadLink
        {
            get
            {
                if (BeatmapSetID != 0) return @"http://osu.ppy.sh/s/" + BeatmapSetID;
                return LANG_NOT_FOUND;
            }
        }
    }
}
