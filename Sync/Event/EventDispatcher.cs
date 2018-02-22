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

    public abstract class BaseEventDispatcher<T> where T : IBaseEvent
    {
        /// <summary>
        /// private event for call
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="insance"></param>
        private void raiseEventAsync<Event>(Event insance) where Event : T
        {
            EventDispatcher.Instance.RaiseEventAsync(this.GetType(), insance);
        }

        private void raiseEvent<Event>(Event insance) where Event : T
        {
            EventDispatcher.Instance.RaiseEvent(this.GetType(), insance);
        }

        /// <summary>
        /// public virtual for sub-classes override
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="event"></param>
        public virtual void RaiseEventAsync<Event>(Event @event) where Event : T
        {
            raiseEventAsync(@event);        
        }

        /// <summary>
        /// public virtual for sub-classes override
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="event"></param>
        public virtual void RaiseEvent<Event>(Event @event) where Event : T
        {
            raiseEvent(@event);
        }

        /// <summary>
        /// regist class for event bind
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        /// <param name="handler"></param>
        public void BindEvent<Event>(EventHandlerFunc<Event> handler) where Event : T
        {
            EventDispatcher.Instance.RegisterEventHandler(GetType(), handler);
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
        /// Register new event type[event dispatcher]
        /// </summary>
        /// <typeparam name="EventDispatcher">New dispatcher classes</typeparam>
        /// <returns></returns>
        public void RegisterNewDispatcher<EventDispatcher, TEvent>() where EventDispatcher : BaseEventDispatcher<TEvent> where TEvent : IBaseEvent
        {
            RegisterNewDispatcher(typeof(EventDispatcher));
        }

        /// <summary>
        /// Register new event type
        /// </summary>
        /// <param name="t"></param>
        public void RegisterNewDispatcher(Type t)
        { 
            if (dispatchers.ContainsKey(t)) return;
            else dispatchers.Add(t, new Dispatcher());
            return;
        }

        /// <summary>
        /// Get all binder of this event
        /// </summary>
        /// <typeparam name="Event">Target event</typeparam>
        /// <param name="eventType">Event dispatcher</param>
        /// <returns></returns>
        public HandlerList GetHandlerList<Event>(Type eventType)
        {
            return GetHandlerList(eventType, typeof(Event));
        }

        /// <summary>
        /// Get all binder of this event
        /// </summary>
        /// <param name="eventType">Event dispatcher</param>
        /// <param name="event">Target event</param>
        /// <returns></returns>
        public HandlerList GetHandlerList(Type eventType, Type @event)
        {
            return GetDispatcher(eventType)[@event];
        }

        /// <summary>
        /// Get dispathcer by type
        /// </summary>
        /// <param name="eventType">type</param>
        /// <returns></returns>
        public Dispatcher GetDispatcher(Type eventType)
        {
            return (dispatchers[eventType]);
        }

        /// <summary>
        /// Get dispathcer by T
        /// </summary>
        /// <typeparam name="EventType">Type</typeparam>
        /// <returns></returns>
        public Dispatcher GetDispatcher<EventType>()
        {
            return (dispatchers[typeof(EventType)]);
        }

        /// <summary>
        /// Return a dispathcer is or not exist
        /// </summary>
        /// <typeparam name="EventType"></typeparam>
        /// <returns></returns>
        public bool ExistDispatcher<EventType>()
        {
            return dispatchers.ContainsKey(typeof(EventType));
        }

        /// <summary>
        /// Return a dispathcer is or not exit
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool ExistDispatcher(Type eventType)
        {
            return dispatchers.ContainsKey(eventType);
        }

        /// <summary>
        /// Fire event with async call
        /// </summary>
        /// <typeparam name="EventType">Dispathcer</typeparam>
        /// <typeparam name="Event">Event</typeparam>
        /// <param name="event">Event instance</param>
        internal void RaiseEventAsync<EventType, Event>(Event @event)  where Event : IBaseEvent
        {
            RaiseEventAsync(typeof(EventType), @event);
        }

        /// <summary>
        /// Fire event with async call
        /// </summary>
        /// <typeparam name="Event">Event</typeparam>
        /// <param name="eventType">dispatcher</param>
        /// <param name="event">event instance</param>
        internal void RaiseEventAsync<Event>(Type eventType, Event @event) where Event : IBaseEvent
        {
            if (!GetDispatcher(eventType).ContainsKey(typeof(Event))) return;
            foreach (var item in GetDispatcher(eventType)[typeof(Event)])
            {
                var p = ((EventHandlerFunc<Event>)(item));
                Task.Run(() => p((@event)));
            }
            
        }

        /// <summary>
        /// Fire event with sync call
        /// </summary>
        /// <typeparam name="EventType">Event dispathcer</typeparam>
        /// <typeparam name="Event">Event</typeparam>
        /// <param name="event">Event instance</param>
        internal void RaiseEvent<EventType, Event>(Event @event) where Event : IBaseEvent
        {
            RaiseEvent(typeof(EventType), @event);
        }

        /// <summary>
        /// Fire event with sync call
        /// </summary>
        /// <typeparam name="Event">Event</typeparam>
        /// <param name="eventType">dispatcher</param>
        /// <param name="event">event instance</param>
        internal void RaiseEvent<Event>(Type eventType, Event @event) where Event : IBaseEvent
        {
            Type typo = typeof(Event);
            if (!GetDispatcher(eventType).ContainsKey(typo)) return;
            foreach (var item in GetDispatcher(eventType)[typo])
            {
                ((EventHandlerFunc<Event>)(item))(@event);
            }
        }

        /// <summary>
        /// Register event handler
        /// </summary>
        /// <typeparam name="Event">Target event</typeparam>
        /// <param name="eventType">Dispatcher</param>
        /// <param name="handler">handler</param>
        /// <returns></returns>
        public bool RegisterEventHandler<Event>(Type eventType, EventHandlerFunc<Event> handler) where Event : IBaseEvent
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

        /// <summary>
        /// Register event handle
        /// </summary>
        /// <typeparam name="EventType">Event dispathcer</typeparam>
        /// <typeparam name="Event">Target Event</typeparam>
        /// <param name="handler">Handler</param>
        /// <returns></returns>
        public bool RegisterEventHandler<EventType, Event>(EventHandlerFunc<Event> handler) where Event : IBaseEvent
        {
            return RegisterEventHandler(typeof(EventType), handler);
        }

        /// <summary>
        /// Remove event handle
        /// </summary>
        /// <typeparam name="EventType">Event dispathcer</typeparam>
        /// <typeparam name="Event">Target Event</typeparam>
        /// <param name="handler">Handler</param>
        public void RemoveEventHandler<EventType, Event>(EventHandlerFunc<Event> handler) where Event : IBaseEvent
        {
            if (dispatchers.TryGetValue(typeof(EventType),out Dispatcher dispatcher))
            {
                var list = dispatcher[typeof(Event)];
                list.Remove(handler);
            }
        }
    }
}
