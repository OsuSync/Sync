using Sync.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sync.Tools
{
    class GiftRecycler
    {
        private Thread giftRecyler;
        private List<CBaseGift> historyGift;
        private bool isRunning = false;

        public GiftRecycler()
        {
            giftRecyler = new Thread(giftShowRecycle);
            historyGift = new List<CBaseGift>();
        }

        public void StartRecycler()
        {
            giftRecyler.Start();
            isRunning = true;
        }

        public void AddGift(CBaseGift gift)
        {
            historyGift.Add(gift);
        }

        private void giftShowRecycle()
        {
            System.Diagnostics.Stopwatch time = new System.Diagnostics.Stopwatch();
            time.Start();

            while (Program.syncInstance.IsConnect && isRunning)
            {
                if (time.ElapsedMilliseconds / 1000 > 180)
                {
                    List<CBaseGift> curList;
                    lock (historyGift)
                    {
                        curList = historyGift.ToList();
                        historyGift.Clear();
                    }

                    if (curList.Count > 0)
                    {
                        string strUsers = string.Empty;
                        CBaseGift mostUser;
                        curList.ForEach(p =>
                        {
                            long giftCount = p.giftCount;
                            var g = curList.Where(cp => cp.senderName == p.senderName);
                            giftCount += g.Sum(cp => cp.giftCount);
                            p.giftCount = (uint)giftCount;

                        });
                        curList.Distinct(new GiftSenderEqv());
                        curList.Select(p => p.senderName).ToList().ForEach(p => strUsers += p + ",");
                        curList.OrderBy(p => p.giftCount);
                        mostUser = curList.Count == 0 ? null : curList.First();
                        //Program.syncInstance.IRCSendAction();
                        //Program.syncInstance.IRCSendAction("送礼物最多的是" + mostUser.senderName + "，共计" + mostUser.giftCount + "个");
                        Program.syncInstance.GetMessageFilter().onIRC("", new StringElement(IRC.IRCClient.STATIC_ACTION_FLAG,  "3分钟内共" + curList.Count() + "个玩家发来礼物, 他们是" + strUsers));
                        Program.syncInstance.GetMessageFilter().onIRC("", new StringElement(IRC.IRCClient.STATIC_ACTION_FLAG, "送礼物最多的是" + mostUser.senderName + "，共计" + mostUser.giftCount + "个"));
                        time.Restart();
                        mostUser = null;
                    }
                    curList.Clear();
                    
                }
            }

            ConsoleWriter.WriteColor("礼物统计线程成功结束", ConsoleColor.Cyan);
        }
    }

    internal class GiftSenderEqv : IEqualityComparer<CBaseGift>
    {
        public bool Equals(CBaseGift x, CBaseGift y)
        {
            return x.senderName.Equals(y.senderName);
        }

        public int GetHashCode(CBaseGift obj)
        {
            return obj.GetHashCode();
        }
    }
}
