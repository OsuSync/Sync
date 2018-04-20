using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    public class StringAttribute : BaseConfigurationAttribute
    {
        public override bool Check(string value) => true;
    }
}
