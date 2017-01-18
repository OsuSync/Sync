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
    public class ConfigurationElement
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
    public class PluginConfiuration<T, U> where T : Plugin where U : IConfigurable
    {
        private T parentPlugin;
        private U configInstance;

        public PluginConfiuration(T plugin, U config)
        {
            parentPlugin = plugin;
            configInstance = config;
            ForceLoad();
        }

        ~PluginConfiuration()
        {
            ForceSave();
        }

        public void ForceLoad()
        {
            foreach (PropertyInfo item in configInstance.GetType().GetProperties())
            {
                if (item.GetType() == typeof(ConfigurationElement))
                {
                    item.SetValue(configInstance, ConfigurationIO.Read(item.Name, parentPlugin.Name));
                }
            }
        }

        public void ForceSave()
        {
            foreach (PropertyInfo item in configInstance.GetType().GetProperties())
            {
                if (item.GetType() == typeof(ConfigurationElement))
                {
                    ConfigurationIO.Write(item.Name, (string)item.GetValue(configInstance), parentPlugin.Name);
                }
            }
        }
    }
}
