using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    [System.AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigurationHolderAttribute:Attribute
    {
        public bool Hide { get; set; } = false;
        public bool NoCheck { get; set; } = false;
    }
}
