using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    public interface IConfigurable
    {
        /// <summary>
        /// Invoke when load configuration
        /// </summary>
        void onConfigurationLoad();
        /// <summary>
        /// Invoke when save configuration
        /// </summary>
        void onConfigurationSave();
        /// <summary>
        /// Detected change form files and reload values
        /// </summary>
        void onConfigurationReload();
    }
}
