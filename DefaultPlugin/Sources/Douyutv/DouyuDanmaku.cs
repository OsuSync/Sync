using Sync.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultPlugin.Sources.Douyutv
{
    public class DouyuDanmaku : IBaseDanmakuEvent
    {
        public DouyuDanmaku(string sender, string text)
        {
            this.Danmuku = text;
            this.SenderName = sender;
            this.SendTime = DateTime.Now.ToShortTimeString();
        }

        public string Danmuku { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }
    }

    class DouyuGift : IBaseGiftEvent
    {
        public DouyuGift(string sender, string giftName, string giftNum)
        {
            this.SenderName = sender;
            this.GiftName = giftName;
            this.GiftCount = int.Parse(giftNum);
            this.SendTime = DateTime.Now.ToShortTimeString();
        }

        public string SenderName { get; set; }
        public string GiftName { get; set; }
        public int GiftCount { get; set; }
        public string SendTime { get; set; }
    }
}
