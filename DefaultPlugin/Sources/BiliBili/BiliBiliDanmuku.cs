using Sync.Source;
using Sync.Source.BiliBili.BiliBili_dm;
using System;

namespace DefaultPlugin.Source
{
    class BiliBiliDanmuku : CBaseDanmuku
    {

        public BiliBiliDanmuku(DanmakuModel instance)
        {
            this.danmuku = instance.CommentText;
            this.senderName = instance.CommentUser;
            this.sendTime = DateTime.Now.ToShortTimeString();
            this.source = instance;
        }

    }

    class BiliBiliGift : CBaseGift
    {
        public BiliBiliGift(DanmakuModel instance)
        {
            this.giftCount = uint.Parse(instance.GiftNum);
            this.giftName = instance.GiftName;
            this.senderName = instance.GiftUser;
            this.sendTime = DateTime.Now.ToShortTimeString();
            this.source = instance;

        }
    }
}
