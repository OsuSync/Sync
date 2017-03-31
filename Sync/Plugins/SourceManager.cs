using Sync.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Plugins
{
    public class SourceManager
    {
        List<ISourceBase> listSources;
        public SourceManager()
        {
            listSources = new List<ISourceBase>();
        }

        public IEnumerable<ISourceBase> SourceList
        {
            get
            {
                return listSources;
            }
        }

        public bool AddSource(ISourceBase src)
        {
            if(listSources.Exists(p => p.getSourceName() == src.getSourceName()))
            {
                return false;
            }
            else
            {
                listSources.Add(src);
            }
            return true;
        }
    }
}
