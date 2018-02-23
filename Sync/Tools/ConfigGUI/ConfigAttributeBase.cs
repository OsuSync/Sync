using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    [System.AttributeUsage(System.AttributeTargets.Property,AllowMultiple = false)]
    public abstract class ConfigAttributeBase:Attribute
    {
        public string Description { get; set; } = "No Description";

        public virtual string CheckFailedFormatMessage { get; set; } = "Parse error:{0}";
        protected void CheckFailedNotify(object obj) => IO.CurrentIO.WriteColor($"[Config]{string.Format(CheckFailedFormatMessage,obj.ToString())}",ConsoleColor.Red);
    }

    public class ConfigBoolAttribute : ConfigAttributeBase
    {
    }

    public class ConfigIntegerAttribute : ConfigAttributeBase
    {
        public int MinValue { get; set; } = int.MinValue;
        public int MaxValue { get; set; } = int.MaxValue;

        public bool Check(int i)
        {
            if (MinValue <= i && i <= MaxValue)
                return true;

            CheckFailedNotify(i);
            return false;
        }
    }

    public class ConfigFloatAttribute : ConfigAttributeBase
    {
        public float MinValue { get; set; } = float.MinValue;
        public float MaxValue { get; set; } = float.MaxValue;

        public bool Check(float i)
        {
            return (MinValue <= i && i <= MaxValue);
        }
    }

    public class ConfigStringAttribute : ConfigAttributeBase { }

    public class ConfigListAttribute : ConfigAttributeBase
    {
        public string[] ValueList { get; set; } = null;
        public bool IgnoreCase { get; set; } = false;

        public bool Check(string val)
        {
            var m_val = IgnoreCase ? val.ToLower() : val;

            if ((ValueList?.Length != 0) && (ValueList.Where((str) => (IgnoreCase ? str.ToLower() : str) == m_val).Count() != 0))
                return true;
            CheckFailedNotify(val);
            return false;
        }
    }

    public class ConfigFilePathAttribute : ConfigAttributeBase
    {
        /// <summary>
        /// 是否钦定这路径是否必须存在,通常用于读取配置文件
        /// </summary>
        public bool MustExsit { get; set; } = false;

        public bool Check(string file_path)
        {
            if (MustExsit&&(!File.Exists(file_path)))
            {
                CheckFailedNotify(file_path);
                return false;
            }

            return true;
        }
    }
}
