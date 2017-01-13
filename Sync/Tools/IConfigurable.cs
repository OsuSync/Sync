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
        /// 在系统读取配置文件时调用
        /// </summary>
        void onConfigurationLoad();
        /// <summary>
        /// 在系统保存配置文件时调用
        /// </summary>
        void onConfigurationSave();
    }
}
