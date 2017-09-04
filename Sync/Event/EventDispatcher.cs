using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sync.Plugins
{
    /// <summary>
    /// Base event class
    /// </summary>
    public interface IBaseEvent
    {
    }

    public class EventDispatcherTaskScheduler : TaskScheduler
    {
        private LinkedList<Task> tasks = new LinkedList<Task>();
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return tasks;
        }

        protected override void QueueTask(Task task)
        {
            var thread = new Thread(() => { TryExecuteTask(task); });
            thread.Start();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }
    }

    public abstract class BaseEventDispatcher
    {
        /// <summary>
        /// private event for call
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="insance"></param>
        private void raiseEventAsync<Event>(Event insance) where Event : IBaseEvent
        {
            EventDispatcher.Instance.RaiseEventAsync(this.GetType(), insance);
        }

        private void raiseEvent<Event>(Event insance) where Event : IBaseEvent
        {
            EventDispatcher.Instance.RaiseEvent(this.GetType(), insance);
        }

        /// <summary>
        /// public virtual for sub-classes override
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="event"></param>
        public virtual void RaiseEventAsync<Event>(Event @event) where Event : IBaseEvent
        {
            raiseEventAsync(@event);        
        }

        /// <summary>
        /// public virtual for sub-classes override
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="event"></param>
        public virtual void RaiseEvent<Event>(Event @event) where Event : IBaseEvent
        {
            raiseEvent(@event);
        }

        /// <summary>
        /// regist class for event bind
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="handler"></param>
        public void BindEvent<Event>(EventHandlerFunc<Event> handler) where Event : IBaseEvent
        {
            EventDispatcher.Instance.RegistEventHandler(GetType(), handler);
        }
    }

    /// <summary>
    /// the generic handler impl
    /// </summary>
    /// <typeparam name="T">which typeof Event should handle</typeparam>
    /// <param name="event">the event fired you target handler</param>
    /// <returns></returns>
    public delegate void EventHandlerFunc<Event>(Event @event) where Event : IBaseEvent;

    public class HandlerList : LinkedList<object>
    {

    }

    /// <summary>
    /// typedef for LinkedList
    /// </summary>
    public class Dispatcher : Dictionary<Type, HandlerList>
    {
        
    }

    /// <summary>
    /// [Singleton] Global event dispatcher
    /// </summary>
    public class EventDispatcher
    {
        private Dictionary<Type, Dispatcher> dispatchers = new Dictionary<Type, Dispatcher>();
        private TaskScheduler tasks = new EventDispatcherTaskScheduler();

        private EventDispatcher()
        {

        }

        public static readonly EventDispatcher Instance = new EventDispatcher();

        /// <summary>
        /// Regist new event type[event dispatcher]
        /// </summary>
        /// <typeparam name="EventDispatcher">New dispatcher classes</typeparam>
        /// <returns></returns>
        public void RegistNewDispatcher<EventDispatcher>() where EventDispatcher : BaseEventDispatcher
        {
            RegistNewDispatcher(typeof(EventDispatcher));
        }

        /// <summary>
        /// Regist new event type
        /// </summary>
        /// <param name="t"></param>
        public void RegistNewDispatcher(Type t)
        { 
            if (dispatchers.ContainsKey(t)) return;
            else dispatchers.Add(t, new Dispatcher());
            return;
        }

        public HandlerList GetHandlerList<Event>(Type eventType)
        {
            return GetHandlerList(eventType, typeof(Event));
        }

        public HandlerList GetHandlerList(Type eventType, Type @event)
        {
            return GetDispatcher(eventType)[@event];
        }

        public Dispatcher GetDispatcher(Type eventType)
        {
            return (dispatchers[eventType]);
        }

        public Dispatcher GetDispatcher<EventType>() where EventType : BaseEventDispatcher
        {
            return (dispatchers[typeof(EventType)]);
        }

        public bool ExistDispatcher<EventType>() where EventType : BaseEventDispatcher
        {
            return dispatchers.ContainsKey(typeof(EventType));
        }

        public bool ExistDispatcher(Type eventType)
        {
            return dispatchers.ContainsKey(eventType);
        }

        internal void RaiseEventAsync<EventType, Event>(Event @event) where EventType : BaseEventDispatcher where Event : IBaseEvent
        {
            RaiseEventAsync(typeof(EventType), @event);
        }

        internal void RaiseEventAsync<Event>(Type eventType, Event @event) where Event : IBaseEvent
        {
            if (!GetDispatcher(eventType).ContainsKey(typeof(Event))) return;
            foreach (var item in GetDispatcher(eventType)[typeof(Event)])
            {
                var p = ((EventHandlerFunc<Event>)(item));
                Task.Run(() => p((@event)));
            }
            
        }

        internal void RaiseEvent<EventType, Event>(Event @event) where EventType : BaseEventDispatcher where Event : IBaseEvent
        {
            RaiseEvent(typeof(EventType), @event);
        }

        internal void RaiseEvent<Event>(Type eventType, Event @event) where Event : IBaseEvent
        {
            Type typo = typeof(Event);
            if (!GetDispatcher(eventType).ContainsKey(typo)) return;
            foreach (var item in GetDispatcher(eventType)[typo])
            {
                ((EventHandlerFunc<Event>)(item))(@event);
            }
        }

        public bool RegistEventHandler<Event>(Type eventType, EventHandlerFunc<Event> handler) where Event : IBaseEvent
        {
            Type typo = typeof(Event);
            Dispatcher dispatcher = null;
            if (ExistDispatcher(eventType))
            {
                dispatcher = GetDispatcher(eventType);
            }
            else
            {
                throw new Exception("Dispatcher not register!");
            }
            
            if(!dispatcher.ContainsKey(typo))
            {
                dispatcher.Add(typo, new HandlerList());
            }
            if (dispatcher[typo].Contains(handler)) return false;
            dispatcher[typo].AddLast(handler);
            return true;
        }

        public bool RegistEventHandler<EventType, Event>(EventHandlerFunc<Event> handler) where EventType : BaseEventDispatcher where Event : IBaseEvent
        {
            return RegistEventHandler(typeof(EventType), handler);
        }
    }
}
