using Sync.Tools;


namespace MemoryReader
{
    class SettingIni:IConfigurable
    {
        public ConfigurationElement ListenInterval { set; get; }
        public ConfigurationElement NoFoundOsuHintInterval { set; get; }
        public ConfigurationElement OsuPath { set; get; }

        public void onConfigurationLoad(){}

        public void onConfigurationSave(){}
    }


    static class Setting
    {
        public static int ListenInterval=33;//ms
        public static int NoFoundOSUHintInterval = 120;//s
        public static string OsuPath="";

        private static SettingIni setting_output =new SettingIni();
        private static PluginConfiuration plugin_config = null;
        public static MemoryReader PluginInstance
        {
            set
            {
                plugin_config = new PluginConfiuration(value, setting_output);
            }
        }

        public static void SaveSetting()
        {
            setting_output.ListenInterval = ListenInterval.ToString();
            setting_output.NoFoundOsuHintInterval = NoFoundOSUHintInterval.ToString();
            setting_output.OsuPath = OsuPath;
            plugin_config.ForceSave();
        }

        public static void LoadSetting()
        {
            plugin_config.ForceLoad();
            if ((setting_output.NoFoundOsuHintInterval == null && setting_output.ListenInterval == null)||
                (setting_output.NoFoundOsuHintInterval == "" && setting_output.ListenInterval == ""))
            {
                SaveSetting();
            }
            else
            {
                ListenInterval = int.Parse(setting_output.ListenInterval);
                NoFoundOSUHintInterval = int.Parse(setting_output.NoFoundOsuHintInterval);
                OsuPath = setting_output.OsuPath;
            }
        }
    }
}
