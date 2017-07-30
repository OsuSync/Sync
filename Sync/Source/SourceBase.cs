using Sync.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sync.Source
{


    #region Old Source impl
    /// <summary>
    /// 礼物基类
    /// </summary>
    [Obsolete("Instead with SourceEvent classes", true)]
    public class CBaseGift
    {
        public string GiftName { get; set; }
        public uint GiftCount { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }
        public object Source { get; set; }
    }

    /// <summary>
    /// 弹幕基类
    /// </summary>
    [Obsolete("Instead with SourceEvent classes", true)]
    public class CBaseDanmuku
    {
        public string Danmuku { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }
        public object Source { get; set; }
    }
    /// <summary>
    /// 在源服务器连接成功时的委托
    /// </summary>
    [Obsolete("Program will not support this event, instead with StatusChangeEvt", true)]
    public delegate void ConnectedEvt();
    /// <summary>
    /// 在源服务器断开连接时的委托
    /// </summary>
    [Obsolete("Program will not support this event, instead with StatusChangeEvt", true)]
    public delegate void DisconnectedEvt();
    /// <summary>
    /// 接收到礼物信息的委托
    /// </summary>
    /// <param name="gift">礼物</param>
    [Obsolete("Program will not support this event, instead with SourceEventEvt", true)]
    public delegate void GiftEvt(CBaseGift gift);
    /// <summary>
    /// 收到弹幕信息时的委托
    /// </summary>
    /// <param name="danmuku">弹幕</param>
    [Obsolete("Program will not support this event, instead with SourceEventEvt", true)]
    public delegate void DanmukuEvt(CBaseDanmuku danmuku);
    /// <summary>
    /// 在源房间观看人数发生变化时的委托
    /// </summary>
    /// <param name="lCount">房间人数</param>
    [Obsolete("Program will not support this event, instead with SourceEventEvt", true)]
    public delegate void CurrentOnlineEvt(uint lCount);

    [Obsolete("Program will not support this interface, instead with SourceBase", true)]
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
    [Obsolete]
    public interface ISendable
    {
        void Send(string str);
        void Login(string user, string password);
        bool LoginStauts();
    }

    #endregion

    /// <summary>
    /// base class help program manager the source impl in program dispatch
    /// </summary>
    public abstract class SourceBase
    {

        public string Name { get; private set; }
        public string Author { get; private set; }
        public bool SupportSend { get; private set; }
        public SourceStatus Status { get; protected set; }

        public SourceBase(string Name, string Author, bool SupportSend = false)
        {
            this.Name = Name;
            this.Author = Author;
            this.SupportSend = SupportSend;
            this.Status = SourceStatus.IDLE;
        }

        /// <summary>
        /// Raise a event synchronized and dispatch it to handler asynchronized
        /// </summary>
        /// <typeparam name="T">Target Event Type</typeparam>
        /// <param name="args">Type args</param>
        protected void RaiseEvent<T>(SourceEventArgs<T> args) where T : SourceEvent
        {
            SourceEventDispatcher.Instance.SourceEventEvt<T>(args);
        }

        internal void connect()
        {
            this.Status = SourceStatus.USER_REQUEST_CONNECT;
            Connect();
        }

        internal void disconnect()
        {
            this.Status = SourceStatus.USER_REQUEST_DISCONNECT;
            Disconenct();
        }

        internal void send(string msg)
        {
            if (SupportSend) return;
            Send(msg);
        }

        public abstract void Connect();
        public abstract void Disconenct();
        public abstract void Send(string Message);

    }

    /// <summary>
    /// fire when Source event raised.
    /// </summary>
    /// <param name="args"></param>
    public delegate void SourceEventEvt<T>(SourceEventArgs<T> args) where T : SourceEvent;

    /// <summary>
    /// Source event base arg class
    /// Including event name and eventobject
    /// </summary>
    public class SourceEventArgs<T> where T : SourceEvent
    {
        private SourceEvent eventObj;
        public string Name { get; private set; }
        public T EventObject { get => (T)eventObj; }
        public SourceEventArgs(T EventObject)
        {
            Name = EventName;
            eventObj = EventObject;
        }

        public T CastTo()
        {
            return (T)EventObject;
        }
    }

    /// <summary>
    /// Source network impossible status
    /// </summary>
    public enum SourceStatus
    {
        /// <summary>
        /// Source working good, still working
        /// </summary>
        CONNECTED_WORKING,
        /// <summary>
        /// Source working good, but waiting for remote server
        /// </summary>
        CONNECTED_WAITING,
        /// <summary>
        /// Still establish connection to target server
        /// </summary>
        CONNECTING,       
        /// <summary>
        /// disconnect by remote
        /// </summary>
        REMOTE_DISCONNECTED,
        /// <summary>
        /// disconnect by user or network drop
        /// </summary>
        USER_DISCONNECTED,
        /// <summary>
        /// no any connection action
        /// </summary>
        IDLE,
        /// <summary>
        /// user request connect and waiting for Source class response.
        /// </summary>
        USER_REQUEST_CONNECT,
        /// <summary>
        /// user request disconnect, and waiting for Source class response.
        /// </summary>
        USER_REQUEST_DISCONNECT,///用户请求断开连接
    }

}
