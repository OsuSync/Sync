using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.IRC.MessageFilter.Filters
{
    class DefaultFormat : FilterBase, IDanmaku, IOsu
    {
        public void onMsg(ref MessageBase msg)
        {
            msg.user.setPerfix("<");
            msg.user.setSuffix(">");
        }
    }
}
