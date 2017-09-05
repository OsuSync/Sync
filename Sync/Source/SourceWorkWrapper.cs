using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Plugins;
using Sync.Tools;

namespace Sync.Source
{
    public class SourceWorkWrapper
    {
        private SourceManager sources;
        public bool Sendable { get; } 

        public SourceWorkWrapper(SourceManager sources)
        {
            this.sources = sources;
            Source = sources.SourceList.Where(p => p.Name == Configuration.Source).FirstOrDefault();
            if(Source == null)
            {
                Source = sources.SourceList.First();
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
