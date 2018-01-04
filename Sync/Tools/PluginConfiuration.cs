using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    public sealed class ConfigurationElement
    {
        private string _cfg = string.Empty;
        public ConfigurationElement()
        {

        }

        public ConfigurationElement(string def)
        {
            _cfg = def;
        }

        public static implicit operator string(ConfigurationElement e)
        {
            return e._cfg;
        }

        public static implicit operator ConfigurationElement(string e)
        {
            return new ConfigurationElement(e);
        }

        public override string ToString()
        {
            return _cfg;
        }
    }

    /// <summary>
    /// Plugins configuration service, create instance to get configuration service
    /// </summary>
    public sealed class PluginConfiuration
    {
        private Plugin instance;
        private IConfigurable config;
        public PluginConfiuration(Plugin instance, IConfigurable config)
        {
            this.instance = instance;
            this.config = config;
            ForceLoad();
        }

        ~PluginConfiuration()
        {
            ForceSave();
        }

        public void ForceLoad()
        {
            foreach (PropertyInfo item in config.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(ConfigurationElement))
                {
                    ConfigurationElement element = ConfigurationIO.Read(item.Name, instance.Name + "." + config.GetType().Name/*,item.GetValue(config).ToString()*/);

                    if (!string.IsNullOrWhiteSpace(element))
                    {
                        item.SetValue(config, element);
                    }
                }
            }
            config.onConfigurationLoad();
        }

        public void ForceSave()
        {
            config.onConfigurationSave();
            foreach (PropertyInfo item in config.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(ConfigurationElement))
                {
                    ConfigurationIO.Write(item.Name, (ConfigurationElement)item.GetValue(config), instance.Name + "." + config.GetType().Name);
                }
            }
        }
    }

    /// <summary>
    /// Configuration Manager
    /// </summary>
    public sealed class PluginConfigurationManager
    {
        internal static LinkedList<PluginConfigurationManager> ConfigurationSet = new LinkedList<PluginConfigurationManager>();
        private List<PluginConfiuration> items;
        private Plugin instance;
        public PluginConfigurationManager(Plugin instance)
        {
            items = new List<PluginConfiuration>();
            this.instance = instance;
            ConfigurationSet.AddLast(this);
        }

        public void AddItem(IConfigurable Config)
        {
            items.Add(new PluginConfiuration(instance, Config));
        }

        public void SaveAll()
        {
            foreach (var item in items)
            {
                item.ForceSave();
            }
        }

        public PluginConfiuration GetInstance(IConfigurable obj)
        {
            foreach (var item in items)
            {
                if (item.GetType() == obj.GetType()) return item;
            }
            return null;
        }

        ~PluginConfigurationManager () => SaveAll();
    }
}
