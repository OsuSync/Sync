using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    public class FloatAttribute : BaseConfigurationAttribute
    {
        public float MinValue { get; set; } = float.MinValue;
        public float MaxValue { get; set; } = float.MaxValue;
        public float Step { get; set; } = 1;

        public override bool Check(string o)
        {
            if (!float.TryParse(o, out float v))
                return false;
            return (MinValue <= v && v <= MaxValue);
        }
    }
}
