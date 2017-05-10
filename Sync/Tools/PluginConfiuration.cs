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
    }

    /// <summary>
    /// 配置文件类，实例化时传入插件类实例和继承配置文件的实例的类即可享受配置文件服务。
    /// </summary>
    /// <typeparam name="T">插件类</typeparam>
    /// <typeparam name="U">配置文件类</typeparam>
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
            foreach (PropertyInfo item in instance.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(ConfigurationElement))
                {
                    item.SetValue(config, (ConfigurationElement)ConfigurationIO.Read(item.Name, instance.Name + "." + config.GetType().Name));
                }
            }
        }

        public void ForceSave()
        {
            foreach (PropertyInfo item in config.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(ConfigurationElement))
                {
                    ConfigurationIO.Write(item.Name, (ConfigurationElement)item.GetValue(instance), instance.Name + "." + config.GetType().Name);
                }
            }
        }
    }

    public sealed class PluginConfigurationManager<T> where T : Plugin
    {
        private List<PluginConfiuration> items;
        private Plugin instance;
        public PluginConfigurationManager(Plugin instance)
        {
            items = new List<PluginConfiuration>();
            this.instance = instance;
        }

        public void AddItem(IConfigurable Config)
        {
            items.Add(new PluginConfiuration(instance, Config));
        }
    }
}
