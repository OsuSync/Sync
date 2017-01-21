using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryReader.BeatmapInfo;
using System.IO;
using Sync.Tools;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace MemoryReader
{
    [DataContract]
    class SettingJson
    {
        [DataMember(Order = 0)]
        public int listen_interval;
        [DataMember(Order = 1)]
        public int no_found_osu_hint_interval;
    }


    static class Setting
    {
        public static int ListenInterval=33;//ms
        public static int NoFoundOSUHintInterval = 120;//s

        public static void SaveSetting()
        {
            FileStream fs=File.Open(@"..\MemoryRenderSetting.json", FileMode.OpenOrCreate);
<<<<<<< HEAD
            SettingJson json = new SettingJson()
            {
                listen_interval = ListenInterval,
                no_found_osu_hint_interval = NoFoundOSUHintInterval
            };
=======
            SettingJson json = new SettingJson()
            {
                listen_interval = ListenInterval,
                no_found_osu_hint_interval = NoFoundOSUHintInterval
            };
>>>>>>> dcf85a747c52fdb14d51bc8a3ce81cb87a552fde
            new DataContractJsonSerializer(typeof(SettingJson)).WriteObject(fs, json);
            fs.Close();
        }

        public static void LoadSetting()
        {
            FileStream fs = File.Open(@"..\MemoryRenderSetting.json", FileMode.Open);
<<<<<<< HEAD
            SettingJson json = new SettingJson();
            json=(SettingJson)new DataContractJsonSerializer(typeof(SettingJson)).ReadObject(fs);
            ListenInterval = json.listen_interval;
            NoFoundOSUHintInterval = json.no_found_osu_hint_interval;
=======
            SettingJson json = new SettingJson();
            json=(SettingJson)new DataContractJsonSerializer(typeof(SettingJson)).ReadObject(fs);
            ListenInterval = json.listen_interval;
            NoFoundOSUHintInterval = json.no_found_osu_hint_interval;
>>>>>>> dcf85a747c52fdb14d51bc8a3ce81cb87a552fde
            fs.Close();
        }
    }
}
