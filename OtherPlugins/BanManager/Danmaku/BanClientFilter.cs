using Sync.MessageFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanManagerPlugin.Ban
{
    class BanClientFilter : IFilter, ISourceDanmaku
    {
        BanManager bindManager = null;
        public void SetBanManager(BanManager manager)
        {
            this.bindManager = manager;
        }

        protected BanClientFilter() {}
        public BanClientFilter(BanManager refManager)
        {
            SetBanManager(refManager);
        }

        private BanInfo GetInfo()
        {
            return bindManager.GetFliterInfo();
        }

        public void onMsg(ref IMessageBase msg)
        {

            if (GetInfo().IsBanned(msg.User.RawText))
                msg.Cancel = true;
        }

        public void Dispose()
        {
            //nothing to do
        }
    }
}
