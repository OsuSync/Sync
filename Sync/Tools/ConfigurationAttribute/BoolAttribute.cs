using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigurationAttribute
{
    public class BoolAttribute : BaseConfigurationAttribute
    {
        public override bool Check(string value) => bool.TryParse(value,out _);
    }
}
