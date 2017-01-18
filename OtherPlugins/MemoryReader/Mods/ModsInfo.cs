using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryReader.Mods
{
    //被OSU混淆，懒得找了
    public class ModsInfo
    {
        [Flags]
        public enum Mods
        {
            None = 0,
            NoFail = 1 << 0,
            Easy = 1 << 1,
            Hidden = 1 << 3,
            HardRock = 1 << 4,
            SuddenDeath = 1 << 5,
            DoubleTime = 1 << 6,
            Relax = 1 << 7,
            HalfTime = 1 << 8,
            Nightcore = 1 << 9,
            Flashlight = 1 << 10,
            Autoplay = 1 << 11,
            SpunOut = 1 << 12,
            Relax2 = 1 << 13,
            Perfect = 1 << 14,
            Key4 = 1 << 15,
            Key5 = 1 << 16,
            Key6 = 1 << 17,
            Key7 = 1 << 18,
            Key8 = 1 << 19,
            FadeIn = 1 << 20,
            Random = 1 << 21,
            Cinema = 1 << 22,
            Target = 1 << 23,
            Key9 = 1 << 24,
            KeyCoop = 1 << 25,
            Key1 = 1 << 26,
            Key3 = 1 << 27,
            Key2 = 1 << 28,
        }

        static Dictionary<Mods, string> ModsStr = new Dictionary<Mods, string>()
        {
            { Mods.None,"" },
            { Mods.NoFail,"NF"},
            { Mods.Easy,"EZ"},
            { Mods.Hidden,"HD"},
            { Mods.HardRock,"HR"},
            { Mods.SuddenDeath,"SD"},
            { Mods.DoubleTime,"DT"},
            { Mods.Nightcore,"NC"},
            { Mods.Relax,"RL"},
            { Mods.HalfTime,"HT"},
            { Mods.Flashlight,"FL"},
            { Mods.Autoplay,"Auto"},
            { Mods.SpunOut,"SO"},
            { Mods.Perfect,"PF"},
            //TODO:先做STD
        };

        public Mods Mod { set; get; }

        public override string ToString()
        {
            string ret="";
            UInt32 mask = 0x80000000;
            do
            {
                if (((UInt32)Mod & mask) == 1)
                    ret += ModsStr[(Mods)mask];
                mask >>= 1;
                ret += ",";
            } while (mask!=0);
            ret.Remove(ret.Length - 1);
            return ret;
        }
    }
}
