using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
            return e?._cfg??string.Empty;
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
    internal sealed class PluginConfiuration
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
        
        internal void Load()
        {
            foreach (PropertyInfo item in config.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(ConfigurationElement))
                {
                    ConfigurationElement element = ConfigurationIO.Read(item.Name, instance.Name + "." + config.GetType().Name/*,item.GetValue(config).ToString()*/);

                    if (!string.IsNullOrWhiteSpace(element))
                    {
                        if (CheckValueVaild(item, element))
                        {
                            item.SetValue(config, element);
                        }
                    }
                    else
                    {
                        //if not exsit,write to config.ini immediately
                        ConfigurationIO.Write(item.Name, (ConfigurationElement)item.GetValue(config), instance.Name + "." + config.GetType().Name);
                    }
                }
            }
        }

        internal void ForceLoad()
        {
            Load();
            config.onConfigurationLoad();
        }
        
        private bool CheckValueVaild(PropertyInfo info, ConfigurationElement element)
        {
            var config_attribute = Attribute.GetCustomAttribute(info, typeof(ConfigGUI.BaseConfigurationAttribute)) as ConfigGUI.BaseConfigurationAttribute;

            if (config_attribute == null)
                return true;

            if (!config_attribute.NoCheck)
                if (!config_attribute.Check(element))
                {
                    config_attribute.CheckFailedNotify(element);
                    return false;
                }

            return true;
        }

        internal void ForceReload()
        {
            Load();
            config.onConfigurationReload();
        }


        internal void ForceSave()
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
        internal static ConcurrentBag<PluginConfigurationManager> ConfigurationSet = new ConcurrentBag<PluginConfigurationManager>();
        internal static bool InSaving = false;
        private List<PluginConfiuration> items;
        private Plugin instance;
        public PluginConfigurationManager(Plugin instance)
        {
            items = new List<PluginConfiuration>();
            this.instance = instance;
            ConfigurationSet.Add(this);
        }

        public void AddItem(IConfigurable Config)
        {
            items.Add(new PluginConfiuration(instance, Config));
        }

        internal void ReloadAll()
        {
            foreach (var item in items)
            {
                item.ForceReload();
            }
        }

        public void SaveAll()
        {
            InSaving = true;
            foreach (var item in items)
            {
                item.ForceSave();
            }
            InSaving = false;
        }

        //public PluginConfiuration GetInstance(IConfigurable obj)
        //{
        //    foreach (var item in items)
        //    {
        //        if (item.GetType() == obj.GetType()) return item;
        //    }
        //    return null;
        //}

        ~PluginConfigurationManager () => SaveAll();
    }
}
