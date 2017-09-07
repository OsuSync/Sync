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

        protected SyncHost getHoster()
        {
            return SyncHost.Instance;
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

        public virtual void OnDisable()
        {

        }

        public virtual void OnEnable()
        {

        }
    }
}
