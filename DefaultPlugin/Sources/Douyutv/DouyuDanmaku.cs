using Sync.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultPlugin.Sources.Douyutv
{
    class DouyuDanmaku : CBaseDanmuku
    {
        public DouyuDanmaku(string sender, string text)
        {
            this.danmuku = text;
            this.senderName = sender;
            this.source = this;
            this.sendTime = DateTime.Now.ToShortTimeString();
        }
    }

    class DouyuGift : CBaseGift
    {
        public DouyuGift(string sender, string giftName, string giftNum)
        {
            this.senderName = sender;
            this.giftName = giftName;
            this.giftCount = uint.Parse(giftNum);
        }
    }
}
