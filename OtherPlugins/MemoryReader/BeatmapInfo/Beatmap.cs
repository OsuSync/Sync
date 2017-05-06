﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MemoryReader.DefaultLanguage;

namespace MemoryReader.BeatmapInfo
{
    public class Beatmap
    {
        public int BeatmapID { get; set; }

        public string DownloadLink {
            get
            {
                if (BeatmapID != 0) return @"http://osu.ppy.sh/b/" + BeatmapID;
                return LANG_NOT_FOUND;
            }
        }
    }
}
