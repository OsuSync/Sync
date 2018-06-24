using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Plugins;
using Sync.Tools;
using static Sync.Tools.DefaultI18n;

namespace Sync.Source
{
    /// <summary>
    /// A wrapper for select source and check source senable
    /// </summary>
    public class SourceWorkWrapper
    {
        private SourceManager sources;
        public bool Sendable { get; } 

        public SourceWorkWrapper(SourceManager sources)
        {
            this.sources = sources;
            Source = sources.SourceList.Where(p => p.Name == DefaultConfiguration.Instance.Source).FirstOrDefault()??sources.SourceList.FirstOrDefault();/*没有的话就默认第一个*/
            if(Source == null)
            {
                IO.CurrentIO.WriteColor(LANG_NO_ANY_SOURCE, ConsoleColor.Red);
            }
            if (Source is SendableSource)
            {
                SendableSource = (SendableSource)Source;
                Sendable = true;
            }
        }

        public SourceBase Source { get; internal set; }
        public SendableSource SendableSource { get; internal set; }
    }
}
 