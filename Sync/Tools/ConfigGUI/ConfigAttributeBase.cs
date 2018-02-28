using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public abstract class ConfigAttributeBase : Attribute
    {
        public bool NeedRestart = false;
        public string Description { get; set; } = "No Description";

        public virtual string CheckFailedFormatMessage { get; set; } = "Parse error:{0}";
        public abstract bool Check(string value);
        public void CheckFailedNotify(object obj) => IO.CurrentIO.WriteColor($"[Config]{string.Format(CheckFailedFormatMessage, obj.ToString())}", ConsoleColor.Red);
    }

    public class ConfigBoolAttribute : ConfigAttributeBase
    {
        public override bool Check(string value) => true;
    }

    public class ConfigIntegerAttribute : ConfigAttributeBase
    {
        public int MinValue { get; set; } = int.MinValue;
        public int MaxValue { get; set; } = int.MaxValue;

        public override bool Check(string i)
        {
            if (!int.TryParse(i, out int v))
                return false;
            return (MinValue <= v && v <= MaxValue);
        }
    }

    public class ConfigFloatAttribute : ConfigAttributeBase
    {
        public float MinValue { get; set; } = float.MinValue;
        public float MaxValue { get; set; } = float.MaxValue;

        public override bool Check(string o)
        {
            if (!float.TryParse(o, out float v))
                return false;
            return (MinValue <= v && v <= MaxValue);
        }
    }

    public class ConfigStringAttribute : ConfigAttributeBase
    {
        public override bool Check(string value) => true;
    }

    public class ConfigListAttribute : ConfigAttributeBase
    {
        public string[] ValueList { get; set; } = new string[] { };

        public bool IgnoreCase { get; set; } = false;

        public bool AllowMultiSelect { get; set; } = false;
        public char SplitSeparator { get; set; } = ',';

        public override bool Check(string val)
        {
            var m_val = IgnoreCase ? val.ToLower() : val;

            if ((ValueList.Length != 0))
            {
                if (AllowMultiSelect)
                {
                    foreach (var str in m_val.Split(new[] { SplitSeparator }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!ContainValue(m_val))
                        {
                            CheckFailedNotify(val);
                            return false;
                        }
                    }
                }
                else
                {
                    if (!ContainValue(m_val))
                    {
                        CheckFailedNotify(val);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ContainValue(string content)
        {
            return ValueList.Where((str) => (IgnoreCase ? str.ToLower() : str) == content).Count() != 0;
        }
    }

    public class ConfigColorAttribute : ConfigAttributeBase
    {
        public byte R, G, B, A;

        //#RRGGBBAA
        public override bool Check(string rgba)
        {
            return rgba[0] == '#'
                && byte.TryParse(rgba.Substring(1, 2), NumberStyles.HexNumber, null, out var _)
                && byte.TryParse(rgba.Substring(3, 2), NumberStyles.HexNumber, null, out var _)
                && byte.TryParse(rgba.Substring(5, 2), NumberStyles.HexNumber, null, out var _)
                && byte.TryParse(rgba.Substring(7, 2), NumberStyles.HexNumber, null, out var _);
        }
    }

    public class ConfigPathAttribute : ConfigAttributeBase
    {
        /// <summary>
        /// 是否钦定这路径是否必须存在,通常用于读取配置文件
        /// </summary>
        public bool MustExsit { get; set; } = false;

        public bool IsFilePath { get; set; } = true;

        public override  bool Check(string file_path)
        {
            if (MustExsit && (!(IsFilePath ? File.Exists(file_path) : Directory.Exists(file_path))))
            {
                CheckFailedNotify(file_path);
                return false;
            }

            return true;
        }
    }
}
