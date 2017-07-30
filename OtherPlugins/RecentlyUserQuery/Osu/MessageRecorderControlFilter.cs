using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync;
using Sync.MessageFilter;
using Sync.Tools;
using Sync.Source;
using Sync.Plugins;

namespace RecentlyUserQuery.Osu
{
    class MessageRecorderControlFilter : IFilter, ISourceOsu
    {
        MessageRecorder recorder = null;
        MessageDispatcher manager = null;

        public MessageRecorderControlFilter(MessageDispatcher manager,MessageRecorder recorder)
        {
            this.recorder = recorder;
            this.manager = manager;
        }

        const string recentlyCommand = "?recently";

        public void onMsg(ref MessageBase msg)
        {
            string message = msg.message.RawText;

            if (msg.message.RawText[0] != '?'|| recorder.IsRecording == false|| !message.StartsWith(recentlyCommand))
                return;

            string[] args = message.Split(' ');
            msg.cancel = true;
            if (args.Length > 1)
                SendResponseMessage(recorder.ProcessCommonCommand(args));
            else
                SendResponseMessage(EnumRecentUser().Result);
        }

        private async Task<string> EnumRecentUser()
        {
            var task = new Task<string>(() =>
            {
                Dictionary<string, int> result = new Dictionary<string, int>();
                foreach (var record in recorder.GetHistoryList())
                    result[record.userName] = (record.id);
                StringBuilder sb = new StringBuilder();
                foreach (var pair in result)
                    sb.AppendFormat("{0}->{1} || ", pair.Value, pair.Key);
                //SendResponseMessage(sb.ToString());
                return sb.ToString();
            });

            return await task;
        }

        private void SendResponseMessage(string message)
        {
            CBaseDanmuku danmaku = new CBaseDanmuku();
            danmaku.Danmuku = message;
            manager.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
        }

        public void Dispose()
        {
            //nothing to do
        }
    }
}
