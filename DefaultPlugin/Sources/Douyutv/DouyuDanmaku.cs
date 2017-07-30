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
            this.Danmuku = text;
            this.SenderName = sender;
            this.Source = this;
            this.SendTime = DateTime.Now.ToShortTimeString();
        }
    }

    class DouyuGift : CBaseGift
    {
        public DouyuGift(string sender, string giftName, string giftNum)
        {
            this.SenderName = sender;
            this.GiftName = giftName;
            this.GiftCount = uint.Parse(giftNum);
        }
    }
}
