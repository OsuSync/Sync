using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Plugins;
using Sync.Tools;

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
            Source = sources.SourceList.Where(p => p.Name == Configuration.Source).FirstOrDefault()??/*没有的话就默认第一个*/sources.SourceList.FirstOrDefault();
            if(Source == null)
            {
                IO.CurrentIO.WriteColor("没有任何弹幕接收源,请检查Plugins目录或使用\"plugins install DefaultPlugin\"来安装默认插件",ConsoleColor.Red);
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
 