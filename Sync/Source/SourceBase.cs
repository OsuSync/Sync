using System;

namespace Sync.Source
{
    /// <summary>
    /// 礼物基类
    /// </summary>
    public class CBaseGift
    {
        public string giftName { get; set; }
        public uint giftCount { get; set; }
        public string senderName { get; set; }
        public string sendTime { get; set; }
        public object source { get; set; }
    }

    /// <summary>
    /// 弹幕基类
    /// </summary>
    public class CBaseDanmuku
    {
        public string danmuku { get; set; }
        public string senderName { get; set; }
        public string sendTime { get; set; }
        public object source { get; set; }
    }

    /// <summary>
    /// 在源服务器连接成功时的委托
    /// </summary>
    public delegate void ConnectedEvt();
    /// <summary>
    /// 在源服务器断开连接时的委托
    /// </summary>
    public delegate void DisconnectedEvt();
    /// <summary>
    /// 接收到礼物信息的委托
    /// </summary>
    /// <param name="gift">礼物</param>
    public delegate void GiftEvt(CBaseGift gift);
    /// <summary>
    /// 收到弹幕信息时的委托
    /// </summary>
    /// <param name="danmuku">弹幕</param>
    public delegate void DanmukuEvt(CBaseDanmuku danmuku);
    /// <summary>
    /// 在源房间观看人数发生变化时的委托
    /// </summary>
    /// <param name="lCount">房间人数</param>
    public delegate void CurrentOnlineEvt(uint lCount);

    /// <summary>
    /// 弹幕源接口
    /// 实现接口即可用于连接。
    /// </summary>
    public interface ISourceBase
    {
        event ConnectedEvt onConnected;
        event DisconnectedEvt onDisconnected;
        event DanmukuEvt onDanmuku;
        event GiftEvt onGift;
        event CurrentOnlineEvt onOnlineChange;

        string getSourceName();
        string getSourceAuthor();

        /// <summary>
        /// 获得原始类型
        /// </summary>
        /// <returns>原始类型</returns>
        Type getSourceType();
        /// <summary>
        /// 开始源服务器连接
        /// </summary>
        /// <param name="room">指定的房间名字，既可以是twitch的channelName也可以是bilibili的id</param>
        /// <returns>true为连接成功, false为连接失败</returns>
        bool Connect(string roomName);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns>true为确认断开, false为断开失败</returns>
        bool Disconnect();

        /// <summary>
        /// 连接状态
        /// </summary>
        /// <returns>true为已经连接, false为尚未连接</returns>
        bool Stauts();

    }

    /// <summary>
    /// 标识当前弹幕源是支持发送弹幕的
    /// </summary>
    public interface ISendable
    {
        void Send(string str);
        void Login(string user, string password);
        bool LoginStauts();
    }

}
