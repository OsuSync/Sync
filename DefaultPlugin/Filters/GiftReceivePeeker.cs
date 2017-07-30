using Sync.MessageFilter;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DefaultPlugin.Filters
{
    class GiftReceivePeeker : IFilter, ISourceGift
    {
        private Thread giftRecyler;
        private List<CBaseGift> historyGift;
        private bool isRunning = false;

        internal GiftReceivePeeker()
        {
            historyGift = new List<CBaseGift>();        }

        public void onMsg(ref MessageBase msg)
        {
            if(msg is GiftMessage)
            {
                msg.cancel = true;
                AddGift((msg as GiftMessage).source);
            }
        }


        internal void StartRecycler()
        {
            giftRecyler = new Thread(giftShowRecycle);
            giftRecyler.IsBackground = true;
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

            while (DefaultPlugin.MainInstance.Connector.IsConnect && isRunning)
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
                            long giftCount = p.GiftCount;
                            var g = curList.Where(cp => cp.SenderName == p.SenderName);
                            giftCount += g.Sum(cp => cp.GiftCount);
                            p.GiftCount = (uint)giftCount;

                        });
                        curList.Distinct(new GiftSenderEqv());
                        curList.Select(p => p.SenderName).ToList().ForEach(p => strUsers += p + ",");
                        curList.OrderBy(p => p.GiftCount);
                        mostUser = curList.Count == 0 ? null : curList.First();
                        DefaultPlugin.MainMessager.onIRC("", new StringElement(Sync.IRC.IRCClient.CONST_ACTION_FLAG, "3分钟内共" + curList.Count() + "个玩家发来礼物, 他们是" + strUsers));
                        DefaultPlugin.MainMessager.onIRC("", new StringElement(Sync.IRC.IRCClient.CONST_ACTION_FLAG, "送礼物最多的是" + mostUser.SenderName + "，共计" + mostUser.GiftCount + "个"));
                        time.Restart();
                        mostUser = null;
                    }
                    curList.Clear();
                }
                Thread.Sleep(1);
            }
            IO.CurrentIO.WriteColor("礼物统计线程成功结束", ConsoleColor.Cyan);
        }

    }


    internal class GiftSenderEqv : IEqualityComparer<CBaseGift>
    {
        public bool Equals(CBaseGift x, CBaseGift y)
        {
            return x.SenderName.Equals(y.SenderName);
        }

        public int GetHashCode(CBaseGift obj)
        {
            return obj.GetHashCode();
        }
    }
}
