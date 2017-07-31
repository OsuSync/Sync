using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.MessageFilter;
using Sync;

namespace RecentlyUserQuery.Danmaku
{
    class MessageRecorderFilter : IFilter, ISourceDanmaku
    {
        MessageRecorder recorder = null;

        public MessageRecorderFilter(MessageRecorder recoder)
        {
            this.recorder = recoder;
        }

        public void Dispose()
        {
            recorder.Clear();
        }

        //listening messages from Danmaku
        public void onMsg(ref IMessageBase msg)
        {
            if (msg.Cancel||recorder==null||msg.User.RawText.Length==0)
                return;
            recorder.Update(msg.User.RawText, msg.Message.RawText);
        }
    }
}
