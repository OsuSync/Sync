using System;

namespace DefaultPlugin.Sources.BiliBili.BiliBili_dm
{
    delegate void DisconnectEvt(object sender, DisconnectEvtArgs e);

    delegate void ReceivedDanmakuEvt(object sender, ReceivedDanmakuArgs e);

    delegate void ReceivedRoomCountEvt(object sender, ReceivedRoomCountArgs e);
    delegate void ConnectedEvt(object sender, ConnectedEvtArgs e);

    class ReceivedRoomCountArgs
    {
        public uint UserCount;
    }

    class DisconnectEvtArgs
    {
        public Exception Error;
    }

    class ReceivedDanmakuArgs
    {
        public DanmakuModel Danmaku;
    }
    abstract class ConnectedEvtArgs
    {
        public int roomid = 0;
    }
}