using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryReader.BeatmapInfo;
using System.IO;
using Sync.Tools;

namespace MemoryReader
{
    static class Setting
    {
        public static bool IsFirstRun=true;
        public static string OSU_Path;
        public static Dictionary<int,Beatmap> Beatmaps=new Dictionary<int,Beatmap>();

        //TODO:
        //-----Header-----
        //IsFirstRun
        //OSU_path
        //---Beatmaps---


        static void SaveSetting()
        {
            FileStream fs=File.Open("MemoryScanner.db", FileMode.OpenOrCreate);
            
        }

        static void LoadSetting()
        {
            FileStream fs = File.Open("MemoryScanner.db", FileMode.Open);
        }
    }
}
