using Sync.MessageFilter;
using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sync.Source
{

    /// <summary>
    /// Flag this source is support send
    /// </summary>
    public abstract class SendableSource : SourceBase
    {
        public bool SendStatus { get; protected set; } = false;
        public SendableSource(string Name, string Author) : base(Name, Author)
        {
        }

        internal void send(IMessageBase message)
        {
            if (!SendStatus)
            {
                IO.CurrentIO.WriteColor(DefaultI18n.LANG_SendNotReady, ConsoleColor.Red);
                return;
            }
            Send(message);
        }



        internal void login(string user, string password)
        {
            Login(user, password);
        }

        public abstract void Login(string user, string password);
        public abstract void Send(IMessageBase message);
    }

    /// <summary>
    /// base class help program manager the source impl in program dispatch
    /// </summary>
    public abstract class SourceBase
    {

        public string Name { get; private set; }
        public string Author { get; private set; }
        public string LiveID { get; set; } = "";
        public BaseEventDispatcher<ISourceEvent> EventBus { get => SourceEvents.Instance; }
        public SourceStatus Status { get; protected set; }
        

        public SourceBase(string Name, string Author)
        {
            this.Name = Name;
            this.Author = Author;
            this.Status = SourceStatus.IDLE;
        }

        /// <summary>
        /// Raise a event synchronized and dispatch it to handler asynchronized
        /// </summary>
        /// <typeparam name="T">Target Event Type</typeparam>
        /// <param name="args">Type args</param>
        protected void RaiseEvent<T>(T args) where T : ISourceEvent
        {
            EventBus.RaiseEvent(args);
        }

        internal void connect()
        {
            this.Status = SourceStatus.USER_REQUEST_CONNECT;
            Connect();
        }

        internal void disconnect()
        {
            this.Status = SourceStatus.USER_REQUEST_DISCONNECT;
            Disconnect();
        }

        public abstract void Connect();
        public abstract void Disconnect();

    }

    ///// <summary>
    ///// fire when Source event raised.
    ///// </summary>
    ///// <param name="args"></param>
    ////public delegate void SourceEventEvt<T>(T args) where T : SourceEvent;

    ///// <summary>
    ///// Source event base arg class
    ///// Including event name and eventobject
    ///// </summary>
    //public class SourceEventArgs<T> where T : SourceEvent
    //{
    //    private SourceEvent eventObj;
    //    public string Name { get; private set; }
    //    public T EventObject { get => (T)eventObj; }
    //    public SourceEventArgs(T EventObject)
    //    {
    //        eventObj = EventObject;
    //    }

    //    public T CastTo()
    //    {
    //        return (T)EventObject;
    //    }
    //}

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
        USER_REQUEST_DISCONNECT,
    }

}
