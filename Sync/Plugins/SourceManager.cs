using Sync.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Plugins
{
    /// <summary>
    /// the generic handler impl
    /// </summary>
    /// <typeparam name="T">which typeof Event should handle</typeparam>
    /// <param name="event">the event fired you target handler</param>
    /// <returns></returns>
    public delegate Task HandlerFunc<T>(T @event) where T : SourceEvent;

    /// <summary>
    /// event dispatcher
    /// </summary>
    public sealed class SourceEventDispatcher
    {
        private LinkedList<SourceBase> allHandlers;
        private Dictionary<Type, LinkedList<object>> typedHandlers;

        private SourceEventDispatcher()
        {
            allHandlers = new LinkedList<SourceBase>();
            typedHandlers = new Dictionary<Type, LinkedList<object>>();
        }
        /// <summary>
        /// Singleton
        /// </summary>
        public static readonly SourceEventDispatcher Instance = new SourceEventDispatcher();

        private bool RegisterEventHandler(Type handleType, object handler)
        {
            var handlers = typedHandlers[handleType];
            if (allHandlers.Contains(handler)) return false;
            else
            {
                handlers.AddLast(handler);
                allHandlers.AddLast((SourceBase)handler);
            }
            return true;
        }

        private bool UnregisterEventHandler(Type handleType, object handler)
        {
            var handlers = typedHandlers[handleType];
            if (!allHandlers.Contains(handler)) return false;
            else
            {
                handlers.Remove(handler);
                allHandlers.Remove((SourceBase)handler);
            }
            return true;
        }

        /// <summary>
        /// Register a handler with a generic type
        /// </summary>
        /// <typeparam name="T">Target Event Type</typeparam>
        /// <param name="handler">Handler instance</param>
        /// <returns><code>True</code> for Success</returns>
        internal bool RegisterEventHandler<T>(HandlerFunc<T> handler) where T : SourceEvent
        {
            return RegisterEventHandler(typeof(T), handler);
        }

        /// <summary>
        /// Unregister a handler with a generic type
        /// </summary>
        /// <typeparam name="T">Target Event Type</typeparam>
        /// <param name="handler">Handler instance</param>
        /// <returns><code>True</code> for Success</returns>
        internal bool UnregisterEventHandler<T>(HandlerFunc<T> handler) where T : SourceEvent
        {
            return UnregisterEventHandler(typeof(T), handler);
        }

        /// <summary>
        /// Raise a generic type Event (In general only call for SourceBase class)
        /// </summary>
        /// <typeparam name="T">Target Event Type</typeparam>
        /// <param name="args">event args</param>
        internal void SourceEventEvt<T>(SourceEventArgs<T> args) where T : SourceEvent
        {
            FireEvent<T>((T)args.EventObject);
        }

        /// <summary>
        /// Directly fire a event with a direct SourceEvent object
        /// </summary>
        /// <typeparam name="T">Target Event Type</typeparam>
        /// <param name="event">event</param>
        internal void FireEvent<T>(T @event) where T : SourceEvent
        {
            foreach (var item in typedHandlers[@event.GetType()])
            {
                ((HandlerFunc<T>)item)(@event).Start();
            }
        }

    }

    public class SourceManager
    {
        List<SourceBase> listSources;
        public SourceManager()
        {
            listSources = new List<SourceBase>();
        }

        public IEnumerable<SourceBase> SourceList
        {
            get
            {
                return listSources;
            }
        }

        public bool AddSource(SourceBase src)
        {
            if(listSources.Exists(p => p.Name == src.Name && p.Author == p.Author))
            {
                return false;
            }
            else
            {
                listSources.Add(src);
            }
            return true;
        }
    }
}
