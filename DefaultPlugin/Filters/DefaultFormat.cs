using Sync.MessageFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultPlugin.Filters
{
    class DefaultFormat : IFilter, ISourceDanmaku, ISourceClient
    {
        public void onMsg(ref IMessageBase msg)
        {
            msg.User = new Sync.Tools.StringElement("<", msg.User.RawText, ">: ");
        }
    }
}
