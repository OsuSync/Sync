using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync;
using Sync.Command;
using MemoryReader.Listen;
using MemoryReader.BeatmapInfo;
using MemoryReader.Listen.InterFace;
using System.IO;

namespace MemoryReader
{
    public class MemoryReader: Plugin
    {
        public const string PLUGIN_NAME = "MemoryReader";
        public const string PLUGIN_AUTHOR = "KedamaOvO";
        private OSUListenerManager m_osu_listener;

        public MemoryReader():base(PLUGIN_NAME,PLUGIN_AUTHOR)
        {
            base.onInitPlugin += OnInitPlugin;
            base.onLoadComplete += OnLoadComplete;
        }

        public void RegisterOSUListener(IOSUListener listener)
        {
            m_osu_listener.AddListener(listener);
        }

        public void UnregisterOSUListener(IOSUListener listener)
        {
            m_osu_listener.RemoveListener(listener);
        }

        private void OnInitPlugin()
        {
            Sync.Tools.ConsoleWriter.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);
        }

        private void OnLoadComplete(SyncHost host)
        {
            if(!File.Exists(@"..\MemoryRenderSetting.json"))
            {
                Setting.SaveSetting();
            }

            Setting.LoadSetting();

            try
            {
                m_osu_listener = new OSUListenerManager(host);
            }
            catch (Exception e)
            {
                Sync.Tools.ConsoleWriter.WriteColor(e.Message, ConsoleColor.Red);
                Sync.Tools.ConsoleWriter.WriteColor(e.StackTrace, ConsoleColor.Red);
            }
#if DEBUG
            //m_osu_listener.AddListener(new OSUTestListener());
#endif
            m_osu_listener.Start();
        }
    }
}
