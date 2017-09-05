using Sync.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Plugins
{
    /// <summary>
    /// Source manager
    /// </summary>
    public class SourceManager
    {
        LinkedList<SourceBase> listSources;


        public SourceManager()
        {
            listSources = new LinkedList<SourceBase>();
        }

        public IEnumerable<SourceBase> SourceList
        {
            get
            {
                return listSources;
            }
        }

        public bool AddSource(SourceBase src)
        {
            if(listSources.Contains(src))
            {
                return false;
            }
            else
            {
                listSources.AddLast(src);
            }
            return true;
        }
    }
}
