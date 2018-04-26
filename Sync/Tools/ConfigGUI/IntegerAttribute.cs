using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    public class IntegerAttribute : BaseConfigurationAttribute
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
}
