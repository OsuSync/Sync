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
        public void onMsg(ref MessageBase msg)
        {
            if (msg.cancel||recorder==null||msg.user.RawText.Length==0)
                return;
            recorder.Update(msg.user.RawText, msg.message.RawText);
        }
    }
}
