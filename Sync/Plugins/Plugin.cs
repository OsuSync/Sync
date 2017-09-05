using Sync.Command;
using System;
using System.Reflection;

namespace Sync.Plugins
{

    public interface IPlugin
    {
        string getName();
        string getAuthor();
    }

    public abstract class Plugin
    {
        public readonly string Name;
        public readonly string Author;
        public BaseEventDispatcher EventBus { get => PluginEvents.Instance; }

        public Plugin(string Name, string Author)
        {
            this.Name = Name;
            this.Author = Author;
        }

        private bool isComplete = false;

        protected SyncHost getHoster()
        {
            if (isComplete)
                return SyncHost.Instance;
            else
                throw new NullReferenceException("Can't get Instance of the Hoster.");
        }

        public string getName()
        {
            return Name;
        }

        public string getAuthor()
        {
            return Author;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
