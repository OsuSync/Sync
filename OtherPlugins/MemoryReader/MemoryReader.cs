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

        private void OnInitPlugin()
        {
            Sync.Tools.ConsoleWriter.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);
        }

        private void OnLoadComplete(SyncHost host)
        {
            try
            {
                m_osu_listener = new OSUListenerManager(host);
            }
            catch (Exception e)
            {
                Sync.Tools.ConsoleWriter.WriteColor(e.Message, ConsoleColor.Red);
                Sync.Tools.ConsoleWriter.WriteColor(e.StackTrace, ConsoleColor.Red);
            }

            m_osu_listener.AddListener(new OSUListener());
            m_osu_listener.Start();
        }
    }
}
