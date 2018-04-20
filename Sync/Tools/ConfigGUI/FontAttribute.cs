using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    public class FontAttribute : BaseConfigurationAttribute
    {
        public override bool Check(string value)
        {
            return true;
        }
    }
}
