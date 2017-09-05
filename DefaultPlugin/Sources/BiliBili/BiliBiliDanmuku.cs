using DefaultPlugin.Sources.BiliBili.BiliBili_dm;
using Sync.Source;
using System;

namespace DefaultPlugin.Sources.BiliBili
{
    class BiliBiliDanmuku : IBaseDanmakuEvent
    {
        public BiliBiliDanmuku(DanmakuModel instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            this.Danmuku = instance.CommentText;
            this.SenderName = instance.CommentUser;
            this.SendTime = DateTime.Now.ToShortTimeString();
        }

        public string Danmuku { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }
    }

    class BiliBiliGift : IBaseGiftEvent
    {
        public BiliBiliGift(DanmakuModel instance)
        {
            this.GiftCount = int.Parse(instance.GiftNum);
            this.GiftName = instance.GiftName;
            this.SenderName = instance.GiftUser;
            this.SendTime = DateTime.Now.ToShortTimeString();

        }

        public int GiftCount { get; set; }
        public string GiftName { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }
    }
}
