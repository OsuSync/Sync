using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    [System.AttributeUsage(System.AttributeTargets.Property,AllowMultiple = false)]
    public abstract class ConfigAttributeBase:Attribute
    {
        public string Description { get; set; }
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
            return (MinValue <= i && i <= MaxValue);
        }
    }

    public class ConfigStringAttribute : ConfigAttributeBase { }

    public class ConfigListAttribute : ConfigAttributeBase
    {
        public string[] ValueList { get; set; }

        public bool Check(string val)
        {
            if (ValueList?.Length == 0)
                return false;
            return ValueList.Where((str) => str == val).Count() != 0;
        }
    }
}
