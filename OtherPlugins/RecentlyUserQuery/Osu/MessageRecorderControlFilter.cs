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
    class MessageRecorderControlFilter : FilterBase, IOsu
    {
        MessageRecorder recorder = null;
        FilterManager manager = null;

        public MessageRecorderControlFilter(FilterManager manager,MessageRecorder recorder)
        {
            this.recorder = recorder;
            this.manager = manager;
        }

        const string recentlyCommand = "?recently";

        public override void onMsg(ref MessageBase msg)
        {
            string message = msg.message.RawText;

            if (msg.message.RawText[0] != '?'|| recorder.IsRecording == false|| !message.StartsWith(recentlyCommand))
                return;

            string[] args = message.Split(' ');
            msg.cancel = true;
            if (args.Length > 1)
                ProcessCommand(args);
            else
                EnumRecentUser();
        }

        private async void EnumRecentUser()
        {
            var task=new Task<int>(() =>
            {
                Dictionary<string, int> result = new Dictionary<string, int>();
                foreach (var record in recorder.GetHistoryList())
                    result[record.userName] = (record.id);
                StringBuilder sb = new StringBuilder();
                foreach (var pair in result)
                    sb.AppendFormat("{0}->{1} || ", pair.Value, pair.Key);
                SendResponseMessage(sb.ToString());
                return 0;
            });

            task.Start();
        }

        private void ProcessCommand(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
                args[i] = args[i].Trim();
            int value = 0;
            switch (args[1])
            {
                case "--status":
                    SendResponseMessage(string.Format("MessageRecord status: {0} | recordCount/Capacity : {1}/{2}",recorder.IsRecording?"running":"stopped",recorder.GetHistoryList().Count,recorder.Capacity));
                    break;

                case "--disable":
                    recorder.IsRecording = false;
                    recorder.Clear();
                    UserIdGenerator.Clear();
                    SendResponseMessage("消息记录器已禁用，数据已清除");
                    break;

                case "--start":
                    recorder.IsRecording = true;
                    SendResponseMessage("消息记录器开启");
                    break;

                case "--realloc":
                    if (args.Length < 3)
                        SendResponseMessage("MessageRecord: 错误的指令");
                    else {
                        value = Convert.ToInt32(args[2]);
                        recorder.Capacity = value;
                        SendResponseMessage("消息记录器现在可记录"+recorder.Capacity+"条历史记录");
                         }
                    break;

                case "--i": //鸽一会
                    SendResponseMessage("咕咕咕~");
                    break;

                case "--u": //鸽一会
                    SendResponseMessage("咕咕咕~");
                    break;

                default:
                    break;
            }
        }

        private void SendResponseMessage(string message)
        {
            CBaseDanmuku danmaku = new CBaseDanmuku();
            danmaku.danmuku = message;
            manager.RaiseMessage(typeof(IDanmaku),new DanmakuMessage(danmaku));
        }
    }
}
