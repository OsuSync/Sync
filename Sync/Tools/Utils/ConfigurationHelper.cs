using Sync.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.Utils
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// dymatic to get config value from other plugin by reflection.
        /// </summary>
        /// <param name="main_section_name">default as plugin name</param>
        /// <param name="sub_section_name">default as IConfigurable name</param>
        /// <param name="config_name"></param>
        /// <returns></returns>
        public static bool TryGetConfigurationElement(string main_section_name,string sub_section_name, string config_name,out ConfigurationElement result)
        {
            foreach(var manager in PluginConfigurationManager.ConfigurationSet)
            {
                if (main_section_name == manager.name)
                {
                    foreach (var plugin_config in manager.items)
                    {
                        var config_instance = plugin_config.config;
                        //sub section name
                        var name = config_instance.GetType().Name;

                        if (name==sub_section_name)
                        {
                            var config_type = config_instance.GetType();

                            foreach (var prop in config_type.GetProperties())
                            {
                                if (prop.Name == config_name)
                                {
                                    //copy
                                    result = prop.GetValue(config_instance).ToString();
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            result = null;
            return false;
        }
    }
}
