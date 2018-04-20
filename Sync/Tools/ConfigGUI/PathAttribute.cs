using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    public class PathAttribute : BaseConfigurationAttribute
    {
        /// <summary>
        /// 是否钦定这路径是否必须存在,通常用于读取配置文件
        /// </summary>
        public bool RequireExist { get; set; } = false;

        public bool IsDirectory { get; set; } = true;

        public override bool Check(string file_path)
        {
            if (RequireExist && (!(IsDirectory ? File.Exists(file_path) : Directory.Exists(file_path))))
            {
                CheckFailedNotify(file_path);
                return false;
            }

            return true;
        }
    }
}
