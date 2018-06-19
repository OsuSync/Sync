using Sync.Plugins;
using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;
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

        public bool ToBool() => _cfg.ToLower() == "true";
        public int ToInt() => int.Parse(_cfg);
        public float ToFloat() => float.Parse(_cfg);
    }

    /// <summary>
    /// Plugins configuration service, create instance to get configuration service
    /// </summary>
    internal sealed class PluginConfiuration
    {
        private string name;
        private Plugin instance;
        private IConfigurable config;

        public PluginConfiuration(Plugin instance, IConfigurable config):this(instance.Name, config)
        {
            this.instance = instance;
        }

        internal PluginConfiuration(string name, IConfigurable config)
        {
            this.name = name;
            this.config = config;
            ForceLoad();
        }

        ~PluginConfiuration()
        {
            ForceSave();
        }
        
        internal void Load()
        {
            var configType = config.GetType();
            var holderAttribute = configType.GetCustomAttribute<ConfigurationHolderAttribute>();

            foreach (PropertyInfo item in configType.GetProperties())
            {
                if (item.PropertyType == typeof(ConfigurationElement))
                {
                    ConfigurationElement element = ConfigurationIO.Read(item.Name, name + "." + config.GetType().Name/*,item.GetValue(config).ToString()*/);

                    if (!string.IsNullOrWhiteSpace(element))
                    {
                        if (CheckValueVaild(item, element, holderAttribute))
                        {
                            item.SetValue(config, element);
                        }
                    }
                    else
                    {
                        //if not exist,write to config.ini immediately
                        ConfigurationIO.Write(item.Name, (ConfigurationElement)item.GetValue(config), name + "." + config.GetType().Name);
                    }
                }
            }
        }

        internal void ForceLoad()
        {
            Load();
            config.onConfigurationLoad();
        }
        
        private bool CheckValueVaild(PropertyInfo info, ConfigurationElement element, ConfigurationHolderAttribute classHolder)
        {
            var configAttribute = info.GetCustomAttribute<BaseConfigurationAttribute>();
            bool noCheck = configAttribute?.NoCheck ?? classHolder?.NoCheck ?? true;

            if (configAttribute == null)
                return true;

            if (!noCheck)
                if (!configAttribute.Check(element))
                {
                    configAttribute.CheckFailedNotify(element);
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
                    ConfigurationIO.Write(item.Name, (ConfigurationElement)item.GetValue(config), name + "." + config.GetType().Name);
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
        private string name;

        public PluginConfigurationManager(Plugin plugin):this(plugin.Name)
        {
            instance = plugin;
        }

        internal PluginConfigurationManager(string name)
        {
            items = new List<PluginConfiuration>();
            this.name = name;
            ConfigurationSet.Add(this);
        }

        public void AddItem(IConfigurable Config)
        {
            items.Add(new PluginConfiuration(name, Config));
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
