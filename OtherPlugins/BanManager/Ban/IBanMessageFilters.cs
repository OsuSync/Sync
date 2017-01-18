using Sync.MessageFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanManagerPlugin.Ban
{
    class IBanMessageFilters : IFilter
    {
        protected BanManager bindManager = null;

        protected IBanMessageFilters() { }

        public IBanMessageFilters(BanManager manager)
        {
            SetBanManager(manager);
        }

        /// <summary>
        /// 钦定过滤器绑定的管理器
        /// </summary>
        /// <param name="manager"></param>
        public void SetBanManager(BanManager manager)
        {
            bindManager = manager;
        }

        public void onMsg(ref MessageBase msg)
        {
            throw new NotImplementedException();
        }
    }
}
