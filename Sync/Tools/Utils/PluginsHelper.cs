using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.Utils
{
    public static class PluginsHelper
    {
        public static bool TryGetPlugin<T>(out T plugin) where T : Plugin
        {
            plugin = SyncHost.Instance.Plugins.GetPlugins().FirstOrDefault(p => p is T) as T;
            return plugin != null;
        }
    }
}
